#if ac2010
using AcApp = Autodesk.AutoCAD.ApplicationServices.Application;
#elif ac2013
using AcApp = Autodesk.AutoCAD.ApplicationServices.Core.Application;
#endif
using Autodesk.AutoCAD.DatabaseServices;
using MahApps.Metro.Controls;
using ModPlus;
using mpMsg;
using mpSettings;
using System;
using System.CodeDom.Compiler;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using Autodesk.AutoCAD.ApplicationServices;

namespace mpLayoutManager.Windows
{
    public partial class MoveCopyLayout
    {
        public string SelectedLayoutName;

        public int SelectedLayoutTabOrder;
        

        public MoveCopyLayout()
        {
            InitializeComponent();
            MpWindowHelpers.OnWindowStartUp(this, 
                MpSettings.GetValue("Settings", "MainSet", "Theme"),
                MpSettings.GetValue("Settings", "MainSet", "AccentColor"),
                MpSettings.GetValue("Settings", "MainSet", "BordersType"));
            base.Loaded += new RoutedEventHandler(RenameLayout_Loaded);
        }

        private void BtAccept_OnClick(object sender, RoutedEventArgs e)
        {
            OnAccept();
        }

        private void BtCancel_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = new bool?(false);
        }
        

        private void MoveCopyLayout_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                DialogResult = new bool?(false);
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
                MoveCopyLayout.LayoutForBinding selectedItem = LbLayouts.SelectedItem as MoveCopyLayout.LayoutForBinding;
                SelectedLayoutName = selectedItem.LayoutName;
                SelectedLayoutTabOrder = selectedItem.TabOrder;
                DialogResult = new bool?(true);
            }
            else
            {
                MpMsgWin.Show("Нужно выбрать лист в списке!");
            }
        }

        private void RenameLayout_Loaded(object sender, RoutedEventArgs e)
        {
            Database database;
            try
            {
                SizeToContent = SizeToContent.Manual;

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
                if (database1 != null)
                {
                    ObservableCollection<MoveCopyLayout.LayoutForBinding> observableCollection = new ObservableCollection<MoveCopyLayout.LayoutForBinding>();
                    using (Transaction transaction = database1.TransactionManager.StartTransaction())
                    {
                        DBDictionary obj = transaction.GetObject(database1.LayoutDictionaryId, OpenMode.ForRead, false) as DBDictionary;
                        if (obj != null)
                        {
                            foreach (DBDictionaryEntry dBDictionaryEntry in obj)
                            {
                                Layout layout = transaction.GetObject(dBDictionaryEntry.Value, OpenMode.ForRead) as Layout;
                                if (layout != null)
                                {
                                    if (!layout.ModelType)
                                    {
                                        observableCollection.Add(new MoveCopyLayout.LayoutForBinding()
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
                    observableCollection = new ObservableCollection<MoveCopyLayout.LayoutForBinding>(
                        from x in observableCollection
                        orderby x.TabOrder
                        select x)
                    {
                        new MoveCopyLayout.LayoutForBinding()
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
                MpExWin.Show(exception);
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