namespace RemoteControl.Shared
{
    public static class ControlCommand 
    {
        public const byte MOUSE_MOVE = 0x01;
        public const byte MOUSE_DOWN = 0x02;
        public const byte MOUSE_UP = 0x03;
        public const byte KEY_DOWN = 0x04;
        public const byte KEY_UP = 0x05;
    }
}
