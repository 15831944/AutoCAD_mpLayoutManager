using mpPInterface;

namespace mpLayoutManager
{
    public class Interface : IPluginInterface
    {
        public string Name => "mpLayoutManager";
        public string AvailCad => "2017";
        public string LName => "Менеджер листов";
        public string Description => "Менеджер листов чертежа, отображаемый в палитре";
        public string Author => "Пекшев Александр aka Modis";
        public string Price => "0";
    }
}