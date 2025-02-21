using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
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
using System.Collections.Concurrent;

namespace Chess22
{

    public partial class Server : Window
    {
        private readonly List<TcpClient> clients = new();
        private TcpListener _server;
        private int count_users = 0;
        private int count_in_game = 0;
        private readonly object _lock = new();

        public Server()
        {
            InitializeComponent();
            Network_label.Content = Chess22.Language.GetTranslation("Network");
            Play_label.Content = Chess22.Language.GetTranslation("Play");
            Start_button.Content = Chess22.Language.GetTranslation("Start");

        }

        private void Start_button_Click(object sender, RoutedEventArgs e)
        {
            _server = new TcpListener(IPAddress.Any, 5000);
            _server.Start();
            Thread serverThread = new Thread(AcceptClients) { IsBackground = true };
            serverThread.Start();
        }

        private void AcceptClients()
        {
            while (true)
            {
                TcpClient client = _server.AcceptTcpClient();
                lock (_lock)
                {
                    clients.Add(client);
                    count_users++;
                }
                BroadcastClientCount();
                Thread clientThread = new Thread(() => HandleClient(client)) { IsBackground = true };
                clientThread.Start();
            }
        }

        private void HandleClient(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = Encoding.UTF8.GetBytes($"{count_users}");
            stream.Write(buffer, 0, buffer.Length);
        }

        private void BroadcastClientCount()
        {
            Dispatcher.Invoke(() => { Network_number_label.Content = count_users; });
            lock (_lock)
            {
                List<TcpClient> disconnectedClients = new();

                foreach (var client in clients)
                {
                    try
                    {
                        NetworkStream stream = client.GetStream();
                        byte[] buffer = Encoding.UTF8.GetBytes($"{count_users}");
                        stream.Write(buffer, 0, buffer.Length);
                    }
                    catch
                    {
                        disconnectedClients.Add(client);
                    }
                }


                foreach (var client in disconnectedClients)
                {
                    clients.Remove(client);
                    count_users--;
                }
            }
        }

    }
    
}

