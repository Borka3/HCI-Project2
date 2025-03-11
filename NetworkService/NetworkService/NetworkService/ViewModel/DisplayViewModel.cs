using NetworkService.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using NetworkService.Model;
using System.Reflection;
using System.Windows.Documents;

namespace NetworkService.ViewModel
{
    public class DisplayViewModel : BindableBase
    {
        private Stack<Action> actionHistory = new Stack<Action>();

        public BindingList<Entity> EntitiesInList { get; set; }

        public ObservableCollection<MyLine> LineCollection { get; set; }
        public ObservableCollection<Canvas> CanvasCollection { get; set; }

        public ObservableCollection<string> CollectionDescription { get; set; }
        public ObservableCollection<Brush> BorderBrushCollection { get; set; }

        private Entity selectedEntity;

        private Entity draggedItem = null;
        private bool dragging = false;
        public int draggingSourceIndex = -1;
        private int previousIndex = -1;

        public MyCommandWithParam<object> DropEntityOnCanvas { get; set; }
        public MyCommandWithParam<string> LeftMouseButtonDownOnCanvas { get; set; }
        public MyICommand MouseLeftButtonUp { get; set; }
        public MyCommandWithParam<TreeView> SelectionChanged { get; set; }
        public MyCommandWithParam<object> FreeCanvas { get; set; }
        public MyICommand<object> RightMouseButtonDownOnCanvas { get; set; }

        public ObservableCollection<EntitiesByType> AllEntities { get; set; }
        public static MyCommandWithParam<string> UndoCommand { get; set; }



        private bool isLineSourceSelected = false;
        private int sourceCanvasIndex = -1;
        private int destinationCanvasIndex = -1;
        private MyLine currentLine = new MyLine();
        private Point linePoint1 = new Point();
        private Point linePoint2 = new Point();

        public DisplayViewModel(ObservableCollection<Entity> entities)
        {
            EntitiesInList = new BindingList<Entity>();
            CollectionDescription = new ObservableCollection<string>();
            CanvasCollection = new ObservableCollection<Canvas>();
            BorderBrushCollection = new ObservableCollection<Brush>();
            for (int i = 0; i < 12; i++)
            {
                CanvasCollection.Add(new Canvas()
                {

                    AllowDrop = true
                });
                CollectionDescription.Add(" ");
            }

            for (int i = 0; i < 12; i++)
            {
                BorderBrushCollection.Add(Brushes.DarkGray);
            }

            AllEntities = new ObservableCollection<EntitiesByType>();
            RefreshTreeViewItems(entities);
            SharedData.AllEntities = AllEntities;

            LineCollection = new ObservableCollection<MyLine>();


            DropEntityOnCanvas = new MyCommandWithParam<object>(OnDrop);
            LeftMouseButtonDownOnCanvas = new MyCommandWithParam<string>(OnLeftMouseButtonDown);
            MouseLeftButtonUp = new MyICommand(OnMouseLeftButtonUp);
            SelectionChanged = new MyCommandWithParam<TreeView>(OnSelectionChanged);
            FreeCanvas = new MyCommandWithParam<object>(OnFreeCanvas);
            RightMouseButtonDownOnCanvas = new MyICommand<object>(OnRightMouseButtonDown);
            

        }

        private void RefreshTreeViewItems(ObservableCollection<Entity> entities)
        {
            EntitiesByType cableSensor = new EntitiesByType();
            EntityType cableType = new EntityType();
            cableSensor.Type = cableType;
            cableSensor.Type.Name = "Cable sensor";

            EntitiesByType digitalManometer = new EntitiesByType();            
            EntityType manometerType = new EntityType();            
            digitalManometer.Type = manometerType;

            digitalManometer.Type.Name = "Digital manometer";
            AllEntities.Add(cableSensor);
            AllEntities.Add(digitalManometer);


            foreach (Entity entity in entities)
            {
                if (!Refresh(entity.Id))
                {
                    if (entity.TypeEntity.Name.Equals("Cable sensor"))
                    {
                        cableSensor.Entities.Add(entity);
                    }
                    else
                    {
                        digitalManometer.Entities.Add(entity);
                    }
                    

                }
            }

        }

        private bool Refresh(int id)
        {
            foreach(Canvas c in CanvasCollection)
            {
                if(((Entity)c.Resources["data"]).Id == id)
                {
                    return true;

                }
            }

            return false;
        }

        private void SaveCurrentState()
        {
            var currentState = CanvasCollection.Select(c => new
            {
                Background = c.Background,
                Taken = c.Resources["taken"],
                Data = c.Resources["data"]
            }).ToList();

            var currentLines = LineCollection.Select(l => new MyLine
            {
                X1 = l.X1,
                Y1 = l.Y1,
                X2 = l.X2,
                Y2 = l.Y2,
                Source = l.Source,
                Destination = l.Destination
            }).ToList();

            var currentDescriptions = CollectionDescription.ToList();
            var currentBorderBrushes = BorderBrushCollection.ToList();

            actionHistory.Push(() =>
            {
                for (int i = 0; i < CanvasCollection.Count; i++)
                {
                    CanvasCollection[i].Background = currentState[i].Background;
                    if (currentState[i].Taken == null)
                    {
                        CanvasCollection[i].Resources.Remove("taken");
                        CanvasCollection[i].Resources.Remove("data");
                    }
                    else
                    {
                        CanvasCollection[i].Resources["taken"] = currentState[i].Taken;
                        CanvasCollection[i].Resources["data"] = currentState[i].Data;
                    }
                    CollectionDescription[i] = currentDescriptions[i];
                    BorderBrushCollection[i] = currentBorderBrushes[i];
                }

                LineCollection.Clear();
                foreach (var line in currentLines)
                {
                    LineCollection.Add(line);
                }
            });
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
        

        private void OnDrop(object parameter)
        {
            if (draggedItem != null)
            {
                SaveCurrentState();
                int index = Convert.ToInt32(parameter);
                BorderBrushCollection[index] = (draggedItem.IsValidValue()) ? Brushes.Green : Brushes.Red;

                if (CanvasCollection[index].Resources["taken"] == null)
                {
                    
                    if (previousIndex != -1)
                    {
                        
                        BitmapImage logo = new BitmapImage();
                        logo.BeginInit();
                        logo.UriSource = new Uri(draggedItem.TypeEntity.ImgSrc, UriKind.RelativeOrAbsolute);
                        logo.EndInit();

                        CanvasCollection[index].Background = new ImageBrush(logo);
                        CanvasCollection[index].Resources.Add("taken", true);
                        CanvasCollection[index].Resources.Add("data", draggedItem);
                        BorderBrushCollection[index] = (draggedItem.IsValidValue()) ? Brushes.Green : Brushes.Red;
                        CollectionDescription[index] = ($"Id: {draggedItem.Id} Value: {draggedItem.Value}");
                        OnFreeCanvas(previousIndex);

                        UpdateLinesForCanvas(previousIndex, index);
                       

                        if (sourceCanvasIndex != -1)
                        {
                            isLineSourceSelected = false;
                            sourceCanvasIndex = -1;
                            linePoint1 = new Point();
                            linePoint2 = new Point();
                            currentLine = new MyLine();
                        }
                        draggingSourceIndex = -1;
                    }
                    else
                    {
                        BitmapImage logo = new BitmapImage();
                        logo.BeginInit();
                        logo.UriSource = new Uri(draggedItem.TypeEntity.ImgSrc, UriKind.RelativeOrAbsolute);
                        logo.EndInit();

                        CanvasCollection[index].Background = new ImageBrush(logo);
                        CanvasCollection[index].Resources.Add("taken", true);
                        CanvasCollection[index].Resources.Add("data", draggedItem);
                        BorderBrushCollection[index] = (draggedItem.IsValidValue()) ? Brushes.Green : Brushes.Red;
                        CollectionDescription[index] = ($"Id: {draggedItem.Id} Value: {draggedItem.Value}");
                        RemoveFromGroupedEntities(draggedItem);
                      
                    }   

                }
                ResetDragState();
            }


        }

        public int FindIndexForId(int id)
        {
            foreach(Canvas canvas in CanvasCollection)
            {
                if (canvas.Resources["data"] == null)
                    continue;

                if ( ((Entity)canvas.Resources["data"]).Id == id)
                {
                   return  CanvasCollection.IndexOf(canvas);
                }
            }
            return -1;
        }

        
        public void AddEntityOnDisplay(Entity entity, int ind)
        {
            if(ind == -1)
            {
                return;
            }
            BitmapImage logo = new BitmapImage();
            logo.BeginInit();
            logo.UriSource = new Uri(entity.TypeEntity.ImgSrc, UriKind.RelativeOrAbsolute);
            logo.EndInit();

            CanvasCollection[ind].Background = new ImageBrush(logo);
            CanvasCollection[ind].Resources.Add("taken", true);
            CanvasCollection[ind].Resources.Add("data", entity);
            BorderBrushCollection[ind] = (entity.IsValidValue()) ? Brushes.Green : Brushes.Red;
            CollectionDescription[ind] = ($"Id: {entity.Id} Value: {entity.Value}");
            OnFreeCanvas(previousIndex);

        }

        public void DeleteEntityOnDisplay(Entity entity)
        {
            for (int i = 0; i < CanvasCollection.Count; i++)
            {
                if (CanvasCollection[i].Resources["data"] as Entity == entity)
                {
                    OnFreeCanvas((object)i);
                    return;
                }
            }
        }



        private void AddEntity(Entity item)
        {
            if (item.TypeEntity.Name.Equals("Digital manometer"))
            {
                EntitiesByType te = AllEntities[1];
                te.Entities.Add(item);
            }
            else
            {
                EntitiesByType te = AllEntities[0];
                te.Entities.Add(item);
            }
        }

        public void RemoveFromGroupedEntities(Entity item)
        {
            if (item.TypeEntity.Name.Equals("Digital manometer"))
            {
                EntitiesByType te = AllEntities[1];
                te.Entities.Remove(item);
            }
            else
            {
                EntitiesByType te = AllEntities[0];
                te.Entities.Remove(item);
            }
        }

        private void ResetDragState()
        {
            draggedItem = null;
            SelectedEntity = null;
            dragging = false;
            draggingSourceIndex = -1;
            previousIndex = -1;
        }

        private void OnSelectionChanged(TreeView treeView)
        {
            if (treeView.SelectedItem != null && !dragging && treeView.SelectedItem.GetType() != typeof(EntitiesByType))
            {
                dragging = true;
                draggedItem = (Entity)treeView.SelectedItem;
                draggingSourceIndex = FindIndexOfSelectedEnt(draggedItem);
                DragDrop.DoDragDrop(treeView, draggedItem, DragDropEffects.Move | DragDropEffects.Copy | DragDropEffects.None);
            }


        }

        private int FindIndexOfSelectedEnt(Entity draggedItem)
        {
            int index = -1;
            if (draggedItem != null && draggedItem.TypeEntity.Name.Equals("Cable sensor"))
            {
                index = AllEntities[0].Entities.IndexOf(draggedItem);
            }
            else
            {
                index = AllEntities[1].Entities.IndexOf(draggedItem);
            }
            return index;

        }

        private void OnFreeCanvas(object parameter)
        {
            SaveCurrentState();
            int index = Convert.ToInt32(parameter);

            if (CanvasCollection[index].Resources["taken"] != null)
            {

                if (sourceCanvasIndex != -1)
                {

                    isLineSourceSelected = false;
                    sourceCanvasIndex = -1;
                    linePoint1 = new Point();
                    linePoint2 = new Point();
                    currentLine = new MyLine();

                }
                DeleteLinesForCanvas(index);

                EntitiesInList.Add((Entity)CanvasCollection[index].Resources["data"]);
                
              
                CanvasCollection[index].Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#CAE6B2"));
                CanvasCollection[index].Resources.Remove("taken");
                if (previousIndex == -1)
                {
                    Entity item = (CanvasCollection[index].Resources["data"]) as Entity;
                    AddEntity(item);
                }
                CanvasCollection[index].Resources.Remove("data");
                BorderBrushCollection[index] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#CAE6B2"));
                CollectionDescription[index] = ($" ");
            }
        }

        public void OnUndo()
        {
            if (actionHistory.Count > 0)
            {
                var lastAction = actionHistory.Pop();
                lastAction();
            }
            else
            {
                Console.WriteLine("There are no undo actions available");
            }
        }

        public void DeleteEntityFromCanvas(Entity entity)
        {
            int canvasIndex = GetCanvasIndexForEntityId(entity.Id);

            if (canvasIndex != -1)
            {
                CanvasCollection[canvasIndex].Background = Brushes.LightGray;
                CanvasCollection[canvasIndex].Resources.Remove("taken");
                CanvasCollection[canvasIndex].Resources.Remove("data");
                BorderBrushCollection[canvasIndex] = Brushes.DarkGray;
                CollectionDescription[canvasIndex] = ($" ");

                DeleteLinesForCanvas(canvasIndex);
            }
        }

        public void UpdateEntityOnCanvas(Entity entity)
        {
            int canvasIndex = GetCanvasIndexForEntityId(entity.Id);

            if (canvasIndex != -1)
            {
                CollectionDescription[canvasIndex] = ($"Id: {entity.Id} Value: {entity.Value}");
                if (entity.IsValidValue())
                {
                    BorderBrushCollection[canvasIndex] = Brushes.Green;
                }
                else
                {
                    BorderBrushCollection[canvasIndex] = Brushes.Red;
                }
            }
        }
        private void OnRightMouseButtonDown(object parameter)
        {
            int index = Convert.ToInt32(parameter);

            if (CanvasCollection[index].Resources["taken"] != null)
            {
                if (!isLineSourceSelected)
                {
                    sourceCanvasIndex = index;

                    linePoint1 = GetPointForCanvasIndex(sourceCanvasIndex);

                    currentLine.X1 = linePoint1.X;
                    currentLine.Y1 = linePoint1.Y;
                    currentLine.Source = sourceCanvasIndex;

                    isLineSourceSelected = true;
                }
                else
                {
                    destinationCanvasIndex = index;

                    if ((sourceCanvasIndex != destinationCanvasIndex) && !DoesLineAlreadyExist(sourceCanvasIndex, destinationCanvasIndex))
                    {
                        linePoint2 = GetPointForCanvasIndex(destinationCanvasIndex);

                        currentLine.X2 = linePoint2.X;
                        currentLine.Y2 = linePoint2.Y;
                        currentLine.Destination = destinationCanvasIndex;

                        LineCollection.Add(new MyLine
                        {
                            X1 = currentLine.X1,
                            Y1 = currentLine.Y1,
                            X2 = currentLine.X2,
                            Y2 = currentLine.Y2,
                            Source = currentLine.Source,
                            Destination = currentLine.Destination
                        });

                        isLineSourceSelected = false;

                        linePoint1 = new Point();
                        linePoint2 = new Point();
                        currentLine = new MyLine();
                    }
                    else
                    {
                        

                        isLineSourceSelected = false;

                        linePoint1 = new Point();
                        linePoint2 = new Point();
                        currentLine = new MyLine();
                    }
                }
            }
            else
            {
              

                isLineSourceSelected = false;

                linePoint1 = new Point();
                linePoint2 = new Point();
                currentLine = new MyLine();
            }
        }

        private void OnLeftMouseButtonDown(string param)
        {

            if (CanvasCollection[int.Parse(param)].Resources["taken"] != null)
            {
                dragging = true;
                draggedItem = (CanvasCollection[int.Parse(param)].Resources["data"]) as Entity;
                previousIndex = int.Parse(param);
                DragDrop.DoDragDrop(CanvasCollection[int.Parse(param)], draggedItem, DragDropEffects.Move | DragDropEffects.Copy | DragDropEffects.None);
            }
        }

        private void OnMouseLeftButtonUp()
        {
            draggedItem = null;
            SelectedEntity = null;
            dragging = false;
            draggingSourceIndex = -1;
        }

        private void UpdateLinesForCanvas(int sourceCanvas, int destinationCanvas)
        {
             for (int i = 0; i < LineCollection.Count; i++)
             {
                 if (LineCollection[i].Source == sourceCanvas)
                 {
                     Point newSourcePoint = GetPointForCanvasIndex(destinationCanvas);
                     LineCollection[i].X1 = newSourcePoint.X;
                     LineCollection[i].Y1 = newSourcePoint.Y;
                     LineCollection[i].Source = destinationCanvas;
                 }
                 else if (LineCollection[i].Destination == sourceCanvas)
                 {
                     Point newDestinationPoint = GetPointForCanvasIndex(destinationCanvas);
                     LineCollection[i].X2 = newDestinationPoint.X;
                     LineCollection[i].Y2 = newDestinationPoint.Y;
                     LineCollection[i].Destination = destinationCanvas;
                 }
             }
             foreach (var canvas in CanvasCollection)
             {
                 canvas.InvalidateVisual();
             }

        }

        public int GetCanvasIndexForEntityId(int entityId)
        {

            for (int i = 0; i < CanvasCollection.Count; i++)
            {
                Entity entity = (CanvasCollection[i].Resources["data"]) as Entity;

                if ((entity != null) && (entity.Id == entityId))
                {
                    return i;
                }
            }
            return -1;

        }

        private bool DoesLineAlreadyExist(int source, int destination)
        {
            foreach (MyLine line in LineCollection)
            {
                if ((line.Source == source) && (line.Destination == destination))
                {
                    return true;
                }
                if ((line.Source == destination) && (line.Destination == source))
                {
                    return true;
                }
            }
            return false;
        }

        private void DeleteLinesForCanvas(int canvasIndex)
        {
            List<MyLine> linesToDelete = new List<MyLine>();

            for (int i = 0; i < LineCollection.Count; i++)
            {
                if ((LineCollection[i].Source == canvasIndex) || (LineCollection[i].Destination == canvasIndex))
                {
                    linesToDelete.Add(LineCollection[i]);
                }
            }

            foreach (MyLine line in linesToDelete)
            {
                LineCollection.Remove(line);
            }
        }

        private Point GetPointForCanvasIndex(int canvasIndex)
        {
            double x = 0, y = 0;

            for (int row = 0; row <= 3; row++)
            {
                for (int col = 0; col <= 2; col++)
                {
                    int currentIndex = row * 3 + col;

                    if (canvasIndex == currentIndex)
                    {
                        x = 44 + (col * 88);
                        y = 44 + (row * 88) - 176;
                        break;
                    }
                }
            }
            return new Point(x, y);
        }

        
        




    }
}
