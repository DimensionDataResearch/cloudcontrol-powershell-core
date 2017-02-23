namespace DD.CloudControl.Powershell
{
    /// <summary>
    ///     Well-known nouns used to identify cmdlets.
    /// </summary>
    public static class Nouns
    {
        /// <summary>
        ///     The prefix for all CloudControl nouns.
        /// </summary>
        const string Prefix = "CloudControl";
    
        /// <summary>
        ///     A connection to CloudControl.
        /// </summary>
        public const string Connection = Prefix + "Connection";

        /// <summary>
        ///     A user account.
        /// </summary>
        public const string UserAccount = Prefix + "UserAccount";

        /// <summary>
        ///     A network domain.
        /// </summary>
        public const string NetworkDomain = Prefix + "NetworkDomain";

        /// <summary>
        ///     A resource's state.
        /// </summary>
        public const string ResourceState = Prefix + "ResourceState";
    }
}