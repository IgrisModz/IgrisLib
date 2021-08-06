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

        /// <summary>
        /// Property to know if you are connected to the ps3.
        /// </summary>
        /// <value>Return true if you are connected to ps3.</value>
        /// <remarks>You have to use <see cref="Connect"/> <see cref="ConnectTarget"/>, <see cref="DisconnectTarget"/> or <see cref="GetConnected"/> for update this property</remarks>
        public bool IsConnected { get => GetValue(() => IsConnected); private set => SetValue(() => IsConnected, value); }

        /// <summary>
        /// Property to find out if you are attached to a process.
        /// </summary>
        /// <value>Return true if you are connected to ps3.</value>
        /// <remarks>You have to use <see cref="AttachProcess"/>, <see cref="DetachProcess"/> or <see cref="GetAttached"/> for update this property</remarks>
        public bool IsAttached { get => GetValue(() => IsAttached); private set => SetValue(() => IsAttached, value); }

        /// <summary>
        /// Property to know the name of the game currently attached.
        /// </summary>
        /// <value>Returns the name of the currently attached game.</value>
        /// <remarks>You have to use <see cref="GetCurrentGame"/> for update this property</remarks>
        public string CurrentGame { get; private set; }

        /// <summary>
        /// Property to know the version of the game currently attached.
        /// </summary>
        /// <value>Returns the version of the currently attached game.</value>
        /// <remarks>You have to use <see cref="GetGameRegion"/> for update this property</remarks>
        public string GameRegion { get; private set; }

        /// <summary>
        /// Property to know the name of the console currently connected.
        /// </summary>
        /// <value>Returns the name of the currently connected console.</value>
        /// <remarks>You have to use <see cref="Connect"/> or <see cref="ConnectTarget"/> for update this property</remarks>
        public string TargetName { get; private set; }

        /// <summary>
        /// Property to know the ip address of the console currently connected.
        /// </summary>
        /// <value>Returns the ip address of the currently connected console.</value>
        /// <remarks>You have to use <see cref="Connect"/> or <see cref="ConnectTarget"/> for update this property</remarks>
        public string TargetIp { get; private set; }

        /// <summary>
        /// Contructor for choose an API by its class.
        /// </summary>
        /// <param name="api">Choose an API by its class.</param>
        public PS3API(IApi api)
        {
            ChangeAPI(api);
        }

        /// <summary>
        /// Constructor for choose an API by an enum.
        /// </summary>
        /// <param name="api">Choose an API by an enum.</param>
        public PS3API(SelectAPI api)
        {
            ChangeAPI(api);
        }

        /// <summary>
        /// Constructor for choose an API by name.
        /// </summary>
        /// <param name="apiName">Choose an API by name</param>
        public PS3API(string apiName)
        {
            ChangeAPI(apiName);
        }

        /// <summary>
        /// Get the name of all APIs.
        /// </summary>
        /// <returns>Returns a list of names of all APIs.</returns>
        public List<string> GetAllApi()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => typeof(IApi).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                .Select(x => x.Name).ToList();
        }

        /// <summary>
        /// Find out if you're connected.
        /// </summary>
        /// <returns>return true if you're connected.</returns>
        public bool GetConnected()
        {
            try
            {
                if (api.GetType() == typeof(TMAPI))
                    IsConnected = (api as TMAPI).GetStatus() == "Connected";
                else if (api.GetType() == typeof(CCAPI))
                    IsConnected = (api as CCAPI).IsConnected();
                else if (api.GetType() == typeof(PS3MAPI))
                    IsConnected = (api as PS3MAPI).IsConnected;
                return IsConnected;
            }
            catch { return false; }
        }

        /// <summary>
        /// Find out if you're attached.
        /// </summary>
        /// <returns>return true if you're attached.</returns>
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

        /// <summary>
        /// Get the current game.
        /// </summary>
        /// <returns>Returns the name of the currently attached game.</returns>
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

        /// <summary>
        /// Get the current game version.
        /// </summary>
        /// <returns>returns the name of the currently attached game version.</returns>
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

        /// <summary>
        /// Connect your console with TargetManager.
        /// </summary>
        /// <param name="target">Define the target console you want to connect to.</param>
        /// <returns>return true if you are connected.</returns>
        public bool Connect(int target = 0)
        {
            IsConnected = false;
            if (api.GetType() == typeof(TMAPI))
                IsConnected = TMAPI.ConnectTarget(target);
            TargetIp = api.IPAddress;
            TargetName = GetConsoleName();
            return IsConnected;
        }

        /// <summary>
        /// Connect your console with selected API.
        /// </summary>
        /// <param name="target">Define the target console you want to connect to (only for tmapi).</param>
        /// <returns>return true if you are connected.</returns>
        public bool ConnectTarget(int target = 0)
        {
            IsConnected = api.GetType() == typeof(TMAPI) ? TMAPI.ConnectTarget(target) : api.ConnectTarget();
            TargetIp = api.IPAddress;
            TargetName = GetConsoleName();
            return IsConnected;
        }

        /// <summary>
        /// Connect your console with CCAPI or PS3MAPI.
        /// </summary>
        /// <param name="ip">Set the ip address of the console you want to connect to.</param>
        /// <returns>return true if you are connected.</returns>
        public bool ConnectTarget(string ip)
        {
            if (api.GetType() == typeof(CCAPI))
            {
                if ((api as CCAPI).ConnectTarget(ip))
                {
                    TargetIp = ip;
                    TargetName = GetConsoleName();
                    IsConnected = true;
                    return true;
                }
            }
            else if (api.GetType() == typeof(PS3MAPI))
            {
                if ((api as PS3MAPI).ConnectTarget(ip))
                {
                    TargetIp = ip;
                    TargetName = GetConsoleName();
                    IsConnected = true;
                    return true;
                }
            }
            IsConnected = false;
            return false;
        }

        /// <summary>
        /// Disconnect the currently connected console.
        /// </summary>
        public void DisconnectTarget()
        {
            api.DisconnectTarget();
            IsConnected = false;
        }

        /// <summary>
        /// Detach the currently attached process.
        /// </summary>
        public void DetachProcess()
        {
            api.DetachProcess();
            IsAttached = false;
        }

        /// <summary>
        /// Attach the current process (current Game) with selected API.
        /// </summary>
        /// <returns>Return true if you're acttached to process.</returns>
        public bool AttachProcess()
        {
            IsAttached = api.AttachProcess();
            return IsAttached;
        }

        /// <summary>
        /// Get the current console name.
        /// </summary>
        /// <returns>return the current console name</returns>
        public string GetConsoleName()
        {
            if (api.GetType() == typeof(TMAPI))
                return (api as TMAPI).SCE.GetTargetName();

            if (TargetName != string.Empty)
                return TargetName;

            if (TargetIp != string.Empty)
            {
                if (api.GetType() == typeof(CCAPI))
                {
                    foreach (var console in (api as CCAPI).GetConsoleList())
                    {
                        if (console.Ip == TargetIp)
                        {
                            return console.Name;
                        }
                    }
                }
                else
                {
                    foreach (var console in (api as PS3MAPI).GetConsoleList())
                    {
                        if (console.Ip == TargetIp)
                        {
                            return console.Name;
                        }
                    }
                }
            }
            return TargetIp;
        }

        /// <summary>
        /// Set memory to offset with selected API.
        /// </summary>
        /// <param name="offset">Targeted function location.</param>
        /// <param name="buffer">Set the byte array to the offset.</param>
        public void SetMemory(uint offset, byte[] buffer)
        {
            api.SetMemory(offset, buffer);
        }

        /// <summary>
        /// Get memory from offset using the Selected API.
        /// </summary>
        /// <param name="offset">Targeted function location.</param>
        /// <param name="buffer">Returns the bytes obtained in this byte array.</param>
        public void GetMemory(uint offset, byte[] buffer)
        {
            api.GetMemory(offset, buffer);
        }

        /// <summary>
        /// Get memory from offset with a length using the Selected API.
        /// </summary>
        /// <param name="offset">Targeted function location.</param>
        /// <param name="length">Length of bytes to get.</param>
        /// <returns>Returns the byte array of the defined length.</returns>
        public byte[] GetBytes(uint offset, int length)
        {
            byte[] buffer = new byte[length];
            api.GetMemory(offset, buffer);
            return buffer;
        }

        /// <summary>
        /// Change current API by its class.
        /// </summary>
        /// <param name="api">Choose an API by its class.</param>
        public void ChangeAPI(IApi api)
        {
            this.api = api;
        }

        /// <summary>
        /// Change current API by an enum.
        /// </summary>
        /// <param name="api">Choose an API by an enum.</param>
        public void ChangeAPI(SelectAPI api)
        {
            foreach (Type mapi in AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => typeof(IApi).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                .Select(x => x).ToList())
            {
                if (mapi.Name == Enum.GetName(typeof(SelectAPI), api))
                {
                    this.api = (IApi)Activator.CreateInstance(mapi);
                    break;
                }
            }
        }

        /// <summary>
        /// Change current API by name.
        /// </summary>
        /// <param name="apiName">Choose an API by name.</param>
        public void ChangeAPI(string apiName)
        {
            foreach (Type mapi in AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => typeof(IApi).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                .Select(x => x).ToList())
            {
                if (mapi.Name == apiName)
                {
                    this.api = (IApi)Activator.CreateInstance(mapi);
                    break;
                }
            }
        }

        /// <summary>
        /// Get the current API.
        /// </summary>
        /// <returns>Return the current api.</returns>
        public IApi GetCurrentAPI()
        {
            return api;
        }

        /// <summary>
        /// Get the full name of the current API.
        /// </summary>
        /// <returns>Return the current api full name.</returns>
        public string GetCurrentAPIFullName()
        {
            return api.FullName;
        }

        /// <summary>
        /// Get the name of the current API.
        /// </summary>
        /// <returns>Return the current api name.</returns>
        public string GetCurrentAPIName()
        {
            return api.Name;
        }

        /// <summary>
        /// Use the extension class with your selected API.
        /// </summary>
        public Extension Extension => new Extension(api);

        /// <summary>
        /// Access to all TMAPI functions.
        /// </summary>
        public TMAPI TMAPI => api as TMAPI;

        /// <summary>
        /// Access to all CCAPI functions.
        /// </summary>
        public CCAPI CCAPI => api as CCAPI;

        /// <summary>
        /// Access to all PS3MAPI functions.
        /// </summary>
        public PS3MAPI PS3MAPI => api as PS3MAPI;
    }
}
