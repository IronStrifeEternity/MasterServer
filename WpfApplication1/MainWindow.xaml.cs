using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using IronStrife.MasterServer;
using IronStrife.ChatServer;

namespace IronStrife
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        StrifeMasterServer server;
        ObservableCollection<ServerInfo> Servers { get { return server.Servers; } }
        StrifeChatServer chatServer;

        public MainWindow()
        {
            InitializeComponent();
            server = new StrifeMasterServer();
            serverListView.ItemsSource = Servers;
            server.Start();
            server.OnMessage += Print;

            chatServer = new StrifeChatServer();
            listOfConnectedChatUsers.ItemsSource = chatServer.ConnectedUsers;
            chatPanelOutput.ItemsSource = chatServer.ConsoleLogs;
            chatServer.Start();

            this.Closed += MainWindow_Closed;
        }

        void MainWindow_Closed(object sender, EventArgs e)
        {
            server.Stop();
            chatServer.Stop();
        }


        void Print(string message)
        {
            outputBox.Items.Add(message);
        }

        private void chatInputArea_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SubmitChatInput(chatInputArea.Text);
            }
        }

        private void SubmitChatInput(string command)
        {
            var split = command.Split(' ');
            switch (split[0])
            {
                case "global":
                    var message = split.Skip(1);
                    StringBuilder sb = new StringBuilder();
                    foreach (string s in message)
                    {
                        sb.Append(s + " ");
                    }
                    chatServer.SendGlobalChatMessage(sb.ToString());
                    break;
            }
        }

        private void chatInputSubmitButton_Click(object sender, RoutedEventArgs e)
        {
            SubmitChatInput(chatInputArea.Text);
        }
    }
}
