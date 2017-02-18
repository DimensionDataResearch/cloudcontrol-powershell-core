using System;
using System.Management.Automation;

namespace DD.CloudControl.Powershell
{
    using Client;

    /// <summary>
    ///     The base class for Cmdlets that use the CloudControl API.
    /// </summary>
    public abstract class CloudControlCmdlet
        : AsyncCmdlet
    {
        /// <summary>
        ///     The name of the CloudControl connection to use.
        /// </summary>
        [Parameter(HelpMessage = "The name of the CloudControl connection to use")]
        public string ConnectionName { get; set; }

        /// <summary>
        ///     Get or create a <see cref="CloudControlClient"/> for Cmdlet.
        /// </summary>
        /// <returns>
        ///     The <see cref="CloudControlClient"/>.
        /// </returns>
        protected CloudControlClient GetClient()
        {
            SessionState.ReadConnections();

            if (String.IsNullOrWhiteSpace(ConnectionName))
                ConnectionName = SessionState.GetDefaultCloudControlConnectionName();
                
            if (String.IsNullOrWhiteSpace(ConnectionName))
            {
                ThrowTerminatingError(
                    Errors.ConnectionRequired(this)
                );

                return null;
            }

            CloudControlClient client;
            if (!SessionState.Clients().TryGetValue(ConnectionName, out client))
            {
                ConnectionSettings connection;
                if (!SessionState.Connections().TryGetValue(ConnectionName, out connection))
                {
                    ThrowTerminatingError(
                        Errors.ConnectionDoesNotExist(ConnectionName)
                    );

                    return null;
                }

                client = CloudControlClient.Create(
                    new Uri($"https://api-{connection.Region.ToLower()}.dimensiondata.com/"),
                    connection.CreateNetworkCredential()
                );
                SessionState.Clients().Add(ConnectionName, client);
            }

            return client;
        }
    }
}