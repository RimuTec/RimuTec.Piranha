using Piranha.Extend;

namespace RimuTec.Piranha.Data.NH
{
    public class Module : IModule
    {
        public string Author => "RimuTec Ltd";

        public string Name => GetType().Assembly.FullName.Split(',')[0];

        public string Version => throw new System.NotImplementedException();

        public string Description => throw new System.NotImplementedException();

        public string PackageUrl => throw new System.NotImplementedException();

        public string IconUrl => throw new System.NotImplementedException();

        public void Init()
        {
            throw new System.NotImplementedException();
        }
    }
}
