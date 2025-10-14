using System.Drawing.Imaging;

namespace RemoteControl.CaptureService
{
    public class ScreenCaptureWin32BitmapImpl
    {
        public static byte[] CaptureScreenBytes()
        {
            Win32.SetProcessDPIAware();

            var screenWidth = Win32.GetSystemMetrics(Win32.SM_CXSCREEN);
            var screenHeight = Win32.GetSystemMetrics(Win32.SM_CYSCREEN);

            var desktopWnd = Win32.GetDesktopWindow();
            var desktopDC = Win32.GetDC(IntPtr.Zero);

            using var bmp = new Bitmap(screenWidth, screenHeight);
            using var g = Graphics.FromImage(bmp);
            var hdc = g.GetHdc();

            Win32.BitBlt(hdc, 0, 0, screenWidth, screenHeight, desktopDC, 0, 0, Win32.SRCCOPY | Win32.CAPTUREBLT);
            g.ReleaseHdc(hdc);

            using var ms = new MemoryStream();
            var jpgEnc = ImageCodecInfo.GetImageEncoders().First(c => c.FormatID == ImageFormat.Jpeg.Guid);
            var encParams = new EncoderParameters(1) { Param = { [0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 30L) } };
            bmp.Save(ms, jpgEnc, encParams);

            return ms.ToArray();
        }
    }
}
