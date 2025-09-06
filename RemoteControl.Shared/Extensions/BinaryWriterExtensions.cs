using System.Text;

namespace RemoteControl.Shared.Extensions
{
    public static class BinaryWriterExtensions
    {
        public static void Write(this BinaryWriter writer, params object[] values)
        {
            foreach (var value in values)
            {
                switch (value)
                {
                    case byte b: writer.Write(b); break;
                    case sbyte sb: writer.Write(sb); break;
                    case short s: writer.Write(s); break;
                    case ushort us: writer.Write(us); break;
                    case int i: writer.Write(i); break;
                    case uint ui: writer.Write(ui); break;
                    case long l: writer.Write(l); break;
                    case ulong ul: writer.Write(ul); break;
                    case float f: writer.Write(f); break;
                    case double d: writer.Write(d); break;
                    case decimal dec: writer.Write(dec); break;
                    case char c: writer.Write(c); break;
                    case bool bo: writer.Write(bo); break;

                    case string str:
                        writer.Write(Encoding.UTF8.GetBytes(str));
                        break;
                    case byte[] buffer:
                        writer.Write(buffer);
                        break;

                    default:
                        throw new InvalidOperationException(
                            $"Unsupported type: {value.GetType().FullName}");
                }
            }
        }
    }
}