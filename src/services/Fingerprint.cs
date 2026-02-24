using DPUruNet;
using FieldTeamEquipmentInventory.Helpers;
using FieldTeamEquipmentInventory.Interfaces;

namespace FieldTeamEquipmentInventory.Services;

public class Fingerprint : IBiometrics
{
    private const int REQUIRED_SAMPLES = 4;
    private const int THRESHOLD = 21474; // FAR 0.0001
    private Reader _reader;
    private CancellationTokenSource _cts;
    private bool _disposed = false;
    private readonly object _lock = new();

    public event Action<byte[]>? OnPreviewFrame;

    public Fingerprint()
    {
        
        var readers = ReaderCollection.GetReaders();

        if (readers.Count == 0)
            throw new Exception(Resources.GetString("FINGERPRINT_DEV_NOT_FOUND"));

        _reader = readers[0];
        var result = _reader.Open(Constants.CapturePriority.DP_PRIORITY_COOPERATIVE);

        if (result != Constants.ResultCode.DP_SUCCESS)
            throw new Exception(Resources.GetString("FINGERPRINT_DEV_FAIL_OPEN", result));
    }

    private Fmd ExtractFmd(Fid fid)
    {
        var extraction = FeatureExtraction.CreateFmdFromFid(
            fid,
            Constants.Formats.Fmd.ANSI
        );

        if (extraction.ResultCode != Constants.ResultCode.DP_SUCCESS)
            throw new Exception(Resources.GetString("FINGERPRINT_EXTRACTION_FAIL"));

        return extraction.Data;
    }

    // Capture single fingerprint template as byte[]
    public byte[] Capture()
    {
        lock (_lock)
        {
            var result = _reader.Capture(
                Constants.Formats.Fid.ANSI,
                Constants.CaptureProcessing.DP_IMG_PROC_DEFAULT,
                5000,
                0 // -1
            );

            if (result.ResultCode != Constants.ResultCode.DP_SUCCESS)
                throw new Exception(Resources.GetString("FINGERPRINT_CAPTURE_FAIL", result.ResultCode));

            if (result.Quality != Constants.CaptureQuality.DP_QUALITY_GOOD)
                throw new Exception(Resources.GetString("FINGERPRINT_QUALITY_FAIL", result.Quality));

            var fmd = ExtractFmd(result.Data);
            OnPreviewFrame?.Invoke(fmd.Bytes);
            return fmd.Bytes;
        }
    }

    public async Task<byte[]?> CaptureAsync(Action<string>? statusCallback, CancellationToken cancellationToken)
    {
        Console.WriteLine(_reader.Description.Name);
        Console.WriteLine(_reader.Description.SerialNumber);
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                statusCallback?.Invoke(Resources.GetString("FINGERPRINT_CAPTURE_WAIT"));

                var sample = Capture(); // same thread

                statusCallback?.Invoke(Resources.GetString("FINGERPRINT_CAPTURE_OK"));
                return sample;
            }
            catch (Exception ex)
            {
                statusCallback?.Invoke(ex.Message);
                try
                {
                    await Task.Delay(300, cancellationToken);
                }
                catch { }
            }

            await Task.Yield(); // keeps UI responsive
        }

        return null;
    }

    public bool Verify(byte[] stored, byte[] captured)
    {
        var fmdStored = new Fmd(stored, 0, Constants.WRAPPER_VERSION);
        var fmdCaptured = new Fmd(captured, 0, Constants.WRAPPER_VERSION);

        var compare = Comparison.Compare(fmdStored, 0, fmdCaptured, 0);

        if (compare.ResultCode != Constants.ResultCode.DP_SUCCESS)
            throw new Exception(Resources.GetString("FINGERPRINT_COMPARISON_FAIL"));

        return compare.Score < THRESHOLD;
    }

    public string ToBase64(byte[] template)
    {
        if (template == null)
            throw new ArgumentNullException(nameof(template));

        return Convert.ToBase64String(template);
    }

    public byte[] FromBase64(string base64)
    {
        if (string.IsNullOrWhiteSpace(base64))
            throw new ArgumentException(Resources.GetString("APPLICATION_ARGUMENT_MISS"));

        return Convert.FromBase64String(base64);
    }

    public byte[]? Enrollment()
    {
        var samples = new List<Fmd>();
        while (samples.Count < REQUIRED_SAMPLES)
        {
            try
            {
                var bytes = Capture();
                var fmd = new Fmd(bytes, 0, Constants.WRAPPER_VERSION);
                samples.Add(fmd);
            }
            catch { }
        }
        var result = DPUruNet.Enrollment.CreateEnrollmentFmd(
            Constants.Formats.Fmd.ANSI, samples);
        return result.Data?.Bytes;
    }

    public async Task<byte[]?> EnrollmentAsync(
        Action<string> statusCallback,
        CancellationToken externalToken)
    {
        _cts = CancellationTokenSource.CreateLinkedTokenSource(externalToken);
        var samples = new List<Fmd>();
        while (true)
        {
            if (_cts.Token.IsCancellationRequested) return null;
            try
            {
                var bytes = await CaptureAsync(statusCallback, _cts.Token);
                statusCallback(Resources.GetString("FINGERPRINT_CAPTURE_SAMPLE", samples.Count + 1, REQUIRED_SAMPLES));
                var fmd = new Fmd(bytes, 0, Constants.WRAPPER_VERSION);
                samples.Add(fmd);
            }
            catch (Exception error)
            {
                statusCallback(error.Message);
            }
            if (samples.Count >= REQUIRED_SAMPLES)
                break;
        }
        var result = DPUruNet.Enrollment.CreateEnrollmentFmd(
            Constants.Formats.Fmd.ANSI, samples);
        return result.Data?.Bytes;
    }

    public void Cancel()
    {
        try { _reader?.CancelCapture(); }
        catch { }
        _cts?.Cancel();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // Dispose managed resources here.
                _reader?.Dispose();
                _cts?.Dispose();
            }
            // Dispose unmanaged resources here.
            _disposed = true;
        }
    }
    ~Fingerprint()
    {
        // Finalizer (optional)
        Dispose(false);
    }
}