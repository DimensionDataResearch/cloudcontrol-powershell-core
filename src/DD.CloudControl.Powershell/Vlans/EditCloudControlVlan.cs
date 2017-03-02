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
    ///     Cmdlet that updates an existing VLAN.
    /// </summary>
    [OutputType(typeof(Vlan))]
    [Cmdlet(VerbsData.Edit, Nouns.Vlan, DefaultParameterSetName = "By Id", SupportsShouldProcess = true)]
    [CmdletSynopsis("Updates an existing VLAN")]
    public class EditCloudControlVlan
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
        ///     A new name for the VLAN.
        /// </summary>
        [ValidateNotNullOrEmpty]
        [Parameter(Position = 1, HelpMessage = "A new name for the VLAN")]
        public string Name { get; set; }

        /// <summary>
        ///     A new description for the VLAN.
        /// </summary>
        [ValidateNotNull]
        [Parameter(Position = 2, HelpMessage = "A new description for the VLAN")]
        public string Description { get; set; }

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

            if (!ShouldProcess(target: $"VLAN '{vlan.Id}' ('{vlan.Name}') in '{vlan.NetworkDomain.Name}'", action: "Edit"))
                return;

            ApiResponseV2 editResponse = await client.EditVlan(vlan, cancellationToken);
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