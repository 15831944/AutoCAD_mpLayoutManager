namespace mpLayoutManager.Windows
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using ModPlusAPI.Windows;

    public partial class RenameLayout 
    {
        private const string LangItem = "mpLayoutManager";
        private readonly List<string> wrongSymbols = new List<string>
        {
            ">","<","/","\\","\"",":",";","?","*","|",",","=","`"
        };

        public List<string> LayoutsNames;

        public RenameLayout()
        {
            InitializeComponent();
            Title = ModPlusAPI.Language.GetItem(LangItem, "h26");
            Loaded += RenameLayout_Loaded;
        }

        private void BtAccept_OnClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TbNewName.Text))
            {
                ModPlusAPI.Windows.MessageBox.Show(ModPlusAPI.Language.GetItem(LangItem, "h11"), MessageBoxIcon.Alert);
                TbNewName.Focus();
                return;
            }

            if (wrongSymbols.Any(wrongSymbol => TbNewName.Text.Contains(wrongSymbol)))
            {
                ModPlusAPI.Windows.MessageBox.Show(
                    $"{ModPlusAPI.Language.GetItem(LangItem, "h29")}:{Environment.NewLine}{string.Join("", wrongSymbols.ToArray())}",
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

        private void RenameLayout_Loaded(object sender, RoutedEventArgs e)
        {
            LayoutsNames.Remove(TbCurrentName.Text);
            TbNewName.Focus();
            TbNewName.CaretIndex = TbNewName.Text.Length;
        }
    }
}