using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using ModPlusAPI.Windows.Helpers;
using mpWin = ModPlusAPI.Windows;

namespace mpLayoutManager.Windows
{
    public partial class LayoutNewName
    {
        public List<string> LayoutsNames;

        public LayoutNewName()
        {
            InitializeComponent();
            this.OnWindowStartUp();
        }

        private void BtAccept_OnClick(object sender, RoutedEventArgs e)
        {
            OnAccept();
        }

        private void BtCancel_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void LayoutNewName_OnKeyDown(object sender, KeyEventArgs e)
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

        private void LayoutNewName_OnLoaded(object sender, RoutedEventArgs e)
        {
            TbNewName.Focus();
            TbNewName.CaretIndex = TbNewName.Text.Length;
        }

        private void OnAccept()
        {
            if (string.IsNullOrEmpty(TbNewName.Text))
            {
                mpWin.MessageBox.Show("Укажите имя листа", mpWin.MessageBoxIcon.Alert);
                TbNewName.Focus();
            }
            else if (!LayoutsNames.Contains(TbNewName.Text))
            {
                DialogResult = true;
            }
            else
            {
                mpWin.MessageBox.Show(string.Concat("В текущем чертеже уже присутствует лист с таким именем!",
                    Environment.NewLine, "Укажите другое имя"), mpWin.MessageBoxIcon.Alert);
                TbNewName.Focus();
            }
        }

    }
}