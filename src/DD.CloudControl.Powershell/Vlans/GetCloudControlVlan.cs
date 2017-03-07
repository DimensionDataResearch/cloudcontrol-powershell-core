using PSReptile;
using System;
using System.Management.Automation;
using System.Threading;
using System.Threading.Tasks;

namespace DD.CloudControl.Powershell.Vlans
{
    using Client;
    using Client.Models.Network;

    /// <summary>
    ///     Cmdlet that retrieves information about one or more VLANs.
    /// </summary>
    [OutputType(typeof(Vlan))]
    [Cmdlet(VerbsCommon.Get, Nouns.Vlan, SupportsPaging = true)]
    [CmdletSynopsis("Retrieves information about one or more VLANs")]
    public class GetCloudControlVlan
        : CloudControlCmdlet
    {
        /// <summary>
        ///     The Id of the VLAN to retrieve.
        /// </summary>
        [Parameter(ParameterSetName = "By Id", Mandatory = true, HelpMessage = "The Id of the network domain to retrieve")]
        public Guid Id { get; set; }

        /// <summary>
        ///     The Id of the network domain where the VLAN is defined.
        /// </summary>
        [Parameter(ParameterSetName = "By name", Mandatory = true, HelpMessage = "The Id of the network domain where the VLAN is defined")]
        [Parameter(ParameterSetName = "By network domain", Mandatory = true, HelpMessage = "The Id of the network domain where the VLAN is defined")]
        public Guid NetworkDomainId { get; set; }

        /// <summary>
        ///     The name of the VLAN to retrieve.
        /// </summary>
        [ValidateNotNullOrEmpty]
        [Parameter(ParameterSetName = "By name", Mandatory = true, HelpMessage = "The name the VLAN to retrieve")]
        public string Name { get; set; }

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
                case "By network domain":
                {
                    Paging paging = GetPagingConfiguration();

                    Vlans vlans = await client.ListVlans(NetworkDomainId, paging, cancellationToken);
                    WriteObject(vlans.Items,
                        enumerateCollection: true
                    );

                    break;
                }
                case "By Id":
                {
                    Vlan vlan = await client.GetVlan(Id, cancellationToken);
                    if (vlan == null)
                    {
                        WriteError(
                            Errors.ResourceNotFoundById<Vlan>(Id)
                        );
                    }
                    else
                        WriteObject(vlan);

                    break;
                }
                case "By name":
                {
                    Vlan vlan = await client.GetVlanByName(Name, NetworkDomainId);
                    if (vlan == null)
                    {
                        WriteError(
                            Errors.ResourceNotFoundByName<Vlan>(Name, 
                                message: $"No network domain named '{Name}' was found in network domain '{NetworkDomainId}'."
                            )
                        );
                    }
                    else
                        WriteObject(vlan);
                    
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
        }
    }
}
