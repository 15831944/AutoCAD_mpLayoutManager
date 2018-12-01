using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace mpLayoutManager
{
    public class ListViewDragDropManager<ItemType>
    where ItemType : class
    {
        private bool canInitiateDrag;

        private DragAdorner dragAdorner;

        private double dragAdornerOpacity;

        private int indexToSelect;

        private bool isDragInProgress;

        private ItemType itemUnderDragCursor;

        private ListView listView;

        private Point ptMouseDown;

        private bool showDragAdorner;

        private bool CanStartDragOperation
        {
            get
            {
                bool flag;
                if (Mouse.LeftButton != MouseButtonState.Pressed)
                {
                    flag = false;
                }
                else if (!canInitiateDrag)
                {
                    flag = false;
                }
                else if (indexToSelect != -1)
                {
                    flag = HasCursorLeftDragThreshold;
                }
                else
                {
                    flag = false;
                }
                return flag;
            }
        }

        public double DragAdornerOpacity
        {
            get => dragAdornerOpacity;
            set
            {
                if (IsDragInProgress)
                {
                    throw new InvalidOperationException("Cannot set the DragAdornerOpacity property during a drag operation.");
                }
                if (value < 0 || value > 1)
                {
                    throw new ArgumentOutOfRangeException("DragAdornerOpacity", value, "Must be between 0 and 1.");
                }
                dragAdornerOpacity = value;
            }
        }

        private bool HasCursorLeftDragThreshold
        {
            get
            {
                bool flag;
                if (indexToSelect >= 0)
                {
                    ListViewItem listViewItem = GetListViewItem(indexToSelect);
                    if (listViewItem != null)
                    {
                        try
                        {
                            Rect descendantBounds = VisualTreeHelper.GetDescendantBounds(listViewItem);
                            Point point = listView.TranslatePoint(ptMouseDown, listViewItem);
                            double num = Math.Abs(point.Y);
                            double num1 = Math.Abs(descendantBounds.Height - point.Y);
                            double num2 = Math.Min(num, num1);
                            double minimumHorizontalDragDistance = SystemParameters.MinimumHorizontalDragDistance * 2;
                            double num3 = Math.Min(SystemParameters.MinimumVerticalDragDistance, num2) * 2;
                            Size size = new Size(minimumHorizontalDragDistance, num3);
                            Rect rect = new Rect(ptMouseDown, size);
                            rect.Offset(size.Width / -2, size.Height / -2);
                            Point mousePosition = MouseUtilities.GetMousePosition(listView);
                            flag = !rect.Contains(mousePosition);
                        }
                        catch 
                        {
                            flag = false;
                        }
                    }
                    else
                    {
                        flag = false;
                    }
                }
                else
                {
                    flag = false;
                }
                return flag;
            }
        }

        private int IndexUnderDragCursor
        {
            get
            {
                int num = -1;
                int num1 = 0;
                while (num1 < listView.Items.Count)
                {
                    if (!IsMouseOver(GetListViewItem(num1)))
                    {
                        num1++;
                    }
                    else
                    {
                        num = num1;
                        break;
                    }
                }
                return num;
            }
        }

        public bool IsDragInProgress
        {
            get => isDragInProgress;
            private set => isDragInProgress = value;
        }

        private bool IsMouseOverScrollbar
        {
            get
            {
                bool flag;
                Point mousePosition = MouseUtilities.GetMousePosition(listView);
                HitTestResult hitTestResult = VisualTreeHelper.HitTest(listView, mousePosition);
                if (hitTestResult != null)
                {
                    DependencyObject visualHit = hitTestResult.VisualHit;
                    while (visualHit != null)
                    {
                        if (!(visualHit is ScrollBar))
                        {
                            visualHit = ((visualHit is Visual ? false : !(visualHit is Visual3D)) ? LogicalTreeHelper.GetParent(visualHit) : VisualTreeHelper.GetParent(visualHit));
                        }
                        else
                        {
                            flag = true;
                            return flag;
                        }
                    }
                    flag = false;
                }
                else
                {
                    flag = false;
                }
                return flag;
            }
        }

        private ItemType ItemUnderDragCursor
        {
            get => itemUnderDragCursor;
            set
            {
                if ((object)itemUnderDragCursor != (object)value)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        if (i == 1)
                        {
                            itemUnderDragCursor = value;
                        }
                        if (itemUnderDragCursor != null)
                        {
                            ListViewItem listViewItem = GetListViewItem(itemUnderDragCursor);
                            if (listViewItem != null)
                            {
                                ListViewItemDragState.SetIsUnderDragCursor(listViewItem, i == 1);
                            }
                        }
                    }
                }
            }
        }

        public ListView ListView
        {
            get
            {
                return listView;
            }
            set
            {
                if (IsDragInProgress)
                {
                    throw new InvalidOperationException("Cannot set the ListView property during a drag operation.");
                }
                if (listView != null)
                {
                    listView.PreviewMouseLeftButtonDown -= new MouseButtonEventHandler(listView_PreviewMouseLeftButtonDown);
                    listView.PreviewMouseMove -= new MouseEventHandler(listView_PreviewMouseMove);
                    listView.DragOver -= new DragEventHandler(listView_DragOver);
                    listView.DragLeave -= new DragEventHandler(listView_DragLeave);
                    listView.DragEnter -= new DragEventHandler(listView_DragEnter);
                    listView.Drop -= new DragEventHandler(listView_Drop);
                }
                listView = value;
                if (listView != null)
                {
                    if (!listView.AllowDrop)
                    {
                        listView.AllowDrop = true;
                    }
                    listView.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(listView_PreviewMouseLeftButtonDown);
                    listView.PreviewMouseMove += new MouseEventHandler(listView_PreviewMouseMove);
                    listView.DragOver += new DragEventHandler(listView_DragOver);
                    listView.DragLeave += new DragEventHandler(listView_DragLeave);
                    listView.DragEnter += new DragEventHandler(listView_DragEnter);
                    listView.Drop += new DragEventHandler(listView_Drop);
                }
            }
        }

        public bool ShowDragAdorner
        {
            get
            {
                return showDragAdorner;
            }
            set
            {
                if (IsDragInProgress)
                {
                    throw new InvalidOperationException("Cannot set the ShowDragAdorner property during a drag operation.");
                }
                showDragAdorner = value;
            }
        }

        private bool ShowDragAdornerResolved => ShowDragAdorner && DragAdornerOpacity > 0;

        public ListViewDragDropManager()
        {
            canInitiateDrag = false;
            dragAdornerOpacity = 0.7;
            indexToSelect = -1;
            showDragAdorner = true;
        }

        public ListViewDragDropManager(ListView listView) : this()
        {
            ListView = listView;
        }

        public ListViewDragDropManager(ListView listView, double dragAdornerOpacity) : this(listView)
        {
            DragAdornerOpacity = dragAdornerOpacity;
        }

        public ListViewDragDropManager(ListView listView, bool showDragAdorner) : this(listView)
        {
            ShowDragAdorner = showDragAdorner;
        }

        private void FinishDragOperation(ListViewItem draggedItem, AdornerLayer adornerLayer)
        {
            ListViewItemDragState.SetIsBeingDragged(draggedItem, false);
            IsDragInProgress = false;
            if (ItemUnderDragCursor != null)
            {
                ItemUnderDragCursor = default(ItemType);
            }
            if (adornerLayer != null)
            {
                adornerLayer.Remove(dragAdorner);
                dragAdorner = null;
            }
        }

        private ListViewItem GetListViewItem(int index)
        {
            ListViewItem listViewItem;
            if (listView.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
            {
                listViewItem = listView.ItemContainerGenerator.ContainerFromIndex(index) as ListViewItem;
            }
            else
            {
                listViewItem = null;
            }
            return listViewItem;
        }

        private ListViewItem GetListViewItem(ItemType dataItem)
        {
            ListViewItem listViewItem;
            if (listView.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
            {
                listViewItem = listView.ItemContainerGenerator.ContainerFromItem(dataItem) as ListViewItem;
            }
            else
            {
                listViewItem = null;
            }
            return listViewItem;
        }

        private AdornerLayer InitializeAdornerLayer(ListViewItem itemToDrag)
        {
            VisualBrush visualBrush = new VisualBrush(itemToDrag);
            dragAdorner = new DragAdorner(listView, itemToDrag.RenderSize, visualBrush)
            {
                Opacity = DragAdornerOpacity
            };
            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(listView);
            adornerLayer.Add(dragAdorner);
            ptMouseDown = MouseUtilities.GetMousePosition(listView);
            return adornerLayer;
        }

        private void InitializeDragOperation(ListViewItem itemToDrag)
        {
            IsDragInProgress = true;
            canInitiateDrag = false;
            ListViewItemDragState.SetIsBeingDragged(itemToDrag, true);
        }

        private bool IsMouseOver(Visual target)
        {
            Rect descendantBounds = VisualTreeHelper.GetDescendantBounds(target);
            return descendantBounds.Contains(MouseUtilities.GetMousePosition(target));
        }

        private void listView_DragEnter(object sender, DragEventArgs e)
        {
            if ((dragAdorner == null ? false : dragAdorner.Visibility != Visibility.Visible))
            {
                UpdateDragAdornerLocation();
                dragAdorner.Visibility = Visibility.Visible;
            }
        }

        private void listView_DragLeave(object sender, DragEventArgs e)
        {
            if (!IsMouseOver(listView))
            {
                if (ItemUnderDragCursor != null)
                {
                    ItemUnderDragCursor = default(ItemType);
                }
                if (dragAdorner != null)
                {
                    dragAdorner.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void listView_DragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Move;
            if (ShowDragAdornerResolved)
            {
                UpdateDragAdornerLocation();
            }
            int indexUnderDragCursor = IndexUnderDragCursor;
            ItemUnderDragCursor = (indexUnderDragCursor < 0 ? default(ItemType) : (ItemType)(ListView.Items[indexUnderDragCursor] as ItemType));
        }

        private void listView_Drop(object sender, DragEventArgs e)
        {
            ObservableCollection<ItemType> itemsSource;
            int indexUnderDragCursor;
            if (ItemUnderDragCursor != null)
            {
                ItemUnderDragCursor = default(ItemType);
            }
            e.Effects = DragDropEffects.None;
            if (e.Data.GetDataPresent(typeof(ItemType)))
            {
                ItemType data = (ItemType)(e.Data.GetData(typeof(ItemType)) as ItemType);
                if (data != null)
                {
                    itemsSource = listView.ItemsSource as ObservableCollection<ItemType>;
                    if (itemsSource == null)
                    {
                        throw new Exception("A ListView managed by ListViewDragManager must have its ItemsSource set to an ObservableCollection<ItemType>.");
                    }
                    int num = itemsSource.IndexOf(data);
                    indexUnderDragCursor = IndexUnderDragCursor;
                    if (indexUnderDragCursor < 0)
                    {
                        if (itemsSource.Count != 0)
                        {
                            if (num < 0)
                            {
                                indexUnderDragCursor = itemsSource.Count;
                            }
                            return;
                        }
                        else
                        {
                            indexUnderDragCursor = 0;
                        }
                    }
                    //Label2:
                    if (num != indexUnderDragCursor)
                    {
                        if (ProcessDrop == null)
                        {
                            if (num <= -1)
                            {
                                itemsSource.Insert(indexUnderDragCursor, data);
                            }
                            else
                            {
                                itemsSource.Move(num, indexUnderDragCursor);
                            }
                            e.Effects = DragDropEffects.Move;
                        }
                        else
                        {
                            ProcessDropEventArgs<ItemType> processDropEventArg = new ProcessDropEventArgs<ItemType>(itemsSource, data, num, indexUnderDragCursor, e.AllowedEffects);
                            ProcessDrop(this, processDropEventArg);
                            e.Effects = processDropEventArg.Effects;
                        }
                    }
                }
            }
            //return;
            //Label1:
            //indexUnderDragCursor = itemsSource.Count;
            //goto Label2;
        }

        private void listView_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!IsMouseOverScrollbar)
            {
                int indexUnderDragCursor = IndexUnderDragCursor;
                canInitiateDrag = indexUnderDragCursor > -1;
                if (!canInitiateDrag)
                {
                    ptMouseDown = new Point(-10000, -10000);
                    indexToSelect = -1;
                }
                else
                {
                    ptMouseDown = MouseUtilities.GetMousePosition(listView);
                    indexToSelect = indexUnderDragCursor;
                }
            }
            else
            {
                canInitiateDrag = false;
            }
        }

        private void listView_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            AdornerLayer adornerLayer;
            if (CanStartDragOperation)
            {
                if (listView.SelectedIndex != indexToSelect)
                {
                    listView.SelectedIndex = indexToSelect;
                }
                if (listView.SelectedItem != null)
                {
                    ListViewItem listViewItem = GetListViewItem(listView.SelectedIndex);
                    if (listViewItem != null)
                    {
                        if (ShowDragAdornerResolved)
                        {
                            adornerLayer = InitializeAdornerLayer(listViewItem);
                        }
                        else
                        {
                            adornerLayer = null;
                        }
                        AdornerLayer adornerLayer1 = adornerLayer;
                        InitializeDragOperation(listViewItem);
                        PerformDragOperation();
                        FinishDragOperation(listViewItem, adornerLayer1);
                    }
                }
            }
        }

        private void PerformDragOperation()
        {
            ItemType selectedItem = (ItemType)(listView.SelectedItem as ItemType);
            if (DragDrop.DoDragDrop(listView, selectedItem, DragDropEffects.Move | DragDropEffects.Link) != DragDropEffects.None)
            {
                listView.SelectedItem = selectedItem;
            }
        }

        private void UpdateDragAdornerLocation()
        {
            if (dragAdorner != null)
            {
                Point mousePosition = MouseUtilities.GetMousePosition(ListView);
                double x = mousePosition.X - ptMouseDown.X;
                ListViewItem listViewItem = GetListViewItem(indexToSelect);
                Point point = listViewItem.TranslatePoint(new Point(0, 0), ListView);
                double y = point.Y + mousePosition.Y - ptMouseDown.Y;
                dragAdorner.SetOffsets(x, y);
            }
        }

        public event EventHandler<ProcessDropEventArgs<ItemType>> ProcessDrop;
    }
    public static class ListViewItemDragState
    {
        public readonly static DependencyProperty IsBeingDraggedProperty;

        public readonly static DependencyProperty IsUnderDragCursorProperty;

        static ListViewItemDragState()
        {
            IsBeingDraggedProperty = DependencyProperty.RegisterAttached("IsBeingDragged", typeof(bool), typeof(ListViewItemDragState), new UIPropertyMetadata(false));
            IsUnderDragCursorProperty = DependencyProperty.RegisterAttached("IsUnderDragCursor", typeof(bool), typeof(ListViewItemDragState), new UIPropertyMetadata(false));
        }

        public static bool GetIsBeingDragged(ListViewItem item)
        {
            return (bool)item.GetValue(IsBeingDraggedProperty);
        }

        public static bool GetIsUnderDragCursor(ListViewItem item)
        {
            return (bool)item.GetValue(IsUnderDragCursorProperty);
        }

        internal static void SetIsBeingDragged(ListViewItem item, bool value)
        {
            item.SetValue(IsBeingDraggedProperty, value);
        }

        internal static void SetIsUnderDragCursor(ListViewItem item, bool value)
        {
            item.SetValue(IsUnderDragCursorProperty, value);
        }
    }
    public class ProcessDropEventArgs<ItemType> : EventArgs
    where ItemType : class
    {
        private ObservableCollection<ItemType> itemsSource;

        private ItemType dataItem;

        private int oldIndex;

        private int newIndex;

        private DragDropEffects allowedEffects;

        private DragDropEffects effects;

        public DragDropEffects AllowedEffects
        {
            get
            {
                return allowedEffects;
            }
        }

        public ItemType DataItem
        {
            get
            {
                return dataItem;
            }
        }

        public DragDropEffects Effects
        {
            get
            {
                return effects;
            }
            set
            {
                effects = value;
            }
        }

        public ObservableCollection<ItemType> ItemsSource
        {
            get
            {
                return itemsSource;
            }
        }

        public int NewIndex
        {
            get
            {
                return newIndex;
            }
        }

        public int OldIndex
        {
            get
            {
                return oldIndex;
            }
        }

        internal ProcessDropEventArgs(ObservableCollection<ItemType> itemsSource, ItemType dataItem, int oldIndex, int newIndex, DragDropEffects allowedEffects)
        {
            this.itemsSource = itemsSource;
            this.dataItem = dataItem;
            this.oldIndex = oldIndex;
            this.newIndex = newIndex;
            this.allowedEffects = allowedEffects;
        }
    }
}