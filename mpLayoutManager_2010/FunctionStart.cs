#if ac2010
using AcApp = Autodesk.AutoCAD.ApplicationServices.Application;
#elif ac2013
using AcApp = Autodesk.AutoCAD.ApplicationServices.Core.Application;
#endif
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Windows;
using ModPlus;
using mpMsg;
using mpSettings;
using System;
using System.Collections;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

namespace mpLayoutManager
{
    public class FunctionStart : IExtensionApplication
    {
        private static PaletteSet _paletteSet;

        public FunctionStart()
        {
        }

        private void _paletteSet_Load(object sender, PalettePersistEventArgs e)
        {
            double num = (double)e.ConfigurationSection.ReadProperty("mpLayoutManager", 22.3);
        }

        private void _paletteSet_Save(object sender, PalettePersistEventArgs e)
        {
            e.ConfigurationSection.WriteProperty("mpLayoutManager", 32.3);
        }

        private void _paletteSet_StateChanged(object sender, PaletteSetStateEventArgs e)
        {
            try
            {
                if (e.NewState == StateEventIndex.Hide)
                {
                    _paletteSet = null;
                }
            }
            catch (System.Exception exception)
            {
                MpExWin.Show(exception);
            }
        }

        public static void AddToMpPalette(bool show)
        {
            PaletteSet mpPaletteSet = MpPalette.MpPaletteSet;
            if (mpPaletteSet != null)
            {
                bool flag = false;
                foreach (Palette palette in mpPaletteSet)
                {
                    if (palette.Name.Equals("Менеджер листов"))
                    {
                        flag = true;
                    }
                }
                if (!flag)
                {
                    LmPalette lmPalette = new LmPalette();
                    mpPaletteSet.Add("Менеджер листов", new ElementHost()
                    {
                        AutoSize = true,
                        Dock = DockStyle.Fill,
                        Child = lmPalette
                    });
                    if (show)
                    {
                        mpPaletteSet.Visible = true;
                    }
                }
            }
            if (_paletteSet != null)
            {
                _paletteSet.Visible = false;
            }
        }

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (args.Name.Contains("ModPlus_"))
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
                Start();
            }
            return null;
        }

        public void Initialize()
        {
            bool flag;
            bool flag1 = bool.TryParse(MpSettings.GetValue("Settings", "mpLayoutManager", "AutoLoad"), out flag) & flag;
            bool flag2 = bool.TryParse(MpSettings.GetValue("Settings", "mpLayoutManager", "AddToMpPalette"), out flag) & flag;
            if (flag1 & !flag2)
            {
                Start();
            }
            else if (flag1 & flag2)
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
            }
        }

        public static void RemoveFromMpPalette(bool fromSettings)
        {
            PaletteSet mpPaletteSet = MpPalette.MpPaletteSet;
            if (mpPaletteSet != null)
            {
                int num = 0;
                while (num < mpPaletteSet.Count)
                {
                    if (!mpPaletteSet[num].Name.Equals("Менеджер листов"))
                    {
                        num++;
                    }
                    else
                    {
                        mpPaletteSet.Remove(num);
                        break;
                    }
                }
            }
            if (_paletteSet != null)
            {
                _paletteSet.Visible = true;
            }
            else if (fromSettings)
            {
                if (AcApp.DocumentManager.MdiActiveDocument != null)
                {
                    AcApp.DocumentManager.MdiActiveDocument.SendStringToExecute("_MPLAYOUTMANAGER ", true, false, false);
                }
            }
        }

        [CommandMethod("ModPlus", "mpLayoutManager", CommandFlags.Modal)]
        public void Start()
        {
            bool flag;
            try
            {
                if (!(!bool.TryParse(MpSettings.GetValue("Settings", "mpLayoutManager", "AddToMpPalette"), out flag) | flag))
                {
                    RemoveFromMpPalette(false);
                    if (_paletteSet != null)
                    {
                        _paletteSet.Visible = true;
                    }
                    else
                    {
                        _paletteSet = new PaletteSet("MP: Менеджер листов", new Guid("CC48331E-B912-44DF-B592-D5EF66D7673E"));
                        _paletteSet.Load += _paletteSet_Load;
                        _paletteSet.Save += _paletteSet_Save;
                        LmPalette lmPalette = new LmPalette();
                        ElementHost elementHost = new ElementHost()
                        {
                            AutoSize = true,
                            Dock = DockStyle.Fill,
                            Child = lmPalette
                        };
                        _paletteSet.Add("MP: Менеджер листов", elementHost);
                        _paletteSet.Style = PaletteSetStyles.ShowCloseButton | PaletteSetStyles.ShowPropertiesMenu | PaletteSetStyles.ShowAutoHideButton;
                        _paletteSet.MinimumSize = new Size(100, 300);
                        _paletteSet.DockEnabled = DockSides.Right | DockSides.Left;
                        _paletteSet.Visible = true;
                    }
                }
                else
                {
                    if (_paletteSet != null)
                    {
                        _paletteSet.Visible = false;
                    }
                    AddToMpPalette(true);
                }
            }
            catch (System.Exception exception)
            {
                MpExWin.Show(exception);
            }
        }

        public void Terminate()
        {
        }
    }
}