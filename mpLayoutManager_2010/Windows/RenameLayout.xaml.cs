using ModPlus;
using mpMsg;
using mpSettings;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace mpLayoutManager.Windows
{
    public partial class RenameLayout 
    {
        public List<string> LayoutsNames;


        public RenameLayout()
        {
            this.InitializeComponent();
            MpWindowHelpers.OnWindowStartUp(this, MpSettings.GetValue("Settings", "MainSet", "Theme"), MpSettings.GetValue("Settings", "MainSet", "AccentColor"), MpSettings.GetValue("Settings", "MainSet", "BordersType"));
            base.Loaded += new RoutedEventHandler(this.RenameLayout_Loaded);
        }

        private void BtAccept_OnClick(object sender, RoutedEventArgs e)
        {
            this.OnAccept();
        }

        private void BtCancel_OnClick(object sender, RoutedEventArgs e)
        {
            base.DialogResult = new bool?(false);
        }
        
        private void OnAccept()
        {
            if (string.IsNullOrEmpty(this.TbNewName.Text))
            {
                MpMsgWin.Show("Укажите имя листа");
                this.TbNewName.Focus();
            }
            else if (!this.LayoutsNames.Contains(this.TbNewName.Text))
            {
                base.DialogResult = new bool?(true);
            }
            else
            {
                MpMsgWin.Show(string.Concat("В текущем чертеже уже присутствует лист с таким именем!", Environment.NewLine, "Укажите другое имя"));
                this.TbNewName.Focus();
            }
        }

        private void RenameLayout_Loaded(object sender, RoutedEventArgs e)
        {
            this.LayoutsNames.Remove(this.TbCurrentName.Text);
            this.TbNewName.Focus();
            this.TbNewName.CaretIndex = this.TbNewName.Text.Length;
        }

        private void RenameLayout_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                base.DialogResult = new bool?(false);
            }
            if (e.Key == Key.Return)
            {
                this.OnAccept();
            }
        }
        
    }
}