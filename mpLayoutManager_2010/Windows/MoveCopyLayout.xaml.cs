#if ac2010
using AcApp = Autodesk.AutoCAD.ApplicationServices.Application;
#elif ac2013
using AcApp = Autodesk.AutoCAD.ApplicationServices.Core.Application;
#endif
using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Autodesk.AutoCAD.ApplicationServices;
using ModPlusAPI.Windows;
using ModPlusAPI.Windows.Helpers;

namespace mpLayoutManager.Windows
{
    public partial class MoveCopyLayout
    {
        public string SelectedLayoutName;

        public int SelectedLayoutTabOrder;
        

        public MoveCopyLayout()
        {
            InitializeComponent();
            this.OnWindowStartUp();
            Loaded += RenameLayout_Loaded;
        }

        private void BtAccept_OnClick(object sender, RoutedEventArgs e)
        {
            OnAccept();
        }

        private void BtCancel_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
        

        private void MoveCopyLayout_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                DialogResult = false;
            }
            if (e.Key == Key.Return)
            {
                OnAccept();
            }
        }

        private void OnAccept()
        {
            if (LbLayouts.SelectedIndex != -1)
            {
                var selectedItem = LbLayouts.SelectedItem as LayoutForBinding;
                if (selectedItem != null)
                {
                    SelectedLayoutName = selectedItem.LayoutName;
                    SelectedLayoutTabOrder = selectedItem.TabOrder;
                }
                DialogResult = true;
            }
            else
            {
                ModPlusAPI.Windows.MessageBox.Show("Нужно выбрать лист в списке!", MessageBoxIcon.Alert);
            }
        }

        private void RenameLayout_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                SizeToContent = SizeToContent.Manual;

                var mdiActiveDocument = AcApp.DocumentManager.MdiActiveDocument;
                var database = mdiActiveDocument != null ? mdiActiveDocument.Database : null;
                if (database != null)
                {
                    ObservableCollection<LayoutForBinding> observableCollection = new ObservableCollection<LayoutForBinding>();
                    using (Transaction transaction = database.TransactionManager.StartTransaction())
                    {
                        DBDictionary obj = transaction.GetObject(database.LayoutDictionaryId, OpenMode.ForRead, false) as DBDictionary;
                        if (obj != null)
                        {
                            foreach (DBDictionaryEntry dBDictionaryEntry in obj)
                            {
                                Layout layout = transaction.GetObject(dBDictionaryEntry.Value, OpenMode.ForRead) as Layout;
                                if (layout != null)
                                {
                                    if (!layout.ModelType)
                                    {
                                        observableCollection.Add(new LayoutForBinding
                                        {
                                            LayoutName = layout.LayoutName,
                                            TabOrder = layout.TabOrder
                                        });
                                    }
                                }
                            }
                        }
                        transaction.Commit();
                    }
                    observableCollection = new ObservableCollection<LayoutForBinding>(
                        from x in observableCollection
                        orderby x.TabOrder
                        select x)
                    {
                        new LayoutForBinding
                        {
                            LayoutName = "(переместить в конец)",
                            TabOrder = -1
                        }
                    };
                    LbLayouts.ItemsSource = observableCollection;
                }
            }
            catch (Exception exception)
            {
                ExceptionBox.Show(exception);
            }
        }
        
        private class LayoutForBinding
        {
            public string LayoutName
            {
                get;
                set;
            }

            public int TabOrder
            {
                get;
                set;
            }

            public LayoutForBinding()
            {
            }
        }
    }
}