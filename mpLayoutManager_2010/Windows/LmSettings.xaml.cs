using ModPlus;
using mpSettings;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace mpLayoutManager.Windows
{
    public partial class LmSettings
    {
        public List<string> LayoutsNames;
        

        public LmSettings()
        {
            InitializeComponent();
            MpWindowHelpers.OnWindowStartUp(this, MpSettings.GetValue("Settings", "MainSet", "Theme"), MpSettings.GetValue("Settings", "MainSet", "AccentColor"), MpSettings.GetValue("Settings", "MainSet", "BordersType"));
            base.Loaded += LmSettings_Loaded;
        }

        private void BtClose_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ChkAddToMpPalette_OnChecked_OnUnchecked(object sender, RoutedEventArgs e)
        {
            bool flag = (!ChkAddToMpPalette.IsChecked.HasValue ? false : ChkAddToMpPalette.IsChecked.Value);
            MpSettings.SetValue("Settings", "mpLayoutManager", "AddToMpPalette", flag.ToString(), true);
        }

        private void ChkAskLayoutName_OnChecked_OnUnchecked(object sender, RoutedEventArgs e)
        {
            bool flag = (!ChkAskLayoutName.IsChecked.HasValue ? false : ChkAskLayoutName.IsChecked.Value);
            MpSettings.SetValue("Settings", "mpLayoutManager", "AskLayoutName", flag.ToString(), true);
        }

        private void ChkAutoLoad_OnChecked_OnUnchecked(object sender, RoutedEventArgs e)
        {
            bool flag = (!ChkAutoLoad.IsChecked.HasValue ? false : ChkAutoLoad.IsChecked.Value);
            MpSettings.SetValue("Settings", "mpLayoutManager", "AutoLoad", flag.ToString(), true);
        }

        private void ChkOpenNewLayout_OnChecked_OnUnchecked(object sender, RoutedEventArgs e)
        {
            bool flag = (!ChkOpenNewLayout.IsChecked.HasValue ? false : ChkOpenNewLayout.IsChecked.Value);
            MpSettings.SetValue("Settings", "mpLayoutManager", "OpenNewLayout", flag.ToString(), true);
        }

        private void ChkShowModel_OnChecked_OnUnchecked(object sender, RoutedEventArgs e)
        {
            bool flag = (!ChkShowModel.IsChecked.HasValue ? false : ChkShowModel.IsChecked.Value);
            MpSettings.SetValue("Settings", "mpLayoutManager", "ShowModel", flag.ToString(), true);
        }
        

        private void LmSettings_Loaded(object sender, RoutedEventArgs e)
        {
            bool flag;
            ChkAutoLoad.IsChecked = new bool?(bool.TryParse(MpSettings.GetValue("Settings", "mpLayoutManager", "AutoLoad"), out flag) & flag);
            ChkAddToMpPalette.IsChecked = new bool?(!bool.TryParse(MpSettings.GetValue("Settings", "mpLayoutManager", "AddToMpPalette"), out flag) | flag);
            ChkOpenNewLayout.IsChecked = new bool?(bool.TryParse(MpSettings.GetValue("Settings", "mpLayoutManager", "OpenNewLayout"), out flag) & flag);
            ChkShowModel.IsChecked = new bool?(!bool.TryParse(MpSettings.GetValue("Settings", "mpLayoutManager", "ShowModel"), out flag) | flag);
            ChkAskLayoutName.IsChecked = new bool?(!bool.TryParse(MpSettings.GetValue("Settings", "mpLayoutManager", "AskLayoutName"), out flag) | flag);
        }

        private void LmSettings_OnKeyDown(object sender, KeyEventArgs e)
        {
            Close();
        }
        
    }
}