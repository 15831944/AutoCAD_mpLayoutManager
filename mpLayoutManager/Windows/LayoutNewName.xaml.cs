namespace mpLayoutManager.Windows
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using ModPlusAPI.Windows;

    public partial class LayoutNewName
    {
        private const string LangItem = "mpLayoutManager";
        private readonly List<string> _wrongSymbols = new List<string>
        {
            ">", "<", "/", "\\", "\"", ":", ";", "?", "*", "|", ",", "=", "`"
        };

        public List<string> LayoutsNames;

        public LayoutNewName()
        {
            InitializeComponent();
            Title = ModPlusAPI.Language.GetItem(LangItem, "h10");
        }

        private void BtAccept_OnClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TbNewName.Text))
            {
                ModPlusAPI.Windows.MessageBox.Show(ModPlusAPI.Language.GetItem(LangItem, "h11"), MessageBoxIcon.Alert);
                TbNewName.Focus();
                return;
            }

            if (_wrongSymbols.Any(wrongSymbol => TbNewName.Text.Contains(wrongSymbol)))
            {
                ModPlusAPI.Windows.MessageBox.Show(
                    $"{ModPlusAPI.Language.GetItem(LangItem, "h29")}:{Environment.NewLine}{string.Join(string.Empty, _wrongSymbols.ToArray())}",
                    MessageBoxIcon.Alert);
                TbNewName.Focus();
                return;
            }

            if (LayoutsNames.Contains(TbNewName.Text))
            {
                ModPlusAPI.Windows.MessageBox.Show(ModPlusAPI.Language.GetItem(LangItem, "h12"), MessageBoxIcon.Alert);
                TbNewName.Focus();
                return;
            }

            DialogResult = true;
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
    }
}