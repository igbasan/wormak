using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace AuthenticationService.WebAPI.Logic.Implementations
{
    public class EnvConfiguration : IConfiguration
    {
        readonly IConfiguration configuration;
        readonly string DOCKER_SECRET_PATH;
        readonly string DOCKER_SECRET_FILENAME = "secrets.json";
        internal Dictionary<string, string> secrets;

        public EnvConfiguration(IConfiguration configuration, string dockerSecretPath)
        {
            this.configuration = configuration ?? throw new ArgumentNullException("configuration");
            this.DOCKER_SECRET_PATH = dockerSecretPath ?? throw new ArgumentNullException("dockerSecretPath");
        }
        public string this[string key] { get => GetSecretOrEnvVar(key); set => configuration[key] = value; }

        public IEnumerable<IConfigurationSection> GetChildren()
        {
            return configuration.GetChildren();
        }

        public IChangeToken GetReloadToken()
        {
            return configuration.GetReloadToken();
        }

        public IConfigurationSection GetSection(string key)
        {
            return configuration.GetSection(key);
        }
        public string GetSecretOrEnvVar(string key)
        {
            if (secrets == null && Directory.Exists(DOCKER_SECRET_PATH))
            {
                IFileProvider provider = new PhysicalFileProvider(DOCKER_SECRET_PATH);
                IFileInfo fileInfo = provider.GetFileInfo(DOCKER_SECRET_FILENAME);
                if (fileInfo.Exists)
                {
                    using (var stream = fileInfo.CreateReadStream())
                    using (var streamReader = new StreamReader(stream))
                    {
                        var jsonString = streamReader.ReadToEnd();
                        secrets = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonString);
                    }
                }
            }
            string result = string.Empty;
            if (secrets?.TryGetValue(key, out result) ?? false)
            {
                return result;
            }

            return configuration.GetValue<string>(key);
        }
    }
}
