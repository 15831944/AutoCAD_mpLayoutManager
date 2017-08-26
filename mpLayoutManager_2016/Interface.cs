using System.Collections.Generic;
using ModPlusAPI.Interfaces;

namespace mpLayoutManager
{
    public class Interface : IModPlusFunctionInterface
    {
        public SupportedProduct SupportedProduct => SupportedProduct.AutoCAD;
        public string Name => "mpLayoutManager";
        public string AvailProductExternalVersion => "2016";
        public string ClassName => string.Empty;
        public string LName => "Менеджер листов";
        public string Description => "Менеджер листов чертежа, отображаемый в палитре";
        public string Author => "Пекшев Александр aka Modis";
        public string Price => "0";
        public bool CanAddToRibbon => true;
        public string FullDescription => "";
        public string ToolTipHelpImage => string.Empty;
        public List<string> SubFunctionsNames => new List<string>();
        public List<string> SubFunctionsLames => new List<string>();
        public List<string> SubDescriptions => new List<string>();
        public List<string> SubFullDescriptions => new List<string>();
        public List<string> SubHelpImages => new List<string>();
        public List<string> SubClassNames => new List<string>();
    }
}