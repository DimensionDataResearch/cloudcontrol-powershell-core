using System;
using System.Management.Automation;
using System.Collections.Generic;

namespace DD.CloudControl.Powershell.Connections
{
    using Client;

    /// <summary>
    ///     Cmdlet that creates a new connection to CloudControl.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, Nouns.Connection)]
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
        protected override void ProcessRecord()
        {
            WriteVerbose($"Create CloudControl connection '{Name}'...");

            // TODO: Define and implement persistence for connection settings (~/.mcp/connection-settings.json).

            Dictionary<string, CloudControlClient> clients = SessionState.GetCloudControlClients();
            if (clients.ContainsKey(Name))
            {
                WriteError(
                    Errors.ConnectionExists(Name)
                );

                return;
            }

            // TODO: Support PSCredential, too.
            CloudControlClient client = CloudControlClient.Create(
                new Uri($"https://api-{Region}.dimensiondata.com/"),
                userName: User,
                password: Password
            );
            clients.Add(Name, client);
        }
    }
}
