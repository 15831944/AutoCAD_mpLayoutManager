namespace mpLayoutManager.Windows
{
    using System.Collections.Generic;
    using System.Windows;
    using mpWin = ModPlusAPI.Windows;

    public partial class LayoutNewName
    {
        private const string LangItem = "mpLayoutManager";

        public List<string> LayoutsNames;

        public LayoutNewName()
        {
            InitializeComponent();
            Title = ModPlusAPI.Language.GetItem(LangItem, "h10");
        }

        private void BtAccept_OnClick(object sender, RoutedEventArgs e)
        {
            OnAccept();
        }

        private void BtCancel_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void LayoutNewName_OnLoaded(object sender, RoutedEventArgs e)
        {
            TbNewName.Focus();
            TbNewName.CaretIndex = TbNewName.Text.Length;
        }

        private void OnAccept()
        {
            if (string.IsNullOrEmpty(TbNewName.Text))
            {
                mpWin.MessageBox.Show(ModPlusAPI.Language.GetItem(LangItem, "h11"), mpWin.MessageBoxIcon.Alert);
                TbNewName.Focus();
            }
            else if (!LayoutsNames.Contains(TbNewName.Text))
            {
                DialogResult = true;
            }
            else
            {
                mpWin.MessageBox.Show(ModPlusAPI.Language.GetItem(LangItem, "h12"), mpWin.MessageBoxIcon.Alert);
                TbNewName.Focus();
            }
        }

    }
}