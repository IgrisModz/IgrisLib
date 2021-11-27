using System.Collections.Generic;

namespace IgrisLib
{
    /// <summary>
    /// Interface for APIs that use the ccapi console list.
    /// </summary>
    public interface IConnectAPI
    {
        /// <summary>
        /// Connect to your ps3 with the internal ip address of the ps3.
        /// </summary>
        /// <param name="ip">The ip address of your ps3.</param>
        /// <returns>Returns <see cref="bool.TrueString"/> if you're successufy connected.</returns>
        bool ConnectTarget(string ip);

        /// <summary>
        /// Get the list of all consoles
        /// </summary>
        /// <returns>Return all consoles for your connection.</returns>
        List<CCAPI.ConsoleInfo> GetConsoleList();
    }
}
