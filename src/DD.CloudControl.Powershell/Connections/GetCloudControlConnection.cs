using System.Management.Automation;
using System.Threading;
using System.Threading.Tasks;

namespace DD.CloudControl.Powershell.Connections
{
    /// <summary>
    ///     Cmdlet that retrieves CloudControl connection details.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, Nouns.Connection)]
    public class GetCloudControlConnection
        : AsyncCmdlet
    {
        /// <summary>
        ///     Create a new <see cref="GetCloudControlConnection"/> cmdlet.
        /// </summary>
        public GetCloudControlConnection()
        {
        }

        /// <summary>
        ///     The name of a specific connection to retrieve.
        /// </summary>
        [Parameter(HelpMessage = "The name of a specific connection to retrieve")]
        public string Name { get; set; }

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
            // TODO: Define and implement persistence for connection settings (~/.mcp/connection-settings.json).

            cancellationToken.ThrowIfCancellationRequested();

            WriteVerbose("Yielding...");

            await Task.Yield();

            WriteVerbose("Hello from Powershell core!");
        }
    }
}
