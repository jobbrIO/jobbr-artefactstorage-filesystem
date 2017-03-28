using System;
using Jobbr.ComponentModel.Registration;

namespace Jobbr.ArtefactStorage.FileSystem
{
    public class ConfigurationValidator : IConfigurationValidator
    {
        public Type ConfigurationType { get; set; } = typeof(FileSystemArtefactStorageConfiguration);

        public bool Validate(object configuration)
        {
            var config = configuration as FileSystemArtefactStorageConfiguration;
            if (config == null)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(config.DataDirectory))
            {
                return false;
            }

            return true;
        }
    }
}
