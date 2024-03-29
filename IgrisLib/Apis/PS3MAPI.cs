﻿using IgrisLib.Resources;
using IgrisLib.Views;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

namespace IgrisLib
{
    /// <summary>
    /// 
    /// </summary>
    public class PS3MAPI : IApi, IConnectAPI
    {
        /// <summary>
        /// 
        /// </summary>
        public string FullName => "PS3 Manager";

        /// <summary>
        /// 
        /// </summary>
        public string Name => "PS3MAPI";

        /// <summary>
        /// 
        /// </summary>
        public string IPAddress { get; private set; } = "127.0.0.1";

        /// <summary>
        /// 
        /// </summary>
        public int PS3M_API_PC_LIB_VERSION { get; set; } = 0x0120;

        /// <summary>
        /// 
        /// </summary>
        public CORE_CMD Core { get; set; } = new CORE_CMD();

        /// <summary>
        /// 
        /// </summary>
        public SERVER_CMD Server { get; set; } = new SERVER_CMD();

        /// <summary>
        /// 
        /// </summary>
        public PS3_CMD PS3 { get; set; } = new PS3_CMD();

        /// <summary>
        /// 
        /// </summary>
        public PROCESS_CMD Process { get; set; } = new PROCESS_CMD();


        internal ResourceDictionary Resources { get; }

        /// <summary>
        /// 
        /// </summary>
        public PS3MAPI()
        {
            ConsoleRegistry.Create();
            Resources = Language.Get;
            Core = new CORE_CMD();
            Server = new SERVER_CMD();
            PS3 = new PS3_CMD();
            Process = new PROCESS_CMD();
        }

        #region PS3MAPI_CLient

        /// <summary>
        /// Get PS3 Manager PC Lib Version.
        /// </summary>
        /// <returns></returns>
        public string GetLibVersion_Str()
        {
            string ver = PS3M_API_PC_LIB_VERSION.ToString("X4");
            string char1 = ver.Substring(1, 1) + ".";
            string char2 = ver.Substring(2, 1) + ".";
            string char3 = ver.Substring(3, 1);
            return char1 + char2 + char3;
        }

        /// <summary>
        /// Indicates if PS3MAPI is connected
        /// </summary>
        public bool IsConnected => PS3MAPI_Client_Server.IsConnected;

        /// <summary>
        /// Indicates if PS3MAPI is attached
        /// </summary>
        public bool IsAttached => PS3MAPI_Client_Server.IsAttached;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<CCAPI.ConsoleInfo> GetConsoleList()
        {
            List<CCAPI.ConsoleInfo> consoles = new List<CCAPI.ConsoleInfo>();
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);
            key = key.OpenSubKey(ConsoleRegistry.RegistryName, true);
            foreach (string subKeyName in key.GetValueNames())
            {
                // Read Value from Registry Sub Key
                string softwareName = (string)key.GetValue(subKeyName);

                if (!string.IsNullOrEmpty(softwareName))
                {
                    consoles.Add(new CCAPI.ConsoleInfo() { Name = subKeyName, Ip = softwareName });
                }
            }
            return consoles;
        }

        /// <summary>
        /// Connect the target with ip.
        /// </summary>
        /// <param name="ip">Ip</param>
        /// <returns></returns>
        public bool ConnectTarget(string ip)
        {
            try
            {
                PS3MAPI_Client_Server.Connect(ip, 7887);
                IPAddress = ip;
                return true;
            }
            catch (Exception ex)
            {
                IPAddress = "127.0.0.1";
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// Connect the target with "ConnectView".
        /// </summary>
        /// <returns></returns>
        public bool ConnectTarget()
        {
            try
            {
                CCAPIView ccapiView = new CCAPIView(this, this.Resources);
                bool isConnected = ccapiView.Show();
                IPAddress = isConnected ? ccapiView.ViewModel.SelectedConsole.Ip : "127.0.0.1";
                return isConnected;
            }
            catch (Exception ex)
            {
                IPAddress = "127.0.0.1";
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// Attach to process by pid.
        /// </summary>
        /// <param name="pid">Process PID</param>
        /// <returns></returns>
        public bool AttachProcess(uint pid)
        {
            try
            {
                Process.Process_Pid = pid;
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// Attach to process.
        /// </summary>
        /// <returns></returns>
        public bool AttachProcess()
        {
            try
            {

                foreach (uint pid in Process.GetPidProcesses())
                {
                    if (pid != 0)
                    {
                        if (!Process.GetName(pid).Contains("dev_flash"))
                        {
                            AttachProcess(pid);
                            return true;
                        }
                    }
                    else
                        break;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// Disconnect the target.
        /// </summary>
        /// <returns></returns>
        public int DisconnectTarget()
        {
            try
            {
                PS3MAPI_Client_Server.Disconnect();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            return 0;
        }

        /// <summary>
        /// Detach the process.
        /// </summary>
        public void DetachProcess()
        {
            Process.Process_Pid = default;
        }

        /// <summary>
        /// Return log.
        /// </summary>
        public string Log => PS3MAPI_Client_Server.Log;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="address"></param>
        /// <param name="buffer"></param>
        public void SetMemory(uint address, byte[] buffer)
        {
            Process.Memory.Set(Process.Process_Pid, address, buffer);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="address"></param>
        /// <param name="buffer"></param>
        public void GetMemory(uint address, byte[] buffer)
        {
            Process.Memory.Get(Process.Process_Pid, address, buffer);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="address"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public byte[] GetBytes(uint address, uint length)
        {
            byte[] buffer = new byte[length];
            Process.Memory.Get(Process.Process_Pid, address, buffer);
            return buffer;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int[] GetModules()
        {
            return Process.Modules.GetPrxIdModules(Process.Process_Pid);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prxid"></param>
        /// <returns></returns>
        public string GetModuleName(int prxid)
        {
            return Process.Modules.GetName(Process.Process_Pid, prxid);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prxid"></param>
        /// <returns></returns>
        public string GetModuleFilename(int prxid)
        {
            return Process.Modules.GetFilename(Process.Process_Pid, prxid);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public ulong LoadModule(string path)
        {
            try
            {
                Process.Modules.Load(Process.Process_Pid, path);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prxid"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public ulong UnloadModule(int prxid)
        {
            try
            {
                Process.Modules.Unload(Process.Process_Pid, prxid);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        public class SERVER_CMD
        {
            /// <summary>
            /// Specified Timeout: Defaults to 5000 (5 seconds)
            /// </summary>
            public int Timeout
            {
                get => PS3MAPI_Client_Server.Timeout;
                set => PS3MAPI_Client_Server.Timeout = value;
            }

            /// <summary>
            /// Get PS3 Manager API Server Version.
            /// </summary>
            /// <returns></returns>
            public uint GetVersion()
            {
                try
                {
                    return PS3MAPI_Client_Server.Server_Get_Version();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
            }
            /// <summary>
            /// Get PS3 Manager API Server Version.
            /// </summary>
            /// <returns></returns>
            public string GetVersion_Str()
            {
                string ver = PS3MAPI_Client_Server.Server_Get_Version().ToString("X4");
                string char1 = ver.Substring(1, 1) + ".";
                string char2 = ver.Substring(2, 1) + ".";
                string char3 = ver.Substring(3, 1);
                return char1 + char2 + char3;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public class CORE_CMD
        {
            /// <summary>
            /// Get PS3 Manager API Core Version.
            /// </summary>
            /// <returns></returns>
            public uint GetVersion()
            {
                try
                {
                    return PS3MAPI_Client_Server.Core_Get_Version();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
            }

            /// <summary>
            /// Get PS3 Manager API Core Version.
            /// </summary>
            /// <returns></returns>
            public string GetVersion_Str()
            {
                string ver = PS3MAPI_Client_Server.Core_Get_Version().ToString("X4");
                string char1 = ver.Substring(1, 1) + ".";
                string char2 = ver.Substring(2, 1) + ".";
                string char3 = ver.Substring(3, 1);
                return char1 + char2 + char3;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public class PS3_CMD
        {
            /// <summary>
            /// Get PS3 Firmware Version.
            /// </summary>
            /// <returns></returns>
            public uint GetFirmwareVersion()
            {
                try
                {
                    return PS3MAPI_Client_Server.PS3_GetFwVersion();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
            }

            /// <summary>
            /// Get PS3 Firmware Version.
            /// </summary>
            /// <returns></returns>
            public string GetFirmwareVersion_Str()
            {
                string ver = PS3MAPI_Client_Server.PS3_GetFwVersion().ToString("X4");
                string char1 = ver.Substring(1, 1) + ".";
                string char2 = ver.Substring(2, 1);
                string char3 = ver.Substring(3, 1);
                return char1 + char2 + char3;
            }

            /// <summary>
            /// Get PS3 Firmware Type.
            /// </summary>
            /// <returns></returns>
            public string GetFirmwareType()
            {
                try
                {
                    return PS3MAPI_Client_Server.PS3_GetFirmwareType();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
            }

            /// <summary>
            /// 
            /// </summary>
            public enum PowerFlags
            {
                /// <summary>
                /// 
                /// </summary>
                ShutDown,
                /// <summary>
                /// 
                /// </summary>
                QuickReboot,
                /// <summary>
                /// 
                /// </summary>
                SoftReboot,
                /// <summary>
                /// 
                /// </summary>
                HardReboot
            }

            /// <summary>
            /// Shutdown Or Reboot PS3.
            /// </summary>
            /// <param name="flag">Shutdown, QuickReboot, SoftReboot, HardReboot</param>
            public void Power(PowerFlags flag)
            {
                try
                {
                    if (flag == PowerFlags.ShutDown) PS3MAPI_Client_Server.PS3_Shutdown();
                    else if (flag == PowerFlags.QuickReboot) PS3MAPI_Client_Server.PS3_Reboot();
                    else if (flag == PowerFlags.SoftReboot) PS3MAPI_Client_Server.PS3_SoftReboot();
                    else if (flag == PowerFlags.HardReboot) PS3MAPI_Client_Server.PS3_HardReboot();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
            }

            /// <summary>
            /// PS3 VSH Notify.
            /// </summary>
            /// <param name="msg)">Your message</param>
            public void Notify(string msg)
            {
                try
                {
                    PS3MAPI_Client_Server.PS3_Notify(msg);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
            }

            /// <summary>
            /// 
            /// </summary>
            public enum BuzzerMode
            {
                /// <summary>
                /// 
                /// </summary>
                Single,
                /// <summary>
                /// 
                /// </summary>
                Double,
                /// <summary>
                /// 
                /// </summary>
                Triple
            }

            /// <summary>
            /// Ring PS3 Buzzer.
            /// </summary>
            /// <param name="mode">Simple, Double, Continuous</param>
            public void RingBuzzer(BuzzerMode mode)
            {
                try
                {
                    if (mode == BuzzerMode.Single) PS3MAPI_Client_Server.PS3_Buzzer(1);
                    else if (mode == BuzzerMode.Double) PS3MAPI_Client_Server.PS3_Buzzer(2);
                    else if (mode == BuzzerMode.Triple) PS3MAPI_Client_Server.PS3_Buzzer(3);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
            }

            /// <summary>
            /// 
            /// </summary>
            public enum LedColor
            {
                /// <summary>
                /// 
                /// </summary>
                Red = 0,
                /// <summary>
                /// 
                /// </summary>
                Green = 1,
                /// <summary>
                /// 
                /// </summary>
                Yellow = 2
            }

            /// <summary>
            /// 
            /// </summary>
            public enum LedMode
            {
                /// <summary>
                /// 
                /// </summary>
                Off = 0,
                /// <summary>
                /// 
                /// </summary>
                On = 1,
                /// <summary>
                /// 
                /// </summary>
                BlinkFast = 2,
                /// <summary>
                /// 
                /// </summary>
                BlinkSlow = 3
            }

            /// <summary>
            /// PS3 Led.
            /// </summary>
            /// <param name="color">Red, Green, Yellow</param>
            /// <param name="mode">Off, On, BlinkFast, BlinkSlow</param>
            public void Led(LedColor color, LedMode mode)
            {
                try
                {
                    PS3MAPI_Client_Server.PS3_Led(Convert.ToInt32(color), Convert.ToInt32(mode));
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
            }

            /// <summary>
            /// Get PS3 Temperature.
            /// </summary>
            /// <param name="cpu">Return value for the cpu temperature in Celsius.</param>
            /// <param name="rsx">Return value for the rsx temperature in Celsius.</param>
            public void GetTemperature(out uint cpu, out uint rsx)
            {
                cpu = 0; rsx = 0;
                try
                {
                    PS3MAPI_Client_Server.PS3_GetTemp(out cpu, out rsx);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
            }

            /// <summary>
            /// Disable PS3 Syscall.
            /// </summary>
            /// <param name="num">Syscall number</param>
            public void DisableSyscall(int num)
            {
                try
                {
                    PS3MAPI_Client_Server.PS3_DisableSyscall(num);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
            }

            /// <summary>
            /// Return true if this syscall is enabled
            /// </summary>
            /// <param name="num">Syscall number</param>
            /// <returns></returns>
            public bool CheckSyscall(int num)
            {
                try
                {
                    return PS3MAPI_Client_Server.PS3_CheckSyscall(num);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
            }

            /// <summary>
            /// 
            /// </summary>
            public enum Syscall8Mode
            {
                /// <summary>
                /// 
                /// </summary>
                Enabled = 0,
                /// <summary>
                /// 
                /// </summary>
                Only_CobraMambaAndPS3MAPI_Enabled = 1,
                /// <summary>
                /// 
                /// </summary>
                Only_PS3MAPI_Enabled = 2,
                /// <summary>
                /// 
                /// </summary>
                FakeDisabled = 3,
                /// <summary>
                /// 
                /// </summary>
                Disabled = 4
            }

            /// <summary>
            /// Partial Disable PS3 Syscall 8.
            /// </summary>
            /// <param name="mode">Enabled, Only Cobra and PS3M_API features enabled, Only PS3M_API features enabled, Fake Disable</param>
            public void PartialDisableSyscall8(Syscall8Mode mode)
            {
                try
                {
                    if (mode == Syscall8Mode.Enabled) PS3MAPI_Client_Server.PS3_PartialDisableSyscall8(0);
                    else if (mode == Syscall8Mode.Only_CobraMambaAndPS3MAPI_Enabled) PS3MAPI_Client_Server.PS3_PartialDisableSyscall8(1);
                    else if (mode == Syscall8Mode.Only_PS3MAPI_Enabled) PS3MAPI_Client_Server.PS3_PartialDisableSyscall8(2);
                    else if (mode == Syscall8Mode.FakeDisabled) PS3MAPI_Client_Server.PS3_PartialDisableSyscall8(3);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
            }

            /// <summary>
            /// Check Partial Syscall8 disable
            /// </summary>
            /// <returns></returns>
            public Syscall8Mode PartialCheckSyscall8()
            {
                try
                {
                    if (PS3MAPI_Client_Server.PS3_PartialCheckSyscall8() == 0) return Syscall8Mode.Enabled;
                    else if (PS3MAPI_Client_Server.PS3_PartialCheckSyscall8() == 1) return Syscall8Mode.Only_CobraMambaAndPS3MAPI_Enabled;
                    else if (PS3MAPI_Client_Server.PS3_PartialCheckSyscall8() == 2) return Syscall8Mode.Only_PS3MAPI_Enabled;
                    else return Syscall8Mode.FakeDisabled;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
            }

            /// <summary>
            /// Remove COBRA/MAMBA Hook.
            /// </summary>
            public void RemoveHook()
            {
                try
                {
                    PS3MAPI_Client_Server.PS3_RemoveHook();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
            }

            /// <summary>
            /// Clear PS3 History.
            /// </summary>
            /// <param name="include_directory">If set to true, "unsafe" directory will be also deleted.</param>
            public void ClearHistory(bool include_directory = true)
            {
                try
                {
                    PS3MAPI_Client_Server.PS3_ClearHistory(include_directory);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
            }

        }

        /// <summary>
        /// 
        /// </summary>
        public class PROCESS_CMD
        {
            /// <summary>
            /// 
            /// </summary>
            public MEMORY_CMD Memory { get; set; } = new MEMORY_CMD();

            /// <summary>
            /// 
            /// </summary>
            public MODULES_CMD Modules { get; set; } = new MODULES_CMD();

            /// <summary>
            /// 
            /// </summary>
            public VSH_PLUGINS_CMD VSH_Plugins { get; set; } = new VSH_PLUGINS_CMD();

            /// <summary>
            /// 
            /// </summary>
            public PROCESS_CMD()
            {
                Memory = new MEMORY_CMD();
                Modules = new MODULES_CMD();
                VSH_Plugins = new VSH_PLUGINS_CMD();
            }

            /// <summary>
            /// Return all process_pid
            /// </summary>
            public uint[] Processes_Pid => PS3MAPI_Client_Server.Processes_Pid;

            /// <summary>
            /// Return current attached process_pid
            /// </summary>
            public uint Process_Pid
            {
                get => PS3MAPI_Client_Server.Process_Pid;
                set => PS3MAPI_Client_Server.Process_Pid = value;
            }

            /// <summary>
            /// Get a pid list of all runing processes.
            /// </summary>
            /// <returns></returns>
            public uint[] GetPidProcesses()
            {
                try
                {
                    return PS3MAPI_Client_Server.Process_GetPidList();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
            }

            /// <summary>
            /// Get name of this process.
            /// </summary>
            /// <param name="pid">Process Pid</param>
            /// <returns></returns>
            public string GetName(uint pid)
            {
                try
                {
                    return PS3MAPI_Client_Server.Process_GetName(pid);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
            }

            /// <summary>
            /// 
            /// </summary>
            public class MEMORY_CMD
            {
                /// <summary>
                /// Set memory to the attached process.
                /// </summary>
                /// <param name="Pid">Process Pid</param>
                /// <param name="Address">Address</param>
                /// <param name="Bytes">Bytes</param>
                public void Set(uint Pid, ulong Address, byte[] Bytes)
                {
                    try
                    {
                        PS3MAPI_Client_Server.Memory_Set(Pid, Address, Bytes);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message, ex);
                    }
                }

                /// <summary>
                /// Get memory from the attached process.
                /// </summary>
                /// <param name="Pid">Process Pid</param>
                /// <param name="Address">Address</param>
                /// <param name="Bytes">Bytes</param>
                public void Get(uint Pid, ulong Address, byte[] Bytes)
                {
                    try
                    {
                        PS3MAPI_Client_Server.Memory_Get(Pid, Address, Bytes);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message, ex);
                    }
                }

                /// <summary>
                /// Get memory from the attached process.
                /// </summary>
                /// <param name="Pid">Process Pid</param>
                /// <param name="Address">Address</param>
                /// <param name="Length">Length</param>
                /// <returns></returns>
                public byte[] Get(uint Pid, ulong Address, uint Length)
                {
                    try
                    {
                        byte[] buffer = new byte[Length];
                        PS3MAPI_Client_Server.Memory_Get(Pid, Address, buffer);
                        return buffer;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message, ex);
                    }
                }
            }

            /// <summary>
            /// 
            /// </summary>
            public class MODULES_CMD
            {
                /// <summary>
                /// Return all modules_prx_id
                /// </summary>
                public int[] Modules_Prx_Id => PS3MAPI_Client_Server.Modules_Prx_Id;


                /// <summary>
                /// Get a prx_id list of all modules for this process.
                /// </summary>
                /// <param name="pid">Process Pid</param>
                /// <returns></returns>
                public int[] GetPrxIdModules(uint pid)
                {
                    try
                    {
                        return PS3MAPI_Client_Server.Module_GetPrxIdList(pid);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message, ex);
                    }
                }

                /// <summary>
                /// Get name of this module
                /// </summary>
                /// <param name="pid">Process Pid</param>
                /// <param name="prxid">Module Prx_id</param>
                /// <returns></returns>
                public string GetName(uint pid, int prxid)
                {
                    try
                    {
                        return PS3MAPI_Client_Server.Module_GetName(pid, prxid);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message, ex);
                    }
                }

                /// <summary>
                /// Get filename of this module
                /// </summary>
                /// <param name="pid">Process Pid</param>
                /// <param name="prxid">Module Prx_id</param>
                /// <returns></returns>
                public string GetFilename(uint pid, int prxid)
                {
                    try
                    {
                        return PS3MAPI_Client_Server.Module_GetFilename(pid, prxid);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message, ex);
                    }
                }

                /// <summary>
                /// Load a module.
                /// </summary>
                /// <param name="pid">Process Pid</param>
                /// <param name="path">Path of the plugin to load.</param>
                /// <returns></returns>
                public void Load(uint pid, string path)
                {
                    try
                    {
                        PS3MAPI_Client_Server.Module_Load(pid, path);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message, ex);
                    }
                }
                /// <summary>
                /// Unload a module.
                /// </summary>
                /// <param name="pid">Process Pid</param>
                /// <param name="prxid">Module Prx_id</param>
                public void Unload(uint pid, int prxid)
                {
                    try
                    {
                        PS3MAPI_Client_Server.Module_Unload(pid, prxid);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message, ex);
                    }
                }
            }

            /// <summary>
            /// 
            /// </summary>
            public class VSH_PLUGINS_CMD
            {
                /// <summary>
                /// Load an vsh plugin.
                /// </summary>
                /// <param name="slot">Slot id</param>
                /// <param name="path">Path of the plugin to load.</param>
                public void Load(uint slot, string path)
                {
                    try
                    {
                        PS3MAPI_Client_Server.VSHPlugins_Load(slot, path);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message, ex);
                    }
                }

                /// <summary>
                /// Unload an vsh plugin.
                /// </summary>
                /// <param name="slot">Slot id</param>
                public void Unload(uint slot)
                {
                    try
                    {
                        PS3MAPI_Client_Server.VSHPlugins_Unload(slot);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message, ex);
                    }
                }

                /// <summary>
                /// Get info of an vsh plugin.
                /// </summary>
                /// <param name="slot">Slot id</param>
                /// <param name="name">Vsh name</param>
                /// <param name="path">Vsh path</param>
                public void GetInfoBySlot(uint slot, out string name, out string path)
                {
                    try
                    {
                        PS3MAPI_Client_Server.VSHPlugins_GetInfoBySlot(slot, out name, out path);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message, ex);
                    }
                }
            }

        }

        #endregion PS3MAPI_CLient

        #region PS3MAPI_Client_Web
        internal class PS3MAPI_Client_Web
        {
            //Not Yet
        }
        #endregion PS3MAPI_Client_Web

        #region PS3MAPI_Client_Server
        internal class PS3MAPI_Client_Server
        {
            #region Private Members

            private static int ps3m_api_server_minversion = 0x0120;
            private static PS3MAPI_ResponseCode eResponseCode;
            private static string sResponse;
            private static string sMessages = "";
            private static string sServerIP = "";
            private static int iPort = 7887;
            private static string sBucket = "";
            private static int iTimeout = 5000;	// 5 Second
            private static uint iPid = 0;
            private static uint[] iprocesses_pid = new uint[16];
            private static int[] imodules_prx_id = new int[64];
            private static string sLog = "";

            #endregion Private Members

            #region Internal Members

            internal static Socket main_sock;
            internal static Socket listening_sock;
            internal static Socket data_sock;
            internal static IPEndPoint main_ipEndPoint;
            internal static IPEndPoint data_ipEndPoint;

            internal enum PS3MAPI_ResponseCode
            {
                DataConnectionAlreadyOpen = 125,
                MemoryStatusOK = 150,
                CommandOK = 200,
                RequestSuccessful = 226,
                EnteringPassiveMode = 227,
                PS3MAPIConnected = 220,
                PS3MAPIConnectedOK = 230,
                MemoryActionCompleted = 250,
                MemoryActionPended = 350
            }

            #endregion Internal Members

            #region Public Properties

            /// <summary>
            /// Return all process_pid
            /// </summary>
            public static string Log => sLog;

            /// <summary>
            /// Return all process_pid
            /// </summary>
            public static uint[] Processes_Pid => iprocesses_pid;

            /// <summary>
            /// Attached process_pid
            /// </summary>
            public static uint Process_Pid
            {
                get => iPid;
                set => iPid = value;
            }

            /// <summary>
            /// Return all modules_prx_id
            /// </summary>
            public static int[] Modules_Prx_Id => imodules_prx_id;

            /// <summary>
            /// User Specified Timeout: Defaults to 5000 (5 seconds)
            /// </summary>
            public static int Timeout
            {
                get => iTimeout;
                set => iTimeout = value;
            }

            /// <summary>
            /// Indicates if PS3MAPI is connected
            /// </summary>
            public static bool IsConnected => main_sock != null && main_sock.Connected;

            /// <summary>
            /// Indicates if PS3MAPI is attached
            /// </summary>
            public static bool IsAttached => iPid != 0;

            #endregion Public Properties

            //SERVER---------------------------------------------------------------------------------
            internal static void Connect()
            {
                Connect(sServerIP, iPort);
            }

            internal static void Connect(string sServer, int Port)
            {
                sServerIP = sServer;
                iPort = Port;
                if (Port.ToString().Length == 0)
                {
                    throw new Exception("Unable to Connect - No Port Specified.");
                }
                if (sServerIP.Length == 0)
                {
                    throw new Exception("Unable to Connect - No Server Specified.");
                }
                if (main_sock != null)
                {
                    if (main_sock.Connected)
                    {
                        return;
                    }
                }
                main_sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                main_ipEndPoint = new IPEndPoint(Dns.GetHostByName(sServerIP).AddressList[0], Port);
                try
                {
                    main_sock.Connect(main_ipEndPoint);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
                ReadResponse();
                if (eResponseCode != PS3MAPI_ResponseCode.PS3MAPIConnected)
                {
                    Fail();
                }
                ReadResponse();
                if (eResponseCode != PS3MAPI_ResponseCode.PS3MAPIConnectedOK)
                {
                    Fail();
                }
                if (Server_GetMinVersion() < ps3m_api_server_minversion)
                {
                    Disconnect();
                    throw new Exception("PS3M_API SERVER (webMAN-MOD) OUTDATED! PLEASE UPDATE.");
                }
                else if (Server_GetMinVersion() > ps3m_api_server_minversion)
                {
                    Disconnect();
                    throw new Exception("PS3M_API PC_LIB (PS3ManagerAPI.dll) OUTDATED! PLEASE UPDATE.");
                }
                return;
            }

            internal static void Disconnect()
            {
                CloseDataSocket();
                if (main_sock != null)
                {
                    if (main_sock.Connected)
                    {
                        SendCommand("DISCONNECT");
                        iPid = 0;
                        main_sock.Close();
                    }
                    main_sock = null;
                }
                main_ipEndPoint = null;
            }

            internal static uint Server_Get_Version()
            {
                if (IsConnected)
                {
                    SendCommand("SERVER GETVERSION");
                    switch (eResponseCode)
                    {
                        case PS3MAPI_ResponseCode.RequestSuccessful:
                        case PS3MAPI_ResponseCode.CommandOK:
                            break;
                        default:
                            Fail();
                            break;
                    }
                    return Convert.ToUInt32(sResponse);
                }
                else
                {
                    throw new Exception("PS3MAPI not connected!");
                }
            }

            internal static uint Server_GetMinVersion()
            {
                if (IsConnected)
                {
                    SendCommand("SERVER GETMINVERSION");
                    switch (eResponseCode)
                    {
                        case PS3MAPI_ResponseCode.RequestSuccessful:
                        case PS3MAPI_ResponseCode.CommandOK:
                            break;
                        default:
                            Fail();
                            break;
                    }
                    return Convert.ToUInt32(sResponse);
                }
                else
                {
                    throw new Exception("PS3MAPI not connected!");
                }
            }

            //CORE-----------------------------------------------------------------------------------
            internal static uint Core_Get_Version()
            {
                if (IsConnected)
                {
                    SendCommand("CORE GETVERSION");
                    switch (eResponseCode)
                    {
                        case PS3MAPI_ResponseCode.RequestSuccessful:
                        case PS3MAPI_ResponseCode.CommandOK:
                            break;
                        default:
                            Fail();
                            break;
                    }
                    return Convert.ToUInt32(sResponse);
                }
                else
                {
                    throw new Exception("PS3MAPI not connected!");
                }
            }
            internal static uint Core_GetMinVersion()
            {
                if (IsConnected)
                {
                    SendCommand("CORE GETMINVERSION");
                    switch (eResponseCode)
                    {
                        case PS3MAPI_ResponseCode.RequestSuccessful:
                        case PS3MAPI_ResponseCode.CommandOK:
                            break;
                        default:
                            Fail();
                            break;
                    }
                    return Convert.ToUInt32(sResponse);
                }
                else
                {
                    throw new Exception("PS3MAPI not connected!");
                }
            }

            //PS3------------------------------------------------------------------------------------
            internal static uint PS3_GetFwVersion()
            {
                if (IsConnected)
                {
                    SendCommand("PS3 GETFWVERSION");
                    switch (eResponseCode)
                    {
                        case PS3MAPI_ResponseCode.RequestSuccessful:
                        case PS3MAPI_ResponseCode.CommandOK:
                            break;
                        default:
                            Fail();
                            break;
                    }
                    return Convert.ToUInt32(sResponse);
                }
                else
                {
                    throw new Exception("PS3MAPI not connected!");
                }
            }

            internal static string PS3_GetFirmwareType()
            {
                if (IsConnected)
                {
                    SendCommand("PS3 GETFWTYPE");
                    switch (eResponseCode)
                    {
                        case PS3MAPI_ResponseCode.RequestSuccessful:
                        case PS3MAPI_ResponseCode.CommandOK:
                            break;
                        default:
                            Fail();
                            break;
                    }
                    return sResponse;
                }
                else
                {
                    throw new Exception("PS3MAPI not connected!");
                }
            }

            internal static void PS3_Shutdown()
            {
                if (IsConnected)
                {
                    SendCommand("PS3 SHUTDOWN");
                    switch (eResponseCode)
                    {
                        case PS3MAPI_ResponseCode.RequestSuccessful:
                        case PS3MAPI_ResponseCode.CommandOK:
                            Disconnect();
                            break;
                        default:
                            Fail();
                            break;
                    }

                }
                else
                {
                    throw new Exception("PS3MAPI not connected!");
                }
            }

            internal static void PS3_Reboot()
            {
                if (IsConnected)
                {
                    SendCommand("PS3 REBOOT");
                    switch (eResponseCode)
                    {
                        case PS3MAPI_ResponseCode.RequestSuccessful:
                        case PS3MAPI_ResponseCode.CommandOK:
                            Disconnect();
                            break;
                        default:
                            Fail();
                            break;
                    }
                }
                else
                {
                    throw new Exception("PS3MAPI not connected!");
                }
            }

            internal static void PS3_SoftReboot()
            {
                if (IsConnected)
                {
                    SendCommand("PS3 SOFTREBOOT");
                    switch (eResponseCode)
                    {
                        case PS3MAPI_ResponseCode.RequestSuccessful:
                        case PS3MAPI_ResponseCode.CommandOK:
                            Disconnect();
                            break;
                        default:
                            Fail();
                            break;
                    }
                }
                else
                {
                    throw new Exception("PS3MAPI not connected!");
                }
            }

            internal static void PS3_HardReboot()
            {
                if (IsConnected)
                {
                    SendCommand("PS3 HARDREBOOT");
                    switch (eResponseCode)
                    {
                        case PS3MAPI_ResponseCode.RequestSuccessful:
                        case PS3MAPI_ResponseCode.CommandOK:
                            Disconnect();
                            break;
                        default:
                            Fail();
                            break;
                    }
                }
                else
                {
                    throw new Exception("PS3MAPI not connected!");
                }
            }

            internal static void PS3_Notify(string msg)
            {
                if (IsConnected)
                {
                    SendCommand("PS3 NOTIFY  " + msg);
                    switch (eResponseCode)
                    {
                        case PS3MAPI_ResponseCode.RequestSuccessful:
                        case PS3MAPI_ResponseCode.CommandOK:
                            break;
                        default:
                            Fail();
                            break;
                    }
                }
                else
                {
                    throw new Exception("PS3MAPI not connected!");
                }
            }

            internal static void PS3_Buzzer(int mode)
            {
                if (IsConnected)
                {
                    SendCommand("PS3 BUZZER" + mode.ToString());
                    switch (eResponseCode)
                    {
                        case PS3MAPI_ResponseCode.RequestSuccessful:
                        case PS3MAPI_ResponseCode.CommandOK:
                            break;
                        default:
                            Fail();
                            break;
                    }
                }
                else
                {
                    throw new Exception("PS3MAPI not connected!");
                }
            }

            internal static void PS3_Led(int color, int mode)
            {
                if (IsConnected)
                {
                    SendCommand("PS3 LED " + color.ToString() + " " + mode.ToString());
                    switch (eResponseCode)
                    {
                        case PS3MAPI_ResponseCode.RequestSuccessful:
                        case PS3MAPI_ResponseCode.CommandOK:
                            break;
                        default:
                            Fail();
                            break;
                    }
                }
                else
                {
                    throw new Exception("PS3MAPI not connected!");
                }
            }

            internal static void PS3_GetTemp(out uint cpu, out uint rsx)
            {
                cpu = 0; rsx = 0;
                if (IsConnected)
                {
                    SendCommand("PS3 GETTEMP");
                    switch (eResponseCode)
                    {
                        case PS3MAPI_ResponseCode.RequestSuccessful:
                        case PS3MAPI_ResponseCode.CommandOK:
                            break;
                        default:
                            Fail();
                            break;
                    }
                    string[] tmp = sResponse.Split(new char[] { '|' });
                    cpu = Convert.ToUInt32(tmp[0], 10);
                    rsx = Convert.ToUInt32(tmp[1], 10);
                }
                else
                {
                    throw new Exception("PS3MAPI not connected!");
                }
            }

            internal static void PS3_DisableSyscall(int num)
            {
                if (IsConnected)
                {
                    SendCommand("PS3 DISABLESYSCALL " + num.ToString());
                    switch (eResponseCode)
                    {
                        case PS3MAPI_ResponseCode.RequestSuccessful:
                        case PS3MAPI_ResponseCode.CommandOK:
                            break;
                        default:
                            Fail();
                            break;
                    }
                }
                else
                {
                    throw new Exception("PS3MAPI not connected!");
                }
            }

            internal static void PS3_ClearHistory(bool include_directory)
            {
                if (IsConnected)
                {
                    if (include_directory) SendCommand("PS3 DELHISTORY+D");
                    else SendCommand("PS3 DELHISTORY");
                    switch (eResponseCode)
                    {
                        case PS3MAPI_ResponseCode.RequestSuccessful:
                        case PS3MAPI_ResponseCode.CommandOK:
                            break;
                        default:
                            Fail();
                            break;
                    }
                }
                else
                {
                    throw new Exception("PS3MAPI not connected!");
                }
            }

            internal static bool PS3_CheckSyscall(int num)
            {
                if (IsConnected)
                {
                    SendCommand("PS3 CHECKSYSCALL " + num.ToString());
                    switch (eResponseCode)
                    {
                        case PS3MAPI_ResponseCode.RequestSuccessful:
                        case PS3MAPI_ResponseCode.CommandOK:
                            break;
                        default:
                            Fail();
                            break;
                    }
                    if (Convert.ToInt32(sResponse) == 0) return true;
                    else return false;
                }
                else
                {
                    throw new Exception("PS3MAPI not connected!");
                }
            }

            internal static void PS3_PartialDisableSyscall8(int mode)
            {
                if (IsConnected)
                {
                    SendCommand("PS3 PDISABLESYSCALL8 " + mode.ToString());
                    switch (eResponseCode)
                    {
                        case PS3MAPI_ResponseCode.RequestSuccessful:
                        case PS3MAPI_ResponseCode.CommandOK:
                            break;
                        default:
                            Fail();
                            break;
                    }
                }
                else
                {
                    throw new Exception("PS3MAPI not connected!");
                }
            }

            internal static int PS3_PartialCheckSyscall8()
            {
                if (IsConnected)
                {
                    SendCommand("PS3 PCHECKSYSCALL8");
                    switch (eResponseCode)
                    {
                        case PS3MAPI_ResponseCode.RequestSuccessful:
                        case PS3MAPI_ResponseCode.CommandOK:
                            break;
                        default:
                            Fail();
                            break;
                    }
                    return Convert.ToInt32(sResponse);
                }
                else
                {
                    throw new Exception("PS3MAPI not connected!");
                }
            }

            internal static void PS3_RemoveHook()
            {
                if (IsConnected)
                {
                    SendCommand("PS3 REMOVEHOOK");
                    switch (eResponseCode)
                    {
                        case PS3MAPI_ResponseCode.RequestSuccessful:
                        case PS3MAPI_ResponseCode.CommandOK:
                            break;
                        default:
                            Fail();
                            break;
                    }
                }
                else
                {
                    throw new Exception("PS3MAPI not connected!");
                }
            }

            internal static string PS3_GetIDPS()
            {
                if (IsConnected)
                {
                    SendCommand("PS3 GETIDPS");
                    switch (eResponseCode)
                    {
                        case PS3MAPI_ResponseCode.RequestSuccessful:
                        case PS3MAPI_ResponseCode.CommandOK:
                            break;
                        default:
                            Fail();
                            break;
                    }
                    return sResponse;
                }
                else
                {
                    throw new Exception("PS3MAPI not connected!");
                }
            }

            internal static void PS3_SetIDPS(string IDPS)
            {
                if (IsConnected)
                {
                    SendCommand("PS3 SETIDPS " + IDPS.Substring(0, 16) + " " + IDPS.Substring(16, 16));
                    switch (eResponseCode)
                    {
                        case PS3MAPI_ResponseCode.RequestSuccessful:
                        case PS3MAPI_ResponseCode.CommandOK:
                            break;
                        default:
                            Fail();
                            break;
                    }
                }
                else
                {
                    throw new Exception("PS3MAPI not connected!");
                }
            }

            internal static string PS3_GetPSID()
            {
                if (IsConnected)
                {
                    SendCommand("PS3 GETPSID");
                    switch (eResponseCode)
                    {
                        case PS3MAPI_ResponseCode.RequestSuccessful:
                        case PS3MAPI_ResponseCode.CommandOK:
                            break;
                        default:
                            Fail();
                            break;
                    }
                    return sResponse;
                }
                else
                {
                    throw new Exception("PS3MAPI not connected!");
                }
            }

            internal static void PS3_SetPSID(string PSID)
            {
                if (IsConnected)
                {
                    SendCommand("PS3 SETPSID " + PSID.Substring(0, 16) + " " + PSID.Substring(16, 16));
                    switch (eResponseCode)
                    {
                        case PS3MAPI_ResponseCode.RequestSuccessful:
                        case PS3MAPI_ResponseCode.CommandOK:
                            break;
                        default:
                            Fail();
                            break;
                    }
                }
                else
                {
                    throw new Exception("PS3MAPI not connected!");
                }
            }

            //PROCESS--------------------------------------------------------------------------------
            internal static string Process_GetName(uint pid)
            {
                if (IsConnected)
                {
                    SendCommand("PROCESS GETNAME " + string.Format("{0}", pid));
                    switch (eResponseCode)
                    {
                        case PS3MAPI_ResponseCode.RequestSuccessful:
                        case PS3MAPI_ResponseCode.CommandOK:
                            break;
                        default:
                            Fail();
                            break;
                    }
                    return sResponse;
                }
                else
                {
                    throw new Exception("PS3MAPI not connected!");
                }
            }

            internal static uint[] Process_GetPidList()
            {
                if (IsConnected)
                {
                    SendCommand("PROCESS GETALLPID");
                    switch (eResponseCode)
                    {
                        case PS3MAPI_ResponseCode.RequestSuccessful:
                        case PS3MAPI_ResponseCode.CommandOK:
                            break;
                        default:
                            Fail();
                            break;
                    }
                    int i = 0;
                    iprocesses_pid = new uint[16];
                    foreach (string s in sResponse.Split(new char[] { '|' }))
                    {
                        if (s.Length != 0 && s != null && s != "" && s != " " && s != "0") { iprocesses_pid[i] = Convert.ToUInt32(s, 10); i++; }
                    }
                    return iprocesses_pid;
                }
                else
                {
                    throw new Exception("PS3MAPI not connected!");
                }
            }

            //MEMORY--------------------------------------------------------------------------------
            internal static void Memory_Get(uint Pid, ulong Address, byte[] Bytes)
            {
                if (IsConnected)
                {
                    SetBinaryMode(true);
                    int BytesLength = Bytes.Length;
                    long TotalBytes = 0;
                    long lBytesReceived;
                    bool bComplete = false;
                    OpenDataSocket();
                    SendCommand("MEMORY GET " + string.Format("{0}", Pid) + " " + string.Format("{0:X16}", Address) + " " + string.Format("{0}", Bytes.Length));
                    switch (eResponseCode)
                    {
                        case PS3MAPI_ResponseCode.DataConnectionAlreadyOpen:
                        case PS3MAPI_ResponseCode.MemoryStatusOK:
                            break;
                        default:
                            throw new Exception(sResponse);
                    }
                    ConnectDataSocket();
                    byte[] buffer = new byte[Bytes.Length];
                    while (bComplete != true)
                    {
                        try
                        {
                            lBytesReceived = data_sock.Receive(buffer, BytesLength, 0);
                            if (lBytesReceived > 0)
                            {
                                Buffer.BlockCopy(buffer, 0, Bytes, (int)TotalBytes, (int)lBytesReceived);
                                TotalBytes += lBytesReceived;
                                if ((int)(((TotalBytes) * 100) / BytesLength) >= 100) bComplete = true;
                            }
                            else
                            {
                                bComplete = true;
                            }
                            if (bComplete)
                            {
                                CloseDataSocket();
                                ReadResponse();
                                switch (eResponseCode)
                                {
                                    case PS3MAPI_ResponseCode.RequestSuccessful:
                                    case PS3MAPI_ResponseCode.MemoryActionCompleted:
                                        break;
                                    default:
                                        throw new Exception(sResponse);
                                }
                                SetBinaryMode(false);
                            }
                        }
                        catch (Exception e)
                        {
                            CloseDataSocket();
                            ReadResponse();
                            SetBinaryMode(false);
                            throw e;
                        }
                    }
                }
                else
                {
                    throw new Exception("PS3MAPI not connected!");
                }
            }

            internal static void Memory_Set(uint Pid, ulong Address, byte[] Bytes)
            {
                if (IsConnected)
                {
                    SetBinaryMode(true);
                    int BytesLength = Bytes.Length;
                    long TotalBytes = 0;
                    long lBytesSended = 0;
                    bool bComplete = false;
                    OpenDataSocket();
                    SendCommand("MEMORY SET " + string.Format("{0}", Pid) + " " + string.Format("{0:X16}", Address));
                    switch (eResponseCode)
                    {
                        case PS3MAPI_ResponseCode.DataConnectionAlreadyOpen:
                        case PS3MAPI_ResponseCode.MemoryStatusOK:
                            break;
                        default:
                            throw new Exception(sResponse);
                    }
                    ConnectDataSocket();
                    while (bComplete != true)
                    {
                        try
                        {
                            byte[] buffer = new byte[BytesLength - (int)TotalBytes];
                            Buffer.BlockCopy(Bytes, (int)lBytesSended, buffer, 0, (BytesLength - (int)TotalBytes));
                            lBytesSended = data_sock.Send(buffer, (Bytes.Length - (int)TotalBytes), 0);
                            bComplete = false;
                            if (lBytesSended > 0)
                            {
                                TotalBytes += lBytesSended;
                                if ((int)(((TotalBytes) * 100) / BytesLength) >= 100) bComplete = true;
                            }
                            else
                            {
                                bComplete = true;
                            }
                            if (bComplete)
                            {
                                CloseDataSocket();
                                ReadResponse();
                                switch (eResponseCode)
                                {
                                    case PS3MAPI_ResponseCode.RequestSuccessful:
                                    case PS3MAPI_ResponseCode.MemoryActionCompleted:
                                        break;
                                    default:
                                        throw new Exception(sResponse);
                                }
                                SetBinaryMode(false);
                            }
                        }
                        catch (Exception e)
                        {
                            CloseDataSocket();
                            ReadResponse();
                            SetBinaryMode(false);
                            throw e;
                        }
                    }
                }
                else
                {
                    throw new Exception("PS3MAPI not connected!");
                }
            }

            //MODULES--------------------------------------------------------------------------------
            internal static int[] Module_GetPrxIdList(uint pid)
            {
                if (IsConnected)
                {
                    SendCommand("MODULE GETALLPRXID " + string.Format("{0}", pid));
                    switch (eResponseCode)
                    {
                        case PS3MAPI_ResponseCode.RequestSuccessful:
                        case PS3MAPI_ResponseCode.CommandOK:
                            break;
                        default:
                            Fail();
                            break;
                    }
                    int i = 0;
                    imodules_prx_id = new int[128];
                    foreach (string s in sResponse.Split(new char[] { '|' }))
                    {
                        if (s.Length != 0 && s != null && s != "" && s != " " && s != "0") { imodules_prx_id[i] = Convert.ToInt32(s, 10); i++; }
                    }
                    return imodules_prx_id;
                }
                else
                {
                    throw new Exception("PS3MAPI not connected!");
                }
            }

            internal static string Module_GetName(uint pid, int prxid)
            {
                if (IsConnected)
                {
                    SendCommand("MODULE GETNAME " + string.Format("{0}", pid) + " " + prxid.ToString());
                    switch (eResponseCode)
                    {
                        case PS3MAPI_ResponseCode.RequestSuccessful:
                        case PS3MAPI_ResponseCode.CommandOK:
                            break;
                        default:
                            Fail();
                            break;
                    }
                    return sResponse;
                }
                else
                {
                    throw new Exception("PS3MAPI not connected!");
                }
            }

            internal static string Module_GetFilename(uint pid, int prxid)
            {
                if (IsConnected)
                {
                    SendCommand("MODULE GETFILENAME " + string.Format("{0}", pid) + " " + prxid.ToString());
                    switch (eResponseCode)
                    {
                        case PS3MAPI_ResponseCode.RequestSuccessful:
                        case PS3MAPI_ResponseCode.CommandOK:
                            break;
                        default:
                            Fail();
                            break;
                    }
                    return sResponse;
                }
                else
                {
                    throw new Exception("PS3MAPI not connected!");
                }
            }

            internal static void Module_Load(uint pid, string path)
            {
                if (IsConnected)
                {
                    SendCommand("MODULE LOAD " + string.Format("{0}", pid) + " " + path);
                    switch (eResponseCode)
                    {
                        case PS3MAPI_ResponseCode.RequestSuccessful:
                        case PS3MAPI_ResponseCode.CommandOK:
                            break;
                        default:
                            Fail();
                            break;
                    }
                }
                else
                {
                    throw new Exception("PS3MAPI not connected!");
                }
            }
            internal static void Module_Unload(uint pid, int prx_id)
            {
                if (IsConnected)
                {
                    SendCommand("MODULE UNLOAD " + string.Format("{0}", pid) + " " + prx_id.ToString());
                    switch (eResponseCode)
                    {
                        case PS3MAPI_ResponseCode.RequestSuccessful:
                        case PS3MAPI_ResponseCode.CommandOK:
                            break;
                        default:
                            Fail();
                            break;
                    }
                }
                else
                {
                    throw new Exception("PS3MAPI not connected!");
                }
            }

            //VSH PLUGINS (MODULES)-------------------------------------------------------------------
            internal static void VSHPlugins_GetInfoBySlot(uint slot, out string name, out string path)
            {
                name = ""; path = "";
                if (IsConnected)
                {
                    SendCommand("MODULE GETVSHPLUGINFO " + string.Format("{0}", slot));
                    switch (eResponseCode)
                    {
                        case PS3MAPI_ResponseCode.RequestSuccessful:
                        case PS3MAPI_ResponseCode.CommandOK:
                            break;
                        default:
                            Fail();
                            break;
                    }
                    string[] tmp = sResponse.Split(new char[] { '|' });
                    name = tmp[0];
                    path = tmp[1];
                }
                else
                {
                    throw new Exception("PS3MAPI not connected!");
                }
            }

            internal static void VSHPlugins_Load(uint slot, string path)
            {
                if (IsConnected)
                {
                    SendCommand("MODULE LOADVSHPLUG " + string.Format("{0}", slot) + " " + path);
                    switch (eResponseCode)
                    {
                        case PS3MAPI_ResponseCode.RequestSuccessful:
                        case PS3MAPI_ResponseCode.CommandOK:
                            break;
                        default:
                            Fail();
                            break;
                    }
                }
                else
                {
                    throw new Exception("PS3MAPI not connected!");
                }
            }

            internal static void VSHPlugins_Unload(uint slot)
            {
                if (IsConnected)
                {
                    SendCommand("MODULE UNLOADVSHPLUGS " + string.Format("{0}", slot));
                    switch (eResponseCode)
                    {
                        case PS3MAPI_ResponseCode.RequestSuccessful:
                        case PS3MAPI_ResponseCode.CommandOK:
                            break;
                        default:
                            Fail();
                            break;
                    }
                }
                else
                {
                    throw new Exception("PS3MAPI not connected!");
                }
            }

            //----------------------------------------------------------------------------------------
            internal static void Fail()
            {
                Fail(new Exception("[" + eResponseCode.ToString() + "] " + sResponse));
            }

            internal static void Fail(Exception e)
            {
                Disconnect();
                throw e;
            }

            internal static void SetBinaryMode(bool bMode)
            {
                SendCommand("TYPE" + ((bMode) ? " I" : " A"));
                switch (eResponseCode)
                {
                    case PS3MAPI_ResponseCode.RequestSuccessful:
                    case PS3MAPI_ResponseCode.CommandOK:
                        break;
                    default:
                        Fail();
                        break;
                }
            }

            internal static void OpenDataSocket()
            {
                string[] pasv;
                string sServer;
                int iPort;
                Connect();
                SendCommand("PASV");
                if (eResponseCode != PS3MAPI_ResponseCode.EnteringPassiveMode)
                {
                    Fail();
                }
                try
                {
                    int i1, i2;
                    i1 = sResponse.IndexOf('(') + 1;
                    i2 = sResponse.IndexOf(')') - i1;
                    pasv = sResponse.Substring(i1, i2).Split(',');
                }
                catch (Exception)
                {
                    Fail(new Exception("Malformed PASV response: " + sResponse));
                    throw new Exception("Malformed PASV response: " + sResponse);
                }

                if (pasv.Length < 6)
                {
                    Fail(new Exception("Malformed PASV response: " + sResponse));
                }

                sServer = string.Format("{0}.{1}.{2}.{3}", pasv[0], pasv[1], pasv[2], pasv[3]);
                iPort = (int.Parse(pasv[4]) << 8) + int.Parse(pasv[5]);
                try
                {
                    CloseDataSocket();
                    data_sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    data_ipEndPoint = new IPEndPoint(Dns.GetHostByName(sServerIP).AddressList[0], iPort);
                    data_sock.Connect(data_ipEndPoint);
                }
                catch (Exception e)
                {
                    throw new Exception("Failed to connect for data transfer: " + e.Message);
                }
            }

            internal static void ConnectDataSocket()
            {
                if (data_sock != null)		// already connected (always so if passive mode)
                    return;
                try
                {
                    data_sock = listening_sock.Accept();	// Accept is blocking
                    listening_sock.Close();
                    listening_sock = null;
                    if (data_sock == null)
                    {
                        throw new Exception("Winsock error: " +
                            Convert.ToString(System.Runtime.InteropServices.Marshal.GetLastWin32Error()));
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to connect for data transfer: " + ex.Message);
                }
            }

            internal static void CloseDataSocket()
            {
                if (data_sock != null)
                {
                    if (data_sock.Connected)
                    {
                        data_sock.Close();
                    }
                    data_sock = null;
                }
                data_ipEndPoint = null;
            }

            internal static void ReadResponse()
            {
                string sBuffer;
                sMessages = "";
                while (true)
                {
                    sBuffer = GetLineFromBucket();
                    if (Regex.Match(sBuffer, "^[0-9]+ ").Success)
                    {
                        sResponse = sBuffer.Substring(4).Replace("\r", "").Replace("\n", "");
                        eResponseCode = (PS3MAPI_ResponseCode)int.Parse(sBuffer.Substring(0, 3));
                        sLog = sLog + "RESPONSE CODE: " + eResponseCode.ToString() + Environment.NewLine;
                        sLog = sLog + "RESPONSE MSG: " + sResponse + Environment.NewLine + Environment.NewLine;
                        break;
                    }
                    else
                    {
                        sMessages += Regex.Replace(sBuffer, "^[0-9]+-", "") + "\n";
                    }
                }
            }
            internal static void SendCommand(string sCommand)
            {
                sLog = sLog + "COMMAND: " + sCommand + Environment.NewLine;
                Connect();
                byte[] byCommand = Encoding.ASCII.GetBytes((sCommand + "\r\n").ToCharArray());
                main_sock.Send(byCommand, byCommand.Length, 0);
                ReadResponse();
            }

            internal static void FillBucket()
            {
                byte[] bytes = new byte[512];
                long lBytesRecieved;
                int iMilliSecondsPassed = 0;
                while (main_sock.Available < 1)
                {
                    System.Threading.Thread.Sleep(50);
                    iMilliSecondsPassed += 50;

                    if (iMilliSecondsPassed > Timeout) // Prevents infinite loop
                    {
                        Fail(new Exception("Timed out waiting on server to respond."));
                    }
                }
                while (main_sock.Available > 0)
                {
                    // gives any further data not yet received, a small chance to arrive
                    lBytesRecieved = main_sock.Receive(bytes, 512, 0);
                    sBucket += Encoding.ASCII.GetString(bytes, 0, (int)lBytesRecieved);
                    System.Threading.Thread.Sleep(50);
                }
            }

            internal static string GetLineFromBucket()
            {
                int i = sBucket.IndexOf('\n');

                while (i < 0)
                {
                    FillBucket();
                    i = sBucket.IndexOf('\n');
                }

                string sBuffer = sBucket.Substring(0, i);
                sBucket = sBucket.Substring(i + 1);

                return sBuffer;
            }
        }
        #endregion PS3MAPI_Client_Server
    }
}
