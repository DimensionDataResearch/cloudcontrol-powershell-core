using PSReptile;
using System;
using System.Management.Automation;
using System.Threading;
using System.Threading.Tasks;

namespace DD.CloudControl.Powershell.Vlans
{
    using Client;
    using Client.Models;
    using Client.Models.Network;

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

            if (IPv4PrefixSize >= vlan.PrivateIPv4Range.PrefixSize)
            {
                InvalidParameter(nameof(IPv4PrefixSize),
                    $"Cannot expand VLAN - the specified IPv4 prefix size is {IPv4PrefixSize}, which is greater than or equal to the VLAN's current IPv4 prefix size ({vlan.PrivateIPv4Range.PrefixSize}). To expand the VLAN's IPv4 network, reduce its prefix size."
                );

                return;
            }

            if (!ShouldProcess(target: $"IPv4 network for VLAN '{vlan.Id}' ('{vlan.Name}') in '{vlan.NetworkDomain.Name}' (from {vlan.PrivateIPv4Range.PrefixSize} prefix bits to {IPv4PrefixSize} prefix bits.", action: "expand"))
                return;

            ApiResponseV2 editResponse = await client.ExpandVlan(vlan.Id, IPv4PrefixSize, cancellationToken);
            if (!editResponse.IsSuccess())
            {
                WriteError(
                    Errors.CloudControlApi(client, editResponse)
                );

                return;
            }

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
