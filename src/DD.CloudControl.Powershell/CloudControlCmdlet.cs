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

        /// <summary>
        ///     Get the Cmdlet's current paging configuration (if any).
        /// </summary>
        /// <returns>
        ///     The configuration as a <see cref="Paging"/>, or <c>null</c> if no paging has been configured.
        /// </returns>
        protected Paging GetPagingConfiguration()
        {
            if (PagingParameters.First == 0 || PagingParameters.First == UInt64.MaxValue)
                return null;

            return new Paging
            {
                PageSize = (int)PagingParameters.First,
                PageNumber = (int)(PagingParameters.Skip / PagingParameters.First) + 1 // Skip 0 = Page 1
            };
        }

        /// <summary>
        ///     Write an <see cref="ErrorRecord"/> to the pipline that indicates the Cmdlet has been passed an invalid parameter.
        /// </summary>
        /// <param name="parameterName">
        ///     The name of the invalid parameter.
        /// </param>
        /// <param name="messageOrFormat">
        ///     The error message or message-format specifier.
        /// </param>
        /// <param name="formatArguments">
        ///     Optional message format arguments.
        /// </param>
        /// <returns>
        ///     The configured <see cref="ErrorRecord"/>.
        /// </returns>
        protected void WriteInvalidParameter(string parameterName, string messageOrFormat, params object[] formatArguments)
        {
            WriteError(
                Errors.InvalidParameter(this, parameterName, messageOrFormat, formatArguments)
            );
        }

		/// <summary>
		/// 	Throw a terminating <see cref="ErrorRecord"/> that indicates a Cmdlet (or part of its functionality) is not currently implemented.
		/// </summary>
		/// <param name="messageOrFormat">
		/// 	An optional error message or message-format specifier.
		/// </param>
		/// <param name="formatArguments">
		/// 	Optional message format arguments.
		/// </param>
		protected void ThrowNotImplemented(string messageOrFormat = null, params object[] formatArguments)
		{
			ThrowTerminatingError(
				Errors.NotImplemented(this, messageOrFormat, formatArguments)
			);
		}

        /// <summary>
		///		Throw a terminating <see cref="ErrorRecord"/> that indicates a Cmdlet was passed an unrecognised combination of parameters.
		/// </summary>
		/// <returns>
		///		The configured <see cref="ErrorRecord"/>.
		/// </returns>
		protected void ThrowUnrecognizedParameterSet(PSCmdlet cmdlet)
        {
            ThrowTerminatingError(
                Errors.UnrecognizedParameterSet(this)
            );
        }
    }
}