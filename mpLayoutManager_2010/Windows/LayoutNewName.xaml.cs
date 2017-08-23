using ModPlus;
using mpMsg;
using mpSettings;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace mpLayoutManager.Windows
{
    public partial class LayoutNewName 
    {
        public List<string> LayoutsNames;
        
        public LayoutNewName()
        {
            InitializeComponent();
            MpWindowHelpers.OnWindowStartUp(this, MpSettings.GetValue("Settings", "MainSet", "Theme"), MpSettings.GetValue("Settings", "MainSet", "AccentColor"), MpSettings.GetValue("Settings", "MainSet", "BordersType"));
        }

        private void BtAccept_OnClick(object sender, RoutedEventArgs e)
        {
            OnAccept();
        }

        private void BtCancel_OnClick(object sender, RoutedEventArgs e)
        {
            base.DialogResult = new bool?(false);
        }
        
        private void LayoutNewName_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                base.DialogResult = new bool?(false);
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
                MpMsgWin.Show("Укажите имя листа");
                TbNewName.Focus();
            }
            else if (!LayoutsNames.Contains(TbNewName.Text))
            {
                base.DialogResult = new bool?(true);
            }
            else
            {
                MpMsgWin.Show(string.Concat("В текущем чертеже уже присутствует лист с таким именем!", Environment.NewLine, "Укажите другое имя"));
                TbNewName.Focus();
            }
        }
        
    }
}