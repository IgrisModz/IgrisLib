using System.Collections.Generic;

namespace IgrisLib
{
    public interface IConnectAPI
    {
        bool ConnectTarget(string ip);

        List<CCAPI.ConsoleInfo> GetConsoleList();
    }
}
