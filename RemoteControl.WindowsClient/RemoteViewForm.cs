using RemoteControl.Shared;
using System.Net.Sockets;
using System.Net;
using System.Text;
using RemoteControl.Shared.Extensions;

namespace RemoteControl.WindowsClient
{
    public partial class RemoteViewForm : Form
    {
        UdpClient _udp;
        IPEndPoint _serverEp;
        string _serverId;
        private bool _running;
        private List<byte[]> _d = new List<byte[]>();

        public RemoteViewForm(UdpClient udp, IPEndPoint serverEP, string serverId)
        {
            _udp = udp;
            _serverEp = serverEP;
            _serverId = serverId;
            InitializeComponent();
            _running = true;
            _ = Task.Run(ReceiveLoop);
        }

        async Task ReceiveLoop()
        {
            while (_running)
            {
                UdpReceiveResult res;
                try { res = await _udp.ReceiveAsync(); }
                catch { break; }
                Process(res.Buffer);
            }
        }

        void Process(byte[] data)
        {
            var id = BitConverter.ToInt32(data, 11);
            var packets = BitConverter.ToInt32(data, 15);
            var index = BitConverter.ToInt32(data, 19);

            var payload = new byte[data.Length - 23];
            Buffer.BlockCopy(data, 23, payload, 0, data.Length - 23);
            _d.Add(payload);

            if (index >= packets)
            {
                try
                {
                    var len = _d.Sum(x => x.Length);
                    var ms = new MemoryStream(len);
                    _d.ForEach(x => ms.Write(x));
                    var currentFrame = new Bitmap(Image.FromStream(ms));
                    pictureBox1.Invoke((Action)(() => pictureBox1.Image = currentFrame));
                }
                catch { }
                finally { _d = new List<byte[]>(); }
            }

        }


        private void PictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            using var ms = new MemoryStream();
            using var writer = new BinaryWriter(ms, Encoding.UTF8);
            writer.Write(Command.CONTROL
                , ControlCommand.MOUSE_MOVE
                , Encoding.UTF8.GetBytes(_serverId)
                , (byte)0x00
                , (byte)pictureBox1.SizeMode
                , ClientSize.Height
                , ClientSize.Width
                , e.Location.X
                , e.Location.Y);

            writer.Flush();
            var chunk = ms.GetBuffer();

            _udp.Send(chunk, chunk.Length, _serverEp);
        }

        private void PictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            var button = e.Button == MouseButtons.Left ? MouseButton.LEFT : MouseButton.RIGHT;

            using var ms = new MemoryStream();
            using var writer = new BinaryWriter(ms, Encoding.UTF8);
            ms.Position = 0;

            writer.Write(Command.CONTROL
                , ControlCommand.MOUSE_DOWN
                , Encoding.UTF8.GetBytes(_serverId)
                , (byte)0x00
                , (byte)pictureBox1.SizeMode
                , ClientSize.Height
                , ClientSize.Width
                , e.Location.X
                , e.Location.Y
                , button);

            var chunk = ms.GetBuffer();

            _udp.Send(chunk, chunk.Length, _serverEp);
        }

        private void PictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            var button = e.Button == MouseButtons.Left ? MouseButton.LEFT : MouseButton.RIGHT;

            using var ms = new MemoryStream();
            using var writer = new BinaryWriter(ms, Encoding.UTF8);

            writer.Write(Command.CONTROL
                , ControlCommand.MOUSE_UP
                , Encoding.UTF8.GetBytes(_serverId)
                , (byte)0x00
                , (byte)pictureBox1.SizeMode
                , ClientSize.Height
                , ClientSize.Width
                , e.Location.X
                , e.Location.Y
                , button);

            var chunk = ms.GetBuffer();

            _udp.Send(chunk, chunk.Length, _serverEp);
        }

        private void RemoteViewForm_KeyDown(object sender, KeyEventArgs e)
        {
            using var ms = new MemoryStream();
            using var writer = new BinaryWriter(ms, Encoding.UTF8);

            writer.Write(Command.CONTROL
                , ControlCommand.KEY_DOWN
                , Encoding.UTF8.GetBytes(_serverId)
                , e.KeyValue);

            writer.Flush();
            var chunk = ms.GetBuffer();

            _udp.Send(chunk, chunk.Length, _serverEp);

        }

        private void RemoteViewForm_KeyUp(object sender, KeyEventArgs e)
        {
            using var ms = new MemoryStream();
            using var writer = new BinaryWriter(ms, Encoding.UTF8);

            writer.Write(Command.CONTROL
                , ControlCommand.KEY_UP
                , Encoding.UTF8.GetBytes(_serverId)
                , e.KeyValue);

            writer.Flush();
            var chunk = ms.GetBuffer();

            _udp.Send(chunk, chunk.Length, _serverEp);
        }
        async void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            using var ms = new MemoryStream();
            using var writer = new BinaryWriter(ms, Encoding.UTF8);

            writer.Write(Command.UNSUBSCRIBE
                , Encoding.UTF8.GetBytes(_serverId));

            writer.Flush();
            var chunk = ms.GetBuffer();

            _udp.Send(chunk, chunk.Length, _serverEp);
        }
    }
}
