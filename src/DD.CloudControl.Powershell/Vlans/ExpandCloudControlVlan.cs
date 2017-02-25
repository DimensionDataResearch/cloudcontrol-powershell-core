using PSReptile;
using System;
using System.Management.Automation;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace DD.CloudControl.Powershell.Vlans
{
    using Client;
    using Client.Models;
    using Client.Models.Network;
    using Utilities;

    /// <summary>
    ///     Cmdlet that expands the private IPv4 network of an existing VLAN.
    /// </summary>
    [OutputType(typeof(Vlan))]
    [Cmdlet(VerbsData.Expand, Nouns.Vlan, DefaultParameterSetName = "By Id", SupportsShouldProcess = true)]
    [CmdletSynopsis("Updates an existing VLAN")]
    public class ExpandCloudControlVlan
        : CloudControlCmdlet
    {
        /// <summary>
        ///     The VLAN to update.
        /// </summary>
        [ValidateNotNull]
        [Parameter(ParameterSetName="From VLAN", Mandatory = true, Position = 0, ValueFromPipeline = true, HelpMessage = "The VLAN to update")]
        public Vlan VLAN { get; set; }

        /// <summary>
        ///     The Id of the VLAN to update.
        /// </summary>
        [Parameter(ParameterSetName="By Id", Mandatory = true, Position = 0, HelpMessage = "The Id of the VLAN to update.")]
        public Guid Id { get; set; }

        /// <summary>
        ///     The new private IPv4 prefix size for the VLAN (decrease the prefix size to expand the network).
        /// </summary>
        [ValidateNotNullOrEmpty]
        [Parameter(Position = 1, HelpMessage = "The new private IPv4 prefix size for the VLAN (decrease the prefix size to expand the network)")]
        public int IPv4PrefixSize { get; set; }

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

            Vlan vlan;
            switch (ParameterSetName)
            {
                case "From VLAN":
                {
                    vlan = VLAN;

                    break;
                }
                case "By Id":
                {
                    vlan = await client.GetVlan(Id, cancellationToken);
                    if (vlan == null)
                    {
                        WriteError(
                            Errors.ResourceNotFoundById<Vlan>(Id)
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

            IPAddress baseAddress = IPAddress.Parse(vlan.PrivateIPv4Range.Address);
                
            IPAddress existingStartAddress, existingEndAddress;
            baseAddress.CalculateIPv4NetworkAddresses(vlan.PrivateIPv4Range.PrefixSize,
                out existingStartAddress,
                out existingEndAddress
            );
            string existingNetwork = $"{baseAddress}/{vlan.PrivateIPv4Range.PrefixSize} ({existingStartAddress}-{existingEndAddress})";

            IPAddress targetStartAddress, targetEndAddress;
            baseAddress.CalculateIPv4NetworkAddresses(IPv4PrefixSize,
                out targetStartAddress,
                out targetEndAddress
            );
            string targetNetwork = $"{baseAddress}/{IPv4PrefixSize} ({targetStartAddress}-{targetEndAddress})";

            if (IPv4PrefixSize >= vlan.PrivateIPv4Range.PrefixSize)
            {
                InvalidParameter(nameof(IPv4PrefixSize),
                    $"Cannot expand VLAN network from {existingNetwork} to {targetNetwork}. To expand the VLAN's IPv4 network, reduce its IPv4 prefix size."
                );

                return;
            }

            WriteVerbose(
                $"Expanding VLAN '{vlan.Name}' (in network domain '{vlan.NetworkDomain.Name}') from {existingNetwork} to {targetNetwork}."
            );

            if (!ShouldProcess(target: $"'{vlan.Name}' ('{vlan.Id}') in '{vlan.NetworkDomain.Name}' (from {existingNetwork} to {targetNetwork}).", action: "Expand"))
                return;

            WriteVerbose("Initiating expansion of VLAN...");

            ApiResponseV2 editResponse = await client.ExpandVlan(vlan.Id, IPv4PrefixSize, cancellationToken);
            if (!editResponse.IsSuccess())
            {
                WriteError(
                    Errors.CloudControlApi(client, editResponse)
                );

                return;
            }

            WriteVerbose("VLAN expansion initiated.");

            Vlan updatedVlan = await client.GetVlan(vlan.Id, cancellationToken);
            if (updatedVlan == null)
            {
                WriteError(
                    Errors.ResourceNotFoundById<Vlan>(vlan.Id)
                );

                return;
            }

            WriteObject(updatedVlan);
        }
    }
}
