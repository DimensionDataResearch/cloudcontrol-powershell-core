using System.Management.Automation;

namespace DD.CloudControl.Powershell.Connections
{
    /// <summary>
    ///     Cmdlet that retrieves CloudControl connection details.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, Nouns.Connection)]
    public class GetCloudControlConnection
        : PSCmdlet
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
		///		Perform Cmdlet processing.
		/// </summary>
        protected override void ProcessRecord()
        {
            SessionState.ReadConnections();

            ConnectionSettings connection;
            if (SessionState.Connections().TryGetValue(Name, out connection))
                WriteObject(connection);
        }
    }
}
