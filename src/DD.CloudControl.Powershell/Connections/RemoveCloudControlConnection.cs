using System.Management.Automation;

namespace DD.CloudControl.Powershell.Connections
{
    using Client;

    /// <summary>
    ///     Cmdlet that removes a connection to CloudControl.
    /// </summary>
    [Cmdlet(VerbsCommon.Remove, Nouns.Connection)]
    public class RemoveCloudControlConnection
        : PSCmdlet
    {
        /// <summary>
        ///     Create a new <see cref="RemoveCloudControlConnection"/> cmdlet.
        /// </summary>
        public RemoveCloudControlConnection()
        {
        }

        /// <summary>
        ///     The name of the connection to remove.
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "The name of the connection to remove")]
        public string Name { get; set; }

        /// <summary>
		///		Perform Cmdlet processing.
		/// </summary>
		protected override void ProcessRecord()
        {
            WriteVerbose($"Remove CloudControl connection '{Name}'...");

            SessionState.ReadConnections();

            CloudControlClient client;
            if (SessionState.Clients().TryGetValue(Name, out client))
            {
                WriteVerbose($"Closing connection '{Name}'...");
                client.Dispose();
                WriteVerbose($"Closed connection '{Name}'.");
            }

            if (!SessionState.Connections().Remove(Name))
            {
                WriteError(
                    Errors.ConnectionDoesNotExist(Name)
                );

                return;
            }

            SessionState.WriteConnections();
        }
    }
}
