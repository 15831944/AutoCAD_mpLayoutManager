using AcApp = Autodesk.AutoCAD.ApplicationServices.Core.Application;
using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using ModPlusAPI.Windows;
using Visibility = System.Windows.Visibility;

namespace mpLayoutManager.Windows
{
    public partial class MoveCopyLayout
    {
        private const string LangItem = "mpLayoutManager";

        public string SelectedLayoutName;

        public int SelectedLayoutTabOrder;
        

        public MoveCopyLayout()
        {
            InitializeComponent();
            Title = ModPlusAPI.Language.GetItem(LangItem, "h19");
            Loaded += MoveCopyLayout_Loaded;
        }

        private void BtAccept_OnClick(object sender, RoutedEventArgs e)
        {
            OnAccept();
        }

        private void BtCancel_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
        
        private void OnAccept()
        {
            if (LbLayouts.SelectedIndex != -1)
            {
                if (LbLayouts.SelectedItem is LayoutForBinding selectedItem)
                {
                    SelectedLayoutName = selectedItem.LayoutName;
                    SelectedLayoutTabOrder = selectedItem.TabOrder;
                }
                DialogResult = true;
            }
            else
            {
                ModPlusAPI.Windows.MessageBox.Show(ModPlusAPI.Language.GetItem(LangItem, "h24"), MessageBoxIcon.Alert);
            }
        }

        private void MoveCopyLayout_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                SizeToContent = SizeToContent.Manual;
                PanelCopyCount.Visibility = Visibility.Collapsed;

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
                            LayoutName = "(" + ModPlusAPI.Language.GetItem(LangItem, "h25") + ")",
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

        private void ChkMakeCopy_OnChecked(object sender, RoutedEventArgs e)
        {
            PanelCopyCount.Visibility = Visibility.Visible;
        }

        private void ChkMakeCopy_OnUnchecked(object sender, RoutedEventArgs e)
        {
            PanelCopyCount.Visibility = Visibility.Collapsed;
        }
    }
}