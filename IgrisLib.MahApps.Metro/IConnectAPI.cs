using System.Collections.Generic;

namespace IgrisLib
{
    /// <summary>
    /// 
    /// </summary>
    public interface IConnectAPI
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        bool ConnectTarget(string ip);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        List<CCAPI.ConsoleInfo> GetConsoleList();
    }
}
