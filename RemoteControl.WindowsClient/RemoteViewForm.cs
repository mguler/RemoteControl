using RemoteControl.Shared;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Windows.Forms;

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
            var command = new byte[] { Command.CONTROL, ControlCommand.MOUSE_MOVE };
            byte[] msg = [.. command,
                ..Encoding.UTF8.GetBytes(_serverId),
                ..new byte[] { 0x0 , (byte)pictureBox1.SizeMode } ,
                ..BitConverter.GetBytes(ClientSize.Height),
                ..BitConverter.GetBytes(ClientSize.Width),
                ..BitConverter.GetBytes(e.Location.X),
                ..BitConverter.GetBytes(e.Location.Y)
            ];

            _udp.Send(msg, msg.Length, _serverEp);
        }

        private void PictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            var command = new byte[] { Command.CONTROL, ControlCommand.MOUSE_DOWN };
            var button = e.Button == MouseButtons.Left ? MouseButton.LEFT : MouseButton.RIGHT;

            byte[] msg = [.. command,
                ..Encoding.UTF8.GetBytes(_serverId),
                ..new byte[] { 0x0 , (byte)pictureBox1.SizeMode } ,
                ..BitConverter.GetBytes(ClientSize.Height),
                ..BitConverter.GetBytes(ClientSize.Width),
                ..BitConverter.GetBytes(e.Location.X),
                ..BitConverter.GetBytes(e.Location.Y),
                .. new byte[]{ button }
            ];
            _udp.Send(msg, msg.Length, _serverEp);

        }

        private void PictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            var command = new byte[] { Command.CONTROL, ControlCommand.MOUSE_UP };
            var button = e.Button == MouseButtons.Left ? MouseButton.LEFT : MouseButton.RIGHT;

            byte[] msg = [.. command,
                ..Encoding.UTF8.GetBytes(_serverId),
                ..new byte[] { 0x0 , (byte)pictureBox1.SizeMode } ,
                ..BitConverter.GetBytes(ClientSize.Height),
                ..BitConverter.GetBytes(ClientSize.Width),
                ..BitConverter.GetBytes(e.Location.X),
                ..BitConverter.GetBytes(e.Location.Y),
                .. new byte[]{ button }
            ];
            _udp.Send(msg, msg.Length, _serverEp);
        }

        private void RemoteViewForm_KeyDown(object sender, KeyEventArgs e)
        {
            var command = new byte[] { Command.CONTROL, ControlCommand.KEY_DOWN };

            byte[] msg = [.. command,
                ..Encoding.UTF8.GetBytes(_serverId),
                ..new byte[]{ (byte)e.KeyValue }
            ];

            _udp.Send(msg, msg.Length, _serverEp);

        }

        private void RemoteViewForm_KeyUp(object sender, KeyEventArgs e)
        {
            var command = new byte[] { Command.CONTROL, ControlCommand.KEY_UP };

            byte[] msg = [.. command,
                ..Encoding.UTF8.GetBytes(_serverId),
                ..new byte[]{ (byte)e.KeyValue }
            ];

            _udp.Send(msg, msg.Length, _serverEp);
        }
        async void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _running = false;
            if (_udp != null && !string.IsNullOrEmpty(_serverId))
            {
                var payload = Encoding.UTF8.GetBytes($" {_serverId}");
                payload[0] = Command.UNSUBSCRIBE;
                await _udp.SendAsync(payload, payload.Length, _serverEp);
                _udp.Close();
            }
        }
    }
}
