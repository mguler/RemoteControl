using System.Runtime.InteropServices;

public static class Win32
{
    public const int CURSOR_SHOWING = 0x00000001;

    public const uint MOUSEEVENTF_MOVE = 0x0001;
    public const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
    public const uint MOUSEEVENTF_LEFTUP = 0x0004;
    public const uint MOUSEEVENTF_RIGHTDOWN = 0x0008;
    public const uint MOUSEEVENTF_RIGHTUP = 0x0010;
    public const uint KEYEVENTF_KEYUP = 0x0002;

    [DllImport("user32.dll")]
    public static extern bool GetCursorInfo(out CursorInfo pci);
    [DllImport("user32.dll")]
    public static extern bool SetProcessDPIAware();

    [DllImport("kernel32.dll")]
    public static extern bool AllocConsole();

    [DllImport("kernel32.dll")]
    public static extern bool FreeConsole();

    [DllImport("user32.dll", SetLastError = true)]
    public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, UIntPtr dwExtraInfo);
    [DllImport("user32.dll")]
    public static extern int GetSystemMetrics(int nIndex);

    public const int SM_CXSCREEN = 0;
    public const int SM_CYSCREEN = 1;
    public const int SM_CXFULLSCREEN = 16;
    public const int SM_CYFULLSCREEN = 17;

    public const int SM_XVIRTUALSCREEN = 76; //	Sanal ekranın sol ucu
    public const int SM_YVIRTUALSCREEN = 77; //Sanal ekranın üst ucu
    public const int SM_CXVIRTUALSCREEN = 78; //Tüm ekranın genişliği(px)
    public const int SM_CYVIRTUALSCREEN = 79; //Tüm ekranın yüksekliği(px)


    [DllImport("user32.dll")]
    public static extern nint GetDesktopWindow();

    [DllImport("user32.dll")]
    public static extern nint GetWindowDC(nint hWnd);
    [DllImport("user32.dll")]
    public static extern IntPtr GetDC(IntPtr hWnd);

    [DllImport("gdi32.dll", EntryPoint = "BitBlt", SetLastError = true)]
    public static extern bool BitBlt([In] IntPtr hdc, int nXDest, int nYDest, int nWidth, int nHeight, [In] IntPtr hdcSrc, int nXSrc, int nYSrc, int dwRop);

    public const int SRCCOPY = 0x00CC0020;  //Kaynağı hedefe aynen kopyala(en yaygın)
    public const int SRCINVERT = 0x00660046; //Kaynak ve hedefi XOR’la
    public const int SRCAND = 0x008800C6; //Kaynak ve hedefin AND’i
    public const int SRCPAINT = 0x00EE0086; //Kaynak ve hedefin OR’u
    public const int MERGECOPY = 0x00C000CA; //Source AND pattern
    public const int MERGEPAINT = 0x00BB0226; //NOT source OR dest
    public const int NOTSRCCOPY = 0x00330008; //NOT source
    public const int NOTSRCERASE = 0x001100A6; //NOT src AND NOT dest
    public const int PATCOPY = 0x00F00021; //Pattern’i hedefe yaz
    public const int PATINVERT = 0x005A0049; //Pattern XOR dest
    public const int BLACKNESS = 0x00000042; //Tamamen siyah doldur
    public const int WHITENESS = 0x00FF0062; //Tamamen beyaz doldur
    public const int CAPTUREBLT = 0x40000000; //Layered window gibi özel efektleri de yakala
}