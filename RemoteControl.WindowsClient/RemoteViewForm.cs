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
        bool _running;
        MemoryStream _viewCache;
        BinaryWriter _viewWriter;
        public RemoteViewForm(UdpClient udp, IPEndPoint serverEP, string serverId)
        {
            _udp = udp;
            _serverEp = serverEP;
            _serverId = serverId;
            InitializeComponent();
            _running = true;
            _viewCache = new MemoryStream();
            _viewWriter = new BinaryWriter(_viewCache); 

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
            using var ms = new MemoryStream(data);
            using var reader = new BinaryReader(ms);
            
            ms.Position = 15;

            var packets = reader.ReadInt32();
            var index = reader.ReadInt32();
            _viewWriter.Write(data, 23, data.Length - 23);

            if (index >= packets)
            {
                try
                {
                    var currentFrame = new Bitmap(Image.FromStream(_viewCache));
                    pictureBox1.Invoke((Action)(() => pictureBox1.Image = currentFrame));
                }
                catch {  }
                finally 
                {
                   _viewCache.SetLength(0);
                }
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
