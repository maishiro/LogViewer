using System.Xml.Linq;

namespace LogViewer.Services
{
    public class XmlConfigService
    {
        private readonly string _configPath;

        public XmlConfigService( string configPath )
        {
            _configPath = configPath;
        }

        public XElement LoadConfig()
        {
            return XElement.Load( _configPath );
        }
    }
}
