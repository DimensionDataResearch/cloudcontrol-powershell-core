using PSReptile;
using System;
using System.Management.Automation;
using System.Threading;
using System.Threading.Tasks;

namespace DD.CloudControl.Powershell.NetworkDomains
{
    using Client;
    using Client.Models.Network;

    /// <summary>
    ///     Cmdlet that creates a new VLAN.
    /// </summary>
    [OutputType(typeof(NetworkDomain))]
    [Cmdlet(VerbsCommon.New, Nouns.Vlan, SupportsShouldProcess = true)]
    [CmdletSynopsis("Creates a new VLAN")]
    public class NewCloudControlVlan
        : CloudControlCmdlet
    {
        /// <summary>
        ///     The network domain where the VLAN will be created.
        /// </summary>
        [Parameter(
            ParameterSetName = "From network domain",
            Mandatory = true,
            ValueFromPipeline = true,
            HelpMessage = "The network domain where the VLAN will be created"
        )]
        [ValidateNotNull]
        public NetworkDomain NetworkDomain { get; set; }

        /// <summary>
        ///     The Id of the network domain where the VLAN will be created.
        /// </summary>
        [Parameter(
            ParameterSetName = "From network domain Id",
            Mandatory = true,
            HelpMessage = "The Id of the network domain where the VLAN will be created"
        )]
        public Guid NetworkDomainId { get; set; }

        /// <summary>
        ///     A name for the new VLAN.
        /// </summary>
        [Parameter(
            Position = 0,
            Mandatory = true,
            HelpMessage = "A name for the new VLAN"
        )]
        [ValidateNotNullOrEmpty]
        public string Name { get; set; }

        /// <summary>
        ///     A description for the new VLAN.
        /// </summary>
        [Parameter(
            HelpMessage = "A description for the new VLAN"
        )]
        public string Description { get; set; }

        /// <summary>
        ///     The base address of the private IPv4 network for the new VLAN.
        /// </summary>
        [Parameter(
            Mandatory = true,
            HelpMessage = "The base address of the private IPv4 network for the new VLAN"
        )]
        [ValidateNotNullOrEmpty]
        public string IPv4BaseAddress { get; set; }

        /// <summary>
        ///     The prefix size for the private IPv4 network of the new VLAN.
        /// </summary>
        [Parameter(
            HelpMessage = "The base address of the private IPv4 network for the new VLAN (default is 24)"
        )]
        [ValidateNotNullOrEmpty]
        public short IPv4PrefixSize { get; set; } = 24;

        /// <summary>
        ///     The type of gateway addressing to use for the new VLAN.
        /// </summary>
        /// <seealso cref="VlanGatewayAddressing"/>
        [Parameter(
            HelpMessage = "The type of gateway addressing (Low or High) to use for the new VLAN"
        )]
        public VlanGatewayAddressing GatewayAddressing { get; set; } = VlanGatewayAddressing.Low;

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

            switch (ParameterSetName)
            {
                case "From network domain":
                {
                    NetworkDomainId = NetworkDomain.Id;

                    break;
                }
                case "From network domain Id":
                {
                    NetworkDomain = await client.GetNetworkDomain(NetworkDomainId, cancellationToken);
                    if (NetworkDomain == null)
                    {
                        WriteError(
                            Errors.ResourceNotFoundById<NetworkDomain>(NetworkDomainId)
                        );

                        return;
                    }

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

            if (!ShouldProcess(target: $"VLAN '{Name}' in '{NetworkDomain.Name}'", action: "create"))
                return;

            Guid vlanId = await client.CreateVlan(Name, Description, NetworkDomainId, IPv4BaseAddress, IPv4PrefixSize, GatewayAddressing, cancellationToken);
            Vlan vlan = await client.GetVlan(vlanId, cancellationToken);
            if (vlan == null)
            {
                WriteError(
                    Errors.ResourceNotFoundById<Vlan>(vlanId)
                );

                return;
            }

            WriteObject(vlan);
        }
    }
}