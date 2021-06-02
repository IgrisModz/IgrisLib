using IgrisLib.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace IgrisLib
{
    public class PS3API : ViewModelBase
    {
        private IApi api;
        private bool isConnected, isAttached;

        public bool IsConnected { get => isConnected; private set => SetProperty(ref isConnected, value); }

        public bool IsAttached { get => isAttached; private set => SetProperty(ref isAttached, value); }

        public string CurrentGame { get; private set; }

        public string GameRegion { get; private set; }

        public string TargetName { get; private set; }

        public string TargetIp { get; private set; }

        public PS3API(IApi api)
        {
            this.api = api;
        }

        public List<string> GetAllApi()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => typeof(IApi).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract).Select(x => x.Name).ToList();
        }

        public bool GetConnected()
        {
            try
            {
                if (api.GetType() == typeof(TMAPI))
                    IsConnected = (api as TMAPI).GetStatus() == "Connected";
                else if (api.GetType() == typeof(CCAPI))
                    IsConnected = (api as CCAPI).IsConnected();
                else if (api.GetType() == typeof(CCAPI))
                    IsConnected = (api as PS3MAPI).IsConnected;
                return IsConnected;
            }
            catch { return false; }
        }

        public bool GetAttached()
        {
            try
            {
                string text = Extension.ReadString(0x10000);
                IsAttached = text.Contains("ELF") || !string.IsNullOrEmpty(text);
                IsConnected = IsAttached;
                return IsAttached;
            }
            catch { return false; }
        }

        public string GetCurrentGame()
        {
            CurrentGame = "Unknown";
            try
            {
                string gameRegion = GetGameRegion();
                WebClient client = new WebClient();
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                string content = client.DownloadString(string.Format("https://a0.ww.np.dl.playstation.net/tpl/np/{0}/{1}-ver.xml", gameRegion, gameRegion)).Replace("<TITLE>", ";");
                string text = content.Split(';')[1].Replace("</TITLE>", ";").Split(';')[0].Replace("Â", "");
                CurrentGame = !string.IsNullOrEmpty(text) ? text : "Unknown";
            }
            catch { }
            return CurrentGame;
        }

        public string GetGameRegion()
        {
            GameRegion = "Unknown";
            try
            {
                GameRegion = Extension.ReadString(0x10010251);
            }
            catch { }
            return GameRegion;
        }

        /// <summary>Connect your console with TargetManager.</summary>
        public bool Connect()
        {
            IsConnected = false;
            if (api.GetType() == typeof(TMAPI))
                IsConnected = api.ConnectTarget();
            return IsConnected;
        }

        /// <summary>Connect your console with selected API.</summary>
        public bool ConnectTarget(int target = 0)
        {
            IsConnected = api.ConnectTarget();
            return IsConnected;
        }

        /// <summary>Connect your console with CCAPI.</summary>
        public bool ConnectTarget(string ip)
        {
            if (api.GetType() == typeof(CCAPI))
            {
                if ((api as CCAPI).ConnectTarget(ip))
                {
                    TargetIp = ip;
                    IsConnected = true;
                    return true;
                }
            }
            else if (api.GetType() == typeof(PS3MAPI))
            {
                if ((api as PS3MAPI).ConnectTarget(ip))
                {
                    TargetIp = ip;
                    IsConnected = true;
                    return true;
                }
            }
            IsConnected = false;
            return false;
        }

        /// <summary>Disconnect Target with selected API.</summary>
        public void DisconnectTarget()
        {
            api.DisconnectTarget();
        }

        /// <summary>Disconnect Target with selected API.</summary>
        public void DetachProcess()
        {
            api.DetachProcess();
        }

        /// <summary>Attach the current process (current Game) with selected API.</summary>
        public bool AttachProcess()
        {
            IsAttached = api.AttachProcess();
            return IsAttached;
        }

        public string GetConsoleName()
        {
            if (api.GetType() == typeof(TMAPI))
                return (api as TMAPI).SCE.GetTargetName();
            else
            {
                if (TargetName != string.Empty)
                    return TargetName;

                if (TargetIp != string.Empty)
                {
                    List<CCAPI.ConsoleInfo> Data = (api as CCAPI).GetConsoleList();
                    if (Data.Count > 0)
                    {
                        for (int i = 0; i < Data.Count; i++)
                            if (Data[i].Ip == TargetIp)
                                return Data[i].Name;
                    }
                }
                return TargetIp;
            }
        }

        /// <summary>Set memory to offset with selected API.</summary>
        public void SetMemory(uint offset, byte[] buffer)
        {
            api.SetMemory(offset, buffer);
        }

        /// <summary>Get memory from offset using the Selected API.</summary>
        public void GetMemory(uint offset, byte[] buffer)
        {
            api.GetMemory(offset, buffer);
        }

        /// <summary>Get memory from offset with a length using the Selected API.</summary>
        public byte[] GetBytes(uint offset, int length)
        {
            byte[] buffer = new byte[length];
            api.GetMemory(offset, buffer);
            return buffer;
        }

        /// <summary>Change current API.</summary>
        public void ChangeAPI(IApi api)
        {
            this.api = api;
        }

        /// <summary>Return selected API.</summary>
        public IApi GetCurrentAPI()
        {
            return api;
        }

        /// <summary>Return selected API full name into string format.</summary>
        public string GetCurrentAPIFullName()
        {
            return api.FullName;
        }

        /// <summary>Return selected API into string format.</summary>
        public string GetCurrentAPIName()
        {
            return api.Name;
        }

        /// <summary>Use the extension class with your selected API.</summary>
        public Extension Extension
        {
            get { return new Extension(api); }
        }

        /// <summary>Access to all TMAPI functions.</summary>
        public TMAPI TMAPI
        {
            get { return new TMAPI(); }
        }

        /// <summary>Access to all CCAPI functions.</summary>
        public CCAPI CCAPI
        {
            get { return new CCAPI(); }
        }

        /// <summary>Access to all PS3MAPI functions.</summary>
        public PS3MAPI PS3MAPI
        {
            get { return new PS3MAPI(); }
        }
    }
}
