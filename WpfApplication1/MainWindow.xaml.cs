using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
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
            Application.Current.Shutdown(0);
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
            var com = new ChatCommand() { name = split[0], parameters = split.Skip(1).ToArray() };
            chatServer.SubmitCommand(com);
        }

        private void chatInputSubmitButton_Click(object sender, RoutedEventArgs e)
        {
            SubmitChatInput(chatInputArea.Text);
        }
    }
}
