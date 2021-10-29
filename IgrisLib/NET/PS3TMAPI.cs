using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace IgrisLib.NET
{
    /// <summary>
    /// 
    /// </summary>
    public class PS3TMAPI
    {
        private static readonly EnumerateTargetsExCallbackPriv ms_enumTargetsExCallbackPriv = new EnumerateTargetsExCallbackPriv(EnumTargetsExPriv);
        [ThreadStatic]
        private static EnumerateTargetsExCallback ms_enumTargetsExCallback = null;
        [ThreadStatic]
        private static object ms_enumTargetsExUserData = null;
        [ThreadStatic]
        private static ServerEventCallback ms_serverEventCallback = null;
        [ThreadStatic]
        private static object ms_serverEventUserData = null;
        [ThreadStatic]
        private static Dictionary<TTYChannel, TTYCallbackAndUserData> ms_userTtyCallbacks = null;
        [ThreadStatic]
        private static Dictionary<int, PadPlaybackCallbackAndUserData> ms_userPadPlaybackCallbacks = null;
        [ThreadStatic]
        private static Dictionary<int, PadCaptureCallbackAndUserData> ms_userPadCaptureCallbacks = null;
        private static readonly CustomProtocolCallbackPriv ms_customProtoCallbackPriv = new CustomProtocolCallbackPriv(CustomProtocolHandler);
        [ThreadStatic]
        private static Dictionary<CustomProtocolId, CusProtoCallbackAndUserData> ms_userCustomProtoCallbacks = null;
        [ThreadStatic]
        private static Dictionary<int, FtpCallbackAndUserData> ms_userFtpCallbacks = null;
        [ThreadStatic]
        private static Dictionary<int, FileTraceCallbackAndUserData> ms_userFileTraceCallbacks = null;
        [ThreadStatic]
        private static Dictionary<int, TargetCallbackAndUserData> ms_userTargetCallbacks = null;
        private static readonly HandleEventCallbackPriv ms_eventHandlerWrapper = new HandleEventCallbackPriv(EventHandlerWrapper);
        /// <summary>
        /// 
        /// </summary>
        public const int InvalidTarget = -1;
        /// <summary>
        /// 
        /// </summary>
        public const int DefaultTarget = -2;
        /// <summary>
        /// 
        /// </summary>
        public const uint AllTTYStreams = 4294967295;
        /// <summary>
        /// 
        /// </summary>
        public const uint DefaultProcessPriority = 999;
        /// <summary>
        /// 
        /// </summary>
        public const uint DefaultProtocolPriority = 128;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="res"></param>
        /// <returns></returns>
        public static bool FAILED(SNRESULT res)
        {
            return !SUCCEEDED(res);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="res"></param>
        /// <returns></returns>
        public static bool SUCCEEDED(SNRESULT res)
        {
            return res >= SNRESULT.SN_S_OK;
        }

        private static bool Is32Bit()
        {
            return IntPtr.Size == 4;
        }

        private static byte VersionMajor(ulong version)
        {
            return (byte)(version >> 16);
        }

        private static byte VersionMinor(ulong version)
        {
            return (byte)(version >> 8);
        }

        private static byte VersionFix(ulong version)
        {
            return (byte)version;
        }

        private static void VersionComponents(ulong version, out byte major, out byte minor, out byte fix)
        {
            major = VersionMajor(version);
            minor = VersionMinor(version);
            fix = VersionFix(version);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sdkVersion"></param>
        /// <returns></returns>
        public static byte SDKVersionMajor(ulong sdkVersion)
        {
            return VersionMajor(sdkVersion);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sdkVersion"></param>
        /// <returns></returns>
        public static byte SDKVersionMinor(ulong sdkVersion)
        {
            return VersionMinor(sdkVersion);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sdkVersion"></param>
        /// <returns></returns>
        public static byte SDKVersionFix(ulong sdkVersion)
        {
            return VersionFix(sdkVersion);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sdkVersion"></param>
        /// <param name="major"></param>
        /// <param name="minor"></param>
        /// <param name="fix"></param>
        public static void SDKVersionComponents(ulong sdkVersion, out byte major, out byte minor, out byte fix)
        {
            major = SDKVersionMajor(sdkVersion);
            minor = SDKVersionMinor(sdkVersion);
            fix = SDKVersionFix(sdkVersion);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cpVersion"></param>
        /// <returns></returns>
        public static byte CPVersionMajor(ulong cpVersion)
        {
            return VersionMajor(cpVersion);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cpVersion"></param>
        /// <returns></returns>
        public static byte CPVersionMinor(ulong cpVersion)
        {
            return VersionMinor(cpVersion);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cpVersion"></param>
        /// <returns></returns>
        public static byte CPVersionFix(ulong cpVersion)
        {
            return VersionFix(cpVersion);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cpVersion"></param>
        /// <param name="major"></param>
        /// <param name="minor"></param>
        /// <param name="fix"></param>
        public static void CPVersionComponents(ulong cpVersion, out byte major, out byte minor, out byte fix)
        {
            major = CPVersionMajor(cpVersion);
            minor = CPVersionMinor(cpVersion);
            fix = CPVersionFix(cpVersion);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetTMVersion", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetTMVersionX86(out IntPtr version);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetTMVersion", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetTMVersionX64(out IntPtr version);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public static SNRESULT GetTMVersion(out string version)
        {
            SNRESULT snresult = Is32Bit() ? GetTMVersionX86(out IntPtr version1) : GetTMVersionX64(out version1);
            version = Utf8ToString(version1, uint.MaxValue);
            return snresult;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetAPIVersion", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetAPIVersionX86(out IntPtr version);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetAPIVersion", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetAPIVersionX64(out IntPtr version);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public static SNRESULT GetAPIVersion(out string version)
        {
            SNRESULT snresult = Is32Bit() ? GetAPIVersionX86(out IntPtr version1) : GetAPIVersionX64(out version1);
            version = Utf8ToString(version1, uint.MaxValue);
            return snresult;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3TranslateError", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT TranslateErrorX86(SNRESULT res, out IntPtr message);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3TranslateError", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT TranslateErrorX64(SNRESULT res, out IntPtr message);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="errorCode"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static SNRESULT TranslateError(SNRESULT errorCode, out string message)
        {
            SNRESULT snresult = Is32Bit() ? TranslateErrorX86(errorCode, out IntPtr message1) : TranslateErrorX64(errorCode, out message1);
            message = Utf8ToString(message1, uint.MaxValue);
            return snresult;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetErrorQualifier", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetErrorQualifierX86(out uint qualifier, out IntPtr message);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetErrorQualifier", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetErrorQualifierX64(out uint qualifier, out IntPtr message);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="qualifier"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static SNRESULT GetErrorQualifier(out uint qualifier, out string message)
        {
            SNRESULT snresult = Is32Bit() ? GetErrorQualifierX86(out qualifier, out IntPtr message1) : GetErrorQualifierX64(out qualifier, out message1);
            message = Utf8ToString(message1, uint.MaxValue);
            return snresult;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetConnectStatus", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetConnectStatusX86(int target, out uint status, out IntPtr usage);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetConnectStatus", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetConnectStatusX64(int target, out uint status, out IntPtr usage);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="status"></param>
        /// <param name="usage"></param>
        /// <returns></returns>
        public static SNRESULT GetConnectStatus(int target, out ConnectStatus status, out string usage)
        {
            SNRESULT snresult = Is32Bit() ? GetConnectStatusX86(target, out uint status1, out IntPtr usage1) : GetConnectStatusX64(target, out status1, out usage1);
            status = (ConnectStatus)status1;
            usage = Utf8ToString(usage1, uint.MaxValue);
            return snresult;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3InitTargetComms", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT InitTargetCommsX86();

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3InitTargetComms", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT InitTargetCommsX64();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static SNRESULT InitTargetComms()
        {
            if (!Is32Bit())
                return InitTargetCommsX64();
            return InitTargetCommsX86();
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3CloseTargetComms", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT CloseTargetCommsX86();

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3CloseTargetComms", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT CloseTargetCommsX64();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static SNRESULT CloseTargetComms()
        {
            if (!Is32Bit())
                return CloseTargetCommsX64();
            return CloseTargetCommsX86();
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3EnumerateTargets", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT EnumerateTargetsX86(EnumerateTargetsCallback callback);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3EnumerateTargets", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT EnumerateTargetsX64(EnumerateTargetsCallback callback);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static SNRESULT EnumerateTargets(EnumerateTargetsCallback callback)
        {
            if (!Is32Bit())
                return EnumerateTargetsX64(callback);
            return EnumerateTargetsX86(callback);
        }

        private static int EnumTargetsExPriv(int target, IntPtr unused)
        {
            if (ms_enumTargetsExCallback == null)
                return -1;
            return ms_enumTargetsExCallback(target, ms_enumTargetsExUserData);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3EnumerateTargetsEx", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT EnumerateTargetsExX86(EnumerateTargetsExCallbackPriv callback, IntPtr unused);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3EnumerateTargetsEx", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT EnumerateTargetsExX64(EnumerateTargetsExCallbackPriv callback, IntPtr unused);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="userData"></param>
        /// <returns></returns>
        public static SNRESULT EnumerateTargetsEx(EnumerateTargetsExCallback callback, ref object userData)
        {
            if (callback == null)
                return SNRESULT.SN_E_BAD_PARAM;
            ms_enumTargetsExCallback = callback;
            ms_enumTargetsExUserData = userData;
            if (!Is32Bit())
                return EnumerateTargetsExX64(ms_enumTargetsExCallbackPriv, IntPtr.Zero);
            return EnumerateTargetsExX86(ms_enumTargetsExCallbackPriv, IntPtr.Zero);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetNumTargets", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetNumTargetsX86(out uint numTargets);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetNumTargets", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetNumTargetsX64(out uint numTargets);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="numTargets"></param>
        /// <returns></returns>
        public static SNRESULT GetNumTargets(out uint numTargets)
        {
            if (!Is32Bit())
                return GetNumTargetsX64(out numTargets);
            return GetNumTargetsX86(out numTargets);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetTargetFromName", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetTargetFromNameX86(IntPtr name, out int target);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetTargetFromName", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetTargetFromNameX64(IntPtr name, out int target);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static SNRESULT GetTargetFromName(string name, out int target)
        {
            ScopedGlobalHeapPtr scopedGlobalHeapPtr = new ScopedGlobalHeapPtr(AllocUtf8FromString(name));
            if (!Is32Bit())
                return GetTargetFromNameX64(scopedGlobalHeapPtr.Get(), out target);
            return GetTargetFromNameX86(scopedGlobalHeapPtr.Get(), out target);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3Reset", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ResetX86(int target, ulong resetParameter);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3Reset", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ResetX64(int target, ulong resetParameter);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="resetParameter"></param>
        /// <returns></returns>
        public static SNRESULT Reset(int target, ResetParameter resetParameter)
        {
            if (!Is32Bit())
                return ResetX64(target, (ulong)resetParameter);
            return ResetX86(target, (ulong)resetParameter);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ResetEx", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ResetExX86(int target, ulong boot, ulong bootMask, ulong reset, ulong resetMask, ulong system, ulong systemMask);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ResetEx", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ResetExX64(int target, ulong boot, ulong bootMask, ulong reset, ulong resetMask, ulong system, ulong systemMask);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="bootParameter"></param>
        /// <param name="bootMask"></param>
        /// <param name="resetParameter"></param>
        /// <param name="resetMask"></param>
        /// <param name="systemParameter"></param>
        /// <param name="systemMask"></param>
        /// <returns></returns>
        public static SNRESULT ResetEx(int target, BootParameter bootParameter, BootParameterMask bootMask, ResetParameter resetParameter, ResetParameterMask resetMask, SystemParameter systemParameter, SystemParameterMask systemMask)
        {
            if (!Is32Bit())
                return ResetExX64(target, (ulong)bootParameter, (ulong)bootMask, (ulong)resetParameter, (ulong)resetMask, (ulong)systemParameter, (ulong)systemMask);
            return ResetExX86(target, (ulong)bootParameter, (ulong)bootMask, (ulong)resetParameter, (ulong)resetMask, (ulong)systemParameter, (ulong)systemMask);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetResetParameters", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetResetParametersX86(int target, out ulong boot, out ulong bootMask, out ulong reset, out ulong resetMask, out ulong system, out ulong systemMask);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetResetParameters", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetResetParametersX64(int target, out ulong boot, out ulong bootMask, out ulong reset, out ulong resetMask, out ulong system, out ulong systemMask);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="bootParameter"></param>
        /// <param name="bootMask"></param>
        /// <param name="resetParameter"></param>
        /// <param name="resetMask"></param>
        /// <param name="systemParameter"></param>
        /// <param name="systemMask"></param>
        /// <returns></returns>
        public static SNRESULT GetResetParameters(int target, out BootParameter bootParameter, out BootParameterMask bootMask, out ResetParameter resetParameter, out ResetParameterMask resetMask, out SystemParameter systemParameter, out SystemParameterMask systemMask)
        {
            SNRESULT snresult = Is32Bit() ? GetResetParametersX86(target, out ulong boot, out ulong bootMask1, out ulong reset, out ulong resetMask1, out ulong system, out ulong systemMask1) : GetResetParametersX64(target, out boot, out bootMask1, out reset, out resetMask1, out system, out systemMask1);
            bootParameter = (BootParameter)boot;
            bootMask = (BootParameterMask)bootMask1;
            resetParameter = (ResetParameter)reset;
            resetMask = (ResetParameterMask)resetMask1;
            systemParameter = (SystemParameter)system;
            systemMask = (SystemParameterMask)systemMask1;
            return snresult;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SetBootParameter", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetBootParameterX86(int target, ulong boot, ulong bootMask);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SetBootParameter", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetBootParameterX64(int target, ulong boot, ulong bootMask);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="bootParameter"></param>
        /// <param name="bootMask"></param>
        /// <returns></returns>
        public static SNRESULT SetBootParameter(int target, BootParameter bootParameter, BootParameterMask bootMask)
        {
            if (!Is32Bit())
                return SetBootParameterX64(target, (ulong)bootParameter, (ulong)bootMask);
            return SetBootParameterX86(target, (ulong)bootParameter, (ulong)bootMask);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetCurrentBootParameter", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetCurrentBootParameterX86(int target, out ulong boot);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetCurrentBootParameter", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetCurrentBootParameterX64(int target, out ulong boot);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="bootParameter"></param>
        /// <returns></returns>
        public static SNRESULT GetCurrentBootParameter(int target, out BootParameter bootParameter)
        {
            SNRESULT snresult = Is32Bit() ? GetCurrentBootParameterX86(target, out ulong boot) : GetCurrentBootParameterX64(target, out boot);
            bootParameter = (BootParameter)boot;
            return snresult;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SetSystemParameter", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetSystemParameterX86(int target, ulong system, ulong systemMask);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SetSystemParameter", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetSystemParameterX64(int target, ulong system, ulong systemMask);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="systemParameter"></param>
        /// <param name="systemMask"></param>
        /// <returns></returns>
        public static SNRESULT SetSystemParameter(int target, SystemParameter systemParameter, SystemParameterMask systemMask)
        {
            if (!Is32Bit())
                return SetSystemParameterX64(target, (ulong)systemParameter, (ulong)systemMask);
            return SetSystemParameterX86(target, (ulong)systemParameter, (ulong)systemMask);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetTargetInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetTargetInfoX86(ref TargetInfoPriv targetInfoPriv);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetTargetInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetTargetInfoX64(ref TargetInfoPriv targetInfoPriv);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetInfo"></param>
        /// <returns></returns>
        public static SNRESULT GetTargetInfo(ref TargetInfo targetInfo)
        {
            TargetInfoPriv targetInfoPriv = new TargetInfoPriv
            {
                Flags = targetInfo.Flags,
                Target = targetInfo.Target
            };
            SNRESULT res = Is32Bit() ? GetTargetInfoX86(ref targetInfoPriv) : GetTargetInfoX64(ref targetInfoPriv);
            if (FAILED(res))
                return res;
            targetInfo.Flags = targetInfoPriv.Flags;
            targetInfo.Target = targetInfoPriv.Target;
            targetInfo.Name = Utf8ToString(targetInfoPriv.Name, uint.MaxValue);
            targetInfo.Type = Utf8ToString(targetInfoPriv.Type, uint.MaxValue);
            targetInfo.Info = Utf8ToString(targetInfoPriv.Info, uint.MaxValue);
            targetInfo.HomeDir = Utf8ToString(targetInfoPriv.HomeDir, uint.MaxValue);
            targetInfo.FSDir = Utf8ToString(targetInfoPriv.FSDir, uint.MaxValue);
            targetInfo.Boot = targetInfoPriv.Boot;
            return res;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SetTargetInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetTargetInfoX86(ref TargetInfoPriv info);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SetTargetInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetTargetInfoX64(ref TargetInfoPriv info);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetInfo"></param>
        /// <returns></returns>
        public static SNRESULT SetTargetInfo(TargetInfo targetInfo)
        {
            TargetInfoPriv info = new TargetInfoPriv
            {
                Flags = targetInfo.Flags,
                Target = targetInfo.Target
            };
            //ScopedGlobalHeapPtr scopedGlobalHeapPtr1 = new ScopedGlobalHeapPtr(IntPtr.Zero);
            //ScopedGlobalHeapPtr scopedGlobalHeapPtr2 = new ScopedGlobalHeapPtr(IntPtr.Zero);
            //ScopedGlobalHeapPtr scopedGlobalHeapPtr3 = new ScopedGlobalHeapPtr(IntPtr.Zero);
            if ((targetInfo.Flags & TargetInfoFlag.Name) > 0)
            {
                ScopedGlobalHeapPtr scopedGlobalHeapPtr4 = new ScopedGlobalHeapPtr(AllocUtf8FromString(targetInfo.Name));
                info.Name = scopedGlobalHeapPtr4.Get();
            }
            if ((targetInfo.Flags & TargetInfoFlag.HomeDir) > 0)
            {
                ScopedGlobalHeapPtr scopedGlobalHeapPtr4 = new ScopedGlobalHeapPtr(AllocUtf8FromString(targetInfo.HomeDir));
                info.HomeDir = scopedGlobalHeapPtr4.Get();
            }
            if ((targetInfo.Flags & TargetInfoFlag.FileServingDir) > 0)
            {
                ScopedGlobalHeapPtr scopedGlobalHeapPtr4 = new ScopedGlobalHeapPtr(AllocUtf8FromString(targetInfo.FSDir));
                info.FSDir = scopedGlobalHeapPtr4.Get();
            }
            if (!Is32Bit())
                return SetTargetInfoX64(ref info);
            return SetTargetInfoX86(ref info);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ListTargetTypes", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ListTargetTypesX86(ref uint size, IntPtr targetTypes);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ListTargetTypes", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ListTargetTypesX64(ref uint size, IntPtr targetTypes);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetTypes"></param>
        /// <returns></returns>
        public static SNRESULT ListTargetTypes(out TargetType[] targetTypes)
        {
            targetTypes = null;
            uint size = 0;
            SNRESULT res1 = Is32Bit() ? ListTargetTypesX86(ref size, IntPtr.Zero) : ListTargetTypesX64(ref size, IntPtr.Zero);
            if (FAILED(res1))
                return res1;
            ScopedGlobalHeapPtr scopedGlobalHeapPtr = new ScopedGlobalHeapPtr(Marshal.AllocHGlobal((int)(Marshal.SizeOf(typeof(TargetType)) * size)));
            SNRESULT res2 = Is32Bit() ? ListTargetTypesX86(ref size, scopedGlobalHeapPtr.Get()) : ListTargetTypesX64(ref size, scopedGlobalHeapPtr.Get());
            if (FAILED(res2))
                return res2;
            targetTypes = new TargetType[(int)size];
            IntPtr utf8Ptr = scopedGlobalHeapPtr.Get();
            for (uint index = 0; index < size; ++index)
            {
                targetTypes[(int)index].Type = Utf8ToString(utf8Ptr, 64U);
                utf8Ptr = new IntPtr(utf8Ptr.ToInt64() + 64L);
                targetTypes[(int)index].Description = Utf8ToString(utf8Ptr, 256U);
                utf8Ptr = new IntPtr(utf8Ptr.ToInt64() + 256L);
            }
            return res2;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3AddTarget", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT AddTargetX86(IntPtr name, IntPtr type, int connParamsSize, IntPtr connectParams, out int target);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3AddTarget", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT AddTargetX64(IntPtr name, IntPtr type, int connParamsSize, IntPtr connectParams, out int target);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="targetType"></param>
        /// <param name="connectProperties"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static SNRESULT AddTarget(string name, string targetType, TCPIPConnectProperties connectProperties, out int target)
        {
            ScopedGlobalHeapPtr scopedGlobalHeapPtr1 = new ScopedGlobalHeapPtr(IntPtr.Zero);
            int num = 0;
            if (connectProperties != null)
            {
                num = Marshal.SizeOf((object)connectProperties);
                scopedGlobalHeapPtr1 = new ScopedGlobalHeapPtr(Marshal.AllocHGlobal(num));
                Marshal.StructureToPtr((object)connectProperties, scopedGlobalHeapPtr1.Get(), false);
            }
            ScopedGlobalHeapPtr scopedGlobalHeapPtr2 = new ScopedGlobalHeapPtr(AllocUtf8FromString(name));
            ScopedGlobalHeapPtr scopedGlobalHeapPtr3 = new ScopedGlobalHeapPtr(AllocUtf8FromString(targetType));
            return Is32Bit() ? AddTargetX86(scopedGlobalHeapPtr2.Get(), scopedGlobalHeapPtr3.Get(), num, scopedGlobalHeapPtr1.Get(), out target) : AddTargetX64(scopedGlobalHeapPtr2.Get(), scopedGlobalHeapPtr3.Get(), num, scopedGlobalHeapPtr1.Get(), out target);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SetDefaultTarget", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetDefaultTargetX86(int target);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SetDefaultTarget", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetDefaultTargetX64(int target);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static SNRESULT SetDefaultTarget(int target)
        {
            if (!Is32Bit())
                return SetDefaultTargetX64(target);
            return SetDefaultTargetX86(target);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetDefaultTarget", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetDefaultTargetX86(out int target);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetDefaultTarget", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetDefaultTargetX64(out int target);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static SNRESULT GetDefaultTarget(out int target)
        {
            if (!Is32Bit())
                return GetDefaultTargetX64(out target);
            return GetDefaultTargetX86(out target);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3RegisterServerEventHandler", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT RegisterServerEventHandlerX86(HandleEventCallbackPriv callback, IntPtr userData);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3RegisterServerEventHandler", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT RegisterServerEventHandlerX64(HandleEventCallbackPriv callback, IntPtr userData);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="userData"></param>
        /// <returns></returns>
        public static SNRESULT RegisterServerEventHandler(ServerEventCallback callback, ref object userData)
        {
            if (callback == null)
                return SNRESULT.SN_E_BAD_PARAM;
            SNRESULT res = Is32Bit() ? RegisterServerEventHandlerX86(ms_eventHandlerWrapper, IntPtr.Zero) : RegisterServerEventHandlerX64(ms_eventHandlerWrapper, IntPtr.Zero);
            if (SUCCEEDED(res))
            {
                ms_serverEventCallback = callback;
                ms_serverEventUserData = userData;
            }
            return res;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3UnRegisterServerEventHandler", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT UnregisterServerEventHandlerX86();

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3UnRegisterServerEventHandler", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT UnregisterServerEventHandlerX64();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static SNRESULT UnregisterServerEventHandler()
        {
            SNRESULT res = Is32Bit() ? UnregisterServerEventHandlerX86() : UnregisterServerEventHandlerX64();
            if (SUCCEEDED(res))
            {
                ms_serverEventCallback = null;
                ms_serverEventUserData = null;
            }
            return res;
        }

        private static void MarshalServerEvent(int target, uint param, SNRESULT result, uint length, IntPtr data)
        {
            if (ms_serverEventCallback == null)
                return;
            ServerEventHeader storage = new ServerEventHeader();
            ReadDataFromUnmanagedIncPtr(data, ref storage);
            ms_serverEventCallback(target, result, storage.eventType, ms_serverEventUserData);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetConnectionInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetConnectionInfoX86(int target, IntPtr connectProperties);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetConnectionInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetConnectionInfoX64(int target, IntPtr connectProperties);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="connectProperties"></param>
        /// <returns></returns>
        public static SNRESULT GetConnectionInfo(int target, out TCPIPConnectProperties connectProperties)
        {
            connectProperties = null;
            ScopedGlobalHeapPtr scopedGlobalHeapPtr = new ScopedGlobalHeapPtr(Marshal.AllocHGlobal(Marshal.SizeOf(typeof(TCPIPConnectProperties))));
            SNRESULT res = Is32Bit() ? GetConnectionInfoX86(target, scopedGlobalHeapPtr.Get()) : GetConnectionInfoX64(target, scopedGlobalHeapPtr.Get());
            if (SUCCEEDED(res))
            {
                connectProperties = new TCPIPConnectProperties();
                Marshal.PtrToStructure(scopedGlobalHeapPtr.Get(), (object)connectProperties);
            }
            return res;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SetConnectionInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetConnectionInfoX86(int target, IntPtr connectProperties);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SetConnectionInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetConnectionInfoX64(int target, IntPtr connectProperties);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="connectProperties"></param>
        /// <returns></returns>
        public static SNRESULT SetConnectionInfo(int target, TCPIPConnectProperties connectProperties)
        {
            ScopedGlobalHeapPtr scopedGlobalHeapPtr = new ScopedGlobalHeapPtr(Marshal.AllocHGlobal(Marshal.SizeOf((object)connectProperties)));
            WriteDataToUnmanagedIncPtr(connectProperties, scopedGlobalHeapPtr.Get());
            if (!Is32Bit())
                return SetConnectionInfoX64(target, scopedGlobalHeapPtr.Get());
            return SetConnectionInfoX86(target, scopedGlobalHeapPtr.Get());
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3DeleteTarget", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT DeleteTargetX86(int target);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3DeleteTarget", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT DeleteTargetX64(int target);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static SNRESULT DeleteTarget(int target)
        {
            if (!Is32Bit())
                return DeleteTargetX64(target);
            return DeleteTargetX86(target);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3Connect", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ConnectX86(int target, string application);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3Connect", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ConnectX64(int target, string application);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="application"></param>
        /// <returns></returns>
        public static SNRESULT Connect(int target, string application)
        {
            if (!Is32Bit())
                return ConnectX64(target, application);
            return ConnectX86(target, application);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ConnectEx", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ConnectExX86(int target, string application, bool bForceFlag);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ConnectEx", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ConnectExX64(int target, string application, bool bForceFlag);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="application"></param>
        /// <param name="bForceFlag"></param>
        /// <returns></returns>
        public static SNRESULT ConnectEx(int target, string application, bool bForceFlag)
        {
            if (!Is32Bit())
                return ConnectExX64(target, application, bForceFlag);
            return ConnectExX86(target, application, bForceFlag);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3Disconnect", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT DisconnectX86(int target);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3Disconnect", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT DisconnectX64(int target);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static SNRESULT Disconnect(int target)
        {
            if (!Is32Bit())
                return DisconnectX64(target);
            return DisconnectX86(target);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ForceDisconnect", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ForceDisconnectX86(int target);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ForceDisconnect", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ForceDisconnectX64(int target);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static SNRESULT ForceDisconnect(int target)
        {
            if (!Is32Bit())
                return ForceDisconnectX64(target);
            return ForceDisconnectX86(target);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetSystemInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetSystemInfoX86(int target, uint reserved, out uint mask, out SystemInfo info);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetSystemInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetSystemInfoX64(int target, uint reserved, out uint mask, out SystemInfo info);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="mask"></param>
        /// <param name="systemInfo"></param>
        /// <returns></returns>
        public static SNRESULT GetSystemInfo(int target, out SystemInfoFlag mask, out SystemInfo systemInfo)
        {
            SNRESULT snresult = Is32Bit() ? GetSystemInfoX86(target, 0U, out uint mask1, out systemInfo) : GetSystemInfoX64(target, 0U, out mask1, out systemInfo);
            mask = (SystemInfoFlag)mask1;
            return snresult;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetExtraLoadFlags", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetExtraLoadFlagsX86(int target, out ulong extraLoadFlags);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetExtraLoadFlags", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetExtraLoadFlagsX64(int target, out ulong extraLoadFlags);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="extraLoadFlags"></param>
        /// <returns></returns>
        public static SNRESULT GetExtraLoadFlags(int target, out ExtraLoadFlag extraLoadFlags)
        {
            SNRESULT snresult = Is32Bit() ? GetExtraLoadFlagsX86(target, out ulong extraLoadFlags1) : GetExtraLoadFlagsX64(target, out extraLoadFlags1);
            extraLoadFlags = (ExtraLoadFlag)extraLoadFlags1;
            return snresult;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SetExtraLoadFlags", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetExtraLoadFlagsX86(int target, ulong extraLoadFlags, ulong mask);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SetExtraLoadFlags", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetExtraLoadFlagsX64(int target, ulong extraLoadFlags, ulong mask);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="extraLoadFlags"></param>
        /// <param name="mask"></param>
        /// <returns></returns>
        public static SNRESULT SetExtraLoadFlags(int target, ExtraLoadFlag extraLoadFlags, ExtraLoadFlagMask mask)
        {
            if (!Is32Bit())
                return SetExtraLoadFlagsX64(target, (ulong)extraLoadFlags, (ulong)mask);
            return SetExtraLoadFlagsX86(target, (ulong)extraLoadFlags, (ulong)mask);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetSDKVersion", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetSDKVersionX86(int target, out ulong sdkVersion);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetSDKVersion", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetSDKVersionX64(int target, out ulong sdkVersion);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="sdkVersion"></param>
        /// <returns></returns>
        public static SNRESULT GetSDKVersion(int target, out ulong sdkVersion)
        {
            if (!Is32Bit())
                return GetSDKVersionX64(target, out sdkVersion);
            return GetSDKVersionX86(target, out sdkVersion);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetCPVersion", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetCPVersionX86(int target, out ulong cpVersion);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetCPVersion", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetCPVersionX64(int target, out ulong cpVersion);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="cpVersion"></param>
        /// <returns></returns>
        public static SNRESULT GetCPVersion(int target, out ulong cpVersion)
        {
            if (!Is32Bit())
                return GetCPVersionX64(target, out cpVersion);
            return GetCPVersionX86(target, out cpVersion);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SetTimeouts", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetTimeoutsX86(int target, uint numTimeouts, TimeoutType[] timeoutIds, uint[] timeoutValues);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SetTimeouts", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetTimeoutsX64(int target, uint numTimeouts, TimeoutType[] timeoutIds, uint[] timeoutValues);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="timeoutTypes"></param>
        /// <param name="timeoutValues"></param>
        /// <returns></returns>
        public static SNRESULT SetTimeouts(int target, TimeoutType[] timeoutTypes, uint[] timeoutValues)
        {
            if (timeoutTypes == null || timeoutTypes.Length < 1 || (timeoutValues == null || timeoutValues.Length != timeoutTypes.Length))
                return SNRESULT.SN_E_BAD_PARAM;
            if (!Is32Bit())
                return SetTimeoutsX64(target, (uint)timeoutTypes.Length, timeoutTypes, timeoutValues);
            return SetTimeoutsX86(target, (uint)timeoutTypes.Length, timeoutTypes, timeoutValues);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetTimeouts", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetTimeoutsX86(int target, out uint numTimeouts, TimeoutType[] timeoutIds, uint[] timeoutValues);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetTimeouts", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetTimeoutsX64(int target, out uint numTimeouts, TimeoutType[] timeoutIds, uint[] timeoutValues);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="timeoutTypes"></param>
        /// <param name="timeoutValues"></param>
        /// <returns></returns>
        public static SNRESULT GetTimeouts(int target, out TimeoutType[] timeoutTypes, out uint[] timeoutValues)
        {
            timeoutTypes = null;
            timeoutValues = null;
            SNRESULT res = Is32Bit() ? GetTimeoutsX86(target, out uint numTimeouts, null, null) : GetTimeoutsX64(target, out numTimeouts, null, null);
            if (FAILED(res))
                return res;
            timeoutTypes = new TimeoutType[(int)numTimeouts];
            timeoutValues = new uint[(int)numTimeouts];
            if (!Is32Bit())
                return GetTimeoutsX64(target, out numTimeouts, timeoutTypes, timeoutValues);
            return GetTimeoutsX86(target, out numTimeouts, timeoutTypes, timeoutValues);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ListTTYStreams", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ListTtyStreamsX86(int target, ref uint size, IntPtr streamArray);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ListTTYStreams", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ListTtyStreamsX64(int target, ref uint size, IntPtr streamArray);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="streamArray"></param>
        /// <returns></returns>
        public static SNRESULT ListTTYStreams(int target, out TTYStream[] streamArray)
        {
            streamArray = null;
            uint size = 0;
            SNRESULT res1 = Is32Bit() ? ListTtyStreamsX86(target, ref size, IntPtr.Zero) : ListTtyStreamsX64(target, ref size, IntPtr.Zero);
            if (FAILED(res1))
                return res1;
            ScopedGlobalHeapPtr scopedGlobalHeapPtr = new ScopedGlobalHeapPtr(Marshal.AllocHGlobal((int)(Marshal.SizeOf(typeof(TTYStream)) * size)));
            SNRESULT res2 = Is32Bit() ? ListTtyStreamsX86(target, ref size, scopedGlobalHeapPtr.Get()) : ListTtyStreamsX64(target, ref size, scopedGlobalHeapPtr.Get());
            if (FAILED(res2))
                return res2;
            IntPtr unmanagedBuf = scopedGlobalHeapPtr.Get();
            streamArray = new TTYStream[(int)size];
            for (uint index = 0; index < size; ++index)
                unmanagedBuf = ReadDataFromUnmanagedIncPtr(unmanagedBuf, ref streamArray[(int)index]);
            return res2;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3RegisterTTYEventHandler", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT RegisterTtyEventHandlerX86(int target, uint streamIndex, HandleEventCallbackPriv callback, IntPtr userData);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3RegisterTTYEventHandler", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT RegisterTtyEventHandlerX64(int target, uint streamIndex, HandleEventCallbackPriv callback, IntPtr userData);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="streamID"></param>
        /// <param name="callback"></param>
        /// <param name="userData"></param>
        /// <returns></returns>
        public static SNRESULT RegisterTTYEventHandler(int target, uint streamID, TTYCallback callback, ref object userData)
        {
            return RegisterTTYEventHandlerHelper(target, streamID, callback, ref userData);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="streamID"></param>
        /// <param name="callback"></param>
        /// <param name="userData"></param>
        /// <returns></returns>
        public static SNRESULT RegisterTTYEventHandlerRaw(int target, uint streamID, TTYCallbackRaw callback, ref object userData)
        {
            return RegisterTTYEventHandlerHelper(target, streamID, callback, ref userData);
        }

        private static SNRESULT RegisterTTYEventHandlerHelper(int target, uint streamID, object callback, ref object userData)
        {
            if (callback == null)
                return SNRESULT.SN_E_BAD_PARAM;
            SNRESULT res = Is32Bit() ? RegisterTtyEventHandlerX86(target, streamID, ms_eventHandlerWrapper, IntPtr.Zero) : RegisterTtyEventHandlerX64(target, streamID, ms_eventHandlerWrapper, IntPtr.Zero);
            if (FAILED(res))
                return res;
            List<TTYChannel> ttyChannelList = new List<TTYChannel>();
            if ((int)streamID == -1)
            {
                res = ListTTYStreams(target, out TTYStream[] streamArray);
                if (FAILED(res) || streamArray == null || streamArray.Length == 0)
                    return res;
                foreach (TTYStream ttyStream in streamArray)
                    ttyChannelList.Add(new TTYChannel(target, ttyStream.Index));
            }
            else
                ttyChannelList.Add(new TTYChannel(target, streamID));
            if (ms_userTtyCallbacks == null)
                ms_userTtyCallbacks = new Dictionary<TTYChannel, TTYCallbackAndUserData>(1);
            foreach (TTYChannel key in ttyChannelList)
            {
                if (!ms_userTtyCallbacks.TryGetValue(key, out TTYCallbackAndUserData callbackAndUserData))
                    callbackAndUserData = new TTYCallbackAndUserData();
                if (callback is TTYCallback)
                {
                    callbackAndUserData.m_callback = (TTYCallback)callback;
                    callbackAndUserData.m_userData = userData;
                }
                else
                {
                    callbackAndUserData.m_callbackRaw = (TTYCallbackRaw)callback;
                    callbackAndUserData.m_userDataRaw = userData;
                }
                ms_userTtyCallbacks[key] = callbackAndUserData;
            }
            return res;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3CancelTTYEvents", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT CancelTtyEventsX86(int target, uint index);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3CancelTTYEvents", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT CancelTtyEventsX64(int target, uint index);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="streamID"></param>
        /// <returns></returns>
        public static SNRESULT CancelTTYEvents(int target, uint streamID)
        {
            SNRESULT res = Is32Bit() ? CancelTtyEventsX86(target, streamID) : CancelTtyEventsX64(target, streamID);
            if (SUCCEEDED(res) && ms_userTtyCallbacks != null)
            {
                if ((int)streamID == -1)
                {
                    List<TTYChannel> ttyChannelList = new List<TTYChannel>();
                    foreach (KeyValuePair<TTYChannel, TTYCallbackAndUserData> msUserTtyCallback in ms_userTtyCallbacks)
                    {
                        if (msUserTtyCallback.Key.Target == target)
                            ttyChannelList.Add(msUserTtyCallback.Key);
                    }
                    foreach (TTYChannel key in ttyChannelList)
                        ms_userTtyCallbacks.Remove(key);
                }
                else
                    ms_userTtyCallbacks.Remove(new TTYChannel(target, streamID));
            }
            return res;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SendTTY", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SendTTYX86(int target, uint index, string text);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SendTTY", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SendTTYX64(int target, uint index, string text);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="streamID"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static SNRESULT SendTTY(int target, uint streamID, string text)
        {
            if (!Is32Bit())
                return SendTTYX64(target, streamID, text);
            return SendTTYX86(target, streamID, text);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SendTTY", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SendTTYRawX86(int target, uint index, byte[] text);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SendTTY", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SendTTYRawX64(int target, uint index, byte[] text);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="streamID"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static SNRESULT SendTTYRaw(int target, uint streamID, byte[] text)
        {
            return Is32Bit() ? SendTTYRawX86(target, streamID, text) : SendTTYRawX64(target, streamID, text);
        }

        private static void MarshalTTYEvent(int target, uint param, SNRESULT result, uint length, IntPtr data)
        {
            if (ms_userTtyCallbacks == null)
                return;
            TTYChannel key = new TTYChannel(target, param);
            if (!ms_userTtyCallbacks.TryGetValue(key, out TTYCallbackAndUserData callbackAndUserData))
                return;
            if (callbackAndUserData.m_callback != null)
            {
                string stringAnsi = Marshal.PtrToStringAnsi(data, (int)length);
                callbackAndUserData.m_callback(target, param, result, stringAnsi, callbackAndUserData.m_userData);
            }
            if (callbackAndUserData.m_callbackRaw == null)
                return;
            byte[] numArray = new byte[(int)length];
            Marshal.Copy(data, numArray, 0, (int)length);
            callbackAndUserData.m_callbackRaw(target, param, result, numArray, callbackAndUserData.m_userDataRaw);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ClearTTYCache", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ClearTTYCacheX86(int target);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ClearTTYCache", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ClearTTYCacheX64(int target);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static SNRESULT ClearTTYCache(int target)
        {
            return Is32Bit() ? ClearTTYCacheX86(target) : ClearTTYCacheX64(target);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3Kick", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT KickX86();

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3Kick", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT KickX64();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static SNRESULT Kick()
        {
            if (!Is32Bit())
                return KickX64();
            return KickX86();
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetStatus", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetStatusX86(int target, UnitType unit, out long status, IntPtr reasonCode);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetStatus", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetStatusX64(int target, UnitType unit, out long status, IntPtr reasonCode);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="unit"></param>
        /// <param name="unitStatus"></param>
        /// <returns></returns>
        public static SNRESULT GetStatus(int target, UnitType unit, out UnitStatus unitStatus)
        {
            SNRESULT snresult = Is32Bit() ? GetStatusX86(target, unit, out long status, IntPtr.Zero) : GetStatusX64(target, unit, out status, IntPtr.Zero);
            unitStatus = (UnitStatus)status;
            return snresult;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ProcessLoad", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ProcessLoadX86(int target, uint priority, IntPtr fileName, int argCount, string[] args, int envCount, string[] env, out uint processId, out ulong threadId, uint flags);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ProcessLoad", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ProcessLoadX64(int target, uint priority, IntPtr fileName, int argCount, string[] args, int envCount, string[] env, out uint processId, out ulong threadId, uint flags);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="priority"></param>
        /// <param name="fileName"></param>
        /// <param name="argv"></param>
        /// <param name="envv"></param>
        /// <param name="processID"></param>
        /// <param name="threadID"></param>
        /// <param name="loadFlags"></param>
        /// <returns></returns>
        public static SNRESULT ProcessLoad(int target, uint priority, string fileName, string[] argv, string[] envv, out uint processID, out ulong threadID, LoadFlag loadFlags)
        {
            ScopedGlobalHeapPtr scopedGlobalHeapPtr = new ScopedGlobalHeapPtr(AllocUtf8FromString(fileName));
            int argCount = 0;
            if (argv != null)
                argCount = argv.Length;
            int envCount = 0;
            if (envv != null)
                envCount = envv.Length;
            if (!Is32Bit())
                return ProcessLoadX64(target, priority, scopedGlobalHeapPtr.Get(), argCount, argv, envCount, envv, out processID, out threadID, (uint)loadFlags);
            return ProcessLoadX86(target, priority, scopedGlobalHeapPtr.Get(), argCount, argv, envCount, envv, out processID, out threadID, (uint)loadFlags);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ProcessList", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetProcessListX86(int target, ref uint count, IntPtr processIdArray);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ProcessList", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetProcessListX64(int target, ref uint count, IntPtr processIdArray);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="processIDs"></param>
        /// <returns></returns>
        public static SNRESULT GetProcessList(int target, out uint[] processIDs)
        {
            processIDs = null;
            uint count = 0;
            SNRESULT res1 = Is32Bit() ? GetProcessListX86(target, ref count, IntPtr.Zero) : GetProcessListX64(target, ref count, IntPtr.Zero);
            if (FAILED(res1))
                return res1;
            ScopedGlobalHeapPtr scopedGlobalHeapPtr = new ScopedGlobalHeapPtr(Marshal.AllocHGlobal(4 * (int)count));
            SNRESULT res2 = Is32Bit() ? GetProcessListX86(target, ref count, scopedGlobalHeapPtr.Get()) : GetProcessListX64(target, ref count, scopedGlobalHeapPtr.Get());
            if (FAILED(res2))
                return res2;
            IntPtr unmanagedBuf = scopedGlobalHeapPtr.Get();
            processIDs = new uint[(int)count];
            for (uint index = 0; index < count; ++index)
                unmanagedBuf = ReadDataFromUnmanagedIncPtr(unmanagedBuf, ref processIDs[(int)index]);
            return res2;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3UserProcessList", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetUserProcessListX86(int target, ref uint count, IntPtr processIdArray);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3UserProcessList", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetUserProcessListX64(int target, ref uint count, IntPtr processIdArray);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="processIDs"></param>
        /// <returns></returns>
        public static SNRESULT GetUserProcessList(int target, out uint[] processIDs)
        {
            uint count = 0;
            processIDs = null;
            SNRESULT res1 = Is32Bit() ? GetUserProcessListX86(target, ref count, IntPtr.Zero) : GetUserProcessListX64(target, ref count, IntPtr.Zero);
            if (FAILED(res1))
                return res1;
            ScopedGlobalHeapPtr scopedGlobalHeapPtr = new ScopedGlobalHeapPtr(Marshal.AllocHGlobal(4 * (int)count));
            SNRESULT res2 = Is32Bit() ? GetUserProcessListX86(target, ref count, scopedGlobalHeapPtr.Get()) : GetUserProcessListX64(target, ref count, scopedGlobalHeapPtr.Get());
            if (FAILED(res2))
                return res2;
            IntPtr unmanagedBuf = scopedGlobalHeapPtr.Get();
            processIDs = new uint[(int)count];
            for (uint index = 0; index < count; ++index)
                unmanagedBuf = ReadDataFromUnmanagedIncPtr(unmanagedBuf, ref processIDs[(int)index]);
            return res2;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ProcessStop", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ProcessStopX86(int target, uint processId);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ProcessStop", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ProcessStopX64(int target, uint processId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="processID"></param>
        /// <returns></returns>
        public static SNRESULT ProcessStop(int target, uint processID)
        {
            if (!Is32Bit())
                return ProcessStopX64(target, processID);
            return ProcessStopX86(target, processID);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ProcessContinue", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ProcessContinueX86(int target, uint processId);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ProcessContinue", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ProcessContinueX64(int target, uint processId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="processID"></param>
        /// <returns></returns>
        public static SNRESULT ProcessContinue(int target, uint processID)
        {
            if (!Is32Bit())
                return ProcessContinueX64(target, processID);
            return ProcessContinueX86(target, processID);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ProcessKill", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ProcessKillX86(int target, uint processId);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ProcessKill", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ProcessKillX64(int target, uint processId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="processID"></param>
        /// <returns></returns>
        public static SNRESULT ProcessKill(int target, uint processID)
        {
            if (!Is32Bit())
                return ProcessKillX64(target, processID);
            return ProcessKillX86(target, processID);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3TerminateGameProcess", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT TerminateGameProcessX86(int target, uint processId, uint timeout);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3TerminateGameProcess", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT TerminateGameProcessX64(int target, uint processId, uint timeout);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="processID"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static SNRESULT TerminateGameProcess(int target, uint processID, uint timeout)
        {
            if (!Is32Bit())
                return TerminateGameProcessX64(target, processID, timeout);
            return TerminateGameProcessX86(target, processID, timeout);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ThreadList", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetThreadListX86(int target, uint processId, ref uint numPPUThreads, ulong[] ppuThreadIds, ref uint numSPUThreadGroups, ulong[] spuThreadGroupIds);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ThreadList", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetThreadListX64(int target, uint processId, ref uint numPPUThreads, ulong[] ppuThreadIds, ref uint numSPUThreadGroups, ulong[] spuThreadGroupIds);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="processID"></param>
        /// <param name="ppuThreadIDs"></param>
        /// <param name="spuThreadGroupIDs"></param>
        /// <returns></returns>
        public static SNRESULT GetThreadList(int target, uint processID, out ulong[] ppuThreadIDs, out ulong[] spuThreadGroupIDs)
        {
            ppuThreadIDs = null;
            spuThreadGroupIDs = null;
            uint numPPUThreads = 0;
            uint numSPUThreadGroups = 0;
            SNRESULT res = Is32Bit() ? GetThreadListX86(target, processID, ref numPPUThreads, null, ref numSPUThreadGroups, null) : GetThreadListX64(target, processID, ref numPPUThreads, null, ref numSPUThreadGroups, null);
            if (FAILED(res))
                return res;
            ppuThreadIDs = new ulong[(int)numPPUThreads];
            spuThreadGroupIDs = new ulong[(int)numSPUThreadGroups];
            return Is32Bit() ? GetThreadListX86(target, processID, ref numPPUThreads, ppuThreadIDs, ref numSPUThreadGroups, spuThreadGroupIDs) : GetThreadListX64(target, processID, ref numPPUThreads, ppuThreadIDs, ref numSPUThreadGroups, spuThreadGroupIDs);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ThreadStop", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ThreadStopX86(int target, UnitType unit, uint processId, ulong threadId);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ThreadStop", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ThreadStopX64(int target, UnitType unit, uint processId, ulong threadId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="unit"></param>
        /// <param name="processID"></param>
        /// <param name="threadID"></param>
        /// <returns></returns>
        public static SNRESULT ThreadStop(int target, UnitType unit, uint processID, ulong threadID)
        {
            if (!Is32Bit())
                return ThreadStopX64(target, unit, processID, threadID);
            return ThreadStopX86(target, unit, processID, threadID);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ThreadContinue", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ThreadContinueX86(int target, UnitType unit, uint processId, ulong threadId);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ThreadContinue", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ThreadContinueX64(int target, UnitType unit, uint processId, ulong threadId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="unit"></param>
        /// <param name="processID"></param>
        /// <param name="threadID"></param>
        /// <returns></returns>
        public static SNRESULT ThreadContinue(int target, UnitType unit, uint processID, ulong threadID)
        {
            if (!Is32Bit())
                return ThreadContinueX64(target, unit, processID, threadID);
            return ThreadContinueX86(target, unit, processID, threadID);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ThreadGetRegisters", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ThreadGetRegistersX86(int target, UnitType unit, uint processId, ulong threadId, uint numRegisters, uint[] registerNums, ulong[] registerValues);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ThreadGetRegisters", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ThreadGetRegistersX64(int target, UnitType unit, uint processId, ulong threadId, uint numRegisters, uint[] registerNums, ulong[] registerValues);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="unit"></param>
        /// <param name="processID"></param>
        /// <param name="threadID"></param>
        /// <param name="registerNums"></param>
        /// <param name="registerValues"></param>
        /// <returns></returns>
        public static SNRESULT ThreadGetRegisters(int target, UnitType unit, uint processID, ulong threadID, uint[] registerNums, out ulong[] registerValues)
        {
            registerValues = null;
            if (registerNums == null)
                return SNRESULT.SN_E_BAD_PARAM;
            registerValues = new ulong[registerNums.Length];
            if (!Is32Bit())
                return ThreadGetRegistersX64(target, unit, processID, threadID, (uint)registerNums.Length, registerNums, registerValues);
            return ThreadGetRegistersX86(target, unit, processID, threadID, (uint)registerNums.Length, registerNums, registerValues);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ThreadSetRegisters", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ThreadSetRegistersX86(int target, UnitType unit, uint processId, ulong threadId, uint numRegisters, uint[] registerNums, ulong[] registerValues);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ThreadSetRegisters", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ThreadSetRegistersX64(int target, UnitType unit, uint processId, ulong threadId, uint numRegisters, uint[] registerNums, ulong[] registerValues);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="unit"></param>
        /// <param name="processID"></param>
        /// <param name="threadID"></param>
        /// <param name="registerNums"></param>
        /// <param name="registerValues"></param>
        /// <returns></returns>
        public static SNRESULT ThreadSetRegisters(int target, UnitType unit, uint processID, ulong threadID, uint[] registerNums, ulong[] registerValues)
        {
            if (registerNums == null || registerValues == null || registerNums.Length != registerValues.Length)
                return SNRESULT.SN_E_BAD_PARAM;
            if (!Is32Bit())
                return ThreadSetRegistersX64(target, unit, processID, threadID, (uint)registerNums.Length, registerNums, registerValues);
            return ThreadSetRegistersX86(target, unit, processID, threadID, (uint)registerNums.Length, registerNums, registerValues);
        }

        private static void ProcessInfoMarshalHelper(IntPtr unmanagedBuf, ref ProcessInfo processInfo)
        {
            uint storage = 0;
            unmanagedBuf = ReadDataFromUnmanagedIncPtr(unmanagedBuf, ref storage);
            processInfo.Hdr.Status = (ProcessStatus)storage;
            unmanagedBuf = ReadDataFromUnmanagedIncPtr(unmanagedBuf, ref processInfo.Hdr.NumPPUThreads);
            unmanagedBuf = ReadDataFromUnmanagedIncPtr(unmanagedBuf, ref processInfo.Hdr.NumSPUThreads);
            unmanagedBuf = ReadDataFromUnmanagedIncPtr(unmanagedBuf, ref processInfo.Hdr.ParentProcessID);
            unmanagedBuf = ReadDataFromUnmanagedIncPtr(unmanagedBuf, ref processInfo.Hdr.MaxMemorySize);
            processInfo.Hdr.ELFPath = Utf8ToString(unmanagedBuf, 512U);
            unmanagedBuf = new IntPtr(unmanagedBuf.ToInt64() + 512L);
            uint num = processInfo.Hdr.NumPPUThreads + processInfo.Hdr.NumSPUThreads;
            processInfo.ThreadIDs = new ulong[(int)num];
            for (int index = 0; index < num; ++index)
                unmanagedBuf = ReadDataFromUnmanagedIncPtr(unmanagedBuf, ref processInfo.ThreadIDs[index]);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ProcessInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetProcessInfoX86(int target, uint processId, ref uint bufferSize, IntPtr processInfo);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ProcessInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetProcessInfoX64(int target, uint processId, ref uint bufferSize, IntPtr processInfo);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="processID"></param>
        /// <param name="processInfo"></param>
        /// <returns></returns>
        public static SNRESULT GetProcessInfo(int target, uint processID, out ProcessInfo processInfo)
        {
            processInfo = new ProcessInfo();
            uint bufferSize = 0;
            SNRESULT res1 = Is32Bit() ? GetProcessInfoX86(target, processID, ref bufferSize, IntPtr.Zero) : GetProcessInfoX64(target, processID, ref bufferSize, IntPtr.Zero);
            if (FAILED(res1))
                return res1;
            ScopedGlobalHeapPtr scopedGlobalHeapPtr = new ScopedGlobalHeapPtr(Marshal.AllocHGlobal((int)bufferSize));
            SNRESULT res2 = Is32Bit() ? GetProcessInfoX86(target, processID, ref bufferSize, scopedGlobalHeapPtr.Get()) : GetProcessInfoX64(target, processID, ref bufferSize, scopedGlobalHeapPtr.Get());
            if (SUCCEEDED(res2))
                ProcessInfoMarshalHelper(scopedGlobalHeapPtr.Get(), ref processInfo);
            return res2;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ProcessInfoEx", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetProcessInfoExX86(int target, uint processId, ref uint bufferSize, IntPtr processInfo, out ExtraProcessInfo extraProcessInfo);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ProcessInfoEx", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetProcessInfoExX64(int target, uint processId, ref uint bufferSize, IntPtr processInfo, out ExtraProcessInfo extraProcessInfo);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="processID"></param>
        /// <param name="processInfo"></param>
        /// <param name="extraProcessInfo"></param>
        /// <returns></returns>
        public static SNRESULT GetProcessInfoEx(int target, uint processID, out ProcessInfo processInfo, out ExtraProcessInfo extraProcessInfo)
        {
            processInfo = new ProcessInfo();
            uint bufferSize = 0;
            SNRESULT res1 = Is32Bit() ? GetProcessInfoExX86(target, processID, ref bufferSize, IntPtr.Zero, out extraProcessInfo) : GetProcessInfoExX64(target, processID, ref bufferSize, IntPtr.Zero, out extraProcessInfo);
            if (FAILED(res1))
                return res1;
            ScopedGlobalHeapPtr scopedGlobalHeapPtr = new ScopedGlobalHeapPtr(Marshal.AllocHGlobal((int)bufferSize));
            SNRESULT res2 = Is32Bit() ? GetProcessInfoExX86(target, processID, ref bufferSize, scopedGlobalHeapPtr.Get(), out extraProcessInfo) : GetProcessInfoExX64(target, processID, ref bufferSize, scopedGlobalHeapPtr.Get(), out extraProcessInfo);
            if (SUCCEEDED(res2))
                ProcessInfoMarshalHelper(scopedGlobalHeapPtr.Get(), ref processInfo);
            return res2;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ProcessInfoEx2", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetProcessInfoEx2X86(int target, uint processId, ref uint bufferSize, IntPtr processInfo, out ExtraProcessInfo extraProcessInfo, out ProcessLoadInfo processLoadInfo);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ProcessInfoEx2", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetProcessInfoEx2X64(int target, uint processId, ref uint bufferSize, IntPtr processInfo, out ExtraProcessInfo extraProcessInfo, out ProcessLoadInfo processLoadInfo);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="processID"></param>
        /// <param name="processInfo"></param>
        /// <param name="extraProcessInfo"></param>
        /// <param name="processLoadInfo"></param>
        /// <returns></returns>
        public static SNRESULT GetProcessInfoEx2(int target, uint processID, out ProcessInfo processInfo, out ExtraProcessInfo extraProcessInfo, out ProcessLoadInfo processLoadInfo)
        {
            uint bufferSize = 0;
            processInfo = new ProcessInfo();
            SNRESULT res1 = Is32Bit() ? GetProcessInfoEx2X86(target, processID, ref bufferSize, IntPtr.Zero, out extraProcessInfo, out processLoadInfo) : GetProcessInfoEx2X64(target, processID, ref bufferSize, IntPtr.Zero, out extraProcessInfo, out processLoadInfo);
            if (FAILED(res1))
                return res1;
            ScopedGlobalHeapPtr scopedGlobalHeapPtr = new ScopedGlobalHeapPtr(Marshal.AllocHGlobal((int)bufferSize));
            SNRESULT res2 = Is32Bit() ? GetProcessInfoEx2X86(target, processID, ref bufferSize, scopedGlobalHeapPtr.Get(), out extraProcessInfo, out processLoadInfo) : GetProcessInfoEx2X64(target, processID, ref bufferSize, scopedGlobalHeapPtr.Get(), out extraProcessInfo, out processLoadInfo);
            if (SUCCEEDED(res2))
                ProcessInfoMarshalHelper(scopedGlobalHeapPtr.Get(), ref processInfo);
            return res2;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetModuleList", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetModuleListX86(int target, uint processId, ref uint numModules, uint[] moduleList);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetModuleList", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetModuleListX64(int target, uint processId, ref uint numModules, uint[] moduleList);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="processID"></param>
        /// <param name="modules"></param>
        /// <returns></returns>
        public static SNRESULT GetModuleList(int target, uint processID, out uint[] modules)
        {
            modules = null;
            uint numModules = 0;
            SNRESULT res = Is32Bit() ? GetModuleListX86(target, processID, ref numModules, null) : GetModuleListX64(target, processID, ref numModules, null);
            if (FAILED(res))
                return res;
            modules = new uint[(int)numModules];
            return Is32Bit() ? GetModuleListX86(target, processID, ref numModules, modules) : GetModuleListX64(target, processID, ref numModules, modules);
        }

        private static IntPtr ModuleInfoHdrMarshalHelper(IntPtr unmanagedBuf, ref ModuleInfoHdr moduleInfoHdr)
        {
            ModuleInfoHdrPriv storage = new ModuleInfoHdrPriv();
            unmanagedBuf = ReadDataFromUnmanagedIncPtr(unmanagedBuf, ref storage);
            moduleInfoHdr.Name = Utf8FixedSizeByteArrayToString(storage.Name);
            moduleInfoHdr.Version = storage.Version;
            moduleInfoHdr.Attribute = storage.Attribute;
            moduleInfoHdr.StartEntry = storage.StartEntry;
            moduleInfoHdr.StopEntry = storage.StopEntry;
            moduleInfoHdr.ELFName = Utf8FixedSizeByteArrayToString(storage.ELFName);
            moduleInfoHdr.NumSegments = storage.NumSegments;
            return unmanagedBuf;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetModuleInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetModuleInfoX86(int target, uint processId, uint moduleId, ref ulong bufferSize, IntPtr moduleInfo);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetModuleInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetModuleInfoX64(int target, uint processId, uint moduleId, ref ulong bufferSize, IntPtr moduleInfo);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="processID"></param>
        /// <param name="moduleID"></param>
        /// <param name="moduleInfo"></param>
        /// <returns></returns>
        public static SNRESULT GetModuleInfo(int target, uint processID, uint moduleID, out ModuleInfo moduleInfo)
        {
            moduleInfo = new ModuleInfo();
            ulong bufferSize = 0;
            SNRESULT res1 = Is32Bit() ? GetModuleInfoX86(target, processID, moduleID, ref bufferSize, IntPtr.Zero) : GetModuleInfoX64(target, processID, moduleID, ref bufferSize, IntPtr.Zero);
            if (FAILED(res1))
                return res1;
            if (bufferSize > int.MaxValue)
                return SNRESULT.SN_E_ERROR;
            ScopedGlobalHeapPtr scopedGlobalHeapPtr = new ScopedGlobalHeapPtr(Marshal.AllocHGlobal((int)bufferSize));
            SNRESULT res2 = Is32Bit() ? GetModuleInfoX86(target, processID, moduleID, ref bufferSize, scopedGlobalHeapPtr.Get()) : GetModuleInfoX64(target, processID, moduleID, ref bufferSize, scopedGlobalHeapPtr.Get());
            if (FAILED(res2))
                return res2;
            IntPtr unmanagedBuf = ModuleInfoHdrMarshalHelper(scopedGlobalHeapPtr.Get(), ref moduleInfo.Hdr);
            moduleInfo.Segments = new PRXSegment[(int)moduleInfo.Hdr.NumSegments];
            for (int index = 0; index < moduleInfo.Hdr.NumSegments; ++index)
                unmanagedBuf = ReadDataFromUnmanagedIncPtr(unmanagedBuf, ref moduleInfo.Segments[index]);
            return res2;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetModuleInfoEx", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetModuleInfoExX86(int target, uint processId, uint moduleId, ref ulong bufferSize, IntPtr moduleInfoEx, out IntPtr mselfInfo, out ExtraModuleInfo extraModuleInfo);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetModuleInfoEx", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetModuleInfoExX64(int target, uint processId, uint moduleId, ref ulong bufferSize, IntPtr moduleInfoEx, out IntPtr mselfInfo, out ExtraModuleInfo extraModuleInfo);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="processID"></param>
        /// <param name="moduleID"></param>
        /// <param name="moduleInfoEx"></param>
        /// <param name="mselfInfo"></param>
        /// <param name="extraModuleInfo"></param>
        /// <returns></returns>
        public static SNRESULT GetModuleInfoEx(int target, uint processID, uint moduleID, out ModuleInfoEx moduleInfoEx, out MSELFInfo mselfInfo, out ExtraModuleInfo extraModuleInfo)
        {
            moduleInfoEx = new ModuleInfoEx();
            mselfInfo = new MSELFInfo();
            ulong bufferSize = 0;
            SNRESULT res1 = Is32Bit() ? GetModuleInfoExX86(target, processID, moduleID, ref bufferSize, IntPtr.Zero, out IntPtr mselfInfo1, out extraModuleInfo) : GetModuleInfoExX64(target, processID, moduleID, ref bufferSize, IntPtr.Zero, out mselfInfo1, out extraModuleInfo);
            if (FAILED(res1))
                return res1;
            if (bufferSize > int.MaxValue)
                return SNRESULT.SN_E_ERROR;
            ScopedGlobalHeapPtr scopedGlobalHeapPtr = new ScopedGlobalHeapPtr(Marshal.AllocHGlobal((int)bufferSize));
            SNRESULT res2 = Is32Bit() ? GetModuleInfoExX86(target, processID, moduleID, ref bufferSize, scopedGlobalHeapPtr.Get(), out mselfInfo1, out extraModuleInfo) : GetModuleInfoExX64(target, processID, moduleID, ref bufferSize, scopedGlobalHeapPtr.Get(), out mselfInfo1, out extraModuleInfo);
            if (FAILED(res2))
                return res2;
            IntPtr unmanagedBuf = ModuleInfoHdrMarshalHelper(scopedGlobalHeapPtr.Get(), ref moduleInfoEx.Hdr);
            moduleInfoEx.Segments = new PRXSegmentEx[(int)moduleInfoEx.Hdr.NumSegments];
            for (int index = 0; index < moduleInfoEx.Hdr.NumSegments; ++index)
                unmanagedBuf = ReadDataFromUnmanagedIncPtr(unmanagedBuf, ref moduleInfoEx.Segments[index]);
            ReadDataFromUnmanagedIncPtr(mselfInfo1, ref mselfInfo);
            return res2;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ThreadInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetThreadInfoX86(int target, UnitType unit, uint processId, ulong threadId, ref uint bufferSize, IntPtr buffer);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ThreadInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetThreadInfoX64(int target, UnitType unit, uint processId, ulong threadId, ref uint bufferSize, IntPtr buffer);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="processID"></param>
        /// <param name="threadID"></param>
        /// <param name="threadInfo"></param>
        /// <returns></returns>
        public static SNRESULT GetPPUThreadInfo(int target, uint processID, ulong threadID, out PPUThreadInfo threadInfo)
        {
            threadInfo = new PPUThreadInfo();
            uint bufferSize = 0;
            SNRESULT res1 = Is32Bit() ? GetThreadInfoX86(target, UnitType.PPU, processID, threadID, ref bufferSize, IntPtr.Zero) : GetThreadInfoX64(target, UnitType.PPU, processID, threadID, ref bufferSize, IntPtr.Zero);
            if (FAILED(res1))
                return res1;
            ScopedGlobalHeapPtr scopedGlobalHeapPtr = new ScopedGlobalHeapPtr(Marshal.AllocHGlobal((int)bufferSize));
            SNRESULT res2 = Is32Bit() ? GetThreadInfoX86(target, UnitType.PPU, processID, threadID, ref bufferSize, scopedGlobalHeapPtr.Get()) : GetThreadInfoX64(target, UnitType.PPU, processID, threadID, ref bufferSize, scopedGlobalHeapPtr.Get());
            if (FAILED(res2))
                return res2;
            PPUThreadInfoPriv storage = new PPUThreadInfoPriv();
            IntPtr utf8Ptr = ReadDataFromUnmanagedIncPtr(scopedGlobalHeapPtr.Get(), ref storage);
            threadInfo.ThreadID = storage.ThreadID;
            threadInfo.Priority = storage.Priority;
            threadInfo.State = (PPUThreadState)storage.State;
            threadInfo.StackAddress = storage.StackAddress;
            threadInfo.StackSize = storage.StackSize;
            if (storage.ThreadNameLen > 0U)
                threadInfo.ThreadName = Utf8ToString(utf8Ptr, uint.MaxValue);
            return res2;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3PPUThreadInfoEx", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetPPUThreadInfoExX86(int target, uint processId, ulong threadId, ref uint bufferSize, IntPtr buffer);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3PPUThreadInfoEx", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetPPUThreadInfoExX64(int target, uint processId, ulong threadId, ref uint bufferSize, IntPtr buffer);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="processID"></param>
        /// <param name="threadID"></param>
        /// <param name="threadInfoEx"></param>
        /// <returns></returns>
        public static SNRESULT GetPPUThreadInfoEx(int target, uint processID, ulong threadID, out PPUThreadInfoEx threadInfoEx)
        {
            threadInfoEx = new PPUThreadInfoEx();
            uint bufferSize = 0;
            SNRESULT res1 = Is32Bit() ? GetPPUThreadInfoExX86(target, processID, threadID, ref bufferSize, IntPtr.Zero) : GetPPUThreadInfoExX64(target, processID, threadID, ref bufferSize, IntPtr.Zero);
            if (FAILED(res1))
                return res1;
            ScopedGlobalHeapPtr scopedGlobalHeapPtr = new ScopedGlobalHeapPtr(Marshal.AllocHGlobal((int)bufferSize));
            SNRESULT res2 = Is32Bit() ? GetPPUThreadInfoExX86(target, processID, threadID, ref bufferSize, scopedGlobalHeapPtr.Get()) : GetPPUThreadInfoExX64(target, processID, threadID, ref bufferSize, scopedGlobalHeapPtr.Get());
            if (FAILED(res2))
                return res2;
            PPUThreadInfoExPriv storage = new PPUThreadInfoExPriv();
            IntPtr utf8Ptr = ReadDataFromUnmanagedIncPtr(scopedGlobalHeapPtr.Get(), ref storage);
            threadInfoEx.ThreadID = storage.ThreadId;
            threadInfoEx.Priority = storage.Priority;
            threadInfoEx.BasePriority = storage.BasePriority;
            threadInfoEx.State = (PPUThreadState)storage.State;
            threadInfoEx.StackAddress = storage.StackAddress;
            threadInfoEx.StackSize = storage.StackSize;
            if (storage.ThreadNameLen > 0U)
                threadInfoEx.ThreadName = Utf8ToString(utf8Ptr, uint.MaxValue);
            return res2;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="processID"></param>
        /// <param name="threadID"></param>
        /// <param name="threadInfo"></param>
        /// <returns></returns>
        public static SNRESULT GetSPUThreadInfo(int target, uint processID, ulong threadID, out SPUThreadInfo threadInfo)
        {
            threadInfo = new SPUThreadInfo();
            uint bufferSize = 0;
            SNRESULT res1 = Is32Bit() ? GetThreadInfoX86(target, UnitType.SPU, processID, threadID, ref bufferSize, IntPtr.Zero) : GetThreadInfoX64(target, UnitType.SPU, processID, threadID, ref bufferSize, IntPtr.Zero);
            if (FAILED(res1))
                return res1;
            ScopedGlobalHeapPtr scopedGlobalHeapPtr = new ScopedGlobalHeapPtr(Marshal.AllocHGlobal((int)bufferSize));
            SNRESULT res2 = Is32Bit() ? GetThreadInfoX86(target, UnitType.SPU, processID, threadID, ref bufferSize, scopedGlobalHeapPtr.Get()) : GetThreadInfoX64(target, UnitType.SPU, processID, threadID, ref bufferSize, scopedGlobalHeapPtr.Get());
            if (FAILED(res2))
                return res2;
            SpuThreadInfoPriv storage = new SpuThreadInfoPriv();
            IntPtr utf8Ptr1 = ReadDataFromUnmanagedIncPtr(scopedGlobalHeapPtr.Get(), ref storage);
            threadInfo.ThreadGroupID = storage.ThreadGroupId;
            threadInfo.ThreadID = storage.ThreadId;
            if (storage.FilenameLen > 0U)
                threadInfo.Filename = Utf8ToString(utf8Ptr1, uint.MaxValue);
            if (storage.ThreadNameLen > 0U)
            {
                IntPtr utf8Ptr2 = new IntPtr(utf8Ptr1.ToInt64() + storage.FilenameLen);
                threadInfo.ThreadName = Utf8ToString(utf8Ptr2, uint.MaxValue);
            }
            return res2;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SetDefaultPPUThreadStackSize", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetDefaultPPUThreadStackSizeX86(int target, ELFStackSize size);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SetDefaultPPUThreadStackSize", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetDefaultPPUThreadStackSizeX64(int target, ELFStackSize size);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="stackSize"></param>
        /// <returns></returns>
        public static SNRESULT SetDefaultPPUThreadStackSize(int target, ELFStackSize stackSize)
        {
            if (!Is32Bit())
                return SetDefaultPPUThreadStackSizeX64(target, stackSize);
            return SetDefaultPPUThreadStackSizeX86(target, stackSize);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetDefaultPPUThreadStackSize", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetDefaultPPUThreadStackSizeX86(int target, out ELFStackSize size);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetDefaultPPUThreadStackSize", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetDefaultPPUThreadStackSizeX64(int target, out ELFStackSize size);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="stackSize"></param>
        /// <returns></returns>
        public static SNRESULT GetDefaultPPUThreadStackSize(int target, out ELFStackSize stackSize)
        {
            if (!Is32Bit())
                return GetDefaultPPUThreadStackSizeX64(target, out stackSize);
            return GetDefaultPPUThreadStackSizeX86(target, out stackSize);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SetSPULoopPoint", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetSPULoopPointX86(int target, uint processId, ulong threadId, uint address, int bCurrentPc);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SetSPULoopPoint", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetSPULoopPointX64(int target, uint processId, ulong threadId, uint address, int bCurrentPc);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="processID"></param>
        /// <param name="threadID"></param>
        /// <param name="address"></param>
        /// <param name="bCurrentPC"></param>
        /// <returns></returns>
        public static SNRESULT SetSPULoopPoint(int target, uint processID, ulong threadID, uint address, bool bCurrentPC)
        {
            int bCurrentPc = bCurrentPC ? 1 : 0;
            if (!Is32Bit())
                return SetSPULoopPointX64(target, processID, threadID, address, bCurrentPc);
            return SetSPULoopPointX86(target, processID, threadID, address, bCurrentPc);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ClearSPULoopPoint", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ClearSPULoopPointX86(int target, uint processId, ulong threadId, uint address, bool bCurrentPc);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ClearSPULoopPoint", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ClearSPULoopPointX64(int target, uint processId, ulong threadId, uint address, bool bCurrentPc);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="processID"></param>
        /// <param name="threadID"></param>
        /// <param name="address"></param>
        /// <param name="bCurrentPC"></param>
        /// <returns></returns>
        public static SNRESULT ClearSPULoopPoint(int target, uint processID, ulong threadID, uint address, bool bCurrentPC)
        {
            if (!Is32Bit())
                return ClearSPULoopPointX64(target, processID, threadID, address, bCurrentPC);
            return ClearSPULoopPointX86(target, processID, threadID, address, bCurrentPC);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SetBreakPoint", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetBreakPointX86(int target, uint unit, uint processId, ulong threadId, ulong address);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SetBreakPoint", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetBreakPointX64(int target, uint unit, uint processId, ulong threadId, ulong address);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="unit"></param>
        /// <param name="processID"></param>
        /// <param name="threadID"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        public static SNRESULT SetBreakPoint(int target, UnitType unit, uint processID, ulong threadID, ulong address)
        {
            if (!Is32Bit())
                return SetBreakPointX64(target, (uint)unit, processID, threadID, address);
            return SetBreakPointX86(target, (uint)unit, processID, threadID, address);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ClearBreakPoint", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ClearBreakPointX86(int target, uint unit, uint processId, ulong threadId, ulong address);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ClearBreakPoint", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ClearBreakPointX64(int target, uint unit, uint processId, ulong threadId, ulong address);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="unit"></param>
        /// <param name="processID"></param>
        /// <param name="threadID"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        public static SNRESULT ClearBreakPoint(int target, UnitType unit, uint processID, ulong threadID, ulong address)
        {
            if (!Is32Bit())
                return ClearBreakPointX64(target, (uint)unit, processID, threadID, address);
            return ClearBreakPointX86(target, (uint)unit, processID, threadID, address);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetBreakPoints", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetBreakPointsX86(int target, uint unit, uint processId, ulong threadId, out uint numBreakpoints, ulong[] addresses);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetBreakPoints", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetBreakPointsX64(int target, uint unit, uint processId, ulong threadId, out uint numBreakpoints, ulong[] addresses);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="unit"></param>
        /// <param name="processID"></param>
        /// <param name="threadID"></param>
        /// <param name="bpAddresses"></param>
        /// <returns></returns>
        public static SNRESULT GetBreakPoints(int target, UnitType unit, uint processID, ulong threadID, out ulong[] bpAddresses)
        {
            bpAddresses = null;
            SNRESULT res = Is32Bit() ? GetBreakPointsX86(target, (uint)unit, processID, threadID, out uint numBreakpoints, null) : GetBreakPointsX64(target, (uint)unit, processID, threadID, out numBreakpoints, null);
            if (FAILED(res))
                return res;
            bpAddresses = new ulong[(int)numBreakpoints];
            if (!Is32Bit())
                return GetBreakPointsX64(target, (uint)unit, processID, threadID, out numBreakpoints, bpAddresses);
            return GetBreakPointsX86(target, (uint)unit, processID, threadID, out numBreakpoints, bpAddresses);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetDebugThreadControlInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetDebugThreadControlInfoX86(int target, uint processId, ref uint bufferSize, IntPtr buffer);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetDebugThreadControlInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetDebugThreadControlInfoX64(int target, uint processId, ref uint bufferSize, IntPtr buffer);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="processID"></param>
        /// <param name="threadCtrlInfo"></param>
        /// <returns></returns>
        public static SNRESULT GetDebugThreadControlInfo(int target, uint processID, out DebugThreadControlInfo threadCtrlInfo)
        {
            threadCtrlInfo = new DebugThreadControlInfo();
            uint bufferSize = 0;
            SNRESULT res1 = Is32Bit() ? GetDebugThreadControlInfoX86(target, processID, ref bufferSize, IntPtr.Zero) : GetDebugThreadControlInfoX64(target, processID, ref bufferSize, IntPtr.Zero);
            if (FAILED(res1))
                return res1;
            ScopedGlobalHeapPtr scopedGlobalHeapPtr = new ScopedGlobalHeapPtr(Marshal.AllocHGlobal((int)bufferSize));
            SNRESULT res2 = Is32Bit() ? GetDebugThreadControlInfoX86(target, processID, ref bufferSize, scopedGlobalHeapPtr.Get()) : GetDebugThreadControlInfoX64(target, processID, ref bufferSize, scopedGlobalHeapPtr.Get());
            if (FAILED(res2))
                return res2;
            DebugThreadControlInfoPriv storage = new DebugThreadControlInfoPriv();
            IntPtr num = ReadDataFromUnmanagedIncPtr(scopedGlobalHeapPtr.Get(), ref storage);
            threadCtrlInfo.ControlFlags = storage.ControlFlags;
            uint numEntries = storage.NumEntries;
            threadCtrlInfo.ControlKeywords = new ControlKeywordEntry[(int)numEntries];
            for (uint index = 0; index < numEntries; ++index)
            {
                num = ReadDataFromUnmanagedIncPtr(num, ref threadCtrlInfo.ControlKeywords[(int)index].MatchConditionFlags);
                threadCtrlInfo.ControlKeywords[(int)index].Keyword = Utf8ToString(num, 128U);
                num = new IntPtr(num.ToInt64() + 128L);
            }
            return res2;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SetDebugThreadControlInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetDebugThreadControlInfoX86(int target, uint processId, IntPtr threadCtrlInfo, out uint maxEntries);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SetDebugThreadControlInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetDebugThreadControlInfoX64(int target, uint processId, IntPtr threadCtrlInfo, out uint maxEntries);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="processID"></param>
        /// <param name="threadCtrlInfo"></param>
        /// <param name="maxEntries"></param>
        /// <returns></returns>
        public static SNRESULT SetDebugThreadControlInfo(int target, uint processID, DebugThreadControlInfo threadCtrlInfo, out uint maxEntries)
        {
            DebugThreadControlInfoPriv storage = new DebugThreadControlInfoPriv();
            storage.ControlFlags = threadCtrlInfo.ControlFlags;
            if (threadCtrlInfo.ControlKeywords != null)
                storage.NumEntries = (uint)threadCtrlInfo.ControlKeywords.Length;
            ScopedGlobalHeapPtr scopedGlobalHeapPtr = new ScopedGlobalHeapPtr(Marshal.AllocHGlobal(Marshal.SizeOf((object)storage) + (int)storage.NumEntries * Marshal.SizeOf(typeof(ControlKeywordEntry))));
            IntPtr unmanagedIncPtr = WriteDataToUnmanagedIncPtr(storage, scopedGlobalHeapPtr.Get());
            for (int index = 0; index < storage.NumEntries; ++index)
                unmanagedIncPtr = WriteDataToUnmanagedIncPtr(threadCtrlInfo.ControlKeywords[index], unmanagedIncPtr);
            if (!Is32Bit())
                return SetDebugThreadControlInfoX64(target, processID, scopedGlobalHeapPtr.Get(), out maxEntries);
            return SetDebugThreadControlInfoX86(target, processID, scopedGlobalHeapPtr.Get(), out maxEntries);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ThreadExceptionClean", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ThreadExceptionCleanX86(int target, uint processId, ulong threadId);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ThreadExceptionClean", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ThreadExceptionCleanX64(int target, uint processId, ulong threadId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="processID"></param>
        /// <param name="threadID"></param>
        /// <returns></returns>
        public static SNRESULT ThreadExceptionClean(int target, uint processID, ulong threadID)
        {
            return Is32Bit() ? ThreadExceptionCleanX86(target, processID, threadID) : ThreadExceptionCleanX64(target, processID, threadID);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetRawSPULogicalIDs", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetRawSPULogicalIdsX86(int target, uint processId, ulong[] logicalIds);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetRawSPULogicalIDs", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetRawSPULogicalIdsX64(int target, uint processId, ulong[] logicalIds);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="processID"></param>
        /// <param name="logicalIDs"></param>
        /// <returns></returns>
        public static SNRESULT GetRawSPULogicalIDs(int target, uint processID, out ulong[] logicalIDs)
        {
            logicalIDs = new ulong[8];
            if (!Is32Bit())
                return GetRawSPULogicalIdsX64(target, processID, logicalIDs);
            return GetRawSPULogicalIdsX86(target, processID, logicalIDs);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SPUThreadGroupStop", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SPUThreadGroupStopX86(int target, uint processId, ulong threadGroupId);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SPUThreadGroupStop", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SPUThreadGroupStopX64(int target, uint processId, ulong threadGroupId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="processID"></param>
        /// <param name="threadGroupID"></param>
        /// <returns></returns>
        public static SNRESULT SPUThreadGroupStop(int target, uint processID, ulong threadGroupID)
        {
            if (!Is32Bit())
                return SPUThreadGroupStopX64(target, processID, threadGroupID);
            return SPUThreadGroupStopX86(target, processID, threadGroupID);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SPUThreadGroupContinue", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SPUThreadGroupContinueX86(int target, uint processId, ulong threadGroupId);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SPUThreadGroupContinue", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SPUThreadGroupContinueX64(int target, uint processId, ulong threadGroupId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="processID"></param>
        /// <param name="threadGroupID"></param>
        /// <returns></returns>
        public static SNRESULT SPUThreadGroupContinue(int target, uint processID, ulong threadGroupID)
        {
            if (!Is32Bit())
                return SPUThreadGroupContinueX64(target, processID, threadGroupID);
            return SPUThreadGroupContinueX86(target, processID, threadGroupID);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetProcessTree", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetProcessTreeX86(int target, ref uint numProcesses, IntPtr buffer);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetProcessTree", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetProcessTreeX64(int target, ref uint numProcesses, IntPtr buffer);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="processTree"></param>
        /// <returns></returns>
        public static SNRESULT GetProcessTree(int target, out ProcessTreeBranch[] processTree)
        {
            processTree = null;
            uint numProcesses = 0;
            SNRESULT res1 = Is32Bit() ? GetProcessTreeX86(target, ref numProcesses, IntPtr.Zero) : GetProcessTreeX64(target, ref numProcesses, IntPtr.Zero);
            if (FAILED(res1))
                return res1;
            ScopedGlobalHeapPtr scopedGlobalHeapPtr = new ScopedGlobalHeapPtr(Marshal.AllocHGlobal((int)numProcesses * Marshal.SizeOf(typeof(ProcessTreeBranchPriv))));
            SNRESULT res2 = Is32Bit() ? GetProcessTreeX86(target, ref numProcesses, scopedGlobalHeapPtr.Get()) : GetProcessTreeX64(target, ref numProcesses, scopedGlobalHeapPtr.Get());
            if (FAILED(res2))
                return res2;
            processTree = new ProcessTreeBranch[(int)numProcesses];
            for (int index1 = 0; index1 < numProcesses; ++index1)
            {
                ProcessTreeBranchPriv structure = (ProcessTreeBranchPriv)Marshal.PtrToStructure(scopedGlobalHeapPtr.Get(), typeof(ProcessTreeBranchPriv));
                processTree[index1].ProcessID = structure.ProcessId;
                processTree[index1].ProcessState = structure.ProcessState;
                processTree[index1].ProcessFlags = structure.ProcessFlags;
                processTree[index1].RawSPU = structure.RawSPU;
                processTree[index1].PPUThreadStatuses = new PPUThreadStatus[(int)structure.NumPpuThreads];
                processTree[index1].SPUThreadGroupStatuses = new SPUThreadGroupStatus[(int)structure.NumSpuThreadGroups];
                for (int index2 = 0; index2 < structure.NumPpuThreads; ++index2)
                    structure.PpuThreadStatuses = ReadDataFromUnmanagedIncPtr(structure.PpuThreadStatuses, ref processTree[index1].PPUThreadStatuses[index2]);
                for (int index2 = 0; index2 < structure.NumSpuThreadGroups; ++index2)
                    structure.SpuThreadGroupStatuses = ReadDataFromUnmanagedIncPtr(structure.SpuThreadGroupStatuses, ref processTree[index1].SPUThreadGroupStatuses[index2]);
            }
            return res2;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetSPUThreadGroupInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetSPUThreadGroupInfoX86(int target, uint processId, ulong threadGroupId, ref uint bufferSize, IntPtr buffer);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetSPUThreadGroupInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetSPUThreadGroupInfoX64(int target, uint processId, ulong threadGroupId, ref uint bufferSize, IntPtr buffer);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="processID"></param>
        /// <param name="threadGroupID"></param>
        /// <param name="threadGroupInfo"></param>
        /// <returns></returns>
        public static SNRESULT GetSPUThreadGroupInfo(int target, uint processID, ulong threadGroupID, out SPUThreadGroupInfo threadGroupInfo)
        {
            threadGroupInfo = new SPUThreadGroupInfo();
            uint bufferSize = 0;
            SNRESULT res1 = Is32Bit() ? GetSPUThreadGroupInfoX86(target, processID, threadGroupID, ref bufferSize, IntPtr.Zero) : GetSPUThreadGroupInfoX64(target, processID, threadGroupID, ref bufferSize, IntPtr.Zero);
            if (FAILED(res1))
                return res1;
            ScopedGlobalHeapPtr scopedGlobalHeapPtr = new ScopedGlobalHeapPtr(Marshal.AllocHGlobal((int)bufferSize));
            SNRESULT res2 = Is32Bit() ? GetSPUThreadGroupInfoX86(target, processID, threadGroupID, ref bufferSize, scopedGlobalHeapPtr.Get()) : GetSPUThreadGroupInfoX64(target, processID, threadGroupID, ref bufferSize, scopedGlobalHeapPtr.Get());
            if (FAILED(res2))
                return res2;
            SpuThreadGroupInfoPriv storage = new SpuThreadGroupInfoPriv();
            IntPtr num = ReadDataFromUnmanagedIncPtr(scopedGlobalHeapPtr.Get(), ref storage);
            threadGroupInfo.ThreadGroupID = storage.ThreadGroupId;
            threadGroupInfo.State = (SPUThreadGroupState)storage.State;
            threadGroupInfo.Priority = storage.Priority;
            threadGroupInfo.ThreadIDs = new uint[(int)storage.NumThreads];
            for (int index = 0; index < storage.NumThreads; ++index)
                num = ReadDataFromUnmanagedIncPtr(num, ref threadGroupInfo.ThreadIDs[index]);
            if (storage.ThreadGroupNameLen > 0U)
                threadGroupInfo.GroupName = Utf8ToString(num, uint.MaxValue);
            return res2;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ProcessGetMemory", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ProcessGetMemoryX86(int target, UnitType unit, uint processId, ulong threadId, ulong address, int count, byte[] buffer);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ProcessGetMemory", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ProcessGetMemoryX64(int target, UnitType unit, uint processId, ulong threadId, ulong address, int count, byte[] buffer);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="unit"></param>
        /// <param name="processID"></param>
        /// <param name="threadID"></param>
        /// <param name="address"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static SNRESULT ProcessGetMemory(int target, UnitType unit, uint processID, ulong threadID, ulong address, ref byte[] buffer)
        {
            if (!Is32Bit())
                return ProcessGetMemoryX64(target, unit, processID, threadID, address, buffer.Length, buffer);
            return ProcessGetMemoryX86(target, unit, processID, threadID, address, buffer.Length, buffer);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ProcessSetMemory", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ProcessSetMemoryX86(int target, UnitType unit, uint processId, ulong threadId, ulong address, int count, byte[] buffer);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ProcessSetMemory", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ProcessSetMemoryX64(int target, UnitType unit, uint processId, ulong threadId, ulong address, int count, byte[] buffer);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="unit"></param>
        /// <param name="processID"></param>
        /// <param name="threadID"></param>
        /// <param name="address"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static SNRESULT ProcessSetMemory(int target, UnitType unit, uint processID, ulong threadID, ulong address, byte[] buffer)
        {
            if (!Is32Bit())
                return ProcessSetMemoryX64(target, unit, processID, threadID, address, buffer.Length, buffer);
            return ProcessSetMemoryX86(target, unit, processID, threadID, address, buffer.Length, buffer);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetMemoryCompressed", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetMemoryCompressedX86(int target, uint processId, uint compressionLevel, uint address, uint size, byte[] buffer);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetMemoryCompressed", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetMemoryCompressedX64(int target, uint processId, uint compressionLevel, uint address, uint size, byte[] buffer);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="processID"></param>
        /// <param name="compressionLevel"></param>
        /// <param name="address"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static SNRESULT GetMemoryCompressed(int target, uint processID, MemoryCompressionLevel compressionLevel, uint address, ref byte[] buffer)
        {
            if (!Is32Bit())
                return GetMemoryCompressedX64(target, processID, (uint)compressionLevel, address, (uint)buffer.Length, buffer);
            return GetMemoryCompressedX86(target, processID, (uint)compressionLevel, address, (uint)buffer.Length, buffer);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetMemory64Compressed", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetMemory64CompressedX86(int target, uint processId, uint compressionLevel, ulong address, uint size, byte[] buffer);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetMemory64Compressed", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetMemory64CompressedX64(int target, uint processId, uint compressionLevel, ulong address, uint size, byte[] buffer);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="processID"></param>
        /// <param name="compressionLevel"></param>
        /// <param name="address"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static SNRESULT GetMemory64Compressed(int target, uint processID, MemoryCompressionLevel compressionLevel, ulong address, ref byte[] buffer)
        {
            if (!Is32Bit())
                return GetMemory64CompressedX64(target, processID, (uint)compressionLevel, address, (uint)buffer.Length, buffer);
            return GetMemory64CompressedX86(target, processID, (uint)compressionLevel, address, (uint)buffer.Length, buffer);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetVirtualMemoryInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetVirtualMemoryInfoX86(int target, uint processId, bool bStatsOnly, out uint areaCount, out uint bufferSize, IntPtr buffer);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetVirtualMemoryInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetVirtualMemoryInfoX64(int target, uint processId, bool bStatsOnly, out uint areaCount, out uint bufferSize, IntPtr buffer);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="processID"></param>
        /// <param name="bStatsOnly"></param>
        /// <param name="vmAreas"></param>
        /// <returns></returns>
        public static SNRESULT GetVirtualMemoryInfo(int target, uint processID, bool bStatsOnly, out VirtualMemoryArea[] vmAreas)
        {
            vmAreas = null;
            SNRESULT res1 = Is32Bit() ? GetVirtualMemoryInfoX86(target, processID, bStatsOnly, out uint areaCount, out uint bufferSize, IntPtr.Zero) : GetVirtualMemoryInfoX64(target, processID, bStatsOnly, out areaCount, out bufferSize, IntPtr.Zero);
            if (FAILED(res1))
                return res1;
            ScopedGlobalHeapPtr scopedGlobalHeapPtr = new ScopedGlobalHeapPtr(Marshal.AllocHGlobal((int)bufferSize));
            SNRESULT res2 = Is32Bit() ? GetVirtualMemoryInfoX86(target, processID, bStatsOnly, out areaCount, out bufferSize, scopedGlobalHeapPtr.Get()) : GetVirtualMemoryInfoX64(target, processID, bStatsOnly, out areaCount, out bufferSize, scopedGlobalHeapPtr.Get());
            if (FAILED(res2))
                return res2;
            vmAreas = new VirtualMemoryArea[(int)areaCount];
            IntPtr unmanagedBuf1 = scopedGlobalHeapPtr.Get();
            for (int index = 0; index < areaCount; ++index)
            {
                IntPtr unmanagedBuf2 = ReadDataFromUnmanagedIncPtr(ReadDataFromUnmanagedIncPtr(ReadDataFromUnmanagedIncPtr(ReadDataFromUnmanagedIncPtr(ReadDataFromUnmanagedIncPtr(ReadDataFromUnmanagedIncPtr(ReadDataFromUnmanagedIncPtr(ReadDataFromUnmanagedIncPtr(ReadDataFromUnmanagedIncPtr(ReadDataFromUnmanagedIncPtr(ReadDataFromUnmanagedIncPtr(unmanagedBuf1, ref vmAreas[index].Address), ref vmAreas[index].Flags), ref vmAreas[index].VSize), ref vmAreas[index].Options), ref vmAreas[index].PageFaultPPU), ref vmAreas[index].PageFaultSPU), ref vmAreas[index].PageIn), ref vmAreas[index].PageOut), ref vmAreas[index].PMemTotal), ref vmAreas[index].PMemUsed), ref vmAreas[index].Time);
                ulong storage = 0;
                IntPtr unmanagedBuf3 = ReadDataFromUnmanagedIncPtr(unmanagedBuf2, ref storage);
                vmAreas[index].Pages = new ulong[storage];
                IntPtr zero = IntPtr.Zero;
                unmanagedBuf1 = ReadDataFromUnmanagedIncPtr(unmanagedBuf3, ref zero);
            }
            for (int index1 = 0; index1 < areaCount; ++index1)
            {
                int length = vmAreas[index1].Pages.Length;
                for (int index2 = 0; index2 < length; ++index2)
                    unmanagedBuf1 = ReadDataFromUnmanagedIncPtr(unmanagedBuf1, ref vmAreas[index1].Pages[index2]);
            }
            return res2;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetSyncPrimitiveCountsEx", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetSyncPrimitiveCountsExX86(int target, uint processId, ref uint bufferSize, IntPtr buffer);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetSyncPrimitiveCountsEx", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetSyncPrimitiveCountsExX64(int target, uint processId, ref uint bufferSize, IntPtr buffer);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="processID"></param>
        /// <param name="primitiveCounts"></param>
        /// <returns></returns>
        public static SNRESULT GetSyncPrimitiveCounts(int target, uint processID, out SyncPrimitiveCounts primitiveCounts)
        {
            primitiveCounts = new SyncPrimitiveCounts();
            uint bufferSize = 32;
            ScopedGlobalHeapPtr scopedGlobalHeapPtr = new ScopedGlobalHeapPtr(Marshal.AllocHGlobal((int)bufferSize));
            SNRESULT res = Is32Bit() ? GetSyncPrimitiveCountsExX86(target, processID, ref bufferSize, scopedGlobalHeapPtr.Get()) : GetSyncPrimitiveCountsExX64(target, processID, ref bufferSize, scopedGlobalHeapPtr.Get());
            if (FAILED(res))
                return res;
            primitiveCounts = (SyncPrimitiveCounts)Marshal.PtrToStructure(scopedGlobalHeapPtr.Get(), typeof(SyncPrimitiveCounts));
            return res;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetMutexList", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetMutexListX86(int target, uint processId, ref uint numMutexes, uint[] mutexList);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetMutexList", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetMutexListX64(int target, uint processId, ref uint numMutexes, uint[] mutexList);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="processID"></param>
        /// <param name="mutexIDs"></param>
        /// <returns></returns>
        public static SNRESULT GetMutexList(int target, uint processID, out uint[] mutexIDs)
        {
            mutexIDs = null;
            uint numMutexes = 0;
            SNRESULT res = Is32Bit() ? GetMutexListX86(target, processID, ref numMutexes, null) : GetMutexListX64(target, processID, ref numMutexes, null);
            if (FAILED(res))
                return res;
            mutexIDs = new uint[(int)numMutexes];
            return Is32Bit() ? GetMutexListX86(target, processID, ref numMutexes, mutexIDs) : GetMutexListX64(target, processID, ref numMutexes, mutexIDs);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetMutexInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetMutexInfoX86(int target, uint processId, uint mutexId, ref uint bufferSize, IntPtr buffer);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetMutexInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetMutexInfoX64(int target, uint processId, uint mutexId, ref uint bufferSize, IntPtr buffer);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="processID"></param>
        /// <param name="mutexID"></param>
        /// <param name="mutexInfo"></param>
        /// <returns></returns>
        public static SNRESULT GetMutexInfo(int target, uint processID, uint mutexID, out MutexInfo mutexInfo)
        {
            mutexInfo = new MutexInfo();
            uint bufferSize = 0;
            SNRESULT res1 = Is32Bit() ? GetMutexInfoX86(target, processID, mutexID, ref bufferSize, IntPtr.Zero) : GetMutexInfoX64(target, processID, mutexID, ref bufferSize, IntPtr.Zero);
            if (FAILED(res1))
                return res1;
            ScopedGlobalHeapPtr scopedGlobalHeapPtr = new ScopedGlobalHeapPtr(Marshal.AllocHGlobal((int)bufferSize));
            SNRESULT res2 = Is32Bit() ? GetMutexInfoX86(target, processID, mutexID, ref bufferSize, scopedGlobalHeapPtr.Get()) : GetMutexInfoX64(target, processID, mutexID, ref bufferSize, scopedGlobalHeapPtr.Get());
            if (FAILED(res2))
                return res2;
            IntPtr num = ReadDataFromUnmanagedIncPtr(ReadDataFromUnmanagedIncPtr(ReadDataFromUnmanagedIncPtr(ReadDataFromUnmanagedIncPtr(ReadDataFromUnmanagedIncPtr(ReadDataFromUnmanagedIncPtr(ReadDataFromUnmanagedIncPtr(scopedGlobalHeapPtr.Get(), ref mutexInfo.ID), ref mutexInfo.Attribute.Protocol), ref mutexInfo.Attribute.Recursive), ref mutexInfo.Attribute.PShared), ref mutexInfo.Attribute.Adaptive), ref mutexInfo.Attribute.Key), ref mutexInfo.Attribute.Flags);
            mutexInfo.Attribute.Name = Utf8ToString(num, 8U);
            num = new IntPtr(num.ToInt64() + 8L);
            num = ReadDataFromUnmanagedIncPtr(num, ref mutexInfo.OwnerThreadID);
            num = ReadDataFromUnmanagedIncPtr(num, ref mutexInfo.LockCounter);
            num = ReadDataFromUnmanagedIncPtr(num, ref mutexInfo.ConditionRefCounter);
            num = ReadDataFromUnmanagedIncPtr(num, ref mutexInfo.ConditionVarID);
            uint storage = 0;
            num = ReadDataFromUnmanagedIncPtr(num, ref storage);
            num = ReadDataFromUnmanagedIncPtr(num, ref mutexInfo.NumWaitAllThreads);
            mutexInfo.WaitingThreads = new ulong[(int)storage];
            for (int index = 0; index < storage; ++index)
                num = ReadDataFromUnmanagedIncPtr(num, ref mutexInfo.WaitingThreads[index]);
            return res2;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetLightWeightMutexList", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetLightWeightMutexListX86(int target, uint processId, ref uint numLWMutexes, uint[] lwMutexList);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetLightWeightMutexList", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetLightWeightMutexListX64(int target, uint processId, ref uint numLWMutexes, uint[] lwMutexList);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="processID"></param>
        /// <param name="lwMutexIDs"></param>
        /// <returns></returns>
        public static SNRESULT GetLightWeightMutexList(int target, uint processID, out uint[] lwMutexIDs)
        {
            lwMutexIDs = null;
            uint numLWMutexes = 0;
            SNRESULT res = Is32Bit() ? GetLightWeightMutexListX86(target, processID, ref numLWMutexes, null) : GetLightWeightMutexListX64(target, processID, ref numLWMutexes, null);
            if (FAILED(res))
                return res;
            lwMutexIDs = new uint[(int)numLWMutexes];
            return Is32Bit() ? GetLightWeightMutexListX86(target, processID, ref numLWMutexes, lwMutexIDs) : GetLightWeightMutexListX64(target, processID, ref numLWMutexes, lwMutexIDs);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetLightWeightMutexInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetLightWeightMutexInfoX86(int target, uint processId, uint lwMutexId, ref uint bufferSize, IntPtr buffer);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetLightWeightMutexInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetLightWeightMutexInfoX64(int target, uint processId, uint lwMutexId, ref uint bufferSize, IntPtr buffer);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="processID"></param>
        /// <param name="lwMutexID"></param>
        /// <param name="lwMutexInfo"></param>
        /// <returns></returns>
        public static SNRESULT GetLightWeightMutexInfo(int target, uint processID, uint lwMutexID, out LWMutexInfo lwMutexInfo)
        {
            lwMutexInfo = new LWMutexInfo();
            uint bufferSize = 0;
            SNRESULT res1 = Is32Bit() ? GetLightWeightMutexInfoX86(target, processID, lwMutexID, ref bufferSize, IntPtr.Zero) : GetLightWeightMutexInfoX64(target, processID, lwMutexID, ref bufferSize, IntPtr.Zero);
            if (FAILED(res1))
                return res1;
            ScopedGlobalHeapPtr scopedGlobalHeapPtr = new ScopedGlobalHeapPtr(Marshal.AllocHGlobal((int)bufferSize));
            SNRESULT res2 = Is32Bit() ? GetLightWeightMutexInfoX86(target, processID, lwMutexID, ref bufferSize, scopedGlobalHeapPtr.Get()) : GetLightWeightMutexInfoX64(target, processID, lwMutexID, ref bufferSize, scopedGlobalHeapPtr.Get());
            if (FAILED(res2))
                return res2;
            LwMutexInfoPriv storage = new LwMutexInfoPriv();
            IntPtr unmanagedBuf = ReadDataFromUnmanagedIncPtr(scopedGlobalHeapPtr.Get(), ref storage);
            lwMutexInfo.ID = storage.Id;
            lwMutexInfo.Attribute = storage.Attribute;
            lwMutexInfo.OwnerThreadID = storage.OwnerThreadId;
            lwMutexInfo.LockCounter = storage.LockCounter;
            lwMutexInfo.NumWaitAllThreads = storage.NumWaitAllThreads;
            lwMutexInfo.WaitingThreads = new ulong[(int)storage.NumWaitingThreads];
            for (int index = 0; index < storage.NumWaitingThreads; ++index)
                unmanagedBuf = ReadDataFromUnmanagedIncPtr(unmanagedBuf, ref lwMutexInfo.WaitingThreads[index]);
            return res2;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetConditionalVariableList", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetConditionalVariableListX86(int target, uint processId, ref uint numConditionVars, uint[] conditionVarList);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetConditionalVariableList", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetConditionalVariableListX64(int target, uint processId, ref uint numConditionVars, uint[] conditionVarList);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="processID"></param>
        /// <param name="conditionVarIDs"></param>
        /// <returns></returns>
        public static SNRESULT GetConditionalVariableList(int target, uint processID, out uint[] conditionVarIDs)
        {
            conditionVarIDs = null;
            uint numConditionVars = 0;
            SNRESULT res = Is32Bit() ? GetConditionalVariableListX86(target, processID, ref numConditionVars, null) : GetConditionalVariableListX64(target, processID, ref numConditionVars, null);
            if (FAILED(res))
                return res;
            conditionVarIDs = new uint[(int)numConditionVars];
            return Is32Bit() ? GetConditionalVariableListX86(target, processID, ref numConditionVars, conditionVarIDs) : GetConditionalVariableListX64(target, processID, ref numConditionVars, conditionVarIDs);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetConditionalVariableInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetConditionalVariableInfoX86(int target, uint processId, uint conditionVarId, ref uint bufferSize, IntPtr buffer);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetConditionalVariableInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetConditionalVariableInfoX64(int target, uint processId, uint conditionVarId, ref uint bufferSize, IntPtr buffer);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="processID"></param>
        /// <param name="conditionVarID"></param>
        /// <param name="conditionVarInfo"></param>
        /// <returns></returns>
        public static SNRESULT GetConditionalVariableInfo(int target, uint processID, uint conditionVarID, out ConditionVarInfo conditionVarInfo)
        {
            conditionVarInfo = new ConditionVarInfo();
            uint bufferSize = 0;
            SNRESULT res1 = Is32Bit() ? GetConditionalVariableInfoX86(target, processID, conditionVarID, ref bufferSize, IntPtr.Zero) : GetConditionalVariableInfoX64(target, processID, conditionVarID, ref bufferSize, IntPtr.Zero);
            if (FAILED(res1))
                return res1;
            ScopedGlobalHeapPtr scopedGlobalHeapPtr = new ScopedGlobalHeapPtr(Marshal.AllocHGlobal((int)bufferSize));
            SNRESULT res2 = Is32Bit() ? GetConditionalVariableInfoX86(target, processID, conditionVarID, ref bufferSize, scopedGlobalHeapPtr.Get()) : GetConditionalVariableInfoX64(target, processID, conditionVarID, ref bufferSize, scopedGlobalHeapPtr.Get());
            if (FAILED(res2))
                return res2;
            ConditionVarInfoPriv storage = new ConditionVarInfoPriv();
            IntPtr unmanagedBuf = ReadDataFromUnmanagedIncPtr(scopedGlobalHeapPtr.Get(), ref storage);
            conditionVarInfo.ID = storage.Id;
            conditionVarInfo.Attribute = storage.Attribute;
            conditionVarInfo.MutexID = storage.MutexId;
            conditionVarInfo.NumWaitAllThreads = storage.NumWaitAllThreads;
            conditionVarInfo.WaitingThreads = new ulong[(int)storage.NumWaitingThreads];
            for (int index = 0; index < storage.NumWaitingThreads; ++index)
                unmanagedBuf = ReadDataFromUnmanagedIncPtr(unmanagedBuf, ref conditionVarInfo.WaitingThreads[index]);
            return res2;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetLightWeightConditionalList", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetLightWeightConditionalListX86(int target, uint processId, ref uint numLWConditionVars, uint[] lwConditionVarList);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetLightWeightConditionalList", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetLightWeightConditionalListX64(int target, uint processId, ref uint numLWConditionVars, uint[] lwConditionVarList);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="processID"></param>
        /// <param name="lwConditionVarIDs"></param>
        /// <returns></returns>
        public static SNRESULT GetLightWeightConditionalList(int target, uint processID, out uint[] lwConditionVarIDs)
        {
            lwConditionVarIDs = null;
            uint numLWConditionVars = 0;
            SNRESULT res = Is32Bit() ? GetLightWeightConditionalListX86(target, processID, ref numLWConditionVars, null) : GetLightWeightConditionalListX64(target, processID, ref numLWConditionVars, null);
            if (FAILED(res))
                return res;
            lwConditionVarIDs = new uint[(int)numLWConditionVars];
            return Is32Bit() ? GetLightWeightConditionalListX86(target, processID, ref numLWConditionVars, lwConditionVarIDs) : GetLightWeightConditionalListX64(target, processID, ref numLWConditionVars, lwConditionVarIDs);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetLightWeightConditionalInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetLightWeightConditionalInfoX86(int target, uint processId, uint lwCondVarId, ref uint bufferSize, IntPtr buffer);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetLightWeightConditionalInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetLightWeightConditionalInfoX64(int target, uint processId, uint lwCondVarId, ref uint bufferSize, IntPtr buffer);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="processID"></param>
        /// <param name="lwCondVarID"></param>
        /// <param name="lwConditonVarInfo"></param>
        /// <returns></returns>
        public static SNRESULT GetLightWeightConditionalInfo(int target, uint processID, uint lwCondVarID, out LWConditionVarInfo lwConditonVarInfo)
        {
            lwConditonVarInfo = new LWConditionVarInfo();
            uint bufferSize = 0;
            SNRESULT res1 = Is32Bit() ? GetLightWeightConditionalInfoX86(target, processID, lwCondVarID, ref bufferSize, IntPtr.Zero) : GetLightWeightConditionalInfoX64(target, processID, lwCondVarID, ref bufferSize, IntPtr.Zero);
            if (FAILED(res1))
                return res1;
            ScopedGlobalHeapPtr scopedGlobalHeapPtr = new ScopedGlobalHeapPtr(Marshal.AllocHGlobal((int)bufferSize));
            SNRESULT res2 = Is32Bit() ? GetLightWeightConditionalInfoX86(target, processID, lwCondVarID, ref bufferSize, scopedGlobalHeapPtr.Get()) : GetLightWeightConditionalInfoX64(target, processID, lwCondVarID, ref bufferSize, scopedGlobalHeapPtr.Get());
            if (FAILED(res2))
                return res2;
            LwConditionVarInfoPriv storage = new LwConditionVarInfoPriv();
            IntPtr unmanagedBuf = ReadDataFromUnmanagedIncPtr(scopedGlobalHeapPtr.Get(), ref storage);
            lwConditonVarInfo = new LWConditionVarInfo
            {
                ID = storage.Id,
                Attribute = storage.Attribute,
                LWMutexID = storage.LwMutexId,
                NumWaitAllThreads = storage.NumWaitAllThreads,
                WaitingThreads = new ulong[(int)storage.NumWaitingThreads]
            };
            for (int index = 0; index < storage.NumWaitingThreads; ++index)
                unmanagedBuf = ReadDataFromUnmanagedIncPtr(unmanagedBuf, ref lwConditonVarInfo.WaitingThreads[index]);
            return res2;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetReadWriteLockList", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetReadWriteLockListX86(int target, uint processId, ref uint numRWLocks, uint[] rwLockList);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetReadWriteLockList", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetReadWriteLockListX64(int target, uint processId, ref uint numRWLocks, uint[] rwLockList);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="processID"></param>
        /// <param name="rwLockList"></param>
        /// <returns></returns>
        public static SNRESULT GetReadWriteLockList(int target, uint processID, out uint[] rwLockList)
        {
            rwLockList = null;
            uint numRWLocks = 0;
            SNRESULT res = Is32Bit() ? GetReadWriteLockListX86(target, processID, ref numRWLocks, null) : GetReadWriteLockListX64(target, processID, ref numRWLocks, null);
            if (FAILED(res))
                return res;
            rwLockList = new uint[(int)numRWLocks];
            return Is32Bit() ? GetReadWriteLockListX86(target, processID, ref numRWLocks, rwLockList) : GetReadWriteLockListX64(target, processID, ref numRWLocks, rwLockList);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetReadWriteLockInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetReadWriteLockInfoX86(int target, uint processId, uint rwLockId, ref uint bufferSize, IntPtr buffer);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetReadWriteLockInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetReadWriteLockInfoX64(int target, uint processId, uint rwLockId, ref uint bufferSize, IntPtr buffer);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="processID"></param>
        /// <param name="rwLockID"></param>
        /// <param name="rwLockInfo"></param>
        /// <returns></returns>
        public static SNRESULT GetReadWriteLockInfo(int target, uint processID, uint rwLockID, out RWLockInfo rwLockInfo)
        {
            rwLockInfo = new RWLockInfo();
            uint bufferSize = 0;
            SNRESULT res1 = Is32Bit() ? GetReadWriteLockInfoX86(target, processID, rwLockID, ref bufferSize, IntPtr.Zero) : GetReadWriteLockInfoX64(target, processID, rwLockID, ref bufferSize, IntPtr.Zero);
            if (FAILED(res1))
                return res1;
            ScopedGlobalHeapPtr scopedGlobalHeapPtr = new ScopedGlobalHeapPtr(Marshal.AllocHGlobal((int)bufferSize));
            SNRESULT res2 = Is32Bit() ? GetReadWriteLockInfoX86(target, processID, rwLockID, ref bufferSize, scopedGlobalHeapPtr.Get()) : GetReadWriteLockInfoX64(target, processID, rwLockID, ref bufferSize, scopedGlobalHeapPtr.Get());
            if (FAILED(res2))
                return res2;
            RwLockInfoPriv storage = new RwLockInfoPriv();
            IntPtr unmanagedBuf = ReadDataFromUnmanagedIncPtr(scopedGlobalHeapPtr.Get(), ref storage);
            rwLockInfo.ID = storage.Id;
            rwLockInfo.Attribute = storage.Attribute;
            rwLockInfo.NumWaitingReadThreads = storage.NumWaitingReadThreads;
            rwLockInfo.NumWaitAllReadThreads = storage.NumWaitAllReadThreads;
            rwLockInfo.NumWaitingWriteThreads = storage.NumWaitingWriteThreads;
            rwLockInfo.NumWaitAllWriteThreads = storage.NumWaitAllWriteThreads;
            uint num = rwLockInfo.NumWaitingReadThreads + rwLockInfo.NumWaitingWriteThreads;
            rwLockInfo.WaitingThreads = new ulong[(int)num];
            for (int index = 0; index < num; ++index)
                unmanagedBuf = ReadDataFromUnmanagedIncPtr(unmanagedBuf, ref rwLockInfo.WaitingThreads[index]);
            return res2;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetSemaphoreList", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetSemaphoreListX86(int target, uint processId, ref uint numSemaphores, uint[] semaphoreList);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetSemaphoreList", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetSemaphoreListX64(int target, uint processId, ref uint numSemaphores, uint[] semaphoreList);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="processID"></param>
        /// <param name="semaphoreIDs"></param>
        /// <returns></returns>
        public static SNRESULT GetSemaphoreList(int target, uint processID, out uint[] semaphoreIDs)
        {
            semaphoreIDs = null;
            uint numSemaphores = 0;
            SNRESULT res = Is32Bit() ? GetSemaphoreListX86(target, processID, ref numSemaphores, null) : GetSemaphoreListX64(target, processID, ref numSemaphores, null);
            if (FAILED(res))
                return res;
            semaphoreIDs = new uint[(int)numSemaphores];
            return Is32Bit() ? GetSemaphoreListX86(target, processID, ref numSemaphores, semaphoreIDs) : GetSemaphoreListX64(target, processID, ref numSemaphores, semaphoreIDs);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetSemaphoreInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetSemaphoreInfoX86(int target, uint processId, uint semaphoreId, ref uint bufferSize, IntPtr buffer);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetSemaphoreInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetSemaphoreInfoX64(int target, uint processId, uint semaphoreId, ref uint bufferSize, IntPtr buffer);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="processID"></param>
        /// <param name="semaphoreID"></param>
        /// <param name="semaphoreInfo"></param>
        /// <returns></returns>
        public static SNRESULT GetSemaphoreInfo(int target, uint processID, uint semaphoreID, out SemaphoreInfo semaphoreInfo)
        {
            semaphoreInfo = new SemaphoreInfo();
            uint bufferSize = 0;
            SNRESULT res1 = Is32Bit() ? GetSemaphoreInfoX86(target, processID, semaphoreID, ref bufferSize, IntPtr.Zero) : GetSemaphoreInfoX64(target, processID, semaphoreID, ref bufferSize, IntPtr.Zero);
            if (FAILED(res1))
                return res1;
            ScopedGlobalHeapPtr scopedGlobalHeapPtr = new ScopedGlobalHeapPtr(Marshal.AllocHGlobal((int)bufferSize));
            SNRESULT res2 = Is32Bit() ? GetSemaphoreInfoX86(target, processID, semaphoreID, ref bufferSize, scopedGlobalHeapPtr.Get()) : GetSemaphoreInfoX64(target, processID, semaphoreID, ref bufferSize, scopedGlobalHeapPtr.Get());
            if (FAILED(res2))
                return res2;
            SemaphoreInfoPriv storage = new SemaphoreInfoPriv();
            IntPtr unmanagedBuf = ReadDataFromUnmanagedIncPtr(scopedGlobalHeapPtr.Get(), ref storage);
            semaphoreInfo.ID = storage.Id;
            semaphoreInfo.Attribute = storage.Attribute;
            semaphoreInfo.MaxValue = storage.MaxValue;
            semaphoreInfo.CurrentValue = storage.CurrentValue;
            semaphoreInfo.NumWaitAllThreads = storage.NumWaitAllThreads;
            semaphoreInfo.WaitingThreads = new ulong[(int)storage.NumWaitingThreads];
            for (int index = 0; index < storage.NumWaitingThreads; ++index)
                unmanagedBuf = ReadDataFromUnmanagedIncPtr(unmanagedBuf, ref semaphoreInfo.WaitingThreads[index]);
            return res2;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetEventQueueList", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetEventQueueListX86(int target, uint processId, ref uint numEventQueues, uint[] eventQueueList);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetEventQueueList", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetEventQueueListX64(int target, uint processId, ref uint numEventQueues, uint[] eventQueueList);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="processID"></param>
        /// <param name="eventQueueIDs"></param>
        /// <returns></returns>
        public static SNRESULT GetEventQueueList(int target, uint processID, out uint[] eventQueueIDs)
        {
            eventQueueIDs = null;
            uint numEventQueues = 0;
            SNRESULT res = Is32Bit() ? GetEventQueueListX86(target, processID, ref numEventQueues, null) : GetEventQueueListX64(target, processID, ref numEventQueues, null);
            if (FAILED(res))
                return res;
            eventQueueIDs = new uint[(int)numEventQueues];
            return Is32Bit() ? GetEventQueueListX86(target, processID, ref numEventQueues, eventQueueIDs) : GetEventQueueListX64(target, processID, ref numEventQueues, eventQueueIDs);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetEventQueueInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetEventQueueInfoX86(int target, uint processId, uint eventQueueId, ref uint bufferSize, IntPtr buffer);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetEventQueueInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetEventQueueInfoX64(int target, uint processId, uint eventQueueId, ref uint bufferSize, IntPtr buffer);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="processID"></param>
        /// <param name="eventQueueID"></param>
        /// <param name="eventQueueInfo"></param>
        /// <returns></returns>
        public static SNRESULT GetEventQueueInfo(int target, uint processID, uint eventQueueID, out EventQueueInfo eventQueueInfo)
        {
            eventQueueInfo = new EventQueueInfo();
            uint bufferSize = 0;
            SNRESULT res1 = Is32Bit() ? GetEventQueueInfoX86(target, processID, eventQueueID, ref bufferSize, IntPtr.Zero) : GetEventQueueInfoX64(target, processID, eventQueueID, ref bufferSize, IntPtr.Zero);
            if (FAILED(res1))
                return res1;
            ScopedGlobalHeapPtr scopedGlobalHeapPtr = new ScopedGlobalHeapPtr(Marshal.AllocHGlobal((int)bufferSize));
            SNRESULT res2 = Is32Bit() ? GetEventQueueInfoX86(target, processID, eventQueueID, ref bufferSize, scopedGlobalHeapPtr.Get()) : GetEventQueueInfoX64(target, processID, eventQueueID, ref bufferSize, scopedGlobalHeapPtr.Get());
            if (FAILED(res2))
                return res2;
            EventQueueInfoPriv structure = (EventQueueInfoPriv)Marshal.PtrToStructure(scopedGlobalHeapPtr.Get(), typeof(EventQueueInfoPriv));
            eventQueueInfo.ID = structure.Id;
            eventQueueInfo.Attribute = structure.Attribute;
            eventQueueInfo.Key = structure.Key;
            eventQueueInfo.Size = structure.Size;
            eventQueueInfo.NumWaitAllThreads = structure.NumWaitAllThreads;
            eventQueueInfo.NumReadableAllEvQueue = structure.NumReadableAllEvQueue;
            eventQueueInfo.WaitingThreadIDs = new ulong[(int)structure.NumWaitingThreads];
            IntPtr unmanagedBuf1 = structure.WaitingThreadIds;
            for (int index = 0; index < structure.NumWaitingThreads; ++index)
                unmanagedBuf1 = ReadDataFromUnmanagedIncPtr(unmanagedBuf1, ref eventQueueInfo.WaitingThreadIDs[index]);
            eventQueueInfo.QueueEntries = new SystemEvent[(int)structure.NumReadableEvQueue];
            IntPtr unmanagedBuf2 = structure.QueueEntries;
            for (int index = 0; index < structure.NumReadableEvQueue; ++index)
                unmanagedBuf2 = ReadDataFromUnmanagedIncPtr(unmanagedBuf2, ref eventQueueInfo.QueueEntries[index]);
            return res2;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetEventFlagList", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetEventFlagListX86(int target, uint processId, ref uint numEventFlags, uint[] eventFlagList);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetEventFlagList", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetEventFlagListX64(int target, uint processId, ref uint numEventFlags, uint[] eventFlagList);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="processID"></param>
        /// <param name="eventFlagIDs"></param>
        /// <returns></returns>
        public static SNRESULT GetEventFlagList(int target, uint processID, out uint[] eventFlagIDs)
        {
            eventFlagIDs = null;
            uint numEventFlags = 0;
            SNRESULT res = Is32Bit() ? GetEventFlagListX86(target, processID, ref numEventFlags, null) : GetEventFlagListX64(target, processID, ref numEventFlags, null);
            if (FAILED(res))
                return res;
            eventFlagIDs = new uint[(int)numEventFlags];
            return Is32Bit() ? GetEventFlagListX86(target, processID, ref numEventFlags, eventFlagIDs) : GetEventFlagListX64(target, processID, ref numEventFlags, eventFlagIDs);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetEventFlagInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetEventFlagInfoX86(int target, uint processId, uint eventFlagId, ref uint bufferSize, IntPtr buffer);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetEventFlagInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetEventFlagInfoX64(int target, uint processId, uint eventFlagId, ref uint bufferSize, IntPtr buffer);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="processID"></param>
        /// <param name="eventFlagID"></param>
        /// <param name="eventFlagInfo"></param>
        /// <returns></returns>
        public static SNRESULT GetEventFlagInfo(int target, uint processID, uint eventFlagID, out EventFlagInfo eventFlagInfo)
        {
            eventFlagInfo = new EventFlagInfo();
            uint bufferSize = 0;
            SNRESULT res1 = Is32Bit() ? GetEventFlagInfoX86(target, processID, eventFlagID, ref bufferSize, IntPtr.Zero) : GetEventFlagInfoX64(target, processID, eventFlagID, ref bufferSize, IntPtr.Zero);
            if (FAILED(res1))
                return res1;
            ScopedGlobalHeapPtr scopedGlobalHeapPtr = new ScopedGlobalHeapPtr(Marshal.AllocHGlobal((int)bufferSize));
            SNRESULT res2 = Is32Bit() ? GetEventFlagInfoX86(target, processID, eventFlagID, ref bufferSize, scopedGlobalHeapPtr.Get()) : GetEventFlagInfoX64(target, processID, eventFlagID, ref bufferSize, scopedGlobalHeapPtr.Get());
            if (FAILED(res2))
                return res2;
            IntPtr unmanagedBuf1 = ReadDataFromUnmanagedIncPtr(ReadDataFromUnmanagedIncPtr(ReadDataFromUnmanagedIncPtr(scopedGlobalHeapPtr.Get(), ref eventFlagInfo.ID), ref eventFlagInfo.Attribute), ref eventFlagInfo.BitPattern);
            uint storage = 0;
            IntPtr unmanagedBuf2 = ReadDataFromUnmanagedIncPtr(ReadDataFromUnmanagedIncPtr(unmanagedBuf1, ref storage), ref eventFlagInfo.NumWaitAllThreads);
            eventFlagInfo.WaitingThreads = new EventFlagWaitThread[(int)storage];
            for (int index = 0; index < storage; ++index)
                unmanagedBuf2 = ReadDataFromUnmanagedIncPtr(unmanagedBuf2, ref eventFlagInfo.WaitingThreads[index]);
            return res2;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3PickTarget", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT PickTargetX86(IntPtr hWndOwner, out int target);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3PickTarget", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT PickTargetX64(IntPtr hWndOwner, out int target);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hWndOwner"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static SNRESULT PickTarget(IntPtr hWndOwner, out int target)
        {
            if (!Is32Bit())
                return PickTargetX64(hWndOwner, out target);
            return PickTargetX86(hWndOwner, out target);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3EnableAutoStatusUpdate", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT EnableAutoStatusUpdateX86(int target, uint enabled, out uint previousState);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3EnableAutoStatusUpdate", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT EnableAutoStatusUpdateX64(int target, uint enabled, out uint previousState);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="bEnabled"></param>
        /// <param name="bPreviousState"></param>
        /// <returns></returns>
        public static SNRESULT EnableAutoStatusUpdate(int target, bool bEnabled, out bool bPreviousState)
        {
            uint enabled = bEnabled ? 1U : 0U;
            SNRESULT snresult = Is32Bit() ? EnableAutoStatusUpdateX86(target, enabled, out uint previousState) : EnableAutoStatusUpdateX64(target, enabled, out previousState);
            bPreviousState = (int)previousState != 0;
            return snresult;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetPowerStatus", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetPowerStatusX86(int target, out PowerStatus status);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetPowerStatus", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetPowerStatusX64(int target, out PowerStatus status);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public static SNRESULT GetPowerStatus(int target, out PowerStatus status)
        {
            if (!Is32Bit())
                return GetPowerStatusX64(target, out status);
            return GetPowerStatusX86(target, out status);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3PowerOn", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT PowerOnX86(int target);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3PowerOn", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT PowerOnX64(int target);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static SNRESULT PowerOn(int target)
        {
            if (!Is32Bit())
                return PowerOnX64(target);
            return PowerOnX86(target);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3PowerOff", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT PowerOffX86(int target, uint force);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3PowerOff", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT PowerOffX64(int target, uint force);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="bForce"></param>
        /// <returns></returns>
        public static SNRESULT PowerOff(int target, bool bForce)
        {
            uint force = bForce ? 1U : 0U;
            if (!Is32Bit())
                return PowerOffX64(target, force);
            return PowerOffX86(target, force);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetUserMemoryStats", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetUserMemoryStatsX86(int target, uint processId, out UserMemoryStats memoryStats);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetUserMemoryStats", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetUserMemoryStatsX64(int target, uint processId, out UserMemoryStats memoryStats);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="processID"></param>
        /// <param name="memoryStats"></param>
        /// <returns></returns>
        public static SNRESULT GetUserMemoryStats(int target, uint processID, out UserMemoryStats memoryStats)
        {
            if (!Is32Bit())
                return GetUserMemoryStatsX64(target, processID, out memoryStats);
            return GetUserMemoryStatsX86(target, processID, out memoryStats);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SetDefaultLoadPriority", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetDefaultLoadPriorityX86(int target, uint priority);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SetDefaultLoadPriority", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetDefaultLoadPriorityX64(int target, uint priority);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="priority"></param>
        /// <returns></returns>
        public static SNRESULT SetDefaultLoadPriority(int target, uint priority)
        {
            if (!Is32Bit())
                return SetDefaultLoadPriorityX64(target, priority);
            return SetDefaultLoadPriorityX86(target, priority);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetDefaultLoadPriority", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetDefaultLoadPriorityX86(int target, out uint priority);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetDefaultLoadPriority", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetDefaultLoadPriorityX64(int target, out uint priority);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="priority"></param>
        /// <returns></returns>
        public static SNRESULT GetDefaultLoadPriority(int target, out uint priority)
        {
            if (!Is32Bit())
                return GetDefaultLoadPriorityX64(target, out priority);
            return GetDefaultLoadPriorityX86(target, out priority);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetGamePortIPAddrData", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetGamePortIPAddrDataX86(int target, string deviceName, out GamePortIPAddressData ipAddressData);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetGamePortIPAddrData", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetGamePortIPAddrDataX64(int target, string deviceName, out GamePortIPAddressData ipAddressData);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="deviceName"></param>
        /// <param name="ipAddressData"></param>
        /// <returns></returns>
        public static SNRESULT GetGamePortIPAddrData(int target, string deviceName, out GamePortIPAddressData ipAddressData)
        {
            if (!Is32Bit())
                return GetGamePortIPAddrDataX64(target, deviceName, out ipAddressData);
            return GetGamePortIPAddrDataX86(target, deviceName, out ipAddressData);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetGamePortDebugIPAddrData", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetGamePortDebugIPAddrDataX86(int target, string deviceName, out GamePortIPAddressData data);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetGamePortDebugIPAddrData", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetGamePortDebugIPAddrDataX64(int target, string deviceName, out GamePortIPAddressData data);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="deviceName"></param>
        /// <param name="ipAddressData"></param>
        /// <returns></returns>
        public static SNRESULT GetGamePortDebugIPAddrData(int target, string deviceName, out GamePortIPAddressData ipAddressData)
        {
            if (!Is32Bit())
                return GetGamePortDebugIPAddrDataX64(target, deviceName, out ipAddressData);
            return GetGamePortDebugIPAddrDataX86(target, deviceName, out ipAddressData);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SetDABR", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetDABRX86(int target, uint processId, ulong address);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SetDABR", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetDABRX64(int target, uint processId, ulong address);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="processID"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        public static SNRESULT SetDABR(int target, uint processID, ulong address)
        {
            if (!Is32Bit())
                return SetDABRX64(target, processID, address);
            return SetDABRX86(target, processID, address);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetDABR", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetDABRX86(int target, uint processId, out ulong address);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetDABR", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetDABRX64(int target, uint processId, out ulong address);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="processID"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        public static SNRESULT GetDABR(int target, uint processID, out ulong address)
        {
            if (!Is32Bit())
                return GetDABRX64(target, processID, out address);
            return GetDABRX86(target, processID, out address);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SetRSXProfilingFlags", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetRSXProfilingFlagsX86(int target, ulong rsxFlags);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SetRSXProfilingFlags", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetRSXProfilingFlagsX64(int target, ulong rsxFlags);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="rsxFlags"></param>
        /// <returns></returns>
        public static SNRESULT SetRSXProfilingFlags(int target, RSXProfilingFlag rsxFlags)
        {
            if (!Is32Bit())
                return SetRSXProfilingFlagsX64(target, (ulong)rsxFlags);
            return SetRSXProfilingFlagsX86(target, (ulong)rsxFlags);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetRSXProfilingFlags", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetRSXProfilingFlagsX86(int target, out ulong rsxFlags);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetRSXProfilingFlags", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetRSXProfilingFlagsX64(int target, out ulong rsxFlags);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="rsxFlags"></param>
        /// <returns></returns>
        public static SNRESULT GetRSXProfilingFlags(int target, out RSXProfilingFlag rsxFlags)
        {
            SNRESULT snresult = Is32Bit() ? GetRSXProfilingFlagsX86(target, out ulong rsxFlags1) : GetRSXProfilingFlagsX64(target, out rsxFlags1);
            rsxFlags = (RSXProfilingFlag)rsxFlags1;
            return snresult;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SetCustomParamSFOMappingDirectory", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetCustomParamSFOMappingDirectoryX86(int target, IntPtr paramSfoDir);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SetCustomParamSFOMappingDirectory", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetCustomParamSFOMappingDirectoryX64(int target, IntPtr paramSfoDir);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="paramSFODir"></param>
        /// <returns></returns>
        public static SNRESULT SetCustomParamSFOMappingDirectory(int target, string paramSFODir)
        {
            ScopedGlobalHeapPtr scopedGlobalHeapPtr = new ScopedGlobalHeapPtr(AllocUtf8FromString(paramSFODir));
            if (!Is32Bit())
                return SetCustomParamSFOMappingDirectoryX64(target, scopedGlobalHeapPtr.Get());
            return SetCustomParamSFOMappingDirectoryX86(target, scopedGlobalHeapPtr.Get());
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3EnableXMBSettings", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT EnableXMBSettingsX86(int target, int enable);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3EnableXMBSettings", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT EnableXMBSettingsX64(int target, int enable);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="bEnable"></param>
        /// <returns></returns>
        public static SNRESULT EnableXMBSettings(int target, bool bEnable)
        {
            int enable = bEnable ? 1 : 0;
            if (!Is32Bit())
                return EnableXMBSettingsX64(target, enable);
            return EnableXMBSettingsX86(target, enable);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetXMBSettings", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetXMBSettingsX86(int target, IntPtr buffer, ref uint bufferSize, bool bUpdateCache);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetXMBSettings", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetXMBSettingsX64(int target, IntPtr buffer, ref uint bufferSize, bool bUpdateCache);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="xmbSettings"></param>
        /// <param name="bUpdateCache"></param>
        /// <returns></returns>
        public static SNRESULT GetXMBSettings(int target, out string xmbSettings, bool bUpdateCache)
        {
            xmbSettings = null;
            uint bufferSize = 0;
            SNRESULT res1 = Is32Bit() ? GetXMBSettingsX86(target, IntPtr.Zero, ref bufferSize, bUpdateCache) : GetXMBSettingsX64(target, IntPtr.Zero, ref bufferSize, bUpdateCache);
            if (FAILED(res1))
                return res1;
            ScopedGlobalHeapPtr scopedGlobalHeapPtr = new ScopedGlobalHeapPtr(Marshal.AllocHGlobal((int)bufferSize));
            SNRESULT res2 = Is32Bit() ? GetXMBSettingsX86(target, scopedGlobalHeapPtr.Get(), ref bufferSize, bUpdateCache) : GetXMBSettingsX64(target, scopedGlobalHeapPtr.Get(), ref bufferSize, bUpdateCache);
            if (SUCCEEDED(res2))
                xmbSettings = Marshal.PtrToStringAnsi(scopedGlobalHeapPtr.Get());
            return res2;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SetXMBSettings", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetXMBSettingsX86(int target, string xmbSettings, bool bUpdateCache);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SetXMBSettings", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetXMBSettingsX64(int target, string xmbSettings, bool bUpdateCache);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="xmbSettings"></param>
        /// <param name="bUpdateCache"></param>
        /// <returns></returns>
        public static SNRESULT SetXMBSettings(int target, string xmbSettings, bool bUpdateCache)
        {
            return Is32Bit() ? SetXMBSettingsX86(target, xmbSettings, bUpdateCache) : SetXMBSettingsX64(target, xmbSettings, bUpdateCache);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3FootswitchControl", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT FootswitchControlX86(int target, uint enabled);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3FootswitchControl", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT FootswitchControlX64(int target, uint enabled);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="bEnabled"></param>
        /// <returns></returns>
        public static SNRESULT FootswitchControl(int target, bool bEnabled)
        {
            uint enabled = bEnabled ? 1U : 0U;
            if (!Is32Bit())
                return FootswitchControlX64(target, enabled);
            return FootswitchControlX86(target, enabled);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3TriggerCoreDump", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT TriggerCoreDumpX86(int target, uint processId, ulong userData1, ulong userData2, ulong userData3);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3TriggerCoreDump", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT TriggerCoreDumpX64(int target, uint processId, ulong userData1, ulong userData2, ulong userData3);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="processID"></param>
        /// <param name="userData1"></param>
        /// <param name="userData2"></param>
        /// <param name="userData3"></param>
        /// <returns></returns>
        public static SNRESULT TriggerCoreDump(int target, uint processID, ulong userData1, ulong userData2, ulong userData3)
        {
            if (!Is32Bit())
                return TriggerCoreDumpX64(target, processID, userData1, userData2, userData3);
            return TriggerCoreDumpX86(target, processID, userData1, userData2, userData3);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetCoreDumpFlags", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetCoreDumpFlagsX86(int target, out ulong flags);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetCoreDumpFlags", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetCoreDumpFlagsX64(int target, out ulong flags);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public static SNRESULT GetCoreDumpFlags(int target, out CoreDumpFlag flags)
        {
            SNRESULT snresult = Is32Bit() ? GetCoreDumpFlagsX86(target, out ulong flags1) : GetCoreDumpFlagsX64(target, out flags1);
            flags = (CoreDumpFlag)flags1;
            return snresult;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SetCoreDumpFlags", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetCoreDumpFlagsX86(int tarSet, ulong flags);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SetCoreDumpFlags", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetCoreDumpFlagsX64(int tarSet, ulong flags);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tarSet"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public static SNRESULT SetCoreDumpFlags(int tarSet, CoreDumpFlag flags)
        {
            if (!Is32Bit())
                return SetCoreDumpFlagsX64(tarSet, (ulong)flags);
            return SetCoreDumpFlagsX86(tarSet, (ulong)flags);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ProcessAttach", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ProcessAttachX86(int target, uint unitId, uint processId);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ProcessAttach", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ProcessAttachX64(int target, uint unitId, uint processId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="unit"></param>
        /// <param name="processID"></param>
        /// <returns></returns>
        public static SNRESULT ProcessAttach(int target, UnitType unit, uint processID)
        {
            if (!Is32Bit())
                return ProcessAttachX64(target, (uint)unit, processID);
            return ProcessAttachX86(target, (uint)unit, processID);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3FlashTarget", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT FlashTargetX86(int target, IntPtr updaterToolPath, IntPtr flashImagePath);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3FlashTarget", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT FlashTargetX64(int target, IntPtr updaterToolPath, IntPtr flashImagePath);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="updaterToolPath"></param>
        /// <param name="flashImagePath"></param>
        /// <returns></returns>
        public static SNRESULT FlashTarget(int target, string updaterToolPath, string flashImagePath)
        {
            ScopedGlobalHeapPtr scopedGlobalHeapPtr1 = new ScopedGlobalHeapPtr(AllocUtf8FromString(updaterToolPath));
            ScopedGlobalHeapPtr scopedGlobalHeapPtr2 = new ScopedGlobalHeapPtr(AllocUtf8FromString(flashImagePath));
            if (!Is32Bit())
                return FlashTargetX64(target, scopedGlobalHeapPtr1.Get(), scopedGlobalHeapPtr2.Get());
            return FlashTargetX86(target, scopedGlobalHeapPtr1.Get(), scopedGlobalHeapPtr2.Get());
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetMacAddress", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetMacAddressX86(int target, out IntPtr stringPtr);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetMacAddress", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetMacAddressX64(int target, out IntPtr stringPtr);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="macAddress"></param>
        /// <returns></returns>
        public static SNRESULT GetMACAddress(int target, out string macAddress)
        {
            SNRESULT snresult = Is32Bit() ? GetMacAddressX86(target, out IntPtr stringPtr) : GetMacAddressX64(target, out stringPtr);
            macAddress = Marshal.PtrToStringAnsi(stringPtr);
            return snresult;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ProcessScatteredSetMemory", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ProcessScatteredSetMemoryX86(int target, uint processId, uint numWrites, uint writeSize, IntPtr writeData, out uint errorCode, out uint failedAddress);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ProcessScatteredSetMemory", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ProcessScatteredSetMemoryX64(int target, uint processId, uint numWrites, uint writeSize, IntPtr writeData, out uint errorCode, out uint failedAddress);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="processID"></param>
        /// <param name="writeData"></param>
        /// <param name="errorCode"></param>
        /// <param name="failedAddress"></param>
        /// <returns></returns>
        public static SNRESULT ProcessScatteredSetMemory(int target, uint processID, ScatteredWrite[] writeData, out uint errorCode, out uint failedAddress)
        {
            errorCode = 0U;
            failedAddress = 0U;
            if (writeData == null || writeData.Length == 0)
                return SNRESULT.SN_E_BAD_PARAM;
            int length1 = writeData.Length;
            if (writeData[0].Data == null)
                return SNRESULT.SN_E_BAD_PARAM;
            int length2 = writeData[0].Data.Length;
            ScopedGlobalHeapPtr scopedGlobalHeapPtr = new ScopedGlobalHeapPtr(Marshal.AllocHGlobal(length1 * (Marshal.SizeOf((object)writeData[0].Address) + length2)));
            IntPtr num = scopedGlobalHeapPtr.Get();
            for (int index = 0; index < length1; ++index)
            {
                num = WriteDataToUnmanagedIncPtr(writeData[index].Address, num);
                if (writeData[index].Data == null || writeData[index].Data.Length != length2)
                    return SNRESULT.SN_E_BAD_PARAM;
                Marshal.Copy(writeData[index].Data, 0, num, writeData[index].Data.Length);
                num = new IntPtr(num.ToInt64() + writeData[index].Data.Length);
            }
            if (!Is32Bit())
                return ProcessScatteredSetMemoryX64(target, processID, (uint)length1, (uint)length2, scopedGlobalHeapPtr.Get(), out errorCode, out failedAddress);
            return ProcessScatteredSetMemoryX86(target, processID, (uint)length1, (uint)length2, scopedGlobalHeapPtr.Get(), out errorCode, out failedAddress);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetMATRanges", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetMATRangesX86(int target, uint processId, ref uint rangeCount, IntPtr matRanges);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetMATRanges", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetMATRangesX64(int target, uint processId, ref uint rangeCount, IntPtr matRanges);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="processID"></param>
        /// <param name="matRanges"></param>
        /// <returns></returns>
        public static SNRESULT GetMATRanges(int target, uint processID, out MATRange[] matRanges)
        {
            matRanges = null;
            uint rangeCount = 0;
            SNRESULT res1 = Is32Bit() ? GetMATRangesX86(target, processID, ref rangeCount, IntPtr.Zero) : GetMATRangesX64(target, processID, ref rangeCount, IntPtr.Zero);
            if (FAILED(res1))
                return res1;
            if ((int)rangeCount == 0)
            {
                matRanges = new MATRange[0];
                return SNRESULT.SN_S_OK;
            }
            ScopedGlobalHeapPtr scopedGlobalHeapPtr = new ScopedGlobalHeapPtr(Marshal.AllocHGlobal((int)((2 * Marshal.SizeOf(typeof(uint))) * rangeCount)));
            SNRESULT res2 = Is32Bit() ? GetMATRangesX86(target, processID, ref rangeCount, scopedGlobalHeapPtr.Get()) : GetMATRangesX64(target, processID, ref rangeCount, scopedGlobalHeapPtr.Get());
            if (FAILED(res2))
                return res2;
            IntPtr unmanagedBuf = scopedGlobalHeapPtr.Get();
            matRanges = new MATRange[(int)rangeCount];
            for (uint index = 0; index < rangeCount; ++index)
                unmanagedBuf = ReadDataFromUnmanagedIncPtr(ReadDataFromUnmanagedIncPtr(unmanagedBuf, ref matRanges[(int)index].StartAddress), ref matRanges[(int)index].Size);
            return res2;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetMATConditions", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetMATConditionsX86(int target, uint processId, ref uint rangeCount, IntPtr ranges, ref uint bufSize, IntPtr outputBuf);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetMATConditions", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetMATConditionsX64(int target, uint processId, ref uint rangeCount, IntPtr ranges, ref uint bufSize, IntPtr outputBuf);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="processID"></param>
        /// <param name="matRanges"></param>
        /// <returns></returns>
        public static SNRESULT GetMATConditions(int target, uint processID, ref MATRange[] matRanges)
        {
            if (matRanges == null || matRanges.Length < 1)
                return SNRESULT.SN_E_BAD_PARAM;
            uint length = (uint)matRanges.Length;
            ScopedGlobalHeapPtr scopedGlobalHeapPtr1 = new ScopedGlobalHeapPtr(Marshal.AllocHGlobal(8 * (int)length));
            IntPtr unmanagedIncPtr1 = scopedGlobalHeapPtr1.Get();
            foreach (MATRange matRange in matRanges)
            {
                IntPtr unmanagedIncPtr2 = WriteDataToUnmanagedIncPtr(matRange.StartAddress, unmanagedIncPtr1);
                unmanagedIncPtr1 = WriteDataToUnmanagedIncPtr(matRange.Size, unmanagedIncPtr2);
            }
            uint bufSize = 0;
            SNRESULT res1 = Is32Bit() ? GetMATConditionsX86(target, processID, ref length, scopedGlobalHeapPtr1.Get(), ref bufSize, IntPtr.Zero) : GetMATConditionsX64(target, processID, ref length, scopedGlobalHeapPtr1.Get(), ref bufSize, IntPtr.Zero);
            if (FAILED(res1))
                return res1;
            ScopedGlobalHeapPtr scopedGlobalHeapPtr2 = new ScopedGlobalHeapPtr(Marshal.AllocHGlobal((int)bufSize));
            SNRESULT res2 = Is32Bit() ? GetMATConditionsX86(target, processID, ref length, scopedGlobalHeapPtr1.Get(), ref bufSize, scopedGlobalHeapPtr2.Get()) : GetMATConditionsX64(target, processID, ref length, scopedGlobalHeapPtr1.Get(), ref bufSize, scopedGlobalHeapPtr2.Get());
            if (FAILED(res2))
                return res2;
            IntPtr unmanagedBuf = scopedGlobalHeapPtr2.Get();
            for (int index1 = 0; index1 < length; ++index1)
            {
                unmanagedBuf = ReadDataFromUnmanagedIncPtr(ReadDataFromUnmanagedIncPtr(unmanagedBuf, ref matRanges[index1].StartAddress), ref matRanges[index1].Size);
                uint num = matRanges[index1].Size / 4096U;
                matRanges[index1].PageConditions = new MATCondition[(int)num];
                for (int index2 = 0; index2 < num; ++index2)
                {
                    byte storage = 0;
                    unmanagedBuf = ReadDataFromUnmanagedIncPtr(unmanagedBuf, ref storage);
                    matRanges[index1].PageConditions[index2] = (MATCondition)storage;
                }
                bufSize -= 8U + num;
            }
            if ((int)bufSize != 0)
                res2 = SNRESULT.SN_E_ERROR;
            return res2;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SetMATConditions", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetMATConditionsX86(int target, uint processId, uint rangeCount, uint bufSize, IntPtr buffer);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SetMATConditions", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetMATConditionsX64(int target, uint processId, uint rangeCount, uint bufSize, IntPtr buffer);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="processID"></param>
        /// <param name="matRanges"></param>
        /// <returns></returns>
        public static SNRESULT SetMATConditions(int target, uint processID, MATRange[] matRanges)
        {
            if (matRanges == null || matRanges.Length < 1)
                return SNRESULT.SN_E_BAD_PARAM;
            int length = matRanges.Length;
            int num = 0;
            foreach (MATRange matRange in matRanges)
            {
                if (matRange.PageConditions == null)
                    return SNRESULT.SN_E_BAD_PARAM;
                num += matRange.PageConditions.Length;
            }
            ScopedGlobalHeapPtr scopedGlobalHeapPtr = new ScopedGlobalHeapPtr(Marshal.AllocHGlobal(num + 2 * length * 4));
            IntPtr unmanagedIncPtr1 = scopedGlobalHeapPtr.Get();
            foreach (MATRange matRange in matRanges)
            {
                IntPtr unmanagedIncPtr2 = WriteDataToUnmanagedIncPtr(matRange.StartAddress, unmanagedIncPtr1);
                unmanagedIncPtr1 = WriteDataToUnmanagedIncPtr(matRange.Size, unmanagedIncPtr2);
                foreach (byte pageCondition in matRange.PageConditions)
                    unmanagedIncPtr1 = WriteDataToUnmanagedIncPtr(pageCondition, unmanagedIncPtr1);
            }
            uint bufSize = 1;
            if (!Is32Bit())
                return SetMATConditionsX64(target, processID, (uint)length, bufSize, scopedGlobalHeapPtr.Get());
            return SetMATConditionsX86(target, processID, (uint)length, bufSize, scopedGlobalHeapPtr.Get());
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SaveSettings", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SaveSettingsX86();

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SaveSettings", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SaveSettingsX64();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static SNRESULT SaveSettings()
        {
            if (!Is32Bit())
                return SaveSettingsX64();
            return SaveSettingsX86();
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3Exit", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ExitX86();

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3Exit", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ExitX64();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static SNRESULT Exit()
        {
            if (!Is32Bit())
                return ExitX64();
            return ExitX86();
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ExitEx", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ExitExX86(uint millisecondTimeout);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ExitEx", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ExitExX64(uint millisecondTimeout);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="millisecondTimeout"></param>
        /// <returns></returns>
        public static SNRESULT ExitEx(uint millisecondTimeout)
        {
            if (!Is32Bit())
                return ExitExX64(millisecondTimeout);
            return ExitExX86(millisecondTimeout);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3RegisterPadPlaybackNotificationHandler", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT RegisterPadPlaybackNotificationHandlerX86(int target, HandleEventCallbackPriv callback, IntPtr userData);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3RegisterPadPlaybackNotificationHandler", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT RegisterPadPlaybackNotificationHandlerX64(int target, HandleEventCallbackPriv callback, IntPtr userData);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="callback"></param>
        /// <param name="userData"></param>
        /// <returns></returns>
        public static SNRESULT RegisterPadPlaybackHandler(int target, PadPlaybackCallback callback, ref object userData)
        {
            if (callback == null)
                return SNRESULT.SN_E_BAD_PARAM;
            SNRESULT res = Is32Bit() ? RegisterPadPlaybackNotificationHandlerX86(target, ms_eventHandlerWrapper, IntPtr.Zero) : RegisterPadPlaybackNotificationHandlerX64(target, ms_eventHandlerWrapper, IntPtr.Zero);
            if (SUCCEEDED(res))
            {
                PadPlaybackCallbackAndUserData callbackAndUserData = new PadPlaybackCallbackAndUserData
                {
                    m_callback = callback,
                    m_userData = userData
                };
                if (ms_userPadPlaybackCallbacks == null)
                    ms_userPadPlaybackCallbacks = new Dictionary<int, PadPlaybackCallbackAndUserData>(1);
                ms_userPadPlaybackCallbacks[target] = callbackAndUserData;
            }
            return res;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3UnRegisterPadPlaybackNotificationHandler", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT UnregisterPadPlaybackHandlerX86(int target);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3UnRegisterPadPlaybackNotificationHandler", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT UnregisterPadPlaybackHandlerX64(int target);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static SNRESULT UnregisterPadPlaybackHandler(int target)
        {
            SNRESULT res = Is32Bit() ? UnregisterPadPlaybackHandlerX86(target) : UnregisterPadPlaybackHandlerX64(target);
            if (SUCCEEDED(res))
            {
                if (ms_userPadPlaybackCallbacks == null)
                    return SNRESULT.SN_E_ERROR;
                ms_userPadPlaybackCallbacks.Remove(target);
            }
            return res;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3StartPadPlayback", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT StartPadPlaybackX86(int target);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3StartPadPlayback", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT StartPadPlaybackX64(int target);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static SNRESULT StartPadPlayback(int target)
        {
            if (!Is32Bit())
                return StartPadPlaybackX64(target);
            return StartPadPlaybackX86(target);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3StopPadPlayback", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT StopPadPlaybackX86(int target);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3StopPadPlayback", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT StopPadPlaybackX64(int target);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static SNRESULT StopPadPlayback(int target)
        {
            if (!Is32Bit())
                return StopPadPlaybackX64(target);
            return StopPadPlaybackX86(target);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SendPadPlaybackData", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SendPadPlaybackDataX86(int target, ref PadData data);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SendPadPlaybackData", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SendPadPlaybackDataX64(int target, ref PadData data);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="padData"></param>
        /// <returns></returns>
        public static SNRESULT SendPadPlaybackData(int target, PadData padData)
        {
            if (padData.buttons == null || padData.buttons.Length != 24)
                return SNRESULT.SN_E_BAD_PARAM;
            if (!Is32Bit())
                return SendPadPlaybackDataX64(target, ref padData);
            return SendPadPlaybackDataX86(target, ref padData);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3RegisterPadCaptureHandler", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT RegisterPadCaptureHandlerX86(int target, HandleEventCallbackPriv callback, IntPtr userData);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3RegisterPadCaptureHandler", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT RegisterPadCaptureHandlerX64(int target, HandleEventCallbackPriv callback, IntPtr userData);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="callback"></param>
        /// <param name="userData"></param>
        /// <returns></returns>
        public static SNRESULT RegisterPadCaptureHandler(int target, PadCaptureCallback callback, ref object userData)
        {
            if (callback == null)
                return SNRESULT.SN_E_BAD_PARAM;
            SNRESULT res = Is32Bit() ? RegisterPadCaptureHandlerX86(target, ms_eventHandlerWrapper, IntPtr.Zero) : RegisterPadCaptureHandlerX64(target, ms_eventHandlerWrapper, IntPtr.Zero);
            if (SUCCEEDED(res))
            {
                PadCaptureCallbackAndUserData callbackAndUserData = new PadCaptureCallbackAndUserData
                {
                    m_callback = callback,
                    m_userData = userData
                };
                if (ms_userPadCaptureCallbacks == null)
                    ms_userPadCaptureCallbacks = new Dictionary<int, PadCaptureCallbackAndUserData>(1);
                ms_userPadCaptureCallbacks[target] = callbackAndUserData;
            }
            return res;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3UnRegisterPadCaptureHandler", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT UnregisterPadCaptureHandlerX86(int target);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3UnRegisterPadCaptureHandler", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT UnregisterPadCaptureHandlerX64(int target);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static SNRESULT UnregisterPadCaptureHandler(int target)
        {
            SNRESULT res = Is32Bit() ? UnregisterPadCaptureHandlerX86(target) : UnregisterPadCaptureHandlerX64(target);
            if (SUCCEEDED(res))
            {
                if (ms_userPadCaptureCallbacks == null)
                    return SNRESULT.SN_E_ERROR;
                ms_userPadCaptureCallbacks.Remove(target);
            }
            return res;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3StartPadCapture", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT StartPadCaptureX86(int target);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3StartPadCapture", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT StartPadCaptureX64(int target);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static SNRESULT StartPadCapture(int target)
        {
            if (!Is32Bit())
                return StartPadCaptureX64(target);
            return StartPadCaptureX86(target);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3StopPadCapture", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT StopPadCaptureX86(int target);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3StopPadCapture", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT StopPadCaptureX64(int target);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static SNRESULT StopPadCapture(int target)
        {
            if (!Is32Bit())
                return StopPadCaptureX64(target);
            return StopPadCaptureX86(target);
        }

        private static void MarshalPadCaptureEvent(int target, uint param, SNRESULT res, uint length, IntPtr data)
        {
            if ((int)length != 1)
                return;
            PadData[] padData = new PadData[1];
            padData[0].buttons = new short[24];
            ReadDataFromUnmanagedIncPtr(data, ref padData[0]);
            if (ms_userPadCaptureCallbacks == null)
                return;
            ms_userPadCaptureCallbacks[target].m_callback(target, res, padData, ms_userPadCaptureCallbacks[target].m_userData);
        }

        private static void MarshalPadPlaybackEvent(int target, uint param, SNRESULT result, uint length, IntPtr data)
        {
            if ((int)length != 1)
                return;
            uint storage = 0;
            ReadDataFromUnmanagedIncPtr(data, ref storage);
            if (ms_userPadPlaybackCallbacks == null)
                return;
            ms_userPadPlaybackCallbacks[target].m_callback(target, result, (PadPlaybackResponse)storage, ms_userPadPlaybackCallbacks[target].m_userData);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetVRAMCaptureFlags", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetVRAMCaptureFlagsX86(int target, out ulong vramFlags);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetVRAMCaptureFlags", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetVRAMCaptureFlagsX64(int target, out ulong vramFlags);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="vramFlags"></param>
        /// <returns></returns>
        public static SNRESULT GetVRAMCaptureFlags(int target, out VRAMCaptureFlag vramFlags)
        {
            SNRESULT snresult = Is32Bit() ? GetVRAMCaptureFlagsX86(target, out ulong vramFlags1) : GetVRAMCaptureFlagsX64(target, out vramFlags1);
            vramFlags = (VRAMCaptureFlag)vramFlags1;
            return snresult;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SetVRAMCaptureFlags", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetVRAMCaptureFlagsX86(int target, ulong vramFlags);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SetVRAMCaptureFlags", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetVRAMCaptureFlagsX64(int target, ulong vramFlags);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="vramFlags"></param>
        /// <returns></returns>
        public static SNRESULT SetVRAMCaptureFlags(int target, VRAMCaptureFlag vramFlags)
        {
            if (!Is32Bit())
                return SetVRAMCaptureFlagsX64(target, (ulong)vramFlags);
            return SetVRAMCaptureFlagsX86(target, (ulong)vramFlags);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3EnableVRAMCapture", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT EnableVRAMCaptureX86(int target);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3EnableVRAMCapture", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT EnableVRAMCaptureX864(int target);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static SNRESULT EnableVRAMCapture(int target)
        {
            if (!Is32Bit())
                return EnableVRAMCaptureX864(target);
            return EnableVRAMCaptureX86(target);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetVRAMInformation", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetVRAMInformationX86(int target, uint processId, out VramInfoPriv primaryVRAMInfo, out VramInfoPriv secondaryVRAMInfo);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetVRAMInformation", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetVRAMInformationX64(int target, uint processId, out VramInfoPriv primaryVRAMInfo, out VramInfoPriv secondaryVRAMInfo);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="processID"></param>
        /// <param name="primaryVRAMInfo"></param>
        /// <param name="secondaryVRAMInfo"></param>
        /// <returns></returns>
        public static SNRESULT GetVRAMInformation(int target, uint processID, out VRAMInfo primaryVRAMInfo, out VRAMInfo secondaryVRAMInfo)
        {
            primaryVRAMInfo = null;
            secondaryVRAMInfo = null;
            SNRESULT res = Is32Bit() ? GetVRAMInformationX86(target, processID, out VramInfoPriv primaryVRAMInfo1, out VramInfoPriv secondaryVRAMInfo1) : GetVRAMInformationX64(target, processID, out primaryVRAMInfo1, out secondaryVRAMInfo1);
            if (FAILED(res))
                return res;
            primaryVRAMInfo = new VRAMInfo
            {
                BPAddress = primaryVRAMInfo1.BpAddress,
                TopAddressPointer = primaryVRAMInfo1.TopAddressPointer,
                Width = primaryVRAMInfo1.Width,
                Height = primaryVRAMInfo1.Height,
                Pitch = primaryVRAMInfo1.Pitch,
                Colour = primaryVRAMInfo1.Colour
            };
            secondaryVRAMInfo = new VRAMInfo
            {
                BPAddress = secondaryVRAMInfo1.BpAddress,
                TopAddressPointer = secondaryVRAMInfo1.TopAddressPointer,
                Width = secondaryVRAMInfo1.Width,
                Height = secondaryVRAMInfo1.Height,
                Pitch = secondaryVRAMInfo1.Pitch,
                Colour = secondaryVRAMInfo1.Colour
            };
            return res;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3VRAMCapture", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT VRAMCaptureX86(int target, uint processId, IntPtr vramInfo, IntPtr fileName);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3VRAMCapture", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT VRAMCaptureX64(int target, uint processId, IntPtr vramInfo, IntPtr fileName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="processID"></param>
        /// <param name="vramInfo"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static SNRESULT VRAMCapture(int target, uint processID, VRAMInfo vramInfo, string fileName)
        {
            ScopedGlobalHeapPtr scopedGlobalHeapPtr1 = new ScopedGlobalHeapPtr(IntPtr.Zero);
            if (vramInfo != null)
            {
                VramInfoPriv vramInfoPriv = new VramInfoPriv
                {
                    BpAddress = vramInfo.BPAddress,
                    TopAddressPointer = vramInfo.TopAddressPointer,
                    Width = vramInfo.Width,
                    Height = vramInfo.Height,
                    Pitch = vramInfo.Pitch,
                    Colour = vramInfo.Colour
                };
                scopedGlobalHeapPtr1 = new ScopedGlobalHeapPtr(Marshal.AllocHGlobal(Marshal.SizeOf((object)vramInfoPriv)));
                Marshal.StructureToPtr((object)vramInfoPriv, scopedGlobalHeapPtr1.Get(), false);
            }
            ScopedGlobalHeapPtr scopedGlobalHeapPtr2 = new ScopedGlobalHeapPtr(AllocUtf8FromString(fileName));
            if (!Is32Bit())
                return VRAMCaptureX64(target, processID, scopedGlobalHeapPtr1.Get(), scopedGlobalHeapPtr2.Get());
            return VRAMCaptureX86(target, processID, scopedGlobalHeapPtr1.Get(), scopedGlobalHeapPtr2.Get());
        }

        private static void CustomProtocolHandler(int target, PS3Protocol ps3Protocol, IntPtr unmanagedBuf, uint length, IntPtr userData)
        {
            PS3ProtocolPriv protocol = new PS3ProtocolPriv(ps3Protocol.Protocol, ps3Protocol.Port);
            CustomProtocolId key = new CustomProtocolId(target, protocol);
            if (ms_userCustomProtoCallbacks == null || !ms_userCustomProtoCallbacks.TryGetValue(key, out CusProtoCallbackAndUserData callbackAndUserData))
                return;
            byte[] numArray = new byte[(int)length];
            Marshal.Copy(unmanagedBuf, numArray, 0, numArray.Length);
            callbackAndUserData.m_callback(target, ps3Protocol, numArray, callbackAndUserData.m_userData);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3RegisterCustomProtocolEx", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT RegisterCustomProtocolExX86(int target, uint protocol, uint port, string lparDesc, uint priority, out PS3Protocol ps3Protocol, CustomProtocolCallbackPriv callback, IntPtr userData);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3RegisterCustomProtocolEx", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT RegisterCustomProtocolExX64(int target, uint protocol, uint port, string lparDesc, uint priority, out PS3Protocol ps3Protocol, CustomProtocolCallbackPriv callback, IntPtr userData);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="protocol"></param>
        /// <param name="port"></param>
        /// <param name="lparDesc"></param>
        /// <param name="priority"></param>
        /// <param name="ps3Protocol"></param>
        /// <param name="callback"></param>
        /// <param name="userData"></param>
        /// <returns></returns>
        public static SNRESULT RegisterCustomProtocol(int target, uint protocol, uint port, string lparDesc, uint priority, out PS3Protocol ps3Protocol, CustomProtocolCallback callback, ref object userData)
        {
            ps3Protocol = new PS3Protocol();
            if (callback == null)
                return SNRESULT.SN_E_BAD_PARAM;
            SNRESULT res = Is32Bit() ? RegisterCustomProtocolExX86(target, protocol, port, lparDesc, priority, out ps3Protocol, ms_customProtoCallbackPriv, IntPtr.Zero) : RegisterCustomProtocolExX64(target, protocol, port, lparDesc, priority, out ps3Protocol, ms_customProtoCallbackPriv, IntPtr.Zero);
            if (SUCCEEDED(res))
            {
                PS3ProtocolPriv protocol1 = new PS3ProtocolPriv(ps3Protocol.Protocol, ps3Protocol.Port);
                CustomProtocolId index = new CustomProtocolId(target, protocol1);
                CusProtoCallbackAndUserData callbackAndUserData = new CusProtoCallbackAndUserData
                {
                    m_callback = callback,
                    m_userData = userData
                };
                if (ms_userCustomProtoCallbacks == null)
                    ms_userCustomProtoCallbacks = new Dictionary<CustomProtocolId, CusProtoCallbackAndUserData>(1);
                ms_userCustomProtoCallbacks[index] = callbackAndUserData;
            }
            return res;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3UnRegisterCustomProtocol", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT UnregisterCustomProtocolX86(int target, ref PS3Protocol protocol);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3UnRegisterCustomProtocol", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT UnregisterCustomProtocolX64(int target, ref PS3Protocol protocol);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="ps3Protocol"></param>
        /// <returns></returns>
        public static SNRESULT UnregisterCustomProtocol(int target, PS3Protocol ps3Protocol)
        {
            SNRESULT res = Is32Bit() ? UnregisterCustomProtocolX86(target, ref ps3Protocol) : UnregisterCustomProtocolX64(target, ref ps3Protocol);
            if (SUCCEEDED(res))
            {
                PS3ProtocolPriv protocol = new PS3ProtocolPriv(ps3Protocol.Protocol, ps3Protocol.Port);
                CustomProtocolId key = new CustomProtocolId(target, protocol);
                if (ms_userCustomProtoCallbacks == null)
                    return SNRESULT.SN_E_ERROR;
                ms_userCustomProtoCallbacks.Remove(key);
            }
            return res;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ForceUnRegisterCustomProtocol", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ForceUnregisterCustomProtocolX86(int target, ref PS3Protocol protocol);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ForceUnRegisterCustomProtocol", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ForceUnregisterCustomProtocolX64(int target, ref PS3Protocol protocol);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="ps3Protocol"></param>
        /// <returns></returns>
        public static SNRESULT ForceUnregisterCustomProtocol(int target, PS3Protocol ps3Protocol)
        {
            SNRESULT res = Is32Bit() ? ForceUnregisterCustomProtocolX86(target, ref ps3Protocol) : ForceUnregisterCustomProtocolX64(target, ref ps3Protocol);
            if (SUCCEEDED(res))
            {
                PS3ProtocolPriv protocol = new PS3ProtocolPriv(ps3Protocol.Protocol, ps3Protocol.Port);
                CustomProtocolId key = new CustomProtocolId(target, protocol);
                if (ms_userCustomProtoCallbacks == null)
                    return SNRESULT.SN_E_ERROR;
                ms_userCustomProtoCallbacks.Remove(key);
            }
            return res;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SendCustomProtocolData", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SendCustomProtocolDataX86(int target, ref PS3Protocol protocol, byte[] data, int length);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SendCustomProtocolData", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SendCustomProtocolDataX64(int target, ref PS3Protocol protocol, byte[] data, int length);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="ps3Protocol"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static SNRESULT SendCustomProtocolData(int target, PS3Protocol ps3Protocol, byte[] data)
        {
            if (data == null || data.Length < 1)
                return SNRESULT.SN_E_BAD_PARAM;
            if (!Is32Bit())
                return SendCustomProtocolDataX64(target, ref ps3Protocol, data, data.Length);
            return SendCustomProtocolDataX86(target, ref ps3Protocol, data, data.Length);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SetFileServingEventFlags", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetFileServingEventFlagsX86(int target, ulong eventFlags);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SetFileServingEventFlags", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetFileServingEventFlagsX64(int target, ulong eventFlags);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="eventFlags"></param>
        /// <returns></returns>
        public static SNRESULT SetFileServingEventFlags(int target, FileServingEventFlag eventFlags)
        {
            if (!Is32Bit())
                return SetFileServingEventFlagsX64(target, (ulong)eventFlags);
            return SetFileServingEventFlagsX86(target, (ulong)eventFlags);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetFileServingEventFlags", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetFileServingEventFlagsX86(int target, ref ulong eventFlags);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetFileServingEventFlags", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetFileServingEventFlagsX64(int target, ref ulong eventFlags);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="eventFlags"></param>
        /// <returns></returns>
        public static SNRESULT GetFileServingEventFlags(int target, out FileServingEventFlag eventFlags)
        {
            ulong eventFlags1 = 0;
            SNRESULT snresult = Is32Bit() ? GetFileServingEventFlagsX86(target, ref eventFlags1) : GetFileServingEventFlagsX64(target, ref eventFlags1);
            eventFlags = (FileServingEventFlag)eventFlags1;
            return snresult;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SetCaseSensitiveFileServing", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetCaseSensitiveFileServingX86(int target, bool bOn, out bool bOldSetting);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SetCaseSensitiveFileServing", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetCaseSensitiveFileServingX64(int target, bool bOn, out bool bOldSetting);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="bOn"></param>
        /// <param name="bOldSetting"></param>
        /// <returns></returns>
        public static SNRESULT SetCaseSensitiveFileServing(int target, bool bOn, out bool bOldSetting)
        {
            if (!Is32Bit())
                return SetCaseSensitiveFileServingX64(target, bOn, out bOldSetting);
            return SetCaseSensitiveFileServingX86(target, bOn, out bOldSetting);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3RegisterFTPEventHandler", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT RegisterFTPEventHandlerX86(int target, HandleEventCallbackPriv callback, IntPtr userData);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3RegisterFTPEventHandler", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT RegisterFTPEventHandlerX64(int target, HandleEventCallbackPriv callback, IntPtr userData);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="callback"></param>
        /// <param name="userData"></param>
        /// <returns></returns>
        public static SNRESULT RegisterFTPEventHandler(int target, FTPEventCallback callback, ref object userData)
        {
            if (callback == null)
                return SNRESULT.SN_E_BAD_PARAM;
            SNRESULT res = Is32Bit() ? RegisterFTPEventHandlerX86(target, ms_eventHandlerWrapper, IntPtr.Zero) : RegisterFTPEventHandlerX64(target, ms_eventHandlerWrapper, IntPtr.Zero);
            if (SUCCEEDED(res))
            {
                FtpCallbackAndUserData callbackAndUserData = new FtpCallbackAndUserData
                {
                    m_callback = callback,
                    m_userData = userData
                };
                if (ms_userFtpCallbacks == null)
                    ms_userFtpCallbacks = new Dictionary<int, FtpCallbackAndUserData>(1);
                ms_userFtpCallbacks[target] = callbackAndUserData;
            }
            return res;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3CancelFTPEvents", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT CancelFTPEventsX86(int target);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3CancelFTPEvents", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT CancelFTPEventsX64(int target);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static SNRESULT CancelFTPEvents(int target)
        {
            SNRESULT res = Is32Bit() ? CancelFTPEventsX86(target) : CancelFTPEventsX64(target);
            if (SUCCEEDED(res))
            {
                if (ms_userFtpCallbacks == null)
                    return SNRESULT.SN_E_ERROR;
                ms_userFtpCallbacks.Remove(target);
            }
            return res;
        }

        private static void MarshalFTPEvent(int target, uint param, SNRESULT result, uint length, IntPtr data)
        {
            FTPNotification[] ftpNotifications = new FTPNotification[0];
            if (length > 0U)
            {
                uint num = (uint)(length / (ulong)Marshal.SizeOf(typeof(FTPNotification)));
                ftpNotifications = new FTPNotification[(int)num];
                for (int index = 0; index < num; ++index)
                    data = ReadDataFromUnmanagedIncPtr(data, ref ftpNotifications[index]);
            }
            if (ms_userFtpCallbacks == null)
                return;
            ms_userFtpCallbacks[target].m_callback(target, result, ftpNotifications, ms_userFtpCallbacks[target].m_userData);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3RegisterFileTraceHandler", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT RegisterFileTraceHandlerX86(int target, HandleEventCallbackPriv callback, IntPtr userData);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3RegisterFileTraceHandler", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT RegisterFileTraceHandlerX64(int target, HandleEventCallbackPriv callback, IntPtr userData);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="callback"></param>
        /// <param name="userData"></param>
        /// <returns></returns>
        public static SNRESULT RegisterFileTraceHandler(int target, FileTraceCallback callback, ref object userData)
        {
            if (callback == null)
                return SNRESULT.SN_E_BAD_PARAM;
            SNRESULT res = Is32Bit() ? RegisterFileTraceHandlerX86(target, ms_eventHandlerWrapper, IntPtr.Zero) : RegisterFileTraceHandlerX64(target, ms_eventHandlerWrapper, IntPtr.Zero);
            if (SUCCEEDED(res))
            {
                FileTraceCallbackAndUserData callbackAndUserData = new FileTraceCallbackAndUserData
                {
                    m_callback = callback,
                    m_userData = userData
                };
                if (ms_userFileTraceCallbacks == null)
                    ms_userFileTraceCallbacks = new Dictionary<int, FileTraceCallbackAndUserData>(1);
                ms_userFileTraceCallbacks[target] = callbackAndUserData;
            }
            return res;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3UnRegisterFileTraceHandler", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT UnregisterFileTraceHandlerX86(int target);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3UnRegisterFileTraceHandler", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT UnregisterFileTraceHandlerX64(int target);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static SNRESULT UnregisterFileTraceHandler(int target)
        {
            SNRESULT res = Is32Bit() ? UnregisterFileTraceHandlerX86(target) : UnregisterFileTraceHandlerX64(target);
            if (SUCCEEDED(res))
            {
                if (ms_userFileTraceCallbacks == null)
                    return SNRESULT.SN_E_ERROR;
                ms_userFileTraceCallbacks.Remove(target);
            }
            return res;
        }

        private static void MarshalFileTraceEvent(int target, uint param, SNRESULT result, uint length, IntPtr data)
        {
            FileTraceEvent fileTraceEvent = new FileTraceEvent();
            IntPtr unmanagedBuf1 = data;
            uint num1 = 44;
            if (length < num1)
                return;
            IntPtr unmanagedBuf2 = ReadDataFromUnmanagedIncPtr(unmanagedBuf1, ref fileTraceEvent.SerialID);
            int storage1 = 0;
            IntPtr unmanagedBuf3 = ReadDataFromUnmanagedIncPtr(unmanagedBuf2, ref storage1);
            fileTraceEvent.TraceType = (FileTraceType)storage1;
            int storage2 = 0;
            IntPtr unmanagedBuf4 = ReadDataFromUnmanagedIncPtr(unmanagedBuf3, ref storage2);
            fileTraceEvent.Status = (FileTraceNotificationStatus)storage2;
            IntPtr unmanagedBuf5 = ReadDataFromUnmanagedIncPtr(ReadDataFromUnmanagedIncPtr(ReadDataFromUnmanagedIncPtr(ReadDataFromUnmanagedIncPtr(unmanagedBuf4, ref fileTraceEvent.ProcessID), ref fileTraceEvent.ThreadID), ref fileTraceEvent.TimeBaseStartOfTrace), ref fileTraceEvent.TimeBase);
            uint storage3 = 0;
            IntPtr unmanagedBuf6 = ReadDataFromUnmanagedIncPtr(unmanagedBuf5, ref storage3);
            uint num2 = num1 + storage3;
            if (length < num2)
                return;
            fileTraceEvent.BackTraceData = new byte[(int)storage3];
            for (int index = 0; index < storage3; ++index)
                unmanagedBuf6 = ReadDataFromUnmanagedIncPtr(unmanagedBuf6, ref fileTraceEvent.BackTraceData[index]);
            IntPtr num3;
            switch (fileTraceEvent.TraceType)
            {
                case FileTraceType.GetBlockSize:
                case FileTraceType.Stat:
                case FileTraceType.WidgetStat:
                case FileTraceType.Unlink:
                case FileTraceType.WidgetUnlink:
                case FileTraceType.RMDir:
                case FileTraceType.WidgetRMDir:
                    fileTraceEvent.LogData.LogType1 = new FileTraceLogType1();
                    uint storage4 = 0;
                    IntPtr utf8Ptr1 = ReadDataFromUnmanagedIncPtr(unmanagedBuf6, ref storage4);
                    if (storage4 > 0U)
                    {
                        fileTraceEvent.LogData.LogType1.Path = Utf8ToString(utf8Ptr1, storage4);
                        break;
                    }
                    break;
                case FileTraceType.Rename:
                case FileTraceType.WidgetRename:
                    fileTraceEvent.LogData.LogType2 = new FileTraceLogType2();
                    uint storage5 = 0;
                    IntPtr unmanagedBuf7 = ReadDataFromUnmanagedIncPtr(unmanagedBuf6, ref storage5);
                    uint storage6 = 0;
                    IntPtr utf8Ptr2 = ReadDataFromUnmanagedIncPtr(unmanagedBuf7, ref storage6);
                    if (storage5 > 0U)
                    {
                        fileTraceEvent.LogData.LogType2.Path1 = Utf8ToString(utf8Ptr2, storage5);
                        utf8Ptr2 = new IntPtr(utf8Ptr2.ToInt64() + storage5);
                    }
                    if (storage6 > 0U)
                    {
                        fileTraceEvent.LogData.LogType2.Path2 = Utf8ToString(utf8Ptr2, storage6);
                        break;
                    }
                    break;
                case FileTraceType.Truncate:
                case FileTraceType.TruncateNoAlloc:
                case FileTraceType.Truncate2:
                case FileTraceType.Truncate2NoInit:
                    fileTraceEvent.LogData.LogType3 = new FileTraceLogType3();
                    IntPtr unmanagedBuf8 = ReadDataFromUnmanagedIncPtr(unmanagedBuf6, ref fileTraceEvent.LogData.LogType3.Arg);
                    uint storage7 = 0;
                    IntPtr utf8Ptr3 = ReadDataFromUnmanagedIncPtr(unmanagedBuf8, ref storage7);
                    if (storage7 > 0U)
                    {
                        fileTraceEvent.LogData.LogType3.Path = Utf8ToString(utf8Ptr3, storage7);
                        break;
                    }
                    break;
                case FileTraceType.OpenDir:
                case FileTraceType.WidgetOpenDir:
                case FileTraceType.CHMod:
                case FileTraceType.MkDir:
                    fileTraceEvent.LogData.LogType4 = new FileTraceLogType4();
                    IntPtr unmanagedBuf9 = ReadDataFromUnmanagedIncPtr(unmanagedBuf6, ref fileTraceEvent.LogData.LogType4.Mode);
                    uint storage8 = 0;
                    IntPtr utf8Ptr4 = ReadDataFromUnmanagedIncPtr(unmanagedBuf9, ref storage8);
                    if (storage8 > 0U)
                    {
                        fileTraceEvent.LogData.LogType4.Path = Utf8ToString(utf8Ptr4, storage8);
                        break;
                    }
                    break;
                case FileTraceType.UTime:
                    fileTraceEvent.LogData.LogType6 = new FileTraceLogType6();
                    IntPtr unmanagedBuf10 = ReadDataFromUnmanagedIncPtr(ReadDataFromUnmanagedIncPtr(unmanagedBuf6, ref fileTraceEvent.LogData.LogType6.Arg1), ref fileTraceEvent.LogData.LogType6.Arg2);
                    uint storage9 = 0;
                    IntPtr utf8Ptr5 = ReadDataFromUnmanagedIncPtr(unmanagedBuf10, ref storage9);
                    if (storage9 > 0U)
                    {
                        fileTraceEvent.LogData.LogType6.Path = Utf8ToString(utf8Ptr5, storage9);
                        break;
                    }
                    break;
                case FileTraceType.Open:
                case FileTraceType.WidgetOpen:
                    fileTraceEvent.LogData.LogType8 = new FileTraceLogType8();
                    IntPtr unmanagedBuf11 = ReadDataFromUnmanagedIncPtr(ReadDataFromUnmanagedIncPtr(ReadDataFromUnmanagedIncPtr(ReadDataFromUnmanagedIncPtr(ReadDataFromUnmanagedIncPtr(unmanagedBuf6, ref fileTraceEvent.LogData.LogType8.ProcessInfo), ref fileTraceEvent.LogData.LogType8.Arg1), ref fileTraceEvent.LogData.LogType8.Arg2), ref fileTraceEvent.LogData.LogType8.Arg3), ref fileTraceEvent.LogData.LogType8.Arg4);
                    uint storage10 = 0;
                    IntPtr unmanagedBuf12 = ReadDataFromUnmanagedIncPtr(unmanagedBuf11, ref storage10);
                    uint storage11 = 0;
                    IntPtr num4 = ReadDataFromUnmanagedIncPtr(unmanagedBuf12, ref storage11);
                    fileTraceEvent.LogData.LogType8.VArg = new byte[(int)storage10];
                    for (int index = 0; index < storage10; ++index)
                        num4 = ReadDataFromUnmanagedIncPtr(num4, ref fileTraceEvent.LogData.LogType8.VArg[index]);
                    if (storage11 > 0U)
                    {
                        fileTraceEvent.LogData.LogType8.Path = Utf8ToString(num4, storage11);
                        break;
                    }
                    break;
                case FileTraceType.Close:
                case FileTraceType.CloseDir:
                case FileTraceType.FSync:
                case FileTraceType.ReadDir:
                case FileTraceType.FStat:
                case FileTraceType.FGetBlockSize:
                    fileTraceEvent.LogData.LogType9 = new FileTraceLogType9();
                    num3 = ReadDataFromUnmanagedIncPtr(unmanagedBuf6, ref fileTraceEvent.LogData.LogType9.ProcessInfo);
                    break;
                case FileTraceType.Read:
                case FileTraceType.Write:
                case FileTraceType.GetDirEntries:
                    fileTraceEvent.LogData.LogType10 = new FileTraceLogType10();
                    num3 = ReadDataFromUnmanagedIncPtr(ReadDataFromUnmanagedIncPtr(ReadDataFromUnmanagedIncPtr(ReadDataFromUnmanagedIncPtr(unmanagedBuf6, ref fileTraceEvent.LogData.LogType10.ProcessInfo), ref fileTraceEvent.LogData.LogType10.Size), ref fileTraceEvent.LogData.LogType10.Address), ref fileTraceEvent.LogData.LogType10.TxSize);
                    break;
                case FileTraceType.ReadOffset:
                case FileTraceType.WriteOffset:
                    fileTraceEvent.LogData.LogType11 = new FileTraceLogType11();
                    ReadDataFromUnmanagedIncPtr(ReadDataFromUnmanagedIncPtr(ReadDataFromUnmanagedIncPtr(ReadDataFromUnmanagedIncPtr(ReadDataFromUnmanagedIncPtr(unmanagedBuf6, ref fileTraceEvent.LogData.LogType11.ProcessInfo), ref fileTraceEvent.LogData.LogType11.Size), ref fileTraceEvent.LogData.LogType11.Address), ref fileTraceEvent.LogData.LogType11.Offset), ref fileTraceEvent.LogData.LogType11.TxSize);
                    break;
                case FileTraceType.FTruncate:
                case FileTraceType.FTruncateNoAlloc:
                    fileTraceEvent.LogData.LogType12 = new FileTraceLogType12();
                    ReadDataFromUnmanagedIncPtr(ReadDataFromUnmanagedIncPtr(unmanagedBuf6, ref fileTraceEvent.LogData.LogType12.ProcessInfo), ref fileTraceEvent.LogData.LogType12.TargetSize);
                    break;
                case FileTraceType.LSeek:
                    fileTraceEvent.LogData.LogType13 = new FileTraceLogType13();
                    ReadDataFromUnmanagedIncPtr(ReadDataFromUnmanagedIncPtr(ReadDataFromUnmanagedIncPtr(ReadDataFromUnmanagedIncPtr(unmanagedBuf6, ref fileTraceEvent.LogData.LogType13.ProcessInfo), ref fileTraceEvent.LogData.LogType13.Size), ref fileTraceEvent.LogData.LogType13.Offset), ref fileTraceEvent.LogData.LogType13.CurPos);
                    break;
                case FileTraceType.SetIOBuffer:
                    fileTraceEvent.LogData.LogType14 = new FileTraceLogType14();
                    ReadDataFromUnmanagedIncPtr(ReadDataFromUnmanagedIncPtr(ReadDataFromUnmanagedIncPtr(ReadDataFromUnmanagedIncPtr(unmanagedBuf6, ref fileTraceEvent.LogData.LogType14.ProcessInfo), ref fileTraceEvent.LogData.LogType14.MaxSize), ref fileTraceEvent.LogData.LogType14.Page), ref fileTraceEvent.LogData.LogType14.ContainerID);
                    break;
            }
            if (ms_userFileTraceCallbacks == null)
                return;
            ms_userFileTraceCallbacks[target].m_callback(target, result, fileTraceEvent, ms_userFileTraceCallbacks[target].m_userData);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3StartFileTrace", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT StartFileTraceX86(int target, uint processId, uint size, IntPtr filename);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3StartFileTrace", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT StartFileTraceX64(int target, uint processId, uint size, IntPtr filename);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="processID"></param>
        /// <param name="size"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static SNRESULT StartFileTrace(int target, uint processID, uint size, string filename)
        {
            ScopedGlobalHeapPtr scopedGlobalHeapPtr = new ScopedGlobalHeapPtr(AllocUtf8FromString(filename));
            if (!Is32Bit())
                return StartFileTraceX64(target, processID, size, scopedGlobalHeapPtr.Get());
            return StartFileTraceX86(target, processID, size, scopedGlobalHeapPtr.Get());
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3StopFileTrace", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT StopFileTraceX86(int target, uint processId);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3StopFileTrace", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT StopFileTraceX64(int target, uint processId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="processID"></param>
        /// <returns></returns>
        public static SNRESULT StopFileTrace(int target, uint processID)
        {
            if (!Is32Bit())
                return StopFileTraceX64(target, processID);
            return StopFileTraceX86(target, processID);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3InstallPackage", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT InstallPackageX86(int target, IntPtr packagePath);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3InstallPackage", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT InstallPackageX64(int target, IntPtr packagePath);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="packagePath"></param>
        /// <returns></returns>
        public static SNRESULT InstallPackage(int target, string packagePath)
        {
            ScopedGlobalHeapPtr scopedGlobalHeapPtr = new ScopedGlobalHeapPtr(AllocUtf8FromString(packagePath));
            if (!Is32Bit())
                return InstallPackageX64(target, scopedGlobalHeapPtr.Get());
            return InstallPackageX86(target, scopedGlobalHeapPtr.Get());
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3UploadFile", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT UploadFileX86(int target, IntPtr source, IntPtr dest, out uint transactionId);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3UploadFile", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT UploadFileX64(int target, IntPtr source, IntPtr dest, out uint transactionId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="source"></param>
        /// <param name="dest"></param>
        /// <param name="txID"></param>
        /// <returns></returns>
        public static SNRESULT UploadFile(int target, string source, string dest, out uint txID)
        {
            ScopedGlobalHeapPtr scopedGlobalHeapPtr1 = new ScopedGlobalHeapPtr(AllocUtf8FromString(source));
            ScopedGlobalHeapPtr scopedGlobalHeapPtr2 = new ScopedGlobalHeapPtr(AllocUtf8FromString(dest));
            if (!Is32Bit())
                return UploadFileX64(target, scopedGlobalHeapPtr1.Get(), scopedGlobalHeapPtr2.Get(), out txID);
            return UploadFileX86(target, scopedGlobalHeapPtr1.Get(), scopedGlobalHeapPtr2.Get(), out txID);
        }

        private static IntPtr FileTransferInfoMarshalHelper(IntPtr dataPtr, ref FileTransferInfo fileTransferInfo)
        {
            FileTransferInfoPriv storage = new FileTransferInfoPriv();
            dataPtr = ReadDataFromUnmanagedIncPtr(dataPtr, ref storage);
            fileTransferInfo.TransferID = storage.TransferId;
            fileTransferInfo.Status = (FileTransferStatus)storage.Status;
            fileTransferInfo.Size = storage.Size;
            fileTransferInfo.BytesRead = storage.BytesRead;
            fileTransferInfo.SourcePath = Utf8FixedSizeByteArrayToString(storage.SourcePath);
            fileTransferInfo.DestinationPath = Utf8FixedSizeByteArrayToString(storage.DestinationPath);
            return dataPtr;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetFileTransferList", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetFileTransferListX86(int target, ref uint count, IntPtr fileTransferInfo);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetFileTransferList", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetFileTransferListX64(int target, ref uint count, IntPtr fileTransferInfo);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="fileTransfers"></param>
        /// <returns></returns>
        public static SNRESULT GetFileTransferList(int target, out FileTransferInfo[] fileTransfers)
        {
            fileTransfers = null;
            uint count = 0;
            SNRESULT res1 = Is32Bit() ? GetFileTransferListX86(target, ref count, IntPtr.Zero) : GetFileTransferListX64(target, ref count, IntPtr.Zero);
            if (FAILED(res1))
                return res1;
            ScopedGlobalHeapPtr scopedGlobalHeapPtr = new ScopedGlobalHeapPtr(Marshal.AllocHGlobal((int)(Marshal.SizeOf(typeof(FileTransferInfoPriv)) * count)));
            SNRESULT res2 = Is32Bit() ? GetFileTransferListX86(target, ref count, scopedGlobalHeapPtr.Get()) : GetFileTransferListX64(target, ref count, scopedGlobalHeapPtr.Get());
            if (FAILED(res2))
                return res2;
            IntPtr dataPtr = scopedGlobalHeapPtr.Get();
            fileTransfers = new FileTransferInfo[(int)count];
            for (uint index = 0; index < count; ++index)
                dataPtr = FileTransferInfoMarshalHelper(dataPtr, ref fileTransfers[(int)index]);
            return res2;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetFileTransferInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetFileTransferInfoX86(int target, uint txId, IntPtr fileTransferInfoPtr);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetFileTransferInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetFileTransferInfoX64(int target, uint txId, IntPtr fileTransferInfoPtr);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="txID"></param>
        /// <param name="fileTransferInfo"></param>
        /// <returns></returns>
        public static SNRESULT GetFileTransferInfo(int target, uint txID, out FileTransferInfo fileTransferInfo)
        {
            fileTransferInfo = new FileTransferInfo();
            ScopedGlobalHeapPtr scopedGlobalHeapPtr = new ScopedGlobalHeapPtr(Marshal.AllocHGlobal(Marshal.SizeOf(typeof(FileTransferInfoPriv))));
            SNRESULT res = Is32Bit() ? GetFileTransferInfoX86(target, txID, scopedGlobalHeapPtr.Get()) : GetFileTransferInfoX64(target, txID, scopedGlobalHeapPtr.Get());
            if (SUCCEEDED(res))
                FileTransferInfoMarshalHelper(scopedGlobalHeapPtr.Get(), ref fileTransferInfo);
            return res;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3CancelFileTransfer", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT CancelFileTransferX86(int target, uint txID);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3CancelFileTransfer", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT CancelFileTransferX64(int target, uint txID);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="txID"></param>
        /// <returns></returns>
        public static SNRESULT CancelFileTransfer(int target, uint txID)
        {
            if (!Is32Bit())
                return CancelFileTransferX64(target, txID);
            return CancelFileTransferX86(target, txID);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3RetryFileTransfer", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT RetryFileTransferX86(int target, uint txID, bool bForce);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3RetryFileTransfer", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT RetryFileTransferX64(int target, uint txID, bool bForce);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="txID"></param>
        /// <param name="bForce"></param>
        /// <returns></returns>
        public static SNRESULT RetryFileTransfer(int target, uint txID, bool bForce)
        {
            if (!Is32Bit())
                return RetryFileTransferX64(target, txID, bForce);
            return RetryFileTransferX86(target, txID, bForce);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3RemoveTransferItemsByStatus", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT RemoveTransferItemsByStatusX86(int target, uint filter);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3RemoveTransferItemsByStatus", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SNPS3RemoveTransferItemsByStatusX64(int target, uint filter);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static SNRESULT RemoveTransferItemsByStatus(int target, uint filter)
        {
            if (!Is32Bit())
                return SNPS3RemoveTransferItemsByStatusX64(target, filter);
            return RemoveTransferItemsByStatusX86(target, filter);
        }

        private static IntPtr DirEntryMarshalHelper(IntPtr dataPtr, ref DirEntry dirEntry)
        {
            DirEntryPriv storage = new DirEntryPriv();
            dataPtr = ReadDataFromUnmanagedIncPtr(dataPtr, ref storage);
            dirEntry.Type = (DirEntryType)storage.Type;
            dirEntry.Mode = storage.Mode;
            dirEntry.AccessTime = storage.AccessTime;
            dirEntry.ModifiedTime = storage.ModifiedTime;
            dirEntry.CreateTime = storage.CreateTime;
            dirEntry.Size = storage.Size;
            dirEntry.Name = Utf8FixedSizeByteArrayToString(storage.Name);
            return dataPtr;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetDirectoryList", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetDirectoryListX86(int target, IntPtr directory, ref uint numDirEntries, IntPtr dirEntryList);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetDirectoryList", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetDirectoryListX64(int target, IntPtr directory, ref uint numDirEntries, IntPtr dirEntryList);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="directory"></param>
        /// <param name="dirEntries"></param>
        /// <returns></returns>
        public static SNRESULT GetDirectoryList(int target, string directory, out DirEntry[] dirEntries)
        {
            dirEntries = null;
            uint numDirEntries = 0;
            ScopedGlobalHeapPtr scopedGlobalHeapPtr1 = new ScopedGlobalHeapPtr(AllocUtf8FromString(directory));
            SNRESULT res1 = Is32Bit() ? GetDirectoryListX86(target, scopedGlobalHeapPtr1.Get(), ref numDirEntries, IntPtr.Zero) : GetDirectoryListX64(target, scopedGlobalHeapPtr1.Get(), ref numDirEntries, IntPtr.Zero);
            if (FAILED(res1))
                return res1;
            ScopedGlobalHeapPtr scopedGlobalHeapPtr2 = new ScopedGlobalHeapPtr(Marshal.AllocHGlobal((int)numDirEntries * Marshal.SizeOf(typeof(DirEntryPriv))));
            SNRESULT res2 = Is32Bit() ? GetDirectoryListX86(target, scopedGlobalHeapPtr1.Get(), ref numDirEntries, scopedGlobalHeapPtr2.Get()) : GetDirectoryListX64(target, scopedGlobalHeapPtr1.Get(), ref numDirEntries, scopedGlobalHeapPtr2.Get());
            if (SUCCEEDED(res2))
            {
                IntPtr dataPtr = scopedGlobalHeapPtr2.Get();
                dirEntries = new DirEntry[(int)numDirEntries];
                for (int index = 0; index < numDirEntries; ++index)
                    dataPtr = DirEntryMarshalHelper(dataPtr, ref dirEntries[index]);
            }
            return res2;
        }

        private static IntPtr DirEntryExMarshalHelper(IntPtr dataPtr, ref DirEntryEx dirEntryEx)
        {
            DirEntryExPriv storage = new DirEntryExPriv();
            dataPtr = ReadDataFromUnmanagedIncPtr(dataPtr, ref storage);
            dirEntryEx.Type = (DirEntryType)storage.Type;
            dirEntryEx.Mode = storage.Mode;
            dirEntryEx.AccessTimeUTC = storage.AccessTimeUTC;
            dirEntryEx.ModifiedTimeUTC = storage.ModifiedTimeUTC;
            dirEntryEx.CreateTimeUTC = storage.CreateTimeUTC;
            dirEntryEx.Size = storage.Size;
            dirEntryEx.Name = Utf8FixedSizeByteArrayToString(storage.Name);
            return dataPtr;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetDirectoryListEx", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetDirectoryListExX86(int target, IntPtr dirPtr, ref uint numDirEntries, IntPtr dirEntryListEx, ref TargetTimezone timeZone);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetDirectoryListEx", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetDirectoryListExX64(int target, IntPtr dirPtr, ref uint numDirEntries, IntPtr dirEntryListEx, ref TargetTimezone timeZone);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="directory"></param>
        /// <param name="dirEntries"></param>
        /// <param name="timeZone"></param>
        /// <returns></returns>
        public static SNRESULT GetDirectoryListEx(int target, string directory, out DirEntryEx[] dirEntries, out TargetTimezone timeZone)
        {
            dirEntries = null;
            timeZone = new TargetTimezone();
            uint numDirEntries = 0;
            ScopedGlobalHeapPtr scopedGlobalHeapPtr1 = new ScopedGlobalHeapPtr(AllocUtf8FromString(directory));
            SNRESULT res1 = Is32Bit() ? GetDirectoryListExX86(target, scopedGlobalHeapPtr1.Get(), ref numDirEntries, IntPtr.Zero, ref timeZone) : GetDirectoryListExX64(target, scopedGlobalHeapPtr1.Get(), ref numDirEntries, IntPtr.Zero, ref timeZone);
            if (FAILED(res1))
                return res1;
            ScopedGlobalHeapPtr scopedGlobalHeapPtr2 = new ScopedGlobalHeapPtr(Marshal.AllocHGlobal((int)numDirEntries * Marshal.SizeOf(typeof(DirEntryExPriv))));
            SNRESULT res2 = Is32Bit() ? GetDirectoryListExX86(target, scopedGlobalHeapPtr1.Get(), ref numDirEntries, scopedGlobalHeapPtr2.Get(), ref timeZone) : GetDirectoryListExX64(target, scopedGlobalHeapPtr1.Get(), ref numDirEntries, scopedGlobalHeapPtr2.Get(), ref timeZone);
            if (SUCCEEDED(res2))
            {
                IntPtr dataPtr = scopedGlobalHeapPtr2.Get();
                dirEntries = new DirEntryEx[(int)numDirEntries];
                for (int index = 0; index < numDirEntries; ++index)
                    dataPtr = DirEntryExMarshalHelper(dataPtr, ref dirEntries[index]);
            }
            return res2;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3MakeDirectory", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT MakeDirectoryX86(int target, IntPtr directory, uint mode);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3MakeDirectory", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT MakeDirectoryX64(int target, IntPtr directory, uint mode);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="directory"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static SNRESULT MakeDirectory(int target, string directory, uint mode)
        {
            ScopedGlobalHeapPtr scopedGlobalHeapPtr = new ScopedGlobalHeapPtr(AllocUtf8FromString(directory));
            if (!Is32Bit())
                return MakeDirectoryX64(target, scopedGlobalHeapPtr.Get(), mode);
            return MakeDirectoryX86(target, scopedGlobalHeapPtr.Get(), mode);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3Delete", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT DeleteFileX86(int target, IntPtr path);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3Delete", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT DeleteFileX64(int target, IntPtr path);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static SNRESULT DeleteFile(int target, string path)
        {
            ScopedGlobalHeapPtr scopedGlobalHeapPtr = new ScopedGlobalHeapPtr(AllocUtf8FromString(path));
            if (!Is32Bit())
                return DeleteFileX64(target, scopedGlobalHeapPtr.Get());
            return DeleteFileX86(target, scopedGlobalHeapPtr.Get());
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3DeleteEx", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT DeleteFileExX86(int target, IntPtr path, uint msTimeout);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3DeleteEx", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT DeleteFileExX64(int target, IntPtr path, uint msTimeout);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="path"></param>
        /// <param name="msTimeout"></param>
        /// <returns></returns>
        public static SNRESULT DeleteFileEx(int target, string path, uint msTimeout)
        {
            ScopedGlobalHeapPtr scopedGlobalHeapPtr = new ScopedGlobalHeapPtr(AllocUtf8FromString(path));
            if (!Is32Bit())
                return DeleteFileExX64(target, scopedGlobalHeapPtr.Get(), msTimeout);
            return DeleteFileExX86(target, scopedGlobalHeapPtr.Get(), msTimeout);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3Rename", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT RenameFileX86(int target, IntPtr source, IntPtr dest);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3Rename", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT RenameFileX64(int target, IntPtr source, IntPtr dest);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="source"></param>
        /// <param name="dest"></param>
        /// <returns></returns>
        public static SNRESULT RenameFile(int target, string source, string dest)
        {
            ScopedGlobalHeapPtr scopedGlobalHeapPtr1 = new ScopedGlobalHeapPtr(AllocUtf8FromString(source));
            ScopedGlobalHeapPtr scopedGlobalHeapPtr2 = new ScopedGlobalHeapPtr(AllocUtf8FromString(dest));
            if (!Is32Bit())
                return RenameFileX64(target, scopedGlobalHeapPtr1.Get(), scopedGlobalHeapPtr2.Get());
            return RenameFileX86(target, scopedGlobalHeapPtr1.Get(), scopedGlobalHeapPtr2.Get());
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3DownloadFile", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT DownloadFileX86(int target, IntPtr source, IntPtr dest, out uint transactionId);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3DownloadFile", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT DownloadFileX64(int target, IntPtr source, IntPtr dest, out uint transactionId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="source"></param>
        /// <param name="dest"></param>
        /// <param name="txID"></param>
        /// <returns></returns>
        public static SNRESULT DownloadFile(int target, string source, string dest, out uint txID)
        {
            ScopedGlobalHeapPtr scopedGlobalHeapPtr1 = new ScopedGlobalHeapPtr(AllocUtf8FromString(source));
            ScopedGlobalHeapPtr scopedGlobalHeapPtr2 = new ScopedGlobalHeapPtr(AllocUtf8FromString(dest));
            if (!Is32Bit())
                return DownloadFileX64(target, scopedGlobalHeapPtr1.Get(), scopedGlobalHeapPtr2.Get(), out txID);
            return DownloadFileX86(target, scopedGlobalHeapPtr1.Get(), scopedGlobalHeapPtr2.Get(), out txID);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3DownloadDirectory", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT DownloadDirectoryX86(int target, IntPtr source, IntPtr dest, out uint lastTransactionId);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3DownloadDirectory", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT DownloadDirectoryX64(int target, IntPtr source, IntPtr dest, out uint lastTransactionId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="source"></param>
        /// <param name="dest"></param>
        /// <param name="lastTxID"></param>
        /// <returns></returns>
        public static SNRESULT DownloadDirectory(int target, string source, string dest, out uint lastTxID)
        {
            ScopedGlobalHeapPtr scopedGlobalHeapPtr1 = new ScopedGlobalHeapPtr(AllocUtf8FromString(source));
            ScopedGlobalHeapPtr scopedGlobalHeapPtr2 = new ScopedGlobalHeapPtr(AllocUtf8FromString(dest));
            if (!Is32Bit())
                return DownloadDirectoryX64(target, scopedGlobalHeapPtr1.Get(), scopedGlobalHeapPtr2.Get(), out lastTxID);
            return DownloadDirectoryX86(target, scopedGlobalHeapPtr1.Get(), scopedGlobalHeapPtr2.Get(), out lastTxID);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3UploadDirectory", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT UploadDirectoryX86(int target, IntPtr source, IntPtr dest, out uint lastTransactionId);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3UploadDirectory", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT UploadDirectoryX64(int target, IntPtr source, IntPtr dest, out uint lastTransactionId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="source"></param>
        /// <param name="dest"></param>
        /// <param name="lastTxID"></param>
        /// <returns></returns>
        public static SNRESULT UploadDirectory(int target, string source, string dest, out uint lastTxID)
        {
            ScopedGlobalHeapPtr scopedGlobalHeapPtr1 = new ScopedGlobalHeapPtr(AllocUtf8FromString(source));
            ScopedGlobalHeapPtr scopedGlobalHeapPtr2 = new ScopedGlobalHeapPtr(AllocUtf8FromString(dest));
            if (!Is32Bit())
                return UploadDirectoryX64(target, scopedGlobalHeapPtr1.Get(), scopedGlobalHeapPtr2.Get(), out lastTxID);
            return UploadDirectoryX86(target, scopedGlobalHeapPtr1.Get(), scopedGlobalHeapPtr2.Get(), out lastTxID);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3StatTargetFile", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT StatTargetFileX86(int target, IntPtr file, IntPtr dirEntry);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3StatTargetFile", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT StatTargetFileX64(int target, IntPtr file, IntPtr dirEntry);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="file"></param>
        /// <param name="dirEntry"></param>
        /// <returns></returns>
        public static SNRESULT StatTargetFile(int target, string file, out DirEntry dirEntry)
        {
            dirEntry = new DirEntry();
            ScopedGlobalHeapPtr scopedGlobalHeapPtr1 = new ScopedGlobalHeapPtr(AllocUtf8FromString(file));
            ScopedGlobalHeapPtr scopedGlobalHeapPtr2 = new ScopedGlobalHeapPtr(Marshal.AllocHGlobal(Marshal.SizeOf(typeof(DirEntryPriv))));
            SNRESULT snresult = Is32Bit() ? StatTargetFileX86(target, scopedGlobalHeapPtr1.Get(), scopedGlobalHeapPtr2.Get()) : StatTargetFileX64(target, scopedGlobalHeapPtr1.Get(), scopedGlobalHeapPtr2.Get());
            DirEntryMarshalHelper(scopedGlobalHeapPtr2.Get(), ref dirEntry);
            return snresult;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3StatTargetFileEx", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT StatTargetFileExX86(int target, IntPtr file, IntPtr dirEntry, out TargetTimezone timeZone);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3StatTargetFileEx", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT StatTargetFileExX64(int target, IntPtr file, IntPtr dirEntry, out TargetTimezone timeZone);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="file"></param>
        /// <param name="dirEntryEx"></param>
        /// <param name="timeZone"></param>
        /// <returns></returns>
        public static SNRESULT StatTargetFileEx(int target, string file, out DirEntryEx dirEntryEx, out TargetTimezone timeZone)
        {
            dirEntryEx = new DirEntryEx();
            ScopedGlobalHeapPtr scopedGlobalHeapPtr1 = new ScopedGlobalHeapPtr(AllocUtf8FromString(file));
            ScopedGlobalHeapPtr scopedGlobalHeapPtr2 = new ScopedGlobalHeapPtr(Marshal.AllocHGlobal(Marshal.SizeOf(typeof(DirEntryExPriv))));
            SNRESULT snresult = Is32Bit() ? StatTargetFileExX86(target, scopedGlobalHeapPtr1.Get(), scopedGlobalHeapPtr2.Get(), out timeZone) : StatTargetFileExX64(target, scopedGlobalHeapPtr1.Get(), scopedGlobalHeapPtr2.Get(), out timeZone);
            DirEntryExMarshalHelper(scopedGlobalHeapPtr2.Get(), ref dirEntryEx);
            return snresult;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3CHMod", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT CHModX86(int target, IntPtr filePath, uint mode);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3CHMod", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT CHModX64(int target, IntPtr filePath, uint mode);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="filePath"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static SNRESULT ChMod(int target, string filePath, ChModFilePermission mode)
        {
            ScopedGlobalHeapPtr scopedGlobalHeapPtr = new ScopedGlobalHeapPtr(AllocUtf8FromString(filePath));
            if (!Is32Bit())
                return CHModX64(target, scopedGlobalHeapPtr.Get(), (uint)mode);
            return CHModX86(target, scopedGlobalHeapPtr.Get(), (uint)mode);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SetFileTime", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetFileTimeX86(int target, IntPtr filePath, ulong accessTime, ulong modifiedTime);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SetFileTime", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetFileTimeX64(int target, IntPtr filePath, ulong accessTime, ulong modifiedTime);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="filePath"></param>
        /// <param name="accessTime"></param>
        /// <param name="modifiedTime"></param>
        /// <returns></returns>
        public static SNRESULT SetFileTime(int target, string filePath, ulong accessTime, ulong modifiedTime)
        {
            ScopedGlobalHeapPtr scopedGlobalHeapPtr = new ScopedGlobalHeapPtr(AllocUtf8FromString(filePath));
            if (!Is32Bit())
                return SetFileTimeX64(target, scopedGlobalHeapPtr.Get(), accessTime, modifiedTime);
            return SetFileTimeX86(target, scopedGlobalHeapPtr.Get(), accessTime, modifiedTime);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3InstallGameEx", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT InstallGameExX86(int target, IntPtr paramSfoPath, out IntPtr titleId, out IntPtr targetPath, out uint txId);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3InstallGameEx", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT InstallGameExX64(int target, IntPtr paramSfoPath, out IntPtr titleId, out IntPtr targetPath, out uint txId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="paramSFOPath"></param>
        /// <param name="titleID"></param>
        /// <param name="targetPath"></param>
        /// <param name="txID"></param>
        /// <returns></returns>
        public static SNRESULT InstallGameEx(int target, string paramSFOPath, out string titleID, out string targetPath, out uint txID)
        {
            ScopedGlobalHeapPtr scopedGlobalHeapPtr = new ScopedGlobalHeapPtr(AllocUtf8FromString(paramSFOPath));
            SNRESULT snresult = Is32Bit() ? InstallGameExX86(target, scopedGlobalHeapPtr.Get(), out IntPtr titleId, out IntPtr targetPath1, out txID) : InstallGameExX64(target, scopedGlobalHeapPtr.Get(), out titleId, out targetPath1, out txID);
            titleID = Utf8ToString(titleId, uint.MaxValue);
            targetPath = Utf8ToString(targetPath1, uint.MaxValue);
            return snresult;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3FormatHDD", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT FormatHDDX86(int target, uint initRegistry);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3FormatHDD", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT FormatHDDX64(int target, uint initRegistry);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="initRegistry"></param>
        /// <returns></returns>
        public static SNRESULT FormatHDD(int target, uint initRegistry)
        {
            if (!Is32Bit())
                return FormatHDDX64(target, initRegistry);
            return FormatHDDX86(target, initRegistry);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3UninstallGame", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT UninstallGameX86(int target, IntPtr gameDirectory);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3UninstallGame", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT UninstallGameX64(int target, IntPtr gameDirectory);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="gameDirectory"></param>
        /// <returns></returns>
        public static SNRESULT UninstallGame(int target, string gameDirectory)
        {
            ScopedGlobalHeapPtr scopedGlobalHeapPtr = new ScopedGlobalHeapPtr(AllocUtf8FromString(gameDirectory));
            if (!Is32Bit())
                return UninstallGameX64(target, scopedGlobalHeapPtr.Get());
            return UninstallGameX86(target, scopedGlobalHeapPtr.Get());
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3WaitForFileTransfer", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT WaitForFileTransferX86(int target, uint txId, out FileTransferNotificationType notificationType, uint msTimeout);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3WaitForFileTransfer", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT WaitForFileTransferX64(int target, uint txId, out FileTransferNotificationType notificationType, uint msTimeout);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="txID"></param>
        /// <param name="notificationType"></param>
        /// <param name="msTimeout"></param>
        /// <returns></returns>
        public static SNRESULT WaitForFileTransfer(int target, uint txID, out FileTransferNotificationType notificationType, uint msTimeout)
        {
            return Is32Bit() ? WaitForFileTransferX86(target, txID, out notificationType, msTimeout) : WaitForFileTransferX64(target, txID, out notificationType, msTimeout);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3FSGetFreeSize", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT FSGetFreeSizeX86(int target, IntPtr fsDir, out uint blockSize, out ulong freeBlockCount);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3FSGetFreeSize", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT FSGetFreeSizeX64(int target, IntPtr fsDir, out uint blockSize, out ulong freeBlockCount);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="fsDir"></param>
        /// <param name="blockSize"></param>
        /// <param name="freeBlockCount"></param>
        /// <returns></returns>
        public static SNRESULT FSGetFreeSize(int target, string fsDir, out uint blockSize, out ulong freeBlockCount)
        {
            ScopedGlobalHeapPtr scopedGlobalHeapPtr = new ScopedGlobalHeapPtr(AllocUtf8FromString(fsDir));
            if (!Is32Bit())
                return FSGetFreeSizeX64(target, scopedGlobalHeapPtr.Get(), out blockSize, out freeBlockCount);
            return FSGetFreeSizeX86(target, scopedGlobalHeapPtr.Get(), out blockSize, out freeBlockCount);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetLogOptions", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetLogOptionsX86(out LogCategory category);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetLogOptions", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetLogOptionsX64(out LogCategory category);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public static SNRESULT GetLogOptions(out LogCategory category)
        {
            return Is32Bit() ? GetLogOptionsX86(out category) : GetLogOptionsX64(out category);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SetLogOptions", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetLogOptionsX86(LogCategory category);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SetLogOptions", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetLogOptionsX64(LogCategory category);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public static SNRESULT SetLogOptions(LogCategory category)
        {
            return Is32Bit() ? SetLogOptionsX86(category) : SetLogOptionsX64(category);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3EnableInternalKick", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT EnableInternalKickX86(bool enable);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3EnableInternalKick", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT EnableInternalKickX64(bool enable);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bEnable"></param>
        /// <returns></returns>
        public static SNRESULT EnableInternalKick(bool bEnable)
        {
            if (!Is32Bit())
                return EnableInternalKickX64(bEnable);
            return EnableInternalKickX86(bEnable);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ProcessOfflineFileTrace", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ProcessOfflineFileTraceX86(int target, IntPtr path);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ProcessOfflineFileTrace", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ProcessOfflineFileTraceX64(int target, IntPtr path);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static SNRESULT ProcessOfflineFileTrace(int target, string path)
        {
            ScopedGlobalHeapPtr scopedGlobalHeapPtr = new ScopedGlobalHeapPtr(AllocUtf8FromString(path));
            if (!Is32Bit())
                return ProcessOfflineFileTraceX64(target, scopedGlobalHeapPtr.Get());
            return ProcessOfflineFileTraceX86(target, scopedGlobalHeapPtr.Get());
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3BDTransferImage", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT BDTransferImageX86(int target, IntPtr sourceFileName, string destinationDevice, out uint transactionId);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3BDTransferImage", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT BDTransferImageX64(int target, IntPtr sourceFileName, string destinationDevice, out uint transactionId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="sourceFileName"></param>
        /// <param name="destinationDevice"></param>
        /// <param name="transactionId"></param>
        /// <returns></returns>
        public static SNRESULT BDTransferImage(int target, string sourceFileName, string destinationDevice, out uint transactionId)
        {
            ScopedGlobalHeapPtr scopedGlobalHeapPtr = new ScopedGlobalHeapPtr(AllocUtf8FromString(sourceFileName));
            if (!Is32Bit())
                return BDTransferImageX64(target, scopedGlobalHeapPtr.Get(), destinationDevice, out transactionId);
            return BDTransferImageX86(target, scopedGlobalHeapPtr.Get(), destinationDevice, out transactionId);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3BDInsert", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT BDInsertX86(int target, string deviceName);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3BDInsert", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT BDInsertX64(int target, string deviceName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="deviceName"></param>
        /// <returns></returns>
        public static SNRESULT BDInsert(int target, string deviceName)
        {
            if (!Is32Bit())
                return BDInsertX64(target, deviceName);
            return BDInsertX86(target, deviceName);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3BDEject", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT BDEjectX86(int target, string deviceName);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3BDEject", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT BDEjectX64(int target, string deviceName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="deviceName"></param>
        /// <returns></returns>
        public static SNRESULT BDEject(int target, string deviceName)
        {
            if (!Is32Bit())
                return BDEjectX64(target, deviceName);
            return BDEjectX86(target, deviceName);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3BDFormat", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT BDFormatX86(int target, string deviceName, uint formatMode);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3BDFormat", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT BDFormatX64(int target, string deviceName, uint formatMode);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="deviceName"></param>
        /// <param name="formatMode"></param>
        /// <returns></returns>
        public static SNRESULT BDFormat(int target, string deviceName, uint formatMode)
        {
            if (!Is32Bit())
                return BDFormatX64(target, deviceName, formatMode);
            return BDFormatX86(target, deviceName, formatMode);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3BDQuery", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT BDQueryX86(int target, string deviceName, ref BDInfoPriv infoPriv);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3BDQuery", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT BDQueryX64(int target, string deviceName, ref BDInfoPriv infoPriv);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="deviceName"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public static SNRESULT BDQuery(int target, string deviceName, ref BDInfo info)
        {
            BDInfoPriv infoPriv = new BDInfoPriv();
            SNRESULT res = Is32Bit() ? BDQueryX86(target, deviceName, ref infoPriv) : BDQueryX64(target, deviceName, ref infoPriv);
            if (SUCCEEDED(res))
            {
                info.bdemu_data_size = infoPriv.bdemu_data_size;
                info.bdemu_total_entry = infoPriv.bdemu_total_entry;
                info.bdemu_selected_index = infoPriv.bdemu_selected_index;
                info.image_index = infoPriv.image_index;
                info.image_type = infoPriv.image_type;
                info.image_file_name = Utf8FixedSizeByteArrayToString(infoPriv.image_file_name);
                info.image_file_size = infoPriv.image_file_size;
                info.image_product_code = Utf8FixedSizeByteArrayToString(infoPriv.image_product_code);
                info.image_producer = Utf8FixedSizeByteArrayToString(infoPriv.image_producer);
                info.image_author = Utf8FixedSizeByteArrayToString(infoPriv.image_author);
                info.image_date = Utf8FixedSizeByteArrayToString(infoPriv.image_date);
                info.image_sector_layer0 = infoPriv.image_sector_layer0;
                info.image_sector_layer1 = infoPriv.image_sector_layer1;
                info.image_memorandum = Utf8FixedSizeByteArrayToString(infoPriv.image_memorandum);
            }
            return res;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3RegisterTargetEventHandler", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT RegisterTargetEventHandlerX86(int target, HandleEventCallbackPriv callback, IntPtr userData);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3RegisterTargetEventHandler", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT RegisterTargetEventHandlerX64(int target, HandleEventCallbackPriv callback, IntPtr userData);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="callback"></param>
        /// <param name="userData"></param>
        /// <returns></returns>
        public static SNRESULT RegisterTargetEventHandler(int target, TargetEventCallback callback, ref object userData)
        {
            if (callback == null)
                return SNRESULT.SN_E_BAD_PARAM;
            SNRESULT res = Is32Bit() ? RegisterTargetEventHandlerX86(target, ms_eventHandlerWrapper, IntPtr.Zero) : RegisterTargetEventHandlerX64(target, ms_eventHandlerWrapper, IntPtr.Zero);
            if (SUCCEEDED(res))
            {
                TargetCallbackAndUserData callbackAndUserData = new TargetCallbackAndUserData
                {
                    m_callback = callback,
                    m_userData = userData
                };
                if (ms_userTargetCallbacks == null)
                    ms_userTargetCallbacks = new Dictionary<int, TargetCallbackAndUserData>(1);
                ms_userTargetCallbacks[target] = callbackAndUserData;
            }
            return res;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3CancelTargetEvents", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT CancelTargetEventsX86(int target);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3CancelTargetEvents", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT CancelTargetEventsX64(int target);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static SNRESULT CancelTargetEvents(int target)
        {
            SNRESULT res = Is32Bit() ? CancelTargetEventsX86(target) : CancelTargetEventsX64(target);
            if (SUCCEEDED(res))
            {
                if (ms_userTargetCallbacks == null)
                    return SNRESULT.SN_E_ERROR;
                ms_userTargetCallbacks.Remove(target);
            }
            return res;
        }

        private static void MarshalTargetEvent(int target, uint param, SNRESULT result, uint length, IntPtr data)
        {
            List<TargetEvent> targetEventList = new List<TargetEvent>();
            uint num1 = length;
            while (num1 > 0U)
            {
                TargetEvent targetEvent = new TargetEvent();
                TargetEventHdrPriv storage1 = new TargetEventHdrPriv();
                IntPtr num2 = ReadDataFromUnmanagedIncPtr(data, ref storage1);
                uint size = storage1.Size;
                targetEvent.TargetID = storage1.TargetID;
                targetEvent.Type = (TargetEventType)storage1.EventType;
                targetEvent.Type.GetType();
                IntPtr num3;
                switch (targetEvent.Type)
                {
                    case TargetEventType.UnitStatusChange:
                        targetEvent.EventData = new TargetEventData();
                        TGTEventUnitStatusChangeDataPriv storage2 = new TGTEventUnitStatusChangeDataPriv();
                        num3 = ReadDataFromUnmanagedIncPtr(num2, ref storage2);
                        targetEvent.EventData.UnitStatusChangeData.Unit = (UnitType)storage2.Unit;
                        targetEvent.EventData.UnitStatusChangeData.Status = (UnitStatus)storage2.Status;
                        break;
                    case TargetEventType.Details:
                        targetEvent.EventData = new TargetEventData();
                        num3 = ReadDataFromUnmanagedIncPtr<uint>(num2, ref targetEvent.EventData.DetailsData.Flags);
                        break;
                    case TargetEventType.ModuleLoad:
                    case TargetEventType.ModuleRunning:
                    case TargetEventType.ModuleStopped:
                        targetEvent.EventData = new TargetEventData();
                        num3 = ReadDataFromUnmanagedIncPtr(num2, ref targetEvent.EventData.ModuleEventData);
                        break;
                    case TargetEventType.BDIsotransferStarted:
                    case TargetEventType.BDIsotransferFinished:
                    case TargetEventType.BDFormatStarted:
                    case TargetEventType.BDFormatFinished:
                    case TargetEventType.BDMountStarted:
                    case TargetEventType.BDMountFinished:
                    case TargetEventType.BDUnmountStarted:
                    case TargetEventType.BDUnmountFinished:
                        targetEvent.EventData = new TargetEventData();
                        TGTEventBDDataPriv storage3 = new TGTEventBDDataPriv();
                        num3 = ReadDataFromUnmanagedIncPtr(num2, ref storage3);
                        targetEvent.EventData.BdData.Source = Utf8FixedSizeByteArrayToString(storage3.Source);
                        targetEvent.EventData.BdData.Destination = Utf8FixedSizeByteArrayToString(storage3.Destination);
                        targetEvent.EventData.BdData.Result = storage3.Result;
                        break;
                    case TargetEventType.TargetSpecific:
                        targetEvent.TargetSpecific = MarshalTargetSpecificEvent(size, num2);
                        break;
                }
                targetEventList.Add(targetEvent);
                num1 -= size;
                data = new IntPtr(data.ToInt64() + size);
            }
            if (ms_userTargetCallbacks == null)
                return;
            ms_userTargetCallbacks[target].m_callback(target, result, targetEventList.ToArray(), ms_userTargetCallbacks[target].m_userData);
        }

        private static TargetSpecificEvent MarshalTargetSpecificEvent(uint eventSize, IntPtr data)
        {
            TargetSpecificEvent targetSpecificEvent = new TargetSpecificEvent();
            TargetSpecificData targetSpecificData = new TargetSpecificData();
            uint storage = 0;
            data = ReadDataFromUnmanagedIncPtr(data, ref targetSpecificEvent.CommandID);
            data = ReadDataFromUnmanagedIncPtr(data, ref targetSpecificEvent.RequestID);
            data = ReadDataFromUnmanagedIncPtr(data, ref storage);
            data = ReadDataFromUnmanagedIncPtr(data, ref targetSpecificEvent.ProcessID);
            data = ReadDataFromUnmanagedIncPtr(data, ref targetSpecificEvent.Result);
            int num1 = Marshal.ReadInt32(data, 0);
            data = new IntPtr(data.ToInt64() + Marshal.SizeOf((object)num1));
            targetSpecificData.Type = (TargetSpecificEventType)num1;
            int num2 = 20;
            switch (targetSpecificData.Type)
            {
                case TargetSpecificEventType.CoreDumpComplete:
                    targetSpecificData.CoreDumpComplete = new CoreDumpComplete();
                    targetSpecificData.CoreDumpComplete.Filename = Utf8ToString(data, 1024U);
                    break;
                case TargetSpecificEventType.CoreDumpStart:
                    targetSpecificData.CoreDumpStart = new CoreDumpStart();
                    targetSpecificData.CoreDumpStart.Filename = Utf8ToString(data, 1024U);
                    break;
                case TargetSpecificEventType.Footswitch:
                    targetSpecificData.Footswitch = new FootswitchData();
                    data = ReadDataFromUnmanagedIncPtr(data, ref targetSpecificData.Footswitch.EventSource);
                    data = ReadDataFromUnmanagedIncPtr(data, ref targetSpecificData.Footswitch.EventData1);
                    data = ReadDataFromUnmanagedIncPtr(data, ref targetSpecificData.Footswitch.EventData2);
                    data = ReadDataFromUnmanagedIncPtr(data, ref targetSpecificData.Footswitch.EventData3);
                    data = ReadDataFromUnmanagedIncPtr(data, ref targetSpecificData.Footswitch.Reserved);
                    break;
                case TargetSpecificEventType.InstallPackageProgress:
                    targetSpecificData.InstallPackageProgress = new InstallPackageProgress();
                    data = ReadDataFromUnmanagedIncPtr(data, ref targetSpecificData.InstallPackageProgress.Percent);
                    break;
                case TargetSpecificEventType.InstallPackagePath:
                    targetSpecificData.InstallPackagePath = new InstallPackagePath();
                    targetSpecificData.InstallPackagePath.Path = Utf8ToString(data, 1024U);
                    break;
                case TargetSpecificEventType.PRXLoad:
                    targetSpecificData.PRXLoad = new NotifyPRXLoadData();
                    data = ReadDataFromUnmanagedIncPtr(data, ref targetSpecificData.PRXLoad.PPUThreadID);
                    data = ReadDataFromUnmanagedIncPtr(data, ref targetSpecificData.PRXLoad.PRXID);
                    data = ReadDataFromUnmanagedIncPtr(data, ref targetSpecificData.PRXLoad.Timestamp);
                    break;
                case TargetSpecificEventType.PRXUnload:
                    targetSpecificData.PRXUnload = new NotifyPRXUnloadData();
                    data = ReadDataFromUnmanagedIncPtr(data, ref targetSpecificData.PRXUnload.PPUThreadID);
                    data = ReadDataFromUnmanagedIncPtr(data, ref targetSpecificData.PRXUnload.PRXID);
                    data = ReadDataFromUnmanagedIncPtr(data, ref targetSpecificData.PRXUnload.Timestamp);
                    break;
                case TargetSpecificEventType.ProcessCreate:
                    targetSpecificData.PPUProcessCreate = new PPUProcessCreateData();
                    data = ReadDataFromUnmanagedIncPtr(data, ref targetSpecificData.PPUProcessCreate.ParentProcessID);
                    if (storage - num2 - 4L > 0L)
                    {
                        targetSpecificData.PPUProcessCreate.Filename = Utf8ToString(data, uint.MaxValue);
                        break;
                    }
                    break;
                case TargetSpecificEventType.ProcessExit:
                    targetSpecificData.PPUProcessExit = new PPUProcessExitData();
                    data = ReadDataFromUnmanagedIncPtr(data, ref targetSpecificData.PPUProcessExit.ExitCode);
                    break;
                case TargetSpecificEventType.PPUExcTrap:
                case TargetSpecificEventType.PPUExcPrevInt:
                case TargetSpecificEventType.PPUExcIllInst:
                case TargetSpecificEventType.PPUExcTextHtabMiss:
                case TargetSpecificEventType.PPUExcTextSlbMiss:
                case TargetSpecificEventType.PPUExcDataHtabMiss:
                case TargetSpecificEventType.PPUExcFloat:
                case TargetSpecificEventType.PPUExcDataSlbMiss:
                case TargetSpecificEventType.PPUExcDabrMatch:
                case TargetSpecificEventType.PPUExcStop:
                case TargetSpecificEventType.PPUExcStopInit:
                    targetSpecificData.PPUException = new PPUExceptionData();
                    data = ReadDataFromUnmanagedIncPtr(data, ref targetSpecificData.PPUException.ThreadID);
                    data = ReadDataFromUnmanagedIncPtr(data, ref targetSpecificData.PPUException.HWThreadNumber);
                    data = ReadDataFromUnmanagedIncPtr(data, ref targetSpecificData.PPUException.PC);
                    data = ReadDataFromUnmanagedIncPtr(data, ref targetSpecificData.PPUException.SP);
                    break;
                case TargetSpecificEventType.PPUExcAlignment:
                    targetSpecificData.PPUAlignmentException = new PPUAlignmentExceptionData();
                    data = ReadDataFromUnmanagedIncPtr(data, ref targetSpecificData.PPUAlignmentException.ThreadID);
                    data = ReadDataFromUnmanagedIncPtr(data, ref targetSpecificData.PPUAlignmentException.HWThreadNumber);
                    data = ReadDataFromUnmanagedIncPtr(data, ref targetSpecificData.PPUAlignmentException.DSISR);
                    data = ReadDataFromUnmanagedIncPtr(data, ref targetSpecificData.PPUAlignmentException.DAR);
                    data = ReadDataFromUnmanagedIncPtr(data, ref targetSpecificData.PPUAlignmentException.PC);
                    data = ReadDataFromUnmanagedIncPtr(data, ref targetSpecificData.PPUAlignmentException.SP);
                    break;
                case TargetSpecificEventType.PPUExcDataMAT:
                    targetSpecificData.PPUDataMatException = new PPUDataMatExceptionData();
                    data = ReadDataFromUnmanagedIncPtr(data, ref targetSpecificData.PPUDataMatException.ThreadID);
                    data = ReadDataFromUnmanagedIncPtr(data, ref targetSpecificData.PPUDataMatException.HWThreadNumber);
                    data = ReadDataFromUnmanagedIncPtr(data, ref targetSpecificData.PPUDataMatException.DSISR);
                    data = ReadDataFromUnmanagedIncPtr(data, ref targetSpecificData.PPUDataMatException.DAR);
                    data = ReadDataFromUnmanagedIncPtr(data, ref targetSpecificData.PPUDataMatException.PC);
                    data = ReadDataFromUnmanagedIncPtr(data, ref targetSpecificData.PPUDataMatException.SP);
                    break;
                case TargetSpecificEventType.PPUThreadCreate:
                    targetSpecificData.PPUThreadCreate = new PPUThreadCreateData();
                    data = ReadDataFromUnmanagedIncPtr(data, ref targetSpecificData.PPUThreadCreate.ThreadID);
                    break;
                case TargetSpecificEventType.PPUThreadExit:
                    targetSpecificData.PPUThreadExit = new PPUThreadExitData();
                    data = ReadDataFromUnmanagedIncPtr(data, ref targetSpecificData.PPUThreadExit.ThreadID);
                    break;
                case TargetSpecificEventType.SPUThreadStart:
                    targetSpecificData.SPUThreadStart = new SPUThreadStartData();
                    data = ReadDataFromUnmanagedIncPtr(data, ref targetSpecificData.SPUThreadStart.ThreadGroupID);
                    data = ReadDataFromUnmanagedIncPtr(data, ref targetSpecificData.SPUThreadStart.ThreadID);
                    if (storage - num2 - 8L > 0L)
                    {
                        targetSpecificData.SPUThreadStart.ElfFilename = Utf8ToString(data, uint.MaxValue);
                        break;
                    }
                    break;
                case TargetSpecificEventType.SPUThreadStop:
                case TargetSpecificEventType.SPUThreadStopInit:
                    targetSpecificData.SPUThreadStop = new SPUThreadStopData();
                    data = ReadDataFromUnmanagedIncPtr(data, ref targetSpecificData.SPUThreadStop.ThreadGroupID);
                    data = ReadDataFromUnmanagedIncPtr(data, ref targetSpecificData.SPUThreadStop.ThreadID);
                    data = ReadDataFromUnmanagedIncPtr(data, ref targetSpecificData.SPUThreadStop.PC);
                    int num3 = Marshal.ReadInt32(data, 0);
                    data = new IntPtr(data.ToInt64() + Marshal.SizeOf((object)num3));
                    targetSpecificData.SPUThreadStop.Reason = (SPUThreadStopReason)num3;
                    data = ReadDataFromUnmanagedIncPtr(data, ref targetSpecificData.SPUThreadStop.SP);
                    break;
                case TargetSpecificEventType.SPUThreadGroupDestroy:
                    targetSpecificData.SPUThreadGroupDestroyData = new SPUThreadGroupDestroyData();
                    data = ReadDataFromUnmanagedIncPtr(data, ref targetSpecificData.SPUThreadGroupDestroyData.ThreadGroupID);
                    break;
                case TargetSpecificEventType.SPUThreadStopEx:
                    targetSpecificData.SPUThreadStopEx = new SPUThreadStopExData();
                    data = ReadDataFromUnmanagedIncPtr(data, ref targetSpecificData.SPUThreadStopEx.ThreadGroupID);
                    data = ReadDataFromUnmanagedIncPtr(data, ref targetSpecificData.SPUThreadStopEx.ThreadID);
                    data = ReadDataFromUnmanagedIncPtr(data, ref targetSpecificData.SPUThreadStopEx.PC);
                    int num4 = Marshal.ReadInt32(data, 0);
                    data = new IntPtr(data.ToInt64() + Marshal.SizeOf((object)num4));
                    targetSpecificData.SPUThreadStopEx.Reason = (SPUThreadStopReason)num4;
                    data = ReadDataFromUnmanagedIncPtr(data, ref targetSpecificData.SPUThreadStopEx.SP);
                    data = ReadDataFromUnmanagedIncPtr(data, ref targetSpecificData.SPUThreadStopEx.MFCDSISR);
                    data = ReadDataFromUnmanagedIncPtr(data, ref targetSpecificData.SPUThreadStopEx.MFCDSIPR);
                    data = ReadDataFromUnmanagedIncPtr(data, ref targetSpecificData.SPUThreadStopEx.MFCDAR);
                    break;
            }
            targetSpecificEvent.Data = targetSpecificData;
            return targetSpecificEvent;
        }

        private static void EventHandlerWrapper(int target, EventType type, uint param, SNRESULT result, uint length, IntPtr data, IntPtr userData)
        {
            switch (type)
            {
                case EventType.TTY:
                    MarshalTTYEvent(target, param, result, length, data);
                    break;
                case EventType.Target:
                    MarshalTargetEvent(target, param, result, length, data);
                    break;
                case EventType.FTP:
                    MarshalFTPEvent(target, param, result, length, data);
                    break;
                case EventType.PadCapture:
                    MarshalPadCaptureEvent(target, param, result, length, data);
                    break;
                case EventType.FileTrace:
                    MarshalFileTraceEvent(target, param, result, length, data);
                    break;
                case EventType.PadPlayback:
                    MarshalPadPlaybackEvent(target, param, result, length, data);
                    break;
                case EventType.Server:
                    MarshalServerEvent(target, param, result, length, data);
                    break;
            }
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SearchForTargets", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SearchForTargetsX86(string ipAddressFrom, string ipAddressTo, SearchTargetsCallbackPriv callback, IntPtr userData, int port);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SearchForTargets", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SearchForTargetsX64(string ipAddressFrom, string ipAddressTo, SearchTargetsCallbackPriv callback, IntPtr userData, int port);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ipAddressFrom"></param>
        /// <param name="ipAddressTo"></param>
        /// <param name="callback"></param>
        /// <param name="userData"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public static SNRESULT SearchForTargets(string ipAddressFrom, string ipAddressTo, SearchTargetsCallback callback, object userData, int port)
        {
            SearchForTargetsCallbackHandler targetsCallbackHandler = new SearchForTargetsCallbackHandler(callback, userData);
            SearchTargetsCallbackPriv callback1 = new SearchTargetsCallbackPriv(SearchForTargetsCallbackHandler.SearchForTargetsCallback);
            IntPtr intPtr = GCHandle.ToIntPtr(GCHandle.Alloc(targetsCallbackHandler));
            if (!Is32Bit())
                return SearchForTargetsX64(ipAddressFrom, ipAddressTo, callback1, intPtr, port);
            return SearchForTargetsX86(ipAddressFrom, ipAddressTo, callback1, intPtr, port);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3StopSearchForTargets", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT StopSearchForTargetsX86();

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3StopSearchForTargets", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT StopSearchForTargetsX64();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static SNRESULT StopSearchForTargets()
        {
            if (!Is32Bit())
                return StopSearchForTargetsX64();
            return StopSearchForTargetsX86();
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3IsScanning", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT IsScanningX86();

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3IsScanning", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT IsScanningX64();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static SNRESULT IsScanning()
        {
            if (!Is32Bit())
                return IsScanningX64();
            return IsScanningX86();
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3IsValidResolution", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT IsValidResolutionX86(uint monitorType, uint startupResolution);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3IsValidResolution", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT IsValidResolutionX64(uint monitorType, uint startupResolution);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="monitorType"></param>
        /// <param name="startupResolution"></param>
        /// <returns></returns>
        public static SNRESULT IsValidResolution(uint monitorType, uint startupResolution)
        {
            if (!Is32Bit())
                return IsValidResolutionX64(monitorType, startupResolution);
            return IsValidResolutionX86(monitorType, startupResolution);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SetDisplaySettings", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetDisplaySettingsX86(int target, IntPtr executable, uint monitorType, uint connectorType, uint startupResolution, bool HDCP, bool resetAfter);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SetDisplaySettings", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetDisplaySettingsX64(int target, IntPtr executable, uint monitorType, uint connectorType, uint startupResolution, bool HDCP, bool resetAfter);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="executable"></param>
        /// <param name="monitorType"></param>
        /// <param name="connectorType"></param>
        /// <param name="startupResolution"></param>
        /// <param name="HDCP"></param>
        /// <param name="resetAfter"></param>
        /// <returns></returns>
        public static SNRESULT SetDisplaySettings(int target, string executable, uint monitorType, uint connectorType, uint startupResolution, bool HDCP, bool resetAfter)
        {
            ScopedGlobalHeapPtr scopedGlobalHeapPtr = new ScopedGlobalHeapPtr(AllocUtf8FromString(executable));
            if (!Is32Bit())
                return SetDisplaySettingsX64(target, scopedGlobalHeapPtr.Get(), monitorType, connectorType, startupResolution, HDCP, resetAfter);
            return SetDisplaySettingsX86(target, scopedGlobalHeapPtr.Get(), monitorType, connectorType, startupResolution, HDCP, resetAfter);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3MapFileSystem", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT MapFileSystemX86(char driveLetter);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3MapFileSystem", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT MapFileSystemX64(char driveLetter);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="driveLetter"></param>
        /// <returns></returns>
        public static SNRESULT MapFileSystem(char driveLetter)
        {
            if (!Is32Bit())
                return MapFileSystemX64(driveLetter);
            return MapFileSystemX86(driveLetter);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3UnmapFileSystem", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT UnmapFileSystemX86();

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3UnmapFileSystem", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT UnmapFileSystemX64();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static SNRESULT UnmapFileSystem()
        {
            if (!Is32Bit())
                return UnmapFileSystemX64();
            return UnmapFileSystemX86();
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ImportTargetSettings", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ImportTargetSettingsX86(int target, IntPtr szFileName);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ImportTargetSettings", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ImportTargetSettingsX64(int target, IntPtr szFileName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static SNRESULT ImportTargetSettings(int target, string fileName)
        {
            ScopedGlobalHeapPtr scopedGlobalHeapPtr = new ScopedGlobalHeapPtr(AllocUtf8FromString(fileName));
            if (!Is32Bit())
                return ImportTargetSettingsX64(target, scopedGlobalHeapPtr.Get());
            return ImportTargetSettingsX86(target, scopedGlobalHeapPtr.Get());
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ExportTargetSettings", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ExportTargetSettingsX86(int target, IntPtr szFileName);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ExportTargetSettings", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ExportTargetSettingsX64(int target, IntPtr szFileName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static SNRESULT ExportTargetSettings(int target, string fileName)
        {
            ScopedGlobalHeapPtr scopedGlobalHeapPtr = new ScopedGlobalHeapPtr(AllocUtf8FromString(fileName));
            if (!Is32Bit())
                return ExportTargetSettingsX64(target, scopedGlobalHeapPtr.Get());
            return ExportTargetSettingsX86(target, scopedGlobalHeapPtr.Get());
        }

        private static IntPtr WriteDataToUnmanagedIncPtr<T>(T storage, IntPtr unmanagedBuf)
        {
            bool fDeleteOld = false;
            Marshal.StructureToPtr((object)storage, unmanagedBuf, fDeleteOld);
            return new IntPtr(unmanagedBuf.ToInt64() + Marshal.SizeOf((object)storage));
        }

        private static IntPtr ReadDataFromUnmanagedIncPtr<T>(IntPtr unmanagedBuf, ref T storage)
        {
            storage = (T)Marshal.PtrToStructure(unmanagedBuf, typeof(T));
            return new IntPtr(unmanagedBuf.ToInt64() + Marshal.SizeOf((object)storage));
        }

        private static IntPtr ReadAnsiStringFromUnmanagedIncPtr(IntPtr unmanagedBuf, ref string inputString)
        {
            inputString = Marshal.PtrToStringAnsi(unmanagedBuf);
            return new IntPtr(unmanagedBuf.ToInt64() + inputString.Length + 1L);
        }

        private static IntPtr AllocUtf8FromString(string wcharString)
        {
            if (wcharString == null)
                return IntPtr.Zero;
            byte[] bytes = Encoding.UTF8.GetBytes(wcharString);
            IntPtr destination = Marshal.AllocHGlobal(bytes.Length + 1);
            Marshal.Copy(bytes, 0, destination, bytes.Length);
            Marshal.WriteByte((IntPtr)(destination.ToInt64() + bytes.Length), 0);
            return destination;
        }

        private static string Utf8ToString(IntPtr utf8Ptr, uint maxLength)
        {
            if (utf8Ptr == IntPtr.Zero)
                return "";
            byte numPtr = (byte)utf8Ptr;
            int length;
            for (length = 0; numPtr != 0 && length < maxLength; ++length)
                ++numPtr;
            byte[] numArray = new byte[length];
            Marshal.Copy(utf8Ptr, numArray, 0, length);
            return Encoding.UTF8.GetString(numArray);
        }

        private static string Utf8FixedSizeByteArrayToString(byte[] byteArray)
        {
            if (byteArray == null)
                return "";
            int count = 0;
            byte[] numArray = byteArray;
            for (int index = 0; index < numArray.Length && numArray[index] != 0; ++index)
                ++count;
            byte[] bytes = new byte[count];
            Buffer.BlockCopy(byteArray, 0, bytes, 0, count);
            return Encoding.UTF8.GetString(bytes);
        }

        /// <summary>
        /// 
        /// </summary>
        public enum SNRESULT
        {
            /// <summary>
            /// 
            /// </summary>
            SN_E_ERROR = -2147483648,
            /// <summary>
            /// 
            /// </summary>
            SN_E_COMMS_EVENT_MISMATCHED_ERR = -39,
            /// <summary>
            /// 
            /// </summary>
            SN_E_CONNECTED = -38,
            /// <summary>
            /// 
            /// </summary>
            SN_E_PROTOCOL_ALREADY_REGISTERED = -37,
            /// <summary>
            /// 
            /// </summary>
            SN_E_COMMAND_CANCELLED = -36,
            /// <summary>
            /// 
            /// </summary>
            SN_E_CONNECT_TO_GAMEPORT_FAILED = -35,
            /// <summary>
            /// 
            /// </summary>
            SN_E_MODULE_NOT_FOUND = -34,
            /// <summary>
            /// 
            /// </summary>
            SN_E_CHECK_TARGET_CONFIGURATION = -33,
            /// <summary>
            /// 
            /// </summary>
            SN_E_LICENSE_ERROR = -32,
            /// <summary>
            /// 
            /// </summary>
            SN_E_LOAD_MODULE_FAILED = -31,
            /// <summary>
            /// 
            /// </summary>
            SN_E_NOT_SUPPORTED_IN_SDK_VERSION = -30,
            /// <summary>
            /// 
            /// </summary>
            SN_E_FILE_ERROR = -29,
            /// <summary>
            /// 
            /// </summary>
            SN_E_BAD_ALIGN = -28,
            /// <summary>
            /// 
            /// </summary>
            SN_E_DEPRECATED = -27,
            /// <summary>
            /// 
            /// </summary>
            SN_E_DATA_TOO_LONG = -26,
            /// <summary>
            /// 
            /// </summary>
            SN_E_INSUFFICIENT_DATA = -25,
            /// <summary>
            /// 
            /// </summary>
            SN_E_EXISTING_CALLBACK = -24,
            /// <summary>
            /// 
            /// </summary>
            SN_E_DECI_ERROR = -23,
            /// <summary>
            /// 
            /// </summary>
            SN_E_BUSY = -22,
            /// <summary>
            /// 
            /// </summary>
            SN_E_BAD_PARAM = -21,
            /// <summary>
            /// 
            /// </summary>
            SN_E_NO_SEL = -20,
            /// <summary>
            /// 
            /// </summary>
            SN_E_NO_TARGETS = -19,
            /// <summary>
            /// 
            /// </summary>
            SN_E_BAD_MEMSPACE = -18,
            /// <summary>
            /// 
            /// </summary>
            SN_E_TARGET_RUNNING = -17,
            /// <summary>
            /// 
            /// </summary>
            SN_E_DLL_NOT_INITIALISED = -15,
            /// <summary>
            /// 
            /// </summary>
            SN_E_TM_VERSION = -14,
            /// <summary>
            /// 
            /// </summary>
            SN_E_NOT_LISTED = -13,
            /// <summary>
            /// 
            /// </summary>
            SN_E_OUT_OF_MEM = -12,
            /// <summary>
            /// 
            /// </summary>
            SN_E_BAD_UNIT = -11,
            /// <summary>
            /// 
            /// </summary>
            SN_E_LOAD_ELF_FAILED = -10,
            /// <summary>
            /// 
            /// </summary>
            SN_E_TARGET_IN_USE = -9,
            /// <summary>
            /// 
            /// </summary>
            SN_E_HOST_NOT_FOUND = -8,
            /// <summary>
            /// 
            /// </summary>
            SN_E_TIMEOUT = -7,
            /// <summary>
            /// 
            /// </summary>
            SN_E_TM_COMMS_ERR = -6,
            /// <summary>
            /// 
            /// </summary>
            SN_E_COMMS_ERR = -5,
            /// <summary>
            /// 
            /// </summary>
            SN_E_NOT_CONNECTED = -4,
            /// <summary>
            /// 
            /// </summary>
            SN_E_BAD_TARGET = -3,
            /// <summary>
            /// 
            /// </summary>
            SN_E_TM_NOT_RUNNING = -2,
            /// <summary>
            /// 
            /// </summary>
            SN_E_NOT_IMPL = -1,
            /// <summary>
            /// 
            /// </summary>
            SN_S_OK = 0,
            /// <summary>
            /// 
            /// </summary>
            SN_S_PENDING = 1,
            /// <summary>
            /// 
            /// </summary>
            SN_S_NO_MSG = 3,
            /// <summary>
            /// 
            /// </summary>
            SN_S_TM_VERSION = 4,
            /// <summary>
            /// 
            /// </summary>
            SN_S_REPLACED = 5,
            /// <summary>
            /// 
            /// </summary>
            SN_S_NO_ACTION = 6,
            /// <summary>
            /// 
            /// </summary>
            SN_S_TARGET_STILL_REGISTERED = 7,
        }

        /// <summary>
        /// 
        /// </summary>
        public enum ConnectStatus
        {
            /// <summary>
            /// 
            /// </summary>
            Connected,
            /// <summary>
            /// 
            /// </summary>
            Connecting,
            /// <summary>
            /// 
            /// </summary>
            NotConnected,
            /// <summary>
            /// 
            /// </summary>
            InUse,
            /// <summary>
            /// 
            /// </summary>
            Unavailable,
        }

        /// <summary>
        /// 
        /// </summary>
        public delegate int EnumerateTargetsCallback(int target);

        /// <summary>
        /// 
        /// </summary>
        public delegate int EnumerateTargetsExCallback(int target, object userData);

        /// <summary>
        /// 
        /// </summary>
        private delegate int EnumerateTargetsExCallbackPriv(int target, IntPtr unused);

        /// <summary>
        /// 
        /// </summary>
        [Flags]
        public enum BootParameter : ulong
        {
            /// <summary>
            /// 
            /// </summary>
            Default = 0,
            /// <summary>
            /// 
            /// </summary>
            SystemMode = 17,
            /// <summary>
            /// 
            /// </summary>
            ReleaseMode = 1,
            /// <summary>
            /// 
            /// </summary>
            DebugMode = 16,
            /// <summary>
            /// 
            /// </summary>
            MemSizeConsole = 2,
            /// <summary>
            /// 
            /// </summary>
            BluRayEmuOff = 4,
            /// <summary>
            /// 
            /// </summary>
            HDDSpeedBluRayEmu = 8,
            /// <summary>
            /// 
            /// </summary>
            BluRayEmuUSB = 32,
            /// <summary>
            /// 
            /// </summary>
            HostFSTarget = 64,
            /// <summary>
            /// 
            /// </summary>
            DualNIC = 128,
        }

        /// <summary>
        /// 
        /// </summary>
        [Flags]
        public enum BootParameterMask : ulong
        {
            /// <summary>
            /// 
            /// </summary>
            BootMode = 17,
            /// <summary>
            /// 
            /// </summary>
            Memsize = 2,
            /// <summary>
            /// 
            /// </summary>
            BlurayEmulation = 4,
            /// <summary>
            /// 
            /// </summary>
            HDDSpeed = 8,
            /// <summary>
            /// 
            /// </summary>
            BlurayEmuSelect = 32,
            /// <summary>
            /// 
            /// </summary>
            HostFS = 64,
            /// <summary>
            /// 
            /// </summary>
            NIC = 128,
            /// <summary>
            /// 
            /// </summary>
            All = NIC | HostFS | BlurayEmuSelect | HDDSpeed | BlurayEmulation | Memsize | BootMode,
        }

        /// <summary>
        /// 
        /// </summary>
        [Flags]
        public enum ResetParameter : ulong
        {
            /// <summary>
            /// 
            /// </summary>
            Soft = 0,
            /// <summary>
            /// 
            /// </summary>
            Hard = 1,
            /// <summary>
            /// 
            /// </summary>
            Quick = 2,
            /// <summary>
            /// 
            /// </summary>
            ResetEx = 9223372036854775808,
        }

        /// <summary>
        /// 
        /// </summary>
        [Flags]
        public enum ResetParameterMask : ulong
        {
            /// <summary>
            /// 
            /// </summary>
            All = 9223372036854775811,
        }

        /// <summary>
        /// 
        /// </summary>
        [Flags]
        public enum SystemParameter : ulong
        {
            /// <summary>
            /// 
            /// </summary>
            TargetModel60GB = 281474976710656,
            /// <summary>
            /// 
            /// </summary>
            TargetModel20GB = 562949953421312,
            /// <summary>
            /// 
            /// </summary>
            ReleaseCheckMode = 140737488355328,
        }

        /// <summary>
        /// 
        /// </summary>
        [Flags]
        public enum SystemParameterMask : ulong
        {
            /// <summary>
            /// 
            /// </summary>
            TargetModel = 71776119061217280,
            /// <summary>
            /// 
            /// </summary>
            ReleaseCheck = 140737488355328,
            /// <summary>
            /// 
            /// </summary>
            All = ReleaseCheck | TargetModel,
        }

        /// <summary>
        /// 
        /// </summary>
        [Flags]
        public enum TargetInfoFlag : uint
        {
            /// <summary>
            /// 
            /// </summary>
            TargetID = 1,
            /// <summary>
            /// 
            /// </summary>
            Name = 2,
            /// <summary>
            /// 
            /// </summary>
            Info = 4,
            /// <summary>
            /// 
            /// </summary>
            HomeDir = 8,
            /// <summary>
            /// 
            /// </summary>
            FileServingDir = 16,
            /// <summary>
            /// 
            /// </summary>
            Boot = 32,
        }

        /// <summary>
        /// 
        /// </summary>
        public struct TargetInfo
        {
            /// <summary>
            /// 
            /// </summary>
            public TargetInfoFlag Flags;
            /// <summary>
            /// 
            /// </summary>
            public int Target;
            /// <summary>
            /// 
            /// </summary>
            public string Name;
            /// <summary>
            /// 
            /// </summary>
            public string Type;
            /// <summary>
            /// 
            /// </summary>
            public string Info;
            /// <summary>
            /// 
            /// </summary>
            public string HomeDir;
            /// <summary>
            /// 
            /// </summary>
            public string FSDir;
            /// <summary>
            /// 
            /// </summary>
            public BootParameter Boot;
        }

        private struct TargetInfoPriv
        {
            public TargetInfoFlag Flags;
            public int Target;
            public IntPtr Name;
            public IntPtr Type;
            public IntPtr Info;
            public IntPtr HomeDir;
            public IntPtr FSDir;
            public BootParameter Boot;
        }

        /// <summary>
        /// 
        /// </summary>
        public struct TargetType
        {
            /// <summary>
            /// 
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string Type;
            /// <summary>
            /// 
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string Description;
        }

        /// <summary>
        /// 
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public class TCPIPConnectProperties
        {
            /// <summary>
            /// 
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 255)]
            public string IPAddress;
            /// <summary>
            /// 
            /// </summary>
            public uint Port;
        }

        /// <summary>
        /// 
        /// </summary>
        public enum ServerEventType : uint
        {
            /// <summary>
            /// 
            /// </summary>
            TargetAdded,
            /// <summary>
            /// 
            /// </summary>
            TargetDeleted,
            /// <summary>
            /// 
            /// </summary>
            DefaultTargetChanged,
        }

        /// <summary>
        /// 
        /// </summary>
        public delegate void ServerEventCallback(int target, SNRESULT res, ServerEventType eventType, object userData);

        private struct ServerEventHeader
        {
            public uint size;
            public uint targetID;
            public ServerEventType eventType;
        }

        /// <summary>
        /// 
        /// </summary>
        [Flags]
        public enum SystemInfoFlag : uint
        {
            /// <summary>
            /// 
            /// </summary>
            SDKVersion = 1,
            /// <summary>
            /// 
            /// </summary>
            TimebaseFreq = 2,
            /// <summary>
            /// 
            /// </summary>
            RTSDKVersion = 4,
            /// <summary>
            /// 
            /// </summary>
            TotalSystemMem = 8,
            /// <summary>
            /// 
            /// </summary>
            AvailableSysMem = 16,
            /// <summary>
            /// 
            /// </summary>
            DCMBufferSize = 32,
        }

        /// <summary>
        /// 
        /// </summary>
        public struct SystemInfo
        {
            /// <summary>
            /// 
            /// </summary>
            public uint CellSDKVersion;
            /// <summary>
            /// 
            /// </summary>
            public ulong TimebaseFrequency;
            /// <summary>
            /// 
            /// </summary>
            public uint CellRuntimeSDKVersion;
            /// <summary>
            /// 
            /// </summary>
            public uint TotalSystemMemory;
            /// <summary>
            /// 
            /// </summary>
            public uint AvailableSystemMemory;
            /// <summary>
            /// 
            /// </summary>
            public uint DCMBufferSize;
        }

        /// <summary>
        /// 
        /// </summary>
        [Flags]
        public enum ExtraLoadFlag : ulong
        {
            /// <summary>
            /// 
            /// </summary>
            EnableLv2ExceptionHandler = 1,
            /// <summary>
            /// 
            /// </summary>
            EnableRemotePlay = 2,
            /// <summary>
            /// 
            /// </summary>
            EnableGCMDebug = 4,
            /// <summary>
            /// 
            /// </summary>
            LoadLibprofSPRXAutomatically = 8,
            /// <summary>
            /// 
            /// </summary>
            EnableCoreDump = 16,
            /// <summary>
            /// 
            /// </summary>
            EnableAccForRemotePlay = 32,
            /// <summary>
            /// 
            /// </summary>
            EnableHUDRSXTools = 64,
            /// <summary>
            /// 
            /// </summary>
            EnableMAT = 128,
            /// <summary>
            /// 
            /// </summary>
            EnableMiscSettings = 9223372036854775808,
            /// <summary>
            /// 
            /// </summary>
            GameAttributeInviteMessage = 256,
            /// <summary>
            /// 
            /// </summary>
            GameAttributeCustomMessage = 512,
            /// <summary>
            /// 
            /// </summary>
            LoadingPatch = 4096,
        }

        /// <summary>
        /// 
        /// </summary>
        [Flags]
        public enum ExtraLoadFlagMask : ulong
        {
            /// <summary>
            /// 
            /// </summary>
            GameAttributeMessageMask = 3840,
            /// <summary>
            /// 
            /// </summary>
            All = 9223372036854783999,
            /// <summary>
            /// 
            /// </summary>
            OverrideTVGUIMask = 9223372036854775808,
        }

        /// <summary>
        /// 
        /// </summary>
        public struct TTYStream
        {
            /// <summary>
            /// 
            /// </summary>
            public uint Index;
            /// <summary>
            /// 
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string Name;
        }

        private enum SNPS3_TM_TIMEOUT
        {
            DEFAULT_TIMEOUT,
            RESET_TIMEOUT,
            CONNECT_TIMEOUT,
            LOAD_TIMEOUT,
            GET_STATUS_TIMEOUT,
            RECONNECT_TIMEOUT,
            GAMEPORT_TIMEOUT,
            GAMEEXIT_TIMEOUT,
        }

        /// <summary>
        /// 
        /// </summary>
        public enum TimeoutType
        {
            /// <summary>
            /// 
            /// </summary>
            Default,
            /// <summary>
            /// 
            /// </summary>
            Reset,
            /// <summary>
            /// 
            /// </summary>
            Connect,
            /// <summary>
            /// 
            /// </summary>
            Load,
            /// <summary>
            /// 
            /// </summary>
            GetStatus,
            /// <summary>
            /// 
            /// </summary>
            Reconnect,
            /// <summary>
            /// 
            /// </summary>
            GamePort,
            /// <summary>
            /// 
            /// </summary>
            GameExit,
        }

        /// <summary>
        /// 
        /// </summary>
        public delegate void TTYCallback(int target, uint streamID, SNRESULT res, string data, object userData);

        /// <summary>
        /// 
        /// </summary>
        public delegate void TTYCallbackRaw(int target, uint streamID, SNRESULT res, byte[] data, object userData);

        private class TTYCallbackAndUserData
        {
            public TTYCallback m_callback;
            public object m_userData;
            public TTYCallbackRaw m_callbackRaw;
            public object m_userDataRaw;
        }

        private struct TTYChannel
        {
            public readonly int Target;
            public readonly uint Channel;

            public TTYChannel(int target, uint channel)
            {
                Target = target;
                Channel = channel;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public enum UnitType
        {
            /// <summary>
            /// 
            /// </summary>
            PPU,
            /// <summary>
            /// 
            /// </summary>
            SPU,
            /// <summary>
            /// 
            /// </summary>
            SPURAW,
        }

        /// <summary>
        /// 
        /// </summary>
        public enum UnitStatus : uint
        {
            /// <summary>
            /// 
            /// </summary>
            Unknown,
            /// <summary>
            /// 
            /// </summary>
            Running,
            /// <summary>
            /// 
            /// </summary>
            Stopped,
            /// <summary>
            /// 
            /// </summary>
            Signalled,
            /// <summary>
            /// 
            /// </summary>
            Resetting,
            /// <summary>
            /// 
            /// </summary>
            Missing,
            /// <summary>
            /// 
            /// </summary>
            Reset,
            /// <summary>
            /// 
            /// </summary>
            NotConnected,
            /// <summary>
            /// 
            /// </summary>
            Connected,
            /// <summary>
            /// 
            /// </summary>
            StatusChange,
        }

        /// <summary>
        /// 
        /// </summary>
        [Flags]
        public enum LoadFlag : uint
        {
            /// <summary>
            /// 
            /// </summary>
            EnableDebugging = 1,
            /// <summary>
            /// 
            /// </summary>
            UseELFPriority = 256,
            /// <summary>
            /// 
            /// </summary>
            UseELFStackSize = 512,
            /// <summary>
            /// 
            /// </summary>
            WaitBDMounted = 8192,
            /// <summary>
            /// 
            /// </summary>
            PPUNotDebug = 65536,
            /// <summary>
            /// 
            /// </summary>
            SPUNotDebug = 131072,
            /// <summary>
            /// 
            /// </summary>
            IgnoreDefaults = 2147483648,
            /// <summary>
            /// 
            /// </summary>
            ParamSFOUseELFDir = 1048576,
            /// <summary>
            /// 
            /// </summary>
            ParamSFOUseCustomDir = 2097152,
        }

        /// <summary>
        /// 
        /// </summary>
        public enum ProcessStatus : uint
        {
            /// <summary>
            /// 
            /// </summary>
            Creating = 1,
            /// <summary>
            /// 
            /// </summary>
            Ready = 2,
            /// <summary>
            /// 
            /// </summary>
            Exited = 3,
        }

        /// <summary>
        /// 
        /// </summary>
        public struct ProcessInfoHdr
        {
            /// <summary>
            /// 
            /// </summary>
            public ProcessStatus Status;
            /// <summary>
            /// 
            /// </summary>
            public uint NumPPUThreads;
            /// <summary>
            /// 
            /// </summary>
            public uint NumSPUThreads;
            /// <summary>
            /// 
            /// </summary>
            public uint ParentProcessID;
            /// <summary>
            /// 
            /// </summary>
            public ulong MaxMemorySize;
            /// <summary>
            /// 
            /// </summary>
            public string ELFPath;
        }

        /// <summary>
        /// 
        /// </summary>
        public struct ProcessInfo
        {
            /// <summary>
            /// 
            /// </summary>
            public ProcessInfoHdr Hdr;
            /// <summary>
            /// 
            /// </summary>
            public ulong[] ThreadIDs;
        }

        /// <summary>
        /// 
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ExtraProcessInfo
        {
            /// <summary>
            /// 
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
            public uint[] PPUGUIDs;
        }

        /// <summary>
        /// 
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ProcessLoadParams
        {
            /// <summary>
            /// 
            /// </summary>
            public ulong Version;
            /// <summary>
            /// 
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public ulong[] Data;
        }

        /// <summary>
        /// 
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ProcessLoadInfo
        {
            /// <summary>
            /// 
            /// </summary>
            public uint InfoValid;
            /// <summary>
            /// 
            /// </summary>
            public uint DebugFlags;
            /// <summary>
            /// 
            /// </summary>
            public ProcessLoadParams LoadInfo;
        }

        /// <summary>
        /// 
        /// </summary>
        public struct ModuleInfoHdr
        {
            /// <summary>
            /// 
            /// </summary>
            public string Name;
            /// <summary>
            /// 
            /// </summary>
            public sbyte[] Version;
            /// <summary>
            /// 
            /// </summary>
            public uint Attribute;
            /// <summary>
            /// 
            /// </summary>
            public uint StartEntry;
            /// <summary>
            /// 
            /// </summary>
            public uint StopEntry;
            /// <summary>
            /// 
            /// </summary>
            public string ELFName;
            /// <summary>
            /// 
            /// </summary>
            public uint NumSegments;
        }

        private struct ModuleInfoHdrPriv
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 30)]
            public byte[] Name;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public sbyte[] Version;
            public uint Attribute;
            public uint StartEntry;
            public uint StopEntry;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 512)]
            public byte[] ELFName;
            public uint NumSegments;
        }

        /// <summary>
        /// 
        /// </summary>
        public struct PRXSegment
        {
            /// <summary>
            /// 
            /// </summary>
            public ulong Base;
            /// <summary>
            /// 
            /// </summary>
            public ulong FileSize;
            /// <summary>
            /// 
            /// </summary>
            public ulong MemSize;
            /// <summary>
            /// 
            /// </summary>
            public ulong Index;
            /// <summary>
            /// 
            /// </summary>
            public ulong ELFType;
        }

        /// <summary>
        /// 
        /// </summary>
        public struct ModuleInfo
        {
            /// <summary>
            /// 
            /// </summary>
            public ModuleInfoHdr Hdr;
            /// <summary>
            /// 
            /// </summary>
            public PRXSegment[] Segments;
        }

        /// <summary>
        /// 
        /// </summary>
        public struct PRXSegmentEx
        {
            /// <summary>
            /// 
            /// </summary>
            public ulong Base;
            /// <summary>
            /// 
            /// </summary>
            public ulong FileSize;
            /// <summary>
            /// 
            /// </summary>
            public ulong MemSize;
            /// <summary>
            /// 
            /// </summary>
            public ulong Index;
            /// <summary>
            /// 
            /// </summary>
            public ulong ELFType;
            /// <summary>
            /// 
            /// </summary>
            public ulong Flags;
            /// <summary>
            /// 
            /// </summary>
            public ulong Align;
        }

        /// <summary>
        /// 
        /// </summary>
        public struct ModuleInfoEx
        {
            /// <summary>
            /// 
            /// </summary>
            public ModuleInfoHdr Hdr;
            /// <summary>
            /// 
            /// </summary>
            public PRXSegmentEx[] Segments;
        }

        /// <summary>
        /// 
        /// </summary>
        public struct MSELFInfo
        {
            /// <summary>
            /// 
            /// </summary>
            public ulong MSELFFileOffset;
            /// <summary>
            /// 
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] Reserved;
        }

        /// <summary>
        /// 
        /// </summary>
        public struct ExtraModuleInfo
        {
            /// <summary>
            /// 
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
            public uint[] PPUGUIDs;
        }

        /// <summary>
        /// 
        /// </summary>
        public enum PPUThreadState
        {
            /// <summary>
            /// 
            /// </summary>
            Idle,
            /// <summary>
            /// 
            /// </summary>
            Runnable,
            /// <summary>
            /// 
            /// </summary>
            OnProc,
            /// <summary>
            /// 
            /// </summary>
            Sleep,
            /// <summary>
            /// 
            /// </summary>
            Suspended,
            /// <summary>
            /// 
            /// </summary>
            SleepSuspended,
            /// <summary>
            /// 
            /// </summary>
            Stop,
            /// <summary>
            /// 
            /// </summary>
            Zombie,
            /// <summary>
            /// 
            /// </summary>
            Deleted,
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct PPUThreadInfoPriv
        {
            public ulong ThreadID;
            public uint Priority;
            public uint State;
            public ulong StackAddress;
            public ulong StackSize;
            public uint ThreadNameLen;
        }

        /// <summary>
        /// 
        /// </summary>
        public struct PPUThreadInfo
        {
            /// <summary>
            /// 
            /// </summary>
            public ulong ThreadID;
            /// <summary>
            /// 
            /// </summary>
            public uint Priority;
            /// <summary>
            /// 
            /// </summary>
            public PPUThreadState State;
            /// <summary>
            /// 
            /// </summary>
            public ulong StackAddress;
            /// <summary>
            /// 
            /// </summary>
            public ulong StackSize;
            /// <summary>
            /// 
            /// </summary>
            public string ThreadName;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct PPUThreadInfoExPriv
        {
            public ulong ThreadId;
            public uint Priority;
            public uint BasePriority;
            public uint State;
            public ulong StackAddress;
            public ulong StackSize;
            public uint ThreadNameLen;
        }

        /// <summary>
        /// 
        /// </summary>
        public struct PPUThreadInfoEx
        {
            /// <summary>
            /// 
            /// </summary>
            public ulong ThreadID;
            /// <summary>
            /// 
            /// </summary>
            public uint Priority;
            /// <summary>
            /// 
            /// </summary>
            public uint BasePriority;
            /// <summary>
            /// 
            /// </summary>
            public PPUThreadState State;
            /// <summary>
            /// 
            /// </summary>
            public ulong StackAddress;
            /// <summary>
            /// 
            /// </summary>
            public ulong StackSize;
            /// <summary>
            /// 
            /// </summary>
            public string ThreadName;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct SpuThreadInfoPriv
        {
            public uint ThreadGroupId;
            public uint ThreadId;
            public uint FilenameLen;
            public uint ThreadNameLen;
        }

        /// <summary>
        /// 
        /// </summary>
        public struct SPUThreadInfo
        {
            /// <summary>
            /// 
            /// </summary>
            public uint ThreadGroupID;
            /// <summary>
            /// 
            /// </summary>
            public uint ThreadID;
            /// <summary>
            /// 
            /// </summary>
            public string Filename;
            /// <summary>
            /// 
            /// </summary>
            public string ThreadName;
        }

        /// <summary>
        /// 
        /// </summary>
        [Flags]
        public enum ELFStackSize : uint
        {
            /// <summary>
            /// 
            /// </summary>
            Stack32k = 32,
            /// <summary>
            /// 
            /// </summary>
            Stack64k = 64,
            /// <summary>
            /// 
            /// </summary>
            Stack96k = Stack64k | Stack32k,
            /// <summary>
            /// 
            /// </summary>
            Stack128k = 128,
            /// <summary>
            /// 
            /// </summary>
            Stack256k = 256,
            /// <summary>
            /// 
            /// </summary>
            Stack512k = 512,
            /// <summary>
            /// 
            /// </summary>
            Stack1024k = 1024,
            /// <summary>
            /// 
            /// </summary>
            StackDefault = Stack64k,
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct DebugThreadControlInfoPriv
        {
            public ulong ControlFlags;
            public uint NumEntries;
        }

        /// <summary>
        /// 
        /// </summary>
        public struct ControlKeywordEntry
        {
            /// <summary>
            /// 
            /// </summary>
            public uint MatchConditionFlags;
            /// <summary>
            /// 
            /// </summary>
            public string Keyword;
        }

        /// <summary>
        /// 
        /// </summary>
        public struct DebugThreadControlInfo
        {
            /// <summary>
            /// 
            /// </summary>
            public ulong ControlFlags;
            /// <summary>
            /// 
            /// </summary>
            public ControlKeywordEntry[] ControlKeywords;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct ProcessTreeBranchPriv
        {
            public uint ProcessId;
            public ProcessStatus ProcessState;
            public uint NumPpuThreads;
            public uint NumSpuThreadGroups;
            public ushort ProcessFlags;
            public ushort RawSPU;
            public IntPtr PpuThreadStatuses;
            public IntPtr SpuThreadGroupStatuses;
        }

        /// <summary>
        /// 
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct PPUThreadStatus
        {
            /// <summary>
            /// 
            /// </summary>
            public ulong ThreadID;
            /// <summary>
            /// 
            /// </summary>
            public PPUThreadState ThreadState;
        }

        /// <summary>
        /// 
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct SPUThreadGroupStatus
        {
            /// <summary>
            /// 
            /// </summary>
            public uint ThreadGroupID;
            /// <summary>
            /// 
            /// </summary>
            public SPUThreadGroupState ThreadGroupState;
        }

        /// <summary>
        /// 
        /// </summary>
        public struct ProcessTreeBranch
        {
            /// <summary>
            /// 
            /// </summary>
            public uint ProcessID;
            /// <summary>
            /// 
            /// </summary>
            public ProcessStatus ProcessState;
            /// <summary>
            /// 
            /// </summary>
            public ushort ProcessFlags;
            /// <summary>
            /// 
            /// </summary>
            public ushort RawSPU;
            /// <summary>
            /// 
            /// </summary>
            public PPUThreadStatus[] PPUThreadStatuses;
            /// <summary>
            /// 
            /// </summary>
            public SPUThreadGroupStatus[] SPUThreadGroupStatuses;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct SpuThreadGroupInfoPriv
        {
            public uint ThreadGroupId;
            public uint State;
            public uint Priority;
            public uint NumThreads;
            public uint ThreadGroupNameLen;
        }

        /// <summary>
        /// 
        /// </summary>
        public enum SPUThreadGroupState : uint
        {
            /// <summary>
            /// 
            /// </summary>
            NotConfigured,
            /// <summary>
            /// 
            /// </summary>
            Configured,
            /// <summary>
            /// 
            /// </summary>
            Ready,
            /// <summary>
            /// 
            /// </summary>
            Waiting,
            /// <summary>
            /// 
            /// </summary>
            Suspended,
            /// <summary>
            /// 
            /// </summary>
            WaitingSuspended,
            /// <summary>
            /// 
            /// </summary>
            Running,
            /// <summary>
            /// 
            /// </summary>
            Stopped,
        }

        /// <summary>
        /// 
        /// </summary>
        public struct SPUThreadGroupInfo
        {
            /// <summary>
            /// 
            /// </summary>
            public uint ThreadGroupID;
            /// <summary>
            /// 
            /// </summary>
            public SPUThreadGroupState State;
            /// <summary>
            /// 
            /// </summary>
            public uint Priority;
            /// <summary>
            /// 
            /// </summary>
            public string GroupName;
            /// <summary>
            /// 
            /// </summary>
            public uint[] ThreadIDs;
        }

        /// <summary>
        /// 
        /// </summary>
        public enum MemoryCompressionLevel : uint
        {
            /// <summary>
            /// 
            /// </summary>
            None = 0,
            /// <summary>
            /// 
            /// </summary>
            BestSpeed = 1,
            /// <summary>
            /// 
            /// </summary>
            BestCompression = 9,
            /// <summary>
            /// 
            /// </summary>
            Default = 4294967295,
        }

        /// <summary>
        /// 
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct VirtualMemoryArea
        {
            /// <summary>
            /// 
            /// </summary>
            public ulong Address;
            /// <summary>
            /// 
            /// </summary>
            public ulong Flags;
            /// <summary>
            /// 
            /// </summary>
            public ulong VSize;
            /// <summary>
            /// 
            /// </summary>
            public ulong Options;
            /// <summary>
            /// 
            /// </summary>
            public ulong PageFaultPPU;
            /// <summary>
            /// 
            /// </summary>
            public ulong PageFaultSPU;
            /// <summary>
            /// 
            /// </summary>
            public ulong PageIn;
            /// <summary>
            /// 
            /// </summary>
            public ulong PageOut;
            /// <summary>
            /// 
            /// </summary>
            public ulong PMemTotal;
            /// <summary>
            /// 
            /// </summary>
            public ulong PMemUsed;
            /// <summary>
            /// 
            /// </summary>
            public ulong Time;
            /// <summary>
            /// 
            /// </summary>
            public ulong[] Pages;
        }

        /// <summary>
        /// 
        /// </summary>
        public struct SyncPrimitiveCounts
        {
            /// <summary>
            /// 
            /// </summary>
            public uint NumMutexes;
            /// <summary>
            /// 
            /// </summary>
            public uint NumConditionVariables;
            /// <summary>
            /// 
            /// </summary>
            public uint NumRWLocks;
            /// <summary>
            /// 
            /// </summary>
            public uint NumLWMutexes;
            /// <summary>
            /// 
            /// </summary>
            public uint NumEventQueues;
            /// <summary>
            /// 
            /// </summary>
            public uint NumSemaphores;
            /// <summary>
            /// 
            /// </summary>
            public uint NumLWConditionVariables;
            /// <summary>
            /// 
            /// </summary>
            public uint NumEventFlag;
        }

        /// <summary>
        /// 
        /// </summary>
        public struct MutexAttr
        {
            /// <summary>
            /// 
            /// </summary>
            public uint Protocol;
            /// <summary>
            /// 
            /// </summary>
            public uint Recursive;
            /// <summary>
            /// 
            /// </summary>
            public uint PShared;
            /// <summary>
            /// 
            /// </summary>
            public uint Adaptive;
            /// <summary>
            /// 
            /// </summary>
            public ulong Key;
            /// <summary>
            /// 
            /// </summary>
            public uint Flags;
            /// <summary>
            /// 
            /// </summary>
            public string Name;
        }

        /// <summary>
        /// 
        /// </summary>
        public struct MutexInfo
        {
            /// <summary>
            /// 
            /// </summary>
            public uint ID;
            /// <summary>
            /// 
            /// </summary>
            public MutexAttr Attribute;
            /// <summary>
            /// 
            /// </summary>
            public ulong OwnerThreadID;
            /// <summary>
            /// 
            /// </summary>
            public uint LockCounter;
            /// <summary>
            /// 
            /// </summary>
            public uint ConditionRefCounter;
            /// <summary>
            /// 
            /// </summary>
            public uint ConditionVarID;
            /// <summary>
            /// 
            /// </summary>
            public uint NumWaitAllThreads;
            /// <summary>
            /// 
            /// </summary>
            public ulong[] WaitingThreads;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct LwMutexInfoPriv
        {
            public uint Id;
            public LWMutexAttr Attribute;
            public ulong OwnerThreadId;
            public uint LockCounter;
            public uint NumWaitingThreads;
            public uint NumWaitAllThreads;
        }

        /// <summary>
        /// 
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct LWMutexAttr
        {
            /// <summary>
            /// 
            /// </summary>
            public uint Protocol;
            /// <summary>
            /// 
            /// </summary>
            public uint Recursive;
            /// <summary>
            /// 
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
            public string Name;
        }

        /// <summary>
        /// 
        /// </summary>
        public struct LWMutexInfo
        {
            /// <summary>
            /// 
            /// </summary>
            public uint ID;
            /// <summary>
            /// 
            /// </summary>
            public LWMutexAttr Attribute;
            /// <summary>
            /// 
            /// </summary>
            public ulong OwnerThreadID;
            /// <summary>
            /// 
            /// </summary>
            public uint LockCounter;
            /// <summary>
            /// 
            /// </summary>
            public uint NumWaitAllThreads;
            /// <summary>
            /// 
            /// </summary>
            public ulong[] WaitingThreads;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct ConditionVarInfoPriv
        {
            public uint Id;
            public ConditionVarAttr Attribute;
            public uint MutexId;
            public uint NumWaitingThreads;
            public uint NumWaitAllThreads;
        }

        /// <summary>
        /// 
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ConditionVarAttr
        {
            /// <summary>
            /// 
            /// </summary>
            public uint PShared;
            /// <summary>
            /// 
            /// </summary>
            public ulong Key;
            /// <summary>
            /// 
            /// </summary>
            public uint Flags;
            /// <summary>
            /// 
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
            public string Name;
        }

        /// <summary>
        /// 
        /// </summary>
        public struct ConditionVarInfo
        {
            /// <summary>
            /// 
            /// </summary>
            public uint ID;
            /// <summary>
            /// 
            /// </summary>
            public ConditionVarAttr Attribute;
            /// <summary>
            /// 
            /// </summary>
            public uint MutexID;
            /// <summary>
            /// 
            /// </summary>
            public uint NumWaitAllThreads;
            /// <summary>
            /// 
            /// </summary>
            public ulong[] WaitingThreads;
        }

        private struct LwConditionVarInfoPriv
        {
            public uint Id;
            public LWConditionVarAttr Attribute;
            public uint LwMutexId;
            public uint NumWaitingThreads;
            public uint NumWaitAllThreads;
        }

        /// <summary>
        /// 
        /// </summary>
        public struct LWConditionVarAttr
        {
            /// <summary>
            /// 
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
            public string Name;
        }

        /// <summary>
        /// 
        /// </summary>
        public struct LWConditionVarInfo
        {
            /// <summary>
            /// 
            /// </summary>
            public uint ID;
            /// <summary>
            /// 
            /// </summary>
            public LWConditionVarAttr Attribute;
            /// <summary>
            /// 
            /// </summary>
            public uint LWMutexID;
            /// <summary>
            /// 
            /// </summary>
            public uint NumWaitAllThreads;
            /// <summary>
            /// 
            /// </summary>
            public ulong[] WaitingThreads;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct RwLockInfoPriv
        {
            public uint Id;
            public RWLockAttr Attribute;
            public ulong OwnerThreadId;
            public uint NumWaitingReadThreads;
            public uint NumWaitAllReadThreads;
            public uint NumWaitingWriteThreads;
            public uint NumWaitAllWriteThreads;
        }

        /// <summary>
        /// 
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct RWLockAttr
        {
            /// <summary>
            /// 
            /// </summary>
            public uint Protocol;
            /// <summary>
            /// 
            /// </summary>
            public uint PShared;
            /// <summary>
            /// 
            /// </summary>
            public ulong Key;
            /// <summary>
            /// 
            /// </summary>
            public uint Flags;
            /// <summary>
            /// 
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
            public string Name;
        }

        /// <summary>
        /// 
        /// </summary>
        public struct RWLockInfo
        {
            /// <summary>
            /// 
            /// </summary>
            public uint ID;
            /// <summary>
            /// 
            /// </summary>
            public RWLockAttr Attribute;
            /// <summary>
            /// 
            /// </summary>
            public ulong OwnerThreadID;
            /// <summary>
            /// 
            /// </summary>
            public uint NumWaitingReadThreads;
            /// <summary>
            /// 
            /// </summary>
            public uint NumWaitAllReadThreads;
            /// <summary>
            /// 
            /// </summary>
            public uint NumWaitingWriteThreads;
            /// <summary>
            /// 
            /// </summary>
            public uint NumWaitAllWriteThreads;
            /// <summary>
            /// 
            /// </summary>
            public ulong[] WaitingThreads;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct SemaphoreInfoPriv
        {
            public uint Id;
            public SemaphoreAttr Attribute;
            public uint MaxValue;
            public uint CurrentValue;
            public uint NumWaitingThreads;
            public uint NumWaitAllThreads;
        }

        /// <summary>
        /// 
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct SemaphoreAttr
        {
            /// <summary>
            /// 
            /// </summary>
            public uint Protocol;
            /// <summary>
            /// 
            /// </summary>
            public uint PShared;
            /// <summary>
            /// 
            /// </summary>
            public ulong Key;
            /// <summary>
            /// 
            /// </summary>
            public uint Flags;
            /// <summary>
            /// 
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
            public string Name;
        }

        /// <summary>
        /// 
        /// </summary>
        public struct SemaphoreInfo
        {
            /// <summary>
            /// 
            /// </summary>
            public uint ID;
            /// <summary>
            /// 
            /// </summary>
            public SemaphoreAttr Attribute;
            /// <summary>
            /// 
            /// </summary>
            public uint MaxValue;
            /// <summary>
            /// 
            /// </summary>
            public uint CurrentValue;
            /// <summary>
            /// 
            /// </summary>
            public uint NumWaitAllThreads;
            /// <summary>
            /// 
            /// </summary>
            public ulong[] WaitingThreads;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct EventQueueInfoPriv
        {
            public uint Id;
            public EventQueueAttr Attribute;
            public ulong Key;
            public uint Size;
            public uint NumWaitingThreads;
            public uint NumWaitAllThreads;
            public uint NumReadableEvQueue;
            public uint NumReadableAllEvQueue;
            public IntPtr WaitingThreadIds;
            public IntPtr QueueEntries;
        }

        /// <summary>
        /// 
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct EventQueueAttr
        {
            /// <summary>
            /// 
            /// </summary>
            public uint Protocol;
            /// <summary>
            /// 
            /// </summary>
            public uint Type;
            /// <summary>
            /// 
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
            public string Name;
        }

        /// <summary>
        /// 
        /// </summary>
        public struct SystemEvent
        {
            /// <summary>
            /// 
            /// </summary>
            public ulong Source;
            /// <summary>
            /// 
            /// </summary>
            public ulong Data1;
            /// <summary>
            /// 
            /// </summary>
            public ulong Data2;
            /// <summary>
            /// 
            /// </summary>
            public ulong Data3;
        }

        /// <summary>
        /// 
        /// </summary>
        public struct EventQueueInfo
        {
            /// <summary>
            /// 
            /// </summary>
            public uint ID;
            /// <summary>
            /// 
            /// </summary>
            public EventQueueAttr Attribute;
            /// <summary>
            /// 
            /// </summary>
            public ulong Key;
            /// <summary>
            /// 
            /// </summary>
            public uint Size;
            /// <summary>
            /// 
            /// </summary>
            public uint NumWaitAllThreads;
            /// <summary>
            /// 
            /// </summary>
            public uint NumReadableAllEvQueue;
            /// <summary>
            /// 
            /// </summary>
            public ulong[] WaitingThreadIDs;
            /// <summary>
            /// 
            /// </summary>
            public SystemEvent[] QueueEntries;
        }

        /// <summary>
        /// 
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct EventFlagWaitThread
        {
            /// <summary>
            /// 
            /// </summary>
            public ulong ID;
            /// <summary>
            /// 
            /// </summary>
            public ulong BitPattern;
            /// <summary>
            /// 
            /// </summary>
            public uint Mode;
        }

        /// <summary>
        /// 
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct EventFlagAttr
        {
            /// <summary>
            /// 
            /// </summary>
            public uint Protocol;
            /// <summary>
            /// 
            /// </summary>
            public uint PShared;
            /// <summary>
            /// 
            /// </summary>
            public ulong Key;
            /// <summary>
            /// 
            /// </summary>
            public uint Flags;
            /// <summary>
            /// 
            /// </summary>
            public uint Type;
            /// <summary>
            /// 
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
            public string Name;
        }

        /// <summary>
        /// 
        /// </summary>
        public struct EventFlagInfo
        {
            /// <summary>
            /// 
            /// </summary>
            public uint ID;
            /// <summary>
            /// 
            /// </summary>
            public EventFlagAttr Attribute;
            /// <summary>
            /// 
            /// </summary>
            public ulong BitPattern;
            /// <summary>
            /// 
            /// </summary>
            public uint NumWaitAllThreads;
            /// <summary>
            /// 
            /// </summary>
            public EventFlagWaitThread[] WaitingThreads;
        }

        /// <summary>
        /// 
        /// </summary>
        public enum PowerStatus
        {
            /// <summary>
            /// 
            /// </summary>
            Off,
            /// <summary>
            /// 
            /// </summary>
            On,
            /// <summary>
            /// 
            /// </summary>
            Suspended,
            /// <summary>
            /// 
            /// </summary>
            Unknown,
            /// <summary>
            /// 
            /// </summary>
            SwitchingOn,
        }

        /// <summary>
        /// 
        /// </summary>
        public struct UserMemoryStats
        {
            /// <summary>
            /// 
            /// </summary>
            public uint CreatedSharedMemorySize;
            /// <summary>
            /// 
            /// </summary>
            public uint AttachedSharedMemorySize;
            /// <summary>
            /// 
            /// </summary>
            public uint ProcessLocalMemorySize;
            /// <summary>
            /// 
            /// </summary>
            public uint ProcessLocalTextSize;
            /// <summary>
            /// 
            /// </summary>
            public uint PRXTextSize;
            /// <summary>
            /// 
            /// </summary>
            public uint PRXDataSize;
            /// <summary>
            /// 
            /// </summary>
            public uint MiscMemorySize;
        }

        /// <summary>
        /// 
        /// </summary>
        public struct GamePortIPAddressData
        {
            /// <summary>
            /// 
            /// </summary>
            public uint ReturnValue;
            /// <summary>
            /// 
            /// </summary>
            public uint IPAddress;
            /// <summary>
            /// 
            /// </summary>
            public uint SubnetMask;
            /// <summary>
            /// 
            /// </summary>
            public uint BroadcastAddress;
        }

        /// <summary>
        /// 
        /// </summary>
        [Flags]
        public enum RSXProfilingFlag : ulong
        {
            /// <summary>
            /// 
            /// </summary>
            UseRSXProfilingTools = 1,
            /// <summary>
            /// 
            /// </summary>
            UseFullHUDFeatures = 2,
        }

        /// <summary>
        /// 
        /// </summary>
        [Flags]
        public enum CoreDumpFlag : ulong
        {
            /// <summary>
            /// 
            /// </summary>
            ToDevMS = 1,
            /// <summary>
            /// 
            /// </summary>
            ToAppHome = 2,
            /// <summary>
            /// 
            /// </summary>
            ToDevUSB = 4,
            /// <summary>
            /// 
            /// </summary>
            ToDevHDD0 = 8,
            /// <summary>
            /// 
            /// </summary>
            DisablePPUExceptionDetection = 36028797018963968,
            /// <summary>
            /// 
            /// </summary>
            DisableSPUExceptionDetection = 18014398509481984,
            /// <summary>
            /// 
            /// </summary>
            DisableRSXExceptionDetection = 9007199254740992,
            /// <summary>
            /// 
            /// </summary>
            DisableFootSwitchDetection = 4503599627370496,
            /// <summary>
            /// 
            /// </summary>
            DisableMemoryDump = 3489660928,
            /// <summary>
            /// 
            /// </summary>
            EnableRestartProcess = 32768,
            /// <summary>
            /// 
            /// </summary>
            EnableKeepRunningHandler = 8192,
        }

        /// <summary>
        /// 
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ScatteredWrite
        {
            /// <summary>
            /// 
            /// </summary>
            public uint Address;
            /// <summary>
            /// 
            /// </summary>
            public byte[] Data;
        }

        /// <summary>
        /// 
        /// </summary>
        public enum MATCondition : byte
        {
            /// <summary>
            /// 
            /// </summary>
            Transparent,
            /// <summary>
            /// 
            /// </summary>
            Write,
            /// <summary>
            /// 
            /// </summary>
            ReadWrite,
            /// <summary>
            /// 
            /// </summary>
            Error,
        }

        /// <summary>
        /// 
        /// </summary>
        public struct MATRange
        {
            /// <summary>
            /// 
            /// </summary>
            public uint StartAddress;
            /// <summary>
            /// 
            /// </summary>
            public uint Size;
            /// <summary>
            /// 
            /// </summary>
            public MATCondition[] PageConditions;
        }

        /// <summary>
        /// 
        /// </summary>
        public enum PadPlaybackResponse : uint
        {
            /// <summary>
            /// 
            /// </summary>
            Ok = 0,
            /// <summary>
            /// 
            /// </summary>
            InvalidPacket = 2147549186,
            /// <summary>
            /// 
            /// </summary>
            InsufficientMemory = 2147549188,
            /// <summary>
            /// 
            /// </summary>
            Busy = 2147549194,
            /// <summary>
            /// 
            /// </summary>
            NoDev = 2147549229,
        }

        /// <summary>
        /// 
        /// </summary>
        public delegate void PadPlaybackCallback(int target, SNRESULT res, PadPlaybackResponse playbackResult, object userData);

        private class PadPlaybackCallbackAndUserData
        {
            public PadPlaybackCallback m_callback;
            public object m_userData;
        }

        /// <summary>
        /// 
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct PadData
        {
            /// <summary>
            /// 
            /// </summary>
            public uint TimeHi;
            /// <summary>
            /// 
            /// </summary>
            public uint TimeLo;
            /// <summary>
            /// 
            /// </summary>
            public uint Reserved0;
            /// <summary>
            /// 
            /// </summary>
            public uint Reserved1;
            /// <summary>
            /// 
            /// </summary>
            public byte Port;
            /// <summary>
            /// 
            /// </summary>
            public byte PortStatus;
            /// <summary>
            /// 
            /// </summary>
            public byte Length;
            /// <summary>
            /// 
            /// </summary>
            public byte Reserved2;
            /// <summary>
            /// 
            /// </summary>
            public uint Reserved3;
            /// <summary>
            /// 
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
            public short[] buttons;
        }

        /// <summary>
        /// 
        /// </summary>
        public delegate void PadCaptureCallback(int target, SNRESULT res, PadData[] padData, object userData);

        private class PadCaptureCallbackAndUserData
        {
            public PadCaptureCallback m_callback;
            public object m_userData;
        }

        /// <summary>
        /// 
        /// </summary>
        [Flags]
        public enum VRAMCaptureFlag : ulong
        {
            /// <summary>
            /// 
            /// </summary>
            Enabled = 1,
            /// <summary>
            /// 
            /// </summary>
            Disabled = 0,
        }

        /// <summary>
        /// 
        /// </summary>
        public class VRAMInfo
        {
            /// <summary>
            /// 
            /// </summary>
            public ulong BPAddress;
            /// <summary>
            /// 
            /// </summary>
            public ulong TopAddressPointer;
            /// <summary>
            /// 
            /// </summary>
            public uint Width;
            /// <summary>
            /// 
            /// </summary>
            public uint Height;
            /// <summary>
            /// 
            /// </summary>
            public uint Pitch;
            /// <summary>
            /// 
            /// </summary>
            public byte Colour;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct VramInfoPriv
        {
            public ulong BpAddress;
            public ulong TopAddressPointer;
            public uint Width;
            public uint Height;
            public uint Pitch;
            public byte Colour;
        }

        /// <summary>
        /// 
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct PS3Protocol
        {
            /// <summary>
            /// 
            /// </summary>
            public uint Protocol;
            /// <summary>
            /// 
            /// </summary>
            public uint Port;
            /// <summary>
            /// 
            /// </summary>
            public uint LPARDesc;
        }

        private struct PS3ProtocolPriv
        {
            public readonly uint Protocol;
            public readonly uint Port;

            public PS3ProtocolPriv(uint protocol, uint port)
            {
                Protocol = port;
                Port = protocol;
            }
        }

        private struct CustomProtocolId
        {
            public readonly int Target;
            public readonly PS3ProtocolPriv Protocol;

            public CustomProtocolId(int target, PS3ProtocolPriv protocol)
            {
                Target = target;
                Protocol = protocol;
            }
        }

        private delegate void CustomProtocolCallbackPriv(int target, PS3Protocol protocol, IntPtr unmanagedBuf, uint length, IntPtr userData);

        /// <summary>
        /// 
        /// </summary>
        public delegate void CustomProtocolCallback(int target, PS3Protocol protocol, byte[] data, object userData);

        private class CusProtoCallbackAndUserData
        {
            public CustomProtocolCallback m_callback;
            public object m_userData;
        }

        /// <summary>
        /// 
        /// </summary>
        [Flags]
        public enum FileServingEventFlag : ulong
        {
            /// <summary>
            /// 
            /// </summary>
            Create = 1,
            /// <summary>
            /// 
            /// </summary>
            Close = 4,
            /// <summary>
            /// 
            /// </summary>
            Read = 8,
            /// <summary>
            /// 
            /// </summary>
            Write = 16,
            /// <summary>
            /// 
            /// </summary>
            Seek = 32,
            /// <summary>
            /// 
            /// </summary>
            Delete = 64,
            /// <summary>
            /// 
            /// </summary>
            Rename = 128,
            /// <summary>
            /// 
            /// </summary>
            SetAttr = 256,
            /// <summary>
            /// 
            /// </summary>
            GetAttr = 512,
            /// <summary>
            /// 
            /// </summary>
            SetTime = 1024,
            /// <summary>
            /// 
            /// </summary>
            MKDir = 2048,
            /// <summary>
            /// 
            /// </summary>
            RMDir = 4096,
            /// <summary>
            /// 
            /// </summary>
            OpenDir = 8192,
            /// <summary>
            /// 
            /// </summary>
            CloseDir = 16384,
            /// <summary>
            /// 
            /// </summary>
            ReadDir = 32768,
            /// <summary>
            /// 
            /// </summary>
            Truncate = 65536,
            /// <summary>
            /// 
            /// </summary>
            FGetAttr64 = 131072,
            /// <summary>
            /// 
            /// </summary>
            GetAttr64 = 262144,
            /// <summary>
            /// 
            /// </summary>
            All = GetAttr64 | FGetAttr64 | Truncate | ReadDir | CloseDir | OpenDir | RMDir | MKDir | SetTime | GetAttr | SetAttr | Rename | Delete | Seek | Write | Read | Close | Create,
        }

        /// <summary>
        /// 
        /// </summary>
        public enum FileTransferNotificationType : uint
        {
            /// <summary>
            /// 
            /// </summary>
            Progress = 0,
            /// <summary>
            /// 
            /// </summary>
            Finish = 1,
            /// <summary>
            /// 
            /// </summary>
            Skipped = 2,
            /// <summary>
            /// 
            /// </summary>
            Cancelled = 3,
            /// <summary>
            /// 
            /// </summary>
            Error = 4,
            /// <summary>
            /// 
            /// </summary>
            Pending = 5,
            /// <summary>
            /// 
            /// </summary>
            Unknown = 6,
            /// <summary>
            /// 
            /// </summary>
            RefreshList = 2147483648,
        }

        /// <summary>
        /// 
        /// </summary>
        public struct FTPNotification
        {
            /// <summary>
            /// 
            /// </summary>
            public FileTransferNotificationType Type;
            /// <summary>
            /// 
            /// </summary>
            public uint TransferID;
            /// <summary>
            /// 
            /// </summary>
            public ulong BytesTransferred;
        }

        /// <summary>
        /// 
        /// </summary>
        public delegate void FTPEventCallback(int target, SNRESULT res, FTPNotification[] ftpNotifications, object userData);

        private class FtpCallbackAndUserData
        {
            public FTPEventCallback m_callback;
            public object m_userData;
        }

        /// <summary>
        /// 
        /// </summary>
        public enum FileTraceType
        {
            /// <summary>
            /// 
            /// </summary>
            GetBlockSize = 1,
            /// <summary>
            /// 
            /// </summary>
            Stat = 2,
            /// <summary>
            /// 
            /// </summary>
            WidgetStat = 3,
            /// <summary>
            /// 
            /// </summary>
            Unlink = 4,
            /// <summary>
            /// 
            /// </summary>
            WidgetUnlink = 5,
            /// <summary>
            /// 
            /// </summary>
            RMDir = 6,
            /// <summary>
            /// 
            /// </summary>
            WidgetRMDir = 7,
            /// <summary>
            /// 
            /// </summary>
            Rename = 14,
            /// <summary>
            /// 
            /// </summary>
            WidgetRename = 15,
            /// <summary>
            /// 
            /// </summary>
            Truncate = 18,
            /// <summary>
            /// 
            /// </summary>
            TruncateNoAlloc = 19,
            /// <summary>
            /// 
            /// </summary>
            Truncate2 = 20,
            /// <summary>
            /// 
            /// </summary>
            Truncate2NoInit = 21,
            /// <summary>
            /// 
            /// </summary>
            OpenDir = 24,
            /// <summary>
            /// 
            /// </summary>
            WidgetOpenDir = 25,
            /// <summary>
            /// 
            /// </summary>
            CHMod = 26,
            /// <summary>
            /// 
            /// </summary>
            MkDir = 27,
            /// <summary>
            /// 
            /// </summary>
            UTime = 29,
            /// <summary>
            /// 
            /// </summary>
            Open = 33,
            /// <summary>
            /// 
            /// </summary>
            WidgetOpen = 34,
            /// <summary>
            /// 
            /// </summary>
            Close = 35,
            /// <summary>
            /// 
            /// </summary>
            CloseDir = 36,
            /// <summary>
            /// 
            /// </summary>
            FSync = 37,
            /// <summary>
            /// 
            /// </summary>
            ReadDir = 38,
            /// <summary>
            /// 
            /// </summary>
            FStat = 39,
            /// <summary>
            /// 
            /// </summary>
            FGetBlockSize = 40,
            /// <summary>
            /// 
            /// </summary>
            Read = 47,
            /// <summary>
            /// 
            /// </summary>
            Write = 48,
            /// <summary>
            /// 
            /// </summary>
            GetDirEntries = 49,
            /// <summary>
            /// 
            /// </summary>
            ReadOffset = 50,
            /// <summary>
            /// 
            /// </summary>
            WriteOffset = 51,
            /// <summary>
            /// 
            /// </summary>
            FTruncate = 52,
            /// <summary>
            /// 
            /// </summary>
            FTruncateNoAlloc = 53,
            /// <summary>
            /// 
            /// </summary>
            LSeek = 56,
            /// <summary>
            /// 
            /// </summary>
            SetIOBuffer = 57,
            /// <summary>
            /// 
            /// </summary>
            OfflineEnd = 9999,
        }

        /// <summary>
        /// 
        /// </summary>
        public enum FileTraceNotificationStatus
        {
            /// <summary>
            /// 
            /// </summary>
            Processed,
            /// <summary>
            /// 
            /// </summary>
            Received,
            /// <summary>
            /// 
            /// </summary>
            Waiting,
            /// <summary>
            /// 
            /// </summary>
            Processing,
            /// <summary>
            /// 
            /// </summary>
            Suspended,
            /// <summary>
            /// 
            /// </summary>
            Finished,
        }

        /// <summary>
        /// 
        /// </summary>
        public struct FileTraceLogData
        {
            /// <summary>
            /// 
            /// </summary>
            public FileTraceLogType1 LogType1;
            /// <summary>
            /// 
            /// </summary>
            public FileTraceLogType2 LogType2;
            /// <summary>
            /// 
            /// </summary>
            public FileTraceLogType3 LogType3;
            /// <summary>
            /// 
            /// </summary>
            public FileTraceLogType4 LogType4;
            /// <summary>
            /// 
            /// </summary>
            public FileTraceLogType6 LogType6;
            /// <summary>
            /// 
            /// </summary>
            public FileTraceLogType8 LogType8;
            /// <summary>
            /// 
            /// </summary>
            public FileTraceLogType9 LogType9;
            /// <summary>
            /// 
            /// </summary>
            public FileTraceLogType10 LogType10;
            /// <summary>
            /// 
            /// </summary>
            public FileTraceLogType11 LogType11;
            /// <summary>
            /// 
            /// </summary>
            public FileTraceLogType12 LogType12;
            /// <summary>
            /// 
            /// </summary>
            public FileTraceLogType13 LogType13;
            /// <summary>
            /// 
            /// </summary>
            public FileTraceLogType14 LogType14;
        }

        /// <summary>
        /// 
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct FileTraceLogType1
        {
            /// <summary>
            /// 
            /// </summary>
            public string Path;
        }

        /// <summary>
        /// 
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct FileTraceLogType2
        {
            /// <summary>
            /// 
            /// </summary>
            public string Path1;
            /// <summary>
            /// 
            /// </summary>
            public string Path2;
        }

        /// <summary>
        /// 
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct FileTraceLogType3
        {
            /// <summary>
            /// 
            /// </summary>
            public ulong Arg;
            /// <summary>
            /// 
            /// </summary>
            public string Path;
        }

        /// <summary>
        /// 
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct FileTraceLogType4
        {
            /// <summary>
            /// 
            /// </summary>
            public uint Mode;
            /// <summary>
            /// 
            /// </summary>
            public string Path;
        }

        /// <summary>
        /// 
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct FileTraceLogType6
        {
            /// <summary>
            /// 
            /// </summary>
            public ulong Arg1;
            /// <summary>
            /// 
            /// </summary>
            public ulong Arg2;
            /// <summary>
            /// 
            /// </summary>
            public string Path;
        }

        /// <summary>
        /// 
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct FileTraceProcessInfo
        {
            /// <summary>
            /// 
            /// </summary>
            public ulong VFSID;
            /// <summary>
            /// 
            /// </summary>
            public ulong FD;
        }

        /// <summary>
        /// 
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct FileTraceLogType8
        {
            /// <summary>
            /// 
            /// </summary>
            public FileTraceProcessInfo ProcessInfo;
            /// <summary>
            /// 
            /// </summary>
            public uint Arg1;
            /// <summary>
            /// 
            /// </summary>
            public uint Arg2;
            /// <summary>
            /// 
            /// </summary>
            public uint Arg3;
            /// <summary>
            /// 
            /// </summary>
            public uint Arg4;
            /// <summary>
            /// 
            /// </summary>
            public byte[] VArg;
            /// <summary>
            /// 
            /// </summary>
            public string Path;
        }

        /// <summary>
        /// 
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct FileTraceLogType9
        {
            /// <summary>
            /// 
            /// </summary>
            public FileTraceProcessInfo ProcessInfo;
        }

        /// <summary>
        /// 
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct FileTraceLogType10
        {
            /// <summary>
            /// 
            /// </summary>
            public FileTraceProcessInfo ProcessInfo;
            /// <summary>
            /// 
            /// </summary>
            public uint Size;
            /// <summary>
            /// 
            /// </summary>
            public ulong Address;
            /// <summary>
            /// 
            /// </summary>
            public uint TxSize;
        }

        /// <summary>
        /// 
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct FileTraceLogType11
        {
            /// <summary>
            /// 
            /// </summary>
            public FileTraceProcessInfo ProcessInfo;
            /// <summary>
            /// 
            /// </summary>
            public uint Size;
            /// <summary>
            /// 
            /// </summary>
            public ulong Address;
            /// <summary>
            /// 
            /// </summary>
            public ulong Offset;
            /// <summary>
            /// 
            /// </summary>
            public uint TxSize;
        }

        /// <summary>
        /// 
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct FileTraceLogType12
        {
            /// <summary>
            /// 
            /// </summary>
            public FileTraceProcessInfo ProcessInfo;
            /// <summary>
            /// 
            /// </summary>
            public ulong TargetSize;
        }

        /// <summary>
        /// 
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct FileTraceLogType13
        {
            /// <summary>
            /// 
            /// </summary>
            public FileTraceProcessInfo ProcessInfo;
            /// <summary>
            /// 
            /// </summary>
            public uint Size;
            /// <summary>
            /// 
            /// </summary>
            public ulong Offset;
            /// <summary>
            /// 
            /// </summary>
            public ulong CurPos;
        }

        /// <summary>
        /// 
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct FileTraceLogType14
        {
            /// <summary>
            /// 
            /// </summary>
            public FileTraceProcessInfo ProcessInfo;
            /// <summary>
            /// 
            /// </summary>
            public uint MaxSize;
            /// <summary>
            /// 
            /// </summary>
            public uint Page;
            /// <summary>
            /// 
            /// </summary>
            public uint ContainerID;
        }

        /// <summary>
        /// 
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct FileTraceEvent
        {
            /// <summary>
            /// 
            /// </summary>
            public ulong SerialID;
            /// <summary>
            /// 
            /// </summary>
            public FileTraceType TraceType;
            /// <summary>
            /// 
            /// </summary>
            public FileTraceNotificationStatus Status;
            /// <summary>
            /// 
            /// </summary>
            public uint ProcessID;
            /// <summary>
            /// 
            /// </summary>
            public uint ThreadID;
            /// <summary>
            /// 
            /// </summary>
            public ulong TimeBaseStartOfTrace;
            /// <summary>
            /// 
            /// </summary>
            public ulong TimeBase;
            /// <summary>
            /// 
            /// </summary>
            public byte[] BackTraceData;
            /// <summary>
            /// 
            /// </summary>
            public FileTraceLogData LogData;
        }

        /// <summary>
        /// 
        /// </summary>
        public delegate void FileTraceCallback(int target, SNRESULT res, FileTraceEvent fileTraceEvent, object userData);

        private class FileTraceCallbackAndUserData
        {
            public FileTraceCallback m_callback;
            public object m_userData;
        }

        /// <summary>
        /// 
        /// </summary>
        public enum FileTransferStatus : uint
        {
            /// <summary>
            /// 
            /// </summary>
            Pending = 1,
            /// <summary>
            /// 
            /// </summary>
            Failed = 2,
            /// <summary>
            /// 
            /// </summary>
            Succeeded = 4,
            /// <summary>
            /// 
            /// </summary>
            Skipped = 8,
            /// <summary>
            /// 
            /// </summary>
            InProgress = 16,
            /// <summary>
            /// 
            /// </summary>
            Cancelled = 32,
        }

        /// <summary>
        /// 
        /// </summary>
        public struct FileTransferInfo
        {
            /// <summary>
            /// 
            /// </summary>
            public uint TransferID;
            /// <summary>
            /// 
            /// </summary>
            public FileTransferStatus Status;
            /// <summary>
            /// 
            /// </summary>
            public string SourcePath;
            /// <summary>
            /// 
            /// </summary>
            public string DestinationPath;
            /// <summary>
            /// 
            /// </summary>
            public ulong Size;
            /// <summary>
            /// 
            /// </summary>
            public ulong BytesRead;
        }

        private struct FileTransferInfoPriv
        {
            public uint TransferId;
            public uint Status;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 260)]
            public byte[] SourcePath;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1056)]
            public byte[] DestinationPath;
            public ulong Size;
            public ulong BytesRead;
        }

        /// <summary>
        /// 
        /// </summary>
        public struct Time
        {
            private int Sec;
            private int Min;
            private int Hour;
            private int MDay;
            private int Mon;
            private int Year;
            private int WDay;
            private int YDay;
            private int IsDST;
        }

        /// <summary>
        /// 
        /// </summary>
        public enum DirEntryType : uint
        {
            /// <summary>
            /// 
            /// </summary>
            Unknown,
            /// <summary>
            /// 
            /// </summary>
            Directory,
            /// <summary>
            /// 
            /// </summary>
            Regular,
            /// <summary>
            /// 
            /// </summary>
            Symlink,
        }

        /// <summary>
        /// 
        /// </summary>
        public struct DirEntry
        {
            /// <summary>
            /// 
            /// </summary>
            public DirEntryType Type;
            /// <summary>
            /// 
            /// </summary>
            public uint Mode;
            /// <summary>
            /// 
            /// </summary>
            public Time AccessTime;
            /// <summary>
            /// 
            /// </summary>
            public Time ModifiedTime;
            /// <summary>
            /// 
            /// </summary>
            public Time CreateTime;
            /// <summary>
            /// 
            /// </summary>
            public ulong Size;
            /// <summary>
            /// 
            /// </summary>
            public string Name;
        }

        private struct DirEntryPriv
        {
            public uint Type;
            public uint Mode;
            public Time AccessTime;
            public Time ModifiedTime;
            public Time CreateTime;
            public ulong Size;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public byte[] Name;
        }

        /// <summary>
        /// 
        /// </summary>
        public struct DirEntryEx
        {
            /// <summary>
            /// 
            /// </summary>
            public DirEntryType Type;
            /// <summary>
            /// 
            /// </summary>
            public uint Mode;
            /// <summary>
            /// 
            /// </summary>
            public ulong AccessTimeUTC;
            /// <summary>
            /// 
            /// </summary>
            public ulong ModifiedTimeUTC;
            /// <summary>
            /// 
            /// </summary>
            public ulong CreateTimeUTC;
            /// <summary>
            /// 
            /// </summary>
            public ulong Size;
            /// <summary>
            /// 
            /// </summary>
            public string Name;
        }

        private struct DirEntryExPriv
        {
            public uint Type;
            public uint Mode;
            public ulong AccessTimeUTC;
            public ulong ModifiedTimeUTC;
            public ulong CreateTimeUTC;
            public ulong Size;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public byte[] Name;
        }

        /// <summary>
        /// 
        /// </summary>
        public struct TargetTimezone
        {
            /// <summary>
            /// 
            /// </summary>
            public int Timezone;
            /// <summary>
            /// 
            /// </summary>
            public int DST;
        }

        /// <summary>
        /// 
        /// </summary>
        public enum ChModFilePermission : uint
        {
            /// <summary>
            /// 
            /// </summary>
            ReadOnly = 256,
            /// <summary>
            /// 
            /// </summary>
            ReadWrite = 384,
        }

        /// <summary>
        /// 
        /// </summary>
        public enum LogCategory : uint
        {
            /// <summary>
            /// 
            /// </summary>
            Off = 0,
            /// <summary>
            /// 
            /// </summary>
            All = 4294967295,
        }

        /// <summary>
        /// 
        /// </summary>
        public struct BDInfo
        {
            /// <summary>
            /// 
            /// </summary>
            public uint bdemu_data_size;
            /// <summary>
            /// 
            /// </summary>
            public byte bdemu_total_entry;
            /// <summary>
            /// 
            /// </summary>
            public byte bdemu_selected_index;
            /// <summary>
            /// 
            /// </summary>
            public byte image_index;
            /// <summary>
            /// 
            /// </summary>
            public byte image_type;
            /// <summary>
            /// 
            /// </summary>
            public string image_file_name;
            /// <summary>
            /// 
            /// </summary>
            public ulong image_file_size;
            /// <summary>
            /// 
            /// </summary>
            public string image_product_code;
            /// <summary>
            /// 
            /// </summary>
            public string image_producer;
            /// <summary>
            /// 
            /// </summary>
            public string image_author;
            /// <summary>
            /// 
            /// </summary>
            public string image_date;
            /// <summary>
            /// 
            /// </summary>
            public uint image_sector_layer0;
            /// <summary>
            /// 
            /// </summary>
            public uint image_sector_layer1;
            /// <summary>
            /// 
            /// </summary>
            public string image_memorandum;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct BDInfoPriv
        {
            public uint bdemu_data_size;
            public byte bdemu_total_entry;
            public byte bdemu_selected_index;
            public byte image_index;
            public byte image_type;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
            public byte[] image_file_name;
            public ulong image_file_size;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] image_product_code;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] image_producer;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] image_author;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] image_date;
            public uint image_sector_layer0;
            public uint image_sector_layer1;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] image_memorandum;
        }

        /// <summary>
        /// 
        /// </summary>
        public enum TargetEventType : uint
        {
            /// <summary>
            /// 
            /// </summary>
            UnitStatusChange = 0,
            /// <summary>
            /// 
            /// </summary>
            ResetStarted = 1,
            /// <summary>
            /// 
            /// </summary>
            ResetEnd = 2,
            /// <summary>
            /// 
            /// </summary>
            Details = 4,
            /// <summary>
            /// 
            /// </summary>
            ModuleLoad = 5,
            /// <summary>
            /// 
            /// </summary>
            ModuleRunning = 6,
            /// <summary>
            /// 
            /// </summary>
            ModuleDoneRemove = 7,
            /// <summary>
            /// 
            /// </summary>
            ModuleDoneResident = 8,
            /// <summary>
            /// 
            /// </summary>
            ModuleStopped = 9,
            /// <summary>
            /// 
            /// </summary>
            ModuleStoppedRemove = 10,
            /// <summary>
            /// 
            /// </summary>
            PowerStatusChange = 11,
            /// <summary>
            /// 
            /// </summary>
            TTYStreamAdded = 12,
            /// <summary>
            /// 
            /// </summary>
            TTYStreamDeleted = 13,
            /// <summary>
            /// 
            /// </summary>
            BDIsotransferStarted = 16,
            /// <summary>
            /// 
            /// </summary>
            BDIsotransferFinished = 17,
            /// <summary>
            /// 
            /// </summary>
            BDFormatStarted = 18,
            /// <summary>
            /// 
            /// </summary>
            BDFormatFinished = 19,
            /// <summary>
            /// 
            /// </summary>
            BDMountStarted = 20,
            /// <summary>
            /// 
            /// </summary>
            BDMountFinished = 21,
            /// <summary>
            /// 
            /// </summary>
            BDUnmountStarted = 22,
            /// <summary>
            /// 
            /// </summary>
            BDUnmountFinished = 23,
            /// <summary>
            /// 
            /// </summary>
            TargetSpecific = 2147483648,
        }

        /// <summary>
        /// 
        /// </summary>
        public struct TGTEventUnitStatusChangeData
        {
            /// <summary>
            /// 
            /// </summary>
            public UnitType Unit;
            /// <summary>
            /// 
            /// </summary>
            public UnitStatus Status;
        }

        private struct TGTEventUnitStatusChangeDataPriv
        {
            public int Unit;
            public uint Status;
        }

        /// <summary>
        /// 
        /// </summary>
        public struct TGTEventDetailsData
        {
            /// <summary>
            /// 
            /// </summary>
            public uint Flags;
        }

        /// <summary>
        /// 
        /// </summary>
        public struct TGTEventModuleEventData
        {
            /// <summary>
            /// 
            /// </summary>
            public uint Unit;
            /// <summary>
            /// 
            /// </summary>
            public uint ModuleID;
        }

        /// <summary>
        /// 
        /// </summary>
        public struct TGTEventBDData
        {
            /// <summary>
            /// 
            /// </summary>
            public uint Result;
            /// <summary>
            /// 
            /// </summary>
            public string Source;
            /// <summary>
            /// 
            /// </summary>
            public string Destination;
        }

        private struct TGTEventBDDataPriv
        {
            public uint Result;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 512)]
            public byte[] Source;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 512)]
            public byte[] Destination;
        }

        /// <summary>
        /// 
        /// </summary>
        public struct TargetEventData
        {
            /// <summary>
            /// 
            /// </summary>
            public TGTEventUnitStatusChangeData UnitStatusChangeData;
            /// <summary>
            /// 
            /// </summary>
            public TGTEventDetailsData DetailsData;
            /// <summary>
            /// 
            /// </summary>
            public TGTEventModuleEventData ModuleEventData;
            /// <summary>
            /// 
            /// </summary>
            public TGTEventBDData BdData;
        }

        /// <summary>
        /// 
        /// </summary>
        public struct TargetEvent
        {
            /// <summary>
            /// 
            /// </summary>
            public uint TargetID;
            /// <summary>
            /// 
            /// </summary>
            public TargetEventType Type;
            /// <summary>
            /// 
            /// </summary>
            public TargetEventData EventData;
            /// <summary>
            /// 
            /// </summary>
            public TargetSpecificEvent TargetSpecific;
        }

        /// <summary>
        /// 
        /// </summary>
        public delegate void TargetEventCallback(int target, SNRESULT res, TargetEvent[] targetEventList, object userData);

        private class TargetCallbackAndUserData
        {
            public TargetEventCallback m_callback;
            public object m_userData;
        }

        private struct TargetEventHdrPriv
        {
            public uint Size;
            public uint TargetID;
            public uint EventType;
        }

        /// <summary>
        /// 
        /// </summary>
        public enum TargetSpecificEventType : uint
        {
            /// <summary>
            /// 
            /// </summary>
            ProcessCreate = 0,
            /// <summary>
            /// 
            /// </summary>
            ProcessExit = 1,
            /// <summary>
            /// 
            /// </summary>
            ProcessKill = 2,
            /// <summary>
            /// 
            /// </summary>
            ProcessExitSpawn = 3,
            /// <summary>
            /// 
            /// </summary>
            PPUExcTrap = 16,
            /// <summary>
            /// 
            /// </summary>
            PPUExcPrevInt = 17,
            /// <summary>
            /// 
            /// </summary>
            PPUExcAlignment = 18,
            /// <summary>
            /// 
            /// </summary>
            PPUExcIllInst = 19,
            /// <summary>
            /// 
            /// </summary>
            PPUExcTextHtabMiss = 20,
            /// <summary>
            /// 
            /// </summary>
            PPUExcTextSlbMiss = 21,
            /// <summary>
            /// 
            /// </summary>
            PPUExcDataHtabMiss = 22,
            /// <summary>
            /// 
            /// </summary>
            PPUExcFloat = 23,
            /// <summary>
            /// 
            /// </summary>
            PPUExcDataSlbMiss = 24,
            /// <summary>
            /// 
            /// </summary>
            PPUExcDabrMatch = 25,
            /// <summary>
            /// 
            /// </summary>
            PPUExcStop = 26,
            /// <summary>
            /// 
            /// </summary>
            PPUExcStopInit = 27,
            /// <summary>
            /// 
            /// </summary>
            PPUExcDataMAT = 28,
            /// <summary>
            /// 
            /// </summary>
            PPUThreadCreate = 32,
            /// <summary>
            /// 
            /// </summary>
            PPUThreadExit = 33,
            /// <summary>
            /// 
            /// </summary>
            SPUThreadStart = 48,
            /// <summary>
            /// 
            /// </summary>
            SPUThreadStop = 49,
            /// <summary>
            /// 
            /// </summary>
            SPUThreadStopInit = 50,
            /// <summary>
            /// 
            /// </summary>
            SPUThreadGroupDestroy = 51,
            /// <summary>
            /// 
            /// </summary>
            SPUThreadStopEx = 52,
            /// <summary>
            /// 
            /// </summary>
            PRXLoad = 64,
            /// <summary>
            /// 
            /// </summary>
            PRXUnload = 65,
            /// <summary>
            /// 
            /// </summary>
            DAInitialised = 96,
            /// <summary>
            /// 
            /// </summary>
            Footswitch = 112,
            /// <summary>
            /// 
            /// </summary>
            InstallPackageProgress = 128,
            /// <summary>
            /// 
            /// </summary>
            InstallPackagePath = 129,
            /// <summary>
            /// 
            /// </summary>
            CoreDumpComplete = 256,
            /// <summary>
            /// 
            /// </summary>
            CoreDumpStart = 257,
            /// <summary>
            /// 
            /// </summary>
            RawNotify = 4026531855,
        }

        /// <summary>
        /// 
        /// </summary>
        public struct PPUProcessCreateData
        {
            /// <summary>
            /// 
            /// </summary>
            public uint ParentProcessID;
            /// <summary>
            /// 
            /// </summary>
            public string Filename;
        }

        /// <summary>
        /// 
        /// </summary>
        public struct PPUProcessExitData
        {
            /// <summary>
            /// 
            /// </summary>
            public ulong ExitCode;
        }

        /// <summary>
        /// 
        /// </summary>
        public struct PPUExceptionData
        {
            /// <summary>
            /// 
            /// </summary>
            public ulong ThreadID;
            /// <summary>
            /// 
            /// </summary>
            public uint HWThreadNumber;
            /// <summary>
            /// 
            /// </summary>
            public ulong PC;
            /// <summary>
            /// 
            /// </summary>
            public ulong SP;
        }

        /// <summary>
        /// 
        /// </summary>
        public struct PPUAlignmentExceptionData
        {
            /// <summary>
            /// 
            /// </summary>
            public ulong ThreadID;
            /// <summary>
            /// 
            /// </summary>
            public uint HWThreadNumber;
            /// <summary>
            /// 
            /// </summary>
            public ulong DSISR;
            /// <summary>
            /// 
            /// </summary>
            public ulong DAR;
            /// <summary>
            /// 
            /// </summary>
            public ulong PC;
            /// <summary>
            /// 
            /// </summary>
            public ulong SP;
        }

        /// <summary>
        /// 
        /// </summary>
        public struct PPUDataMatExceptionData
        {
            /// <summary>
            /// 
            /// </summary>
            /// <summary>
            /// 
            /// </summary>
            public ulong ThreadID;
            /// <summary>
            /// 
            /// </summary>
            public uint HWThreadNumber;
            /// <summary>
            /// 
            /// </summary>
            public ulong DSISR;
            /// <summary>
            /// 
            /// </summary>
            public ulong DAR;
            /// <summary>
            /// 
            /// </summary>
            public ulong PC;
            /// <summary>
            /// 
            /// </summary>
            public ulong SP;
        }

        /// <summary>
        /// 
        /// </summary>
        public struct PPUThreadCreateData
        {
            /// <summary>
            /// 
            /// </summary>
            public ulong ThreadID;
        }

        /// <summary>
        /// 
        /// </summary>
        public struct PPUThreadExitData
        {
            /// <summary>
            /// 
            /// </summary>
            public ulong ThreadID;
        }

        /// <summary>
        /// 
        /// </summary>
        public struct SPUThreadStartData
        {
            /// <summary>
            /// 
            /// </summary>
            public uint ThreadGroupID;
            /// <summary>
            /// 
            /// </summary>
            public uint ThreadID;
            /// <summary>
            /// 
            /// </summary>
            public string ElfFilename;
        }

        /// <summary>
        /// 
        /// </summary>
        public enum SPUThreadStopReason : uint
        {
            /// <summary>
            /// 
            /// </summary>
            NoException = 0,
            /// <summary>
            /// 
            /// </summary>
            DMAAlignment = 1,
            /// <summary>
            /// 
            /// </summary>
            DMACommand = 2,
            /// <summary>
            /// 
            /// </summary>
            Error = 4,
            /// <summary>
            /// 
            /// </summary>
            MFCFIR = 8,
            /// <summary>
            /// 
            /// </summary>
            MFCSegment = 16,
            /// <summary>
            /// 
            /// </summary>
            MFCStorage = 32,
            /// <summary>
            /// 
            /// </summary>
            NoValue = 64,
            /// <summary>
            /// 
            /// </summary>
            StopCall = 256,
            /// <summary>
            /// 
            /// </summary>
            StopDCall = 512,
            /// <summary>
            /// 
            /// </summary>
            Halt = 1024,
        }

        /// <summary>
        /// 
        /// </summary>
        public struct SPUThreadStopData
        {
            /// <summary>
            /// 
            /// </summary>
            public uint ThreadGroupID;
            /// <summary>
            /// 
            /// </summary>
            public uint ThreadID;
            /// <summary>
            /// 
            /// </summary>
            public uint PC;
            /// <summary>
            /// 
            /// </summary>
            public SPUThreadStopReason Reason;
            /// <summary>
            /// 
            /// </summary>
            public uint SP;
        }

        /// <summary>
        /// 
        /// </summary>
        public struct SPUThreadStopExData
        {
            /// <summary>
            /// 
            /// </summary>
            public uint ThreadGroupID;
            /// <summary>
            /// 
            /// </summary>
            public uint ThreadID;
            /// <summary>
            /// 
            /// </summary>
            public uint PC;
            /// <summary>
            /// 
            /// </summary>
            public SPUThreadStopReason Reason;
            /// <summary>
            /// 
            /// </summary>
            public uint SP;
            /// <summary>
            /// 
            /// </summary>
            public ulong MFCDSISR;
            /// <summary>
            /// 
            /// </summary>
            public ulong MFCDSIPR;
            /// <summary>
            /// 
            /// </summary>
            public ulong MFCDAR;
        }

        /// <summary>
        /// 
        /// </summary>
        public struct SPUThreadGroupDestroyData
        {
            /// <summary>
            /// 
            /// </summary>
            public uint ThreadGroupID;
        }

        /// <summary>
        /// 
        /// </summary>
        public struct NotifyPRXLoadData
        {
            /// <summary>
            /// 
            /// </summary>
            public ulong PPUThreadID;
            /// <summary>
            /// 
            /// </summary>
            public uint PRXID;
            /// <summary>
            /// 
            /// </summary>
            public ulong Timestamp;
        }

        /// <summary>
        /// 
        /// </summary>
        public struct NotifyPRXUnloadData
        {
            /// <summary>
            /// 
            /// </summary>
            public ulong PPUThreadID;
            /// <summary>
            /// 
            /// </summary>
            public uint PRXID;
            /// <summary>
            /// 
            /// </summary>
            public ulong Timestamp;
        }

        /// <summary>
        /// 
        /// </summary>
        public struct FootswitchData
        {
            /// <summary>
            /// 
            /// </summary>
            public ulong EventSource;
            /// <summary>
            /// 
            /// </summary>
            public ulong EventData1;
            /// <summary>
            /// 
            /// </summary>
            public ulong EventData2;
            /// <summary>
            /// 
            /// </summary>
            public ulong EventData3;
            /// <summary>
            /// 
            /// </summary>
            public ulong Reserved;
        }

        /// <summary>
        /// 
        /// </summary>
        public struct InstallPackageProgress
        {
            /// <summary>
            /// 
            /// </summary>
            public uint Percent;
        }

        /// <summary>
        /// 
        /// </summary>
        public struct InstallPackagePath
        {
            /// <summary>
            /// 
            /// </summary>
            public string Path;
        }

        /// <summary>
        /// 
        /// </summary>
        public struct CoreDumpComplete
        {
            /// <summary>
            /// 
            /// </summary>
            public string Filename;
        }

        /// <summary>
        /// 
        /// </summary>
        public struct CoreDumpStart
        {
            /// <summary>
            /// 
            /// </summary>
            public string Filename;
        }

        /// <summary>
        /// 
        /// </summary>
        public struct TargetSpecificData
        {
            /// <summary>
            /// 
            /// </summary>
            public TargetSpecificEventType Type;
            /// <summary>
            /// 
            /// </summary>
            public PPUProcessCreateData PPUProcessCreate;
            /// <summary>
            /// 
            /// </summary>
            public PPUProcessExitData PPUProcessExit;
            /// <summary>
            /// 
            /// </summary>
            public PPUExceptionData PPUException;
            /// <summary>
            /// 
            /// </summary>
            public PPUAlignmentExceptionData PPUAlignmentException;
            /// <summary>
            /// 
            /// </summary>
            public PPUDataMatExceptionData PPUDataMatException;
            /// <summary>
            /// 
            /// </summary>
            public PPUThreadCreateData PPUThreadCreate;
            /// <summary>
            /// 
            /// </summary>
            public PPUThreadExitData PPUThreadExit;
            /// <summary>
            /// 
            /// </summary>
            public SPUThreadStartData SPUThreadStart;
            /// <summary>
            /// 
            /// </summary>
            public SPUThreadStopData SPUThreadStop;
            /// <summary>
            /// 
            /// </summary>
            public SPUThreadStopExData SPUThreadStopEx;
            /// <summary>
            /// 
            /// </summary>
            public SPUThreadGroupDestroyData SPUThreadGroupDestroyData;
            /// <summary>
            /// 
            /// </summary>
            public NotifyPRXLoadData PRXLoad;
            /// <summary>
            /// 
            /// </summary>
            public NotifyPRXUnloadData PRXUnload;
            /// <summary>
            /// 
            /// </summary>
            public FootswitchData Footswitch;
            /// <summary>
            /// 
            /// </summary>
            public InstallPackageProgress InstallPackageProgress;
            /// <summary>
            /// 
            /// </summary>
            public InstallPackagePath InstallPackagePath;
            /// <summary>
            /// 
            /// </summary>
            public CoreDumpStart CoreDumpStart;
            /// <summary>
            /// 
            /// </summary>
            public CoreDumpComplete CoreDumpComplete;
        }

        /// <summary>
        /// 
        /// </summary>
        public struct TargetSpecificEvent
        {
            /// <summary>
            /// 
            /// </summary>
            public uint CommandID;
            /// <summary>
            /// 
            /// </summary>
            public uint RequestID;
            /// <summary>
            /// 
            /// </summary>
            public uint ProcessID;
            /// <summary>
            /// 
            /// </summary>
            public uint Result;
            /// <summary>
            /// 
            /// </summary>
            public TargetSpecificData Data;
        }

        private enum EventType
        {
            TTY = 100,
            Target = 101,
            System = 102,
            FTP = 103,
            PadCapture = 104,
            FileTrace = 105,
            PadPlayback = 106,
            Server = 107,
        }

        private delegate void HandleEventCallbackPriv(int target, EventType type, uint param, SNRESULT result, uint length, IntPtr data, IntPtr userData);

        /// <summary>
        /// 
        /// </summary>
        public delegate void SearchTargetsCallback(string name, string type, TCPIPConnectProperties ConnectInfo, object userData);

        private delegate void SearchTargetsCallbackPriv(IntPtr name, IntPtr type, IntPtr connectInfo, IntPtr userData);

        private class SearchForTargetsCallbackHandler
        {
            private SearchTargetsCallback m_SearchForTargetCallback;
            private object m_UserData;

            public SearchForTargetsCallbackHandler(SearchTargetsCallback callback, object userData)
            {
                m_SearchForTargetCallback = callback;
                m_UserData = userData;
            }

            public static void SearchForTargetsCallback(IntPtr namePtr, IntPtr typePtr, IntPtr connectInfoPtr, IntPtr userDataPtr)
            {
                SearchForTargetsCallbackHandler target = (SearchForTargetsCallbackHandler)GCHandle.FromIntPtr(userDataPtr).Target;
                TCPIPConnectProperties ConnectInfo = null;
                if (connectInfoPtr != IntPtr.Zero)
                {
                    ConnectInfo = new TCPIPConnectProperties();
                    Marshal.PtrToStructure(connectInfoPtr, (object)ConnectInfo);
                }
                string name = Utf8ToString(namePtr, uint.MaxValue);
                if (name == "")
                    name = null;
                string type = Utf8ToString(typePtr, uint.MaxValue);
                target.m_SearchForTargetCallback(name, type, ConnectInfo, target.m_UserData);
            }
        }

        private class ScopedGlobalHeapPtr
        {
            private readonly IntPtr m_intPtr = IntPtr.Zero;

            public ScopedGlobalHeapPtr(IntPtr intPtr)
            {
                m_intPtr = intPtr;
            }

            ~ScopedGlobalHeapPtr()
            {
                if (!(m_intPtr != IntPtr.Zero))
                    return;
                Marshal.FreeHGlobal(m_intPtr);
            }

            public IntPtr Get()
            {
                return m_intPtr;
            }
        }
    }
}