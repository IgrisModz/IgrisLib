/**
*
* CCAPI C# Wrapper made by Enstone
* Compatible with CCAPI 2.60, CCAPI 2.70, CCAPI 2.80
* Requires CCAPI.dll
* V1.00
*
**/

using IgrisLib.Resources;
using IgrisLib.Views;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;

namespace IgrisLib
{
    /// <summary>
    /// 
    /// </summary>
    public class CCAPI : IApi, IConnectAPI
    {
        /// <summary>
        /// 
        /// </summary>
        public string FullName => "Control Console";

        /// <summary>
        /// 
        /// </summary>
        public string Name => "CCAPI";

        /// <summary>
        /// 
        /// </summary>
        public string IPAddress { get; private set; } = "127.0.0.1";

        /// <summary>
        /// 
        /// </summary>
        public Extension Extension
        {
            get
            {
                return new Extension(this);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public struct ProcessInfo
        {
            /// <summary>
            /// 
            /// </summary>
            public uint pid;
            /// <summary>
            /// 
            /// </summary>
            public string name;
        };

        /// <summary>
        /// 
        /// </summary>
        public class ConsoleInfo
        {
            /// <summary>
            /// 
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string Ip { get; set; }
        };
        /// <summary>
        /// 
        /// </summary>
        public enum ConsoleIdType
        {
            /// <summary>
            /// 
            /// </summary>
            Idps = 0,
            /// <summary>
            /// 
            /// </summary>
            Psid = 1,
        };

        /// <summary>
        /// 
        /// </summary>
        public enum ShutdownMode
        {
            /// <summary>
            /// 
            /// </summary>
            Shutdown = 1,
            /// <summary>
            /// 
            /// </summary>
            SoftReboot = 2,
            /// <summary>
            /// 
            /// </summary>
            HardReboot = 3,
        };

        /// <summary>
        /// 
        /// </summary>
        public enum BuzzerType
        {
            /// <summary>
            /// 
            /// </summary>
            Continuous = 0,
            /// <summary>
            /// 
            /// </summary>
            Single = 1,
            /// <summary>
            /// 
            /// </summary>
            Double = 2,
            /// <summary>
            /// 
            /// </summary>
            Triple = 3,
        };

        /// <summary>
        /// 
        /// </summary>
        public enum ColorLed
        {
            /// <summary>
            /// 
            /// </summary>
            Green = 1,
            /// <summary>
            /// 
            /// </summary>
            Red = 2,
        };

        /// <summary>
        /// 
        /// </summary>
        public enum StatusLed
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
            Blink = 2,
        };

        /// <summary>
        /// 
        /// </summary>
        public enum NotifyIcon
        {
            /// <summary>
            /// 
            /// </summary>
            Info = 0,
            /// <summary>
            /// 
            /// </summary>
            Caution = 1,
            /// <summary>
            /// 
            /// </summary>
            Friend = 2,
            /// <summary>
            /// 
            /// </summary>
            Slider = 3,
            /// <summary>
            /// 
            /// </summary>
            WrongWay = 4,
            /// <summary>
            /// 
            /// </summary>
            Dialog = 5,
            /// <summary>
            /// 
            /// </summary>
            DalogShadow = 6,
            /// <summary>
            /// 
            /// </summary>
            Text = 7,
            /// <summary>
            /// 
            /// </summary>
            Pointer = 8,
            /// <summary>
            /// 
            /// </summary>
            Grab = 9,
            /// <summary>
            /// 
            /// </summary>
            Hand = 10,
            /// <summary>
            /// 
            /// </summary>
            Pen = 11,
            /// <summary>
            /// 
            /// </summary>
            Finger = 12,
            /// <summary>
            /// 
            /// </summary>
            Arrow = 13,
            /// <summary>
            /// 
            /// </summary>
            ArrowRight = 14,
            /// <summary>
            /// 
            /// </summary>
            Progress = 15,
            /// <summary>
            /// 
            /// </summary>
            Trophy1 = 16,
            /// <summary>
            /// 
            /// </summary>
            Trophy2 = 17,
            /// <summary>
            /// 
            /// </summary>
            Trophy3 = 18,
            /// <summary>
            /// 
            /// </summary>
            Trophy4 = 19
        };

        /// <summary>
        /// 
        /// </summary>
        public enum ConsoleType
        {
            /// <summary>
            /// 
            /// </summary>
            UNK = 0,
            /// <summary>
            /// 
            /// </summary>
            CEX = 1,
            /// <summary>
            /// 
            /// </summary>
            DEX = 2,
            /// <summary>
            /// 
            /// </summary>
            TOOL = 3,
        };

        /// <summary>
        /// 
        /// </summary>
        public CCAPI()
        {
            resources = Language.Get;
            ProcessId = 0xFFFFFFFF;
            LibHandle = IntPtr.Zero;
            LibLoaded = false;

            LibLoaded = Init();
        }
        private bool Init()
        {
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("Software\\FrenchModdingTeam\\CCAPI\\InstallFolder");
            if (registryKey != null)
            {
                string text = registryKey.GetValue("path") as string;
                if (!string.IsNullOrEmpty(text))
                {
                    string text2 = text + "\\CCAPI.dll";
                    if (File.Exists(text2))
                    {
                        LibHandle = LoadLibrary(text2);

                        if (LibHandle == IntPtr.Zero)
                        {
                            if (GetLastError() == 193)
                            {

                            }
                            MessageBox.Show(resources["impossibleLoadCCAPI"].ToString(), resources["impossibleLoadCCAPITitle"].ToString(), MessageBoxButton.OK, MessageBoxImage.Hand);
                            return false;
                        }

                        IntPtr pCCAPIConnectConsole = GetProcAddress(LibHandle, "CCAPIConnectConsole");
                        IntPtr pCCAPIDisconnectConsole = GetProcAddress(LibHandle, "CCAPIDisconnectConsole");
                        IntPtr pCCAPIGetConnectionStatus = GetProcAddress(LibHandle, "CCAPIGetConnectionStatus");
                        IntPtr pCCAPISetBootConsoleIds = GetProcAddress(LibHandle, "CCAPISetBootConsoleIds");
                        IntPtr pCCAPISetConsoleIds = GetProcAddress(LibHandle, "CCAPISetConsoleIds");
                        IntPtr pCCAPISetMemory = GetProcAddress(LibHandle, "CCAPISetMemory");
                        IntPtr pCCAPIGetMemory = GetProcAddress(LibHandle, "CCAPIGetMemory");
                        IntPtr pCCAPIGetProcessList = GetProcAddress(LibHandle, "CCAPIGetProcessList");
                        IntPtr pCCAPIGetProcessName = GetProcAddress(LibHandle, "CCAPIGetProcessName");
                        IntPtr pCCAPIGetTemperature = GetProcAddress(LibHandle, "CCAPIGetTemperature");
                        IntPtr pCCAPIShutdown = GetProcAddress(LibHandle, "CCAPIShutdown");
                        IntPtr pCCAPIRingBuzzer = GetProcAddress(LibHandle, "CCAPIRingBuzzer");
                        IntPtr pCCAPISetConsoleLed = GetProcAddress(LibHandle, "CCAPISetConsoleLed");
                        IntPtr pCCAPIGetFirmwareInfo = GetProcAddress(LibHandle, "CCAPIGetFirmwareInfo");
                        IntPtr pCCAPIVshNotify = GetProcAddress(LibHandle, "CCAPIVshNotify");
                        IntPtr pCCAPIGetNumberOfConsoles = GetProcAddress(LibHandle, "CCAPIGetNumberOfConsoles");
                        IntPtr pCCAPIGetConsoleInfo = GetProcAddress(LibHandle, "CCAPIGetConsoleInfo");
                        IntPtr pCCAPIGetDllVersion = GetProcAddress(LibHandle, "CCAPIGetDllVersion");

                        bool loaded = (pCCAPIConnectConsole != IntPtr.Zero)
                             && (pCCAPIDisconnectConsole != IntPtr.Zero)
                             && (pCCAPIGetConnectionStatus != IntPtr.Zero)
                             && (pCCAPISetBootConsoleIds != IntPtr.Zero)
                             && (pCCAPISetConsoleIds != IntPtr.Zero)
                             && (pCCAPISetMemory != IntPtr.Zero)
                             && (pCCAPIGetMemory != IntPtr.Zero)
                             && (pCCAPIGetProcessList != IntPtr.Zero)
                             && (pCCAPIGetProcessName != IntPtr.Zero)
                             && (pCCAPIGetTemperature != IntPtr.Zero)
                             && (pCCAPIShutdown != IntPtr.Zero)
                             && (pCCAPIRingBuzzer != IntPtr.Zero)
                             && (pCCAPISetConsoleLed != IntPtr.Zero)
                             && (pCCAPIGetFirmwareInfo != IntPtr.Zero)
                             && (pCCAPIVshNotify != IntPtr.Zero)
                             && (pCCAPIGetNumberOfConsoles != IntPtr.Zero)
                             && (pCCAPIGetConsoleInfo != IntPtr.Zero)
                             && (pCCAPIGetDllVersion != IntPtr.Zero);

                        if (!loaded)
                        {
                            MessageBox.Show(resources["impossibleLoadCCAPI"].ToString(), resources["ccapiIsntCompatible"].ToString(), MessageBoxButton.OK, MessageBoxImage.Hand);
                            return false;
                        }

                        CCAPIConnectConsole = (CCAPIConnectConsole_t)Marshal.GetDelegateForFunctionPointer(pCCAPIConnectConsole, typeof(CCAPIConnectConsole_t));
                        CCAPIDisconnectConsole = (CCAPIDisconnectConsole_t)Marshal.GetDelegateForFunctionPointer(pCCAPIDisconnectConsole, typeof(CCAPIDisconnectConsole_t));
                        CCAPIGetConnectionStatus = (CCAPIGetConnectionStatus_t)Marshal.GetDelegateForFunctionPointer(pCCAPIGetConnectionStatus, typeof(CCAPIGetConnectionStatus_t));
                        CCAPISetBootConsoleIds = (CCAPISetBootConsoleIds_t)Marshal.GetDelegateForFunctionPointer(pCCAPISetBootConsoleIds, typeof(CCAPISetBootConsoleIds_t));
                        CCAPISetConsoleIds = (CCAPISetConsoleIds_t)Marshal.GetDelegateForFunctionPointer(pCCAPISetConsoleIds, typeof(CCAPISetConsoleIds_t));
                        CCAPISetMemory = (CCAPISetMemory_t)Marshal.GetDelegateForFunctionPointer(pCCAPISetMemory, typeof(CCAPISetMemory_t));
                        CCAPIGetMemory = (CCAPIGetMemory_t)Marshal.GetDelegateForFunctionPointer(pCCAPIGetMemory, typeof(CCAPIGetMemory_t));
                        CCAPIGetProcessList = (CCAPIGetProcessList_t)Marshal.GetDelegateForFunctionPointer(pCCAPIGetProcessList, typeof(CCAPIGetProcessList_t));
                        CCAPIGetProcessName = (CCAPIGetProcessName_t)Marshal.GetDelegateForFunctionPointer(pCCAPIGetProcessName, typeof(CCAPIGetProcessName_t));
                        CCAPIGetTemperature = (CCAPIGetTemperature_t)Marshal.GetDelegateForFunctionPointer(pCCAPIGetTemperature, typeof(CCAPIGetTemperature_t));
                        CCAPIShutdown = (CCAPIShutdown_t)Marshal.GetDelegateForFunctionPointer(pCCAPIShutdown, typeof(CCAPIShutdown_t));
                        CCAPIRingBuzzer = (CCAPIRingBuzzer_t)Marshal.GetDelegateForFunctionPointer(pCCAPIRingBuzzer, typeof(CCAPIRingBuzzer_t));
                        CCAPISetConsoleLed = (CCAPISetConsoleLed_t)Marshal.GetDelegateForFunctionPointer(pCCAPISetConsoleLed, typeof(CCAPISetConsoleLed_t));
                        CCAPIGetFirmwareInfo = (CCAPIGetFirmwareInfo_t)Marshal.GetDelegateForFunctionPointer(pCCAPIGetFirmwareInfo, typeof(CCAPIGetFirmwareInfo_t));
                        CCAPIVshNotify = (CCAPIVshNotify_t)Marshal.GetDelegateForFunctionPointer(pCCAPIVshNotify, typeof(CCAPIVshNotify_t));
                        CCAPIGetNumberOfConsoles = (CCAPIGetNumberOfConsoles_t)Marshal.GetDelegateForFunctionPointer(pCCAPIGetNumberOfConsoles, typeof(CCAPIGetNumberOfConsoles_t));
                        CCAPIGetConsoleInfo = (CCAPIGetConsoleInfo_t)Marshal.GetDelegateForFunctionPointer(pCCAPIGetConsoleInfo, typeof(CCAPIGetConsoleInfo_t));
                        CCAPIGetDllVersion = (CCAPIGetDllVersion_t)Marshal.GetDelegateForFunctionPointer(pCCAPIGetDllVersion, typeof(CCAPIGetDllVersion_t));

                        return true;
                    }
                    else
                    {
                        MessageBox.Show(resources["ccapiNotInstalled"].ToString(), resources["ccapiNotFound"].ToString(), MessageBoxButton.OK, MessageBoxImage.Hand);
                    }
                }
                else
                {
                    MessageBox.Show(resources["invalidCCAPIFolder"].ToString(), resources["ccapiNotInstalledTitle"].ToString(), MessageBoxButton.OK, MessageBoxImage.Hand);
                }
            }
            else
            {
                MessageBox.Show(resources["ccapiNotInstalled"].ToString(), resources["ccapiNotInstalledTitle"].ToString(), MessageBoxButton.OK, MessageBoxImage.Hand);
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        ~CCAPI()
        {
            LibLoaded = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool GetLibraryState()
        {
            return LibLoaded;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsConnected()
        {
            int state = 0;
            int res = CCAPIGetConnectionStatus(ref state);
            return (res == OK) && (state != 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public bool ConnectTarget(string ip)
        {
            bool isConnected = CCAPIConnectConsole(ip) == OK;
            IPAddress = isConnected ? ip : "127.0.0.1";
            return isConnected;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool ConnectTarget()
        {
            CCAPIView ccapiView = new CCAPIView(this, this.resources);
            bool isConnected = ccapiView.Show();
            IPAddress = isConnected ? ccapiView.ViewModel.SelectedConsole.Ip : "127.0.0.1";
            return isConnected;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int DisconnectTarget()
        {
            return CCAPIDisconnectConsole();
        }

        /// <summary>
        /// 
        /// </summary>
        public void DetachProcess()
        {
            this.ProcessId = default;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetDllVersion()
        {
            return CCAPIGetDllVersion();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<ConsoleInfo> GetConsoleList()
        {
            List<ConsoleInfo> list = new List<ConsoleInfo>();

            IntPtr name = Malloc(512 * sizeof(char));
            IntPtr ip = Malloc(512 * sizeof(char));

            for (int i = 0; i < CCAPIGetNumberOfConsoles(); i++)
            {
                ConsoleInfo c = new ConsoleInfo();
                CCAPIGetConsoleInfo(i, name, ip);
                c.Name = Ptr2String(name);
                c.Ip = Ptr2String(ip);
                list.Add(c);
            }

            Free(name);
            Free(ip);

            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<ProcessInfo> GetProcessList()
        {
            List<ProcessInfo> list = new List<ProcessInfo>();

            IntPtr ProcessIds = Malloc(sizeof(uint) * 32);
            uint NProcessIds = 32;
            int ret = CCAPIGetProcessList(ref NProcessIds, ProcessIds);
            if (ret != OK)
            {
                Free(ProcessIds);
                return list;
            }
            else
            {
                IntPtr pName = Malloc(512 * sizeof(char));

                for (uint i = 0; i < NProcessIds; i++)
                {
                    uint pid = ReadFromBuffer<uint>(ProcessIds, i * sizeof(uint));

                    ret = CCAPIGetProcessName(pid, pName);
                    if (ret != OK)
                    {
                        Free(ProcessIds);
                        Free(pName);
                        return list;
                    }
                    else
                    {
                        ProcessInfo info = new ProcessInfo
                        {
                            pid = pid,
                            name = Ptr2String(pName)
                        };
                        list.Add(info);
                    }
                }
                Free(pName);
                Free(ProcessIds);
                return list;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public uint GetAttachedProcess()
        {
            return ProcessId;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ProcessId"></param>
        public void AttachProcess(uint ProcessId)
        {
            this.ProcessId = ProcessId;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool AttachProcess()
        {
            List<ProcessInfo> list = GetProcessList();
            for (int i = 0; i < list.Count; i++)
            {
                if (!list[i].name.Contains("dev_flash"))
                {
                    AttachProcess(list[i].pid);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Like Get memory but this function return directly the buffer from the offset (uint).
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public byte[] GetBytes(uint offset, uint length)
        {
            byte[] buffer = new byte[length];
            GetMemory(offset, buffer);
            return buffer;
        }

        /// <summary>
        /// Like Get memory but this function return directly the buffer from the offset (ulong).
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public byte[] GetBytes(ulong offset, uint length)
        {
            byte[] buffer = new byte[length];
            GetMemory(offset, buffer);
            return buffer;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="data"></param>
        public void GetMemory(ulong addr, byte[] data)
        {
            CCAPIGetMemory(ProcessId, (ulong)addr, (uint)data.Length, data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="data"></param>
        public void GetMemory(uint addr, byte[] data)
        {
            CCAPIGetMemory(ProcessId, (ulong)addr, (uint)data.Length, data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="size"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public int ReadMemory(ulong addr, uint size, byte[] data)
        {
            return CCAPIGetMemory(ProcessId, addr, size, data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="size"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public int ReadMemory(uint addr, uint size, byte[] data)
        {
            return CCAPIGetMemory(ProcessId, (ulong)addr, size, data);
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetMemory(ulong addr, byte[] data)
        {
            CCAPISetMemory(ProcessId, addr, (uint)data.Length, data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="data"></param>
        public void SetMemory(uint addr, byte[] data)
        {
            CCAPISetMemory(ProcessId, addr, (uint)data.Length, data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="size"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public int WriteMemory(ulong addr, uint size, byte[] data)
        {
            return CCAPISetMemory(ProcessId, addr, size, data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="size"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public int WriteMemory(uint addr, uint size, byte[] data)
        {
            return CCAPISetMemory(ProcessId, (ulong)addr, size, data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="rsx"></param>
        /// <returns></returns>
        public int GetTemperature(ref int cell, ref int rsx)
        {
            return CCAPIGetTemperature(ref cell, ref rsx);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public int Shutdown(ShutdownMode m)
        {
            return CCAPIShutdown((int)m);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public int RingBuzzer(BuzzerType t)
        {
            return CCAPIRingBuzzer((int)t);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        /// <param name="st"></param>
        /// <returns></returns>
        public int SetConsoleLed(ColorLed color, StatusLed st)
        {
            return CCAPISetConsoleLed((int)color, (int)st);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public int SetConsoleIds(ConsoleIdType t, string id)
        {
            if (id.Length != 32)
                return ERROR;
            return SetConsoleIds(t, StringToArray(id));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public int SetConsoleIds(ConsoleIdType t, byte[] id)
        {
            return CCAPISetConsoleIds((int)t, id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public int SetBootConsoleIds(ConsoleIdType t, string id)
        {
            if (id.Length != 32)
                return ERROR;
            return SetBootConsoleIds(t, StringToArray(id));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public int SetBootConsoleIds(ConsoleIdType t, byte[] id)
        {
            return CCAPISetBootConsoleIds((int)t, 1, id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public int ResetBootConsoleIds(ConsoleIdType t)
        {
            return CCAPISetBootConsoleIds((int)t, 0, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="icon"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public int VshNotify(NotifyIcon icon, string msg)
        {
            return CCAPIVshNotify((int)icon, msg);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="firmware"></param>
        /// <param name="ccapiVersion"></param>
        /// <param name="consoleType"></param>
        /// <returns></returns>
        public int GetFirmwareInfo(ref int firmware, ref int ccapiVersion, ref ConsoleType consoleType)
        {
            int cType = 0;
            int ret = CCAPIGetFirmwareInfo(ref firmware, ref ccapiVersion, ref cType);
            consoleType = (ConsoleType)cType;
            return ret;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="ret"></param>
        /// <returns></returns>
        public sbyte ReadMemoryI8(uint addr, out int ret)
        {
            byte[] data = new byte[sizeof(sbyte)];
            ret = ReadMemory(addr, sizeof(sbyte), data);
            return (sbyte)data[0];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="addr"></param>
        /// <returns></returns>
        public sbyte ReadMemoryI8(uint addr)
        {
            byte[] data = new byte[sizeof(sbyte)];
            ReadMemory(addr, sizeof(sbyte), data);
            return (sbyte)data[0];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="ret"></param>
        /// <returns></returns>
        public byte ReadMemoryU8(uint addr, out int ret)
        {
            byte[] data = new byte[sizeof(byte)];
            ret = ReadMemory(addr, sizeof(byte), data);
            return data[0];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="addr"></param>
        /// <returns></returns>
        public byte ReadMemoryU8(uint addr)
        {
            byte[] data = new byte[sizeof(byte)];
            ReadMemory(addr, sizeof(byte), data);
            return data[0];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="ret"></param>
        /// <returns></returns>
        public int ReadMemoryI32(uint addr, out int ret)
        {
            byte[] data = new byte[sizeof(int)];
            ret = ReadMemory(addr, sizeof(int), data);
            Array.Reverse(data);
            return BitConverter.ToInt32(data, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="addr"></param>
        /// <returns></returns>
        public int ReadMemoryI32(uint addr)
        {
            byte[] data = new byte[sizeof(int)];
            ReadMemory(addr, sizeof(int), data);
            Array.Reverse(data);
            return BitConverter.ToInt32(data, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="ret"></param>
        /// <returns></returns>
        public uint ReadMemoryU32(uint addr, out int ret)
        {
            byte[] data = new byte[sizeof(uint)];
            ret = ReadMemory(addr, sizeof(uint), data);
            Array.Reverse(data);
            return BitConverter.ToUInt32(data, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="addr"></param>
        /// <returns></returns>
        public uint ReadMemoryU32(uint addr)
        {
            byte[] data = new byte[sizeof(uint)];
            ReadMemory(addr, sizeof(uint), data);
            Array.Reverse(data);
            return BitConverter.ToUInt32(data, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="ret"></param>
        /// <returns></returns>
        public float ReadMemoryF32(uint addr, out int ret)
        {
            byte[] data = new byte[sizeof(float)];
            ret = ReadMemory(addr, sizeof(float), data);
            Array.Reverse(data);
            return BitConverter.ToSingle(data, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="addr"></param>
        /// <returns></returns>
        public float ReadMemoryF32(uint addr)
        {
            byte[] data = new byte[sizeof(float)];
            ReadMemory(addr, sizeof(float), data);
            Array.Reverse(data);
            return BitConverter.ToSingle(data, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="ret"></param>
        /// <returns></returns>
        public long ReadMemoryI64(uint addr, out int ret)
        {
            byte[] data = new byte[sizeof(long)];
            ret = ReadMemory(addr, sizeof(long), data);
            Array.Reverse(data);
            return BitConverter.ToInt64(data, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="addr"></param>
        /// <returns></returns>
        public long ReadMemoryI64(uint addr)
        {
            byte[] data = new byte[sizeof(long)];
            ReadMemory(addr, sizeof(long), data);
            Array.Reverse(data);
            return BitConverter.ToInt64(data, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="ret"></param>
        /// <returns></returns>
        public ulong ReadMemoryU64(uint addr, out int ret)
        {
            byte[] data = new byte[sizeof(ulong)];
            ret = ReadMemory(addr, sizeof(ulong), data);
            Array.Reverse(data);
            return BitConverter.ToUInt64(data, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="addr"></param>
        /// <returns></returns>
        public ulong ReadMemoryU64(uint addr)
        {
            byte[] data = new byte[sizeof(ulong)];
            ReadMemory(addr, sizeof(ulong), data);
            Array.Reverse(data);
            return BitConverter.ToUInt64(data, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="ret"></param>
        /// <returns></returns>
        public double ReadMemoryF64(uint addr, out int ret)
        {
            byte[] data = new byte[sizeof(double)];
            ret = ReadMemory(addr, sizeof(double), data);
            Array.Reverse(data);
            return BitConverter.ToDouble(data, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="addr"></param>
        /// <returns></returns>
        public double ReadMemoryF64(uint addr)
        {
            byte[] data = new byte[sizeof(double)];
            ReadMemory(addr, sizeof(double), data);
            Array.Reverse(data);
            return BitConverter.ToDouble(data, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="addr"></param>
        /// <returns></returns>
        public string ReadMemoryString(uint addr)
        {
            string s = "";

            while (true)
            {
                byte[] chunk = new byte[0x100];
                int r = ReadMemory(addr, (uint)chunk.Length, chunk);
                if (r != OK)
                {
                    break;
                }
                else
                {
                    for (int i = 0; i < chunk.Length; i++)
                    {
                        if (chunk[i] == 0)
                        {
                            s += Encoding.ASCII.GetString(chunk, 0, i);
                            goto end;
                        }
                    }

                    addr += (uint)chunk.Length;
                    s += Encoding.ASCII.GetString(chunk);
                }
            }

        end:

            return s;
        }

        //write
        /// <summary>
        /// 
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        public int WriteMemoryI8(uint addr, sbyte i)
        {
            byte[] data = new byte[sizeof(sbyte)];
            data[0] = (byte)i;
            return WriteMemory(addr, sizeof(sbyte), data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        public int WriteMemoryU8(uint addr, byte i)
        {
            byte[] data = new byte[sizeof(byte)];
            data[0] = i;
            return WriteMemory(addr, sizeof(byte), data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        public int WriteMemoryI16(uint addr, short i)
        {
            byte[] data = BitConverter.GetBytes(i);
            Array.Reverse(data);
            return WriteMemory(addr, sizeof(short), data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        public int WriteMemoryU16(uint addr, ushort i)
        {
            byte[] data = BitConverter.GetBytes(i);
            Array.Reverse(data);
            return WriteMemory(addr, sizeof(ushort), data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        public int WriteMemoryI32(uint addr, int i)
        {
            byte[] data = BitConverter.GetBytes(i);
            Array.Reverse(data);
            return WriteMemory(addr, sizeof(int), data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        public int WriteMemoryU32(uint addr, uint i)
        {
            byte[] data = BitConverter.GetBytes(i);
            Array.Reverse(data);
            return WriteMemory(addr, sizeof(uint), data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        public int WriteMemoryF32(uint addr, float f)
        {
            byte[] data = BitConverter.GetBytes(f);
            Array.Reverse(data);
            return WriteMemory(addr, sizeof(float), data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        public int WriteMemoryI64(uint addr, long i)
        {
            byte[] data = BitConverter.GetBytes(i);
            Array.Reverse(data);
            return WriteMemory(addr, sizeof(long), data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        public int WriteMemoryU64(uint addr, ulong i)
        {
            byte[] data = BitConverter.GetBytes(i);
            Array.Reverse(data);
            return WriteMemory(addr, sizeof(long), data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        public int WriteMemoryF64(uint addr, double d)
        {
            byte[] data = BitConverter.GetBytes(d);
            Array.Reverse(data);
            return WriteMemory(addr, sizeof(double), data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public int WriteMemoryString(uint addr, string s)
        {
            byte[] b = Encoding.ASCII.GetBytes(s + "\0");
            return WriteMemory(addr, (uint)b.Length, b);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="firmware"></param>
        /// <returns></returns>
        public static string FirmwareToString(int firmware)
        {
            int l = (firmware >> 12) & 0xFF;
            int h = firmware >> 24;

            return String.Format("{0:X}.{1:X}", h, l);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cType"></param>
        /// <returns></returns>
        public static string ConsoleTypeToString(ConsoleType cType)
        {
            string s = "UNK";

            switch (cType)
            {
                case ConsoleType.CEX:
                    s = "CEX";
                    break;

                case ConsoleType.DEX:
                    s = "DEX";
                    break;

                case ConsoleType.TOOL:
                    s = "TOOL";
                    break;

                default:
                    break;
            }

            return s;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static byte[] StringToArray(string s)
        {
            if (s.Length == 0)
            {
                return null;
            }
            if ((s.Length % 2) != 0)
            {
                s += "0";
            }
            byte[] b = new byte[s.Length / 2];
            int j = 0;
            for (int i = 0; i < s.Length; i += 2)
            {
                var sb = s.Substring(i, 2);
                b[j++] = Convert.ToByte(sb, 16);
            }

            return b;
        }

        /// <summary>
        /// 
        /// </summary>
        public const int OK = 0;
        /// <summary>
        /// 
        /// </summary>
        public const int ERROR = -1;

        /// <summary>
        /// 
        /// </summary>
        protected uint ProcessId;
        private IntPtr LibHandle;
        private bool LibLoaded;

        private IntPtr Malloc(int n)
        {
            return Marshal.AllocHGlobal(n);
        }

        private void Free(IntPtr p)
        {
            Marshal.FreeHGlobal(p);
        }

        private string Ptr2String(IntPtr p)
        {
            return Marshal.PtrToStringAnsi(p);
        }

        private T ReadFromBuffer<T>(IntPtr ptr, uint off)
        {
            return (T)Marshal.PtrToStructure(new IntPtr(ptr.ToInt64() + off), typeof(T));
        }

        [DllImport("kernel32.dll")]
        static extern uint GetLastError();
        [DllImport("kernel32.dll")]
        static extern IntPtr LoadLibrary(string a);
        [DllImport("kernel32.dll")]
        static extern IntPtr GetProcAddress(IntPtr a, string b);
        [DllImport("kernel32.dll")]
        static extern bool FreeLibrary(IntPtr a);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int CCAPIConnectConsole_t(string ip);
        private CCAPIConnectConsole_t CCAPIConnectConsole;
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int CCAPIDisconnectConsole_t();
        private CCAPIDisconnectConsole_t CCAPIDisconnectConsole;
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int CCAPIGetConnectionStatus_t(ref int status);
        private CCAPIGetConnectionStatus_t CCAPIGetConnectionStatus;
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int CCAPISetBootConsoleIds_t(int idType, int on, byte[] id);
        private CCAPISetBootConsoleIds_t CCAPISetBootConsoleIds;
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int CCAPISetConsoleIds_t(int idType, byte[] id);
        private CCAPISetConsoleIds_t CCAPISetConsoleIds;
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int CCAPISetMemory_t(uint pid, ulong addr, uint size, byte[] data);
        private CCAPISetMemory_t CCAPISetMemory;
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int CCAPIGetMemory_t(uint pid, ulong addr, uint size, byte[] data);
        private CCAPIGetMemory_t CCAPIGetMemory;
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int CCAPIGetProcessList_t(ref uint npid, IntPtr pids);
        private CCAPIGetProcessList_t CCAPIGetProcessList;
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int CCAPIGetProcessName_t(uint pid, IntPtr name);
        private CCAPIGetProcessName_t CCAPIGetProcessName;
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int CCAPIGetTemperature_t(ref int cell, ref int rsx);
        private CCAPIGetTemperature_t CCAPIGetTemperature;
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int CCAPIShutdown_t(int mode);
        private CCAPIShutdown_t CCAPIShutdown;
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int CCAPIRingBuzzer_t(int type);
        private CCAPIRingBuzzer_t CCAPIRingBuzzer;
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int CCAPISetConsoleLed_t(int color, int status);
        private CCAPISetConsoleLed_t CCAPISetConsoleLed;
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int CCAPIGetFirmwareInfo_t(ref int firmware, ref int ccapiVersion, ref int consoleType);
        private CCAPIGetFirmwareInfo_t CCAPIGetFirmwareInfo;
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int CCAPIVshNotify_t(int mode, string msg);
        private CCAPIVshNotify_t CCAPIVshNotify;
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int CCAPIGetNumberOfConsoles_t();
        private CCAPIGetNumberOfConsoles_t CCAPIGetNumberOfConsoles;
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int CCAPIGetConsoleInfo_t(int index, IntPtr name, IntPtr ip);
        private CCAPIGetConsoleInfo_t CCAPIGetConsoleInfo;
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int CCAPIGetDllVersion_t();
        private CCAPIGetDllVersion_t CCAPIGetDllVersion;

        internal ResourceDictionary resources = new ResourceDictionary();
    }
}

