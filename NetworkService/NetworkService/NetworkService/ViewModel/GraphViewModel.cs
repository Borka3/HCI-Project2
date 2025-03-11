using NetworkService.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.ComponentModel;
using NetworkService.Helpers;

namespace NetworkService.ViewModel
{
    public class GraphViewModel : BindableBase
    {
        public string SelectedItemName { get; set; }
        private Entity selectedEntity;
        private Entity selectedEntityToShow;
        private int keyCount;

        public BindingList<Entity> EntitiesInList;
        public ObservableCollection<Entity> Entities { get; set; }
        public ObservableCollection<string> EntityNames
        {
            get
            {
                return new ObservableCollection<string>(Entities.Select(e => e.Name).ToList());
            }
        }

     
        public Dictionary<string, List<DateTimeValue>> MeasurementDict { get; set; }
        public ObservableCollection<ElipseHandler> CircleMarkers { get; set; }
        public MyICommand ShowCommand { get; set; }

        public GraphViewModel(ObservableCollection<Entity> entities)
        {
            Entities = entities;


            
            MeasurementDict = new Dictionary<string, List<DateTimeValue>>();
            CircleMarkers = new ObservableCollection<ElipseHandler>();

            for (int i = 0; i < 5; i++)
            {
                CircleMarkers.Add(new ElipseHandler());
            }
            EntitiesInList = new BindingList<Entity>();

            UpdateComboBoxItems();
            ShowCommand = new MyICommand(OnShow);
            
            
        }

        public Entity SelectedEntity
        {
            get { return selectedEntity; }
            set
            {
                selectedEntity = value;
                
                OnPropertyChanged("SelectedEntity");
            }
        }

        public Entity SelectedEntityToShow
        {
            get { return selectedEntityToShow; }
            set
            {
                selectedEntityToShow = value;
                OnPropertyChanged("SelectedEntityToShow");
            }
        }

        private void UpdateComboBoxItems()
        {
            OnPropertyChanged(nameof(EntityNames));
        }

        private void OnEntitiesInListChanged(object sender, ListChangedEventArgs e)
        {
            UpdateComboBoxItems();

        }

        public void OnShow()
        {
             foreach(Entity en in Entities) 
             {
                if(en.Name == SelectedItemName)
                {
                    SelectedEntityToShow = en;
                    break;
                }
            }
             keyCount = 0;
             foreach (var entity in Entities)
             {
                 if (entity.Name == SelectedEntityToShow.Name)
                 {
                     break;
                 }
                 keyCount++;
             }
             foreach (ElipseHandler marker in CircleMarkers)
             {
                 marker.CmTime = "";
                 marker.CmValue = 1;
                 marker.CmDate = "";
             }
             UpdateValue();

            
        }

        private bool CanShow()
        {
            return SelectedEntity != null;
        }

        public void UpdateValue()
        {
            foreach (var item in MeasurementDict.Keys)
            {
                string customKey = $"Entity_{keyCount}";

                if (item == customKey)
                {
                    List<DateTimeValue> list = MeasurementDict[item];
                    int cnt = 0;
                    foreach (DateTimeValue measurement in list)
                    {
                        ElipseHandler cm = CircleMarkers[cnt];
                        cm.CmValue = measurement.Value;
                        cm.CmDate = measurement.Date;
                        cm.CmTime = measurement.Time;
                        CircleMarkers[cnt] = cm;
                        cnt++;
                        if (cnt == 5)
                        {
                            break;
                        }
                    }
                }
            }
        }

        public void AutoShow()
        {
            string filePath = "Log.txt";
            string endLine = ReadLastLine(filePath);
            if (endLine != null)
            {
                (string date, string time, string entity, double value) = ParseLine(endLine);
                string[] entityParts = entity.Split('_');
                int entityNumber = int.Parse(entityParts[1]);

                DateTimeValue measurement = new DateTimeValue(date, time, value);
                string key = $"Entity_{entityNumber}";

                if (MeasurementDict.ContainsKey(key))
                {
                    MeasurementDict[key].Add(measurement);
                    if (MeasurementDict[key].Count > 5)
                    {
                        MeasurementDict[key].RemoveAt(0);
                    }
                }
                else
                {
                    MeasurementDict[key] = new List<DateTimeValue> { measurement };
                }
                UpdateValue();
            }
        }

        static string ReadLastLine(string filePath)
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                string lastLine = null;

                while ((line = reader.ReadLine()) != null)
                {
                    lastLine = line;
                }

                return lastLine;
            }
        }

        static (string date, string time, string entity, double value) ParseLine(string line)
        {
            string[] parts = line.Split(';');

            string dateTimePart = parts[0].Trim();
            string[] dateTimeParts = dateTimePart.Split(' ');
            string date = dateTimeParts[0];
            string time = dateTimeParts[1];

            string leftover = parts[1].Trim();
            string[] leftoverParts = leftover.Split(',');

            string entity = leftoverParts[0].Trim();
            double value = double.Parse(leftoverParts[1].Trim());

            return (date, time, entity, value);
        }

    }
}
