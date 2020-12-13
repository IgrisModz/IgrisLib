using IgrisLib.MessageBox;
using IgrisLib.NET;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Windows;

namespace IgrisLib
{
    public class TMAPI : IApi
    {
        public enum GPRegisters
        {
            SNPS3_gpr_0 = 0x00,
            SNPS3_gpr_1 = 0x01,
            SNPS3_gpr_2 = 0x02,
            SNPS3_gpr_3 = 0x03,
            SNPS3_gpr_4 = 0x04,
            SNPS3_gpr_5 = 0x05,
            SNPS3_gpr_6 = 0x06,
            SNPS3_gpr_7 = 0x07,
            SNPS3_gpr_8 = 0x08,
            SNPS3_gpr_9 = 0x09,
            SNPS3_gpr_10 = 0x0a,
            SNPS3_gpr_11 = 0x0b,
            SNPS3_gpr_12 = 0x0c,
            SNPS3_gpr_13 = 0x0d,
            SNPS3_gpr_14 = 0x0e,
            SNPS3_gpr_15 = 0x0f,
            SNPS3_gpr_16 = 0x10,
            SNPS3_gpr_17 = 0x11,
            SNPS3_gpr_18 = 0x12,
            SNPS3_gpr_19 = 0x13,
            SNPS3_gpr_20 = 0x14,
            SNPS3_gpr_21 = 0x15,
            SNPS3_gpr_22 = 0x16,
            SNPS3_gpr_23 = 0x17,
            SNPS3_gpr_24 = 0x18,
            SNPS3_gpr_25 = 0x19,
            SNPS3_gpr_26 = 0x1a,
            SNPS3_gpr_27 = 0x1b,
            SNPS3_gpr_28 = 0x1c,
            SNPS3_gpr_29 = 0x1d,
            SNPS3_gpr_30 = 0x1e,
            SNPS3_gpr_31 = 0x1f
        }

        public enum FPRegisters
        {
            SNPS3_fpr_0 = 0x20,
            SNPS3_fpr_1 = 0x21,
            SNPS3_fpr_2 = 0x22,
            SNPS3_fpr_3 = 0x23,
            SNPS3_fpr_4 = 0x24,
            SNPS3_fpr_5 = 0x25,
            SNPS3_fpr_6 = 0x26,
            SNPS3_fpr_7 = 0x27,
            SNPS3_fpr_8 = 0x28,
            SNPS3_fpr_9 = 0x29,
            SNPS3_fpr_10 = 0x2A,
            SNPS3_fpr_11 = 0x2B,
            SNPS3_fpr_12 = 0x2C,
            SNPS3_fpr_13 = 0x2D,
            SNPS3_fpr_14 = 0x2E,
            SNPS3_fpr_15 = 0x2F,
            SNPS3_fpr_16 = 0x30,
            SNPS3_fpr_17 = 0x31,
            SNPS3_fpr_18 = 0x32,
            SNPS3_fpr_19 = 0x33,
            SNPS3_fpr_20 = 0x34,
            SNPS3_fpr_21 = 0x35,
            SNPS3_fpr_22 = 0x36,
            SNPS3_fpr_23 = 0x37,
            SNPS3_fpr_24 = 0x38,
            SNPS3_fpr_25 = 0x39,
            SNPS3_fpr_26 = 0x3A,
            SNPS3_fpr_27 = 0x3B,
            SNPS3_fpr_28 = 0x3C,
            SNPS3_fpr_29 = 0x3D,
            SNPS3_fpr_30 = 0x3E,
            SNPS3_fpr_31 = 0x3F,
        }

        public enum SPRegisters
        {
            SNPS3_pc = 0x40,
            SNPS3_cr = 0x41,
            SNPS3_lr = 0x42,
            SNPS3_ctr = 0x43,
            SNPS3_xer = 0x44,
            SNPS3_fpscr = 0x45,
            SNPS3_vscr = 0x46,
            SNPS3_vrsave = 0x47,
            SNPS3_msr = 0x48,
        }
        public string Name => "Target Manager";
        public static int Target = 0xFF;
        public bool IsConnected = false;
        public static bool AssemblyLoaded = true;
        public static PS3TMAPI.ResetParameter resetParameter;

        public TMAPI()
        {

        }

        public Extension Extension
        {
            get { return new Extension(this); }
        }

        public class SCECMD
        {
            /// <summary>Get the target status and return the string value.</summary>
            public string SNRESULT()
            {
                return Parameters.snresult;
            }

            /// <summary>Get the target name.</summary>
            public string GetTargetName()
            {
                if (Parameters.ConsoleName == null || Parameters.ConsoleName == string.Empty)
                {
                    PS3TMAPI.InitTargetComms();
                    PS3TMAPI.TargetInfo TargetInfo = new PS3TMAPI.TargetInfo
                    {
                        Flags = PS3TMAPI.TargetInfoFlag.TargetID,
                        Target = Target
                    };
                    PS3TMAPI.GetTargetInfo(ref TargetInfo);
                    Parameters.ConsoleName = TargetInfo.Name;
                }
                return Parameters.ConsoleName;
            }

            /// <summary>Get the target status and return the string value.</summary>
            public string GetStatus()
            {
                if (TMAPI.AssemblyLoaded)
                    return "NotConnected";
                Parameters.connectStatus = new PS3TMAPI.ConnectStatus();
                PS3TMAPI.GetConnectStatus(Target, out Parameters.connectStatus, out Parameters.usage);
                Parameters.Status = Parameters.connectStatus.ToString();
                return Parameters.Status;
            }

            /// <summary>Get the ProcessID by the current process.</summary>
            public uint ProcessID()
            {
                return Parameters.ProcessID;
            }

            /// <summary>Get an array of processID's.</summary>
            public uint[] ProcessIDs()
            {
                return Parameters.processIDs;
            }

            /// <summary>Get some details from your target.</summary>
            public PS3TMAPI.ConnectStatus DetailStatus()
            {
                return Parameters.connectStatus;
            }
        }

        public SCECMD SCE
        {
            get { return new SCECMD(); }
        }

        public class Parameters
        {
            public static string
                usage,
                info,
                snresult,
                Status,
                MemStatus,
                ConsoleName;
            public static uint ProcessID;
            public static uint[] processIDs;
            public static byte[] Retour;
            public static uint[] ModuleIDs;
            public static PS3TMAPI.ConnectStatus connectStatus;
            public static PS3TMAPI.PPUThreadInfo threadInfo;
        }

        /// <summary>Enum of flag reset.</summary>
        public enum ResetTarget
        {
            Hard,
            Quick,
            ResetEx,
            Soft
        }

        public void InitComms()
        {
            PS3TMAPI.InitTargetComms();
        }

        /// <summary>Connect the default target and initialize the dll. Possible to put an int as arugment for determine which target to connect.</summary>
        public bool ConnectTarget()
        {
            return ConnectTarget(0);
        }

        /// <summary>Connect the default target and initialize the dll. Possible to put an int as arugment for determine which target to connect.</summary>
        public bool ConnectTarget(int TargetIndex = 0)
        {
            if (AssemblyLoaded)
                PS3TMAPI_NET();
            AssemblyLoaded = false;
            Target = TargetIndex;
            PS3TMAPI.SUCCEEDED(PS3TMAPI.InitTargetComms());
            bool result = PS3TMAPI.SUCCEEDED(PS3TMAPI.Connect(TargetIndex, null));
            IsConnected = result;
            return result;
        }

        /// <summary>Connect the target by is name.</summary>
        public bool ConnectTarget(string TargetName)
        {
            if (AssemblyLoaded)
                PS3TMAPI_NET();
            AssemblyLoaded = false;
            bool result = PS3TMAPI.SUCCEEDED(PS3TMAPI.InitTargetComms());
            if (result)
            {
                PS3TMAPI.SUCCEEDED(PS3TMAPI.GetTargetFromName(TargetName, out Target));
                result = PS3TMAPI.SUCCEEDED(PS3TMAPI.Connect(Target, null));
            }
            IsConnected = result;
            return result;
        }

        /// <summary>Disconnect the target.</summary>
        public int DisconnectTarget()
        {
            PS3TMAPI.SNRESULT sNRESULT = PS3TMAPI.Disconnect(Target);
            IsConnected = false;
            return (int)sNRESULT;
        }

        /// <summary>Detach the process.</summary>
        public void DetachProcess()
        {
            PS3TMAPI.ProcessStop(Target, Parameters.ProcessID);
            IsConnected = false;
        }

        /// <summary>Power on selected target.</summary>
        public void PowerOn(int numTarget = 0)
        {
            if (Target != 0xFF)
                numTarget = Target;
            PS3TMAPI.PowerOn(numTarget);
        }

        /// <summary>Power off selected target.</summary>
        public void PowerOff(bool Force)
        {
            PS3TMAPI.PowerOff(Target, Force);
        }

        public bool AttachProcess()
        {
            try
            {
                PS3TMAPI.GetProcessList(Target, out Parameters.processIDs);

                if (Parameters.processIDs.Length > 0)
                {
                    ulong uProcess = Parameters.processIDs[0];
                    Parameters.ProcessID = Convert.ToUInt32(uProcess);

                    PS3TMAPI.GetModuleList(Target, Parameters.ProcessID, out Parameters.ModuleIDs);
                    PS3TMAPI.ProcessAttach(Target, PS3TMAPI.UnitType.PPU, Parameters.ProcessID);
                    PS3TMAPI.ProcessContinue(Target, Parameters.ProcessID);

                    GetThreadInfo();

                    Parameters.info = $"The Process 0x{Parameters.ProcessID:X8} Has Been Attached!";
                    return true;
                }
            }
            catch { }
            return false;
        }

        /// <summary>Set memory to the target (byte[]).</summary>
        public void SetMemory(uint Address, byte[] Bytes)
        {
            PS3TMAPI.ProcessSetMemory(Target, PS3TMAPI.UnitType.PPU, Parameters.ProcessID, 0, Address, Bytes);
        }

        /// <summary>Set memory to the address (byte[]).</summary>
        public void SetMemory(uint Address, ulong value)
        {
            byte[] b = BitConverter.GetBytes(value);
            Array.Reverse(b);
            PS3TMAPI.ProcessSetMemory(Target, PS3TMAPI.UnitType.PPU, Parameters.ProcessID, 0, Address, b);
        }

        /// <summary>Set memory with value as string hexadecimal to the address (string).</summary>
        public void SetMemory(uint Address, string hexadecimal, EndianType Type = EndianType.BigEndian)
        {
            byte[] Entry = StringToByteArray(hexadecimal);
            if (Type == EndianType.LittleEndian)
                Array.Reverse(Entry);
            PS3TMAPI.ProcessSetMemory(Target, PS3TMAPI.UnitType.PPU, Parameters.ProcessID, 0, Address, Entry);
        }

        /// <summary>Get memory from the address.</summary>
        public void GetMemory(uint Address, byte[] Bytes)
        {
            PS3TMAPI.ProcessGetMemory(Target, PS3TMAPI.UnitType.PPU, Parameters.ProcessID, 0, Address, ref Bytes);
        }

        /// <summary>Get a bytes array with the length input.</summary>
        public byte[] GetBytes(uint Address, uint lengthByte)
        {
            byte[] Longueur = new byte[lengthByte];
            PS3TMAPI.ProcessGetMemory(Target, PS3TMAPI.UnitType.PPU, Parameters.ProcessID, 0, Address, ref Longueur);
            return Longueur;
        }

        /// <summary>Get a string with the length input.</summary>
        public string GetString(uint Address, uint lengthString)
        {
            byte[] Longueur = new byte[lengthString];
            PS3TMAPI.ProcessGetMemory(Target, PS3TMAPI.UnitType.PPU, Parameters.ProcessID, 0, Address, ref Longueur);
            string StringResult = Hex2Ascii(ReplaceString(Longueur));
            return StringResult;
        }

        internal static string Hex2Ascii(string iMCSxString)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i <= (iMCSxString.Length - 2); i += 2)
            {
                builder.Append(Convert.ToString(Convert.ToChar(int.Parse(iMCSxString.Substring(i, 2), NumberStyles.HexNumber))));
            }
            return builder.ToString();
        }

        internal static byte[] StringToByteArray(string hex)
        {
            string replace = hex.Replace("0x", "");
            string Stringz = replace.Insert(replace.Length - 1, "0");

            int Odd = replace.Length;
            bool Nombre = false;
            if (Odd % 2 == 0) Nombre = true;

            try
            {
                return Enumerable.Range(0, replace.Length)
                 .Where(x => x % 2 == 0)
                 .Select(x => Convert.ToByte(Nombre ? replace.Substring(x, 2) : Stringz.Substring(x, 2), 16))
                 .ToArray();
            }
            catch { throw new ArgumentException("Value not possible.", "Byte Array"); }
        }

        internal static string ReplaceString(byte[] bytes)
        {
            string PSNString = BitConverter.ToString(bytes);
            PSNString = PSNString.Replace("00", string.Empty);
            PSNString = PSNString.Replace("-", string.Empty);
            for (int i = 0; i < 10; i++)
                PSNString = PSNString.Replace("^" + i.ToString(), string.Empty);
            return PSNString;
        }

        /// <summary>Reset target to XMB , Sometimes the target restart quickly.</summary>
        public void ResetToXMB(ResetTarget flag)
        {
            if (flag == ResetTarget.Hard)
                resetParameter = PS3TMAPI.ResetParameter.Hard;
            else if (flag == ResetTarget.Quick)
                resetParameter = PS3TMAPI.ResetParameter.Quick;
            else if (flag == ResetTarget.ResetEx)
                resetParameter = PS3TMAPI.ResetParameter.ResetEx;
            else if (flag == ResetTarget.Soft)
                resetParameter = PS3TMAPI.ResetParameter.Soft;
            PS3TMAPI.Reset(Target, resetParameter);
        }

        internal static Assembly LoadApi;
        ///<summary>Load the PS3 API for use with your Application .NET.</summary>
        public Assembly PS3TMAPI_NET()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (s, e) =>
            {
                var filename = new AssemblyName(e.Name).Name;
                var x = string.Format(@"C:\Program Files\SN Systems\PS3\bin\ps3tmapi_net.dll", filename);
                var x64 = string.Format(@"C:\Program Files (x64)\SN Systems\PS3\bin\ps3tmapi_net.dll", filename);
                var x86 = string.Format(@"C:\Program Files (x86)\SN Systems\PS3\bin\ps3tmapi_net.dll", filename);
                if (File.Exists(x))
                    LoadApi = Assembly.LoadFile(x);
                else
                {
                    if (File.Exists(x64))
                        LoadApi = Assembly.LoadFile(x64);

                    else
                    {
                        if (File.Exists(x86))
                            LoadApi = Assembly.LoadFile(x86);
                        else
                        {
                            IgrisMessageBox.Show($"Target Manager API cannot be founded to:\r\n\r\n{x86}", "Error with PS3 API!", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                return LoadApi;
            };
            return LoadApi;
        }

        public static void GetThreadInfo()
        {
            PS3TMAPI.GetProcessInfo(Target, Parameters.ProcessID, out PS3TMAPI.ProcessInfo processInfo);

            if (processInfo.ThreadIDs.Length <= 0)
                return;

            for (int i = 0; i < processInfo.ThreadIDs.Length; i++)
            {
                PS3TMAPI.GetPPUThreadInfo(Target, Parameters.ProcessID, processInfo.ThreadIDs[i], out Parameters.threadInfo);

                if (Parameters.threadInfo.ThreadName == null)
                    continue;
                if (Parameters.threadInfo.ThreadName.Contains("EBOOT"))
                    break;
            }
        }

        public bool GetThreadByName(string name, ref PS3TMAPI.PPUThreadInfo LocalthreadInfo)
        {
            PS3TMAPI.GetProcessInfo(Target, Parameters.ProcessID, out PS3TMAPI.ProcessInfo processInfo);

            if (processInfo.ThreadIDs.Length <= 0)
                return false;

            for (int i = 0; i < processInfo.ThreadIDs.Length; i++)
            {
                PS3TMAPI.GetPPUThreadInfo(Target, Parameters.ProcessID, processInfo.ThreadIDs[i], out LocalthreadInfo);

                if (LocalthreadInfo.ThreadName == null)
                    continue;
                if (LocalthreadInfo.ThreadName.Contains(name))
                    return true;
            }
            return false;
        }

        public void Shutdown(bool Force)
        {
            PS3TMAPI.PowerOff(Target, Force);
        }

        public void ResetToXMB()
        {
            PS3TMAPI.Reset(Target, PS3TMAPI.ResetParameter.Hard);
        }

        public void MainThreadStop()
        {
            PS3TMAPI.ThreadStop(Target, PS3TMAPI.UnitType.PPU, Parameters.ProcessID, Parameters.threadInfo.ThreadID);
        }

        public void MainThreadContinue()
        {
            PS3TMAPI.ThreadContinue(Target, PS3TMAPI.UnitType.PPU, Parameters.ProcessID, Parameters.threadInfo.ThreadID);
        }

        public void StopThreadyID(ulong ID)
        {
            PS3TMAPI.ThreadStop(Target, PS3TMAPI.UnitType.PPU, Parameters.ProcessID, ID);
        }

        public void ContinueThreadByID(ulong ID)
        {
            PS3TMAPI.ThreadContinue(Target, PS3TMAPI.UnitType.PPU, Parameters.ProcessID, ID);
        }

        public static ulong ULongReverse(ulong value)
        {
            return (value & 0x00000000000000FFUL) << 56 | (value & 0x000000000000FF00UL) << 40 |
                   (value & 0x0000000000FF0000UL) << 24 | (value & 0x00000000FF000000UL) << 8 |
                   (value & 0x000000FF00000000UL) >> 8 | (value & 0x0000FF0000000000UL) >> 24 |
                   (value & 0x00FF000000000000UL) >> 40 | (value & 0xFF00000000000000UL) >> 56;
        }

        public ulong GetSingleRegister(uint Register)
        {
            uint[] Registers = new uint[1];

            Registers[0] = Register;
            PS3TMAPI.ThreadGetRegisters(Target, PS3TMAPI.UnitType.PPU, Parameters.ProcessID, Parameters.threadInfo.ThreadID, Registers, out ulong[] Return);

            return ULongReverse(Return[0]);
        }

        public void SetSingleRegister(uint Register, ulong Value)
        {
            ulong[] Return = new ulong[1];
            uint[] Registers = new uint[1];

            Registers[0] = Register;
            Return[0] = ULongReverse(Value);
            _ = PS3TMAPI.ThreadSetRegisters(Target, PS3TMAPI.UnitType.PPU, Parameters.ProcessID, Parameters.threadInfo.ThreadID, Registers, Return);
        }

        public ulong GetSingleRegisterByThreadID(ulong ID, uint Register)
        {
            uint[] Registers = new uint[1];

            Registers[0] = Register;

            PS3TMAPI.ThreadGetRegisters(Target, PS3TMAPI.UnitType.PPU, Parameters.ProcessID, ID, Registers, out ulong[] Return);
            return ULongReverse(Return[0]);
        }

        public void SetSingleRegisterByThreadID(ulong ID, uint Register, ulong Value)
        {
            ulong[] Return = new ulong[1];
            uint[] Registers = new uint[1];

            Registers[0] = Register;
            Return[0] = ULongReverse(Value);
            _ = PS3TMAPI.ThreadSetRegisters(Target, PS3TMAPI.UnitType.PPU, Parameters.ProcessID, ID, Registers, Return);
        }

        public string GetTargetName()
        {
            if (Parameters.ConsoleName == null || Parameters.ConsoleName == string.Empty)
            {
                PS3TMAPI.InitTargetComms();
                PS3TMAPI.TargetInfo TargetInfo = new PS3TMAPI.TargetInfo
                {
                    Flags = PS3TMAPI.TargetInfoFlag.TargetID,
                    Target = Target
                };
                PS3TMAPI.GetTargetInfo(ref TargetInfo);
                Parameters.ConsoleName = TargetInfo.Name;
            }
            return Parameters.ConsoleName;
        }

        public string GetStatus()
        {
            Parameters.connectStatus = new PS3TMAPI.ConnectStatus();
            PS3TMAPI.GetConnectStatus(Target, out Parameters.connectStatus, out Parameters.usage);
            Parameters.Status = Parameters.connectStatus.ToString();
            return Parameters.Status;
        }

        public uint ProcessID()
        {
            PS3TMAPI.GetProcessList(Target, out Parameters.processIDs);
            Parameters.ProcessID = Parameters.processIDs[0];
            return Parameters.ProcessID;
        }

        public uint[] ProcessIDs()
        {
            PS3TMAPI.GetProcessList(Target, out Parameters.processIDs);
            return Parameters.processIDs;
        }

        public string GetProcessName(uint processId)
        {
            if (processId != 0)
            {
                PS3TMAPI.GetProcessInfo(Target, processId, out PS3TMAPI.ProcessInfo processInfo);
                return processInfo.Hdr.ToString();
            }
            return "";
        }

        public uint GetProcessParent(uint processId)
        {
            if (processId != 0)
            {
                PS3TMAPI.GetProcessInfo(Target, processId, out PS3TMAPI.ProcessInfo processInfo);
                return processInfo.Hdr.ParentProcessID;
            }
            return 0x0;
        }

        public ulong GetProcessSize(uint processId)
        {
            if (processId != 0)
            {
                PS3TMAPI.GetProcessInfo(Target, processId, out PS3TMAPI.ProcessInfo processInfo);
                return processInfo.Hdr.MaxMemorySize;
            }
            return 0x0;
        }

        public uint[] ModuleIds()
        {
            PS3TMAPI.GetModuleList(Target, Parameters.ProcessID, out Parameters.ModuleIDs);

            return Parameters.ModuleIDs;
        }

        public string GetModuleName(uint moduleId)
        {
            if (Parameters.processIDs.Length > 0)
            {
                PS3TMAPI.GetModuleInfo(Target, Parameters.ProcessID, moduleId, out PS3TMAPI.ModuleInfo moduleInfo);
                return moduleInfo.Hdr.Name;
            }
            return "";
        }

        public uint GetModuleStartAddress(uint moduleId)
        {
            if (Parameters.processIDs.Length > 0)
            {
                PS3TMAPI.GetModuleInfo(Target, Parameters.ProcessID, moduleId, out PS3TMAPI.ModuleInfo moduleInfo);
                return moduleInfo.Hdr.StartEntry;
            }
            return 0x0;
        }

        public uint GetModuleStopAddress(uint moduleId)
        {
            if (Parameters.processIDs.Length > 0)
            {
                PS3TMAPI.GetModuleInfo(Target, Parameters.ProcessID, moduleId, out PS3TMAPI.ModuleInfo moduleInfo);
                return moduleInfo.Hdr.StopEntry;
            }
            return 0x0;
        }

        public ulong GetModuleSize(uint moduleId)
        {
            if (Parameters.processIDs.Length > 0)
            {
                PS3TMAPI.GetModuleInfo(Target, Parameters.ProcessID, moduleId, out PS3TMAPI.ModuleInfo moduleInfo);
                ulong memSize = 0x0;
                for (int i = 0; i < moduleInfo.Segments.Length; i++)
                    memSize += moduleInfo.Segments[i].MemSize;
                return memSize;
            }
            return 0x0;
        }

        public string GetCurrentGame()
        {
            PS3TMAPI.ProcessInfo processInfo = new PS3TMAPI.ProcessInfo();
            PS3TMAPI.GetProcessInfo(0, ProcessID(), out processInfo);
            string GameCode = processInfo.Hdr.ELFPath.Split('/')[3];

            try
            {
                WebClient client = new WebClient();
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                string content = client.DownloadString(string.Format("https://a0.ww.np.dl.playstation.net/tpl/np/{0}/{1}-ver.xml", GameCode, GameCode)).Replace("<TITLE>", ";");
                return content.Split(';')[1].Replace("</TITLE>", ";").Split(';')[0].Replace("Â", "");
            }
            catch
            {
                return "Unknown Game";
            }
        }
    }
}
