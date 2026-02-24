namespace FieldTeamEquipmentInventory.Interfaces;

public interface IBiometrics : IDisposable
{
    event Action<byte[]>? OnPreviewFrame;
    public byte[] Capture();
    public Task<byte[]?> CaptureAsync(Action<string> callback, CancellationToken cls);
    public byte[]? Enrollment();
    public Task<byte[]?> EnrollmentAsync(Action<string> callback, CancellationToken cls);
    public bool Verify(byte[] stored, byte[] captured);
    public string ToBase64(byte[] biometric);
    public byte[] FromBase64(string base64);
}
