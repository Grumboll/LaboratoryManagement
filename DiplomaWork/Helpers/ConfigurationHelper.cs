using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DiplomaWork.Helpers
{
    public static class ConfigurationHelper
    {
        public static string getEntropyKey()
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var section = config.GetSection("appSettings") as AppSettingsSection;

            // Get the encrypted key from the configuration file
            return section.Settings["EncryptionKey"].Value;
        }
        
    }
}
