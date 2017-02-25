using System;
using System.Net;
using System.Net.Sockets;

namespace DD.CloudControl.Powershell.Utilities
{
    /// <summary>
    ///     Helper functions for working with IP addresses.
    /// </summary>
    /// <remarks>
    ///     TODO: Handle IPv6 (quite similar, I think, but 64-bit prefix / ulong instead of uint).
    /// </remarks>
    public static class IPHelper
    {
        /// <summary>
        ///     Calculate IPv4 network start / end addresses, assuming the specified network prefix size.
        /// </summary>
        /// <param name="baseAddress">
        ///     The IP address acting as the network base address.
        /// </param>
        /// <param name="prefixSize">
        ///     The number of bits in the network prefix (1-31).
        /// </param>
        /// <param name="startAddress">
        ///     Receives the first IP address in the network.
        /// </param>
        /// <param name="endAddress">
        ///     Receives the last IP address in the network.
        /// </param>
        public static void CalculateIPv4NetworkAddresses(this IPAddress baseAddress, int prefixSize, out IPAddress startAddress, out IPAddress endAddress)
        {
            if (baseAddress == null)
                throw new ArgumentNullException(nameof(baseAddress));

            if (baseAddress.AddressFamily != AddressFamily.InterNetwork)
                throw new NotSupportedException($"This operation is only supported for the 'InterNetwork' address family (not '{baseAddress.AddressFamily}').");

            if (1 < prefixSize || prefixSize > 31)
                throw new ArgumentOutOfRangeException(nameof(prefixSize), prefixSize, "IPv4 network prefix size must be between 1 and 31.");

            // The mask is simply the set of bits we're interested (i.e. the prefix)
            uint netMask = UInt32.MaxValue >> (32 - prefixSize);
            uint invertedNetMask = netMask ^ UInt32.MaxValue;

            // The bits comprising our network base address.
            uint network = BitConverter.ToUInt32(
                baseAddress.GetAddressBytes(),
                startIndex: 0
            );

            // The start address is the network base address with non-prefix bits masked out.
            startAddress = new IPAddress(BitConverter.GetBytes(
                network & netMask
            ));

            // The end address is the network base address with all non-prefix bits switched on.
            endAddress = new IPAddress(BitConverter.GetBytes(
                network | invertedNetMask
            ));
        }

        /// <summary>
        ///     Calculate the IPv4 network start address, assuming the specified network prefix size.
        /// </summary>
        /// <param name="baseAddress">
        ///     The IP address acting as the network base address.
        /// </param>
        /// <param name="prefixSize">
        ///     The number of bits in the network prefix (1-31).
        /// </param>
        /// <returns>
        ///     Receives the first IP address in the network.
        /// </returns>
        public static IPAddress CalculateIPv4NetworkStartAddress(this IPAddress baseAddress, int prefixSize)
        {
            IPAddress startAddress, endAddress;
            baseAddress.CalculateIPv4NetworkAddresses(prefixSize, out startAddress, out endAddress);

            return startAddress;
        }

        /// <summary>
        ///     Calculate the IPv4 network end address, assuming the specified network prefix size.
        /// </summary>
        /// <param name="baseAddress">
        ///     The IP address acting as the network base address.
        /// </param>
        /// <param name="prefixSize">
        ///     The number of bits in the network prefix (1-31).
        /// </param>
        /// <returns>
        ///     Receives the last IP address in the network.
        /// </returns>
        public static IPAddress CalculateIPv4NetworkEndAddress(this IPAddress baseAddress, int prefixSize)
        {
            IPAddress startAddress, endAddress;
            baseAddress.CalculateIPv4NetworkAddresses(prefixSize, out startAddress, out endAddress);

            return endAddress;
        }
    }
}