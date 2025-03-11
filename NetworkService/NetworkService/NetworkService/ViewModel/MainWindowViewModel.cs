using MVVMLight.Messaging;
using NetworkService.Helpers;
using NetworkService.Model;
using Notification.Wpf;
using Notification.Wpf.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

namespace NetworkService.ViewModel
{
    public class MainWindowViewModel : BindableBase
    {
        private int count = 15; 
                                
                                
        
        public MyICommand<string> NavCommand { get; private set; }

        private NetworkEntitiesViewModel networkEntitiesViewModel;
        private DisplayViewModel displayViewModel;
        private GraphViewModel graphViewModel;

        public NetworkEntitiesViewModel NetworkEntitiesViewModel
        {
            get { return networkEntitiesViewModel; }
        }
        public DisplayViewModel DisplayViewModel { get { return displayViewModel; } }
        public GraphViewModel GraphViewModel { get { return graphViewModel; } }
        public static ObservableCollection<Entity> Entities { get; set; }
        private NotificationManager notificationManager;


        private string help = "List of commands: \n\n" +
            "- add(id,name,type) - add entity  " +
            "(type can be Cable sensor/Digital manometer)\n" +
            "- delete(id) - delete entity\n" +
            "- filter1(id,criterion,type) - filter entity with id and type\n" +
            "_ filter2(id,crtierion) - filter entity with id\n" +
            "_filter3(type) - filter entity with type\n"+
            "- displayView - switch to display view\n" +
            "- graphView - switch to graph view\n" +
            "- networkView - switch to network view\n" +
            "- undo - a step back";

        

        private bool ind = false;
        public MyICommand<TextBox> terminal { get; set; }
        public MyICommand GoToNetworkEntitiesView {  get; set; }
        public MyICommand GoToDisplayView { get; set; }
        public MyICommand GoToGraphView { get; set; }

        private BindableBase currentViewModel;
        public MainWindowViewModel()
        {
            Entities = new ObservableCollection<Entity>();  
            
            displayViewModel = new DisplayViewModel(Entities);
            networkEntitiesViewModel = new NetworkEntitiesViewModel(displayViewModel, Entities);
            graphViewModel = new GraphViewModel(Entities);


            NavCommand = new MyICommand<string>(OnNav);
            CurrentViewModel = networkEntitiesViewModel;
            GoToNetworkEntitiesView = new MyICommand(ToNetworkView);
            GoToDisplayView = new MyICommand(ToDisplayView);
            GoToGraphView = new MyICommand(ToGraphView);

            Entities = new ObservableCollection<Entity>();
            notificationManager = new NotificationManager();
            terminal = new MyICommand<TextBox>(Terminal);

            createListener(); 

            Messenger.Default.Register<NotificationContent>(this, ShowToastNotification);
        }

        private void ToNetworkView()
        {
            CurrentViewModel = networkEntitiesViewModel;
        }

        private void ToDisplayView()
        {
            CurrentViewModel = displayViewModel;
        }

        private void ToGraphView()
        {
            CurrentViewModel = graphViewModel;
        }

        public void Terminal(TextBox tb)
        {
            string[] p;
            string f = "";
            if (ind == true)
            {
                f = tb.Text.Substring(help.Length);
                p = f.Split('(', ')', ',');
                ind = false;
            }
            else
            {
                p = tb.Text.Split('(', ')', ',');
                f = tb.Text;
            }

            if (p[0] == "help" && p.Length == 1)
            {
                ind = true;
                tb.Text = help;
                tb.FontSize = 16;

            }
            else if (p[0] == "add")
            {
                NetworkEntitiesViewModel.AddCommand.Execute(f);
                tb.Text = "";
            }
            else if (p[0] == "delete")
            {
                if (p.Length > 1)
                {
                    NetworkEntitiesViewModel.DeleteCommand.Execute(p[1]);
                }
                else
                {
                    MessageBox.Show("Invalid parameters for delete command.", "Warning!", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                tb.Text = "";

            }
            else if (p[0] == "graphView")
            {
                CurrentViewModel = graphViewModel;
                tb.Text = "";
            }
            else if (p[0] == "displayView")
            {
                CurrentViewModel = displayViewModel;
                tb.Text = "";
            }
            else if (p[0] == "networkView")
            {
                CurrentViewModel = networkEntitiesViewModel;
                tb.Text = "";
            }
            else if (p[0] == "undo")
            {
                NetworkEntitiesViewModel.UndoCommand.Execute(f);
                tb.Text = "";
            }
            else if (p[0] == "filter1")
            {
                NetworkEntitiesViewModel.FilterCommandConsole.Execute(f);
                tb.Text = "";
            }
            else if (p[0] == "filter2")
            {
                NetworkEntitiesViewModel.FilterCommandConsole.Execute(f);
                tb.Text = "";
            }
            else if (p[0] == "filter3")
            {
                NetworkEntitiesViewModel.FilterCommandConsole.Execute(f);
                tb.Text = "";
            }
            else
            {
                MessageBox.Show("Invalid command", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                tb.Text = "";
            }
        }

        private void OnCollectionChangedMeasurementGraphViewModel(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Entity newEntity in e.NewItems)
                {
                    if (!graphViewModel.EntitiesInList.Contains(newEntity))
                    {
                        graphViewModel.EntitiesInList.Add(newEntity);
                    }
                }
            }
            if (e.OldItems != null)
            {
                foreach (Entity oldEntity in e.OldItems)
                {
                    if (graphViewModel.Entities.Contains(oldEntity))
                    {
                        graphViewModel.EntitiesInList.Remove(oldEntity);
                    }
                }
            }
        }


        public BindableBase CurrentViewModel
        {
            get { return currentViewModel; }
            set
            {
                SetProperty(ref currentViewModel, value);
            }
        }

        private void OnNav(string destination)
        {
            switch (destination)
            {
                case "NetworkEntities":
                    CurrentViewModel = networkEntitiesViewModel;
                    break;
                case "NetworkDisplay":
                    CurrentViewModel = displayViewModel;
                    break;
                case "MeasurementGraph":
                    CurrentViewModel = graphViewModel;
                    break;
            }
        }

        private void ShowToastNotification(NotificationContent notificationContent)
        {
            notificationManager.Show(notificationContent, "WindowNotificationArea");
        }

        private void createListener()
        {
            var tcp = new TcpListener(IPAddress.Any, 25675);
            tcp.Start();

            var listeningThread = new Thread(() =>
            {
                while (true)
                {
                    var tcpClient = tcp.AcceptTcpClient();
                    ThreadPool.QueueUserWorkItem(param =>
                    {
                        
                        NetworkStream stream = tcpClient.GetStream();
                        string incomming;
                        byte[] bytes = new byte[1024];
                        int i = stream.Read(bytes, 0, bytes.Length);
                        
                        incomming = System.Text.Encoding.ASCII.GetString(bytes, 0, i);

                        
                        if (incomming.Equals("Need object count"))
                        {
                           
                            Byte[] data = System.Text.Encoding.ASCII.GetBytes(Entities.Count.ToString());
                            stream.Write(data, 0, data.Length);
                            if(File.Exists("Log.txt"))
                            {
                                File.WriteAllText("Log.txt", String.Empty);
                            }
                            else
                            {
                                File.Create("Log.txt");
                            }
                        }
                        else
                        {
                            //U suprotnom, server je poslao promenu stanja nekog objekta u sistemu
                            Console.WriteLine(incomming); //Na primer: "Entitet_1:272"

                            //################ IMPLEMENTACIJA ####################
                            // Obraditi poruku kako bi se dobile informacije o izmeni
                            // Azuriranje potrebnih stvari u aplikaciji

                            
                            if (networkEntitiesViewModel.Entities.Count > 0)
                            {
                                var splited = incomming.Split(':');
                                DateTime dt = DateTime.Now;
                                using (StreamWriter sw = File.AppendText("Log.txt"))
                                    sw.WriteLine(dt + "; " + splited[0] + ", " + splited[1]);

                                int id = Int32.Parse(splited[0].Split('_')[1]);
                                networkEntitiesViewModel.Entities[id].Value = Double.Parse(splited[1]);

                                displayViewModel.UpdateEntityOnCanvas(networkEntitiesViewModel.Entities[id]);
                                graphViewModel.AutoShow();
                            }

                        }
                    }, null);
                }
            });

            listeningThread.IsBackground = true;
            listeningThread.Start();
        }
    }
}
