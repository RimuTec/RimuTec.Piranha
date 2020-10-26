using Piranha.Extend;

namespace RimuTec.Piranha.Data.NH
{
    public class Module : IModule
    {
        public string Author => "RimuTec Ltd";

        public string Name => GetType().Assembly.FullName.Split(',')[0];

        public string Version => GetType().Assembly.FullName.Split(',')[1].Trim().Split('=')[1];

        public string Description => "Data implementation for NHibnernate.";

        public string PackageUrl => "tbd";

        public string IconUrl => "tbd";

        public void Init()
        {
            throw new System.NotImplementedException();
        }
    }
}
