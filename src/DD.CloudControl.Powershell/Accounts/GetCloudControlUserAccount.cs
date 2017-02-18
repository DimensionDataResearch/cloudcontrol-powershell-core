using System;
using System.Management.Automation;
using System.Threading;
using System.Threading.Tasks;

namespace DD.CloudControl.Powershell.Accounts
{
    using Client;

    /// <summary>
    ///     Cmdlet that retrieves information about one or more CloudControl accounts.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, Nouns.UserAccount)]
    public class GetCloudControlUserAccount
        : AsyncCmdlet
    {
        /// <summary>
        ///     The name of the CloudControl connection to use.
        /// </summary>
        [Parameter(HelpMessage = "The name of the CloudControl connection to use")]
        public string ConnectionName { get; set; }

        /// <summary>
        ///     Retrieve the current user's account details.
        /// </summary>
        [Parameter(Mandatory = true)]
        public SwitchParameter My { get; set; }

        /// <summary>
        ///     Asynchronously perform Cmdlet processing.
        /// </summary>
        /// <param name="cancellationToken">
        ///     A <see cref="CancellationToken"/> that can be used to cancel Cmdlet processing.
        /// </param>
        /// <returns>
        ///     A <see cref="Task"/> representing the asynchronous operation.
        /// </returns>
        protected override async Task ProcessRecordAsync(CancellationToken cancellationToken)
        {
            CloudControlClient client = GetClient();

            WriteObject(
                await client.GetAccount(cancellationToken)
            );
        }

        // TODO: Move this to CloudControlCmdlet base class.

        /// <summary>
        ///     Get or create a <see cref="CloudControlClient"/> for Cmdlet.
        /// </summary>
        /// <returns>
        ///     The <see cref="CloudControlClient"/>.
        /// </returns>
        CloudControlClient GetClient()
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
                    new Uri($"https://api-{connection.Region}.dimensiondata.com/"),
                    connection.CreateNetworkCredential()
                );
                SessionState.Clients().Add(ConnectionName, client);
            }

            return client;
        }
    }
}