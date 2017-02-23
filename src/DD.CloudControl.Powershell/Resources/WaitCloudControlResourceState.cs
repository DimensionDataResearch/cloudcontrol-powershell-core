using PSReptile;
using System;
using System.Management.Automation;
using System.Threading;
using System.Threading.Tasks;

namespace DD.CloudControl.Powershell.Resources
{
    using Client;
    using Client.Models;
    using Client.Models.Network;

    /// <summary>
    ///     Cmdlet that waits for a CloudControl resource to reach the specified state.
    /// </summary>
    [OutputType(typeof(Resource))]
    [Cmdlet(VerbsLifecycle.Wait, Nouns.ResourceState)]
    [CmdletSynopsis("Wait for a resource to reach the specified status")]
    [CmdletDescription(@"
        Can wait for any CloudControl resource with a ""State"" field (e.g. network domain, VLAN, etc).
    ")]
    public class WaitCloudControlResourceState
        : CloudControlCmdlet
    {
        /// <summary>
        ///     Wait for a network domain.
        /// </summary>
        [Parameter(Position = 0, ParameterSetName = "Network domain", Mandatory = true, HelpMessage = "Wait for a network domain")]
        public SwitchParameter NetworkDomain { get; set; }

        /// <summary>
        ///     Wait for a VLAN.
        /// </summary>
        [Parameter(Position = 0, ParameterSetName = "VLAN", Mandatory = true, HelpMessage = "Wait for a VLAN")]
        public SwitchParameter VLAN { get; set; }

        /// <summary>
        ///     The Id of the target resource.
        /// </summary>
        [Parameter(Position = 1, ParameterSetName = "Network domain", Mandatory = true, HelpMessage = "The Id of the target resource")]
        [Parameter(Position = 1, ParameterSetName = "VLAN", Mandatory = true, HelpMessage = "The Id of the target resource")]
        public Guid Id { get; set; }

        /// <summary>
        ///     The state to wait for.
        /// </summary>
        [Parameter(Position = 2, ParameterSetName = "Network domain", Mandatory = true, HelpMessage = "The state to wait for")]
        [Parameter(Position = 2, ParameterSetName = "VLAN", Mandatory = true, HelpMessage = "The state to wait for")]
        public ResourceState State { get; set; }

        /// <summary>
        ///     The amount of time to wait.
        /// </summary>
        [Parameter(Position = 3, ParameterSetName = "Network domain", HelpMessage = "The amount of time to wait (default is 30 seconds)")]
        [Parameter(Position = 3, ParameterSetName = "VLAN", HelpMessage = "The amount of time to wait (default is 30 seconds)")]
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);

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

            WriteVerbose(
                $"Waiting {Timeout.TotalSeconds} seconds for {ParameterSetName} with Id '{Id}' to reach state '{State}'..."
            );

            Resource resource;
            switch (ParameterSetName)
            {
                case "Network domain":
                {
                    resource = await client.WaitForState<NetworkDomain>(Id, State, Timeout, cancellationToken);

                    break;
                }
                case "VLAN":
                {
                    resource = await client.WaitForState<Vlan>(Id, State, Timeout, cancellationToken);

                    break;
                }
                default:
                {
                    ThrowTerminatingError(
                        Errors.UnrecognizedParameterSet(this)
                    );

                    return;
                }
            }

            if (resource != null)
            {
                WriteVerbose(
                    $"{ParameterSetName} with Id '{Id}' has reached state '{State}'."
                );
                WriteObject(resource);
            }
        }
    }
}