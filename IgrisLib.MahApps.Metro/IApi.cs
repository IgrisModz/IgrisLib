namespace IgrisLib
{
    /// <summary>
    /// 
    /// </summary>
    public interface IApi
    {
        /// <summary>
        /// 
        /// </summary>
        string FullName { get; }

        /// <summary>
        /// 
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 
        /// </summary>
        string IPAddress { get; }

        /// <summary>
        /// 
        /// </summary>
        bool ConnectTarget();

        /// <summary>
        /// 
        /// </summary>
        bool AttachProcess();

        /// <summary>
        /// 
        /// </summary>
        int DisconnectTarget();

        /// <summary>
        /// 
        /// </summary>
        void DetachProcess();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="address"></param>
        /// <param name="buffer"></param>
        void SetMemory(uint address, byte[] buffer);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="address"></param>
        /// <param name="buffer"></param>
        void GetMemory(uint address, byte[] buffer);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="address"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        byte[] GetBytes(uint address, uint length);
    }
}
