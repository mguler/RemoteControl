namespace RemoteControl.Shared
{
    public static class Command 
    {
        public const byte REGISTER = 0x01;
        public const byte ID = 0x02;
        public const byte SUBSCRIBE = 0x03;
        public const byte UNSUBSCRIBE = 0x04;
        public const byte FRAME = 0x05;
        public const byte CONTROL = 0x06;
    }
}
