using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using ModPlusAPI.Windows;
using ModPlusAPI.Windows.Helpers;

namespace mpLayoutManager.Windows
{
    public partial class RenameLayout 
    {
        public List<string> LayoutsNames;


        public RenameLayout()
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
        
        private void OnAccept()
        {
            if (string.IsNullOrEmpty(TbNewName.Text))
            {
                ModPlusAPI.Windows.MessageBox.Show("Укажите имя листа", MessageBoxIcon.Alert);
                TbNewName.Focus();
            }
            else if (!LayoutsNames.Contains(TbNewName.Text))
            {
                DialogResult = true;
            }
            else
            {
                ModPlusAPI.Windows.MessageBox.Show(string.Concat("В текущем чертеже уже присутствует лист с таким именем!", 
                    Environment.NewLine, "Укажите другое имя"), MessageBoxIcon.Alert);
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