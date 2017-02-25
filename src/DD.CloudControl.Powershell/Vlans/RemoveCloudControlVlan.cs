using PSReptile;
using System;
using System.Management.Automation;
using System.Threading;
using System.Threading.Tasks;

namespace DD.CloudControl.Powershell.NetworkDomains
{
    using Client;
    using Client.Models;
    using Client.Models.Network;

    /// <summary>
    ///     Cmdlet that updates an existing VLAN.
    /// </summary>
    [OutputType(typeof(ApiResponseV2))]
    [Cmdlet(VerbsCommon.Remove, Nouns.Vlan, DefaultParameterSetName = "By Id", SupportsShouldProcess = true)]
    [CmdletSynopsis("Destroys a network domain")]
    public class RemoveCloudControlVan
        : CloudControlCmdlet
    {
        /// <summary>
        ///     The VLAN to destroy.
        /// </summary>
        [ValidateNotNull]
        [Parameter(ParameterSetName="From VLAN", Mandatory = true, Position = 0, ValueFromPipeline = true, HelpMessage = "The VLAN to destroy")]
        public Vlan VLAN { get; set; }

        /// <summary>
        ///     The Id of the VLAN to update.
        /// </summary>
        [Parameter(ParameterSetName="By Id", Mandatory = true, Position = 0, HelpMessage = "The Id of the VLAN to destroy")]
        public Guid Id { get; set; }

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

            if (!ShouldProcess(target: $"VLAN '{vlan.Id}' ('{vlan.Name}') in '{vlan.NetworkDomain.Name}'", action: "Destroy"))
                return;

            ApiResponseV2 apiResponse = await client.DeleteVlan(vlan.Id, cancellationToken);
            if (!apiResponse.IsSuccess())
            {
                WriteError(
                    Errors.CloudControlApi(client, apiResponse)
                );
            }
            else
                WriteObject(apiResponse);
        }
    }
}