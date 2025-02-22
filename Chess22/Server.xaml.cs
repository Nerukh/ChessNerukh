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
using System.IO;
using System.Windows.Threading;

namespace Chess22
{

    public partial class Server : Window
    {
        private readonly List<TcpClient> clients = new();
        private TcpListener _server;
        private int count_users = 0;
        private int count_in_game = 0;
        private readonly object _lock = new();
        List<string> users = new List<string>();
        private DispatcherTimer broadcastTimer;

        public Server()
        {
            InitializeComponent();
            Network_label.Content = Chess22.Language.GetTranslation("Network");
            Play_label.Content = Chess22.Language.GetTranslation("Play");
            Start_button.Content = Chess22.Language.GetTranslation("Start");
            Users_Table_Label.Content = Chess22.Language.GetTranslation("Users_Table");
            filling();
            
        }

        private void UsersListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (UsersListBox.SelectedItem != null)
            {

                string selectedUser = UsersListBox.SelectedItem.ToString();

                UserLibrary userLibrary = new UserLibrary(selectedUser);
                userLibrary.Show();

            }
        }

        private void Start_button_Click(object sender, RoutedEventArgs e)
        {
            Running_Server_Label.Content = Chess22.Language.GetTranslation("Running_Server");
            Start_button.IsEnabled = false;
            _server = new TcpListener(IPAddress.Any, 5000);
            _server.Start();
            Thread serverThread = new Thread(AcceptClients) { IsBackground = true };
            serverThread.Start();
            StartBroadcastTimer();
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
                filling();
            }
        }

        private void HandleClient(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = Encoding.UTF8.GetBytes($"{count_users}");
            stream.Write(buffer, 0, buffer.Length);
        }

        private void StartBroadcastTimer()
        {
            broadcastTimer = new DispatcherTimer();
            broadcastTimer.Interval = TimeSpan.FromSeconds(1);
            broadcastTimer.Tick += (sender, args) => BroadcastClientCount();
            broadcastTimer.Start();
        }

        private void BroadcastClientCount()
        {
            Dispatcher.Invoke(() => { Network_number_label.Content = count_users; });
            lock (_lock)
            {
                List<TcpClient> disconnectedClients = new List<TcpClient>();

                foreach (var client in clients)
                {
                    try
                    {
                        if (client.Client.Poll(0, SelectMode.SelectRead) && client.Client.Available == 0)
                        {
                            disconnectedClients.Add(client);
                            continue;
                        }

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

        private void filling()
        {
            string filePath = @"C:\Users\nv192\source\repos\Chess\Chess22\Users.txt";
            if (File.Exists(filePath))
            {
                string[] lines = File.ReadAllLines(filePath);

                foreach (string line in lines)
                {
                    string[] parts = line.Split(';');
                    // Якщо формат: "Name: UserName; ..." використовуйте Split(':') замість пробілу
                    string[] nameParts = parts[0].Split(':');
                    if (nameParts.Length > 1)
                    {
                        string names = nameParts[1].Trim().ToLower();
                        bool userExists = users.Contains(names);
                        if (!userExists)
                        {
                            users.Add(names);
                        }
                    }
                }
                Dispatcher.Invoke(() =>
                {
                    UsersListBox.ItemsSource = null;
                    UsersListBox.ItemsSource = users;
                });
            }
        }

    }
    
}

