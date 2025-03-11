using MVVMLight.Messaging;
using NetworkService.Helpers;
using NetworkService.Model;
using Notification.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace NetworkService.ViewModel
{
    public class NetworkEntitiesViewModel : BindableBase
    {
        public int idInt;
        private Stack<Action> actionHistory = new Stack<Action>();
        private int lastAddedEntityId;
        private DisplayViewModel displayViewModel;
        public List<string> ComboBoxItems { get; set; } = Helpers.ComboBoxItems.entityTypes.Keys.ToList();

        public ObservableCollection<Entity> EntitiesToShow { get; set; }
        public ObservableCollection<Entity> Entities { get; set; }
        public ObservableCollection<Entity> FilteredEntities { get; set; }

        public MyICommand AddEntityCommand { get; set; }
        public MyICommand DeleteEntityCommand { get; set; }
        public MyICommand FilterEntityCommand { get; set; }
        public MyICommand ResetFilterCommand { get; set; }

        
        public static MyCommandWithParam<string> AddCommand { get; set; }
        public static MyCommandWithParam<string> FilterCommandConsole { get; set; }
        public static MyCommandWithParam<string> DeleteCommand { get; set; }

        public static MyCommandWithParam<string> UndoCommand { get; set; }

        private Entity currentEntity = new Entity();
        private EntityType currentEntityType = new EntityType();

        private Entity selectedEntity;

        
        private string selectedEntityTypeToFilter;
        private bool isGreaterThanRadioButtonSelected;
        private bool isLessThanRadioButtonSelected;
        private bool isEquallyRadioButtonSelected;
        private string selectedIdMarginToFilterText;
        private string filterErrorMessage;

        private int cableSensorCount = 0;
        private int digitalManometerCount = 0;

        public NetworkEntitiesViewModel(DisplayViewModel displayViewModel, ObservableCollection<Entity> entities)
        {
            Entities = entities;
            EntitiesToShow = Entities;
            this.displayViewModel = displayViewModel;


            AddEntityCommand = new MyICommand(OnAdd);
            DeleteEntityCommand = new MyICommand(OnDelete, CanDelete);
            FilterEntityCommand = new MyICommand(OnFilter);
            ResetFilterCommand = new MyICommand(OnResetFilter);

            AddCommand = new MyCommandWithParam<string>(param => OnAddConsole(param));
            DeleteCommand = new MyCommandWithParam<string>(param => OnDeleteConsole(param));
            UndoCommand = new MyCommandWithParam<string>(param => OnUndo(param));
            FilterCommandConsole = new MyCommandWithParam<string>(param => OnFilterConsole(param));
        }

        public Entity CurrentEntity
        {
            get { return currentEntity; }
            set
            {
                currentEntity = value;
                OnPropertyChanged("CurrentEntity");
            }
        }

        public EntityType CurrentEntityType
        {
            get { return currentEntityType; }
            set
            {
                currentEntityType = value;
                OnPropertyChanged("CurrentEntityType");
            }
        }

        private void AddToHistory(Action action)
        {
            actionHistory.Push(action);
        }

        public Entity SelectedEntity
        {
            get { return selectedEntity; }
            set
            {
                selectedEntity = value;
                DeleteEntityCommand.RaiseCanExecuteChanged();
            }
        }

        public string SelectedEntityTypeToFilter
        {
            get { return selectedEntityTypeToFilter; }
            set
            {
                selectedEntityTypeToFilter = value;
                OnPropertyChanged("SelectedEntityTypeToFilter");
            }
        }

        public bool IsGreaterThanRadioButtonSelected
        {
            get { return isGreaterThanRadioButtonSelected; }
            set
            {
                isGreaterThanRadioButtonSelected = value;
                OnPropertyChanged("IsGreaterThanRadioButtonSelected");
            }
        }

        public bool IsLessThanRadioButtonSelected
        {
            get { return isLessThanRadioButtonSelected; }
            set
            {
                isLessThanRadioButtonSelected = value;
                OnPropertyChanged("IsLessThanRadioButtonSelected");
            }
        }

        public bool IsEquallyRadioButtonSelected
        {
            get { return isEquallyRadioButtonSelected; }
            set
            {
                isEquallyRadioButtonSelected = value;
                OnPropertyChanged("IsEquallyRadioButtonSelected");
            }
        }

        public string SelectedIdMarginToFilterText
        {
            get { return selectedIdMarginToFilterText; }
            set
            {
                selectedIdMarginToFilterText = value;
                OnPropertyChanged("SelectedIdMarginToFilterText");
            }
        }

        public string FilterErrorMessage
        {
            get { return filterErrorMessage; }
            set
            {
                filterErrorMessage = value;
                OnPropertyChanged("FilterErrorMessage");
            }
        }

        private void OnFilter()
        {
            FilterErrorMessage = String.Empty;
            List<Entity> originalEntities = new List<Entity>(Entities);

            AddToHistory(() =>
            {
                Entities.Clear();
                foreach (var entity in originalEntities)
                {
                    Entities.Add(entity);
                }
                EntitiesToShow = new ObservableCollection<Entity>(Entities);
                OnPropertyChanged("EntitiesToShow");
            });

            if (SelectedEntityTypeToFilter == null && !IsGreaterThanRadioButtonSelected && !IsLessThanRadioButtonSelected && IsEquallyRadioButtonSelected && string.IsNullOrWhiteSpace(SelectedIdMarginToFilterText))
            {
                FilterErrorMessage = "Fields can't be empty.";
                return;
            }

            List<Entity> filteredList = new List<Entity>();

            foreach(Entity entity in Entities)
            {
                filteredList.Add(entity);
            }

           

            if (SelectedEntityTypeToFilter != null)
            {
                List<Entity> entitiesToDelete = new List<Entity>();
                for(int i=0;i<filteredList.Count;i++)
                {
                    if (filteredList[i].TypeEntity.Name != SelectedEntityTypeToFilter)
                    {
                       
                        entitiesToDelete.Add(filteredList[i]);
                    }
                }

                foreach(Entity entity in entitiesToDelete)
                {
                    filteredList.Remove(entity);
                }
            }

            if(IsGreaterThanRadioButtonSelected)
            {
                List<Entity> entitiesToDelete = new List<Entity>();

                if(string.IsNullOrWhiteSpace(SelectedIdMarginToFilterText))
                {
                    FilterErrorMessage = "Id is required.";
                    Messenger.Default.Send<NotificationContent>(CreateToastNotificationError());
                }
                else
                {
                    int selectedId;
                    bool parsingSuccessful = int.TryParse(SelectedIdMarginToFilterText, out selectedId);
                    if(parsingSuccessful)
                    {
                        FilterErrorMessage = String.Empty;

                        if(selectedId >= 0)
                        {
                            FilterErrorMessage = String.Empty;

                            for(int i=0;i<filteredList.Count;i++)
                            {
                                if (filteredList[i].Id <= selectedId)
                                {
                                    entitiesToDelete.Add(filteredList[i]);
                                }
                            }

                            foreach(Entity entity in entitiesToDelete)
                            {
                                filteredList.Remove(entity);
                            }
                        }
                        else
                        {
                            FilterErrorMessage = "Id can't be negative.";
                            Messenger.Default.Send<NotificationContent>(CreateToastNotificationError());
                        }
                    }
                    else
                    {
                        FilterErrorMessage = "Id must be an integer.";
                        Messenger.Default.Send<NotificationContent>(CreateToastNotificationError());
                    }
                }
            }
            else if(IsLessThanRadioButtonSelected)
            {
                List<Entity> entitiesToDelete = new List<Entity>();

                if(string.IsNullOrWhiteSpace(SelectedIdMarginToFilterText))
                {
                    FilterErrorMessage = "Id is required.";
                    Messenger.Default.Send<NotificationContent>(CreateToastNotificationError());
                }
                else
                {
                    int selectedId;
                    bool parsingSuccessful = int.TryParse(SelectedIdMarginToFilterText, out selectedId);

                    if(parsingSuccessful)
                    {
                        FilterErrorMessage = String.Empty;

                        if(selectedId >= 0)
                        {
                            FilterErrorMessage = String.Empty;
                            for(int i=0; i < filteredList.Count;i++)
                            {
                                if (filteredList[i].Id >= selectedId)
                                {
                                    entitiesToDelete.Add(filteredList[i]);
                                }
                            }

                            foreach(Entity entity in entitiesToDelete)
                            {
                                filteredList.Remove(entity);
                            }

                        }
                        else
                        {
                            FilterErrorMessage = "Id can't be negative.";
                            Messenger.Default.Send<NotificationContent>(CreateToastNotificationError());
                        }
                    }
                    else
                    {
                        FilterErrorMessage = "Id must be an integer.";
                        Messenger.Default.Send<NotificationContent>(CreateToastNotificationError());
                    }
                }
            }
            else if(IsEquallyRadioButtonSelected)
            {
                List<Entity> entitiesToDelete = new List<Entity>();

                if(string.IsNullOrWhiteSpace(SelectedIdMarginToFilterText))
                {
                    FilterErrorMessage = "Id is required.";
                    Messenger.Default.Send<NotificationContent>(CreateToastNotificationError());
                }
                else
                {
                    int selectedId;
                    bool parsingSuccessful = int.TryParse(SelectedIdMarginToFilterText, out selectedId);

                    if(parsingSuccessful)
                    {
                        FilterErrorMessage = String.Empty;

                        if(selectedId >= 0)
                        {
                            FilterErrorMessage = String.Empty;

                            for(int i=0;i<filteredList.Count;i++)
                            {
                                if (filteredList[i].Id > selectedId || filteredList[i].Id < selectedId)
                                {
                                    entitiesToDelete.Add(filteredList[i]);
                                }
                            }

                            foreach(Entity entity in entitiesToDelete)
                            {
                                filteredList.Remove(entity);
                            }
                        }
                        else
                        {
                            FilterErrorMessage = "Id can'be negative.";
                            Messenger.Default.Send<NotificationContent>(CreateToastNotificationError());
                        }
                    }
                    else
                    {
                        FilterErrorMessage = "Id must be an integer.";
                        Messenger.Default.Send<NotificationContent>(CreateToastNotificationError());
                    }
                }
            }
            else
            {
                if(!string.IsNullOrWhiteSpace(SelectedIdMarginToFilterText))
                {
                    FilterErrorMessage = "Selected '>' or '<' or '='";
                }
            }

            if(filteredList.Count > 0)
            {
                FilteredEntities = new ObservableCollection<Entity>();
                foreach(Entity entity in filteredList)
                {
                    FilteredEntities.Add(entity);
                }
                EntitiesToShow = FilteredEntities;
                OnPropertyChanged("EntitiesToShow");

            }
            else
            {
                FilterErrorMessage = "No entities to show.";
                EntitiesToShow = null;
                OnPropertyChanged("EntitiesToShow");
            }
        }

        private void OnResetFilter()
        {
            SelectedEntityTypeToFilter = null;
            IsGreaterThanRadioButtonSelected = false;
            IsLessThanRadioButtonSelected = false;
            IsEquallyRadioButtonSelected = false;
            SelectedIdMarginToFilterText = String.Empty;
            FilterErrorMessage = String.Empty;

            EntitiesToShow = Entities;
            FilteredEntities = new ObservableCollection<Entity>();
            OnPropertyChanged("EntitiesToShow");
        }

        private bool CanDelete()
        {
            return SelectedEntity != null;
        }

        private void OnDelete()
        {
            Entity deletedEntity = SelectedEntity;
            int ind = displayViewModel.FindIndexForId(deletedEntity.Id);

            AddToHistory(() =>
            {
                Entities.Add(deletedEntity);
                displayViewModel.AddEntityOnDisplay(deletedEntity, ind);                                
                RefreshTreeViewItems();
            });

            switch (SelectedEntity.TypeEntity.Name)
            {
                case "CableSensor":
                    cableSensorCount--;
                    break;
                case "DigitalManometer":
                    digitalManometerCount--;
                    break;
            }

            MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure you want to delete entity?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                Entity entity = SelectedEntity;
                Entities.Remove(entity);
                Messenger.Default.Send<NotificationContent>(CreateSuccessToastNotificationDelete());                
                displayViewModel.DeleteEntityOnDisplay(entity);
                foreach(Entity e in MainWindowViewModel.Entities.ToList())
                {
                    if(e.Id == entity.Id)
                    {
                        Entities.Remove(e);
                        break;
                    }
                }               
                RefreshTreeViewItems();
            }
            else
            {

            }

        }

       

        private void RefreshTreeViewItems()
        {
            SharedData.AllEntities.Clear();
            EntitiesByType cableSensor = new EntitiesByType();
            cableSensor.Type = new EntityType();
            cableSensor.Type.Name = "Cable sensor";

            foreach (Entity entity in Entities)
            {
                if (entity.TypeEntity.Name.Equals("Cable sensor"))
                {
                    cableSensor.Entities.Add(entity);
                }
            }
            SharedData.AllEntities.Add(cableSensor);


            EntitiesByType digitalManometer = new EntitiesByType();
            digitalManometer.Type = new EntityType();

            digitalManometer.Type.Name = "Digital manometer";

            foreach (Entity entity in Entities)
            {
                if (entity.TypeEntity.Name.Equals("Digital manometer"))
                {
                    digitalManometer.Entities.Add(entity);
                }
            }
            SharedData.AllEntities.Add(digitalManometer);
        }

      
        public void OnAdd()
        {
            try
            {
                int parsedId;
                bool canParse = int.TryParse(CurrentEntity.TextId, out parsedId);
                bool idAlreadyExists = false;

                if (canParse)
                {
                    foreach (Entity entity in Entities)
                    {
                        if (entity.Id == parsedId)
                        {
                            idAlreadyExists = true;
                            break;
                        }
                    }
                }

                CurrentEntity.DoesIdAlreadyExist = idAlreadyExists;

                CurrentEntity.Validate();
                CurrentEntityType.Validate();

                if (CurrentEntity.IsValid)
                {
                    Entities.Add(new Entity()
                    {
                        Id = int.Parse(CurrentEntity.TextId),
                        Name = CurrentEntity.Name,
                        TypeEntity = new EntityType
                        {
                            Name = CurrentEntityType.Name,
                            ImgSrc = CurrentEntityType.ImgSrc
                        },
                        
                        Value = 0
                    });

                    RefreshTreeViewItems();

                    lastAddedEntityId = int.Parse(CurrentEntity.TextId);

                    AddToHistory(() =>
                    {
                        
                        var entityToRemove = Entities.FirstOrDefault(e => e.Id == lastAddedEntityId);
                        if (entityToRemove != null)
                        {
                            Entities.Remove(entityToRemove);
                            displayViewModel.DeleteEntityOnDisplay(entityToRemove);
                            RefreshTreeViewItems();

                        }
                    });

                    MainWindowViewModel.Entities.Add(new Entity()
                    {
                        Id = int.Parse(CurrentEntity.TextId),
                        Name = CurrentEntity.Name,
                        TypeEntity = new EntityType
                        {
                            Name = CurrentEntityType.Name,
                            ImgSrc = CurrentEntityType.ImgSrc
                        },
                        Value = 0
                    });

                    switch (CurrentEntityType.Name)
                    {
                        case "CableSensor":
                            cableSensorCount++;
                            break;
                        case "DigitalManometer":
                            digitalManometerCount++;
                            break;
                    }
                  


                    Messenger.Default.Send<NotificationContent>(CreateSuccessToastNotification());
                    
                    CurrentEntity.TextId = String.Empty;
                    CurrentEntity.Name = String.Empty;
                }
                else
                {
                    Messenger.Default.Send<NotificationContent>(CreateToastNotificationError());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now} - {ex.Message}");
            }
        }
       

        public void OnAddConsole(string param)
        {
            try
            {
                Console.WriteLine($"Received param: {param}");

                string[] parts = param.Trim('(', ')').Split(',');

                if (parts.Length != 3)
                {
                    Console.WriteLine("Invalid parameter format.");
                    return;
                }

                int id;
                if (!int.TryParse(parts[0].Split('(')[1], out id))
                {
                    Console.WriteLine("Invalid ID format.");
                    Messenger.Default.Send<NotificationContent>(CreateToastNotificationError());

                    return;
                }

                

                string name = parts[1].Trim();
                string typeName = parts[2].Trim();

                string imgSrc; 

            
                bool idAlreadyExists = Entities.Any(e => e.Id == id);
                if (idAlreadyExists)
                {
                    Console.WriteLine("ID already exists.");
                    Messenger.Default.Send<NotificationContent>(CreateToastNotificationErrorIdConsole());
                    return;
                }

                if (Helpers.ComboBoxItems.entityTypes.TryGetValue(typeName, out imgSrc))
                {
                    
                    EntityType entityType = new EntityType { Name = typeName, ImgSrc = imgSrc };
                    Entity newEntity = new Entity { Id = id, Name = name, TypeEntity = entityType, Value = 0 };

                    

                    if(newEntity.Id < 0)
                    {

                        Messenger.Default.Send<NotificationContent>(CreateToastNotificationErrorId());
                    }
                    else
                    {
                        Entities.Add(newEntity);
                        lastAddedEntityId = id;
                        Messenger.Default.Send<NotificationContent>(CreateSuccessToastNotification());
                    }
        

                    AddToHistory(() =>
                    {
                        var entityToRemove = Entities.FirstOrDefault(e => e.Id == lastAddedEntityId);
                        if (entityToRemove != null)
                        {
                            Entities.Remove(entityToRemove);
                            displayViewModel.DeleteEntityOnDisplay(entityToRemove);
                            RefreshTreeViewItems();
                        }
                    });

                   MainWindowViewModel.Entities.Add(newEntity);
                   RefreshTreeViewItems();
                    

                    switch (typeName)
                    {
                        case "Cable sensor":
                            cableSensorCount++;
                            break;
                        case "Digital manometer":
                            digitalManometerCount++;
                            break;
                    }

                   

                }
                else
                {
                    Console.WriteLine("Invalid entity type.");
                    Messenger.Default.Send<NotificationContent>(CreateToastNotificationEntityType());
                    return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now} - {ex.Message}");
            }
        }
        
 
        public void OnFilterConsole(string param)
        {
            FilterErrorMessage = String.Empty;
            List<Entity> originalEntities = new List<Entity>(Entities);

            string idString = null;
            string criterion = null;
            string typeName = null;

            string[] parts = param.Split(new char[] {'(', ')'}, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 2)
            {
                Console.WriteLine("Invalid parameter format.");
                return;
            }

            if (parts[0].Equals("filter1"))
            {
                string[] parts2 = parts[1].Split(',');

                idString = parts2[0].Trim();
                criterion = parts2[1].Trim();
                typeName = parts2[2].Trim();
            }
            else if (parts[0].Equals("filter2"))
            {
                string[] parts2 = parts[1].Split(',');

                idString = parts2[0].Trim();
                criterion = parts2[1].Trim();
            }
            else
            {
                string[] parts2 = parts[1].Split(',');

                typeName = parts2[0].Trim();
            }

          

            AddToHistory(() =>
            {
                Entities.Clear();
                foreach (var entity in originalEntities)
                {
                    Entities.Add(entity);
                }
                EntitiesToShow = new ObservableCollection<Entity>(Entities);
                OnPropertyChanged("EntitiesToShow");
            });

            if (typeName == null && criterion == null)
            {
                FilterErrorMessage = "Fields can't be empty.";
                return;
            }

            List<Entity> filteredList = new List<Entity>();

            foreach (Entity entity in Entities)
            {
                filteredList.Add(entity);
            }

            if (typeName != null)
            {
                List<Entity> entitiesToDelete = new List<Entity>();
                for (int i = 0; i < filteredList.Count; i++)
                {
                    if (filteredList[i].TypeEntity.Name != typeName)
                    {

                        entitiesToDelete.Add(filteredList[i]);
                    }
                }

                foreach (Entity entity in entitiesToDelete)
                {
                    filteredList.Remove(entity);
                }
            }

            if (criterion == ">")
            {
                List<Entity> entitiesToDelete = new List<Entity>();

                if (string.IsNullOrWhiteSpace(idString))
                {
                    FilterErrorMessage = "Id is required.";
                    Messenger.Default.Send<NotificationContent>(CreateToastNotificationError());
                }
                else
                {
                    int selectedId;
                    bool parsingSuccessful = int.TryParse(idString, out selectedId);
                    if (parsingSuccessful)
                    {
                        FilterErrorMessage = String.Empty;

                        if (selectedId >= 0)
                        {
                            FilterErrorMessage = String.Empty;

                            for (int i = 0; i < filteredList.Count; i++)
                            {
                                if (filteredList[i].Id <= selectedId)
                                {
                                    entitiesToDelete.Add(filteredList[i]);
                                }
                            }

                            foreach (Entity entity in entitiesToDelete)
                            {
                                filteredList.Remove(entity);
                            }
                        }
                        else
                        {
                            FilterErrorMessage = "Id can't be negative.";
                            Messenger.Default.Send<NotificationContent>(CreateToastNotificationError());
                        }
                    }
                    else
                    {
                        FilterErrorMessage = "Id must be an integer.";
                        Messenger.Default.Send<NotificationContent>(CreateToastNotificationError());
                    }
                }
            }
            else if (criterion == "<")
            {
                List<Entity> entitiesToDelete = new List<Entity>();

                if (string.IsNullOrWhiteSpace(idString))
                {
                    FilterErrorMessage = "Id is required.";
                    Messenger.Default.Send<NotificationContent>(CreateToastNotificationError());
                }
                else
                {
                    int selectedId;
                    bool parsingSuccessful = int.TryParse(idString, out selectedId);

                    if (parsingSuccessful)
                    {
                        FilterErrorMessage = String.Empty;

                        if (selectedId >= 0)
                        {
                            FilterErrorMessage = String.Empty;
                            for (int i = 0; i < filteredList.Count; i++)
                            {
                                if (filteredList[i].Id >= selectedId)
                                {
                                    entitiesToDelete.Add(filteredList[i]);
                                }
                            }

                            foreach (Entity entity in entitiesToDelete)
                            {
                                filteredList.Remove(entity);
                            }

                        }
                        else
                        {
                            FilterErrorMessage = "Id can't be negative.";
                            Messenger.Default.Send<NotificationContent>(CreateToastNotificationError());
                        }
                    }
                    else
                    {
                        FilterErrorMessage = "Id must be an integer.";
                        Messenger.Default.Send<NotificationContent>(CreateToastNotificationError());
                    }
                }
            }
            else if (criterion == "=")
            {
                List<Entity> entitiesToDelete = new List<Entity>();

                if (string.IsNullOrWhiteSpace(idString))
                {
                    FilterErrorMessage = "Id is required.";
                    Messenger.Default.Send<NotificationContent>(CreateToastNotificationError());
                }
                else
                {
                    int selectedId;
                    bool parsingSuccessful = int.TryParse(idString, out selectedId);

                    if (parsingSuccessful)
                    {
                        FilterErrorMessage = String.Empty;

                        if (selectedId >= 0)
                        {
                            FilterErrorMessage = String.Empty;

                            for (int i = 0; i < filteredList.Count; i++)
                            {
                                if (filteredList[i].Id > selectedId || filteredList[i].Id < selectedId)
                                {
                                    entitiesToDelete.Add(filteredList[i]);
                                }
                            }

                            foreach (Entity entity in entitiesToDelete)
                            {
                                filteredList.Remove(entity);
                            }
                        }
                        else
                        {
                            FilterErrorMessage = "Id can'be negative.";
                            Messenger.Default.Send<NotificationContent>(CreateToastNotificationError());
                        }
                    }
                    else
                    {
                        FilterErrorMessage = "Id must be an integer.";
                        Messenger.Default.Send<NotificationContent>(CreateToastNotificationError());
                    }
                }
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(idString))
                {
                    FilterErrorMessage = "Selected '>' or '<' or '='";
                }
            }

            if (filteredList.Count > 0)
            {
                FilteredEntities = new ObservableCollection<Entity>();
                foreach (Entity entity in filteredList)
                {
                    FilteredEntities.Add(entity);
                }
                EntitiesToShow = FilteredEntities;
                OnPropertyChanged("EntitiesToShow");

            }
            else
            {
                FilterErrorMessage = "No entities to show.";
                EntitiesToShow = null;
                OnPropertyChanged("EntitiesToShow");
            }
        }

        public void OnDeleteConsole(string param)
        {
            try
            {
                
                if (int.TryParse(param, out int id))
                {
                    
                    Entity entityToDelete = Entities.FirstOrDefault(e => e.Id == id);

                    if(entityToDelete == null)
                    {
                        Messenger.Default.Send<NotificationContent>(CreateToastNotificationErrorDeleteConsole());
                        return;
                    }


                    int ind = displayViewModel.FindIndexForId(entityToDelete.Id);


                    if (entityToDelete != null)
                    {
                        
                        AddToHistory(() =>
                        {
                            Entities.Add(entityToDelete);
                            displayViewModel.AddEntityOnDisplay(entityToDelete, ind);
                            RefreshTreeViewItems();
                        });

                        
                        switch (entityToDelete.TypeEntity.Name)
                        {
                            case "CableSensor":
                                cableSensorCount--;
                                break;
                            case "DigitalManometer":
                                digitalManometerCount--;
                                break;
                        }
                        MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure you want to delete entity?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (messageBoxResult == MessageBoxResult.Yes)
                        {
                            Messenger.Default.Send<NotificationContent>(CreateSuccessToastNotificationDelete());
                            Entities.Remove(entityToDelete);
                            displayViewModel.DeleteEntityOnDisplay(entityToDelete);
                            RefreshTreeViewItems();
                        }
                        else
                        {

                        }

                    }
                    else
                    {
                        Console.WriteLine($"Entity with ID {id} not found.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid ID format.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now} - {ex.Message}");
            }
        }

        public void OnUndo(string s)
        {
            try
            {
               
                if (actionHistory.Count > 0)
                {
                    
                    Action lastAction = actionHistory.Pop();
                    lastAction();
                }
                else
                {
                    Console.WriteLine("There are no undo actions available");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now} - {ex.Message}");
            }
        }
        private NotificationContent CreateSuccessToastNotification()
        {
            var notificationContent = new NotificationContent
            {
                Title = "Success",
                Message = "Entity successfully added.",
                Type = NotificationType.Success,
                TrimType = NotificationTextTrimType.AttachIfMoreRows, 
                RowsCount = 2, 
              
                CloseOnClick = true, 

                Background = new SolidColorBrush(Colors.LimeGreen),
                Foreground = new SolidColorBrush(Colors.White),

              
            };

            return notificationContent;
        }

        private NotificationContent CreateSuccessToastNotificationDelete()
        {
            var notificationContent = new NotificationContent
            {
                Title = "Success",
                Message = "Entity successfully deleted.",
                Type = NotificationType.Success,
                TrimType = NotificationTextTrimType.AttachIfMoreRows, 
                RowsCount = 2, 

                Background = new SolidColorBrush(Colors.LimeGreen),
                Foreground = new SolidColorBrush(Colors.White),

                
            };

            return notificationContent;
        }

        private NotificationContent CreateToastNotificationError()
        {
            var notificationContent = new NotificationContent
            {
                Title = "Error",
                Message = "Some field is not filled in correctly.",
                Type = NotificationType.Error,
                TrimType = NotificationTextTrimType.AttachIfMoreRows, 
                RowsCount = 2, 

                Background = new SolidColorBrush(Colors.Red),
                Foreground = new SolidColorBrush(Colors.White),


            };

            return notificationContent;
        }

        private NotificationContent CreateToastNotificationErrorId()
        {
            var notificationContent = new NotificationContent
            {
                Title = "Error",
                Message = "Id can't be negative.",
                Type = NotificationType.Error,
                TrimType = NotificationTextTrimType.AttachIfMoreRows,
                RowsCount = 2,

                Background = new SolidColorBrush(Colors.Red),
                Foreground = new SolidColorBrush(Colors.White),


            };

            return notificationContent;
        }

        private NotificationContent CreateToastNotificationErrorIdConsole()
        {
            var notificationContent = new NotificationContent
            {
                Title = "Error",
                Message = "Id already exists.",
                Type = NotificationType.Error,
                TrimType = NotificationTextTrimType.AttachIfMoreRows,
                RowsCount = 2,

                Background = new SolidColorBrush(Colors.Red),
                Foreground = new SolidColorBrush(Colors.White),


            };

            return notificationContent;
        }

        private NotificationContent CreateToastNotificationEntityType()
        {
            var notificationContent = new NotificationContent
            {
                Title = "Error",
                Message = "Invalid entity type.",
                Type = NotificationType.Error,
                TrimType = NotificationTextTrimType.AttachIfMoreRows,
                RowsCount = 2,

                Background = new SolidColorBrush(Colors.Red),
                Foreground = new SolidColorBrush(Colors.White),


            };

            return notificationContent;
        }

        private NotificationContent CreateToastNotificationErrorDeleteConsole()
        {
            var notificationContent = new NotificationContent
            {
                Title = "Error",
                Message = "Entity with this id not found",
                Type = NotificationType.Error,
                TrimType = NotificationTextTrimType.AttachIfMoreRows,
                RowsCount = 2,

                Background = new SolidColorBrush(Colors.Red),
                Foreground = new SolidColorBrush(Colors.White),


            };

            return notificationContent;
        }

    }
}
