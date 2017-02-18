using System.Management.Automation;
using System.Threading;
using System.Threading.Tasks;

namespace DD.CloudControl.Powershell.Accounts
{
    using Client;
    using Client.Models.Directory;

    /// <summary>
    ///     Cmdlet that retrieves information about one or more CloudControl accounts.
    /// </summary>
    [OutputType(typeof(UserAccount))]
    [Cmdlet(VerbsCommon.Get, Nouns.UserAccount)]
    public class GetCloudControlUserAccount
        : CloudControlCmdlet
    {
        /// <summary>
        ///     Retrieve the current user's account details.
        /// </summary>
        [Parameter(Mandatory = true)]
        public SwitchParameter My { get; set; }

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

            WriteObject(
                await client.GetAccount(cancellationToken)
            );
        }
    }
}