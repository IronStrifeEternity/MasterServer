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
using IronStrifeMasterServer;
using System.Collections.ObjectModel;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MasterServer server;
        ObservableCollection<ServerInfo> Servers { get { return server.Servers; } }

        public MainWindow()
        {
            InitializeComponent();
            server = new MasterServer();
            serverListView.ItemsSource = Servers;
            server.Start();
            server.OnMessage += Print;
        }

        void Print(string message)
        {
            outputBox.Items.Add(message);
        }

    }
}
