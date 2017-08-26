﻿#if ac2010
using AcApp = Autodesk.AutoCAD.ApplicationServices.Application;
#elif ac2013
using AcApp = Autodesk.AutoCAD.ApplicationServices.Core.Application;
#endif
using Autodesk.AutoCAD.DatabaseServices;
using mpLayoutManager.Windows;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Autodesk.AutoCAD.ApplicationServices;
using ModPlusAPI;
using ModPlusAPI.Windows;

namespace mpLayoutManager
{
    public partial class LmPalette
    {

        private ListViewDragDropManager<LayoutForBinding> _dragMgr;

        private static ObservableCollection<LayoutForBinding> _currentDocLayouts;

        private bool _showModel;
        
        public LmPalette()
        {
            InitializeComponent();
            ModPlusAPI.Windows.Helpers.WindowHelpers.ChangeThemeForResurceDictionary(Resources, true);
            Loaded += LmPalette_Loaded;
        }

        private void _dragMgr_ProcessDrop(object sender, ProcessDropEventArgs<LayoutForBinding> e)
        {
            try
            {
                Document mdiActiveDocument = AcApp.DocumentManager.MdiActiveDocument;
                if (mdiActiveDocument != null)
                {
                    int newIndex = e.NewIndex;
                    int oldIndex = e.OldIndex;
                    if (e.ItemsSource[oldIndex] != null && e.ItemsSource[newIndex] != null)
                    {
                        if (!_showModel)
                        {
                            e.ItemsSource.Move(oldIndex, newIndex);
                            int num = 1;
                            foreach (LayoutForBinding itemsSource in e.ItemsSource)
                            {
                                itemsSource.TabOrder = num;
                                num++;
                            }
                        }
                        else if (!(oldIndex == 0 | newIndex == -1 | newIndex == 0))
                        {
                            e.ItemsSource.Move(oldIndex, newIndex);
                            int num1 = 0;
                            foreach (LayoutForBinding layoutForBinding in e.ItemsSource)
                            {
                                layoutForBinding.TabOrder = num1;
                                num1++;
                            }
                        }
                        else
                        {
                            return;
                        }
                        using (mdiActiveDocument.LockDocument())
                        {
                            using (Transaction transaction = mdiActiveDocument.Database.TransactionManager.StartTransaction())
                            {
                                LayoutManager current = LayoutManager.Current;
                                foreach (LayoutForBinding itemsSource1 in e.ItemsSource)
                                {
                                    ObjectId layoutId = current.GetLayoutId(itemsSource1.LayoutName);
                                    Layout obj = transaction.GetObject(layoutId, OpenMode.ForWrite) as Layout;
                                    if (obj != null)
                                    {
                                        obj.TabOrder = itemsSource1.TabOrder;
                                    }
                                }
                                transaction.Commit();
                            }
                            mdiActiveDocument.Editor.Regen();
                        }
                        e.Effects = DragDropEffects.Move;
                    }
                }
            }
            catch (Exception exception)
            {
                ExceptionBox.Show(exception);
            }
        }

        private void BindingLayoutsToListView()
        {
            try
            {
                LvLayouts.ItemsSource = null;
                string currentLayoutName = GetCurrentLayoutName();
                foreach (LayoutForBinding _currentDocLayout in _currentDocLayouts)
                {
                    _currentDocLayout.TabSelected = _currentDocLayout.LayoutName.Equals(currentLayoutName);
                }
                LvLayouts.ItemsSource = _currentDocLayouts;
            }
            catch (Exception exception)
            {
                ExceptionBox.Show(exception);
            }
        }

        private void BtAddLayuot_OnClick(object sender, RoutedEventArgs e)
        {
            bool flag;
            Database database;
            string layoutName;
            string str;
            try
            {
                Document mdiActiveDocument = AcApp.DocumentManager.MdiActiveDocument;
                database = mdiActiveDocument != null ? mdiActiveDocument.Database : null;
                if (database != null)
                {
                    bool flag1 = !bool.TryParse(UserConfigFile.GetValue(UserConfigFile.ConfigFileZone.Settings, "mpLayoutManager", "AskLayoutName"), out flag) | flag;
                    LayoutManager current = LayoutManager.Current;
                    if (!flag1)
                    {
                        string str1 = string.Concat("Лист", GetNewLayoutNumber(current));
                        using (mdiActiveDocument.LockDocument())
                        {
                            using (Transaction transaction = mdiActiveDocument.TransactionManager.StartTransaction())
                            {
                                ObjectId objectId = current.CreateLayout(str1);
                                if (bool.TryParse(UserConfigFile.GetValue(UserConfigFile.ConfigFileZone.Settings, "mpLayoutManager", "OpenNewLayout"), out flag) & flag)
                                {
                                    Layout obj = transaction.GetObject(objectId, OpenMode.ForWrite) as Layout;
                                    if (obj != null)
                                    {
                                        obj.Initialize();
                                    }
                                    LayoutManager layoutManager = current;
                                    if (obj != null)
                                    {
                                        layoutName = obj.LayoutName;
                                    }
                                    else
                                    {
                                        layoutName = null;
                                    }
                                    layoutManager.CurrentLayout = layoutName;
                                    database.TileMode = false;
                                    mdiActiveDocument.Editor.SwitchToPaperSpace();
                                }
                                GetCurrentDocLayouts();
                                transaction.Commit();
                                mdiActiveDocument.Editor.Regen();
                            }
                        }
                    }
                    else
                    {
                        LayoutNewName layoutNewName = new LayoutNewName()
                        {
                            LayoutsNames = (
                                from layout in _currentDocLayouts
                                select layout.LayoutName).ToList()
                        };
                        layoutNewName.TbNewName.Text = string.Concat("Лист", GetNewLayoutNumber(current));
                        layoutNewName.Topmost = true;
                        LayoutNewName layoutNewName1 = layoutNewName;
                        bool? nullable = layoutNewName1.ShowDialog();
                        if ((nullable.GetValueOrDefault() && nullable.HasValue))
                        {
                            using (mdiActiveDocument.LockDocument())
                            {
                                using (Transaction transaction1 = mdiActiveDocument.TransactionManager.StartTransaction())
                                {
                                    ObjectId objectId1 = current.CreateLayout(layoutNewName1.TbNewName.Text);
                                    if (bool.TryParse(UserConfigFile.GetValue(UserConfigFile.ConfigFileZone.Settings, "mpLayoutManager", "OpenNewLayout"), out flag) & flag)
                                    {
                                        Layout obj1 = transaction1.GetObject(objectId1, OpenMode.ForWrite) as Layout;
                                        if (obj1 != null)
                                        {
                                            obj1.Initialize();
                                        }
                                        LayoutManager layoutManager1 = current;
                                        if (obj1 != null)
                                        {
                                            str = obj1.LayoutName;
                                        }
                                        else
                                        {
                                            str = null;
                                        }
                                        layoutManager1.CurrentLayout = str;
                                        database.TileMode = false;
                                        mdiActiveDocument.Editor.SwitchToPaperSpace();
                                    }
                                    GetCurrentDocLayouts();
                                    transaction1.Commit();
                                    mdiActiveDocument.Editor.Regen();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                ExceptionBox.Show(exception);
            }
        }

        private void BtAddLayuotByTemplate_OnClick(object sender, RoutedEventArgs e)
        {
            AcApp.DocumentManager.MdiActiveDocument.SendStringToExecute("_.LAYOUT _T ", true, false, false);
        }

        private void DocumentManager_DocumentBecameCurrent(object sender, DocumentCollectionEventArgs e)
        {
            GetCurrentDocLayouts();
        }

        private void DocumentManager_DocumentDestroyed(object sender, DocumentDestroyedEventArgs e)
        {
            GetCurrentDocLayouts();
        }

        private string GetCopyLayoutName(LayoutForBinding selectedLayout)
        {
            int num = 1 + _currentDocLayouts.Count(currentDocLayout => currentDocLayout.LayoutName.Contains(selectedLayout.LayoutName));
            return string.Concat(selectedLayout.LayoutName, " (", num, ")");
        }

        private void GetCurrentDocLayouts()
        {
            Database database;
            try
            {
                Document mdiActiveDocument = AcApp.DocumentManager.MdiActiveDocument;
                if (mdiActiveDocument != null)
                {
                    database = mdiActiveDocument.Database;
                }
                else
                {
                    database = null;
                }
                Database database1 = database;
                if (!((database1 == null) | (mdiActiveDocument == null)))
                {
                    _currentDocLayouts = new ObservableCollection<LayoutForBinding>();
                    using (Transaction transaction = database1.TransactionManager.StartTransaction())
                    {
                        LayoutManager current = LayoutManager.Current;
                        DBDictionary obj = transaction.GetObject(database1.LayoutDictionaryId, OpenMode.ForRead, false) as DBDictionary;
                        if (obj != null)
                        {
                            foreach (DBDictionaryEntry dBDictionaryEntry in obj)
                            {
                                Layout layout = transaction.GetObject(dBDictionaryEntry.Value, OpenMode.ForRead) as Layout;
                                if (layout != null)
                                {
                                    LayoutForBinding layoutForBinding = new LayoutForBinding()
                                    {
                                        LayoutName = layout.LayoutName,
                                        ModelType = layout.ModelType,
                                        TabOrder = (layout.TabOrder == -1 ? current.LayoutCount : layout.TabOrder),
                                        TabSelected = layout.TabSelected
                                    };
                                    if (_showModel)
                                    {
                                        _currentDocLayouts.Add(layoutForBinding);
                                    }
                                    else if (!layout.ModelType)
                                    {
                                        _currentDocLayouts.Add(layoutForBinding);
                                    }
                                }
                            }
                        }
                        transaction.Commit();
                    }
                    _currentDocLayouts = new ObservableCollection<LayoutForBinding>(
                        from x in _currentDocLayouts
                        orderby x.TabOrder
                        select x);
                }
                else
                {
                    LvLayouts.ItemsSource = null;
                    return;
                }
            }
            catch (Exception exception)
            {
                ExceptionBox.Show(exception);
            }
            BindingLayoutsToListView();
        }

        private static string GetCurrentLayoutName()
        {
            return LayoutManager.Current.CurrentLayout;
        }

        private int GetNewLayoutNumber(LayoutManager lm)
        {
            int layoutCount = lm.LayoutCount;
            bool flag = true;
            while (flag)
            {
                bool flag1 = false;
                foreach (LayoutForBinding _currentDocLayout in _currentDocLayouts)
                {
                    if (_currentDocLayout.LayoutName.Equals(string.Concat("Лист", layoutCount)))
                    {
                        flag1 = true;
                    }
                }
                if (!flag1)
                {
                    flag = false;
                }
                else
                {
                    layoutCount++;
                }
            }
            return layoutCount;
        }
        
        private void ItemContextMenu_OnOpened(object sender, RoutedEventArgs e)
        {
            try
            {
                ContextMenu contextMenu = sender as ContextMenu;
                if (contextMenu != null)
                {
                    ListViewItem placementTarget = contextMenu.PlacementTarget as ListViewItem;
                    LayoutManager current = LayoutManager.Current;
                    foreach (object item in contextMenu.Items)
                    {
                        MenuItem menuItem = item as MenuItem;
                        if (menuItem != null)
                        {
                            if (LvLayouts.SelectedItems.Count > 1)
                            {
                                if ((
                                    from object selectedItem in LvLayouts.SelectedItems
                                    select selectedItem as LayoutForBinding).Any(
                                    si => si.ModelType))
                                {
                                    menuItem.IsEnabled = false;
                                }
                                else if (
                                    !(menuItem.Name.Equals("MiOpen") | menuItem.Name.Equals("MiRename") |
                                      menuItem.Name.Equals("MiPageSetup") | menuItem.Name.Equals("MiPlot")))
                                {
                                    menuItem.IsEnabled = true;
                                }
                                else
                                {
                                    menuItem.IsEnabled = false;
                                }
                            }
                            else
                            {
                                var layoutForBinding = LvLayouts.SelectedItem as LayoutForBinding;
                                if (layoutForBinding != null && LvLayouts.SelectedItems.Count == 1 &
                                    layoutForBinding.ModelType)
                                {
                                    if (!(menuItem.Name.Equals("MiOpen") | menuItem.Name.Equals("MiSelectAll")))
                                    {
                                        menuItem.IsEnabled = false;
                                    }
                                    else
                                    {
                                        menuItem.IsEnabled = true;
                                    }
                                }
                                else if (
                                    placementTarget != null && current.CurrentLayout.Equals(
                                        ((LayoutForBinding) placementTarget.Content).LayoutName))
                                {
                                    menuItem.IsEnabled = true;
                                }
                                else if (menuItem.Name.Equals("MiPageSetup") | menuItem.Name.Equals("MiPlot") |
                                         menuItem.Name.Equals("MiExportLayout"))
                                {
                                    menuItem.IsEnabled = false;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                ExceptionBox.Show(exception);
            }
        }

        private void LayoutItem_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                _dragMgr.ListView = null;
                OpenSelectedLayout();
                _dragMgr.ListView = LvLayouts;
            }
            catch (Exception exception)
            {
                ExceptionBox.Show(exception);
            }
        }

        private void Lm_LayoutCopied(object sender, LayoutCopiedEventArgs e)
        {
            GetCurrentDocLayouts();
        }

        private void Lm_LayoutCreated(object sender, LayoutEventArgs e)
        {
            GetCurrentDocLayouts();
        }

        private void Lm_LayoutRemoved(object sender, LayoutEventArgs e)
        {
            GetCurrentDocLayouts();
        }

        private void Lm_LayoutRenamed(object sender, LayoutRenamedEventArgs e)
        {
            GetCurrentDocLayouts();
        }

        private void Lm_LayoutsReordered(object sender, EventArgs e)
        {
            GetCurrentDocLayouts();
        }

        private void Lm_LayoutSwitched(object sender, LayoutEventArgs e)
        {
            BindingLayoutsToListView();
        }

        private void LmPalette_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                _showModel = !bool.TryParse(UserConfigFile.GetValue(UserConfigFile.ConfigFileZone.Settings, "mpLayoutManager", "ShowModel"), out bool flag) | flag;
                _dragMgr = new ListViewDragDropManager<LayoutForBinding>(LvLayouts)
                {
                    DragAdornerOpacity = 1
                };
                _dragMgr.ProcessDrop += _dragMgr_ProcessDrop;
                LvLayouts.DragEnter += LvLayouts_DragEnter;
                GetCurrentDocLayouts();
                LayoutManager current = LayoutManager.Current;
                current.LayoutCreated += Lm_LayoutCreated;
                current.LayoutRenamed += Lm_LayoutRenamed;
                current.LayoutRemoved += Lm_LayoutRemoved;
                current.LayoutCopied += Lm_LayoutCopied;
                current.LayoutsReordered += Lm_LayoutsReordered;
                current.LayoutSwitched += Lm_LayoutSwitched;
                AcApp.DocumentManager.DocumentBecameCurrent += DocumentManager_DocumentBecameCurrent;
                AcApp.DocumentManager.DocumentDestroyed += DocumentManager_DocumentDestroyed;
            }
            catch (Exception exception)
            {
                ExceptionBox.Show(exception);
            }
        }

        private void LmSettings_OnClick(object sender, RoutedEventArgs e)
        {
            LmSettings lmSetting = new LmSettings()
            {
                Topmost = true
            };
            lmSetting.ShowDialog();
            if (lmSetting.ChkShowModel.IsChecked.HasValue)
            {
                _showModel = lmSetting.ChkShowModel.IsChecked.Value;
            }
            if ((!lmSetting.ChkAddToMpPalette.IsChecked.HasValue ? true : !lmSetting.ChkAddToMpPalette.IsChecked.Value))
            {
                FunctionStart.RemoveFromMpPalette(true);
            }
            else
            {
                FunctionStart.AddToMpPalette(true);
            }
            GetCurrentDocLayouts();
            BindingLayoutsToListView();
        }

        private void LvLayouts_DragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Move;
        }

        private void LvLayouts_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListView lvLayouts;
            if (e != null & LvLayouts != null)
            {
                ListViewDragDropManager<LayoutForBinding> listViewDragDropManager = _dragMgr;
                if (e.AddedItems.Count > 1)
                {
                    lvLayouts = null;
                }
                else
                {
                    lvLayouts = LvLayouts;
                }
                listViewDragDropManager.ListView = lvLayouts;
            }
        }

        private void MenuItem_Delete_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Document mdiActiveDocument = AcApp.DocumentManager.MdiActiveDocument;
                if (LvLayouts.SelectedItems.Count == 1)
                {
                    LayoutForBinding selectedItem = LvLayouts.SelectedItem as LayoutForBinding;
                    if (selectedItem != null && 
                        ModPlusAPI.Windows.MessageBox.ShowYesNo(string.Concat("Вы уверены, что хотите удалить лист",
                        Environment.NewLine, selectedItem.LayoutName, "?"), MessageBoxIcon.Question))
                    {
                        using (mdiActiveDocument.LockDocument())
                        {
                            LayoutManager.Current.DeleteLayout(selectedItem.LayoutName);
                            _currentDocLayouts.Remove(selectedItem);
                            BindingLayoutsToListView();
                            mdiActiveDocument.Editor.Regen();
                        }
                    }
                }
                else if (LvLayouts.SelectedItems.Count > 1)
                {
                    if (ModPlusAPI.Windows.MessageBox.ShowYesNo("Вы уверены, что хотите удалить выбранные листы?", MessageBoxIcon.Question))
                    {
                        List<LayoutForBinding> list = (
                            from selectedLayout in LvLayouts.SelectedItems.OfType<LayoutForBinding>()
                            where !selectedLayout.ModelType
                            select selectedLayout).ToList();
                        if (list.Count > 0)
                        {
                            using (mdiActiveDocument.LockDocument())
                            {
                                foreach (LayoutForBinding layoutForBinding in list)
                                {
                                    LayoutManager.Current.DeleteLayout(layoutForBinding.LayoutName);
                                    _currentDocLayouts.Remove(layoutForBinding);
                                }
                            }
                            BindingLayoutsToListView();
                            mdiActiveDocument.Editor.Regen();
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                ExceptionBox.Show(exception);
            }
        }

        private void MenuItem_Open_OnClick(object sender, RoutedEventArgs e)
        {
            OpenSelectedLayout();
        }

        private void MenuItem_Rename_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Document mdiActiveDocument = AcApp.DocumentManager.MdiActiveDocument;
                if (LvLayouts.SelectedItems.Count <= 1)
                {
                    LayoutForBinding selectedItem = LvLayouts.SelectedItem as LayoutForBinding;
                    if (selectedItem != null)
                    {
                        if (!selectedItem.ModelType)
                        {
                            RenameLayout renameLayout = new RenameLayout()
                            {
                                LayoutsNames = (
                                    from layout in _currentDocLayouts
                                    select layout.LayoutName).ToList()
                            };
                            renameLayout.TbCurrentName.Text = selectedItem.LayoutName;
                            renameLayout.TbNewName.Text = selectedItem.LayoutName;
                            renameLayout.Topmost = true;
                            RenameLayout renameLayout1 = renameLayout;
                            bool? nullable = renameLayout1.ShowDialog();
                            if ((nullable.GetValueOrDefault() && nullable.HasValue))
                            {
                                if (renameLayout1.TbNewName.Text != renameLayout1.TbCurrentName.Text)
                                {
                                    using (Transaction transaction = mdiActiveDocument.TransactionManager.StartTransaction())
                                    {
                                        using (mdiActiveDocument.LockDocument())
                                        {
                                            LayoutManager current = LayoutManager.Current;
                                            current.RenameLayout(renameLayout1.TbCurrentName.Text, renameLayout1.TbNewName.Text);
                                            GetCurrentDocLayouts();
                                        }
                                        transaction.Commit();
                                        mdiActiveDocument.Editor.Regen();
                                    }
                                }
                            }
                        }
                        else
                        {
                            ModPlusAPI.Windows.MessageBox.Show("Нельзя переименовать вкладку \"Модель\"");
                        }
                    }
                }
                else
                {
                    ModPlusAPI.Windows.MessageBox.Show("Выберите, пожалуйста, один лист");
                }
            }
            catch (Exception exception)
            {
                ExceptionBox.Show(exception);
            }
        }

        private void MiExportLayout_OnClick(object sender, RoutedEventArgs e)
        {
            AcApp.DocumentManager.MdiActiveDocument.SendStringToExecute("_.EXPORTLAYOUT ", true, false, false);
        }

        private void MiMoveCopy_OnClick(object sender, RoutedEventArgs e)
        {
            bool value;
            try
            {
                Document mdiActiveDocument = AcApp.DocumentManager.MdiActiveDocument;
                LayoutManager current = LayoutManager.Current;
                IList selectedItems = LvLayouts.SelectedItems;
                if (selectedItems != null)
                {
                    if (selectedItems.Count != 0)
                    {
                        if (!selectedItems.Cast<object>().Any(selectedItem => {
                            LayoutForBinding layoutForBinding = selectedItem as LayoutForBinding;
                            return layoutForBinding?.ModelType ?? false;
                        }))
                        {
                            MoveCopyLayout moveCopyLayout = new MoveCopyLayout()
                            {
                                Topmost = true
                            };
                            bool? isChecked = moveCopyLayout.ShowDialog();
                            if ((isChecked.GetValueOrDefault() && isChecked.HasValue))
                            {
                                isChecked = moveCopyLayout.ChkMakeCopy.IsChecked;
                                if (!isChecked.HasValue)
                                {
                                    value = false;
                                }
                                else
                                {
                                    isChecked = moveCopyLayout.ChkMakeCopy.IsChecked;
                                    value = isChecked.Value;
                                }
                                if (!value)
                                {
                                    if (moveCopyLayout.SelectedLayoutTabOrder != -1)
                                    {
                                        int num = _currentDocLayouts.IndexOf(_currentDocLayouts.Single(x => x.LayoutName.Equals(moveCopyLayout.SelectedLayoutName)));
                                        for (int i = selectedItems.Count - 1; i >= 0; i--)
                                        {
                                            LayoutForBinding item = selectedItems[i] as LayoutForBinding;
                                            _currentDocLayouts.Move(_currentDocLayouts.IndexOf(item), num);
                                        }
                                        SetNewTabOrderByListPositions();
                                    }
                                    else
                                    {
                                        foreach (LayoutForBinding layoutForBinding1 in selectedItems)
                                        {
                                            _currentDocLayouts.Move(_currentDocLayouts.IndexOf(layoutForBinding1), _currentDocLayouts.Count - 1);
                                        }
                                        SetNewTabOrderByListPositions();
                                    }
                                }
                                else if (moveCopyLayout.SelectedLayoutTabOrder != -1)
                                {
                                    int num1 = _currentDocLayouts.IndexOf(_currentDocLayouts.Single(x => x.LayoutName.Equals(moveCopyLayout.SelectedLayoutName)));
                                    current.LayoutCreated -= Lm_LayoutCreated;
                                    current.LayoutCopied -= Lm_LayoutCopied;
                                    using (mdiActiveDocument.LockDocument())
                                    {
                                        for (int j = selectedItems.Count - 1; j >= 0; j--)
                                        {
                                            LayoutForBinding item1 = selectedItems[j] as LayoutForBinding;
                                            if (item1 != null)
                                            {
                                                if (num1 == 1)
                                                {
                                                    current.CloneLayout(item1.LayoutName, GetCopyLayoutName(item1), 1);
                                                }
                                                else
                                                {
                                                    current.CloneLayout(item1.LayoutName, GetCopyLayoutName(item1), num1 - 1);
                                                }
                                            }
                                        }
                                    }
                                    current.LayoutCreated += Lm_LayoutCreated;
                                    current.LayoutCopied += Lm_LayoutCopied;
                                    mdiActiveDocument.Editor.Regen();
                                    GetCurrentDocLayouts();
                                }
                                else
                                {
                                    current.LayoutCreated -= Lm_LayoutCreated;
                                    current.LayoutCopied -= Lm_LayoutCopied;
                                    using (mdiActiveDocument.LockDocument())
                                    {
                                        foreach (LayoutForBinding layoutForBinding2 in selectedItems)
                                        {
                                            current.CloneLayout(layoutForBinding2.LayoutName, GetCopyLayoutName(layoutForBinding2), current.LayoutCount);
                                        }
                                        mdiActiveDocument.Editor.Regen();
                                    }
                                    current.LayoutCreated += Lm_LayoutCreated;
                                    current.LayoutCopied += Lm_LayoutCopied;
                                    GetCurrentDocLayouts();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                ExceptionBox.Show(exception);
            }
        }

        private void MiPageSetup_OnClick(object sender, RoutedEventArgs e)
        {
            AcApp.DocumentManager.MdiActiveDocument.SendStringToExecute("_.PAGESETUP ", true, false, false);
        }

        private void MiPlot_OnClick(object sender, RoutedEventArgs e)
        {
            AcApp.DocumentManager.MdiActiveDocument.SendStringToExecute("_.PLOT ", true, false, false);
        }

        private void MiSelectAll_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!_showModel)
                {
                    LvLayouts.SelectAll();
                }
                else
                {
                    for (int i = 0; i < LvLayouts.Items.Count; i++)
                    {
                        ListViewItem listViewItem = LvLayouts.ItemContainerGenerator.ContainerFromIndex(i) as ListViewItem;
                        if (listViewItem != null)
                        {
                            listViewItem.IsSelected = i != 0;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                ExceptionBox.Show(exception);
            }
        }

        private void OpenSelectedLayout()
        {
            LayoutManager current = LayoutManager.Current;
            try
            {
                try
                {
                    Document mdiActiveDocument = AcApp.DocumentManager.MdiActiveDocument;
                    if (LvLayouts.SelectedItems.Count <= 1)
                    {
                        current.LayoutSwitched -= Lm_LayoutSwitched;
                        LvLayouts.SelectionChanged -= LvLayouts_OnSelectionChanged;
                        LayoutForBinding selectedItem = LvLayouts.SelectedItem as LayoutForBinding;
                        if (selectedItem != null)
                        {
                            using (mdiActiveDocument.LockDocument())
                            {
                                current.CurrentLayout = selectedItem.LayoutName;
                                mdiActiveDocument.Editor.Regen();
                            }
                            BindingLayoutsToListView();
                        }
                    }
                    else
                    {
                        ModPlusAPI.Windows.MessageBox.Show("Выберите, пожалуйста, один лист");
                    }
                }
                catch (Exception exception)
                {
                    ExceptionBox.Show(exception);
                }
            }
            finally
            {
                current.LayoutSwitched += Lm_LayoutSwitched;
                LvLayouts.SelectionChanged += LvLayouts_OnSelectionChanged;
            }
        }

        private void SetNewTabOrderByListPositions()
        {
            Document mdiActiveDocument = AcApp.DocumentManager.MdiActiveDocument;
            if (mdiActiveDocument != null)
            {
                using (mdiActiveDocument.LockDocument())
                {
                    using (Transaction transaction = mdiActiveDocument.Database.TransactionManager.StartTransaction())
                    {
                        LayoutManager current = LayoutManager.Current;
                        int num = 0;
                        if (!_showModel)
                        {
                            num = 1;
                        }
                        foreach (LayoutForBinding _currentDocLayout in _currentDocLayouts)
                        {
                            ObjectId layoutId = current.GetLayoutId(_currentDocLayout.LayoutName);
                            Layout obj = transaction.GetObject(layoutId, OpenMode.ForWrite) as Layout;
                            if (obj != null)
                            {
                                obj.TabOrder = num;
                            }
                            num++;
                        }
                        transaction.Commit();
                    }
                    mdiActiveDocument.Editor.Regen();
                }
            }
        }
        
        private class LayoutForBinding
        {
            public string LayoutName
            {
                get;
                set;
            }

            public bool ModelType
            {
                get;
                set;
            }

            public int TabOrder
            {
                get;
                set;
            }

            public bool TabSelected
            {
                get;
                set;
            }
        }
    }
}