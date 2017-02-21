using PSReptile;
using System.Management.Automation;

namespace DD.CloudControl.Powershell.Connections
{
    using Client;

    /// <summary>
    ///     Cmdlet that removes a CloudControl connection.
    /// </summary>
    [Cmdlet(VerbsCommon.Remove, Nouns.Connection)]
    [CmdletSynopsis("Removes a CloudControl connection")]
    public class RemoveCloudControlConnection
        : PSCmdlet
    {
        /// <summary>
        ///     Create a new <see cref="RemoveCloudControlConnection"/> cmdlet.
        /// </summary>
        public RemoveCloudControlConnection()
        {
        }

        // TODO: Accept ConnectionInfo from pipeline (so there are 2 parameter sets - "Name" or "Connection").

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
