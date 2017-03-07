using PSReptile;
using System;
using System.Management.Automation;
using System.Threading;
using System.Threading.Tasks;

namespace DD.CloudControl.Powershell.NatRules
{
    using Client;
    using Client.Models.Network;

    /// <summary>
    ///     Cmdlet that retrieves information about one or more NAT rules.
    /// </summary>
    [OutputType(typeof(NatRule))]
    [Cmdlet(VerbsCommon.Get, Nouns.NatRule, SupportsPaging = true)]
    [CmdletSynopsis("Retrieves information about one or more NAT rules")]
    public class GetCloudControlNatRule
        : CloudControlCmdlet
    {
        /// <summary>
        ///     The Id of the NAT rule to retrieve.
        /// </summary>
        [Parameter(ParameterSetName = "By Id", Mandatory = true, HelpMessage = "The Id of the network domain to retrieve")]
        public Guid Id { get; set; }

        /// <summary>
        ///     Retrieve all NAT rules in the specified network domain.
        /// </summary>
        [Parameter(ParameterSetName = "By network domain", Mandatory = true, HelpMessage = "Retrieve all NAT rules in the specified network domain")]
        public Guid NetworkDomainId { get; set; }

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
                case "By Id":
                {
                    NatRule natRule = await client.GetNatRule(Id, cancellationToken);
                    if (natRule == null)
                    {
                        WriteError(
                            Errors.ResourceNotFoundById<NatRule>(Id)
                        );
                    }
                    else
                        WriteObject(natRule);

                    break;
                }
                case "By network domain":
                {
                    Paging paging = GetPagingConfiguration();
                    
                    NatRules natRules = await client.ListNatRules(NetworkDomainId, paging, cancellationToken);
                    WriteObject(natRules.Items,
                        enumerateCollection: true
                    );

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
