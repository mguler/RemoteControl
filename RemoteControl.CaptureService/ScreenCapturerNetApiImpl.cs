using System.Drawing.Imaging;

namespace RemoteControl.CaptureService
{
    public class ScreenCapturerNetApiImpl
    {
        public static byte[] CaptureScreenBytes()
        {
            var screenWidth = Screen.PrimaryScreen.Bounds.Width;
            var screenHeight = Screen.PrimaryScreen.Bounds.Height;

            using var bmp = new Bitmap(screenWidth, screenHeight);
            using var g = Graphics.FromImage(bmp);

            g.CopyFromScreen(new Point(0, 0), Point.Empty, new Size(screenWidth, screenHeight));
            g.Flush();

            using var ms = new MemoryStream();
            var jpgEnc = ImageCodecInfo.GetImageEncoders().First(c => c.FormatID == ImageFormat.Jpeg.Guid);
            var encParams = new EncoderParameters(1) { Param = { [0] = new EncoderParameter(Encoder.Quality, 30L) } };
            bmp.Save(ms, jpgEnc, encParams);

            return ms.ToArray();
        }
    }
}
