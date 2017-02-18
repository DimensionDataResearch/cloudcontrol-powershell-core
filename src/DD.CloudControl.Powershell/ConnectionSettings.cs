using Newtonsoft.Json;
using System.Net;

namespace DD.CloudControl.Powershell
{
    /// <summary>
    ///     Settings for connecting to CloudControl.
    /// </summary>
    public class ConnectionSettings
    {
        /// <summary>
        ///     The connection name.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        ///     The target MCP region.
        /// </summary>
        [JsonProperty("region")]
        public string Region { get; set; }

        /// <summary>
        ///     The username for authenticating to CloudControl.
        /// </summary>
        [JsonProperty("user")]
        public string UserName { get; set; }

        /// <summary>
        ///     The password for authenticating to CloudControl.
        /// </summary>
        [JsonProperty("password")]
        public string Password { get; set; }

        /// <summary>
        ///     Is this connection the default connection?
        /// </summary>
        [JsonProperty("isDefault")]
        public bool IsDefault { get; set; }

        /// <summary>
        ///     Create a <see cref="NetworkCredential"/> representing the connection settings.
        /// </summary>
        /// <returns>
        ///     The configured <see cref="NetworkCredential"/>.
        /// </returns>
        public NetworkCredential CreateNetworkCredential() => new NetworkCredential(UserName, Password);
    }
}
