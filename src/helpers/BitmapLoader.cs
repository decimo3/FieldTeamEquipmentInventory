using System.Windows.Media.Imaging;

namespace FieldTeamEquipmentInventory.Helpers;

public static class BitmapLoader
{
    public static BitmapImage? LoadBitmap(byte[] bytes)
    {
        try
        {
            var bitmap = new BitmapImage();
            using var ms = new System.IO.MemoryStream(bytes);
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.StreamSource = ms;
            bitmap.EndInit();
            bitmap.Freeze();
            return bitmap;
        }
        catch
        {
            return null;
        }
    }
}
