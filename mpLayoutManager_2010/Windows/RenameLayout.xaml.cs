using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using ModPlusAPI.Windows;

namespace mpLayoutManager.Windows
{
    public partial class RenameLayout 
    {
        private const string LangItem = "mpLayoutManager";

        public List<string> LayoutsNames;

        public RenameLayout()
        {
            InitializeComponent();
            Title = ModPlusAPI.Language.GetItem(LangItem, "h26");
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
        
        private void OnAccept()
        {
            if (string.IsNullOrEmpty(TbNewName.Text))
            {
                ModPlusAPI.Windows.MessageBox.Show(ModPlusAPI.Language.GetItem(LangItem, "h11"), MessageBoxIcon.Alert);
                TbNewName.Focus();
            }
            else if (!LayoutsNames.Contains(TbNewName.Text))
            {
                DialogResult = true;
            }
            else
            {
                ModPlusAPI.Windows.MessageBox.Show(ModPlusAPI.Language.GetItem(LangItem, "h12"), MessageBoxIcon.Alert);
                TbNewName.Focus();
            }
        }

        private void RenameLayout_Loaded(object sender, RoutedEventArgs e)
        {
            LayoutsNames.Remove(TbCurrentName.Text);
            TbNewName.Focus();
            TbNewName.CaretIndex = TbNewName.Text.Length;
        }

        private void RenameLayout_OnKeyDown(object sender, KeyEventArgs e)
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
        
    }
}