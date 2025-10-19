using System.Net.Sockets;
using System.Net;
using System.Text;
using RemoteControl.Shared;

namespace RemoteControl.WindowsClient
{
    public partial class MainForm : Form
    {
        private RemoteViewForm _remoteViewForm;
        const int Port = 7000;

        public MainForm()
        {
            InitializeComponent();
        }

        async void Connect()
        {
            var serverId = textBox1.Text.Trim();

            if (string.IsNullOrWhiteSpace(serverId)) return;


            var payload = Encoding.UTF8.GetBytes($" {serverId}");
            payload[0] = Command.SUBSCRIBE;

            var ip = new byte[] { 192,168,1,42 }; //new byte[] { 172, 20, 10, 2 }
            IPEndPoint serverEp = new IPEndPoint(new IPAddress(ip), Port);
            UdpClient  udp = new UdpClient(0);
            await udp.SendAsync(payload, serverEp);


            _remoteViewForm = new RemoteViewForm(udp, serverEp, serverId);
            _remoteViewForm.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Connect();
        }
    }
}
