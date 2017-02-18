using System.Management.Automation;

namespace DD.CloudControl.Powershell.Connections
{
    using Utilities;

    /// <summary>
    ///     Cmdlet that creates a new connection to CloudControl.
    /// </summary>
    [Cmdlet(VerbsCommon.New, Nouns.Connection)]
    public class NewCloudControlConnection
        : PSCmdlet
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
        [Parameter(Mandatory = true, HelpMessage = "A name for the new connection")]
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
        public SwitchParameter SetDefault { get; set; }

        /// <summary>
		///		Perform Cmdlet processing.
		/// </summary>
		protected override void ProcessRecord()
        {
            WriteVerbose($"Create CloudControl connection '{Name}'...");

            SessionState.ReadConnections();

            if (SessionState.Connections().ContainsKey(Name))
            {
                WriteError(
                    Errors.ConnectionExists(Name)
                );

                return;
            }

            ConnectionSettings settings = new ConnectionSettings
            {
                Name = Name,
                Region = Region
            };
            if (Credentials != null)
            {
                settings.UserName = Credentials.UserName;
                settings.Password = Credentials.Password.ToInsecureString();
            }
            else
            {
                settings.UserName = User;
                settings.Password = Password;
            }
            SessionState.Connections().Add(settings.Name, settings);

            if (Default)
                SessionState.SetDefaultCloudControlConnection(Name);

            SessionState.WriteConnections();
        }
    }
}
