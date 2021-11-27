namespace IgrisLib
{
    /// <summary>
    /// Interface for all api in this library.
    /// </summary>
    public interface IApi
    {
        /// <summary>
        /// Full name of this api.
        /// </summary>
        string FullName { get; }

        /// <summary>
        /// Name of this api.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// IP address currently in use.
        /// </summary>
        string IPAddress { get; }

        /// <summary>
        /// Connect your tool to your ps3.
        /// </summary>
        /// <returns>Returns <see cref="bool.TrueString"/> if you're successufy connected.</returns>
        bool ConnectTarget();

        /// <summary>
        /// Attach your tool to your current game.
        /// </summary>
        /// <returns>Returns <see cref="bool.TrueString"/> if you're successufy attached.</returns>
        bool AttachProcess();

        /// <summary>
        /// Disconnect your tool from your ps3.
        /// </summary>
        /// Return 0 if you are disconnected.
        int DisconnectTarget();

        /// <summary>
        /// Detach your tool FROM you game.
        /// </summary>
        void DetachProcess();

        /// <summary>
        /// Get byte array and save the result in your variable <paramref name="address"/>.
        /// </summary>
        /// <param name="address">Start offset for the search.</param>
        /// <param name="buffer">Byte array to save search result.</param>
        void GetMemory(uint address, byte[] buffer);

        /// <summary>
        /// Set byte array.
        /// </summary>
        /// <param name="address">Start offset for sending byte array.</param>
        /// <param name="buffer">Byte array to send.</param>
        void SetMemory(uint address, byte[] buffer);

        /// <summary>
        /// Get byte array
        /// </summary>
        /// <param name="address">Start offset for the search.</param>
        /// <param name="length">Length of a byte array to get.</param>
        /// <returns>Returns a byte array with the bytes obtained during the search.</returns>
        byte[] GetBytes(uint address, uint length);
    }
}
