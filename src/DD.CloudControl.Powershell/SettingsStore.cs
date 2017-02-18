using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DD.CloudControl.Powershell
{
    /// <summary>
    ///     Persistence for CloudControl Powershell module settings.
    /// </summary>
    public static class SettingsStore
    {
        /// <summary>
        ///     The directory used to store settings for the CloudControl powershell module.
        /// </summary>
        public static DirectoryInfo SettingsDirectory => new DirectoryInfo(Path.Combine(
            Environment.GetEnvironmentVariable("HOME"), ".mcp"
        ));

        /// <summary>
        ///     The file used to store connection settings.
        /// </summary>
        public static FileInfo ConnectionSettingsFile => new FileInfo(Path.Combine(
            SettingsDirectory.FullName, "connection-settings.json"
        ));

        /// <summary>
        ///     Ensure that the settings directory exists.
        /// </summary>
        /// <returns>
        ///     A <see cref="DirectoryInfo"/> representing the settings directory.
        /// </returns>
        public static DirectoryInfo EnsureSettingsDirectory()
        {
            DirectoryInfo settingsDirectory = SettingsDirectory;
            if (!settingsDirectory.Exists)
            {
                settingsDirectory.Create();

                // On Windows, dotfiles aren't hidden just because they're dotfiles.
                if (OS.IsWindows)
                    settingsDirectory.Attributes |= FileAttributes.Hidden;
            }

            return settingsDirectory;
        }

        /// <summary>
        ///     Write connection settings to the connection-settings.json.
        /// </summary>
        /// <param name="connectionSettings">
        ///     The connection settings.
        /// </param>
        /// <returns>
        ///     The connection settings.
        /// </returns>
        public static List<ConnectionSettings> ReadConnectionSettings()
        {
            if (!ConnectionSettingsFile.Exists)
                return new List<ConnectionSettings>();

            using (TextReader settingsReader = ConnectionSettingsFile.OpenText())
            {
                JsonSerializer serializer = CreateStoreSerializer();
                
                return serializer.Deserialize<List<ConnectionSettings>>(
                    new JsonTextReader(settingsReader)
                );
            }
        }

        /// <summary>
        ///     Write connection settings to the connection-settings.json.
        /// </summary>
        /// <param name="connections">
        ///     The connection settings.
        /// </param>
        public static void WriteConnectionSettings(Dictionary<string, ConnectionSettings> connections)
        {
            if (connections == null)
                throw new ArgumentNullException(nameof(connections));

            // Sort by name before persisting.
            WriteConnectionSettings(
                connections.Keys.OrderBy(name => name).Select(
                    name => connections[name]
                )
            );
        }

        /// <summary>
        ///     Write connection settings to the connection-settings.json.
        /// </summary>
        /// <param name="connectionSettings">
        ///     The connection settings.
        /// </param>
        public static void WriteConnectionSettings(IEnumerable<ConnectionSettings> connectionSettings)
        {
            if (connectionSettings == null)
                throw new ArgumentNullException(nameof(connectionSettings));

            EnsureSettingsDirectory();

            if (ConnectionSettingsFile.Exists)
                ConnectionSettingsFile.Delete();

            using (TextWriter settingsWriter = ConnectionSettingsFile.CreateText())
            {
                JsonSerializer serializer = CreateStoreSerializer();
                serializer.Serialize(settingsWriter, connectionSettings);
                
                settingsWriter.Flush();
            }
        }

        /// <summary>
        ///     Create a <see cref="JsonSerializer"/> for use by the store.
        /// </summary>
        /// <returns>
        ///     The configured <see cref="JsonSerializer"/>.
        /// </returns>
        static JsonSerializer CreateStoreSerializer()
        {
            return JsonSerializer.Create(new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            });
        }
    }
}