using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using ModPlusAPI;

namespace mpLayoutManager.Windows
{
    public partial class LmSettings
    {
        public List<string> LayoutsNames;
        

        public LmSettings()
        {
            InitializeComponent();
            Title = ModPlusAPI.Language.GetItem("mpLayoutManager", "h13");
            Loaded += LmSettings_Loaded;
        }

        private void BtClose_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ChkAddToMpPalette_OnChecked_OnUnchecked(object sender, RoutedEventArgs e)
        {
            var flag = ChkAddToMpPalette.IsChecked ?? false;
            UserConfigFile.SetValue(UserConfigFile.ConfigFileZone.Settings, "mpLayoutManager", "AddToMpPalette", flag.ToString(), true);
        }

        private void ChkAskLayoutName_OnChecked_OnUnchecked(object sender, RoutedEventArgs e)
        {
            var flag = ChkAskLayoutName.IsChecked ?? false;
            UserConfigFile.SetValue(UserConfigFile.ConfigFileZone.Settings, "mpLayoutManager", "AskLayoutName", flag.ToString(), true);
        }

        private void ChkAutoLoad_OnChecked_OnUnchecked(object sender, RoutedEventArgs e)
        {
            var flag = ChkAutoLoad.IsChecked ?? false;
            UserConfigFile.SetValue(UserConfigFile.ConfigFileZone.Settings, "mpLayoutManager", "AutoLoad", flag.ToString(), true);
        }

        private void ChkOpenNewLayout_OnChecked_OnUnchecked(object sender, RoutedEventArgs e)
        {
            var flag = ChkOpenNewLayout.IsChecked ?? false;
            UserConfigFile.SetValue(UserConfigFile.ConfigFileZone.Settings, "mpLayoutManager", "OpenNewLayout", flag.ToString(), true);
        }

        private void ChkShowModel_OnChecked_OnUnchecked(object sender, RoutedEventArgs e)
        {
            var flag = ChkShowModel.IsChecked ?? false;
            UserConfigFile.SetValue(UserConfigFile.ConfigFileZone.Settings, "mpLayoutManager", "ShowModel", flag.ToString(), true);
        }
        

        private void LmSettings_Loaded(object sender, RoutedEventArgs e)
        {
            ChkAutoLoad.IsChecked = bool.TryParse(UserConfigFile.GetValue(UserConfigFile.ConfigFileZone.Settings, "mpLayoutManager", "AutoLoad"), out bool flag) & flag;
            ChkAddToMpPalette.IsChecked = !bool.TryParse(UserConfigFile.GetValue(UserConfigFile.ConfigFileZone.Settings, "mpLayoutManager", "AddToMpPalette"), out flag) | flag;
            ChkOpenNewLayout.IsChecked = bool.TryParse(UserConfigFile.GetValue(UserConfigFile.ConfigFileZone.Settings, "mpLayoutManager", "OpenNewLayout"), out flag) & flag;
            ChkShowModel.IsChecked = !bool.TryParse(UserConfigFile.GetValue(UserConfigFile.ConfigFileZone.Settings, "mpLayoutManager", "ShowModel"), out flag) | flag;
            ChkAskLayoutName.IsChecked = !bool.TryParse(UserConfigFile.GetValue(UserConfigFile.ConfigFileZone.Settings, "mpLayoutManager", "AskLayoutName"), out flag) | flag;
        }

        private void LmSettings_OnKeyDown(object sender, KeyEventArgs e)
        {
            Close();
        }
    }
}