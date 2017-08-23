using System;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using WPF.JoshSmith.Adorners;
using WPF.JoshSmith.Controls.Utilities;

namespace WPF.JoshSmith.ServiceProviders.UI
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
                else if (!this.canInitiateDrag)
                {
                    flag = false;
                }
                else if (this.indexToSelect != -1)
                {
                    flag = (this.HasCursorLeftDragThreshold ? true : false);
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
            get
            {
                return this.dragAdornerOpacity;
            }
            set
            {
                if (this.IsDragInProgress)
                {
                    throw new InvalidOperationException("Cannot set the DragAdornerOpacity property during a drag operation.");
                }
                if ((value < 0 ? true : value > 1))
                {
                    throw new ArgumentOutOfRangeException("DragAdornerOpacity", (object)value, "Must be between 0 and 1.");
                }
                this.dragAdornerOpacity = value;
            }
        }

        private bool HasCursorLeftDragThreshold
        {
            get
            {
                bool flag;
                if (this.indexToSelect >= 0)
                {
                    ListViewItem listViewItem = this.GetListViewItem(this.indexToSelect);
                    if (listViewItem != null)
                    {
                        try
                        {
                            Rect descendantBounds = VisualTreeHelper.GetDescendantBounds(listViewItem);
                            Point point = this.listView.TranslatePoint(this.ptMouseDown, listViewItem);
                            double num = Math.Abs(point.Y);
                            double num1 = Math.Abs(descendantBounds.Height - point.Y);
                            double num2 = Math.Min(num, num1);
                            double minimumHorizontalDragDistance = SystemParameters.MinimumHorizontalDragDistance * 2;
                            double num3 = Math.Min(SystemParameters.MinimumVerticalDragDistance, num2) * 2;
                            Size size = new Size(minimumHorizontalDragDistance, num3);
                            Rect rect = new Rect(this.ptMouseDown, size);
                            rect.Offset(size.Width / -2, size.Height / -2);
                            Point mousePosition = MouseUtilities.GetMousePosition(this.listView);
                            flag = !rect.Contains(mousePosition);
                        }
                        catch (Exception exception)
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
                while (num1 < this.listView.Items.Count)
                {
                    if (!this.IsMouseOver(this.GetListViewItem(num1)))
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
            get
            {
                return this.isDragInProgress;
            }
            private set
            {
                this.isDragInProgress = value;
            }
        }

        private bool IsMouseOverScrollbar
        {
            get
            {
                bool flag;
                Point mousePosition = MouseUtilities.GetMousePosition(this.listView);
                HitTestResult hitTestResult = VisualTreeHelper.HitTest(this.listView, mousePosition);
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
            get
            {
                return this.itemUnderDragCursor;
            }
            set
            {
                if ((object)this.itemUnderDragCursor != (object)value)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        if (i == 1)
                        {
                            this.itemUnderDragCursor = value;
                        }
                        if (this.itemUnderDragCursor != null)
                        {
                            ListViewItem listViewItem = this.GetListViewItem(this.itemUnderDragCursor);
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
                return this.listView;
            }
            set
            {
                if (this.IsDragInProgress)
                {
                    throw new InvalidOperationException("Cannot set the ListView property during a drag operation.");
                }
                if (this.listView != null)
                {
                    this.listView.PreviewMouseLeftButtonDown -= new MouseButtonEventHandler(this.listView_PreviewMouseLeftButtonDown);
                    this.listView.PreviewMouseMove -= new MouseEventHandler(this.listView_PreviewMouseMove);
                    this.listView.DragOver -= new DragEventHandler(this.listView_DragOver);
                    this.listView.DragLeave -= new DragEventHandler(this.listView_DragLeave);
                    this.listView.DragEnter -= new DragEventHandler(this.listView_DragEnter);
                    this.listView.Drop -= new DragEventHandler(this.listView_Drop);
                }
                this.listView = value;
                if (this.listView != null)
                {
                    if (!this.listView.AllowDrop)
                    {
                        this.listView.AllowDrop = true;
                    }
                    this.listView.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(this.listView_PreviewMouseLeftButtonDown);
                    this.listView.PreviewMouseMove += new MouseEventHandler(this.listView_PreviewMouseMove);
                    this.listView.DragOver += new DragEventHandler(this.listView_DragOver);
                    this.listView.DragLeave += new DragEventHandler(this.listView_DragLeave);
                    this.listView.DragEnter += new DragEventHandler(this.listView_DragEnter);
                    this.listView.Drop += new DragEventHandler(this.listView_Drop);
                }
            }
        }

        public bool ShowDragAdorner
        {
            get
            {
                return this.showDragAdorner;
            }
            set
            {
                if (this.IsDragInProgress)
                {
                    throw new InvalidOperationException("Cannot set the ShowDragAdorner property during a drag operation.");
                }
                this.showDragAdorner = value;
            }
        }

        private bool ShowDragAdornerResolved
        {
            get
            {
                return (!this.ShowDragAdorner ? false : this.DragAdornerOpacity > 0);
            }
        }

        public ListViewDragDropManager()
        {
            this.canInitiateDrag = false;
            this.dragAdornerOpacity = 0.7;
            this.indexToSelect = -1;
            this.showDragAdorner = true;
        }

        public ListViewDragDropManager(ListView listView) : this()
        {
            this.ListView = listView;
        }

        public ListViewDragDropManager(ListView listView, double dragAdornerOpacity) : this(listView)
        {
            this.DragAdornerOpacity = dragAdornerOpacity;
        }

        public ListViewDragDropManager(ListView listView, bool showDragAdorner) : this(listView)
        {
            this.ShowDragAdorner = showDragAdorner;
        }

        private void FinishDragOperation(ListViewItem draggedItem, AdornerLayer adornerLayer)
        {
            ListViewItemDragState.SetIsBeingDragged(draggedItem, false);
            this.IsDragInProgress = false;
            if (this.ItemUnderDragCursor != null)
            {
                this.ItemUnderDragCursor = default(ItemType);
            }
            if (adornerLayer != null)
            {
                adornerLayer.Remove(this.dragAdorner);
                this.dragAdorner = null;
            }
        }

        private ListViewItem GetListViewItem(int index)
        {
            ListViewItem listViewItem;
            if (this.listView.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
            {
                listViewItem = this.listView.ItemContainerGenerator.ContainerFromIndex(index) as ListViewItem;
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
            if (this.listView.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
            {
                listViewItem = this.listView.ItemContainerGenerator.ContainerFromItem(dataItem) as ListViewItem;
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
            this.dragAdorner = new DragAdorner(this.listView, itemToDrag.RenderSize, visualBrush)
            {
                Opacity = this.DragAdornerOpacity
            };
            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this.listView);
            adornerLayer.Add(this.dragAdorner);
            this.ptMouseDown = MouseUtilities.GetMousePosition(this.listView);
            return adornerLayer;
        }

        private void InitializeDragOperation(ListViewItem itemToDrag)
        {
            this.IsDragInProgress = true;
            this.canInitiateDrag = false;
            ListViewItemDragState.SetIsBeingDragged(itemToDrag, true);
        }

        private bool IsMouseOver(Visual target)
        {
            Rect descendantBounds = VisualTreeHelper.GetDescendantBounds(target);
            return descendantBounds.Contains(MouseUtilities.GetMousePosition(target));
        }

        private void listView_DragEnter(object sender, DragEventArgs e)
        {
            if ((this.dragAdorner == null ? false : this.dragAdorner.Visibility != Visibility.Visible))
            {
                this.UpdateDragAdornerLocation();
                this.dragAdorner.Visibility = Visibility.Visible;
            }
        }

        private void listView_DragLeave(object sender, DragEventArgs e)
        {
            if (!this.IsMouseOver(this.listView))
            {
                if (this.ItemUnderDragCursor != null)
                {
                    this.ItemUnderDragCursor = default(ItemType);
                }
                if (this.dragAdorner != null)
                {
                    this.dragAdorner.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void listView_DragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Move;
            if (this.ShowDragAdornerResolved)
            {
                this.UpdateDragAdornerLocation();
            }
            int indexUnderDragCursor = this.IndexUnderDragCursor;
            this.ItemUnderDragCursor = (indexUnderDragCursor < 0 ? default(ItemType) : (ItemType)(this.ListView.Items[indexUnderDragCursor] as ItemType));
        }

        private void listView_Drop(object sender, DragEventArgs e)
        {
            ObservableCollection<ItemType> itemsSource;
            int indexUnderDragCursor;
            if (this.ItemUnderDragCursor != null)
            {
                this.ItemUnderDragCursor = default(ItemType);
            }
            e.Effects = DragDropEffects.None;
            if (e.Data.GetDataPresent(typeof(ItemType)))
            {
                ItemType data = (ItemType)(e.Data.GetData(typeof(ItemType)) as ItemType);
                if (data != null)
                {
                    itemsSource = this.listView.ItemsSource as ObservableCollection<ItemType>;
                    if (itemsSource == null)
                    {
                        throw new Exception("A ListView managed by ListViewDragManager must have its ItemsSource set to an ObservableCollection<ItemType>.");
                    }
                    int num = itemsSource.IndexOf(data);
                    indexUnderDragCursor = this.IndexUnderDragCursor;
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
                        if (this.ProcessDrop == null)
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
                            this.ProcessDrop(this, processDropEventArg);
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
            if (!this.IsMouseOverScrollbar)
            {
                int indexUnderDragCursor = this.IndexUnderDragCursor;
                this.canInitiateDrag = indexUnderDragCursor > -1;
                if (!this.canInitiateDrag)
                {
                    this.ptMouseDown = new Point(-10000, -10000);
                    this.indexToSelect = -1;
                }
                else
                {
                    this.ptMouseDown = MouseUtilities.GetMousePosition(this.listView);
                    this.indexToSelect = indexUnderDragCursor;
                }
            }
            else
            {
                this.canInitiateDrag = false;
            }
        }

        private void listView_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            AdornerLayer adornerLayer;
            if (this.CanStartDragOperation)
            {
                if (this.listView.SelectedIndex != this.indexToSelect)
                {
                    this.listView.SelectedIndex = this.indexToSelect;
                }
                if (this.listView.SelectedItem != null)
                {
                    ListViewItem listViewItem = this.GetListViewItem(this.listView.SelectedIndex);
                    if (listViewItem != null)
                    {
                        if (this.ShowDragAdornerResolved)
                        {
                            adornerLayer = this.InitializeAdornerLayer(listViewItem);
                        }
                        else
                        {
                            adornerLayer = null;
                        }
                        AdornerLayer adornerLayer1 = adornerLayer;
                        this.InitializeDragOperation(listViewItem);
                        this.PerformDragOperation();
                        this.FinishDragOperation(listViewItem, adornerLayer1);
                    }
                }
            }
        }

        private void PerformDragOperation()
        {
            ItemType selectedItem = (ItemType)(this.listView.SelectedItem as ItemType);
            if (DragDrop.DoDragDrop(this.listView, selectedItem, DragDropEffects.Move | DragDropEffects.Link) != DragDropEffects.None)
            {
                this.listView.SelectedItem = selectedItem;
            }
        }

        private void UpdateDragAdornerLocation()
        {
            if (this.dragAdorner != null)
            {
                Point mousePosition = MouseUtilities.GetMousePosition(this.ListView);
                double x = mousePosition.X - this.ptMouseDown.X;
                ListViewItem listViewItem = this.GetListViewItem(this.indexToSelect);
                Point point = listViewItem.TranslatePoint(new Point(0, 0), this.ListView);
                double y = point.Y + mousePosition.Y - this.ptMouseDown.Y;
                this.dragAdorner.SetOffsets(x, y);
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
            ListViewItemDragState.IsBeingDraggedProperty = DependencyProperty.RegisterAttached("IsBeingDragged", typeof(bool), typeof(ListViewItemDragState), new UIPropertyMetadata(false));
            ListViewItemDragState.IsUnderDragCursorProperty = DependencyProperty.RegisterAttached("IsUnderDragCursor", typeof(bool), typeof(ListViewItemDragState), new UIPropertyMetadata(false));
        }

        public static bool GetIsBeingDragged(ListViewItem item)
        {
            return (bool)item.GetValue(ListViewItemDragState.IsBeingDraggedProperty);
        }

        public static bool GetIsUnderDragCursor(ListViewItem item)
        {
            return (bool)item.GetValue(ListViewItemDragState.IsUnderDragCursorProperty);
        }

        internal static void SetIsBeingDragged(ListViewItem item, bool value)
        {
            item.SetValue(ListViewItemDragState.IsBeingDraggedProperty, value);
        }

        internal static void SetIsUnderDragCursor(ListViewItem item, bool value)
        {
            item.SetValue(ListViewItemDragState.IsUnderDragCursorProperty, value);
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
                return this.allowedEffects;
            }
        }

        public ItemType DataItem
        {
            get
            {
                return this.dataItem;
            }
        }

        public DragDropEffects Effects
        {
            get
            {
                return this.effects;
            }
            set
            {
                this.effects = value;
            }
        }

        public ObservableCollection<ItemType> ItemsSource
        {
            get
            {
                return this.itemsSource;
            }
        }

        public int NewIndex
        {
            get
            {
                return this.newIndex;
            }
        }

        public int OldIndex
        {
            get
            {
                return this.oldIndex;
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