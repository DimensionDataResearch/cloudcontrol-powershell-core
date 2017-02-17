using System.Management.Automation;
using System.Threading;
using System.Threading.Tasks;

namespace DD.CloudControl.Powershell.Connections
{
    /// <summary>
    ///     Cmdlet that retrieves CloudControl connection details.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, Nouns.Connection)]
    public class NewCloudControlConnection
        : AsyncCmdlet
    {
        /// <summary>
        ///     Create a new <see cref="NewCloudControlConnection"/> cmdlet.
        /// </summary>
        public NewCloudControlConnection()
        {
        }

        /// <summary>
        ///     The Id of the target MCP region (e.g. AU9).
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "The Id of the target MCP region (e.g. AU9)")]
        public string Region { get; set; }

        /// <summary>
        ///     A name for the new connection.
        /// </summary>
        [Parameter(HelpMessage = "A name for the new connection")]
        public string Name { get; set; }

        /// <summary>
        ///     The credentials used to authenticate to CloudControl.
        /// </summary>
        [Parameter(ParameterSetName = "Credentials", Mandatory = true, HelpMessage = "The credentials used to authenticate to CloudControl")]
        public PSCredential Credentials { get; set; }

        /// <summary>
        ///     The username used to authenticate to CloudControl.
        /// </summary>
        [Parameter(ParameterSetName = "Username and password", Mandatory = true, HelpMessage = "The username used to authenticate to CloudControl")]
        public string User { get; set; }

        /// <summary>
        ///     The password used to authenticate to CloudControl.
        /// </summary>
        [Parameter(ParameterSetName = "Username and password", Mandatory = true, HelpMessage = "The password used to authenticate to CloudControl")]
        public string Password { get; set; }

        /// <summary>
        ///     Mark the new connection as the default connection.
        /// </summary>
        [Parameter(HelpMessage = "Mark the new connection as the default connection")]
        public SwitchParameter Default { get; set; }

        /// <summary>
		///		Asynchronously perform Cmdlet processing.
		/// </summary>
		/// <param name="cancellationToken">
		///		A <see cref="CancellationToken"/> that can be used to cancel the asynchronous operation.
		/// </param>
		/// <returns>
		///		A <see cref="Task"/> representing the asynchronous operation.
		/// </returns>
        protected override async Task ProcessRecordAsync(CancellationToken cancellationToken)
        {
            // TODO: Implement persistence for connection settings (~/.mcp/connection-settings.json).

            cancellationToken.ThrowIfCancellationRequested();

            WriteVerbose("Yielding...");

            await Task.Yield();

            WriteVerbose("Hello from Powershell core!");
        }
    }
}
