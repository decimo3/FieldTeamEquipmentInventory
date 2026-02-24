using FieldTeamEquipmentInventory.Helpers;
using FieldTeamEquipmentInventory.Interfaces;
using OpenCvSharp;
using OpenCvSharp.Face;
using System.Windows.Media.Media3D;

namespace FieldTeamEquipmentInventory.Services;

public class FacialRecognition : IBiometrics
{
    private const int THRESHOLD = 60;
    private const int FACE_SIZE = 200;
    private const int REQUIRED_SAMPLES = 4;
    // ratio of good matches to number of keypoints to accept as same person
    private const double ORB_MATCH_RATIO = 0.15; // calibrate: 0.1..0.25

    public event Action<byte[]>? OnPreviewFrame;
    private readonly VideoCapture _capture;
    private readonly CascadeClassifier _cascade;
    private readonly LBPHFaceRecognizer _recognizer;

    public FacialRecognition()
    {
        _capture = new VideoCapture(0, VideoCaptureAPIs.DSHOW);
        _capture.Set(VideoCaptureProperties.BufferSize, 1);
        var cascade_filepath = System.IO.Path.Combine(
            System.AppContext.BaseDirectory,
            "assets", "haarcascade_frontalface_default.xml"
            );
        _cascade = new CascadeClassifier(cascade_filepath);
        _recognizer = LBPHFaceRecognizer.Create();
    }

    // --------------------------------------------
    // Capture single face and return as byte[]
    // --------------------------------------------
    public byte[] Capture()
    {
        using var frame = new Mat();
        // _capture.Read(frame);
        // Discart 3 frames
        for (int i = 0; i < 5; i++)
            _capture.Grab();

        _capture.Retrieve(frame);

        if (frame.Empty())
            throw new Exception("Camera frame empty");

        using var gray = new Mat();
        Cv2.CvtColor(frame, gray, ColorConversionCodes.BGR2GRAY);

        var faces = _cascade.DetectMultiScale(gray, 1.1, 5);

        if (faces.Length == 0)
            throw new Exception("No face detected");

        var face = new Mat(gray, faces[0]);
        Cv2.Resize(face, face, new Size(FACE_SIZE, FACE_SIZE));
        Cv2.EqualizeHist(face, face);

        return face.ToBytes(".png");
    }

    // --------------------------------------------
    // Capture frames continuously until a face sample is captured or cancellation requested
    // Provides preview frames via OnPreviewFrame and optional status callback
    // --------------------------------------------
    public async Task<byte[]?> CaptureAsync(Action<string>? statusCallback, CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            try
            {
                var frame = Mat.FromImageData(Capture());
                try
                {
                    OnPreviewFrame?.Invoke(frame.ToBytes());
                }
                catch { }
                statusCallback?.Invoke("Hold still...");
                // try to detect a face
                Cv2.CvtColor(frame, frame, ColorConversionCodes.BGR2GRAY);
                var faces = _cascade.DetectMultiScale(frame, 1.1, 5);

                if (faces.Length > 0)
                {
                    var face = new Mat(frame, faces[0]);
                    Cv2.Resize(face, face, new Size(FACE_SIZE, FACE_SIZE));
                    Cv2.EqualizeHist(face, face);
                    return face.ToBytes(".png");
                }
            }
            catch (Exception error)
            {
                statusCallback?.Invoke(error.Message);
            }
            await Task.Delay(30, ct).ContinueWith(_ => { });
        }
        // cancellation requested - return null
        return null;
    }

    // --------------------------------------------
    // Compare two stored images using ORB + BFMatcher
    // --------------------------------------------
    public bool Verify(byte[] stored, byte[] captured)
    {
        using var storedMat = Cv2.ImDecode(stored, ImreadModes.Grayscale);
        using var capturedMat = Cv2.ImDecode(captured, ImreadModes.Grayscale);

        if (storedMat.Empty() || capturedMat.Empty())
            return false;

        // equalize hist to reduce lighting differences
        try { Cv2.EqualizeHist(storedMat, storedMat); } catch { }
        try { Cv2.EqualizeHist(capturedMat, capturedMat); } catch { }

        // resize captured to stored size for more consistent feature extraction
        if (storedMat.Size() != capturedMat.Size())
            Cv2.Resize(capturedMat, capturedMat, storedMat.Size());

        using var orb = ORB.Create();
        KeyPoint[] kpStored, kpCaptured;
        using var descStored = new Mat();
        using var descCaptured = new Mat();

        orb.DetectAndCompute(storedMat, null, out kpStored, descStored);
        orb.DetectAndCompute(capturedMat, null, out kpCaptured, descCaptured);

        if (descStored.Empty() || descCaptured.Empty())
            return false;

        using var matcher = new BFMatcher(NormTypes.Hamming, crossCheck: false);
        var matches = matcher.KnnMatch(descStored, descCaptured, k: 2);

        int good = 0;
        foreach (var m in matches)
        {
            if (m.Length >= 2)
            {
                var m0 = m[0];
                var m1 = m[1];
                if (m0.Distance < 0.75f * m1.Distance)
                    good++;
            }
        }

        var denom = Math.Min(kpStored.Length, kpCaptured.Length);
        if (denom == 0) return false;

        double ratio = (double)good / denom;

        // optional: log for calibration
        // Console.WriteLine($"ORB verify: good={good}, kpStored={kpStored.Length}, kpCaptured={kpCaptured.Length}, ratio={ratio:F3}");

        return ratio >= ORB_MATCH_RATIO;
    }

    // --------------------------------------------
    // Convert to Base64
    // --------------------------------------------
    public string ToBase64(byte[] biometric)
    {
        return Convert.ToBase64String(biometric);
    }

    public byte[] FromBase64(string base64)
    {
        return Convert.FromBase64String(base64);
    }

    private byte[] MergeSamples(List<Mat> samples)
    {
        var accumulator = new Mat(samples[0].Size(), MatType.CV_32FC1, Scalar.All(0));

        foreach (var sample in samples)
        {
            using var floatMat = new Mat();
            sample.ConvertTo(floatMat, MatType.CV_32FC1);
            accumulator += floatMat;
        }

        accumulator /= samples.Count;

        var result = new Mat();
        accumulator.ConvertTo(result, MatType.CV_8UC1);

        return result.ToBytes(".png");
    }

    // --------------------------------------------
    // Enrollment (capture multiple samples synchronously)
    // --------------------------------------------
    public byte[]? Enrollment()
    {
        var samples = new List<Mat>();
        while (samples.Count < REQUIRED_SAMPLES)
        {
            try
            {
                var bytes = Capture();
                var mat = Cv2.ImDecode(bytes, ImreadModes.Grayscale);
                samples.Add(mat);
                Thread.Sleep(500); // small delay between captures
            }
            catch
            {
                return null;
            }
        }
        return MergeSamples(samples);
    }

    // --------------------------------------------
    // Async enrollment with feedback and preview
    // --------------------------------------------
    public async Task<byte[]?> EnrollmentAsync(
        Action<string> callback,
        CancellationToken cls)
    {
        var samples = new List<Mat>();

        while (samples.Count < REQUIRED_SAMPLES)
        {
            if (cls.IsCancellationRequested)
                return null;

            callback(Resources.GetString("FINGERPRINT_CAPTURE_SAMPLE", samples.Count + 1, REQUIRED_SAMPLES));

            try
            {
                var bytes = await CaptureAsync(callback, cls);
                if (bytes is null)
                    return null;

                var mat = Cv2.ImDecode(bytes, ImreadModes.Grayscale);
                samples.Add(mat);
            }
            catch (Exception error)
            {
                callback(Resources.GetString("FINGERPRINT_CAPTURE_FAIL", error.Message));
            }

            try { await Task.Delay(700, cls); } catch { }
        }

        callback("Merging samples...");

        return MergeSamples(samples);
    }

    // --------------------------------------------
    // Dispose
    // --------------------------------------------
    public void Dispose()
    {
        _capture?.Release();
        _capture?.Dispose();
        _cascade?.Dispose();
        _recognizer?.Dispose();
    }
}