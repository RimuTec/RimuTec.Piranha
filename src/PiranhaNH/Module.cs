using Piranha;
using Piranha.Extend;

namespace RimuTec.PiranhaNH
{
    public class Module : IModule
    {
        public string Author => "RimuTec Ltd";

        public string Name => GetType().Assembly.FullName.Split(',')[0];

        public string Version => Utils.GetAssemblyVersion(GetType().Assembly);

        public string Description => "Data implementation for NHibnernate.";

        public string PackageUrl => "tbd";

        public string IconUrl => "tbd";

        public void Init()
        {
        }
    }
}
