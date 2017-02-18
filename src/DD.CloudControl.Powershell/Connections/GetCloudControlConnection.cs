using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

namespace DD.CloudControl.Powershell.Connections
{
    /// <summary>
    ///     Cmdlet that retrieves CloudControl connection details.
    /// </summary>
    [OutputType(typeof(ConnectionSettings))]
    [Cmdlet(VerbsCommon.Get, Nouns.Connection, DefaultParameterSetName = "All")]
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
        ///     Retrieve all connections.
        /// </summary>
        [Parameter(ParameterSetName = "All", HelpMessage = "Retrieve all connections")]
        public SwitchParameter All { get; set; }

        /// <summary>
        ///     The name of a specific connection to retrieve.
        /// </summary>
        [Parameter(ParameterSetName = "By Name", Mandatory = true, HelpMessage = "The name of a specific connection to retrieve")]
        public string Name { get; set; }

        /// <summary>
		///		Perform Cmdlet processing.
		/// </summary>
        protected override void ProcessRecord()
        {
            SessionState.ReadConnections();

            switch (ParameterSetName)
            {
                case "All":
                {
                    IEnumerable<ConnectionSettings> connections =
                        SessionState.Connections().Values.OrderBy(
                            connection => connection.Name
                        );

                    foreach (ConnectionSettings connection in connections)
                        WriteObject(connection);

                    break;
                }
                case "By Name":
                {
                    ConnectionSettings connection;
                    if (SessionState.Connections().TryGetValue(Name, out connection))
                        WriteObject(connection);

                    break;
                }
                default:
                {
                    ThrowTerminatingError(
                        Errors.UnrecognizedParameterSet(this)
                    );

                    break;
                }
            }

            
        }
    }
}
