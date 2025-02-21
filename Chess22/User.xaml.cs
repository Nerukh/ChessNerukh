using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Chess22
{
    /// <summary>
    /// Interaction logic for User.xaml
    /// </summary>
    public partial class User : Window
    {
        private TcpClient client;
        private NetworkStream _stream;
        private Thread _listenerThread;

        public User()
        {
            InitializeComponent();
            ConnectToServer();
        }

        private void ConnectToServer()
        {
                client = new TcpClient("127.0.0.1", 5000);
                _stream = client.GetStream();
                _listenerThread = new Thread(ListenForUpdates) { IsBackground = true };
                _listenerThread.Start();
        }

        private void ListenForUpdates()
        {
        while (true)
        {
                    byte[] buffer = new byte[256];
                    int bytesRead = _stream.Read(buffer, 0, buffer.Length);
                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    
        }


        }

    }
}
