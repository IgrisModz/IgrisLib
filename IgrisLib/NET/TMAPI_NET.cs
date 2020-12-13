using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace IgrisLib.NET
{
    public class PS3TMAPI
    {
        private static PS3TMAPI.EnumerateTargetsExCallbackPriv ms_enumTargetsExCallbackPriv = new PS3TMAPI.EnumerateTargetsExCallbackPriv(PS3TMAPI.EnumTargetsExPriv);
        [ThreadStatic]
        private static PS3TMAPI.EnumerateTargetsExCallback ms_enumTargetsExCallback = (PS3TMAPI.EnumerateTargetsExCallback)null;
        [ThreadStatic]
        private static object ms_enumTargetsExUserData = (object)null;
        [ThreadStatic]
        private static PS3TMAPI.ServerEventCallback ms_serverEventCallback = (PS3TMAPI.ServerEventCallback)null;
        [ThreadStatic]
        private static object ms_serverEventUserData = (object)null;
        [ThreadStatic]
        private static Dictionary<PS3TMAPI.TTYChannel, PS3TMAPI.TTYCallbackAndUserData> ms_userTtyCallbacks = (Dictionary<PS3TMAPI.TTYChannel, PS3TMAPI.TTYCallbackAndUserData>)null;
        [ThreadStatic]
        private static Dictionary<int, PS3TMAPI.PadPlaybackCallbackAndUserData> ms_userPadPlaybackCallbacks = (Dictionary<int, PS3TMAPI.PadPlaybackCallbackAndUserData>)null;
        [ThreadStatic]
        private static Dictionary<int, PS3TMAPI.PadCaptureCallbackAndUserData> ms_userPadCaptureCallbacks = (Dictionary<int, PS3TMAPI.PadCaptureCallbackAndUserData>)null;
        private static PS3TMAPI.CustomProtocolCallbackPriv ms_customProtoCallbackPriv = new PS3TMAPI.CustomProtocolCallbackPriv(PS3TMAPI.CustomProtocolHandler);
        [ThreadStatic]
        private static Dictionary<PS3TMAPI.CustomProtocolId, PS3TMAPI.CusProtoCallbackAndUserData> ms_userCustomProtoCallbacks = (Dictionary<PS3TMAPI.CustomProtocolId, PS3TMAPI.CusProtoCallbackAndUserData>)null;
        [ThreadStatic]
        private static Dictionary<int, PS3TMAPI.FtpCallbackAndUserData> ms_userFtpCallbacks = (Dictionary<int, PS3TMAPI.FtpCallbackAndUserData>)null;
        [ThreadStatic]
        private static Dictionary<int, PS3TMAPI.FileTraceCallbackAndUserData> ms_userFileTraceCallbacks = (Dictionary<int, PS3TMAPI.FileTraceCallbackAndUserData>)null;
        [ThreadStatic]
        private static Dictionary<int, PS3TMAPI.TargetCallbackAndUserData> ms_userTargetCallbacks = (Dictionary<int, PS3TMAPI.TargetCallbackAndUserData>)null;
        private static PS3TMAPI.HandleEventCallbackPriv ms_eventHandlerWrapper = new PS3TMAPI.HandleEventCallbackPriv(PS3TMAPI.EventHandlerWrapper);
        public const int InvalidTarget = -1;
        public const int DefaultTarget = -2;
        public const uint AllTTYStreams = 4294967295;
        public const uint DefaultProcessPriority = 999;
        public const uint DefaultProtocolPriority = 128;

        public static bool FAILED(PS3TMAPI.SNRESULT res)
        {
            return !PS3TMAPI.SUCCEEDED(res);
        }

        public static bool SUCCEEDED(PS3TMAPI.SNRESULT res)
        {
            return res >= PS3TMAPI.SNRESULT.SN_S_OK;
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
            major = PS3TMAPI.VersionMajor(version);
            minor = PS3TMAPI.VersionMinor(version);
            fix = PS3TMAPI.VersionFix(version);
        }

        public static byte SDKVersionMajor(ulong sdkVersion)
        {
            return PS3TMAPI.VersionMajor(sdkVersion);
        }

        public static byte SDKVersionMinor(ulong sdkVersion)
        {
            return PS3TMAPI.VersionMinor(sdkVersion);
        }

        public static byte SDKVersionFix(ulong sdkVersion)
        {
            return PS3TMAPI.VersionFix(sdkVersion);
        }

        public static void SDKVersionComponents(ulong sdkVersion, out byte major, out byte minor, out byte fix)
        {
            major = PS3TMAPI.SDKVersionMajor(sdkVersion);
            minor = PS3TMAPI.SDKVersionMinor(sdkVersion);
            fix = PS3TMAPI.SDKVersionFix(sdkVersion);
        }

        public static byte CPVersionMajor(ulong cpVersion)
        {
            return PS3TMAPI.VersionMajor(cpVersion);
        }

        public static byte CPVersionMinor(ulong cpVersion)
        {
            return PS3TMAPI.VersionMinor(cpVersion);
        }

        public static byte CPVersionFix(ulong cpVersion)
        {
            return PS3TMAPI.VersionFix(cpVersion);
        }

        public static void CPVersionComponents(ulong cpVersion, out byte major, out byte minor, out byte fix)
        {
            major = PS3TMAPI.CPVersionMajor(cpVersion);
            minor = PS3TMAPI.CPVersionMinor(cpVersion);
            fix = PS3TMAPI.CPVersionFix(cpVersion);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetTMVersion", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetTMVersionX86(out IntPtr version);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetTMVersion", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetTMVersionX64(out IntPtr version);

        public static PS3TMAPI.SNRESULT GetTMVersion(out string version)
        {
            IntPtr version1;
            PS3TMAPI.SNRESULT snresult = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetTMVersionX86(out version1) : PS3TMAPI.GetTMVersionX64(out version1);
            version = PS3TMAPI.Utf8ToString(version1, uint.MaxValue);
            return snresult;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetAPIVersion", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetAPIVersionX86(out IntPtr version);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetAPIVersion", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetAPIVersionX64(out IntPtr version);

        public static PS3TMAPI.SNRESULT GetAPIVersion(out string version)
        {
            IntPtr version1;
            PS3TMAPI.SNRESULT snresult = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetAPIVersionX86(out version1) : PS3TMAPI.GetAPIVersionX64(out version1);
            version = PS3TMAPI.Utf8ToString(version1, uint.MaxValue);
            return snresult;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3TranslateError", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT TranslateErrorX86(PS3TMAPI.SNRESULT res, out IntPtr message);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3TranslateError", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT TranslateErrorX64(PS3TMAPI.SNRESULT res, out IntPtr message);

        public static PS3TMAPI.SNRESULT TranslateError(PS3TMAPI.SNRESULT errorCode, out string message)
        {
            IntPtr message1;
            PS3TMAPI.SNRESULT snresult = PS3TMAPI.Is32Bit() ? PS3TMAPI.TranslateErrorX86(errorCode, out message1) : PS3TMAPI.TranslateErrorX64(errorCode, out message1);
            message = PS3TMAPI.Utf8ToString(message1, uint.MaxValue);
            return snresult;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetErrorQualifier", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetErrorQualifierX86(out uint qualifier, out IntPtr message);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetErrorQualifier", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetErrorQualifierX64(out uint qualifier, out IntPtr message);

        public static PS3TMAPI.SNRESULT GetErrorQualifier(out uint qualifier, out string message)
        {
            IntPtr message1;
            PS3TMAPI.SNRESULT snresult = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetErrorQualifierX86(out qualifier, out message1) : PS3TMAPI.GetErrorQualifierX64(out qualifier, out message1);
            message = PS3TMAPI.Utf8ToString(message1, uint.MaxValue);
            return snresult;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetConnectStatus", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetConnectStatusX86(int target, out uint status, out IntPtr usage);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetConnectStatus", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetConnectStatusX64(int target, out uint status, out IntPtr usage);

        public static PS3TMAPI.SNRESULT GetConnectStatus(int target, out PS3TMAPI.ConnectStatus status, out string usage)
        {
            uint status1;
            IntPtr usage1;
            PS3TMAPI.SNRESULT snresult = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetConnectStatusX86(target, out status1, out usage1) : PS3TMAPI.GetConnectStatusX64(target, out status1, out usage1);
            status = (PS3TMAPI.ConnectStatus)status1;
            usage = PS3TMAPI.Utf8ToString(usage1, uint.MaxValue);
            return snresult;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3InitTargetComms", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT InitTargetCommsX86();

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3InitTargetComms", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT InitTargetCommsX64();

        public static PS3TMAPI.SNRESULT InitTargetComms()
        {
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.InitTargetCommsX64();
            return PS3TMAPI.InitTargetCommsX86();
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3CloseTargetComms", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT CloseTargetCommsX86();

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3CloseTargetComms", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT CloseTargetCommsX64();

        public static PS3TMAPI.SNRESULT CloseTargetComms()
        {
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.CloseTargetCommsX64();
            return PS3TMAPI.CloseTargetCommsX86();
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3EnumerateTargets", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT EnumerateTargetsX86(PS3TMAPI.EnumerateTargetsCallback callback);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3EnumerateTargets", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT EnumerateTargetsX64(PS3TMAPI.EnumerateTargetsCallback callback);

        public static PS3TMAPI.SNRESULT EnumerateTargets(PS3TMAPI.EnumerateTargetsCallback callback)
        {
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.EnumerateTargetsX64(callback);
            return PS3TMAPI.EnumerateTargetsX86(callback);
        }

        private static int EnumTargetsExPriv(int target, IntPtr unused)
        {
            if (PS3TMAPI.ms_enumTargetsExCallback == null)
                return -1;
            return PS3TMAPI.ms_enumTargetsExCallback(target, PS3TMAPI.ms_enumTargetsExUserData);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3EnumerateTargetsEx", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT EnumerateTargetsExX86(PS3TMAPI.EnumerateTargetsExCallbackPriv callback, IntPtr unused);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3EnumerateTargetsEx", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT EnumerateTargetsExX64(PS3TMAPI.EnumerateTargetsExCallbackPriv callback, IntPtr unused);

        public static PS3TMAPI.SNRESULT EnumerateTargetsEx(PS3TMAPI.EnumerateTargetsExCallback callback, ref object userData)
        {
            if (callback == null)
                return PS3TMAPI.SNRESULT.SN_E_BAD_PARAM;
            PS3TMAPI.ms_enumTargetsExCallback = callback;
            PS3TMAPI.ms_enumTargetsExUserData = userData;
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.EnumerateTargetsExX64(PS3TMAPI.ms_enumTargetsExCallbackPriv, IntPtr.Zero);
            return PS3TMAPI.EnumerateTargetsExX86(PS3TMAPI.ms_enumTargetsExCallbackPriv, IntPtr.Zero);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetNumTargets", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetNumTargetsX86(out uint numTargets);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetNumTargets", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetNumTargetsX64(out uint numTargets);

        public static PS3TMAPI.SNRESULT GetNumTargets(out uint numTargets)
        {
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.GetNumTargetsX64(out numTargets);
            return PS3TMAPI.GetNumTargetsX86(out numTargets);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetTargetFromName", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetTargetFromNameX86(IntPtr name, out int target);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetTargetFromName", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetTargetFromNameX64(IntPtr name, out int target);

        public static PS3TMAPI.SNRESULT GetTargetFromName(string name, out int target)
        {
            PS3TMAPI.ScopedGlobalHeapPtr scopedGlobalHeapPtr = new PS3TMAPI.ScopedGlobalHeapPtr(PS3TMAPI.AllocUtf8FromString(name));
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.GetTargetFromNameX64(scopedGlobalHeapPtr.Get(), out target);
            return PS3TMAPI.GetTargetFromNameX86(scopedGlobalHeapPtr.Get(), out target);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3Reset", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT ResetX86(int target, ulong resetParameter);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3Reset", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT ResetX64(int target, ulong resetParameter);

        public static PS3TMAPI.SNRESULT Reset(int target, PS3TMAPI.ResetParameter resetParameter)
        {
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.ResetX64(target, (ulong)resetParameter);
            return PS3TMAPI.ResetX86(target, (ulong)resetParameter);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ResetEx", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT ResetExX86(int target, ulong boot, ulong bootMask, ulong reset, ulong resetMask, ulong system, ulong systemMask);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ResetEx", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT ResetExX64(int target, ulong boot, ulong bootMask, ulong reset, ulong resetMask, ulong system, ulong systemMask);

        public static PS3TMAPI.SNRESULT ResetEx(int target, PS3TMAPI.BootParameter bootParameter, PS3TMAPI.BootParameterMask bootMask, PS3TMAPI.ResetParameter resetParameter, PS3TMAPI.ResetParameterMask resetMask, PS3TMAPI.SystemParameter systemParameter, PS3TMAPI.SystemParameterMask systemMask)
        {
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.ResetExX64(target, (ulong)bootParameter, (ulong)bootMask, (ulong)resetParameter, (ulong)resetMask, (ulong)systemParameter, (ulong)systemMask);
            return PS3TMAPI.ResetExX86(target, (ulong)bootParameter, (ulong)bootMask, (ulong)resetParameter, (ulong)resetMask, (ulong)systemParameter, (ulong)systemMask);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetResetParameters", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetResetParametersX86(int target, out ulong boot, out ulong bootMask, out ulong reset, out ulong resetMask, out ulong system, out ulong systemMask);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetResetParameters", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetResetParametersX64(int target, out ulong boot, out ulong bootMask, out ulong reset, out ulong resetMask, out ulong system, out ulong systemMask);

        public static PS3TMAPI.SNRESULT GetResetParameters(int target, out PS3TMAPI.BootParameter bootParameter, out PS3TMAPI.BootParameterMask bootMask, out PS3TMAPI.ResetParameter resetParameter, out PS3TMAPI.ResetParameterMask resetMask, out PS3TMAPI.SystemParameter systemParameter, out PS3TMAPI.SystemParameterMask systemMask)
        {
            ulong boot;
            ulong bootMask1;
            ulong reset;
            ulong resetMask1;
            ulong system;
            ulong systemMask1;
            PS3TMAPI.SNRESULT snresult = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetResetParametersX86(target, out boot, out bootMask1, out reset, out resetMask1, out system, out systemMask1) : PS3TMAPI.GetResetParametersX64(target, out boot, out bootMask1, out reset, out resetMask1, out system, out systemMask1);
            bootParameter = (PS3TMAPI.BootParameter)boot;
            bootMask = (PS3TMAPI.BootParameterMask)bootMask1;
            resetParameter = (PS3TMAPI.ResetParameter)reset;
            resetMask = (PS3TMAPI.ResetParameterMask)resetMask1;
            systemParameter = (PS3TMAPI.SystemParameter)system;
            systemMask = (PS3TMAPI.SystemParameterMask)systemMask1;
            return snresult;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SetBootParameter", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT SetBootParameterX86(int target, ulong boot, ulong bootMask);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SetBootParameter", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT SetBootParameterX64(int target, ulong boot, ulong bootMask);

        public static PS3TMAPI.SNRESULT SetBootParameter(int target, PS3TMAPI.BootParameter bootParameter, PS3TMAPI.BootParameterMask bootMask)
        {
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.SetBootParameterX64(target, (ulong)bootParameter, (ulong)bootMask);
            return PS3TMAPI.SetBootParameterX86(target, (ulong)bootParameter, (ulong)bootMask);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetCurrentBootParameter", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetCurrentBootParameterX86(int target, out ulong boot);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetCurrentBootParameter", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetCurrentBootParameterX64(int target, out ulong boot);

        public static PS3TMAPI.SNRESULT GetCurrentBootParameter(int target, out PS3TMAPI.BootParameter bootParameter)
        {
            ulong boot;
            PS3TMAPI.SNRESULT snresult = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetCurrentBootParameterX86(target, out boot) : PS3TMAPI.GetCurrentBootParameterX64(target, out boot);
            bootParameter = (PS3TMAPI.BootParameter)boot;
            return snresult;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SetSystemParameter", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT SetSystemParameterX86(int target, ulong system, ulong systemMask);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SetSystemParameter", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT SetSystemParameterX64(int target, ulong system, ulong systemMask);

        public static PS3TMAPI.SNRESULT SetSystemParameter(int target, PS3TMAPI.SystemParameter systemParameter, PS3TMAPI.SystemParameterMask systemMask)
        {
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.SetSystemParameterX64(target, (ulong)systemParameter, (ulong)systemMask);
            return PS3TMAPI.SetSystemParameterX86(target, (ulong)systemParameter, (ulong)systemMask);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetTargetInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetTargetInfoX86(ref PS3TMAPI.TargetInfoPriv targetInfoPriv);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetTargetInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetTargetInfoX64(ref PS3TMAPI.TargetInfoPriv targetInfoPriv);

        public static PS3TMAPI.SNRESULT GetTargetInfo(ref PS3TMAPI.TargetInfo targetInfo)
        {
            PS3TMAPI.TargetInfoPriv targetInfoPriv = new PS3TMAPI.TargetInfoPriv();
            targetInfoPriv.Flags = targetInfo.Flags;
            targetInfoPriv.Target = targetInfo.Target;
            PS3TMAPI.SNRESULT res = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetTargetInfoX86(ref targetInfoPriv) : PS3TMAPI.GetTargetInfoX64(ref targetInfoPriv);
            if (PS3TMAPI.FAILED(res))
                return res;
            targetInfo.Flags = targetInfoPriv.Flags;
            targetInfo.Target = targetInfoPriv.Target;
            targetInfo.Name = PS3TMAPI.Utf8ToString(targetInfoPriv.Name, uint.MaxValue);
            targetInfo.Type = PS3TMAPI.Utf8ToString(targetInfoPriv.Type, uint.MaxValue);
            targetInfo.Info = PS3TMAPI.Utf8ToString(targetInfoPriv.Info, uint.MaxValue);
            targetInfo.HomeDir = PS3TMAPI.Utf8ToString(targetInfoPriv.HomeDir, uint.MaxValue);
            targetInfo.FSDir = PS3TMAPI.Utf8ToString(targetInfoPriv.FSDir, uint.MaxValue);
            targetInfo.Boot = targetInfoPriv.Boot;
            return res;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SetTargetInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT SetTargetInfoX86(ref PS3TMAPI.TargetInfoPriv info);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SetTargetInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT SetTargetInfoX64(ref PS3TMAPI.TargetInfoPriv info);

        public static PS3TMAPI.SNRESULT SetTargetInfo(PS3TMAPI.TargetInfo targetInfo)
        {
            PS3TMAPI.TargetInfoPriv info = new PS3TMAPI.TargetInfoPriv();
            info.Flags = targetInfo.Flags;
            info.Target = targetInfo.Target;
            PS3TMAPI.ScopedGlobalHeapPtr scopedGlobalHeapPtr1 = new PS3TMAPI.ScopedGlobalHeapPtr(IntPtr.Zero);
            PS3TMAPI.ScopedGlobalHeapPtr scopedGlobalHeapPtr2 = new PS3TMAPI.ScopedGlobalHeapPtr(IntPtr.Zero);
            PS3TMAPI.ScopedGlobalHeapPtr scopedGlobalHeapPtr3 = new PS3TMAPI.ScopedGlobalHeapPtr(IntPtr.Zero);
            if ((targetInfo.Flags & PS3TMAPI.TargetInfoFlag.Name) > (PS3TMAPI.TargetInfoFlag)0)
            {
                PS3TMAPI.ScopedGlobalHeapPtr scopedGlobalHeapPtr4 = new PS3TMAPI.ScopedGlobalHeapPtr(PS3TMAPI.AllocUtf8FromString(targetInfo.Name));
                info.Name = scopedGlobalHeapPtr4.Get();
            }
            if ((targetInfo.Flags & PS3TMAPI.TargetInfoFlag.HomeDir) > (PS3TMAPI.TargetInfoFlag)0)
            {
                PS3TMAPI.ScopedGlobalHeapPtr scopedGlobalHeapPtr4 = new PS3TMAPI.ScopedGlobalHeapPtr(PS3TMAPI.AllocUtf8FromString(targetInfo.HomeDir));
                info.HomeDir = scopedGlobalHeapPtr4.Get();
            }
            if ((targetInfo.Flags & PS3TMAPI.TargetInfoFlag.FileServingDir) > (PS3TMAPI.TargetInfoFlag)0)
            {
                PS3TMAPI.ScopedGlobalHeapPtr scopedGlobalHeapPtr4 = new PS3TMAPI.ScopedGlobalHeapPtr(PS3TMAPI.AllocUtf8FromString(targetInfo.FSDir));
                info.FSDir = scopedGlobalHeapPtr4.Get();
            }
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.SetTargetInfoX64(ref info);
            return PS3TMAPI.SetTargetInfoX86(ref info);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ListTargetTypes", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT ListTargetTypesX86(ref uint size, IntPtr targetTypes);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ListTargetTypes", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT ListTargetTypesX64(ref uint size, IntPtr targetTypes);

        public static PS3TMAPI.SNRESULT ListTargetTypes(out PS3TMAPI.TargetType[] targetTypes)
        {
            targetTypes = (PS3TMAPI.TargetType[])null;
            uint size = 0;
            PS3TMAPI.SNRESULT res1 = PS3TMAPI.Is32Bit() ? PS3TMAPI.ListTargetTypesX86(ref size, IntPtr.Zero) : PS3TMAPI.ListTargetTypesX64(ref size, IntPtr.Zero);
            if (PS3TMAPI.FAILED(res1))
                return res1;
            PS3TMAPI.ScopedGlobalHeapPtr scopedGlobalHeapPtr = new PS3TMAPI.ScopedGlobalHeapPtr(Marshal.AllocHGlobal((int)((long)Marshal.SizeOf(typeof(PS3TMAPI.TargetType)) * (long)size)));
            PS3TMAPI.SNRESULT res2 = PS3TMAPI.Is32Bit() ? PS3TMAPI.ListTargetTypesX86(ref size, scopedGlobalHeapPtr.Get()) : PS3TMAPI.ListTargetTypesX64(ref size, scopedGlobalHeapPtr.Get());
            if (PS3TMAPI.FAILED(res2))
                return res2;
            targetTypes = new PS3TMAPI.TargetType[(int)size];
            IntPtr utf8Ptr = scopedGlobalHeapPtr.Get();
            for (uint index = 0; index < size; ++index)
            {
                targetTypes[(int)index].Type = PS3TMAPI.Utf8ToString(utf8Ptr, 64U);
                utf8Ptr = new IntPtr(utf8Ptr.ToInt64() + 64L);
                targetTypes[(int)index].Description = PS3TMAPI.Utf8ToString(utf8Ptr, 256U);
                utf8Ptr = new IntPtr(utf8Ptr.ToInt64() + 256L);
            }
            return res2;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3AddTarget", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT AddTargetX86(IntPtr name, IntPtr type, int connParamsSize, IntPtr connectParams, out int target);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3AddTarget", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT AddTargetX64(IntPtr name, IntPtr type, int connParamsSize, IntPtr connectParams, out int target);

        public static PS3TMAPI.SNRESULT AddTarget(string name, string targetType, PS3TMAPI.TCPIPConnectProperties connectProperties, out int target)
        {
            PS3TMAPI.ScopedGlobalHeapPtr scopedGlobalHeapPtr1 = new PS3TMAPI.ScopedGlobalHeapPtr(IntPtr.Zero);
            int num = 0;
            if (connectProperties != null)
            {
                num = Marshal.SizeOf((object)connectProperties);
                scopedGlobalHeapPtr1 = new PS3TMAPI.ScopedGlobalHeapPtr(Marshal.AllocHGlobal(num));
                Marshal.StructureToPtr((object)connectProperties, scopedGlobalHeapPtr1.Get(), false);
            }
            PS3TMAPI.ScopedGlobalHeapPtr scopedGlobalHeapPtr2 = new PS3TMAPI.ScopedGlobalHeapPtr(PS3TMAPI.AllocUtf8FromString(name));
            PS3TMAPI.ScopedGlobalHeapPtr scopedGlobalHeapPtr3 = new PS3TMAPI.ScopedGlobalHeapPtr(PS3TMAPI.AllocUtf8FromString(targetType));
            return PS3TMAPI.Is32Bit() ? PS3TMAPI.AddTargetX86(scopedGlobalHeapPtr2.Get(), scopedGlobalHeapPtr3.Get(), num, scopedGlobalHeapPtr1.Get(), out target) : PS3TMAPI.AddTargetX64(scopedGlobalHeapPtr2.Get(), scopedGlobalHeapPtr3.Get(), num, scopedGlobalHeapPtr1.Get(), out target);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SetDefaultTarget", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT SetDefaultTargetX86(int target);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SetDefaultTarget", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT SetDefaultTargetX64(int target);

        public static PS3TMAPI.SNRESULT SetDefaultTarget(int target)
        {
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.SetDefaultTargetX64(target);
            return PS3TMAPI.SetDefaultTargetX86(target);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetDefaultTarget", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetDefaultTargetX86(out int target);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetDefaultTarget", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetDefaultTargetX64(out int target);

        public static PS3TMAPI.SNRESULT GetDefaultTarget(out int target)
        {
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.GetDefaultTargetX64(out target);
            return PS3TMAPI.GetDefaultTargetX86(out target);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3RegisterServerEventHandler", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT RegisterServerEventHandlerX86(PS3TMAPI.HandleEventCallbackPriv callback, IntPtr userData);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3RegisterServerEventHandler", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT RegisterServerEventHandlerX64(PS3TMAPI.HandleEventCallbackPriv callback, IntPtr userData);

        public static PS3TMAPI.SNRESULT RegisterServerEventHandler(PS3TMAPI.ServerEventCallback callback, ref object userData)
        {
            if (callback == null)
                return PS3TMAPI.SNRESULT.SN_E_BAD_PARAM;
            PS3TMAPI.SNRESULT res = PS3TMAPI.Is32Bit() ? PS3TMAPI.RegisterServerEventHandlerX86(PS3TMAPI.ms_eventHandlerWrapper, IntPtr.Zero) : PS3TMAPI.RegisterServerEventHandlerX64(PS3TMAPI.ms_eventHandlerWrapper, IntPtr.Zero);
            if (PS3TMAPI.SUCCEEDED(res))
            {
                PS3TMAPI.ms_serverEventCallback = callback;
                PS3TMAPI.ms_serverEventUserData = userData;
            }
            return res;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3UnRegisterServerEventHandler", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT UnregisterServerEventHandlerX86();

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3UnRegisterServerEventHandler", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT UnregisterServerEventHandlerX64();

        public static PS3TMAPI.SNRESULT UnregisterServerEventHandler()
        {
            PS3TMAPI.SNRESULT res = PS3TMAPI.Is32Bit() ? PS3TMAPI.UnregisterServerEventHandlerX86() : PS3TMAPI.UnregisterServerEventHandlerX64();
            if (PS3TMAPI.SUCCEEDED(res))
            {
                PS3TMAPI.ms_serverEventCallback = (PS3TMAPI.ServerEventCallback)null;
                PS3TMAPI.ms_serverEventUserData = (object)null;
            }
            return res;
        }

        private static void MarshalServerEvent(int target, uint param, PS3TMAPI.SNRESULT result, uint length, IntPtr data)
        {
            if (PS3TMAPI.ms_serverEventCallback == null)
                return;
            PS3TMAPI.ServerEventHeader storage = new PS3TMAPI.ServerEventHeader();
            PS3TMAPI.ReadDataFromUnmanagedIncPtr<PS3TMAPI.ServerEventHeader>(data, ref storage);
            PS3TMAPI.ms_serverEventCallback(target, result, storage.eventType, PS3TMAPI.ms_serverEventUserData);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetConnectionInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetConnectionInfoX86(int target, IntPtr connectProperties);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetConnectionInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetConnectionInfoX64(int target, IntPtr connectProperties);

        public static PS3TMAPI.SNRESULT GetConnectionInfo(int target, out PS3TMAPI.TCPIPConnectProperties connectProperties)
        {
            connectProperties = (PS3TMAPI.TCPIPConnectProperties)null;
            PS3TMAPI.ScopedGlobalHeapPtr scopedGlobalHeapPtr = new PS3TMAPI.ScopedGlobalHeapPtr(Marshal.AllocHGlobal(Marshal.SizeOf(typeof(PS3TMAPI.TCPIPConnectProperties))));
            PS3TMAPI.SNRESULT res = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetConnectionInfoX86(target, scopedGlobalHeapPtr.Get()) : PS3TMAPI.GetConnectionInfoX64(target, scopedGlobalHeapPtr.Get());
            if (PS3TMAPI.SUCCEEDED(res))
            {
                connectProperties = new PS3TMAPI.TCPIPConnectProperties();
                Marshal.PtrToStructure(scopedGlobalHeapPtr.Get(), (object)connectProperties);
            }
            return res;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SetConnectionInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT SetConnectionInfoX86(int target, IntPtr connectProperties);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SetConnectionInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT SetConnectionInfoX64(int target, IntPtr connectProperties);

        public static PS3TMAPI.SNRESULT SetConnectionInfo(int target, PS3TMAPI.TCPIPConnectProperties connectProperties)
        {
            PS3TMAPI.ScopedGlobalHeapPtr scopedGlobalHeapPtr = new PS3TMAPI.ScopedGlobalHeapPtr(Marshal.AllocHGlobal(Marshal.SizeOf((object)connectProperties)));
            PS3TMAPI.WriteDataToUnmanagedIncPtr<PS3TMAPI.TCPIPConnectProperties>(connectProperties, scopedGlobalHeapPtr.Get());
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.SetConnectionInfoX64(target, scopedGlobalHeapPtr.Get());
            return PS3TMAPI.SetConnectionInfoX86(target, scopedGlobalHeapPtr.Get());
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3DeleteTarget", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT DeleteTargetX86(int target);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3DeleteTarget", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT DeleteTargetX64(int target);

        public static PS3TMAPI.SNRESULT DeleteTarget(int target)
        {
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.DeleteTargetX64(target);
            return PS3TMAPI.DeleteTargetX86(target);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3Connect", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT ConnectX86(int target, string application);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3Connect", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT ConnectX64(int target, string application);

        public static PS3TMAPI.SNRESULT Connect(int target, string application)
        {
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.ConnectX64(target, application);
            return PS3TMAPI.ConnectX86(target, application);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ConnectEx", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT ConnectExX86(int target, string application, bool bForceFlag);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ConnectEx", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT ConnectExX64(int target, string application, bool bForceFlag);

        public static PS3TMAPI.SNRESULT ConnectEx(int target, string application, bool bForceFlag)
        {
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.ConnectExX64(target, application, bForceFlag);
            return PS3TMAPI.ConnectExX86(target, application, bForceFlag);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3Disconnect", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT DisconnectX86(int target);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3Disconnect", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT DisconnectX64(int target);

        public static PS3TMAPI.SNRESULT Disconnect(int target)
        {
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.DisconnectX64(target);
            return PS3TMAPI.DisconnectX86(target);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ForceDisconnect", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT ForceDisconnectX86(int target);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ForceDisconnect", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT ForceDisconnectX64(int target);

        public static PS3TMAPI.SNRESULT ForceDisconnect(int target)
        {
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.ForceDisconnectX64(target);
            return PS3TMAPI.ForceDisconnectX86(target);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetSystemInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetSystemInfoX86(int target, uint reserved, out uint mask, out PS3TMAPI.SystemInfo info);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetSystemInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetSystemInfoX64(int target, uint reserved, out uint mask, out PS3TMAPI.SystemInfo info);

        public static PS3TMAPI.SNRESULT GetSystemInfo(int target, out PS3TMAPI.SystemInfoFlag mask, out PS3TMAPI.SystemInfo systemInfo)
        {
            systemInfo = new PS3TMAPI.SystemInfo();
            uint mask1;
            PS3TMAPI.SNRESULT snresult = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetSystemInfoX86(target, 0U, out mask1, out systemInfo) : PS3TMAPI.GetSystemInfoX64(target, 0U, out mask1, out systemInfo);
            mask = (PS3TMAPI.SystemInfoFlag)mask1;
            return snresult;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetExtraLoadFlags", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetExtraLoadFlagsX86(int target, out ulong extraLoadFlags);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetExtraLoadFlags", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetExtraLoadFlagsX64(int target, out ulong extraLoadFlags);

        public static PS3TMAPI.SNRESULT GetExtraLoadFlags(int target, out PS3TMAPI.ExtraLoadFlag extraLoadFlags)
        {
            ulong extraLoadFlags1 = 0;
            PS3TMAPI.SNRESULT snresult = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetExtraLoadFlagsX86(target, out extraLoadFlags1) : PS3TMAPI.GetExtraLoadFlagsX64(target, out extraLoadFlags1);
            extraLoadFlags = (PS3TMAPI.ExtraLoadFlag)extraLoadFlags1;
            return snresult;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SetExtraLoadFlags", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT SetExtraLoadFlagsX86(int target, ulong extraLoadFlags, ulong mask);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SetExtraLoadFlags", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT SetExtraLoadFlagsX64(int target, ulong extraLoadFlags, ulong mask);

        public static PS3TMAPI.SNRESULT SetExtraLoadFlags(int target, PS3TMAPI.ExtraLoadFlag extraLoadFlags, PS3TMAPI.ExtraLoadFlagMask mask)
        {
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.SetExtraLoadFlagsX64(target, (ulong)extraLoadFlags, (ulong)mask);
            return PS3TMAPI.SetExtraLoadFlagsX86(target, (ulong)extraLoadFlags, (ulong)mask);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetSDKVersion", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetSDKVersionX86(int target, out ulong sdkVersion);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetSDKVersion", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetSDKVersionX64(int target, out ulong sdkVersion);

        public static PS3TMAPI.SNRESULT GetSDKVersion(int target, out ulong sdkVersion)
        {
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.GetSDKVersionX64(target, out sdkVersion);
            return PS3TMAPI.GetSDKVersionX86(target, out sdkVersion);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetCPVersion", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetCPVersionX86(int target, out ulong cpVersion);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetCPVersion", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetCPVersionX64(int target, out ulong cpVersion);

        public static PS3TMAPI.SNRESULT GetCPVersion(int target, out ulong cpVersion)
        {
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.GetCPVersionX64(target, out cpVersion);
            return PS3TMAPI.GetCPVersionX86(target, out cpVersion);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SetTimeouts", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT SetTimeoutsX86(int target, uint numTimeouts, PS3TMAPI.TimeoutType[] timeoutIds, uint[] timeoutValues);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SetTimeouts", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT SetTimeoutsX64(int target, uint numTimeouts, PS3TMAPI.TimeoutType[] timeoutIds, uint[] timeoutValues);

        public static PS3TMAPI.SNRESULT SetTimeouts(int target, PS3TMAPI.TimeoutType[] timeoutTypes, uint[] timeoutValues)
        {
            if (timeoutTypes == null || timeoutTypes.Length < 1 || (timeoutValues == null || timeoutValues.Length != timeoutTypes.Length))
                return PS3TMAPI.SNRESULT.SN_E_BAD_PARAM;
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.SetTimeoutsX64(target, (uint)timeoutTypes.Length, timeoutTypes, timeoutValues);
            return PS3TMAPI.SetTimeoutsX86(target, (uint)timeoutTypes.Length, timeoutTypes, timeoutValues);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetTimeouts", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetTimeoutsX86(int target, out uint numTimeouts, PS3TMAPI.TimeoutType[] timeoutIds, uint[] timeoutValues);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetTimeouts", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetTimeoutsX64(int target, out uint numTimeouts, PS3TMAPI.TimeoutType[] timeoutIds, uint[] timeoutValues);

        public static PS3TMAPI.SNRESULT GetTimeouts(int target, out PS3TMAPI.TimeoutType[] timeoutTypes, out uint[] timeoutValues)
        {
            timeoutTypes = (PS3TMAPI.TimeoutType[])null;
            timeoutValues = (uint[])null;
            uint numTimeouts;
            PS3TMAPI.SNRESULT res = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetTimeoutsX86(target, out numTimeouts, (PS3TMAPI.TimeoutType[])null, (uint[])null) : PS3TMAPI.GetTimeoutsX64(target, out numTimeouts, (PS3TMAPI.TimeoutType[])null, (uint[])null);
            if (PS3TMAPI.FAILED(res))
                return res;
            timeoutTypes = new PS3TMAPI.TimeoutType[(int)numTimeouts];
            timeoutValues = new uint[(int)numTimeouts];
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.GetTimeoutsX64(target, out numTimeouts, timeoutTypes, timeoutValues);
            return PS3TMAPI.GetTimeoutsX86(target, out numTimeouts, timeoutTypes, timeoutValues);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ListTTYStreams", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT ListTtyStreamsX86(int target, ref uint size, IntPtr streamArray);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ListTTYStreams", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT ListTtyStreamsX64(int target, ref uint size, IntPtr streamArray);

        public static PS3TMAPI.SNRESULT ListTTYStreams(int target, out PS3TMAPI.TTYStream[] streamArray)
        {
            streamArray = (PS3TMAPI.TTYStream[])null;
            uint size = 0;
            PS3TMAPI.SNRESULT res1 = PS3TMAPI.Is32Bit() ? PS3TMAPI.ListTtyStreamsX86(target, ref size, IntPtr.Zero) : PS3TMAPI.ListTtyStreamsX64(target, ref size, IntPtr.Zero);
            if (PS3TMAPI.FAILED(res1))
                return res1;
            PS3TMAPI.ScopedGlobalHeapPtr scopedGlobalHeapPtr = new PS3TMAPI.ScopedGlobalHeapPtr(Marshal.AllocHGlobal((int)((long)Marshal.SizeOf(typeof(PS3TMAPI.TTYStream)) * (long)size)));
            PS3TMAPI.SNRESULT res2 = PS3TMAPI.Is32Bit() ? PS3TMAPI.ListTtyStreamsX86(target, ref size, scopedGlobalHeapPtr.Get()) : PS3TMAPI.ListTtyStreamsX64(target, ref size, scopedGlobalHeapPtr.Get());
            if (PS3TMAPI.FAILED(res2))
                return res2;
            IntPtr unmanagedBuf = scopedGlobalHeapPtr.Get();
            streamArray = new PS3TMAPI.TTYStream[(int)size];
            for (uint index = 0; index < size; ++index)
                unmanagedBuf = PS3TMAPI.ReadDataFromUnmanagedIncPtr<PS3TMAPI.TTYStream>(unmanagedBuf, ref streamArray[(int)index]);
            return res2;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3RegisterTTYEventHandler", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT RegisterTtyEventHandlerX86(int target, uint streamIndex, PS3TMAPI.HandleEventCallbackPriv callback, IntPtr userData);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3RegisterTTYEventHandler", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT RegisterTtyEventHandlerX64(int target, uint streamIndex, PS3TMAPI.HandleEventCallbackPriv callback, IntPtr userData);

        public static PS3TMAPI.SNRESULT RegisterTTYEventHandler(int target, uint streamID, PS3TMAPI.TTYCallback callback, ref object userData)
        {
            return PS3TMAPI.RegisterTTYEventHandlerHelper(target, streamID, (object)callback, ref userData);
        }

        public static PS3TMAPI.SNRESULT RegisterTTYEventHandlerRaw(int target, uint streamID, PS3TMAPI.TTYCallbackRaw callback, ref object userData)
        {
            return PS3TMAPI.RegisterTTYEventHandlerHelper(target, streamID, (object)callback, ref userData);
        }

        private static PS3TMAPI.SNRESULT RegisterTTYEventHandlerHelper(int target, uint streamID, object callback, ref object userData)
        {
            if (callback == null)
                return PS3TMAPI.SNRESULT.SN_E_BAD_PARAM;
            PS3TMAPI.SNRESULT res = PS3TMAPI.Is32Bit() ? PS3TMAPI.RegisterTtyEventHandlerX86(target, streamID, PS3TMAPI.ms_eventHandlerWrapper, IntPtr.Zero) : PS3TMAPI.RegisterTtyEventHandlerX64(target, streamID, PS3TMAPI.ms_eventHandlerWrapper, IntPtr.Zero);
            if (PS3TMAPI.FAILED(res))
                return res;
            List<PS3TMAPI.TTYChannel> ttyChannelList = new List<PS3TMAPI.TTYChannel>();
            if ((int)streamID == -1)
            {
                PS3TMAPI.TTYStream[] streamArray = (PS3TMAPI.TTYStream[])null;
                res = PS3TMAPI.ListTTYStreams(target, out streamArray);
                if (PS3TMAPI.FAILED(res) || streamArray == null || streamArray.Length == 0)
                    return res;
                foreach (PS3TMAPI.TTYStream ttyStream in streamArray)
                    ttyChannelList.Add(new PS3TMAPI.TTYChannel(target, ttyStream.Index));
            }
            else
                ttyChannelList.Add(new PS3TMAPI.TTYChannel(target, streamID));
            if (PS3TMAPI.ms_userTtyCallbacks == null)
                PS3TMAPI.ms_userTtyCallbacks = new Dictionary<PS3TMAPI.TTYChannel, PS3TMAPI.TTYCallbackAndUserData>(1);
            foreach (PS3TMAPI.TTYChannel key in ttyChannelList)
            {
                PS3TMAPI.TTYCallbackAndUserData callbackAndUserData;
                if (!PS3TMAPI.ms_userTtyCallbacks.TryGetValue(key, out callbackAndUserData))
                    callbackAndUserData = new PS3TMAPI.TTYCallbackAndUserData();
                if (callback is PS3TMAPI.TTYCallback)
                {
                    callbackAndUserData.m_callback = (PS3TMAPI.TTYCallback)callback;
                    callbackAndUserData.m_userData = userData;
                }
                else
                {
                    callbackAndUserData.m_callbackRaw = (PS3TMAPI.TTYCallbackRaw)callback;
                    callbackAndUserData.m_userDataRaw = userData;
                }
                PS3TMAPI.ms_userTtyCallbacks[key] = callbackAndUserData;
            }
            return res;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3CancelTTYEvents", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT CancelTtyEventsX86(int target, uint index);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3CancelTTYEvents", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT CancelTtyEventsX64(int target, uint index);

        public static PS3TMAPI.SNRESULT CancelTTYEvents(int target, uint streamID)
        {
            PS3TMAPI.SNRESULT res = PS3TMAPI.Is32Bit() ? PS3TMAPI.CancelTtyEventsX86(target, streamID) : PS3TMAPI.CancelTtyEventsX64(target, streamID);
            if (PS3TMAPI.SUCCEEDED(res) && PS3TMAPI.ms_userTtyCallbacks != null)
            {
                if ((int)streamID == -1)
                {
                    List<PS3TMAPI.TTYChannel> ttyChannelList = new List<PS3TMAPI.TTYChannel>();
                    foreach (KeyValuePair<PS3TMAPI.TTYChannel, PS3TMAPI.TTYCallbackAndUserData> msUserTtyCallback in PS3TMAPI.ms_userTtyCallbacks)
                    {
                        if (msUserTtyCallback.Key.Target == target)
                            ttyChannelList.Add(msUserTtyCallback.Key);
                    }
                    foreach (PS3TMAPI.TTYChannel key in ttyChannelList)
                        PS3TMAPI.ms_userTtyCallbacks.Remove(key);
                }
                else
                    PS3TMAPI.ms_userTtyCallbacks.Remove(new PS3TMAPI.TTYChannel(target, streamID));
            }
            return res;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SendTTY", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT SendTTYX86(int target, uint index, string text);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SendTTY", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT SendTTYX64(int target, uint index, string text);

        public static PS3TMAPI.SNRESULT SendTTY(int target, uint streamID, string text)
        {
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.SendTTYX64(target, streamID, text);
            return PS3TMAPI.SendTTYX86(target, streamID, text);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SendTTY", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT SendTTYRawX86(int target, uint index, byte[] text);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SendTTY", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT SendTTYRawX64(int target, uint index, byte[] text);

        public static PS3TMAPI.SNRESULT SendTTYRaw(int target, uint streamID, byte[] text)
        {
            return PS3TMAPI.Is32Bit() ? PS3TMAPI.SendTTYRawX86(target, streamID, text) : PS3TMAPI.SendTTYRawX64(target, streamID, text);
        }

        private static void MarshalTTYEvent(int target, uint param, PS3TMAPI.SNRESULT result, uint length, IntPtr data)
        {
            if (PS3TMAPI.ms_userTtyCallbacks == null)
                return;
            PS3TMAPI.TTYChannel key = new PS3TMAPI.TTYChannel(target, param);
            PS3TMAPI.TTYCallbackAndUserData callbackAndUserData;
            if (!PS3TMAPI.ms_userTtyCallbacks.TryGetValue(key, out callbackAndUserData))
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
        private static extern PS3TMAPI.SNRESULT ClearTTYCacheX86(int target);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ClearTTYCache", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT ClearTTYCacheX64(int target);

        public static PS3TMAPI.SNRESULT ClearTTYCache(int target)
        {
            return PS3TMAPI.Is32Bit() ? PS3TMAPI.ClearTTYCacheX86(target) : PS3TMAPI.ClearTTYCacheX64(target);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3Kick", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT KickX86();

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3Kick", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT KickX64();

        public static PS3TMAPI.SNRESULT Kick()
        {
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.KickX64();
            return PS3TMAPI.KickX86();
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetStatus", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetStatusX86(int target, PS3TMAPI.UnitType unit, out long status, IntPtr reasonCode);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetStatus", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetStatusX64(int target, PS3TMAPI.UnitType unit, out long status, IntPtr reasonCode);

        public static PS3TMAPI.SNRESULT GetStatus(int target, PS3TMAPI.UnitType unit, out PS3TMAPI.UnitStatus unitStatus)
        {
            long status;
            PS3TMAPI.SNRESULT snresult = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetStatusX86(target, unit, out status, IntPtr.Zero) : PS3TMAPI.GetStatusX64(target, unit, out status, IntPtr.Zero);
            unitStatus = (PS3TMAPI.UnitStatus)status;
            return snresult;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ProcessLoad", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT ProcessLoadX86(int target, uint priority, IntPtr fileName, int argCount, string[] args, int envCount, string[] env, out uint processId, out ulong threadId, uint flags);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ProcessLoad", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT ProcessLoadX64(int target, uint priority, IntPtr fileName, int argCount, string[] args, int envCount, string[] env, out uint processId, out ulong threadId, uint flags);

        public static PS3TMAPI.SNRESULT ProcessLoad(int target, uint priority, string fileName, string[] argv, string[] envv, out uint processID, out ulong threadID, PS3TMAPI.LoadFlag loadFlags)
        {
            PS3TMAPI.ScopedGlobalHeapPtr scopedGlobalHeapPtr = new PS3TMAPI.ScopedGlobalHeapPtr(PS3TMAPI.AllocUtf8FromString(fileName));
            int argCount = 0;
            if (argv != null)
                argCount = argv.Length;
            int envCount = 0;
            if (envv != null)
                envCount = envv.Length;
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.ProcessLoadX64(target, priority, scopedGlobalHeapPtr.Get(), argCount, argv, envCount, envv, out processID, out threadID, (uint)loadFlags);
            return PS3TMAPI.ProcessLoadX86(target, priority, scopedGlobalHeapPtr.Get(), argCount, argv, envCount, envv, out processID, out threadID, (uint)loadFlags);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ProcessList", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetProcessListX86(int target, ref uint count, IntPtr processIdArray);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ProcessList", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetProcessListX64(int target, ref uint count, IntPtr processIdArray);

        public static PS3TMAPI.SNRESULT GetProcessList(int target, out uint[] processIDs)
        {
            processIDs = (uint[])null;
            uint count = 0;
            PS3TMAPI.SNRESULT res1 = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetProcessListX86(target, ref count, IntPtr.Zero) : PS3TMAPI.GetProcessListX64(target, ref count, IntPtr.Zero);
            if (PS3TMAPI.FAILED(res1))
                return res1;
            PS3TMAPI.ScopedGlobalHeapPtr scopedGlobalHeapPtr = new PS3TMAPI.ScopedGlobalHeapPtr(Marshal.AllocHGlobal(4 * (int)count));
            PS3TMAPI.SNRESULT res2 = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetProcessListX86(target, ref count, scopedGlobalHeapPtr.Get()) : PS3TMAPI.GetProcessListX64(target, ref count, scopedGlobalHeapPtr.Get());
            if (PS3TMAPI.FAILED(res2))
                return res2;
            IntPtr unmanagedBuf = scopedGlobalHeapPtr.Get();
            processIDs = new uint[(int)count];
            for (uint index = 0; index < count; ++index)
                unmanagedBuf = PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(unmanagedBuf, ref processIDs[(int)index]);
            return res2;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3UserProcessList", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetUserProcessListX86(int target, ref uint count, IntPtr processIdArray);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3UserProcessList", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetUserProcessListX64(int target, ref uint count, IntPtr processIdArray);

        public static PS3TMAPI.SNRESULT GetUserProcessList(int target, out uint[] processIDs)
        {
            uint count = 0;
            processIDs = (uint[])null;
            PS3TMAPI.SNRESULT res1 = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetUserProcessListX86(target, ref count, IntPtr.Zero) : PS3TMAPI.GetUserProcessListX64(target, ref count, IntPtr.Zero);
            if (PS3TMAPI.FAILED(res1))
                return res1;
            PS3TMAPI.ScopedGlobalHeapPtr scopedGlobalHeapPtr = new PS3TMAPI.ScopedGlobalHeapPtr(Marshal.AllocHGlobal(4 * (int)count));
            PS3TMAPI.SNRESULT res2 = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetUserProcessListX86(target, ref count, scopedGlobalHeapPtr.Get()) : PS3TMAPI.GetUserProcessListX64(target, ref count, scopedGlobalHeapPtr.Get());
            if (PS3TMAPI.FAILED(res2))
                return res2;
            IntPtr unmanagedBuf = scopedGlobalHeapPtr.Get();
            processIDs = new uint[(int)count];
            for (uint index = 0; index < count; ++index)
                unmanagedBuf = PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(unmanagedBuf, ref processIDs[(int)index]);
            return res2;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ProcessStop", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT ProcessStopX86(int target, uint processId);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ProcessStop", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT ProcessStopX64(int target, uint processId);

        public static PS3TMAPI.SNRESULT ProcessStop(int target, uint processID)
        {
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.ProcessStopX64(target, processID);
            return PS3TMAPI.ProcessStopX86(target, processID);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ProcessContinue", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT ProcessContinueX86(int target, uint processId);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ProcessContinue", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT ProcessContinueX64(int target, uint processId);

        public static PS3TMAPI.SNRESULT ProcessContinue(int target, uint processID)
        {
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.ProcessContinueX64(target, processID);
            return PS3TMAPI.ProcessContinueX86(target, processID);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ProcessKill", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT ProcessKillX86(int target, uint processId);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ProcessKill", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT ProcessKillX64(int target, uint processId);

        public static PS3TMAPI.SNRESULT ProcessKill(int target, uint processID)
        {
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.ProcessKillX64(target, processID);
            return PS3TMAPI.ProcessKillX86(target, processID);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3TerminateGameProcess", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT TerminateGameProcessX86(int target, uint processId, uint timeout);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3TerminateGameProcess", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT TerminateGameProcessX64(int target, uint processId, uint timeout);

        public static PS3TMAPI.SNRESULT TerminateGameProcess(int target, uint processID, uint timeout)
        {
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.TerminateGameProcessX64(target, processID, timeout);
            return PS3TMAPI.TerminateGameProcessX86(target, processID, timeout);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ThreadList", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetThreadListX86(int target, uint processId, ref uint numPPUThreads, ulong[] ppuThreadIds, ref uint numSPUThreadGroups, ulong[] spuThreadGroupIds);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ThreadList", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetThreadListX64(int target, uint processId, ref uint numPPUThreads, ulong[] ppuThreadIds, ref uint numSPUThreadGroups, ulong[] spuThreadGroupIds);

        public static PS3TMAPI.SNRESULT GetThreadList(int target, uint processID, out ulong[] ppuThreadIDs, out ulong[] spuThreadGroupIDs)
        {
            ppuThreadIDs = (ulong[])null;
            spuThreadGroupIDs = (ulong[])null;
            uint numPPUThreads = 0;
            uint numSPUThreadGroups = 0;
            PS3TMAPI.SNRESULT res = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetThreadListX86(target, processID, ref numPPUThreads, (ulong[])null, ref numSPUThreadGroups, (ulong[])null) : PS3TMAPI.GetThreadListX64(target, processID, ref numPPUThreads, (ulong[])null, ref numSPUThreadGroups, (ulong[])null);
            if (PS3TMAPI.FAILED(res))
                return res;
            ppuThreadIDs = new ulong[(int)numPPUThreads];
            spuThreadGroupIDs = new ulong[(int)numSPUThreadGroups];
            return PS3TMAPI.Is32Bit() ? PS3TMAPI.GetThreadListX86(target, processID, ref numPPUThreads, ppuThreadIDs, ref numSPUThreadGroups, spuThreadGroupIDs) : PS3TMAPI.GetThreadListX64(target, processID, ref numPPUThreads, ppuThreadIDs, ref numSPUThreadGroups, spuThreadGroupIDs);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ThreadStop", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT ThreadStopX86(int target, PS3TMAPI.UnitType unit, uint processId, ulong threadId);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ThreadStop", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT ThreadStopX64(int target, PS3TMAPI.UnitType unit, uint processId, ulong threadId);

        public static PS3TMAPI.SNRESULT ThreadStop(int target, PS3TMAPI.UnitType unit, uint processID, ulong threadID)
        {
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.ThreadStopX64(target, unit, processID, threadID);
            return PS3TMAPI.ThreadStopX86(target, unit, processID, threadID);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ThreadContinue", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT ThreadContinueX86(int target, PS3TMAPI.UnitType unit, uint processId, ulong threadId);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ThreadContinue", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT ThreadContinueX64(int target, PS3TMAPI.UnitType unit, uint processId, ulong threadId);

        public static PS3TMAPI.SNRESULT ThreadContinue(int target, PS3TMAPI.UnitType unit, uint processID, ulong threadID)
        {
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.ThreadContinueX64(target, unit, processID, threadID);
            return PS3TMAPI.ThreadContinueX86(target, unit, processID, threadID);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ThreadGetRegisters", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT ThreadGetRegistersX86(int target, PS3TMAPI.UnitType unit, uint processId, ulong threadId, uint numRegisters, uint[] registerNums, ulong[] registerValues);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ThreadGetRegisters", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT ThreadGetRegistersX64(int target, PS3TMAPI.UnitType unit, uint processId, ulong threadId, uint numRegisters, uint[] registerNums, ulong[] registerValues);

        public static PS3TMAPI.SNRESULT ThreadGetRegisters(int target, PS3TMAPI.UnitType unit, uint processID, ulong threadID, uint[] registerNums, out ulong[] registerValues)
        {
            registerValues = (ulong[])null;
            if (registerNums == null)
                return PS3TMAPI.SNRESULT.SN_E_BAD_PARAM;
            registerValues = new ulong[registerNums.Length];
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.ThreadGetRegistersX64(target, unit, processID, threadID, (uint)registerNums.Length, registerNums, registerValues);
            return PS3TMAPI.ThreadGetRegistersX86(target, unit, processID, threadID, (uint)registerNums.Length, registerNums, registerValues);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ThreadSetRegisters", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT ThreadSetRegistersX86(int target, PS3TMAPI.UnitType unit, uint processId, ulong threadId, uint numRegisters, uint[] registerNums, ulong[] registerValues);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ThreadSetRegisters", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT ThreadSetRegistersX64(int target, PS3TMAPI.UnitType unit, uint processId, ulong threadId, uint numRegisters, uint[] registerNums, ulong[] registerValues);

        public static PS3TMAPI.SNRESULT ThreadSetRegisters(int target, PS3TMAPI.UnitType unit, uint processID, ulong threadID, uint[] registerNums, ulong[] registerValues)
        {
            if (registerNums == null || registerValues == null || registerNums.Length != registerValues.Length)
                return PS3TMAPI.SNRESULT.SN_E_BAD_PARAM;
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.ThreadSetRegistersX64(target, unit, processID, threadID, (uint)registerNums.Length, registerNums, registerValues);
            return PS3TMAPI.ThreadSetRegistersX86(target, unit, processID, threadID, (uint)registerNums.Length, registerNums, registerValues);
        }

        private static void ProcessInfoMarshalHelper(IntPtr unmanagedBuf, ref PS3TMAPI.ProcessInfo processInfo)
        {
            uint storage = 0;
            unmanagedBuf = PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(unmanagedBuf, ref storage);
            processInfo.Hdr.Status = (PS3TMAPI.ProcessStatus)storage;
            unmanagedBuf = PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(unmanagedBuf, ref processInfo.Hdr.NumPPUThreads);
            unmanagedBuf = PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(unmanagedBuf, ref processInfo.Hdr.NumSPUThreads);
            unmanagedBuf = PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(unmanagedBuf, ref processInfo.Hdr.ParentProcessID);
            unmanagedBuf = PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(unmanagedBuf, ref processInfo.Hdr.MaxMemorySize);
            processInfo.Hdr.ELFPath = PS3TMAPI.Utf8ToString(unmanagedBuf, 512U);
            unmanagedBuf = new IntPtr(unmanagedBuf.ToInt64() + 512L);
            uint num = processInfo.Hdr.NumPPUThreads + processInfo.Hdr.NumSPUThreads;
            processInfo.ThreadIDs = new ulong[(int)num];
            for (int index = 0; (long)index < (long)num; ++index)
                unmanagedBuf = PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(unmanagedBuf, ref processInfo.ThreadIDs[index]);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ProcessInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetProcessInfoX86(int target, uint processId, ref uint bufferSize, IntPtr processInfo);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ProcessInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetProcessInfoX64(int target, uint processId, ref uint bufferSize, IntPtr processInfo);

        public static PS3TMAPI.SNRESULT GetProcessInfo(int target, uint processID, out PS3TMAPI.ProcessInfo processInfo)
        {
            processInfo = new PS3TMAPI.ProcessInfo();
            uint bufferSize = 0;
            PS3TMAPI.SNRESULT res1 = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetProcessInfoX86(target, processID, ref bufferSize, IntPtr.Zero) : PS3TMAPI.GetProcessInfoX64(target, processID, ref bufferSize, IntPtr.Zero);
            if (PS3TMAPI.FAILED(res1))
                return res1;
            PS3TMAPI.ScopedGlobalHeapPtr scopedGlobalHeapPtr = new PS3TMAPI.ScopedGlobalHeapPtr(Marshal.AllocHGlobal((int)bufferSize));
            PS3TMAPI.SNRESULT res2 = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetProcessInfoX86(target, processID, ref bufferSize, scopedGlobalHeapPtr.Get()) : PS3TMAPI.GetProcessInfoX64(target, processID, ref bufferSize, scopedGlobalHeapPtr.Get());
            if (PS3TMAPI.SUCCEEDED(res2))
                PS3TMAPI.ProcessInfoMarshalHelper(scopedGlobalHeapPtr.Get(), ref processInfo);
            return res2;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ProcessInfoEx", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetProcessInfoExX86(int target, uint processId, ref uint bufferSize, IntPtr processInfo, out PS3TMAPI.ExtraProcessInfo extraProcessInfo);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ProcessInfoEx", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetProcessInfoExX64(int target, uint processId, ref uint bufferSize, IntPtr processInfo, out PS3TMAPI.ExtraProcessInfo extraProcessInfo);

        public static PS3TMAPI.SNRESULT GetProcessInfoEx(int target, uint processID, out PS3TMAPI.ProcessInfo processInfo, out PS3TMAPI.ExtraProcessInfo extraProcessInfo)
        {
            processInfo = new PS3TMAPI.ProcessInfo();
            extraProcessInfo = new PS3TMAPI.ExtraProcessInfo();
            uint bufferSize = 0;
            PS3TMAPI.SNRESULT res1 = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetProcessInfoExX86(target, processID, ref bufferSize, IntPtr.Zero, out extraProcessInfo) : PS3TMAPI.GetProcessInfoExX64(target, processID, ref bufferSize, IntPtr.Zero, out extraProcessInfo);
            if (PS3TMAPI.FAILED(res1))
                return res1;
            PS3TMAPI.ScopedGlobalHeapPtr scopedGlobalHeapPtr = new PS3TMAPI.ScopedGlobalHeapPtr(Marshal.AllocHGlobal((int)bufferSize));
            PS3TMAPI.SNRESULT res2 = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetProcessInfoExX86(target, processID, ref bufferSize, scopedGlobalHeapPtr.Get(), out extraProcessInfo) : PS3TMAPI.GetProcessInfoExX64(target, processID, ref bufferSize, scopedGlobalHeapPtr.Get(), out extraProcessInfo);
            if (PS3TMAPI.SUCCEEDED(res2))
                PS3TMAPI.ProcessInfoMarshalHelper(scopedGlobalHeapPtr.Get(), ref processInfo);
            return res2;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ProcessInfoEx2", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetProcessInfoEx2X86(int target, uint processId, ref uint bufferSize, IntPtr processInfo, out PS3TMAPI.ExtraProcessInfo extraProcessInfo, out PS3TMAPI.ProcessLoadInfo processLoadInfo);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ProcessInfoEx2", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetProcessInfoEx2X64(int target, uint processId, ref uint bufferSize, IntPtr processInfo, out PS3TMAPI.ExtraProcessInfo extraProcessInfo, out PS3TMAPI.ProcessLoadInfo processLoadInfo);

        public static PS3TMAPI.SNRESULT GetProcessInfoEx2(int target, uint processID, out PS3TMAPI.ProcessInfo processInfo, out PS3TMAPI.ExtraProcessInfo extraProcessInfo, out PS3TMAPI.ProcessLoadInfo processLoadInfo)
        {
            uint bufferSize = 0;
            processInfo = new PS3TMAPI.ProcessInfo();
            extraProcessInfo = new PS3TMAPI.ExtraProcessInfo();
            processLoadInfo = new PS3TMAPI.ProcessLoadInfo();
            PS3TMAPI.SNRESULT res1 = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetProcessInfoEx2X86(target, processID, ref bufferSize, IntPtr.Zero, out extraProcessInfo, out processLoadInfo) : PS3TMAPI.GetProcessInfoEx2X64(target, processID, ref bufferSize, IntPtr.Zero, out extraProcessInfo, out processLoadInfo);
            if (PS3TMAPI.FAILED(res1))
                return res1;
            PS3TMAPI.ScopedGlobalHeapPtr scopedGlobalHeapPtr = new PS3TMAPI.ScopedGlobalHeapPtr(Marshal.AllocHGlobal((int)bufferSize));
            PS3TMAPI.SNRESULT res2 = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetProcessInfoEx2X86(target, processID, ref bufferSize, scopedGlobalHeapPtr.Get(), out extraProcessInfo, out processLoadInfo) : PS3TMAPI.GetProcessInfoEx2X64(target, processID, ref bufferSize, scopedGlobalHeapPtr.Get(), out extraProcessInfo, out processLoadInfo);
            if (PS3TMAPI.SUCCEEDED(res2))
                PS3TMAPI.ProcessInfoMarshalHelper(scopedGlobalHeapPtr.Get(), ref processInfo);
            return res2;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetModuleList", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetModuleListX86(int target, uint processId, ref uint numModules, uint[] moduleList);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetModuleList", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetModuleListX64(int target, uint processId, ref uint numModules, uint[] moduleList);

        public static PS3TMAPI.SNRESULT GetModuleList(int target, uint processID, out uint[] modules)
        {
            modules = (uint[])null;
            uint numModules = 0;
            PS3TMAPI.SNRESULT res = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetModuleListX86(target, processID, ref numModules, (uint[])null) : PS3TMAPI.GetModuleListX64(target, processID, ref numModules, (uint[])null);
            if (PS3TMAPI.FAILED(res))
                return res;
            modules = new uint[(int)numModules];
            return PS3TMAPI.Is32Bit() ? PS3TMAPI.GetModuleListX86(target, processID, ref numModules, modules) : PS3TMAPI.GetModuleListX64(target, processID, ref numModules, modules);
        }

        private static IntPtr ModuleInfoHdrMarshalHelper(IntPtr unmanagedBuf, ref PS3TMAPI.ModuleInfoHdr moduleInfoHdr)
        {
            PS3TMAPI.ModuleInfoHdrPriv storage = new PS3TMAPI.ModuleInfoHdrPriv();
            unmanagedBuf = PS3TMAPI.ReadDataFromUnmanagedIncPtr<PS3TMAPI.ModuleInfoHdrPriv>(unmanagedBuf, ref storage);
            moduleInfoHdr.Name = PS3TMAPI.Utf8FixedSizeByteArrayToString(storage.Name);
            moduleInfoHdr.Version = storage.Version;
            moduleInfoHdr.Attribute = storage.Attribute;
            moduleInfoHdr.StartEntry = storage.StartEntry;
            moduleInfoHdr.StopEntry = storage.StopEntry;
            moduleInfoHdr.ELFName = PS3TMAPI.Utf8FixedSizeByteArrayToString(storage.ELFName);
            moduleInfoHdr.NumSegments = storage.NumSegments;
            return unmanagedBuf;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetModuleInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetModuleInfoX86(int target, uint processId, uint moduleId, ref ulong bufferSize, IntPtr moduleInfo);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetModuleInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetModuleInfoX64(int target, uint processId, uint moduleId, ref ulong bufferSize, IntPtr moduleInfo);

        public static PS3TMAPI.SNRESULT GetModuleInfo(int target, uint processID, uint moduleID, out PS3TMAPI.ModuleInfo moduleInfo)
        {
            moduleInfo = new PS3TMAPI.ModuleInfo();
            ulong bufferSize = 0;
            PS3TMAPI.SNRESULT res1 = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetModuleInfoX86(target, processID, moduleID, ref bufferSize, IntPtr.Zero) : PS3TMAPI.GetModuleInfoX64(target, processID, moduleID, ref bufferSize, IntPtr.Zero);
            if (PS3TMAPI.FAILED(res1))
                return res1;
            if (bufferSize > (ulong)int.MaxValue)
                return PS3TMAPI.SNRESULT.SN_E_ERROR;
            PS3TMAPI.ScopedGlobalHeapPtr scopedGlobalHeapPtr = new PS3TMAPI.ScopedGlobalHeapPtr(Marshal.AllocHGlobal((int)bufferSize));
            PS3TMAPI.SNRESULT res2 = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetModuleInfoX86(target, processID, moduleID, ref bufferSize, scopedGlobalHeapPtr.Get()) : PS3TMAPI.GetModuleInfoX64(target, processID, moduleID, ref bufferSize, scopedGlobalHeapPtr.Get());
            if (PS3TMAPI.FAILED(res2))
                return res2;
            IntPtr unmanagedBuf = PS3TMAPI.ModuleInfoHdrMarshalHelper(scopedGlobalHeapPtr.Get(), ref moduleInfo.Hdr);
            moduleInfo.Segments = new PS3TMAPI.PRXSegment[(int)moduleInfo.Hdr.NumSegments];
            for (int index = 0; (long)index < (long)moduleInfo.Hdr.NumSegments; ++index)
                unmanagedBuf = PS3TMAPI.ReadDataFromUnmanagedIncPtr<PS3TMAPI.PRXSegment>(unmanagedBuf, ref moduleInfo.Segments[index]);
            return res2;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetModuleInfoEx", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetModuleInfoExX86(int target, uint processId, uint moduleId, ref ulong bufferSize, IntPtr moduleInfoEx, out IntPtr mselfInfo, out PS3TMAPI.ExtraModuleInfo extraModuleInfo);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetModuleInfoEx", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetModuleInfoExX64(int target, uint processId, uint moduleId, ref ulong bufferSize, IntPtr moduleInfoEx, out IntPtr mselfInfo, out PS3TMAPI.ExtraModuleInfo extraModuleInfo);

        public static PS3TMAPI.SNRESULT GetModuleInfoEx(int target, uint processID, uint moduleID, out PS3TMAPI.ModuleInfoEx moduleInfoEx, out PS3TMAPI.MSELFInfo mselfInfo, out PS3TMAPI.ExtraModuleInfo extraModuleInfo)
        {
            moduleInfoEx = new PS3TMAPI.ModuleInfoEx();
            mselfInfo = new PS3TMAPI.MSELFInfo();
            extraModuleInfo = new PS3TMAPI.ExtraModuleInfo();
            ulong bufferSize = 0;
            IntPtr mselfInfo1 = IntPtr.Zero;
            PS3TMAPI.SNRESULT res1 = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetModuleInfoExX86(target, processID, moduleID, ref bufferSize, IntPtr.Zero, out mselfInfo1, out extraModuleInfo) : PS3TMAPI.GetModuleInfoExX64(target, processID, moduleID, ref bufferSize, IntPtr.Zero, out mselfInfo1, out extraModuleInfo);
            if (PS3TMAPI.FAILED(res1))
                return res1;
            if (bufferSize > (ulong)int.MaxValue)
                return PS3TMAPI.SNRESULT.SN_E_ERROR;
            PS3TMAPI.ScopedGlobalHeapPtr scopedGlobalHeapPtr = new PS3TMAPI.ScopedGlobalHeapPtr(Marshal.AllocHGlobal((int)bufferSize));
            PS3TMAPI.SNRESULT res2 = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetModuleInfoExX86(target, processID, moduleID, ref bufferSize, scopedGlobalHeapPtr.Get(), out mselfInfo1, out extraModuleInfo) : PS3TMAPI.GetModuleInfoExX64(target, processID, moduleID, ref bufferSize, scopedGlobalHeapPtr.Get(), out mselfInfo1, out extraModuleInfo);
            if (PS3TMAPI.FAILED(res2))
                return res2;
            IntPtr unmanagedBuf = PS3TMAPI.ModuleInfoHdrMarshalHelper(scopedGlobalHeapPtr.Get(), ref moduleInfoEx.Hdr);
            moduleInfoEx.Segments = new PS3TMAPI.PRXSegmentEx[(int)moduleInfoEx.Hdr.NumSegments];
            for (int index = 0; (long)index < (long)moduleInfoEx.Hdr.NumSegments; ++index)
                unmanagedBuf = PS3TMAPI.ReadDataFromUnmanagedIncPtr<PS3TMAPI.PRXSegmentEx>(unmanagedBuf, ref moduleInfoEx.Segments[index]);
            PS3TMAPI.ReadDataFromUnmanagedIncPtr<PS3TMAPI.MSELFInfo>(mselfInfo1, ref mselfInfo);
            return res2;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ThreadInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetThreadInfoX86(int target, PS3TMAPI.UnitType unit, uint processId, ulong threadId, ref uint bufferSize, IntPtr buffer);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ThreadInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetThreadInfoX64(int target, PS3TMAPI.UnitType unit, uint processId, ulong threadId, ref uint bufferSize, IntPtr buffer);

        public static PS3TMAPI.SNRESULT GetPPUThreadInfo(int target, uint processID, ulong threadID, out PS3TMAPI.PPUThreadInfo threadInfo)
        {
            threadInfo = new PS3TMAPI.PPUThreadInfo();
            uint bufferSize = 0;
            PS3TMAPI.SNRESULT res1 = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetThreadInfoX86(target, PS3TMAPI.UnitType.PPU, processID, threadID, ref bufferSize, IntPtr.Zero) : PS3TMAPI.GetThreadInfoX64(target, PS3TMAPI.UnitType.PPU, processID, threadID, ref bufferSize, IntPtr.Zero);
            if (PS3TMAPI.FAILED(res1))
                return res1;
            PS3TMAPI.ScopedGlobalHeapPtr scopedGlobalHeapPtr = new PS3TMAPI.ScopedGlobalHeapPtr(Marshal.AllocHGlobal((int)bufferSize));
            PS3TMAPI.SNRESULT res2 = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetThreadInfoX86(target, PS3TMAPI.UnitType.PPU, processID, threadID, ref bufferSize, scopedGlobalHeapPtr.Get()) : PS3TMAPI.GetThreadInfoX64(target, PS3TMAPI.UnitType.PPU, processID, threadID, ref bufferSize, scopedGlobalHeapPtr.Get());
            if (PS3TMAPI.FAILED(res2))
                return res2;
            PS3TMAPI.PPUThreadInfoPriv storage = new PS3TMAPI.PPUThreadInfoPriv();
            IntPtr utf8Ptr = PS3TMAPI.ReadDataFromUnmanagedIncPtr<PS3TMAPI.PPUThreadInfoPriv>(scopedGlobalHeapPtr.Get(), ref storage);
            threadInfo.ThreadID = storage.ThreadID;
            threadInfo.Priority = storage.Priority;
            threadInfo.State = (PS3TMAPI.PPUThreadState)storage.State;
            threadInfo.StackAddress = storage.StackAddress;
            threadInfo.StackSize = storage.StackSize;
            if (storage.ThreadNameLen > 0U)
                threadInfo.ThreadName = PS3TMAPI.Utf8ToString(utf8Ptr, uint.MaxValue);
            return res2;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3PPUThreadInfoEx", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetPPUThreadInfoExX86(int target, uint processId, ulong threadId, ref uint bufferSize, IntPtr buffer);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3PPUThreadInfoEx", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetPPUThreadInfoExX64(int target, uint processId, ulong threadId, ref uint bufferSize, IntPtr buffer);

        public static PS3TMAPI.SNRESULT GetPPUThreadInfoEx(int target, uint processID, ulong threadID, out PS3TMAPI.PPUThreadInfoEx threadInfoEx)
        {
            threadInfoEx = new PS3TMAPI.PPUThreadInfoEx();
            uint bufferSize = 0;
            PS3TMAPI.SNRESULT res1 = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetPPUThreadInfoExX86(target, processID, threadID, ref bufferSize, IntPtr.Zero) : PS3TMAPI.GetPPUThreadInfoExX64(target, processID, threadID, ref bufferSize, IntPtr.Zero);
            if (PS3TMAPI.FAILED(res1))
                return res1;
            PS3TMAPI.ScopedGlobalHeapPtr scopedGlobalHeapPtr = new PS3TMAPI.ScopedGlobalHeapPtr(Marshal.AllocHGlobal((int)bufferSize));
            PS3TMAPI.SNRESULT res2 = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetPPUThreadInfoExX86(target, processID, threadID, ref bufferSize, scopedGlobalHeapPtr.Get()) : PS3TMAPI.GetPPUThreadInfoExX64(target, processID, threadID, ref bufferSize, scopedGlobalHeapPtr.Get());
            if (PS3TMAPI.FAILED(res2))
                return res2;
            PS3TMAPI.PPUThreadInfoExPriv storage = new PS3TMAPI.PPUThreadInfoExPriv();
            IntPtr utf8Ptr = PS3TMAPI.ReadDataFromUnmanagedIncPtr<PS3TMAPI.PPUThreadInfoExPriv>(scopedGlobalHeapPtr.Get(), ref storage);
            threadInfoEx.ThreadID = storage.ThreadId;
            threadInfoEx.Priority = storage.Priority;
            threadInfoEx.BasePriority = storage.BasePriority;
            threadInfoEx.State = (PS3TMAPI.PPUThreadState)storage.State;
            threadInfoEx.StackAddress = storage.StackAddress;
            threadInfoEx.StackSize = storage.StackSize;
            if (storage.ThreadNameLen > 0U)
                threadInfoEx.ThreadName = PS3TMAPI.Utf8ToString(utf8Ptr, uint.MaxValue);
            return res2;
        }

        public static PS3TMAPI.SNRESULT GetSPUThreadInfo(int target, uint processID, ulong threadID, out PS3TMAPI.SPUThreadInfo threadInfo)
        {
            threadInfo = new PS3TMAPI.SPUThreadInfo();
            uint bufferSize = 0;
            PS3TMAPI.SNRESULT res1 = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetThreadInfoX86(target, PS3TMAPI.UnitType.SPU, processID, threadID, ref bufferSize, IntPtr.Zero) : PS3TMAPI.GetThreadInfoX64(target, PS3TMAPI.UnitType.SPU, processID, threadID, ref bufferSize, IntPtr.Zero);
            if (PS3TMAPI.FAILED(res1))
                return res1;
            PS3TMAPI.ScopedGlobalHeapPtr scopedGlobalHeapPtr = new PS3TMAPI.ScopedGlobalHeapPtr(Marshal.AllocHGlobal((int)bufferSize));
            PS3TMAPI.SNRESULT res2 = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetThreadInfoX86(target, PS3TMAPI.UnitType.SPU, processID, threadID, ref bufferSize, scopedGlobalHeapPtr.Get()) : PS3TMAPI.GetThreadInfoX64(target, PS3TMAPI.UnitType.SPU, processID, threadID, ref bufferSize, scopedGlobalHeapPtr.Get());
            if (PS3TMAPI.FAILED(res2))
                return res2;
            PS3TMAPI.SpuThreadInfoPriv storage = new PS3TMAPI.SpuThreadInfoPriv();
            IntPtr utf8Ptr1 = PS3TMAPI.ReadDataFromUnmanagedIncPtr<PS3TMAPI.SpuThreadInfoPriv>(scopedGlobalHeapPtr.Get(), ref storage);
            threadInfo.ThreadGroupID = storage.ThreadGroupId;
            threadInfo.ThreadID = storage.ThreadId;
            if (storage.FilenameLen > 0U)
                threadInfo.Filename = PS3TMAPI.Utf8ToString(utf8Ptr1, uint.MaxValue);
            if (storage.ThreadNameLen > 0U)
            {
                IntPtr utf8Ptr2 = new IntPtr(utf8Ptr1.ToInt64() + (long)storage.FilenameLen);
                threadInfo.ThreadName = PS3TMAPI.Utf8ToString(utf8Ptr2, uint.MaxValue);
            }
            return res2;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SetDefaultPPUThreadStackSize", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT SetDefaultPPUThreadStackSizeX86(int target, PS3TMAPI.ELFStackSize size);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SetDefaultPPUThreadStackSize", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT SetDefaultPPUThreadStackSizeX64(int target, PS3TMAPI.ELFStackSize size);

        public static PS3TMAPI.SNRESULT SetDefaultPPUThreadStackSize(int target, PS3TMAPI.ELFStackSize stackSize)
        {
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.SetDefaultPPUThreadStackSizeX64(target, stackSize);
            return PS3TMAPI.SetDefaultPPUThreadStackSizeX86(target, stackSize);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetDefaultPPUThreadStackSize", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetDefaultPPUThreadStackSizeX86(int target, out PS3TMAPI.ELFStackSize size);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetDefaultPPUThreadStackSize", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetDefaultPPUThreadStackSizeX64(int target, out PS3TMAPI.ELFStackSize size);

        public static PS3TMAPI.SNRESULT GetDefaultPPUThreadStackSize(int target, out PS3TMAPI.ELFStackSize stackSize)
        {
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.GetDefaultPPUThreadStackSizeX64(target, out stackSize);
            return PS3TMAPI.GetDefaultPPUThreadStackSizeX86(target, out stackSize);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SetSPULoopPoint", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT SetSPULoopPointX86(int target, uint processId, ulong threadId, uint address, int bCurrentPc);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SetSPULoopPoint", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT SetSPULoopPointX64(int target, uint processId, ulong threadId, uint address, int bCurrentPc);
        
        public static PS3TMAPI.SNRESULT SetSPULoopPoint(int target, uint processID, ulong threadID, uint address, bool bCurrentPC)
        {
            int bCurrentPc = bCurrentPC ? 1 : 0;
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.SetSPULoopPointX64(target, processID, threadID, address, bCurrentPc);
            return PS3TMAPI.SetSPULoopPointX86(target, processID, threadID, address, bCurrentPc);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ClearSPULoopPoint", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT ClearSPULoopPointX86(int target, uint processId, ulong threadId, uint address, bool bCurrentPc);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ClearSPULoopPoint", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT ClearSPULoopPointX64(int target, uint processId, ulong threadId, uint address, bool bCurrentPc);

        public static PS3TMAPI.SNRESULT ClearSPULoopPoint(int target, uint processID, ulong threadID, uint address, bool bCurrentPC)
        {
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.ClearSPULoopPointX64(target, processID, threadID, address, bCurrentPC);
            return PS3TMAPI.ClearSPULoopPointX86(target, processID, threadID, address, bCurrentPC);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SetBreakPoint", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT SetBreakPointX86(int target, uint unit, uint processId, ulong threadId, ulong address);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SetBreakPoint", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT SetBreakPointX64(int target, uint unit, uint processId, ulong threadId, ulong address);

        public static PS3TMAPI.SNRESULT SetBreakPoint(int target, PS3TMAPI.UnitType unit, uint processID, ulong threadID, ulong address)
        {
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.SetBreakPointX64(target, (uint)unit, processID, threadID, address);
            return PS3TMAPI.SetBreakPointX86(target, (uint)unit, processID, threadID, address);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ClearBreakPoint", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT ClearBreakPointX86(int target, uint unit, uint processId, ulong threadId, ulong address);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ClearBreakPoint", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT ClearBreakPointX64(int target, uint unit, uint processId, ulong threadId, ulong address);

        public static PS3TMAPI.SNRESULT ClearBreakPoint(int target, PS3TMAPI.UnitType unit, uint processID, ulong threadID, ulong address)
        {
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.ClearBreakPointX64(target, (uint)unit, processID, threadID, address);
            return PS3TMAPI.ClearBreakPointX86(target, (uint)unit, processID, threadID, address);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetBreakPoints", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetBreakPointsX86(int target, uint unit, uint processId, ulong threadId, out uint numBreakpoints, ulong[] addresses);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetBreakPoints", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetBreakPointsX64(int target, uint unit, uint processId, ulong threadId, out uint numBreakpoints, ulong[] addresses);

        public static PS3TMAPI.SNRESULT GetBreakPoints(int target, PS3TMAPI.UnitType unit, uint processID, ulong threadID, out ulong[] bpAddresses)
        {
            bpAddresses = (ulong[])null;
            uint numBreakpoints;
            PS3TMAPI.SNRESULT res = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetBreakPointsX86(target, (uint)unit, processID, threadID, out numBreakpoints, (ulong[])null) : PS3TMAPI.GetBreakPointsX64(target, (uint)unit, processID, threadID, out numBreakpoints, (ulong[])null);
            if (PS3TMAPI.FAILED(res))
                return res;
            bpAddresses = new ulong[(int)numBreakpoints];
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.GetBreakPointsX64(target, (uint)unit, processID, threadID, out numBreakpoints, bpAddresses);
            return PS3TMAPI.GetBreakPointsX86(target, (uint)unit, processID, threadID, out numBreakpoints, bpAddresses);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetDebugThreadControlInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetDebugThreadControlInfoX86(int target, uint processId, ref uint bufferSize, IntPtr buffer);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetDebugThreadControlInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetDebugThreadControlInfoX64(int target, uint processId, ref uint bufferSize, IntPtr buffer);

        public static PS3TMAPI.SNRESULT GetDebugThreadControlInfo(int target, uint processID, out PS3TMAPI.DebugThreadControlInfo threadCtrlInfo)
        {
            threadCtrlInfo = new PS3TMAPI.DebugThreadControlInfo();
            uint bufferSize = 0;
            PS3TMAPI.SNRESULT res1 = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetDebugThreadControlInfoX86(target, processID, ref bufferSize, IntPtr.Zero) : PS3TMAPI.GetDebugThreadControlInfoX64(target, processID, ref bufferSize, IntPtr.Zero);
            if (PS3TMAPI.FAILED(res1))
                return res1;
            PS3TMAPI.ScopedGlobalHeapPtr scopedGlobalHeapPtr = new PS3TMAPI.ScopedGlobalHeapPtr(Marshal.AllocHGlobal((int)bufferSize));
            PS3TMAPI.SNRESULT res2 = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetDebugThreadControlInfoX86(target, processID, ref bufferSize, scopedGlobalHeapPtr.Get()) : PS3TMAPI.GetDebugThreadControlInfoX64(target, processID, ref bufferSize, scopedGlobalHeapPtr.Get());
            if (PS3TMAPI.FAILED(res2))
                return res2;
            PS3TMAPI.DebugThreadControlInfoPriv storage = new PS3TMAPI.DebugThreadControlInfoPriv();
            IntPtr num = PS3TMAPI.ReadDataFromUnmanagedIncPtr<PS3TMAPI.DebugThreadControlInfoPriv>(scopedGlobalHeapPtr.Get(), ref storage);
            threadCtrlInfo.ControlFlags = storage.ControlFlags;
            uint numEntries = storage.NumEntries;
            threadCtrlInfo.ControlKeywords = new PS3TMAPI.ControlKeywordEntry[(int)numEntries];
            for (uint index = 0; index < numEntries; ++index)
            {
                num = PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(num, ref threadCtrlInfo.ControlKeywords[(int)index].MatchConditionFlags);
                threadCtrlInfo.ControlKeywords[(int)index].Keyword = PS3TMAPI.Utf8ToString(num, 128U);
                num = new IntPtr(num.ToInt64() + 128L);
            }
            return res2;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SetDebugThreadControlInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT SetDebugThreadControlInfoX86(int target, uint processId, IntPtr threadCtrlInfo, out uint maxEntries);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SetDebugThreadControlInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT SetDebugThreadControlInfoX64(int target, uint processId, IntPtr threadCtrlInfo, out uint maxEntries);

        public static PS3TMAPI.SNRESULT SetDebugThreadControlInfo(int target, uint processID, PS3TMAPI.DebugThreadControlInfo threadCtrlInfo, out uint maxEntries)
        {
            PS3TMAPI.DebugThreadControlInfoPriv storage = new PS3TMAPI.DebugThreadControlInfoPriv();
            storage.ControlFlags = threadCtrlInfo.ControlFlags;
            if (threadCtrlInfo.ControlKeywords != null)
                storage.NumEntries = (uint)threadCtrlInfo.ControlKeywords.Length;
            PS3TMAPI.ScopedGlobalHeapPtr scopedGlobalHeapPtr = new PS3TMAPI.ScopedGlobalHeapPtr(Marshal.AllocHGlobal(Marshal.SizeOf((object)storage) + (int)storage.NumEntries * Marshal.SizeOf(typeof(PS3TMAPI.ControlKeywordEntry))));
            IntPtr unmanagedIncPtr = PS3TMAPI.WriteDataToUnmanagedIncPtr<PS3TMAPI.DebugThreadControlInfoPriv>(storage, scopedGlobalHeapPtr.Get());
            for (int index = 0; (long)index < (long)storage.NumEntries; ++index)
                unmanagedIncPtr = PS3TMAPI.WriteDataToUnmanagedIncPtr<PS3TMAPI.ControlKeywordEntry>(threadCtrlInfo.ControlKeywords[index], unmanagedIncPtr);
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.SetDebugThreadControlInfoX64(target, processID, scopedGlobalHeapPtr.Get(), out maxEntries);
            return PS3TMAPI.SetDebugThreadControlInfoX86(target, processID, scopedGlobalHeapPtr.Get(), out maxEntries);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ThreadExceptionClean", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT ThreadExceptionCleanX86(int target, uint processId, ulong threadId);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ThreadExceptionClean", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT ThreadExceptionCleanX64(int target, uint processId, ulong threadId);

        public static PS3TMAPI.SNRESULT ThreadExceptionClean(int target, uint processID, ulong threadID)
        {
            return PS3TMAPI.Is32Bit() ? PS3TMAPI.ThreadExceptionCleanX86(target, processID, threadID) : PS3TMAPI.ThreadExceptionCleanX64(target, processID, threadID);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetRawSPULogicalIDs", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetRawSPULogicalIdsX86(int target, uint processId, ulong[] logicalIds);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetRawSPULogicalIDs", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetRawSPULogicalIdsX64(int target, uint processId, ulong[] logicalIds);

        public static PS3TMAPI.SNRESULT GetRawSPULogicalIDs(int target, uint processID, out ulong[] logicalIDs)
        {
            logicalIDs = new ulong[8];
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.GetRawSPULogicalIdsX64(target, processID, logicalIDs);
            return PS3TMAPI.GetRawSPULogicalIdsX86(target, processID, logicalIDs);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SPUThreadGroupStop", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT SPUThreadGroupStopX86(int target, uint processId, ulong threadGroupId);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SPUThreadGroupStop", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT SPUThreadGroupStopX64(int target, uint processId, ulong threadGroupId);

        public static PS3TMAPI.SNRESULT SPUThreadGroupStop(int target, uint processID, ulong threadGroupID)
        {
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.SPUThreadGroupStopX64(target, processID, threadGroupID);
            return PS3TMAPI.SPUThreadGroupStopX86(target, processID, threadGroupID);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SPUThreadGroupContinue", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT SPUThreadGroupContinueX86(int target, uint processId, ulong threadGroupId);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SPUThreadGroupContinue", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT SPUThreadGroupContinueX64(int target, uint processId, ulong threadGroupId);

        public static PS3TMAPI.SNRESULT SPUThreadGroupContinue(int target, uint processID, ulong threadGroupID)
        {
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.SPUThreadGroupContinueX64(target, processID, threadGroupID);
            return PS3TMAPI.SPUThreadGroupContinueX86(target, processID, threadGroupID);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetProcessTree", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetProcessTreeX86(int target, ref uint numProcesses, IntPtr buffer);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetProcessTree", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetProcessTreeX64(int target, ref uint numProcesses, IntPtr buffer);

        public static PS3TMAPI.SNRESULT GetProcessTree(int target, out PS3TMAPI.ProcessTreeBranch[] processTree)
        {
            processTree = (PS3TMAPI.ProcessTreeBranch[])null;
            uint numProcesses = 0;
            PS3TMAPI.SNRESULT res1 = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetProcessTreeX86(target, ref numProcesses, IntPtr.Zero) : PS3TMAPI.GetProcessTreeX64(target, ref numProcesses, IntPtr.Zero);
            if (PS3TMAPI.FAILED(res1))
                return res1;
            PS3TMAPI.ScopedGlobalHeapPtr scopedGlobalHeapPtr = new PS3TMAPI.ScopedGlobalHeapPtr(Marshal.AllocHGlobal((int)numProcesses * Marshal.SizeOf(typeof(PS3TMAPI.ProcessTreeBranchPriv))));
            PS3TMAPI.SNRESULT res2 = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetProcessTreeX86(target, ref numProcesses, scopedGlobalHeapPtr.Get()) : PS3TMAPI.GetProcessTreeX64(target, ref numProcesses, scopedGlobalHeapPtr.Get());
            if (PS3TMAPI.FAILED(res2))
                return res2;
            processTree = new PS3TMAPI.ProcessTreeBranch[(int)numProcesses];
            for (int index1 = 0; (long)index1 < (long)numProcesses; ++index1)
            {
                PS3TMAPI.ProcessTreeBranchPriv structure = (PS3TMAPI.ProcessTreeBranchPriv)Marshal.PtrToStructure(scopedGlobalHeapPtr.Get(), typeof(PS3TMAPI.ProcessTreeBranchPriv));
                processTree[index1].ProcessID = structure.ProcessId;
                processTree[index1].ProcessState = structure.ProcessState;
                processTree[index1].ProcessFlags = structure.ProcessFlags;
                processTree[index1].RawSPU = structure.RawSPU;
                processTree[index1].PPUThreadStatuses = new PS3TMAPI.PPUThreadStatus[(int)structure.NumPpuThreads];
                processTree[index1].SPUThreadGroupStatuses = new PS3TMAPI.SPUThreadGroupStatus[(int)structure.NumSpuThreadGroups];
                for (int index2 = 0; (long)index2 < (long)structure.NumPpuThreads; ++index2)
                    structure.PpuThreadStatuses = PS3TMAPI.ReadDataFromUnmanagedIncPtr<PS3TMAPI.PPUThreadStatus>(structure.PpuThreadStatuses, ref processTree[index1].PPUThreadStatuses[index2]);
                for (int index2 = 0; (long)index2 < (long)structure.NumSpuThreadGroups; ++index2)
                    structure.SpuThreadGroupStatuses = PS3TMAPI.ReadDataFromUnmanagedIncPtr<PS3TMAPI.SPUThreadGroupStatus>(structure.SpuThreadGroupStatuses, ref processTree[index1].SPUThreadGroupStatuses[index2]);
            }
            return res2;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetSPUThreadGroupInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetSPUThreadGroupInfoX86(int target, uint processId, ulong threadGroupId, ref uint bufferSize, IntPtr buffer);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetSPUThreadGroupInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetSPUThreadGroupInfoX64(int target, uint processId, ulong threadGroupId, ref uint bufferSize, IntPtr buffer);

        public static PS3TMAPI.SNRESULT GetSPUThreadGroupInfo(int target, uint processID, ulong threadGroupID, out PS3TMAPI.SPUThreadGroupInfo threadGroupInfo)
        {
            threadGroupInfo = new PS3TMAPI.SPUThreadGroupInfo();
            uint bufferSize = 0;
            PS3TMAPI.SNRESULT res1 = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetSPUThreadGroupInfoX86(target, processID, threadGroupID, ref bufferSize, IntPtr.Zero) : PS3TMAPI.GetSPUThreadGroupInfoX64(target, processID, threadGroupID, ref bufferSize, IntPtr.Zero);
            if (PS3TMAPI.FAILED(res1))
                return res1;
            PS3TMAPI.ScopedGlobalHeapPtr scopedGlobalHeapPtr = new PS3TMAPI.ScopedGlobalHeapPtr(Marshal.AllocHGlobal((int)bufferSize));
            PS3TMAPI.SNRESULT res2 = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetSPUThreadGroupInfoX86(target, processID, threadGroupID, ref bufferSize, scopedGlobalHeapPtr.Get()) : PS3TMAPI.GetSPUThreadGroupInfoX64(target, processID, threadGroupID, ref bufferSize, scopedGlobalHeapPtr.Get());
            if (PS3TMAPI.FAILED(res2))
                return res2;
            PS3TMAPI.SpuThreadGroupInfoPriv storage = new PS3TMAPI.SpuThreadGroupInfoPriv();
            IntPtr num = PS3TMAPI.ReadDataFromUnmanagedIncPtr<PS3TMAPI.SpuThreadGroupInfoPriv>(scopedGlobalHeapPtr.Get(), ref storage);
            threadGroupInfo.ThreadGroupID = storage.ThreadGroupId;
            threadGroupInfo.State = (PS3TMAPI.SPUThreadGroupState)storage.State;
            threadGroupInfo.Priority = storage.Priority;
            threadGroupInfo.ThreadIDs = new uint[(int)storage.NumThreads];
            for (int index = 0; (long)index < (long)storage.NumThreads; ++index)
                num = PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(num, ref threadGroupInfo.ThreadIDs[index]);
            if (storage.ThreadGroupNameLen > 0U)
                threadGroupInfo.GroupName = PS3TMAPI.Utf8ToString(num, uint.MaxValue);
            return res2;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ProcessGetMemory", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT ProcessGetMemoryX86(int target, PS3TMAPI.UnitType unit, uint processId, ulong threadId, ulong address, int count, byte[] buffer);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ProcessGetMemory", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT ProcessGetMemoryX64(int target, PS3TMAPI.UnitType unit, uint processId, ulong threadId, ulong address, int count, byte[] buffer);

        public static PS3TMAPI.SNRESULT ProcessGetMemory(int target, PS3TMAPI.UnitType unit, uint processID, ulong threadID, ulong address, ref byte[] buffer)
        {
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.ProcessGetMemoryX64(target, unit, processID, threadID, address, buffer.Length, buffer);
            return PS3TMAPI.ProcessGetMemoryX86(target, unit, processID, threadID, address, buffer.Length, buffer);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ProcessSetMemory", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT ProcessSetMemoryX86(int target, PS3TMAPI.UnitType unit, uint processId, ulong threadId, ulong address, int count, byte[] buffer);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ProcessSetMemory", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT ProcessSetMemoryX64(int target, PS3TMAPI.UnitType unit, uint processId, ulong threadId, ulong address, int count, byte[] buffer);

        public static PS3TMAPI.SNRESULT ProcessSetMemory(int target, PS3TMAPI.UnitType unit, uint processID, ulong threadID, ulong address, byte[] buffer)
        {
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.ProcessSetMemoryX64(target, unit, processID, threadID, address, buffer.Length, buffer);
            return PS3TMAPI.ProcessSetMemoryX86(target, unit, processID, threadID, address, buffer.Length, buffer);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetMemoryCompressed", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetMemoryCompressedX86(int target, uint processId, uint compressionLevel, uint address, uint size, byte[] buffer);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetMemoryCompressed", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetMemoryCompressedX64(int target, uint processId, uint compressionLevel, uint address, uint size, byte[] buffer);

        public static PS3TMAPI.SNRESULT GetMemoryCompressed(int target, uint processID, PS3TMAPI.MemoryCompressionLevel compressionLevel, uint address, ref byte[] buffer)
        {
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.GetMemoryCompressedX64(target, processID, (uint)compressionLevel, address, (uint)buffer.Length, buffer);
            return PS3TMAPI.GetMemoryCompressedX86(target, processID, (uint)compressionLevel, address, (uint)buffer.Length, buffer);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetMemory64Compressed", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetMemory64CompressedX86(int target, uint processId, uint compressionLevel, ulong address, uint size, byte[] buffer);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetMemory64Compressed", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetMemory64CompressedX64(int target, uint processId, uint compressionLevel, ulong address, uint size, byte[] buffer);

        public static PS3TMAPI.SNRESULT GetMemory64Compressed(int target, uint processID, PS3TMAPI.MemoryCompressionLevel compressionLevel, ulong address, ref byte[] buffer)
        {
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.GetMemory64CompressedX64(target, processID, (uint)compressionLevel, address, (uint)buffer.Length, buffer);
            return PS3TMAPI.GetMemory64CompressedX86(target, processID, (uint)compressionLevel, address, (uint)buffer.Length, buffer);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetVirtualMemoryInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetVirtualMemoryInfoX86(int target, uint processId, bool bStatsOnly, out uint areaCount, out uint bufferSize, IntPtr buffer);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetVirtualMemoryInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetVirtualMemoryInfoX64(int target, uint processId, bool bStatsOnly, out uint areaCount, out uint bufferSize, IntPtr buffer);

        public static PS3TMAPI.SNRESULT GetVirtualMemoryInfo(int target, uint processID, bool bStatsOnly, out PS3TMAPI.VirtualMemoryArea[] vmAreas)
        {
            vmAreas = (PS3TMAPI.VirtualMemoryArea[])null;
            uint areaCount;
            uint bufferSize;
            PS3TMAPI.SNRESULT res1 = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetVirtualMemoryInfoX86(target, processID, bStatsOnly, out areaCount, out bufferSize, IntPtr.Zero) : PS3TMAPI.GetVirtualMemoryInfoX64(target, processID, bStatsOnly, out areaCount, out bufferSize, IntPtr.Zero);
            if (PS3TMAPI.FAILED(res1))
                return res1;
            PS3TMAPI.ScopedGlobalHeapPtr scopedGlobalHeapPtr = new PS3TMAPI.ScopedGlobalHeapPtr(Marshal.AllocHGlobal((int)bufferSize));
            PS3TMAPI.SNRESULT res2 = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetVirtualMemoryInfoX86(target, processID, bStatsOnly, out areaCount, out bufferSize, scopedGlobalHeapPtr.Get()) : PS3TMAPI.GetVirtualMemoryInfoX64(target, processID, bStatsOnly, out areaCount, out bufferSize, scopedGlobalHeapPtr.Get());
            if (PS3TMAPI.FAILED(res2))
                return res2;
            vmAreas = new PS3TMAPI.VirtualMemoryArea[(int)areaCount];
            IntPtr unmanagedBuf1 = scopedGlobalHeapPtr.Get();
            for (int index = 0; (long)index < (long)areaCount; ++index)
            {
                IntPtr unmanagedBuf2 = PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(unmanagedBuf1, ref vmAreas[index].Address), ref vmAreas[index].Flags), ref vmAreas[index].VSize), ref vmAreas[index].Options), ref vmAreas[index].PageFaultPPU), ref vmAreas[index].PageFaultSPU), ref vmAreas[index].PageIn), ref vmAreas[index].PageOut), ref vmAreas[index].PMemTotal), ref vmAreas[index].PMemUsed), ref vmAreas[index].Time);
                ulong storage = 0;
                IntPtr unmanagedBuf3 = PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(unmanagedBuf2, ref storage);
                vmAreas[index].Pages = new ulong[storage];
                IntPtr zero = IntPtr.Zero;
                unmanagedBuf1 = PS3TMAPI.ReadDataFromUnmanagedIncPtr<IntPtr>(unmanagedBuf3, ref zero);
            }
            for (int index1 = 0; (long)index1 < (long)areaCount; ++index1)
            {
                int length = vmAreas[index1].Pages.Length;
                for (int index2 = 0; index2 < length; ++index2)
                    unmanagedBuf1 = PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(unmanagedBuf1, ref vmAreas[index1].Pages[index2]);
            }
            return res2;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetSyncPrimitiveCountsEx", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetSyncPrimitiveCountsExX86(int target, uint processId, ref uint bufferSize, IntPtr buffer);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetSyncPrimitiveCountsEx", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetSyncPrimitiveCountsExX64(int target, uint processId, ref uint bufferSize, IntPtr buffer);

        public static PS3TMAPI.SNRESULT GetSyncPrimitiveCounts(int target, uint processID, out PS3TMAPI.SyncPrimitiveCounts primitiveCounts)
        {
            primitiveCounts = new PS3TMAPI.SyncPrimitiveCounts();
            uint bufferSize = 32;
            PS3TMAPI.ScopedGlobalHeapPtr scopedGlobalHeapPtr = new PS3TMAPI.ScopedGlobalHeapPtr(Marshal.AllocHGlobal((int)bufferSize));
            PS3TMAPI.SNRESULT res = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetSyncPrimitiveCountsExX86(target, processID, ref bufferSize, scopedGlobalHeapPtr.Get()) : PS3TMAPI.GetSyncPrimitiveCountsExX64(target, processID, ref bufferSize, scopedGlobalHeapPtr.Get());
            if (PS3TMAPI.FAILED(res))
                return res;
            primitiveCounts = (PS3TMAPI.SyncPrimitiveCounts)Marshal.PtrToStructure(scopedGlobalHeapPtr.Get(), typeof(PS3TMAPI.SyncPrimitiveCounts));
            return res;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetMutexList", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetMutexListX86(int target, uint processId, ref uint numMutexes, uint[] mutexList);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetMutexList", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetMutexListX64(int target, uint processId, ref uint numMutexes, uint[] mutexList);

        public static PS3TMAPI.SNRESULT GetMutexList(int target, uint processID, out uint[] mutexIDs)
        {
            mutexIDs = (uint[])null;
            uint numMutexes = 0;
            PS3TMAPI.SNRESULT res = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetMutexListX86(target, processID, ref numMutexes, (uint[])null) : PS3TMAPI.GetMutexListX64(target, processID, ref numMutexes, (uint[])null);
            if (PS3TMAPI.FAILED(res))
                return res;
            mutexIDs = new uint[(int)numMutexes];
            return PS3TMAPI.Is32Bit() ? PS3TMAPI.GetMutexListX86(target, processID, ref numMutexes, mutexIDs) : PS3TMAPI.GetMutexListX64(target, processID, ref numMutexes, mutexIDs);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetMutexInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetMutexInfoX86(int target, uint processId, uint mutexId, ref uint bufferSize, IntPtr buffer);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetMutexInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetMutexInfoX64(int target, uint processId, uint mutexId, ref uint bufferSize, IntPtr buffer);

        public static PS3TMAPI.SNRESULT GetMutexInfo(int target, uint processID, uint mutexID, out PS3TMAPI.MutexInfo mutexInfo)
        {
            mutexInfo = new PS3TMAPI.MutexInfo();
            uint bufferSize = 0;
            PS3TMAPI.SNRESULT res1 = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetMutexInfoX86(target, processID, mutexID, ref bufferSize, IntPtr.Zero) : PS3TMAPI.GetMutexInfoX64(target, processID, mutexID, ref bufferSize, IntPtr.Zero);
            if (PS3TMAPI.FAILED(res1))
                return res1;
            PS3TMAPI.ScopedGlobalHeapPtr scopedGlobalHeapPtr = new PS3TMAPI.ScopedGlobalHeapPtr(Marshal.AllocHGlobal((int)bufferSize));
            PS3TMAPI.SNRESULT res2 = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetMutexInfoX86(target, processID, mutexID, ref bufferSize, scopedGlobalHeapPtr.Get()) : PS3TMAPI.GetMutexInfoX64(target, processID, mutexID, ref bufferSize, scopedGlobalHeapPtr.Get());
            if (PS3TMAPI.FAILED(res2))
                return res2;
            IntPtr num = PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(scopedGlobalHeapPtr.Get(), ref mutexInfo.ID), ref mutexInfo.Attribute.Protocol), ref mutexInfo.Attribute.Recursive), ref mutexInfo.Attribute.PShared), ref mutexInfo.Attribute.Adaptive), ref mutexInfo.Attribute.Key), ref mutexInfo.Attribute.Flags);
            mutexInfo.Attribute.Name = PS3TMAPI.Utf8ToString(num, 8U);
            num = new IntPtr(num.ToInt64() + 8L);
            num = PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(num, ref mutexInfo.OwnerThreadID);
            num = PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(num, ref mutexInfo.LockCounter);
            num = PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(num, ref mutexInfo.ConditionRefCounter);
            num = PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(num, ref mutexInfo.ConditionVarID);
            uint storage = 0;
            num = PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(num, ref storage);
            num = PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(num, ref mutexInfo.NumWaitAllThreads);
            mutexInfo.WaitingThreads = new ulong[(int)storage];
            for (int index = 0; (long)index < (long)storage; ++index)
                num = PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(num, ref mutexInfo.WaitingThreads[index]);
            return res2;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetLightWeightMutexList", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetLightWeightMutexListX86(int target, uint processId, ref uint numLWMutexes, uint[] lwMutexList);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetLightWeightMutexList", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetLightWeightMutexListX64(int target, uint processId, ref uint numLWMutexes, uint[] lwMutexList);

        public static PS3TMAPI.SNRESULT GetLightWeightMutexList(int target, uint processID, out uint[] lwMutexIDs)
        {
            lwMutexIDs = (uint[])null;
            uint numLWMutexes = 0;
            PS3TMAPI.SNRESULT res = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetLightWeightMutexListX86(target, processID, ref numLWMutexes, (uint[])null) : PS3TMAPI.GetLightWeightMutexListX64(target, processID, ref numLWMutexes, (uint[])null);
            if (PS3TMAPI.FAILED(res))
                return res;
            lwMutexIDs = new uint[(int)numLWMutexes];
            return PS3TMAPI.Is32Bit() ? PS3TMAPI.GetLightWeightMutexListX86(target, processID, ref numLWMutexes, lwMutexIDs) : PS3TMAPI.GetLightWeightMutexListX64(target, processID, ref numLWMutexes, lwMutexIDs);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetLightWeightMutexInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetLightWeightMutexInfoX86(int target, uint processId, uint lwMutexId, ref uint bufferSize, IntPtr buffer);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetLightWeightMutexInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetLightWeightMutexInfoX64(int target, uint processId, uint lwMutexId, ref uint bufferSize, IntPtr buffer);

        public static PS3TMAPI.SNRESULT GetLightWeightMutexInfo(int target, uint processID, uint lwMutexID, out PS3TMAPI.LWMutexInfo lwMutexInfo)
        {
            lwMutexInfo = new PS3TMAPI.LWMutexInfo();
            uint bufferSize = 0;
            PS3TMAPI.SNRESULT res1 = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetLightWeightMutexInfoX86(target, processID, lwMutexID, ref bufferSize, IntPtr.Zero) : PS3TMAPI.GetLightWeightMutexInfoX64(target, processID, lwMutexID, ref bufferSize, IntPtr.Zero);
            if (PS3TMAPI.FAILED(res1))
                return res1;
            PS3TMAPI.ScopedGlobalHeapPtr scopedGlobalHeapPtr = new PS3TMAPI.ScopedGlobalHeapPtr(Marshal.AllocHGlobal((int)bufferSize));
            PS3TMAPI.SNRESULT res2 = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetLightWeightMutexInfoX86(target, processID, lwMutexID, ref bufferSize, scopedGlobalHeapPtr.Get()) : PS3TMAPI.GetLightWeightMutexInfoX64(target, processID, lwMutexID, ref bufferSize, scopedGlobalHeapPtr.Get());
            if (PS3TMAPI.FAILED(res2))
                return res2;
            PS3TMAPI.LwMutexInfoPriv storage = new PS3TMAPI.LwMutexInfoPriv();
            IntPtr unmanagedBuf = PS3TMAPI.ReadDataFromUnmanagedIncPtr<PS3TMAPI.LwMutexInfoPriv>(scopedGlobalHeapPtr.Get(), ref storage);
            lwMutexInfo.ID = storage.Id;
            lwMutexInfo.Attribute = storage.Attribute;
            lwMutexInfo.OwnerThreadID = storage.OwnerThreadId;
            lwMutexInfo.LockCounter = storage.LockCounter;
            lwMutexInfo.NumWaitAllThreads = storage.NumWaitAllThreads;
            lwMutexInfo.WaitingThreads = new ulong[(int)storage.NumWaitingThreads];
            for (int index = 0; (long)index < (long)storage.NumWaitingThreads; ++index)
                unmanagedBuf = PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(unmanagedBuf, ref lwMutexInfo.WaitingThreads[index]);
            return res2;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetConditionalVariableList", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetConditionalVariableListX86(int target, uint processId, ref uint numConditionVars, uint[] conditionVarList);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetConditionalVariableList", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetConditionalVariableListX64(int target, uint processId, ref uint numConditionVars, uint[] conditionVarList);

        public static PS3TMAPI.SNRESULT GetConditionalVariableList(int target, uint processID, out uint[] conditionVarIDs)
        {
            conditionVarIDs = (uint[])null;
            uint numConditionVars = 0;
            PS3TMAPI.SNRESULT res = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetConditionalVariableListX86(target, processID, ref numConditionVars, (uint[])null) : PS3TMAPI.GetConditionalVariableListX64(target, processID, ref numConditionVars, (uint[])null);
            if (PS3TMAPI.FAILED(res))
                return res;
            conditionVarIDs = new uint[(int)numConditionVars];
            return PS3TMAPI.Is32Bit() ? PS3TMAPI.GetConditionalVariableListX86(target, processID, ref numConditionVars, conditionVarIDs) : PS3TMAPI.GetConditionalVariableListX64(target, processID, ref numConditionVars, conditionVarIDs);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetConditionalVariableInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetConditionalVariableInfoX86(int target, uint processId, uint conditionVarId, ref uint bufferSize, IntPtr buffer);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetConditionalVariableInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetConditionalVariableInfoX64(int target, uint processId, uint conditionVarId, ref uint bufferSize, IntPtr buffer);

        public static PS3TMAPI.SNRESULT GetConditionalVariableInfo(int target, uint processID, uint conditionVarID, out PS3TMAPI.ConditionVarInfo conditionVarInfo)
        {
            conditionVarInfo = new PS3TMAPI.ConditionVarInfo();
            uint bufferSize = 0;
            PS3TMAPI.SNRESULT res1 = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetConditionalVariableInfoX86(target, processID, conditionVarID, ref bufferSize, IntPtr.Zero) : PS3TMAPI.GetConditionalVariableInfoX64(target, processID, conditionVarID, ref bufferSize, IntPtr.Zero);
            if (PS3TMAPI.FAILED(res1))
                return res1;
            PS3TMAPI.ScopedGlobalHeapPtr scopedGlobalHeapPtr = new PS3TMAPI.ScopedGlobalHeapPtr(Marshal.AllocHGlobal((int)bufferSize));
            PS3TMAPI.SNRESULT res2 = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetConditionalVariableInfoX86(target, processID, conditionVarID, ref bufferSize, scopedGlobalHeapPtr.Get()) : PS3TMAPI.GetConditionalVariableInfoX64(target, processID, conditionVarID, ref bufferSize, scopedGlobalHeapPtr.Get());
            if (PS3TMAPI.FAILED(res2))
                return res2;
            PS3TMAPI.ConditionVarInfoPriv storage = new PS3TMAPI.ConditionVarInfoPriv();
            IntPtr unmanagedBuf = PS3TMAPI.ReadDataFromUnmanagedIncPtr<PS3TMAPI.ConditionVarInfoPriv>(scopedGlobalHeapPtr.Get(), ref storage);
            conditionVarInfo.ID = storage.Id;
            conditionVarInfo.Attribute = storage.Attribute;
            conditionVarInfo.MutexID = storage.MutexId;
            conditionVarInfo.NumWaitAllThreads = storage.NumWaitAllThreads;
            conditionVarInfo.WaitingThreads = new ulong[(int)storage.NumWaitingThreads];
            for (int index = 0; (long)index < (long)storage.NumWaitingThreads; ++index)
                unmanagedBuf = PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(unmanagedBuf, ref conditionVarInfo.WaitingThreads[index]);
            return res2;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetLightWeightConditionalList", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetLightWeightConditionalListX86(int target, uint processId, ref uint numLWConditionVars, uint[] lwConditionVarList);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetLightWeightConditionalList", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetLightWeightConditionalListX64(int target, uint processId, ref uint numLWConditionVars, uint[] lwConditionVarList);

        public static PS3TMAPI.SNRESULT GetLightWeightConditionalList(int target, uint processID, out uint[] lwConditionVarIDs)
        {
            lwConditionVarIDs = (uint[])null;
            uint numLWConditionVars = 0;
            PS3TMAPI.SNRESULT res = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetLightWeightConditionalListX86(target, processID, ref numLWConditionVars, (uint[])null) : PS3TMAPI.GetLightWeightConditionalListX64(target, processID, ref numLWConditionVars, (uint[])null);
            if (PS3TMAPI.FAILED(res))
                return res;
            lwConditionVarIDs = new uint[(int)numLWConditionVars];
            return PS3TMAPI.Is32Bit() ? PS3TMAPI.GetLightWeightConditionalListX86(target, processID, ref numLWConditionVars, lwConditionVarIDs) : PS3TMAPI.GetLightWeightConditionalListX64(target, processID, ref numLWConditionVars, lwConditionVarIDs);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetLightWeightConditionalInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetLightWeightConditionalInfoX86(int target, uint processId, uint lwCondVarId, ref uint bufferSize, IntPtr buffer);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetLightWeightConditionalInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetLightWeightConditionalInfoX64(int target, uint processId, uint lwCondVarId, ref uint bufferSize, IntPtr buffer);

        public static PS3TMAPI.SNRESULT GetLightWeightConditionalInfo(int target, uint processID, uint lwCondVarID, out PS3TMAPI.LWConditionVarInfo lwConditonVarInfo)
        {
            lwConditonVarInfo = new PS3TMAPI.LWConditionVarInfo();
            uint bufferSize = 0;
            PS3TMAPI.SNRESULT res1 = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetLightWeightConditionalInfoX86(target, processID, lwCondVarID, ref bufferSize, IntPtr.Zero) : PS3TMAPI.GetLightWeightConditionalInfoX64(target, processID, lwCondVarID, ref bufferSize, IntPtr.Zero);
            if (PS3TMAPI.FAILED(res1))
                return res1;
            PS3TMAPI.ScopedGlobalHeapPtr scopedGlobalHeapPtr = new PS3TMAPI.ScopedGlobalHeapPtr(Marshal.AllocHGlobal((int)bufferSize));
            PS3TMAPI.SNRESULT res2 = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetLightWeightConditionalInfoX86(target, processID, lwCondVarID, ref bufferSize, scopedGlobalHeapPtr.Get()) : PS3TMAPI.GetLightWeightConditionalInfoX64(target, processID, lwCondVarID, ref bufferSize, scopedGlobalHeapPtr.Get());
            if (PS3TMAPI.FAILED(res2))
                return res2;
            PS3TMAPI.LwConditionVarInfoPriv storage = new PS3TMAPI.LwConditionVarInfoPriv();
            IntPtr unmanagedBuf = PS3TMAPI.ReadDataFromUnmanagedIncPtr<PS3TMAPI.LwConditionVarInfoPriv>(scopedGlobalHeapPtr.Get(), ref storage);
            lwConditonVarInfo = new PS3TMAPI.LWConditionVarInfo();
            lwConditonVarInfo.ID = storage.Id;
            lwConditonVarInfo.Attribute = storage.Attribute;
            lwConditonVarInfo.LWMutexID = storage.LwMutexId;
            lwConditonVarInfo.NumWaitAllThreads = storage.NumWaitAllThreads;
            lwConditonVarInfo.WaitingThreads = new ulong[(int)storage.NumWaitingThreads];
            for (int index = 0; (long)index < (long)storage.NumWaitingThreads; ++index)
                unmanagedBuf = PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(unmanagedBuf, ref lwConditonVarInfo.WaitingThreads[index]);
            return res2;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetReadWriteLockList", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetReadWriteLockListX86(int target, uint processId, ref uint numRWLocks, uint[] rwLockList);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetReadWriteLockList", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetReadWriteLockListX64(int target, uint processId, ref uint numRWLocks, uint[] rwLockList);

        public static PS3TMAPI.SNRESULT GetReadWriteLockList(int target, uint processID, out uint[] rwLockList)
        {
            rwLockList = (uint[])null;
            uint numRWLocks = 0;
            PS3TMAPI.SNRESULT res = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetReadWriteLockListX86(target, processID, ref numRWLocks, (uint[])null) : PS3TMAPI.GetReadWriteLockListX64(target, processID, ref numRWLocks, (uint[])null);
            if (PS3TMAPI.FAILED(res))
                return res;
            rwLockList = new uint[(int)numRWLocks];
            return PS3TMAPI.Is32Bit() ? PS3TMAPI.GetReadWriteLockListX86(target, processID, ref numRWLocks, rwLockList) : PS3TMAPI.GetReadWriteLockListX64(target, processID, ref numRWLocks, rwLockList);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetReadWriteLockInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetReadWriteLockInfoX86(int target, uint processId, uint rwLockId, ref uint bufferSize, IntPtr buffer);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetReadWriteLockInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetReadWriteLockInfoX64(int target, uint processId, uint rwLockId, ref uint bufferSize, IntPtr buffer);

        public static PS3TMAPI.SNRESULT GetReadWriteLockInfo(int target, uint processID, uint rwLockID, out PS3TMAPI.RWLockInfo rwLockInfo)
        {
            rwLockInfo = new PS3TMAPI.RWLockInfo();
            uint bufferSize = 0;
            PS3TMAPI.SNRESULT res1 = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetReadWriteLockInfoX86(target, processID, rwLockID, ref bufferSize, IntPtr.Zero) : PS3TMAPI.GetReadWriteLockInfoX64(target, processID, rwLockID, ref bufferSize, IntPtr.Zero);
            if (PS3TMAPI.FAILED(res1))
                return res1;
            PS3TMAPI.ScopedGlobalHeapPtr scopedGlobalHeapPtr = new PS3TMAPI.ScopedGlobalHeapPtr(Marshal.AllocHGlobal((int)bufferSize));
            PS3TMAPI.SNRESULT res2 = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetReadWriteLockInfoX86(target, processID, rwLockID, ref bufferSize, scopedGlobalHeapPtr.Get()) : PS3TMAPI.GetReadWriteLockInfoX64(target, processID, rwLockID, ref bufferSize, scopedGlobalHeapPtr.Get());
            if (PS3TMAPI.FAILED(res2))
                return res2;
            PS3TMAPI.RwLockInfoPriv storage = new PS3TMAPI.RwLockInfoPriv();
            IntPtr unmanagedBuf = PS3TMAPI.ReadDataFromUnmanagedIncPtr<PS3TMAPI.RwLockInfoPriv>(scopedGlobalHeapPtr.Get(), ref storage);
            rwLockInfo.ID = storage.Id;
            rwLockInfo.Attribute = storage.Attribute;
            rwLockInfo.NumWaitingReadThreads = storage.NumWaitingReadThreads;
            rwLockInfo.NumWaitAllReadThreads = storage.NumWaitAllReadThreads;
            rwLockInfo.NumWaitingWriteThreads = storage.NumWaitingWriteThreads;
            rwLockInfo.NumWaitAllWriteThreads = storage.NumWaitAllWriteThreads;
            uint num = rwLockInfo.NumWaitingReadThreads + rwLockInfo.NumWaitingWriteThreads;
            rwLockInfo.WaitingThreads = new ulong[(int)num];
            for (int index = 0; (long)index < (long)num; ++index)
                unmanagedBuf = PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(unmanagedBuf, ref rwLockInfo.WaitingThreads[index]);
            return res2;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetSemaphoreList", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetSemaphoreListX86(int target, uint processId, ref uint numSemaphores, uint[] semaphoreList);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetSemaphoreList", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetSemaphoreListX64(int target, uint processId, ref uint numSemaphores, uint[] semaphoreList);

        public static PS3TMAPI.SNRESULT GetSemaphoreList(int target, uint processID, out uint[] semaphoreIDs)
        {
            semaphoreIDs = (uint[])null;
            uint numSemaphores = 0;
            PS3TMAPI.SNRESULT res = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetSemaphoreListX86(target, processID, ref numSemaphores, (uint[])null) : PS3TMAPI.GetSemaphoreListX64(target, processID, ref numSemaphores, (uint[])null);
            if (PS3TMAPI.FAILED(res))
                return res;
            semaphoreIDs = new uint[(int)numSemaphores];
            return PS3TMAPI.Is32Bit() ? PS3TMAPI.GetSemaphoreListX86(target, processID, ref numSemaphores, semaphoreIDs) : PS3TMAPI.GetSemaphoreListX64(target, processID, ref numSemaphores, semaphoreIDs);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetSemaphoreInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetSemaphoreInfoX86(int target, uint processId, uint semaphoreId, ref uint bufferSize, IntPtr buffer);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetSemaphoreInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetSemaphoreInfoX64(int target, uint processId, uint semaphoreId, ref uint bufferSize, IntPtr buffer);

        public static PS3TMAPI.SNRESULT GetSemaphoreInfo(int target, uint processID, uint semaphoreID, out PS3TMAPI.SemaphoreInfo semaphoreInfo)
        {
            semaphoreInfo = new PS3TMAPI.SemaphoreInfo();
            uint bufferSize = 0;
            PS3TMAPI.SNRESULT res1 = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetSemaphoreInfoX86(target, processID, semaphoreID, ref bufferSize, IntPtr.Zero) : PS3TMAPI.GetSemaphoreInfoX64(target, processID, semaphoreID, ref bufferSize, IntPtr.Zero);
            if (PS3TMAPI.FAILED(res1))
                return res1;
            PS3TMAPI.ScopedGlobalHeapPtr scopedGlobalHeapPtr = new PS3TMAPI.ScopedGlobalHeapPtr(Marshal.AllocHGlobal((int)bufferSize));
            PS3TMAPI.SNRESULT res2 = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetSemaphoreInfoX86(target, processID, semaphoreID, ref bufferSize, scopedGlobalHeapPtr.Get()) : PS3TMAPI.GetSemaphoreInfoX64(target, processID, semaphoreID, ref bufferSize, scopedGlobalHeapPtr.Get());
            if (PS3TMAPI.FAILED(res2))
                return res2;
            PS3TMAPI.SemaphoreInfoPriv storage = new PS3TMAPI.SemaphoreInfoPriv();
            IntPtr unmanagedBuf = PS3TMAPI.ReadDataFromUnmanagedIncPtr<PS3TMAPI.SemaphoreInfoPriv>(scopedGlobalHeapPtr.Get(), ref storage);
            semaphoreInfo.ID = storage.Id;
            semaphoreInfo.Attribute = storage.Attribute;
            semaphoreInfo.MaxValue = storage.MaxValue;
            semaphoreInfo.CurrentValue = storage.CurrentValue;
            semaphoreInfo.NumWaitAllThreads = storage.NumWaitAllThreads;
            semaphoreInfo.WaitingThreads = new ulong[(int)storage.NumWaitingThreads];
            for (int index = 0; (long)index < (long)storage.NumWaitingThreads; ++index)
                unmanagedBuf = PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(unmanagedBuf, ref semaphoreInfo.WaitingThreads[index]);
            return res2;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetEventQueueList", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetEventQueueListX86(int target, uint processId, ref uint numEventQueues, uint[] eventQueueList);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetEventQueueList", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetEventQueueListX64(int target, uint processId, ref uint numEventQueues, uint[] eventQueueList);

        public static PS3TMAPI.SNRESULT GetEventQueueList(int target, uint processID, out uint[] eventQueueIDs)
        {
            eventQueueIDs = (uint[])null;
            uint numEventQueues = 0;
            PS3TMAPI.SNRESULT res = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetEventQueueListX86(target, processID, ref numEventQueues, (uint[])null) : PS3TMAPI.GetEventQueueListX64(target, processID, ref numEventQueues, (uint[])null);
            if (PS3TMAPI.FAILED(res))
                return res;
            eventQueueIDs = new uint[(int)numEventQueues];
            return PS3TMAPI.Is32Bit() ? PS3TMAPI.GetEventQueueListX86(target, processID, ref numEventQueues, eventQueueIDs) : PS3TMAPI.GetEventQueueListX64(target, processID, ref numEventQueues, eventQueueIDs);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetEventQueueInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetEventQueueInfoX86(int target, uint processId, uint eventQueueId, ref uint bufferSize, IntPtr buffer);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetEventQueueInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetEventQueueInfoX64(int target, uint processId, uint eventQueueId, ref uint bufferSize, IntPtr buffer);

        public static PS3TMAPI.SNRESULT GetEventQueueInfo(int target, uint processID, uint eventQueueID, out PS3TMAPI.EventQueueInfo eventQueueInfo)
        {
            eventQueueInfo = new PS3TMAPI.EventQueueInfo();
            uint bufferSize = 0;
            PS3TMAPI.SNRESULT res1 = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetEventQueueInfoX86(target, processID, eventQueueID, ref bufferSize, IntPtr.Zero) : PS3TMAPI.GetEventQueueInfoX64(target, processID, eventQueueID, ref bufferSize, IntPtr.Zero);
            if (PS3TMAPI.FAILED(res1))
                return res1;
            PS3TMAPI.ScopedGlobalHeapPtr scopedGlobalHeapPtr = new PS3TMAPI.ScopedGlobalHeapPtr(Marshal.AllocHGlobal((int)bufferSize));
            PS3TMAPI.SNRESULT res2 = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetEventQueueInfoX86(target, processID, eventQueueID, ref bufferSize, scopedGlobalHeapPtr.Get()) : PS3TMAPI.GetEventQueueInfoX64(target, processID, eventQueueID, ref bufferSize, scopedGlobalHeapPtr.Get());
            if (PS3TMAPI.FAILED(res2))
                return res2;
            PS3TMAPI.EventQueueInfoPriv structure = (PS3TMAPI.EventQueueInfoPriv)Marshal.PtrToStructure(scopedGlobalHeapPtr.Get(), typeof(PS3TMAPI.EventQueueInfoPriv));
            eventQueueInfo.ID = structure.Id;
            eventQueueInfo.Attribute = structure.Attribute;
            eventQueueInfo.Key = structure.Key;
            eventQueueInfo.Size = structure.Size;
            eventQueueInfo.NumWaitAllThreads = structure.NumWaitAllThreads;
            eventQueueInfo.NumReadableAllEvQueue = structure.NumReadableAllEvQueue;
            eventQueueInfo.WaitingThreadIDs = new ulong[(int)structure.NumWaitingThreads];
            IntPtr unmanagedBuf1 = structure.WaitingThreadIds;
            for (int index = 0; (long)index < (long)structure.NumWaitingThreads; ++index)
                unmanagedBuf1 = PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(unmanagedBuf1, ref eventQueueInfo.WaitingThreadIDs[index]);
            eventQueueInfo.QueueEntries = new PS3TMAPI.SystemEvent[(int)structure.NumReadableEvQueue];
            IntPtr unmanagedBuf2 = structure.QueueEntries;
            for (int index = 0; (long)index < (long)structure.NumReadableEvQueue; ++index)
                unmanagedBuf2 = PS3TMAPI.ReadDataFromUnmanagedIncPtr<PS3TMAPI.SystemEvent>(unmanagedBuf2, ref eventQueueInfo.QueueEntries[index]);
            return res2;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetEventFlagList", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetEventFlagListX86(int target, uint processId, ref uint numEventFlags, uint[] eventFlagList);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetEventFlagList", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetEventFlagListX64(int target, uint processId, ref uint numEventFlags, uint[] eventFlagList);

        public static PS3TMAPI.SNRESULT GetEventFlagList(int target, uint processID, out uint[] eventFlagIDs)
        {
            eventFlagIDs = (uint[])null;
            uint numEventFlags = 0;
            PS3TMAPI.SNRESULT res = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetEventFlagListX86(target, processID, ref numEventFlags, (uint[])null) : PS3TMAPI.GetEventFlagListX64(target, processID, ref numEventFlags, (uint[])null);
            if (PS3TMAPI.FAILED(res))
                return res;
            eventFlagIDs = new uint[(int)numEventFlags];
            return PS3TMAPI.Is32Bit() ? PS3TMAPI.GetEventFlagListX86(target, processID, ref numEventFlags, eventFlagIDs) : PS3TMAPI.GetEventFlagListX64(target, processID, ref numEventFlags, eventFlagIDs);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetEventFlagInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetEventFlagInfoX86(int target, uint processId, uint eventFlagId, ref uint bufferSize, IntPtr buffer);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetEventFlagInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetEventFlagInfoX64(int target, uint processId, uint eventFlagId, ref uint bufferSize, IntPtr buffer);

        public static PS3TMAPI.SNRESULT GetEventFlagInfo(int target, uint processID, uint eventFlagID, out PS3TMAPI.EventFlagInfo eventFlagInfo)
        {
            eventFlagInfo = new PS3TMAPI.EventFlagInfo();
            uint bufferSize = 0;
            PS3TMAPI.SNRESULT res1 = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetEventFlagInfoX86(target, processID, eventFlagID, ref bufferSize, IntPtr.Zero) : PS3TMAPI.GetEventFlagInfoX64(target, processID, eventFlagID, ref bufferSize, IntPtr.Zero);
            if (PS3TMAPI.FAILED(res1))
                return res1;
            PS3TMAPI.ScopedGlobalHeapPtr scopedGlobalHeapPtr = new PS3TMAPI.ScopedGlobalHeapPtr(Marshal.AllocHGlobal((int)bufferSize));
            PS3TMAPI.SNRESULT res2 = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetEventFlagInfoX86(target, processID, eventFlagID, ref bufferSize, scopedGlobalHeapPtr.Get()) : PS3TMAPI.GetEventFlagInfoX64(target, processID, eventFlagID, ref bufferSize, scopedGlobalHeapPtr.Get());
            if (PS3TMAPI.FAILED(res2))
                return res2;
            IntPtr unmanagedBuf1 = PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(PS3TMAPI.ReadDataFromUnmanagedIncPtr<PS3TMAPI.EventFlagAttr>(PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(scopedGlobalHeapPtr.Get(), ref eventFlagInfo.ID), ref eventFlagInfo.Attribute), ref eventFlagInfo.BitPattern);
            uint storage = 0;
            IntPtr unmanagedBuf2 = PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(unmanagedBuf1, ref storage), ref eventFlagInfo.NumWaitAllThreads);
            eventFlagInfo.WaitingThreads = new PS3TMAPI.EventFlagWaitThread[(int)storage];
            for (int index = 0; (long)index < (long)storage; ++index)
                unmanagedBuf2 = PS3TMAPI.ReadDataFromUnmanagedIncPtr<PS3TMAPI.EventFlagWaitThread>(unmanagedBuf2, ref eventFlagInfo.WaitingThreads[index]);
            return res2;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3PickTarget", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT PickTargetX86(IntPtr hWndOwner, out int target);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3PickTarget", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT PickTargetX64(IntPtr hWndOwner, out int target);

        public static PS3TMAPI.SNRESULT PickTarget(IntPtr hWndOwner, out int target)
        {
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.PickTargetX64(hWndOwner, out target);
            return PS3TMAPI.PickTargetX86(hWndOwner, out target);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3EnableAutoStatusUpdate", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT EnableAutoStatusUpdateX86(int target, uint enabled, out uint previousState);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3EnableAutoStatusUpdate", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT EnableAutoStatusUpdateX64(int target, uint enabled, out uint previousState);

        public static PS3TMAPI.SNRESULT EnableAutoStatusUpdate(int target, bool bEnabled, out bool bPreviousState)
        {
            uint enabled = bEnabled ? 1U : 0U;
            uint previousState;
            PS3TMAPI.SNRESULT snresult = PS3TMAPI.Is32Bit() ? PS3TMAPI.EnableAutoStatusUpdateX86(target, enabled, out previousState) : PS3TMAPI.EnableAutoStatusUpdateX64(target, enabled, out previousState);
            bPreviousState = (int)previousState != 0;
            return snresult;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetPowerStatus", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetPowerStatusX86(int target, out PS3TMAPI.PowerStatus status);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetPowerStatus", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetPowerStatusX64(int target, out PS3TMAPI.PowerStatus status);

        public static PS3TMAPI.SNRESULT GetPowerStatus(int target, out PS3TMAPI.PowerStatus status)
        {
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.GetPowerStatusX64(target, out status);
            return PS3TMAPI.GetPowerStatusX86(target, out status);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3PowerOn", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT PowerOnX86(int target);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3PowerOn", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT PowerOnX64(int target);

        public static PS3TMAPI.SNRESULT PowerOn(int target)
        {
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.PowerOnX64(target);
            return PS3TMAPI.PowerOnX86(target);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3PowerOff", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT PowerOffX86(int target, uint force);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3PowerOff", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT PowerOffX64(int target, uint force);

        public static PS3TMAPI.SNRESULT PowerOff(int target, bool bForce)
        {
            uint force = bForce ? 1U : 0U;
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.PowerOffX64(target, force);
            return PS3TMAPI.PowerOffX86(target, force);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetUserMemoryStats", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetUserMemoryStatsX86(int target, uint processId, out PS3TMAPI.UserMemoryStats memoryStats);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetUserMemoryStats", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetUserMemoryStatsX64(int target, uint processId, out PS3TMAPI.UserMemoryStats memoryStats);

        public static PS3TMAPI.SNRESULT GetUserMemoryStats(int target, uint processID, out PS3TMAPI.UserMemoryStats memoryStats)
        {
            memoryStats = new PS3TMAPI.UserMemoryStats();
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.GetUserMemoryStatsX64(target, processID, out memoryStats);
            return PS3TMAPI.GetUserMemoryStatsX86(target, processID, out memoryStats);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SetDefaultLoadPriority", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT SetDefaultLoadPriorityX86(int target, uint priority);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SetDefaultLoadPriority", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT SetDefaultLoadPriorityX64(int target, uint priority);

        public static PS3TMAPI.SNRESULT SetDefaultLoadPriority(int target, uint priority)
        {
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.SetDefaultLoadPriorityX64(target, priority);
            return PS3TMAPI.SetDefaultLoadPriorityX86(target, priority);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetDefaultLoadPriority", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetDefaultLoadPriorityX86(int target, out uint priority);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetDefaultLoadPriority", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetDefaultLoadPriorityX64(int target, out uint priority);

        public static PS3TMAPI.SNRESULT GetDefaultLoadPriority(int target, out uint priority)
        {
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.GetDefaultLoadPriorityX64(target, out priority);
            return PS3TMAPI.GetDefaultLoadPriorityX86(target, out priority);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetGamePortIPAddrData", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetGamePortIPAddrDataX86(int target, string deviceName, out PS3TMAPI.GamePortIPAddressData ipAddressData);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetGamePortIPAddrData", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetGamePortIPAddrDataX64(int target, string deviceName, out PS3TMAPI.GamePortIPAddressData ipAddressData);

        public static PS3TMAPI.SNRESULT GetGamePortIPAddrData(int target, string deviceName, out PS3TMAPI.GamePortIPAddressData ipAddressData)
        {
            ipAddressData = new PS3TMAPI.GamePortIPAddressData();
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.GetGamePortIPAddrDataX64(target, deviceName, out ipAddressData);
            return PS3TMAPI.GetGamePortIPAddrDataX86(target, deviceName, out ipAddressData);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetGamePortDebugIPAddrData", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetGamePortDebugIPAddrDataX86(int target, string deviceName, out PS3TMAPI.GamePortIPAddressData data);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetGamePortDebugIPAddrData", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetGamePortDebugIPAddrDataX64(int target, string deviceName, out PS3TMAPI.GamePortIPAddressData data);

        public static PS3TMAPI.SNRESULT GetGamePortDebugIPAddrData(int target, string deviceName, out PS3TMAPI.GamePortIPAddressData ipAddressData)
        {
            ipAddressData = new PS3TMAPI.GamePortIPAddressData();
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.GetGamePortDebugIPAddrDataX64(target, deviceName, out ipAddressData);
            return PS3TMAPI.GetGamePortDebugIPAddrDataX86(target, deviceName, out ipAddressData);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SetDABR", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT SetDABRX86(int target, uint processId, ulong address);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SetDABR", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT SetDABRX64(int target, uint processId, ulong address);

        public static PS3TMAPI.SNRESULT SetDABR(int target, uint processID, ulong address)
        {
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.SetDABRX64(target, processID, address);
            return PS3TMAPI.SetDABRX86(target, processID, address);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetDABR", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetDABRX86(int target, uint processId, out ulong address);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetDABR", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetDABRX64(int target, uint processId, out ulong address);

        public static PS3TMAPI.SNRESULT GetDABR(int target, uint processID, out ulong address)
        {
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.GetDABRX64(target, processID, out address);
            return PS3TMAPI.GetDABRX86(target, processID, out address);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SetRSXProfilingFlags", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT SetRSXProfilingFlagsX86(int target, ulong rsxFlags);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SetRSXProfilingFlags", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT SetRSXProfilingFlagsX64(int target, ulong rsxFlags);

        public static PS3TMAPI.SNRESULT SetRSXProfilingFlags(int target, PS3TMAPI.RSXProfilingFlag rsxFlags)
        {
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.SetRSXProfilingFlagsX64(target, (ulong)rsxFlags);
            return PS3TMAPI.SetRSXProfilingFlagsX86(target, (ulong)rsxFlags);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetRSXProfilingFlags", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetRSXProfilingFlagsX86(int target, out ulong rsxFlags);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetRSXProfilingFlags", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetRSXProfilingFlagsX64(int target, out ulong rsxFlags);

        public static PS3TMAPI.SNRESULT GetRSXProfilingFlags(int target, out PS3TMAPI.RSXProfilingFlag rsxFlags)
        {
            ulong rsxFlags1;
            PS3TMAPI.SNRESULT snresult = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetRSXProfilingFlagsX86(target, out rsxFlags1) : PS3TMAPI.GetRSXProfilingFlagsX64(target, out rsxFlags1);
            rsxFlags = (PS3TMAPI.RSXProfilingFlag)rsxFlags1;
            return snresult;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SetCustomParamSFOMappingDirectory", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT SetCustomParamSFOMappingDirectoryX86(int target, IntPtr paramSfoDir);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SetCustomParamSFOMappingDirectory", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT SetCustomParamSFOMappingDirectoryX64(int target, IntPtr paramSfoDir);

        public static PS3TMAPI.SNRESULT SetCustomParamSFOMappingDirectory(int target, string paramSFODir)
        {
            PS3TMAPI.ScopedGlobalHeapPtr scopedGlobalHeapPtr = new PS3TMAPI.ScopedGlobalHeapPtr(PS3TMAPI.AllocUtf8FromString(paramSFODir));
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.SetCustomParamSFOMappingDirectoryX64(target, scopedGlobalHeapPtr.Get());
            return PS3TMAPI.SetCustomParamSFOMappingDirectoryX86(target, scopedGlobalHeapPtr.Get());
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3EnableXMBSettings", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT EnableXMBSettingsX86(int target, int enable);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3EnableXMBSettings", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT EnableXMBSettingsX64(int target, int enable);

        public static PS3TMAPI.SNRESULT EnableXMBSettings(int target, bool bEnable)
        {
            int enable = bEnable ? 1 : 0;
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.EnableXMBSettingsX64(target, enable);
            return PS3TMAPI.EnableXMBSettingsX86(target, enable);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetXMBSettings", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetXMBSettingsX86(int target, IntPtr buffer, ref uint bufferSize, bool bUpdateCache);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetXMBSettings", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetXMBSettingsX64(int target, IntPtr buffer, ref uint bufferSize, bool bUpdateCache);

        public static PS3TMAPI.SNRESULT GetXMBSettings(int target, out string xmbSettings, bool bUpdateCache)
        {
            xmbSettings = (string)null;
            uint bufferSize = 0;
            PS3TMAPI.SNRESULT res1 = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetXMBSettingsX86(target, IntPtr.Zero, ref bufferSize, bUpdateCache) : PS3TMAPI.GetXMBSettingsX64(target, IntPtr.Zero, ref bufferSize, bUpdateCache);
            if (PS3TMAPI.FAILED(res1))
                return res1;
            PS3TMAPI.ScopedGlobalHeapPtr scopedGlobalHeapPtr = new PS3TMAPI.ScopedGlobalHeapPtr(Marshal.AllocHGlobal((int)bufferSize));
            PS3TMAPI.SNRESULT res2 = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetXMBSettingsX86(target, scopedGlobalHeapPtr.Get(), ref bufferSize, bUpdateCache) : PS3TMAPI.GetXMBSettingsX64(target, scopedGlobalHeapPtr.Get(), ref bufferSize, bUpdateCache);
            if (PS3TMAPI.SUCCEEDED(res2))
                xmbSettings = Marshal.PtrToStringAnsi(scopedGlobalHeapPtr.Get());
            return res2;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SetXMBSettings", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT SetXMBSettingsX86(int target, string xmbSettings, bool bUpdateCache);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SetXMBSettings", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT SetXMBSettingsX64(int target, string xmbSettings, bool bUpdateCache);

        public static PS3TMAPI.SNRESULT SetXMBSettings(int target, string xmbSettings, bool bUpdateCache)
        {
            return PS3TMAPI.Is32Bit() ? PS3TMAPI.SetXMBSettingsX86(target, xmbSettings, bUpdateCache) : PS3TMAPI.SetXMBSettingsX64(target, xmbSettings, bUpdateCache);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3FootswitchControl", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT FootswitchControlX86(int target, uint enabled);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3FootswitchControl", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT FootswitchControlX64(int target, uint enabled);

        public static PS3TMAPI.SNRESULT FootswitchControl(int target, bool bEnabled)
        {
            uint enabled = bEnabled ? 1U : 0U;
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.FootswitchControlX64(target, enabled);
            return PS3TMAPI.FootswitchControlX86(target, enabled);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3TriggerCoreDump", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT TriggerCoreDumpX86(int target, uint processId, ulong userData1, ulong userData2, ulong userData3);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3TriggerCoreDump", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT TriggerCoreDumpX64(int target, uint processId, ulong userData1, ulong userData2, ulong userData3);

        public static PS3TMAPI.SNRESULT TriggerCoreDump(int target, uint processID, ulong userData1, ulong userData2, ulong userData3)
        {
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.TriggerCoreDumpX64(target, processID, userData1, userData2, userData3);
            return PS3TMAPI.TriggerCoreDumpX86(target, processID, userData1, userData2, userData3);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetCoreDumpFlags", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetCoreDumpFlagsX86(int target, out ulong flags);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetCoreDumpFlags", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetCoreDumpFlagsX64(int target, out ulong flags);

        public static PS3TMAPI.SNRESULT GetCoreDumpFlags(int target, out PS3TMAPI.CoreDumpFlag flags)
        {
            ulong flags1;
            PS3TMAPI.SNRESULT snresult = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetCoreDumpFlagsX86(target, out flags1) : PS3TMAPI.GetCoreDumpFlagsX64(target, out flags1);
            flags = (PS3TMAPI.CoreDumpFlag)flags1;
            return snresult;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SetCoreDumpFlags", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT SetCoreDumpFlagsX86(int tarSet, ulong flags);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SetCoreDumpFlags", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT SetCoreDumpFlagsX64(int tarSet, ulong flags);

        public static PS3TMAPI.SNRESULT SetCoreDumpFlags(int tarSet, PS3TMAPI.CoreDumpFlag flags)
        {
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.SetCoreDumpFlagsX64(tarSet, (ulong)flags);
            return PS3TMAPI.SetCoreDumpFlagsX86(tarSet, (ulong)flags);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ProcessAttach", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT ProcessAttachX86(int target, uint unitId, uint processId);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ProcessAttach", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT ProcessAttachX64(int target, uint unitId, uint processId);

        public static PS3TMAPI.SNRESULT ProcessAttach(int target, PS3TMAPI.UnitType unit, uint processID)
        {
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.ProcessAttachX64(target, (uint)unit, processID);
            return PS3TMAPI.ProcessAttachX86(target, (uint)unit, processID);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3FlashTarget", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT FlashTargetX86(int target, IntPtr updaterToolPath, IntPtr flashImagePath);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3FlashTarget", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT FlashTargetX64(int target, IntPtr updaterToolPath, IntPtr flashImagePath);

        public static PS3TMAPI.SNRESULT FlashTarget(int target, string updaterToolPath, string flashImagePath)
        {
            PS3TMAPI.ScopedGlobalHeapPtr scopedGlobalHeapPtr1 = new PS3TMAPI.ScopedGlobalHeapPtr(PS3TMAPI.AllocUtf8FromString(updaterToolPath));
            PS3TMAPI.ScopedGlobalHeapPtr scopedGlobalHeapPtr2 = new PS3TMAPI.ScopedGlobalHeapPtr(PS3TMAPI.AllocUtf8FromString(flashImagePath));
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.FlashTargetX64(target, scopedGlobalHeapPtr1.Get(), scopedGlobalHeapPtr2.Get());
            return PS3TMAPI.FlashTargetX86(target, scopedGlobalHeapPtr1.Get(), scopedGlobalHeapPtr2.Get());
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetMacAddress", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetMacAddressX86(int target, out IntPtr stringPtr);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetMacAddress", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetMacAddressX64(int target, out IntPtr stringPtr);

        public static PS3TMAPI.SNRESULT GetMACAddress(int target, out string macAddress)
        {
            IntPtr stringPtr;
            PS3TMAPI.SNRESULT snresult = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetMacAddressX86(target, out stringPtr) : PS3TMAPI.GetMacAddressX64(target, out stringPtr);
            macAddress = Marshal.PtrToStringAnsi(stringPtr);
            return snresult;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ProcessScatteredSetMemory", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT ProcessScatteredSetMemoryX86(int target, uint processId, uint numWrites, uint writeSize, IntPtr writeData, out uint errorCode, out uint failedAddress);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ProcessScatteredSetMemory", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT ProcessScatteredSetMemoryX64(int target, uint processId, uint numWrites, uint writeSize, IntPtr writeData, out uint errorCode, out uint failedAddress);

        public static PS3TMAPI.SNRESULT ProcessScatteredSetMemory(int target, uint processID, PS3TMAPI.ScatteredWrite[] writeData, out uint errorCode, out uint failedAddress)
        {
            errorCode = 0U;
            failedAddress = 0U;
            if (writeData == null || writeData.Length == 0)
                return PS3TMAPI.SNRESULT.SN_E_BAD_PARAM;
            int length1 = writeData.Length;
            if (writeData[0].Data == null)
                return PS3TMAPI.SNRESULT.SN_E_BAD_PARAM;
            int length2 = writeData[0].Data.Length;
            PS3TMAPI.ScopedGlobalHeapPtr scopedGlobalHeapPtr = new PS3TMAPI.ScopedGlobalHeapPtr(Marshal.AllocHGlobal(length1 * (Marshal.SizeOf((object)writeData[0].Address) + length2)));
            IntPtr num = scopedGlobalHeapPtr.Get();
            for (int index = 0; index < length1; ++index)
            {
                num = PS3TMAPI.WriteDataToUnmanagedIncPtr<uint>(writeData[index].Address, num);
                if (writeData[index].Data == null || writeData[index].Data.Length != length2)
                    return PS3TMAPI.SNRESULT.SN_E_BAD_PARAM;
                Marshal.Copy(writeData[index].Data, 0, num, writeData[index].Data.Length);
                num = new IntPtr(num.ToInt64() + (long)writeData[index].Data.Length);
            }
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.ProcessScatteredSetMemoryX64(target, processID, (uint)length1, (uint)length2, scopedGlobalHeapPtr.Get(), out errorCode, out failedAddress);
            return PS3TMAPI.ProcessScatteredSetMemoryX86(target, processID, (uint)length1, (uint)length2, scopedGlobalHeapPtr.Get(), out errorCode, out failedAddress);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetMATRanges", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetMATRangesX86(int target, uint processId, ref uint rangeCount, IntPtr matRanges);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetMATRanges", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetMATRangesX64(int target, uint processId, ref uint rangeCount, IntPtr matRanges);

        public static PS3TMAPI.SNRESULT GetMATRanges(int target, uint processID, out PS3TMAPI.MATRange[] matRanges)
        {
            matRanges = (PS3TMAPI.MATRange[])null;
            uint rangeCount = 0;
            PS3TMAPI.SNRESULT res1 = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetMATRangesX86(target, processID, ref rangeCount, IntPtr.Zero) : PS3TMAPI.GetMATRangesX64(target, processID, ref rangeCount, IntPtr.Zero);
            if (PS3TMAPI.FAILED(res1))
                return res1;
            if ((int)rangeCount == 0)
            {
                matRanges = new PS3TMAPI.MATRange[0];
                return PS3TMAPI.SNRESULT.SN_S_OK;
            }
            PS3TMAPI.ScopedGlobalHeapPtr scopedGlobalHeapPtr = new PS3TMAPI.ScopedGlobalHeapPtr(Marshal.AllocHGlobal((int)((long)(2 * Marshal.SizeOf(typeof(uint))) * (long)rangeCount)));
            PS3TMAPI.SNRESULT res2 = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetMATRangesX86(target, processID, ref rangeCount, scopedGlobalHeapPtr.Get()) : PS3TMAPI.GetMATRangesX64(target, processID, ref rangeCount, scopedGlobalHeapPtr.Get());
            if (PS3TMAPI.FAILED(res2))
                return res2;
            IntPtr unmanagedBuf = scopedGlobalHeapPtr.Get();
            matRanges = new PS3TMAPI.MATRange[(int)rangeCount];
            for (uint index = 0; index < rangeCount; ++index)
                unmanagedBuf = PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(unmanagedBuf, ref matRanges[(int)index].StartAddress), ref matRanges[(int)index].Size);
            return res2;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetMATConditions", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetMATConditionsX86(int target, uint processId, ref uint rangeCount, IntPtr ranges, ref uint bufSize, IntPtr outputBuf);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetMATConditions", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetMATConditionsX64(int target, uint processId, ref uint rangeCount, IntPtr ranges, ref uint bufSize, IntPtr outputBuf);

        public static PS3TMAPI.SNRESULT GetMATConditions(int target, uint processID, ref PS3TMAPI.MATRange[] matRanges)
        {
            if (matRanges == null || matRanges.Length < 1)
                return PS3TMAPI.SNRESULT.SN_E_BAD_PARAM;
            uint length = (uint)matRanges.Length;
            PS3TMAPI.ScopedGlobalHeapPtr scopedGlobalHeapPtr1 = new PS3TMAPI.ScopedGlobalHeapPtr(Marshal.AllocHGlobal(8 * (int)length));
            IntPtr unmanagedIncPtr1 = scopedGlobalHeapPtr1.Get();
            foreach (PS3TMAPI.MATRange matRange in matRanges)
            {
                IntPtr unmanagedIncPtr2 = PS3TMAPI.WriteDataToUnmanagedIncPtr<uint>(matRange.StartAddress, unmanagedIncPtr1);
                unmanagedIncPtr1 = PS3TMAPI.WriteDataToUnmanagedIncPtr<uint>(matRange.Size, unmanagedIncPtr2);
            }
            uint bufSize = 0;
            PS3TMAPI.SNRESULT res1 = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetMATConditionsX86(target, processID, ref length, scopedGlobalHeapPtr1.Get(), ref bufSize, IntPtr.Zero) : PS3TMAPI.GetMATConditionsX64(target, processID, ref length, scopedGlobalHeapPtr1.Get(), ref bufSize, IntPtr.Zero);
            if (PS3TMAPI.FAILED(res1))
                return res1;
            PS3TMAPI.ScopedGlobalHeapPtr scopedGlobalHeapPtr2 = new PS3TMAPI.ScopedGlobalHeapPtr(Marshal.AllocHGlobal((int)bufSize));
            PS3TMAPI.SNRESULT res2 = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetMATConditionsX86(target, processID, ref length, scopedGlobalHeapPtr1.Get(), ref bufSize, scopedGlobalHeapPtr2.Get()) : PS3TMAPI.GetMATConditionsX64(target, processID, ref length, scopedGlobalHeapPtr1.Get(), ref bufSize, scopedGlobalHeapPtr2.Get());
            if (PS3TMAPI.FAILED(res2))
                return res2;
            IntPtr unmanagedBuf = scopedGlobalHeapPtr2.Get();
            for (int index1 = 0; (long)index1 < (long)length; ++index1)
            {
                unmanagedBuf = PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(unmanagedBuf, ref matRanges[index1].StartAddress), ref matRanges[index1].Size);
                uint num = matRanges[index1].Size / 4096U;
                matRanges[index1].PageConditions = new PS3TMAPI.MATCondition[(int)num];
                for (int index2 = 0; (long)index2 < (long)num; ++index2)
                {
                    byte storage = 0;
                    unmanagedBuf = PS3TMAPI.ReadDataFromUnmanagedIncPtr<byte>(unmanagedBuf, ref storage);
                    matRanges[index1].PageConditions[index2] = (PS3TMAPI.MATCondition)storage;
                }
                bufSize -= 8U + num;
            }
            if ((int)bufSize != 0)
                res2 = PS3TMAPI.SNRESULT.SN_E_ERROR;
            return res2;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SetMATConditions", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT SetMATConditionsX86(int target, uint processId, uint rangeCount, uint bufSize, IntPtr buffer);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SetMATConditions", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT SetMATConditionsX64(int target, uint processId, uint rangeCount, uint bufSize, IntPtr buffer);

        public static PS3TMAPI.SNRESULT SetMATConditions(int target, uint processID, PS3TMAPI.MATRange[] matRanges)
        {
            if (matRanges == null || matRanges.Length < 1)
                return PS3TMAPI.SNRESULT.SN_E_BAD_PARAM;
            int length = matRanges.Length;
            int num = 0;
            foreach (PS3TMAPI.MATRange matRange in matRanges)
            {
                if (matRange.PageConditions == null)
                    return PS3TMAPI.SNRESULT.SN_E_BAD_PARAM;
                num += matRange.PageConditions.Length;
            }
            PS3TMAPI.ScopedGlobalHeapPtr scopedGlobalHeapPtr = new PS3TMAPI.ScopedGlobalHeapPtr(Marshal.AllocHGlobal(num + 2 * length * 4));
            IntPtr unmanagedIncPtr1 = scopedGlobalHeapPtr.Get();
            foreach (PS3TMAPI.MATRange matRange in matRanges)
            {
                IntPtr unmanagedIncPtr2 = PS3TMAPI.WriteDataToUnmanagedIncPtr<uint>(matRange.StartAddress, unmanagedIncPtr1);
                unmanagedIncPtr1 = PS3TMAPI.WriteDataToUnmanagedIncPtr<uint>(matRange.Size, unmanagedIncPtr2);
                foreach (byte pageCondition in matRange.PageConditions)
                    unmanagedIncPtr1 = PS3TMAPI.WriteDataToUnmanagedIncPtr<byte>(pageCondition, unmanagedIncPtr1);
            }
            uint bufSize = 1;
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.SetMATConditionsX64(target, processID, (uint)length, bufSize, scopedGlobalHeapPtr.Get());
            return PS3TMAPI.SetMATConditionsX86(target, processID, (uint)length, bufSize, scopedGlobalHeapPtr.Get());
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SaveSettings", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT SaveSettingsX86();

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SaveSettings", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT SaveSettingsX64();

        public static PS3TMAPI.SNRESULT SaveSettings()
        {
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.SaveSettingsX64();
            return PS3TMAPI.SaveSettingsX86();
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3Exit", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT ExitX86();

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3Exit", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT ExitX64();

        public static PS3TMAPI.SNRESULT Exit()
        {
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.ExitX64();
            return PS3TMAPI.ExitX86();
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ExitEx", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT ExitExX86(uint millisecondTimeout);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ExitEx", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT ExitExX64(uint millisecondTimeout);

        public static PS3TMAPI.SNRESULT ExitEx(uint millisecondTimeout)
        {
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.ExitExX64(millisecondTimeout);
            return PS3TMAPI.ExitExX86(millisecondTimeout);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3RegisterPadPlaybackNotificationHandler", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT RegisterPadPlaybackNotificationHandlerX86(int target, PS3TMAPI.HandleEventCallbackPriv callback, IntPtr userData);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3RegisterPadPlaybackNotificationHandler", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT RegisterPadPlaybackNotificationHandlerX64(int target, PS3TMAPI.HandleEventCallbackPriv callback, IntPtr userData);

        public static PS3TMAPI.SNRESULT RegisterPadPlaybackHandler(int target, PS3TMAPI.PadPlaybackCallback callback, ref object userData)
        {
            if (callback == null)
                return PS3TMAPI.SNRESULT.SN_E_BAD_PARAM;
            PS3TMAPI.SNRESULT res = PS3TMAPI.Is32Bit() ? PS3TMAPI.RegisterPadPlaybackNotificationHandlerX86(target, PS3TMAPI.ms_eventHandlerWrapper, IntPtr.Zero) : PS3TMAPI.RegisterPadPlaybackNotificationHandlerX64(target, PS3TMAPI.ms_eventHandlerWrapper, IntPtr.Zero);
            if (PS3TMAPI.SUCCEEDED(res))
            {
                PS3TMAPI.PadPlaybackCallbackAndUserData callbackAndUserData = new PS3TMAPI.PadPlaybackCallbackAndUserData();
                callbackAndUserData.m_callback = callback;
                callbackAndUserData.m_userData = userData;
                if (PS3TMAPI.ms_userPadPlaybackCallbacks == null)
                    PS3TMAPI.ms_userPadPlaybackCallbacks = new Dictionary<int, PS3TMAPI.PadPlaybackCallbackAndUserData>(1);
                PS3TMAPI.ms_userPadPlaybackCallbacks[target] = callbackAndUserData;
            }
            return res;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3UnRegisterPadPlaybackNotificationHandler", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT UnregisterPadPlaybackHandlerX86(int target);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3UnRegisterPadPlaybackNotificationHandler", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT UnregisterPadPlaybackHandlerX64(int target);

        public static PS3TMAPI.SNRESULT UnregisterPadPlaybackHandler(int target)
        {
            PS3TMAPI.SNRESULT res = PS3TMAPI.Is32Bit() ? PS3TMAPI.UnregisterPadPlaybackHandlerX86(target) : PS3TMAPI.UnregisterPadPlaybackHandlerX64(target);
            if (PS3TMAPI.SUCCEEDED(res))
            {
                if (PS3TMAPI.ms_userPadPlaybackCallbacks == null)
                    return PS3TMAPI.SNRESULT.SN_E_ERROR;
                PS3TMAPI.ms_userPadPlaybackCallbacks.Remove(target);
            }
            return res;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3StartPadPlayback", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT StartPadPlaybackX86(int target);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3StartPadPlayback", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT StartPadPlaybackX64(int target);

        public static PS3TMAPI.SNRESULT StartPadPlayback(int target)
        {
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.StartPadPlaybackX64(target);
            return PS3TMAPI.StartPadPlaybackX86(target);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3StopPadPlayback", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT StopPadPlaybackX86(int target);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3StopPadPlayback", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT StopPadPlaybackX64(int target);

        public static PS3TMAPI.SNRESULT StopPadPlayback(int target)
        {
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.StopPadPlaybackX64(target);
            return PS3TMAPI.StopPadPlaybackX86(target);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SendPadPlaybackData", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT SendPadPlaybackDataX86(int target, ref PS3TMAPI.PadData data);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SendPadPlaybackData", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT SendPadPlaybackDataX64(int target, ref PS3TMAPI.PadData data);

        public static PS3TMAPI.SNRESULT SendPadPlaybackData(int target, PS3TMAPI.PadData padData)
        {
            if (padData.buttons == null || padData.buttons.Length != 24)
                return PS3TMAPI.SNRESULT.SN_E_BAD_PARAM;
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.SendPadPlaybackDataX64(target, ref padData);
            return PS3TMAPI.SendPadPlaybackDataX86(target, ref padData);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3RegisterPadCaptureHandler", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT RegisterPadCaptureHandlerX86(int target, PS3TMAPI.HandleEventCallbackPriv callback, IntPtr userData);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3RegisterPadCaptureHandler", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT RegisterPadCaptureHandlerX64(int target, PS3TMAPI.HandleEventCallbackPriv callback, IntPtr userData);

        public static PS3TMAPI.SNRESULT RegisterPadCaptureHandler(int target, PS3TMAPI.PadCaptureCallback callback, ref object userData)
        {
            if (callback == null)
                return PS3TMAPI.SNRESULT.SN_E_BAD_PARAM;
            PS3TMAPI.SNRESULT res = PS3TMAPI.Is32Bit() ? PS3TMAPI.RegisterPadCaptureHandlerX86(target, PS3TMAPI.ms_eventHandlerWrapper, IntPtr.Zero) : PS3TMAPI.RegisterPadCaptureHandlerX64(target, PS3TMAPI.ms_eventHandlerWrapper, IntPtr.Zero);
            if (PS3TMAPI.SUCCEEDED(res))
            {
                PS3TMAPI.PadCaptureCallbackAndUserData callbackAndUserData = new PS3TMAPI.PadCaptureCallbackAndUserData();
                callbackAndUserData.m_callback = callback;
                callbackAndUserData.m_userData = userData;
                if (PS3TMAPI.ms_userPadCaptureCallbacks == null)
                    PS3TMAPI.ms_userPadCaptureCallbacks = new Dictionary<int, PS3TMAPI.PadCaptureCallbackAndUserData>(1);
                PS3TMAPI.ms_userPadCaptureCallbacks[target] = callbackAndUserData;
            }
            return res;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3UnRegisterPadCaptureHandler", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT UnregisterPadCaptureHandlerX86(int target);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3UnRegisterPadCaptureHandler", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT UnregisterPadCaptureHandlerX64(int target);

        public static PS3TMAPI.SNRESULT UnregisterPadCaptureHandler(int target)
        {
            PS3TMAPI.SNRESULT res = PS3TMAPI.Is32Bit() ? PS3TMAPI.UnregisterPadCaptureHandlerX86(target) : PS3TMAPI.UnregisterPadCaptureHandlerX64(target);
            if (PS3TMAPI.SUCCEEDED(res))
            {
                if (PS3TMAPI.ms_userPadCaptureCallbacks == null)
                    return PS3TMAPI.SNRESULT.SN_E_ERROR;
                PS3TMAPI.ms_userPadCaptureCallbacks.Remove(target);
            }
            return res;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3StartPadCapture", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT StartPadCaptureX86(int target);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3StartPadCapture", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT StartPadCaptureX64(int target);

        public static PS3TMAPI.SNRESULT StartPadCapture(int target)
        {
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.StartPadCaptureX64(target);
            return PS3TMAPI.StartPadCaptureX86(target);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3StopPadCapture", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT StopPadCaptureX86(int target);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3StopPadCapture", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT StopPadCaptureX64(int target);

        public static PS3TMAPI.SNRESULT StopPadCapture(int target)
        {
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.StopPadCaptureX64(target);
            return PS3TMAPI.StopPadCaptureX86(target);
        }

        private static void MarshalPadCaptureEvent(int target, uint param, PS3TMAPI.SNRESULT res, uint length, IntPtr data)
        {
            if ((int)length != 1)
                return;
            PS3TMAPI.PadData[] padData = new PS3TMAPI.PadData[1];
            padData[0].buttons = new short[24];
            PS3TMAPI.ReadDataFromUnmanagedIncPtr<PS3TMAPI.PadData>(data, ref padData[0]);
            if (PS3TMAPI.ms_userPadCaptureCallbacks == null)
                return;
            PS3TMAPI.ms_userPadCaptureCallbacks[target].m_callback(target, res, padData, PS3TMAPI.ms_userPadCaptureCallbacks[target].m_userData);
        }

        private static void MarshalPadPlaybackEvent(int target, uint param, PS3TMAPI.SNRESULT result, uint length, IntPtr data)
        {
            if ((int)length != 1)
                return;
            uint storage = 0;
            PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(data, ref storage);
            if (PS3TMAPI.ms_userPadPlaybackCallbacks == null)
                return;
            PS3TMAPI.ms_userPadPlaybackCallbacks[target].m_callback(target, result, (PS3TMAPI.PadPlaybackResponse)storage, PS3TMAPI.ms_userPadPlaybackCallbacks[target].m_userData);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetVRAMCaptureFlags", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetVRAMCaptureFlagsX86(int target, out ulong vramFlags);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetVRAMCaptureFlags", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetVRAMCaptureFlagsX64(int target, out ulong vramFlags);

        public static PS3TMAPI.SNRESULT GetVRAMCaptureFlags(int target, out PS3TMAPI.VRAMCaptureFlag vramFlags)
        {
            ulong vramFlags1;
            PS3TMAPI.SNRESULT snresult = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetVRAMCaptureFlagsX86(target, out vramFlags1) : PS3TMAPI.GetVRAMCaptureFlagsX64(target, out vramFlags1);
            vramFlags = (PS3TMAPI.VRAMCaptureFlag)vramFlags1;
            return snresult;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SetVRAMCaptureFlags", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT SetVRAMCaptureFlagsX86(int target, ulong vramFlags);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SetVRAMCaptureFlags", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT SetVRAMCaptureFlagsX64(int target, ulong vramFlags);

        public static PS3TMAPI.SNRESULT SetVRAMCaptureFlags(int target, PS3TMAPI.VRAMCaptureFlag vramFlags)
        {
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.SetVRAMCaptureFlagsX64(target, (ulong)vramFlags);
            return PS3TMAPI.SetVRAMCaptureFlagsX86(target, (ulong)vramFlags);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3EnableVRAMCapture", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT EnableVRAMCaptureX86(int target);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3EnableVRAMCapture", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT EnableVRAMCaptureX864(int target);

        public static PS3TMAPI.SNRESULT EnableVRAMCapture(int target)
        {
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.EnableVRAMCaptureX864(target);
            return PS3TMAPI.EnableVRAMCaptureX86(target);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetVRAMInformation", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetVRAMInformationX86(int target, uint processId, out PS3TMAPI.VramInfoPriv primaryVRAMInfo, out PS3TMAPI.VramInfoPriv secondaryVRAMInfo);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetVRAMInformation", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetVRAMInformationX64(int target, uint processId, out PS3TMAPI.VramInfoPriv primaryVRAMInfo, out PS3TMAPI.VramInfoPriv secondaryVRAMInfo);

        public static PS3TMAPI.SNRESULT GetVRAMInformation(int target, uint processID, out PS3TMAPI.VRAMInfo primaryVRAMInfo, out PS3TMAPI.VRAMInfo secondaryVRAMInfo)
        {
            primaryVRAMInfo = (PS3TMAPI.VRAMInfo)null;
            secondaryVRAMInfo = (PS3TMAPI.VRAMInfo)null;
            PS3TMAPI.VramInfoPriv primaryVRAMInfo1 = new PS3TMAPI.VramInfoPriv();
            PS3TMAPI.VramInfoPriv secondaryVRAMInfo1 = new PS3TMAPI.VramInfoPriv();
            PS3TMAPI.SNRESULT res = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetVRAMInformationX86(target, processID, out primaryVRAMInfo1, out secondaryVRAMInfo1) : PS3TMAPI.GetVRAMInformationX64(target, processID, out primaryVRAMInfo1, out secondaryVRAMInfo1);
            if (PS3TMAPI.FAILED(res))
                return res;
            primaryVRAMInfo = new PS3TMAPI.VRAMInfo();
            primaryVRAMInfo.BPAddress = primaryVRAMInfo1.BpAddress;
            primaryVRAMInfo.TopAddressPointer = primaryVRAMInfo1.TopAddressPointer;
            primaryVRAMInfo.Width = primaryVRAMInfo1.Width;
            primaryVRAMInfo.Height = primaryVRAMInfo1.Height;
            primaryVRAMInfo.Pitch = primaryVRAMInfo1.Pitch;
            primaryVRAMInfo.Colour = primaryVRAMInfo1.Colour;
            secondaryVRAMInfo = new PS3TMAPI.VRAMInfo();
            secondaryVRAMInfo.BPAddress = secondaryVRAMInfo1.BpAddress;
            secondaryVRAMInfo.TopAddressPointer = secondaryVRAMInfo1.TopAddressPointer;
            secondaryVRAMInfo.Width = secondaryVRAMInfo1.Width;
            secondaryVRAMInfo.Height = secondaryVRAMInfo1.Height;
            secondaryVRAMInfo.Pitch = secondaryVRAMInfo1.Pitch;
            secondaryVRAMInfo.Colour = secondaryVRAMInfo1.Colour;
            return res;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3VRAMCapture", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT VRAMCaptureX86(int target, uint processId, IntPtr vramInfo, IntPtr fileName);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3VRAMCapture", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT VRAMCaptureX64(int target, uint processId, IntPtr vramInfo, IntPtr fileName);

        public static PS3TMAPI.SNRESULT VRAMCapture(int target, uint processID, PS3TMAPI.VRAMInfo vramInfo, string fileName)
        {
            PS3TMAPI.ScopedGlobalHeapPtr scopedGlobalHeapPtr1 = new PS3TMAPI.ScopedGlobalHeapPtr(IntPtr.Zero);
            if (vramInfo != null)
            {
                PS3TMAPI.VramInfoPriv vramInfoPriv = new PS3TMAPI.VramInfoPriv();
                vramInfoPriv.BpAddress = vramInfo.BPAddress;
                vramInfoPriv.TopAddressPointer = vramInfo.TopAddressPointer;
                vramInfoPriv.Width = vramInfo.Width;
                vramInfoPriv.Height = vramInfo.Height;
                vramInfoPriv.Pitch = vramInfo.Pitch;
                vramInfoPriv.Colour = vramInfo.Colour;
                scopedGlobalHeapPtr1 = new PS3TMAPI.ScopedGlobalHeapPtr(Marshal.AllocHGlobal(Marshal.SizeOf((object)vramInfoPriv)));
                Marshal.StructureToPtr((object)vramInfoPriv, scopedGlobalHeapPtr1.Get(), false);
            }
            PS3TMAPI.ScopedGlobalHeapPtr scopedGlobalHeapPtr2 = new PS3TMAPI.ScopedGlobalHeapPtr(PS3TMAPI.AllocUtf8FromString(fileName));
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.VRAMCaptureX64(target, processID, scopedGlobalHeapPtr1.Get(), scopedGlobalHeapPtr2.Get());
            return PS3TMAPI.VRAMCaptureX86(target, processID, scopedGlobalHeapPtr1.Get(), scopedGlobalHeapPtr2.Get());
        }

        private static void CustomProtocolHandler(int target, PS3TMAPI.PS3Protocol ps3Protocol, IntPtr unmanagedBuf, uint length, IntPtr userData)
        {
            PS3TMAPI.PS3ProtocolPriv protocol = new PS3TMAPI.PS3ProtocolPriv(ps3Protocol.Protocol, ps3Protocol.Port);
            PS3TMAPI.CustomProtocolId key = new PS3TMAPI.CustomProtocolId(target, protocol);
            PS3TMAPI.CusProtoCallbackAndUserData callbackAndUserData;
            if (PS3TMAPI.ms_userCustomProtoCallbacks == null || !PS3TMAPI.ms_userCustomProtoCallbacks.TryGetValue(key, out callbackAndUserData))
                return;
            byte[] numArray = new byte[(int)length];
            Marshal.Copy(unmanagedBuf, numArray, 0, numArray.Length);
            callbackAndUserData.m_callback(target, ps3Protocol, numArray, callbackAndUserData.m_userData);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3RegisterCustomProtocolEx", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT RegisterCustomProtocolExX86(int target, uint protocol, uint port, string lparDesc, uint priority, out PS3TMAPI.PS3Protocol ps3Protocol, PS3TMAPI.CustomProtocolCallbackPriv callback, IntPtr userData);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3RegisterCustomProtocolEx", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT RegisterCustomProtocolExX64(int target, uint protocol, uint port, string lparDesc, uint priority, out PS3TMAPI.PS3Protocol ps3Protocol, PS3TMAPI.CustomProtocolCallbackPriv callback, IntPtr userData);

        public static PS3TMAPI.SNRESULT RegisterCustomProtocol(int target, uint protocol, uint port, string lparDesc, uint priority, out PS3TMAPI.PS3Protocol ps3Protocol, PS3TMAPI.CustomProtocolCallback callback, ref object userData)
        {
            ps3Protocol = new PS3TMAPI.PS3Protocol();
            if (callback == null)
                return PS3TMAPI.SNRESULT.SN_E_BAD_PARAM;
            PS3TMAPI.SNRESULT res = PS3TMAPI.Is32Bit() ? PS3TMAPI.RegisterCustomProtocolExX86(target, protocol, port, lparDesc, priority, out ps3Protocol, PS3TMAPI.ms_customProtoCallbackPriv, IntPtr.Zero) : PS3TMAPI.RegisterCustomProtocolExX64(target, protocol, port, lparDesc, priority, out ps3Protocol, PS3TMAPI.ms_customProtoCallbackPriv, IntPtr.Zero);
            if (PS3TMAPI.SUCCEEDED(res))
            {
                PS3TMAPI.PS3ProtocolPriv protocol1 = new PS3TMAPI.PS3ProtocolPriv(ps3Protocol.Protocol, ps3Protocol.Port);
                PS3TMAPI.CustomProtocolId index = new PS3TMAPI.CustomProtocolId(target, protocol1);
                PS3TMAPI.CusProtoCallbackAndUserData callbackAndUserData = new PS3TMAPI.CusProtoCallbackAndUserData();
                callbackAndUserData.m_callback = callback;
                callbackAndUserData.m_userData = userData;
                if (PS3TMAPI.ms_userCustomProtoCallbacks == null)
                    PS3TMAPI.ms_userCustomProtoCallbacks = new Dictionary<PS3TMAPI.CustomProtocolId, PS3TMAPI.CusProtoCallbackAndUserData>(1);
                PS3TMAPI.ms_userCustomProtoCallbacks[index] = callbackAndUserData;
            }
            return res;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3UnRegisterCustomProtocol", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT UnregisterCustomProtocolX86(int target, ref PS3TMAPI.PS3Protocol protocol);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3UnRegisterCustomProtocol", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT UnregisterCustomProtocolX64(int target, ref PS3TMAPI.PS3Protocol protocol);

        public static PS3TMAPI.SNRESULT UnregisterCustomProtocol(int target, PS3TMAPI.PS3Protocol ps3Protocol)
        {
            PS3TMAPI.SNRESULT res = PS3TMAPI.Is32Bit() ? PS3TMAPI.UnregisterCustomProtocolX86(target, ref ps3Protocol) : PS3TMAPI.UnregisterCustomProtocolX64(target, ref ps3Protocol);
            if (PS3TMAPI.SUCCEEDED(res))
            {
                PS3TMAPI.PS3ProtocolPriv protocol = new PS3TMAPI.PS3ProtocolPriv(ps3Protocol.Protocol, ps3Protocol.Port);
                PS3TMAPI.CustomProtocolId key = new PS3TMAPI.CustomProtocolId(target, protocol);
                if (PS3TMAPI.ms_userCustomProtoCallbacks == null)
                    return PS3TMAPI.SNRESULT.SN_E_ERROR;
                PS3TMAPI.ms_userCustomProtoCallbacks.Remove(key);
            }
            return res;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ForceUnRegisterCustomProtocol", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT ForceUnregisterCustomProtocolX86(int target, ref PS3TMAPI.PS3Protocol protocol);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ForceUnRegisterCustomProtocol", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT ForceUnregisterCustomProtocolX64(int target, ref PS3TMAPI.PS3Protocol protocol);

        public static PS3TMAPI.SNRESULT ForceUnregisterCustomProtocol(int target, PS3TMAPI.PS3Protocol ps3Protocol)
        {
            PS3TMAPI.SNRESULT res = PS3TMAPI.Is32Bit() ? PS3TMAPI.ForceUnregisterCustomProtocolX86(target, ref ps3Protocol) : PS3TMAPI.ForceUnregisterCustomProtocolX64(target, ref ps3Protocol);
            if (PS3TMAPI.SUCCEEDED(res))
            {
                PS3TMAPI.PS3ProtocolPriv protocol = new PS3TMAPI.PS3ProtocolPriv(ps3Protocol.Protocol, ps3Protocol.Port);
                PS3TMAPI.CustomProtocolId key = new PS3TMAPI.CustomProtocolId(target, protocol);
                if (PS3TMAPI.ms_userCustomProtoCallbacks == null)
                    return PS3TMAPI.SNRESULT.SN_E_ERROR;
                PS3TMAPI.ms_userCustomProtoCallbacks.Remove(key);
            }
            return res;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SendCustomProtocolData", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT SendCustomProtocolDataX86(int target, ref PS3TMAPI.PS3Protocol protocol, byte[] data, int length);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SendCustomProtocolData", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT SendCustomProtocolDataX64(int target, ref PS3TMAPI.PS3Protocol protocol, byte[] data, int length);

        public static PS3TMAPI.SNRESULT SendCustomProtocolData(int target, PS3TMAPI.PS3Protocol ps3Protocol, byte[] data)
        {
            if (data == null || data.Length < 1)
                return PS3TMAPI.SNRESULT.SN_E_BAD_PARAM;
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.SendCustomProtocolDataX64(target, ref ps3Protocol, data, data.Length);
            return PS3TMAPI.SendCustomProtocolDataX86(target, ref ps3Protocol, data, data.Length);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SetFileServingEventFlags", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT SetFileServingEventFlagsX86(int target, ulong eventFlags);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SetFileServingEventFlags", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT SetFileServingEventFlagsX64(int target, ulong eventFlags);

        public static PS3TMAPI.SNRESULT SetFileServingEventFlags(int target, PS3TMAPI.FileServingEventFlag eventFlags)
        {
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.SetFileServingEventFlagsX64(target, (ulong)eventFlags);
            return PS3TMAPI.SetFileServingEventFlagsX86(target, (ulong)eventFlags);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetFileServingEventFlags", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetFileServingEventFlagsX86(int target, ref ulong eventFlags);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetFileServingEventFlags", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetFileServingEventFlagsX64(int target, ref ulong eventFlags);

        public static PS3TMAPI.SNRESULT GetFileServingEventFlags(int target, out PS3TMAPI.FileServingEventFlag eventFlags)
        {
            ulong eventFlags1 = 0;
            PS3TMAPI.SNRESULT snresult = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetFileServingEventFlagsX86(target, ref eventFlags1) : PS3TMAPI.GetFileServingEventFlagsX64(target, ref eventFlags1);
            eventFlags = (PS3TMAPI.FileServingEventFlag)eventFlags1;
            return snresult;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SetCaseSensitiveFileServing", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT SetCaseSensitiveFileServingX86(int target, bool bOn, out bool bOldSetting);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SetCaseSensitiveFileServing", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT SetCaseSensitiveFileServingX64(int target, bool bOn, out bool bOldSetting);

        public static PS3TMAPI.SNRESULT SetCaseSensitiveFileServing(int target, bool bOn, out bool bOldSetting)
        {
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.SetCaseSensitiveFileServingX64(target, bOn, out bOldSetting);
            return PS3TMAPI.SetCaseSensitiveFileServingX86(target, bOn, out bOldSetting);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3RegisterFTPEventHandler", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT RegisterFTPEventHandlerX86(int target, PS3TMAPI.HandleEventCallbackPriv callback, IntPtr userData);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3RegisterFTPEventHandler", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT RegisterFTPEventHandlerX64(int target, PS3TMAPI.HandleEventCallbackPriv callback, IntPtr userData);

        public static PS3TMAPI.SNRESULT RegisterFTPEventHandler(int target, PS3TMAPI.FTPEventCallback callback, ref object userData)
        {
            if (callback == null)
                return PS3TMAPI.SNRESULT.SN_E_BAD_PARAM;
            PS3TMAPI.SNRESULT res = PS3TMAPI.Is32Bit() ? PS3TMAPI.RegisterFTPEventHandlerX86(target, PS3TMAPI.ms_eventHandlerWrapper, IntPtr.Zero) : PS3TMAPI.RegisterFTPEventHandlerX64(target, PS3TMAPI.ms_eventHandlerWrapper, IntPtr.Zero);
            if (PS3TMAPI.SUCCEEDED(res))
            {
                PS3TMAPI.FtpCallbackAndUserData callbackAndUserData = new PS3TMAPI.FtpCallbackAndUserData();
                callbackAndUserData.m_callback = callback;
                callbackAndUserData.m_userData = userData;
                if (PS3TMAPI.ms_userFtpCallbacks == null)
                    PS3TMAPI.ms_userFtpCallbacks = new Dictionary<int, PS3TMAPI.FtpCallbackAndUserData>(1);
                PS3TMAPI.ms_userFtpCallbacks[target] = callbackAndUserData;
            }
            return res;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3CancelFTPEvents", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT CancelFTPEventsX86(int target);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3CancelFTPEvents", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT CancelFTPEventsX64(int target);

        public static PS3TMAPI.SNRESULT CancelFTPEvents(int target)
        {
            PS3TMAPI.SNRESULT res = PS3TMAPI.Is32Bit() ? PS3TMAPI.CancelFTPEventsX86(target) : PS3TMAPI.CancelFTPEventsX64(target);
            if (PS3TMAPI.SUCCEEDED(res))
            {
                if (PS3TMAPI.ms_userFtpCallbacks == null)
                    return PS3TMAPI.SNRESULT.SN_E_ERROR;
                PS3TMAPI.ms_userFtpCallbacks.Remove(target);
            }
            return res;
        }

        private static void MarshalFTPEvent(int target, uint param, PS3TMAPI.SNRESULT result, uint length, IntPtr data)
        {
            PS3TMAPI.FTPNotification[] ftpNotifications = new PS3TMAPI.FTPNotification[0];
            if (length > 0U)
            {
                uint num = (uint)((ulong)length / (ulong)Marshal.SizeOf(typeof(PS3TMAPI.FTPNotification)));
                ftpNotifications = new PS3TMAPI.FTPNotification[(int)num];
                for (int index = 0; (long)index < (long)num; ++index)
                    data = PS3TMAPI.ReadDataFromUnmanagedIncPtr<PS3TMAPI.FTPNotification>(data, ref ftpNotifications[index]);
            }
            if (PS3TMAPI.ms_userFtpCallbacks == null)
                return;
            PS3TMAPI.ms_userFtpCallbacks[target].m_callback(target, result, ftpNotifications, PS3TMAPI.ms_userFtpCallbacks[target].m_userData);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3RegisterFileTraceHandler", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT RegisterFileTraceHandlerX86(int target, PS3TMAPI.HandleEventCallbackPriv callback, IntPtr userData);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3RegisterFileTraceHandler", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT RegisterFileTraceHandlerX64(int target, PS3TMAPI.HandleEventCallbackPriv callback, IntPtr userData);

        public static PS3TMAPI.SNRESULT RegisterFileTraceHandler(int target, PS3TMAPI.FileTraceCallback callback, ref object userData)
        {
            if (callback == null)
                return PS3TMAPI.SNRESULT.SN_E_BAD_PARAM;
            PS3TMAPI.SNRESULT res = PS3TMAPI.Is32Bit() ? PS3TMAPI.RegisterFileTraceHandlerX86(target, PS3TMAPI.ms_eventHandlerWrapper, IntPtr.Zero) : PS3TMAPI.RegisterFileTraceHandlerX64(target, PS3TMAPI.ms_eventHandlerWrapper, IntPtr.Zero);
            if (PS3TMAPI.SUCCEEDED(res))
            {
                PS3TMAPI.FileTraceCallbackAndUserData callbackAndUserData = new PS3TMAPI.FileTraceCallbackAndUserData();
                callbackAndUserData.m_callback = callback;
                callbackAndUserData.m_userData = userData;
                if (PS3TMAPI.ms_userFileTraceCallbacks == null)
                    PS3TMAPI.ms_userFileTraceCallbacks = new Dictionary<int, PS3TMAPI.FileTraceCallbackAndUserData>(1);
                PS3TMAPI.ms_userFileTraceCallbacks[target] = callbackAndUserData;
            }
            return res;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3UnRegisterFileTraceHandler", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT UnregisterFileTraceHandlerX86(int target);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3UnRegisterFileTraceHandler", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT UnregisterFileTraceHandlerX64(int target);

        public static PS3TMAPI.SNRESULT UnregisterFileTraceHandler(int target)
        {
            PS3TMAPI.SNRESULT res = PS3TMAPI.Is32Bit() ? PS3TMAPI.UnregisterFileTraceHandlerX86(target) : PS3TMAPI.UnregisterFileTraceHandlerX64(target);
            if (PS3TMAPI.SUCCEEDED(res))
            {
                if (PS3TMAPI.ms_userFileTraceCallbacks == null)
                    return PS3TMAPI.SNRESULT.SN_E_ERROR;
                PS3TMAPI.ms_userFileTraceCallbacks.Remove(target);
            }
            return res;
        }

        private static void MarshalFileTraceEvent(int target, uint param, PS3TMAPI.SNRESULT result, uint length, IntPtr data)
        {
            PS3TMAPI.FileTraceEvent fileTraceEvent = new PS3TMAPI.FileTraceEvent();
            IntPtr unmanagedBuf1 = data;
            uint num1 = 44;
            if (length < num1)
                return;
            IntPtr unmanagedBuf2 = PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(unmanagedBuf1, ref fileTraceEvent.SerialID);
            int storage1 = 0;
            IntPtr unmanagedBuf3 = PS3TMAPI.ReadDataFromUnmanagedIncPtr<int>(unmanagedBuf2, ref storage1);
            fileTraceEvent.TraceType = (PS3TMAPI.FileTraceType)storage1;
            int storage2 = 0;
            IntPtr unmanagedBuf4 = PS3TMAPI.ReadDataFromUnmanagedIncPtr<int>(unmanagedBuf3, ref storage2);
            fileTraceEvent.Status = (PS3TMAPI.FileTraceNotificationStatus)storage2;
            IntPtr unmanagedBuf5 = PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(unmanagedBuf4, ref fileTraceEvent.ProcessID), ref fileTraceEvent.ThreadID), ref fileTraceEvent.TimeBaseStartOfTrace), ref fileTraceEvent.TimeBase);
            uint storage3 = 0;
            IntPtr unmanagedBuf6 = PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(unmanagedBuf5, ref storage3);
            uint num2 = num1 + storage3;
            if (length < num2)
                return;
            fileTraceEvent.BackTraceData = new byte[(int)storage3];
            for (int index = 0; (long)index < (long)storage3; ++index)
                unmanagedBuf6 = PS3TMAPI.ReadDataFromUnmanagedIncPtr<byte>(unmanagedBuf6, ref fileTraceEvent.BackTraceData[index]);
            IntPtr num3;
            switch (fileTraceEvent.TraceType)
            {
                case PS3TMAPI.FileTraceType.GetBlockSize:
                case PS3TMAPI.FileTraceType.Stat:
                case PS3TMAPI.FileTraceType.WidgetStat:
                case PS3TMAPI.FileTraceType.Unlink:
                case PS3TMAPI.FileTraceType.WidgetUnlink:
                case PS3TMAPI.FileTraceType.RMDir:
                case PS3TMAPI.FileTraceType.WidgetRMDir:
                    fileTraceEvent.LogData.LogType1 = new PS3TMAPI.FileTraceLogType1();
                    uint storage4 = 0;
                    IntPtr utf8Ptr1 = PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(unmanagedBuf6, ref storage4);
                    if (storage4 > 0U)
                    {
                        fileTraceEvent.LogData.LogType1.Path = PS3TMAPI.Utf8ToString(utf8Ptr1, storage4);
                        break;
                    }
                    break;
                case PS3TMAPI.FileTraceType.Rename:
                case PS3TMAPI.FileTraceType.WidgetRename:
                    fileTraceEvent.LogData.LogType2 = new PS3TMAPI.FileTraceLogType2();
                    uint storage5 = 0;
                    IntPtr unmanagedBuf7 = PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(unmanagedBuf6, ref storage5);
                    uint storage6 = 0;
                    IntPtr utf8Ptr2 = PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(unmanagedBuf7, ref storage6);
                    if (storage5 > 0U)
                    {
                        fileTraceEvent.LogData.LogType2.Path1 = PS3TMAPI.Utf8ToString(utf8Ptr2, storage5);
                        utf8Ptr2 = new IntPtr(utf8Ptr2.ToInt64() + (long)storage5);
                    }
                    if (storage6 > 0U)
                    {
                        fileTraceEvent.LogData.LogType2.Path2 = PS3TMAPI.Utf8ToString(utf8Ptr2, storage6);
                        break;
                    }
                    break;
                case PS3TMAPI.FileTraceType.Truncate:
                case PS3TMAPI.FileTraceType.TruncateNoAlloc:
                case PS3TMAPI.FileTraceType.Truncate2:
                case PS3TMAPI.FileTraceType.Truncate2NoInit:
                    fileTraceEvent.LogData.LogType3 = new PS3TMAPI.FileTraceLogType3();
                    IntPtr unmanagedBuf8 = PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(unmanagedBuf6, ref fileTraceEvent.LogData.LogType3.Arg);
                    uint storage7 = 0;
                    IntPtr utf8Ptr3 = PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(unmanagedBuf8, ref storage7);
                    if (storage7 > 0U)
                    {
                        fileTraceEvent.LogData.LogType3.Path = PS3TMAPI.Utf8ToString(utf8Ptr3, storage7);
                        break;
                    }
                    break;
                case PS3TMAPI.FileTraceType.OpenDir:
                case PS3TMAPI.FileTraceType.WidgetOpenDir:
                case PS3TMAPI.FileTraceType.CHMod:
                case PS3TMAPI.FileTraceType.MkDir:
                    fileTraceEvent.LogData.LogType4 = new PS3TMAPI.FileTraceLogType4();
                    IntPtr unmanagedBuf9 = PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(unmanagedBuf6, ref fileTraceEvent.LogData.LogType4.Mode);
                    uint storage8 = 0;
                    IntPtr utf8Ptr4 = PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(unmanagedBuf9, ref storage8);
                    if (storage8 > 0U)
                    {
                        fileTraceEvent.LogData.LogType4.Path = PS3TMAPI.Utf8ToString(utf8Ptr4, storage8);
                        break;
                    }
                    break;
                case PS3TMAPI.FileTraceType.UTime:
                    fileTraceEvent.LogData.LogType6 = new PS3TMAPI.FileTraceLogType6();
                    IntPtr unmanagedBuf10 = PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(unmanagedBuf6, ref fileTraceEvent.LogData.LogType6.Arg1), ref fileTraceEvent.LogData.LogType6.Arg2);
                    uint storage9 = 0;
                    IntPtr utf8Ptr5 = PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(unmanagedBuf10, ref storage9);
                    if (storage9 > 0U)
                    {
                        fileTraceEvent.LogData.LogType6.Path = PS3TMAPI.Utf8ToString(utf8Ptr5, storage9);
                        break;
                    }
                    break;
                case PS3TMAPI.FileTraceType.Open:
                case PS3TMAPI.FileTraceType.WidgetOpen:
                    fileTraceEvent.LogData.LogType8 = new PS3TMAPI.FileTraceLogType8();
                    IntPtr unmanagedBuf11 = PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(PS3TMAPI.ReadDataFromUnmanagedIncPtr<PS3TMAPI.FileTraceProcessInfo>(unmanagedBuf6, ref fileTraceEvent.LogData.LogType8.ProcessInfo), ref fileTraceEvent.LogData.LogType8.Arg1), ref fileTraceEvent.LogData.LogType8.Arg2), ref fileTraceEvent.LogData.LogType8.Arg3), ref fileTraceEvent.LogData.LogType8.Arg4);
                    uint storage10 = 0;
                    IntPtr unmanagedBuf12 = PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(unmanagedBuf11, ref storage10);
                    uint storage11 = 0;
                    IntPtr num4 = PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(unmanagedBuf12, ref storage11);
                    fileTraceEvent.LogData.LogType8.VArg = new byte[(int)storage10];
                    for (int index = 0; (long)index < (long)storage10; ++index)
                        num4 = PS3TMAPI.ReadDataFromUnmanagedIncPtr<byte>(num4, ref fileTraceEvent.LogData.LogType8.VArg[index]);
                    if (storage11 > 0U)
                    {
                        fileTraceEvent.LogData.LogType8.Path = PS3TMAPI.Utf8ToString(num4, storage11);
                        break;
                    }
                    break;
                case PS3TMAPI.FileTraceType.Close:
                case PS3TMAPI.FileTraceType.CloseDir:
                case PS3TMAPI.FileTraceType.FSync:
                case PS3TMAPI.FileTraceType.ReadDir:
                case PS3TMAPI.FileTraceType.FStat:
                case PS3TMAPI.FileTraceType.FGetBlockSize:
                    fileTraceEvent.LogData.LogType9 = new PS3TMAPI.FileTraceLogType9();
                    num3 = PS3TMAPI.ReadDataFromUnmanagedIncPtr<PS3TMAPI.FileTraceProcessInfo>(unmanagedBuf6, ref fileTraceEvent.LogData.LogType9.ProcessInfo);
                    break;
                case PS3TMAPI.FileTraceType.Read:
                case PS3TMAPI.FileTraceType.Write:
                case PS3TMAPI.FileTraceType.GetDirEntries:
                    fileTraceEvent.LogData.LogType10 = new PS3TMAPI.FileTraceLogType10();
                    num3 = PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(PS3TMAPI.ReadDataFromUnmanagedIncPtr<PS3TMAPI.FileTraceProcessInfo>(unmanagedBuf6, ref fileTraceEvent.LogData.LogType10.ProcessInfo), ref fileTraceEvent.LogData.LogType10.Size), ref fileTraceEvent.LogData.LogType10.Address), ref fileTraceEvent.LogData.LogType10.TxSize);
                    break;
                case PS3TMAPI.FileTraceType.ReadOffset:
                case PS3TMAPI.FileTraceType.WriteOffset:
                    fileTraceEvent.LogData.LogType11 = new PS3TMAPI.FileTraceLogType11();
                    PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(PS3TMAPI.ReadDataFromUnmanagedIncPtr<PS3TMAPI.FileTraceProcessInfo>(unmanagedBuf6, ref fileTraceEvent.LogData.LogType11.ProcessInfo), ref fileTraceEvent.LogData.LogType11.Size), ref fileTraceEvent.LogData.LogType11.Address), ref fileTraceEvent.LogData.LogType11.Offset), ref fileTraceEvent.LogData.LogType11.TxSize);
                    break;
                case PS3TMAPI.FileTraceType.FTruncate:
                case PS3TMAPI.FileTraceType.FTruncateNoAlloc:
                    fileTraceEvent.LogData.LogType12 = new PS3TMAPI.FileTraceLogType12();
                    PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(PS3TMAPI.ReadDataFromUnmanagedIncPtr<PS3TMAPI.FileTraceProcessInfo>(unmanagedBuf6, ref fileTraceEvent.LogData.LogType12.ProcessInfo), ref fileTraceEvent.LogData.LogType12.TargetSize);
                    break;
                case PS3TMAPI.FileTraceType.LSeek:
                    fileTraceEvent.LogData.LogType13 = new PS3TMAPI.FileTraceLogType13();
                    PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(PS3TMAPI.ReadDataFromUnmanagedIncPtr<PS3TMAPI.FileTraceProcessInfo>(unmanagedBuf6, ref fileTraceEvent.LogData.LogType13.ProcessInfo), ref fileTraceEvent.LogData.LogType13.Size), ref fileTraceEvent.LogData.LogType13.Offset), ref fileTraceEvent.LogData.LogType13.CurPos);
                    break;
                case PS3TMAPI.FileTraceType.SetIOBuffer:
                    fileTraceEvent.LogData.LogType14 = new PS3TMAPI.FileTraceLogType14();
                    PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(PS3TMAPI.ReadDataFromUnmanagedIncPtr<PS3TMAPI.FileTraceProcessInfo>(unmanagedBuf6, ref fileTraceEvent.LogData.LogType14.ProcessInfo), ref fileTraceEvent.LogData.LogType14.MaxSize), ref fileTraceEvent.LogData.LogType14.Page), ref fileTraceEvent.LogData.LogType14.ContainerID);
                    break;
            }
            if (PS3TMAPI.ms_userFileTraceCallbacks == null)
                return;
            PS3TMAPI.ms_userFileTraceCallbacks[target].m_callback(target, result, fileTraceEvent, PS3TMAPI.ms_userFileTraceCallbacks[target].m_userData);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3StartFileTrace", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT StartFileTraceX86(int target, uint processId, uint size, IntPtr filename);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3StartFileTrace", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT StartFileTraceX64(int target, uint processId, uint size, IntPtr filename);

        public static PS3TMAPI.SNRESULT StartFileTrace(int target, uint processID, uint size, string filename)
        {
            PS3TMAPI.ScopedGlobalHeapPtr scopedGlobalHeapPtr = new PS3TMAPI.ScopedGlobalHeapPtr(PS3TMAPI.AllocUtf8FromString(filename));
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.StartFileTraceX64(target, processID, size, scopedGlobalHeapPtr.Get());
            return PS3TMAPI.StartFileTraceX86(target, processID, size, scopedGlobalHeapPtr.Get());
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3StopFileTrace", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT StopFileTraceX86(int target, uint processId);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3StopFileTrace", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT StopFileTraceX64(int target, uint processId);

        public static PS3TMAPI.SNRESULT StopFileTrace(int target, uint processID)
        {
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.StopFileTraceX64(target, processID);
            return PS3TMAPI.StopFileTraceX86(target, processID);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3InstallPackage", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT InstallPackageX86(int target, IntPtr packagePath);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3InstallPackage", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT InstallPackageX64(int target, IntPtr packagePath);

        public static PS3TMAPI.SNRESULT InstallPackage(int target, string packagePath)
        {
            PS3TMAPI.ScopedGlobalHeapPtr scopedGlobalHeapPtr = new PS3TMAPI.ScopedGlobalHeapPtr(PS3TMAPI.AllocUtf8FromString(packagePath));
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.InstallPackageX64(target, scopedGlobalHeapPtr.Get());
            return PS3TMAPI.InstallPackageX86(target, scopedGlobalHeapPtr.Get());
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3UploadFile", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT UploadFileX86(int target, IntPtr source, IntPtr dest, out uint transactionId);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3UploadFile", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT UploadFileX64(int target, IntPtr source, IntPtr dest, out uint transactionId);

        public static PS3TMAPI.SNRESULT UploadFile(int target, string source, string dest, out uint txID)
        {
            PS3TMAPI.ScopedGlobalHeapPtr scopedGlobalHeapPtr1 = new PS3TMAPI.ScopedGlobalHeapPtr(PS3TMAPI.AllocUtf8FromString(source));
            PS3TMAPI.ScopedGlobalHeapPtr scopedGlobalHeapPtr2 = new PS3TMAPI.ScopedGlobalHeapPtr(PS3TMAPI.AllocUtf8FromString(dest));
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.UploadFileX64(target, scopedGlobalHeapPtr1.Get(), scopedGlobalHeapPtr2.Get(), out txID);
            return PS3TMAPI.UploadFileX86(target, scopedGlobalHeapPtr1.Get(), scopedGlobalHeapPtr2.Get(), out txID);
        }

        private static IntPtr FileTransferInfoMarshalHelper(IntPtr dataPtr, ref PS3TMAPI.FileTransferInfo fileTransferInfo)
        {
            PS3TMAPI.FileTransferInfoPriv storage = new PS3TMAPI.FileTransferInfoPriv();
            dataPtr = PS3TMAPI.ReadDataFromUnmanagedIncPtr<PS3TMAPI.FileTransferInfoPriv>(dataPtr, ref storage);
            fileTransferInfo.TransferID = storage.TransferId;
            fileTransferInfo.Status = (PS3TMAPI.FileTransferStatus)storage.Status;
            fileTransferInfo.Size = storage.Size;
            fileTransferInfo.BytesRead = storage.BytesRead;
            fileTransferInfo.SourcePath = PS3TMAPI.Utf8FixedSizeByteArrayToString(storage.SourcePath);
            fileTransferInfo.DestinationPath = PS3TMAPI.Utf8FixedSizeByteArrayToString(storage.DestinationPath);
            return dataPtr;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetFileTransferList", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetFileTransferListX86(int target, ref uint count, IntPtr fileTransferInfo);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetFileTransferList", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetFileTransferListX64(int target, ref uint count, IntPtr fileTransferInfo);

        public static PS3TMAPI.SNRESULT GetFileTransferList(int target, out PS3TMAPI.FileTransferInfo[] fileTransfers)
        {
            fileTransfers = (PS3TMAPI.FileTransferInfo[])null;
            uint count = 0;
            PS3TMAPI.SNRESULT res1 = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetFileTransferListX86(target, ref count, IntPtr.Zero) : PS3TMAPI.GetFileTransferListX64(target, ref count, IntPtr.Zero);
            if (PS3TMAPI.FAILED(res1))
                return res1;
            PS3TMAPI.ScopedGlobalHeapPtr scopedGlobalHeapPtr = new PS3TMAPI.ScopedGlobalHeapPtr(Marshal.AllocHGlobal((int)((long)Marshal.SizeOf(typeof(PS3TMAPI.FileTransferInfoPriv)) * (long)count)));
            PS3TMAPI.SNRESULT res2 = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetFileTransferListX86(target, ref count, scopedGlobalHeapPtr.Get()) : PS3TMAPI.GetFileTransferListX64(target, ref count, scopedGlobalHeapPtr.Get());
            if (PS3TMAPI.FAILED(res2))
                return res2;
            IntPtr dataPtr = scopedGlobalHeapPtr.Get();
            fileTransfers = new PS3TMAPI.FileTransferInfo[(int)count];
            for (uint index = 0; index < count; ++index)
                dataPtr = PS3TMAPI.FileTransferInfoMarshalHelper(dataPtr, ref fileTransfers[(int)index]);
            return res2;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetFileTransferInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetFileTransferInfoX86(int target, uint txId, IntPtr fileTransferInfoPtr);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetFileTransferInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetFileTransferInfoX64(int target, uint txId, IntPtr fileTransferInfoPtr);

        public static PS3TMAPI.SNRESULT GetFileTransferInfo(int target, uint txID, out PS3TMAPI.FileTransferInfo fileTransferInfo)
        {
            fileTransferInfo = new PS3TMAPI.FileTransferInfo();
            PS3TMAPI.ScopedGlobalHeapPtr scopedGlobalHeapPtr = new PS3TMAPI.ScopedGlobalHeapPtr(Marshal.AllocHGlobal(Marshal.SizeOf(typeof(PS3TMAPI.FileTransferInfoPriv))));
            PS3TMAPI.SNRESULT res = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetFileTransferInfoX86(target, txID, scopedGlobalHeapPtr.Get()) : PS3TMAPI.GetFileTransferInfoX64(target, txID, scopedGlobalHeapPtr.Get());
            if (PS3TMAPI.SUCCEEDED(res))
                PS3TMAPI.FileTransferInfoMarshalHelper(scopedGlobalHeapPtr.Get(), ref fileTransferInfo);
            return res;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3CancelFileTransfer", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT CancelFileTransferX86(int target, uint txID);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3CancelFileTransfer", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT CancelFileTransferX64(int target, uint txID);

        public static PS3TMAPI.SNRESULT CancelFileTransfer(int target, uint txID)
        {
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.CancelFileTransferX64(target, txID);
            return PS3TMAPI.CancelFileTransferX86(target, txID);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3RetryFileTransfer", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT RetryFileTransferX86(int target, uint txID, bool bForce);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3RetryFileTransfer", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT RetryFileTransferX64(int target, uint txID, bool bForce);

        public static PS3TMAPI.SNRESULT RetryFileTransfer(int target, uint txID, bool bForce)
        {
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.RetryFileTransferX64(target, txID, bForce);
            return PS3TMAPI.RetryFileTransferX86(target, txID, bForce);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3RemoveTransferItemsByStatus", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT RemoveTransferItemsByStatusX86(int target, uint filter);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3RemoveTransferItemsByStatus", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT SNPS3RemoveTransferItemsByStatusX64(int target, uint filter);

        public static PS3TMAPI.SNRESULT RemoveTransferItemsByStatus(int target, uint filter)
        {
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.SNPS3RemoveTransferItemsByStatusX64(target, filter);
            return PS3TMAPI.RemoveTransferItemsByStatusX86(target, filter);
        }

        private static IntPtr DirEntryMarshalHelper(IntPtr dataPtr, ref PS3TMAPI.DirEntry dirEntry)
        {
            PS3TMAPI.DirEntryPriv storage = new PS3TMAPI.DirEntryPriv();
            dataPtr = PS3TMAPI.ReadDataFromUnmanagedIncPtr<PS3TMAPI.DirEntryPriv>(dataPtr, ref storage);
            dirEntry.Type = (PS3TMAPI.DirEntryType)storage.Type;
            dirEntry.Mode = storage.Mode;
            dirEntry.AccessTime = storage.AccessTime;
            dirEntry.ModifiedTime = storage.ModifiedTime;
            dirEntry.CreateTime = storage.CreateTime;
            dirEntry.Size = storage.Size;
            dirEntry.Name = PS3TMAPI.Utf8FixedSizeByteArrayToString(storage.Name);
            return dataPtr;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetDirectoryList", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetDirectoryListX86(int target, IntPtr directory, ref uint numDirEntries, IntPtr dirEntryList);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetDirectoryList", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetDirectoryListX64(int target, IntPtr directory, ref uint numDirEntries, IntPtr dirEntryList);

        public static PS3TMAPI.SNRESULT GetDirectoryList(int target, string directory, out PS3TMAPI.DirEntry[] dirEntries)
        {
            dirEntries = (PS3TMAPI.DirEntry[])null;
            uint numDirEntries = 0;
            PS3TMAPI.ScopedGlobalHeapPtr scopedGlobalHeapPtr1 = new PS3TMAPI.ScopedGlobalHeapPtr(PS3TMAPI.AllocUtf8FromString(directory));
            PS3TMAPI.SNRESULT res1 = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetDirectoryListX86(target, scopedGlobalHeapPtr1.Get(), ref numDirEntries, IntPtr.Zero) : PS3TMAPI.GetDirectoryListX64(target, scopedGlobalHeapPtr1.Get(), ref numDirEntries, IntPtr.Zero);
            if (PS3TMAPI.FAILED(res1))
                return res1;
            PS3TMAPI.ScopedGlobalHeapPtr scopedGlobalHeapPtr2 = new PS3TMAPI.ScopedGlobalHeapPtr(Marshal.AllocHGlobal((int)numDirEntries * Marshal.SizeOf(typeof(PS3TMAPI.DirEntryPriv))));
            PS3TMAPI.SNRESULT res2 = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetDirectoryListX86(target, scopedGlobalHeapPtr1.Get(), ref numDirEntries, scopedGlobalHeapPtr2.Get()) : PS3TMAPI.GetDirectoryListX64(target, scopedGlobalHeapPtr1.Get(), ref numDirEntries, scopedGlobalHeapPtr2.Get());
            if (PS3TMAPI.SUCCEEDED(res2))
            {
                IntPtr dataPtr = scopedGlobalHeapPtr2.Get();
                dirEntries = new PS3TMAPI.DirEntry[(int)numDirEntries];
                for (int index = 0; (long)index < (long)numDirEntries; ++index)
                    dataPtr = PS3TMAPI.DirEntryMarshalHelper(dataPtr, ref dirEntries[index]);
            }
            return res2;
        }

        private static IntPtr DirEntryExMarshalHelper(IntPtr dataPtr, ref PS3TMAPI.DirEntryEx dirEntryEx)
        {
            PS3TMAPI.DirEntryExPriv storage = new PS3TMAPI.DirEntryExPriv();
            dataPtr = PS3TMAPI.ReadDataFromUnmanagedIncPtr<PS3TMAPI.DirEntryExPriv>(dataPtr, ref storage);
            dirEntryEx.Type = (PS3TMAPI.DirEntryType)storage.Type;
            dirEntryEx.Mode = storage.Mode;
            dirEntryEx.AccessTimeUTC = storage.AccessTimeUTC;
            dirEntryEx.ModifiedTimeUTC = storage.ModifiedTimeUTC;
            dirEntryEx.CreateTimeUTC = storage.CreateTimeUTC;
            dirEntryEx.Size = storage.Size;
            dirEntryEx.Name = PS3TMAPI.Utf8FixedSizeByteArrayToString(storage.Name);
            return dataPtr;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetDirectoryListEx", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetDirectoryListExX86(int target, IntPtr dirPtr, ref uint numDirEntries, IntPtr dirEntryListEx, ref PS3TMAPI.TargetTimezone timeZone);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetDirectoryListEx", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetDirectoryListExX64(int target, IntPtr dirPtr, ref uint numDirEntries, IntPtr dirEntryListEx, ref PS3TMAPI.TargetTimezone timeZone);

        public static PS3TMAPI.SNRESULT GetDirectoryListEx(int target, string directory, out PS3TMAPI.DirEntryEx[] dirEntries, out PS3TMAPI.TargetTimezone timeZone)
        {
            dirEntries = (PS3TMAPI.DirEntryEx[])null;
            timeZone = new PS3TMAPI.TargetTimezone();
            uint numDirEntries = 0;
            PS3TMAPI.ScopedGlobalHeapPtr scopedGlobalHeapPtr1 = new PS3TMAPI.ScopedGlobalHeapPtr(PS3TMAPI.AllocUtf8FromString(directory));
            PS3TMAPI.SNRESULT res1 = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetDirectoryListExX86(target, scopedGlobalHeapPtr1.Get(), ref numDirEntries, IntPtr.Zero, ref timeZone) : PS3TMAPI.GetDirectoryListExX64(target, scopedGlobalHeapPtr1.Get(), ref numDirEntries, IntPtr.Zero, ref timeZone);
            if (PS3TMAPI.FAILED(res1))
                return res1;
            PS3TMAPI.ScopedGlobalHeapPtr scopedGlobalHeapPtr2 = new PS3TMAPI.ScopedGlobalHeapPtr(Marshal.AllocHGlobal((int)numDirEntries * Marshal.SizeOf(typeof(PS3TMAPI.DirEntryExPriv))));
            PS3TMAPI.SNRESULT res2 = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetDirectoryListExX86(target, scopedGlobalHeapPtr1.Get(), ref numDirEntries, scopedGlobalHeapPtr2.Get(), ref timeZone) : PS3TMAPI.GetDirectoryListExX64(target, scopedGlobalHeapPtr1.Get(), ref numDirEntries, scopedGlobalHeapPtr2.Get(), ref timeZone);
            if (PS3TMAPI.SUCCEEDED(res2))
            {
                IntPtr dataPtr = scopedGlobalHeapPtr2.Get();
                dirEntries = new PS3TMAPI.DirEntryEx[(int)numDirEntries];
                for (int index = 0; (long)index < (long)numDirEntries; ++index)
                    dataPtr = PS3TMAPI.DirEntryExMarshalHelper(dataPtr, ref dirEntries[index]);
            }
            return res2;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3MakeDirectory", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT MakeDirectoryX86(int target, IntPtr directory, uint mode);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3MakeDirectory", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT MakeDirectoryX64(int target, IntPtr directory, uint mode);

        public static PS3TMAPI.SNRESULT MakeDirectory(int target, string directory, uint mode)
        {
            PS3TMAPI.ScopedGlobalHeapPtr scopedGlobalHeapPtr = new PS3TMAPI.ScopedGlobalHeapPtr(PS3TMAPI.AllocUtf8FromString(directory));
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.MakeDirectoryX64(target, scopedGlobalHeapPtr.Get(), mode);
            return PS3TMAPI.MakeDirectoryX86(target, scopedGlobalHeapPtr.Get(), mode);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3Delete", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT DeleteFileX86(int target, IntPtr path);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3Delete", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT DeleteFileX64(int target, IntPtr path);

        public static PS3TMAPI.SNRESULT DeleteFile(int target, string path)
        {
            PS3TMAPI.ScopedGlobalHeapPtr scopedGlobalHeapPtr = new PS3TMAPI.ScopedGlobalHeapPtr(PS3TMAPI.AllocUtf8FromString(path));
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.DeleteFileX64(target, scopedGlobalHeapPtr.Get());
            return PS3TMAPI.DeleteFileX86(target, scopedGlobalHeapPtr.Get());
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3DeleteEx", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT DeleteFileExX86(int target, IntPtr path, uint msTimeout);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3DeleteEx", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT DeleteFileExX64(int target, IntPtr path, uint msTimeout);

        public static PS3TMAPI.SNRESULT DeleteFileEx(int target, string path, uint msTimeout)
        {
            PS3TMAPI.ScopedGlobalHeapPtr scopedGlobalHeapPtr = new PS3TMAPI.ScopedGlobalHeapPtr(PS3TMAPI.AllocUtf8FromString(path));
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.DeleteFileExX64(target, scopedGlobalHeapPtr.Get(), msTimeout);
            return PS3TMAPI.DeleteFileExX86(target, scopedGlobalHeapPtr.Get(), msTimeout);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3Rename", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT RenameFileX86(int target, IntPtr source, IntPtr dest);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3Rename", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT RenameFileX64(int target, IntPtr source, IntPtr dest);

        public static PS3TMAPI.SNRESULT RenameFile(int target, string source, string dest)
        {
            PS3TMAPI.ScopedGlobalHeapPtr scopedGlobalHeapPtr1 = new PS3TMAPI.ScopedGlobalHeapPtr(PS3TMAPI.AllocUtf8FromString(source));
            PS3TMAPI.ScopedGlobalHeapPtr scopedGlobalHeapPtr2 = new PS3TMAPI.ScopedGlobalHeapPtr(PS3TMAPI.AllocUtf8FromString(dest));
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.RenameFileX64(target, scopedGlobalHeapPtr1.Get(), scopedGlobalHeapPtr2.Get());
            return PS3TMAPI.RenameFileX86(target, scopedGlobalHeapPtr1.Get(), scopedGlobalHeapPtr2.Get());
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3DownloadFile", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT DownloadFileX86(int target, IntPtr source, IntPtr dest, out uint transactionId);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3DownloadFile", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT DownloadFileX64(int target, IntPtr source, IntPtr dest, out uint transactionId);

        public static PS3TMAPI.SNRESULT DownloadFile(int target, string source, string dest, out uint txID)
        {
            PS3TMAPI.ScopedGlobalHeapPtr scopedGlobalHeapPtr1 = new PS3TMAPI.ScopedGlobalHeapPtr(PS3TMAPI.AllocUtf8FromString(source));
            PS3TMAPI.ScopedGlobalHeapPtr scopedGlobalHeapPtr2 = new PS3TMAPI.ScopedGlobalHeapPtr(PS3TMAPI.AllocUtf8FromString(dest));
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.DownloadFileX64(target, scopedGlobalHeapPtr1.Get(), scopedGlobalHeapPtr2.Get(), out txID);
            return PS3TMAPI.DownloadFileX86(target, scopedGlobalHeapPtr1.Get(), scopedGlobalHeapPtr2.Get(), out txID);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3DownloadDirectory", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT DownloadDirectoryX86(int target, IntPtr source, IntPtr dest, out uint lastTransactionId);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3DownloadDirectory", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT DownloadDirectoryX64(int target, IntPtr source, IntPtr dest, out uint lastTransactionId);

        public static PS3TMAPI.SNRESULT DownloadDirectory(int target, string source, string dest, out uint lastTxID)
        {
            PS3TMAPI.ScopedGlobalHeapPtr scopedGlobalHeapPtr1 = new PS3TMAPI.ScopedGlobalHeapPtr(PS3TMAPI.AllocUtf8FromString(source));
            PS3TMAPI.ScopedGlobalHeapPtr scopedGlobalHeapPtr2 = new PS3TMAPI.ScopedGlobalHeapPtr(PS3TMAPI.AllocUtf8FromString(dest));
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.DownloadDirectoryX64(target, scopedGlobalHeapPtr1.Get(), scopedGlobalHeapPtr2.Get(), out lastTxID);
            return PS3TMAPI.DownloadDirectoryX86(target, scopedGlobalHeapPtr1.Get(), scopedGlobalHeapPtr2.Get(), out lastTxID);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3UploadDirectory", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT UploadDirectoryX86(int target, IntPtr source, IntPtr dest, out uint lastTransactionId);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3UploadDirectory", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT UploadDirectoryX64(int target, IntPtr source, IntPtr dest, out uint lastTransactionId);

        public static PS3TMAPI.SNRESULT UploadDirectory(int target, string source, string dest, out uint lastTxID)
        {
            PS3TMAPI.ScopedGlobalHeapPtr scopedGlobalHeapPtr1 = new PS3TMAPI.ScopedGlobalHeapPtr(PS3TMAPI.AllocUtf8FromString(source));
            PS3TMAPI.ScopedGlobalHeapPtr scopedGlobalHeapPtr2 = new PS3TMAPI.ScopedGlobalHeapPtr(PS3TMAPI.AllocUtf8FromString(dest));
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.UploadDirectoryX64(target, scopedGlobalHeapPtr1.Get(), scopedGlobalHeapPtr2.Get(), out lastTxID);
            return PS3TMAPI.UploadDirectoryX86(target, scopedGlobalHeapPtr1.Get(), scopedGlobalHeapPtr2.Get(), out lastTxID);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3StatTargetFile", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT StatTargetFileX86(int target, IntPtr file, IntPtr dirEntry);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3StatTargetFile", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT StatTargetFileX64(int target, IntPtr file, IntPtr dirEntry);

        public static PS3TMAPI.SNRESULT StatTargetFile(int target, string file, out PS3TMAPI.DirEntry dirEntry)
        {
            dirEntry = new PS3TMAPI.DirEntry();
            PS3TMAPI.ScopedGlobalHeapPtr scopedGlobalHeapPtr1 = new PS3TMAPI.ScopedGlobalHeapPtr(PS3TMAPI.AllocUtf8FromString(file));
            PS3TMAPI.ScopedGlobalHeapPtr scopedGlobalHeapPtr2 = new PS3TMAPI.ScopedGlobalHeapPtr(Marshal.AllocHGlobal(Marshal.SizeOf(typeof(PS3TMAPI.DirEntryPriv))));
            PS3TMAPI.SNRESULT snresult = PS3TMAPI.Is32Bit() ? PS3TMAPI.StatTargetFileX86(target, scopedGlobalHeapPtr1.Get(), scopedGlobalHeapPtr2.Get()) : PS3TMAPI.StatTargetFileX64(target, scopedGlobalHeapPtr1.Get(), scopedGlobalHeapPtr2.Get());
            PS3TMAPI.DirEntryMarshalHelper(scopedGlobalHeapPtr2.Get(), ref dirEntry);
            return snresult;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3StatTargetFileEx", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT StatTargetFileExX86(int target, IntPtr file, IntPtr dirEntry, out PS3TMAPI.TargetTimezone timeZone);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3StatTargetFileEx", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT StatTargetFileExX64(int target, IntPtr file, IntPtr dirEntry, out PS3TMAPI.TargetTimezone timeZone);

        public static PS3TMAPI.SNRESULT StatTargetFileEx(int target, string file, out PS3TMAPI.DirEntryEx dirEntryEx, out PS3TMAPI.TargetTimezone timeZone)
        {
            dirEntryEx = new PS3TMAPI.DirEntryEx();
            PS3TMAPI.ScopedGlobalHeapPtr scopedGlobalHeapPtr1 = new PS3TMAPI.ScopedGlobalHeapPtr(PS3TMAPI.AllocUtf8FromString(file));
            PS3TMAPI.ScopedGlobalHeapPtr scopedGlobalHeapPtr2 = new PS3TMAPI.ScopedGlobalHeapPtr(Marshal.AllocHGlobal(Marshal.SizeOf(typeof(PS3TMAPI.DirEntryExPriv))));
            PS3TMAPI.SNRESULT snresult = PS3TMAPI.Is32Bit() ? PS3TMAPI.StatTargetFileExX86(target, scopedGlobalHeapPtr1.Get(), scopedGlobalHeapPtr2.Get(), out timeZone) : PS3TMAPI.StatTargetFileExX64(target, scopedGlobalHeapPtr1.Get(), scopedGlobalHeapPtr2.Get(), out timeZone);
            PS3TMAPI.DirEntryExMarshalHelper(scopedGlobalHeapPtr2.Get(), ref dirEntryEx);
            return snresult;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3CHMod", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT CHModX86(int target, IntPtr filePath, uint mode);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3CHMod", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT CHModX64(int target, IntPtr filePath, uint mode);

        public static PS3TMAPI.SNRESULT ChMod(int target, string filePath, PS3TMAPI.ChModFilePermission mode)
        {
            PS3TMAPI.ScopedGlobalHeapPtr scopedGlobalHeapPtr = new PS3TMAPI.ScopedGlobalHeapPtr(PS3TMAPI.AllocUtf8FromString(filePath));
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.CHModX64(target, scopedGlobalHeapPtr.Get(), (uint)mode);
            return PS3TMAPI.CHModX86(target, scopedGlobalHeapPtr.Get(), (uint)mode);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SetFileTime", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT SetFileTimeX86(int target, IntPtr filePath, ulong accessTime, ulong modifiedTime);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SetFileTime", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT SetFileTimeX64(int target, IntPtr filePath, ulong accessTime, ulong modifiedTime);

        public static PS3TMAPI.SNRESULT SetFileTime(int target, string filePath, ulong accessTime, ulong modifiedTime)
        {
            PS3TMAPI.ScopedGlobalHeapPtr scopedGlobalHeapPtr = new PS3TMAPI.ScopedGlobalHeapPtr(PS3TMAPI.AllocUtf8FromString(filePath));
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.SetFileTimeX64(target, scopedGlobalHeapPtr.Get(), accessTime, modifiedTime);
            return PS3TMAPI.SetFileTimeX86(target, scopedGlobalHeapPtr.Get(), accessTime, modifiedTime);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3InstallGameEx", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT InstallGameExX86(int target, IntPtr paramSfoPath, out IntPtr titleId, out IntPtr targetPath, out uint txId);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3InstallGameEx", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT InstallGameExX64(int target, IntPtr paramSfoPath, out IntPtr titleId, out IntPtr targetPath, out uint txId);

        public static PS3TMAPI.SNRESULT InstallGameEx(int target, string paramSFOPath, out string titleID, out string targetPath, out uint txID)
        {
            PS3TMAPI.ScopedGlobalHeapPtr scopedGlobalHeapPtr = new PS3TMAPI.ScopedGlobalHeapPtr(PS3TMAPI.AllocUtf8FromString(paramSFOPath));
            IntPtr titleId;
            IntPtr targetPath1;
            PS3TMAPI.SNRESULT snresult = PS3TMAPI.Is32Bit() ? PS3TMAPI.InstallGameExX86(target, scopedGlobalHeapPtr.Get(), out titleId, out targetPath1, out txID) : PS3TMAPI.InstallGameExX64(target, scopedGlobalHeapPtr.Get(), out titleId, out targetPath1, out txID);
            titleID = PS3TMAPI.Utf8ToString(titleId, uint.MaxValue);
            targetPath = PS3TMAPI.Utf8ToString(targetPath1, uint.MaxValue);
            return snresult;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3FormatHDD", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT FormatHDDX86(int target, uint initRegistry);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3FormatHDD", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT FormatHDDX64(int target, uint initRegistry);

        public static PS3TMAPI.SNRESULT FormatHDD(int target, uint initRegistry)
        {
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.FormatHDDX64(target, initRegistry);
            return PS3TMAPI.FormatHDDX86(target, initRegistry);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3UninstallGame", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT UninstallGameX86(int target, IntPtr gameDirectory);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3UninstallGame", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT UninstallGameX64(int target, IntPtr gameDirectory);

        public static PS3TMAPI.SNRESULT UninstallGame(int target, string gameDirectory)
        {
            PS3TMAPI.ScopedGlobalHeapPtr scopedGlobalHeapPtr = new PS3TMAPI.ScopedGlobalHeapPtr(PS3TMAPI.AllocUtf8FromString(gameDirectory));
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.UninstallGameX64(target, scopedGlobalHeapPtr.Get());
            return PS3TMAPI.UninstallGameX86(target, scopedGlobalHeapPtr.Get());
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3WaitForFileTransfer", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT WaitForFileTransferX86(int target, uint txId, out PS3TMAPI.FileTransferNotificationType notificationType, uint msTimeout);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3WaitForFileTransfer", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT WaitForFileTransferX64(int target, uint txId, out PS3TMAPI.FileTransferNotificationType notificationType, uint msTimeout);

        public static PS3TMAPI.SNRESULT WaitForFileTransfer(int target, uint txID, out PS3TMAPI.FileTransferNotificationType notificationType, uint msTimeout)
        {
            return PS3TMAPI.Is32Bit() ? PS3TMAPI.WaitForFileTransferX86(target, txID, out notificationType, msTimeout) : PS3TMAPI.WaitForFileTransferX64(target, txID, out notificationType, msTimeout);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3FSGetFreeSize", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT FSGetFreeSizeX86(int target, IntPtr fsDir, out uint blockSize, out ulong freeBlockCount);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3FSGetFreeSize", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT FSGetFreeSizeX64(int target, IntPtr fsDir, out uint blockSize, out ulong freeBlockCount);

        public static PS3TMAPI.SNRESULT FSGetFreeSize(int target, string fsDir, out uint blockSize, out ulong freeBlockCount)
        {
            PS3TMAPI.ScopedGlobalHeapPtr scopedGlobalHeapPtr = new PS3TMAPI.ScopedGlobalHeapPtr(PS3TMAPI.AllocUtf8FromString(fsDir));
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.FSGetFreeSizeX64(target, scopedGlobalHeapPtr.Get(), out blockSize, out freeBlockCount);
            return PS3TMAPI.FSGetFreeSizeX86(target, scopedGlobalHeapPtr.Get(), out blockSize, out freeBlockCount);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetLogOptions", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetLogOptionsX86(out PS3TMAPI.LogCategory category);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetLogOptions", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT GetLogOptionsX64(out PS3TMAPI.LogCategory category);

        public static PS3TMAPI.SNRESULT GetLogOptions(out PS3TMAPI.LogCategory category)
        {
            return PS3TMAPI.Is32Bit() ? PS3TMAPI.GetLogOptionsX86(out category) : PS3TMAPI.GetLogOptionsX64(out category);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SetLogOptions", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT SetLogOptionsX86(PS3TMAPI.LogCategory category);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SetLogOptions", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT SetLogOptionsX64(PS3TMAPI.LogCategory category);

        public static PS3TMAPI.SNRESULT SetLogOptions(PS3TMAPI.LogCategory category)
        {
            return PS3TMAPI.Is32Bit() ? PS3TMAPI.SetLogOptionsX86(category) : PS3TMAPI.SetLogOptionsX64(category);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3EnableInternalKick", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT EnableInternalKickX86(bool enable);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3EnableInternalKick", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT EnableInternalKickX64(bool enable);

        public static PS3TMAPI.SNRESULT EnableInternalKick(bool bEnable)
        {
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.EnableInternalKickX64(bEnable);
            return PS3TMAPI.EnableInternalKickX86(bEnable);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ProcessOfflineFileTrace", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT ProcessOfflineFileTraceX86(int target, IntPtr path);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ProcessOfflineFileTrace", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT ProcessOfflineFileTraceX64(int target, IntPtr path);

        public static PS3TMAPI.SNRESULT ProcessOfflineFileTrace(int target, string path)
        {
            PS3TMAPI.ScopedGlobalHeapPtr scopedGlobalHeapPtr = new PS3TMAPI.ScopedGlobalHeapPtr(PS3TMAPI.AllocUtf8FromString(path));
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.ProcessOfflineFileTraceX64(target, scopedGlobalHeapPtr.Get());
            return PS3TMAPI.ProcessOfflineFileTraceX86(target, scopedGlobalHeapPtr.Get());
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3BDTransferImage", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT BDTransferImageX86(int target, IntPtr sourceFileName, string destinationDevice, out uint transactionId);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3BDTransferImage", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT BDTransferImageX64(int target, IntPtr sourceFileName, string destinationDevice, out uint transactionId);

        public static PS3TMAPI.SNRESULT BDTransferImage(int target, string sourceFileName, string destinationDevice, out uint transactionId)
        {
            PS3TMAPI.ScopedGlobalHeapPtr scopedGlobalHeapPtr = new PS3TMAPI.ScopedGlobalHeapPtr(PS3TMAPI.AllocUtf8FromString(sourceFileName));
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.BDTransferImageX64(target, scopedGlobalHeapPtr.Get(), destinationDevice, out transactionId);
            return PS3TMAPI.BDTransferImageX86(target, scopedGlobalHeapPtr.Get(), destinationDevice, out transactionId);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3BDInsert", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT BDInsertX86(int target, string deviceName);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3BDInsert", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT BDInsertX64(int target, string deviceName);

        public static PS3TMAPI.SNRESULT BDInsert(int target, string deviceName)
        {
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.BDInsertX64(target, deviceName);
            return PS3TMAPI.BDInsertX86(target, deviceName);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3BDEject", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT BDEjectX86(int target, string deviceName);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3BDEject", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT BDEjectX64(int target, string deviceName);

        public static PS3TMAPI.SNRESULT BDEject(int target, string deviceName)
        {
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.BDEjectX64(target, deviceName);
            return PS3TMAPI.BDEjectX86(target, deviceName);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3BDFormat", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT BDFormatX86(int target, string deviceName, uint formatMode);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3BDFormat", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT BDFormatX64(int target, string deviceName, uint formatMode);

        public static PS3TMAPI.SNRESULT BDFormat(int target, string deviceName, uint formatMode)
        {
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.BDFormatX64(target, deviceName, formatMode);
            return PS3TMAPI.BDFormatX86(target, deviceName, formatMode);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3BDQuery", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT BDQueryX86(int target, string deviceName, ref PS3TMAPI.BDInfoPriv infoPriv);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3BDQuery", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT BDQueryX64(int target, string deviceName, ref PS3TMAPI.BDInfoPriv infoPriv);

        public static PS3TMAPI.SNRESULT BDQuery(int target, string deviceName, ref PS3TMAPI.BDInfo info)
        {
            PS3TMAPI.BDInfoPriv infoPriv = new PS3TMAPI.BDInfoPriv();
            PS3TMAPI.SNRESULT res = PS3TMAPI.Is32Bit() ? PS3TMAPI.BDQueryX86(target, deviceName, ref infoPriv) : PS3TMAPI.BDQueryX64(target, deviceName, ref infoPriv);
            if (PS3TMAPI.SUCCEEDED(res))
            {
                info.bdemu_data_size = infoPriv.bdemu_data_size;
                info.bdemu_total_entry = infoPriv.bdemu_total_entry;
                info.bdemu_selected_index = infoPriv.bdemu_selected_index;
                info.image_index = infoPriv.image_index;
                info.image_type = infoPriv.image_type;
                info.image_file_name = PS3TMAPI.Utf8FixedSizeByteArrayToString(infoPriv.image_file_name);
                info.image_file_size = infoPriv.image_file_size;
                info.image_product_code = PS3TMAPI.Utf8FixedSizeByteArrayToString(infoPriv.image_product_code);
                info.image_producer = PS3TMAPI.Utf8FixedSizeByteArrayToString(infoPriv.image_producer);
                info.image_author = PS3TMAPI.Utf8FixedSizeByteArrayToString(infoPriv.image_author);
                info.image_date = PS3TMAPI.Utf8FixedSizeByteArrayToString(infoPriv.image_date);
                info.image_sector_layer0 = infoPriv.image_sector_layer0;
                info.image_sector_layer1 = infoPriv.image_sector_layer1;
                info.image_memorandum = PS3TMAPI.Utf8FixedSizeByteArrayToString(infoPriv.image_memorandum);
            }
            return res;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3RegisterTargetEventHandler", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT RegisterTargetEventHandlerX86(int target, PS3TMAPI.HandleEventCallbackPriv callback, IntPtr userData);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3RegisterTargetEventHandler", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT RegisterTargetEventHandlerX64(int target, PS3TMAPI.HandleEventCallbackPriv callback, IntPtr userData);

        public static PS3TMAPI.SNRESULT RegisterTargetEventHandler(int target, PS3TMAPI.TargetEventCallback callback, ref object userData)
        {
            if (callback == null)
                return PS3TMAPI.SNRESULT.SN_E_BAD_PARAM;
            PS3TMAPI.SNRESULT res = PS3TMAPI.Is32Bit() ? PS3TMAPI.RegisterTargetEventHandlerX86(target, PS3TMAPI.ms_eventHandlerWrapper, IntPtr.Zero) : PS3TMAPI.RegisterTargetEventHandlerX64(target, PS3TMAPI.ms_eventHandlerWrapper, IntPtr.Zero);
            if (PS3TMAPI.SUCCEEDED(res))
            {
                PS3TMAPI.TargetCallbackAndUserData callbackAndUserData = new PS3TMAPI.TargetCallbackAndUserData();
                callbackAndUserData.m_callback = callback;
                callbackAndUserData.m_userData = userData;
                if (PS3TMAPI.ms_userTargetCallbacks == null)
                    PS3TMAPI.ms_userTargetCallbacks = new Dictionary<int, PS3TMAPI.TargetCallbackAndUserData>(1);
                PS3TMAPI.ms_userTargetCallbacks[target] = callbackAndUserData;
            }
            return res;
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3CancelTargetEvents", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT CancelTargetEventsX86(int target);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3CancelTargetEvents", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT CancelTargetEventsX64(int target);

        public static PS3TMAPI.SNRESULT CancelTargetEvents(int target)
        {
            PS3TMAPI.SNRESULT res = PS3TMAPI.Is32Bit() ? PS3TMAPI.CancelTargetEventsX86(target) : PS3TMAPI.CancelTargetEventsX64(target);
            if (PS3TMAPI.SUCCEEDED(res))
            {
                if (PS3TMAPI.ms_userTargetCallbacks == null)
                    return PS3TMAPI.SNRESULT.SN_E_ERROR;
                PS3TMAPI.ms_userTargetCallbacks.Remove(target);
            }
            return res;
        }

        private static void MarshalTargetEvent(int target, uint param, PS3TMAPI.SNRESULT result, uint length, IntPtr data)
        {
            List<PS3TMAPI.TargetEvent> targetEventList = new List<PS3TMAPI.TargetEvent>();
            uint num1 = length;
            while (num1 > 0U)
            {
                PS3TMAPI.TargetEvent targetEvent = new PS3TMAPI.TargetEvent();
                PS3TMAPI.TargetEventHdrPriv storage1 = new PS3TMAPI.TargetEventHdrPriv();
                IntPtr num2 = PS3TMAPI.ReadDataFromUnmanagedIncPtr<PS3TMAPI.TargetEventHdrPriv>(data, ref storage1);
                uint size = storage1.Size;
                targetEvent.TargetID = storage1.TargetID;
                targetEvent.Type = (PS3TMAPI.TargetEventType)storage1.EventType;
                targetEvent.Type.GetType();
                IntPtr num3;
                switch (targetEvent.Type)
                {
                    case PS3TMAPI.TargetEventType.UnitStatusChange:
                        targetEvent.EventData = new PS3TMAPI.TargetEventData();
                        PS3TMAPI.TGTEventUnitStatusChangeDataPriv storage2 = new PS3TMAPI.TGTEventUnitStatusChangeDataPriv();
                        num3 = PS3TMAPI.ReadDataFromUnmanagedIncPtr<PS3TMAPI.TGTEventUnitStatusChangeDataPriv>(num2, ref storage2);
                        targetEvent.EventData.UnitStatusChangeData.Unit = (PS3TMAPI.UnitType)storage2.Unit;
                        targetEvent.EventData.UnitStatusChangeData.Status = (PS3TMAPI.UnitStatus)storage2.Status;
                        break;
                    case PS3TMAPI.TargetEventType.Details:
                        targetEvent.EventData = new PS3TMAPI.TargetEventData();
                        num3 = PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(num2, ref targetEvent.EventData.DetailsData.Flags);
                        break;
                    case PS3TMAPI.TargetEventType.ModuleLoad:
                    case PS3TMAPI.TargetEventType.ModuleRunning:
                    case PS3TMAPI.TargetEventType.ModuleStopped:
                        targetEvent.EventData = new PS3TMAPI.TargetEventData();
                        num3 = PS3TMAPI.ReadDataFromUnmanagedIncPtr<PS3TMAPI.TGTEventModuleEventData>(num2, ref targetEvent.EventData.ModuleEventData);
                        break;
                    case PS3TMAPI.TargetEventType.BDIsotransferStarted:
                    case PS3TMAPI.TargetEventType.BDIsotransferFinished:
                    case PS3TMAPI.TargetEventType.BDFormatStarted:
                    case PS3TMAPI.TargetEventType.BDFormatFinished:
                    case PS3TMAPI.TargetEventType.BDMountStarted:
                    case PS3TMAPI.TargetEventType.BDMountFinished:
                    case PS3TMAPI.TargetEventType.BDUnmountStarted:
                    case PS3TMAPI.TargetEventType.BDUnmountFinished:
                        targetEvent.EventData = new PS3TMAPI.TargetEventData();
                        PS3TMAPI.TGTEventBDDataPriv storage3 = new PS3TMAPI.TGTEventBDDataPriv();
                        num3 = PS3TMAPI.ReadDataFromUnmanagedIncPtr<PS3TMAPI.TGTEventBDDataPriv>(num2, ref storage3);
                        targetEvent.EventData.BdData.Source = PS3TMAPI.Utf8FixedSizeByteArrayToString(storage3.Source);
                        targetEvent.EventData.BdData.Destination = PS3TMAPI.Utf8FixedSizeByteArrayToString(storage3.Destination);
                        targetEvent.EventData.BdData.Result = storage3.Result;
                        break;
                    case PS3TMAPI.TargetEventType.TargetSpecific:
                        targetEvent.TargetSpecific = PS3TMAPI.MarshalTargetSpecificEvent(size, num2);
                        break;
                }
                targetEventList.Add(targetEvent);
                num1 -= size;
                data = new IntPtr(data.ToInt64() + (long)size);
            }
            if (PS3TMAPI.ms_userTargetCallbacks == null)
                return;
            PS3TMAPI.ms_userTargetCallbacks[target].m_callback(target, result, targetEventList.ToArray(), PS3TMAPI.ms_userTargetCallbacks[target].m_userData);
        }

        private static PS3TMAPI.TargetSpecificEvent MarshalTargetSpecificEvent(uint eventSize, IntPtr data)
        {
            PS3TMAPI.TargetSpecificEvent targetSpecificEvent = new PS3TMAPI.TargetSpecificEvent();
            PS3TMAPI.TargetSpecificData targetSpecificData = new PS3TMAPI.TargetSpecificData();
            uint storage = 0;
            data = PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(data, ref targetSpecificEvent.CommandID);
            data = PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(data, ref targetSpecificEvent.RequestID);
            data = PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(data, ref storage);
            data = PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(data, ref targetSpecificEvent.ProcessID);
            data = PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(data, ref targetSpecificEvent.Result);
            int num1 = Marshal.ReadInt32(data, 0);
            data = new IntPtr(data.ToInt64() + (long)Marshal.SizeOf((object)num1));
            targetSpecificData.Type = (PS3TMAPI.TargetSpecificEventType)num1;
            int num2 = 20;
            switch (targetSpecificData.Type)
            {
                case PS3TMAPI.TargetSpecificEventType.CoreDumpComplete:
                    targetSpecificData.CoreDumpComplete = new PS3TMAPI.CoreDumpComplete();
                    targetSpecificData.CoreDumpComplete.Filename = PS3TMAPI.Utf8ToString(data, 1024U);
                    break;
                case PS3TMAPI.TargetSpecificEventType.CoreDumpStart:
                    targetSpecificData.CoreDumpStart = new PS3TMAPI.CoreDumpStart();
                    targetSpecificData.CoreDumpStart.Filename = PS3TMAPI.Utf8ToString(data, 1024U);
                    break;
                case PS3TMAPI.TargetSpecificEventType.Footswitch:
                    targetSpecificData.Footswitch = new PS3TMAPI.FootswitchData();
                    data = PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(data, ref targetSpecificData.Footswitch.EventSource);
                    data = PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(data, ref targetSpecificData.Footswitch.EventData1);
                    data = PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(data, ref targetSpecificData.Footswitch.EventData2);
                    data = PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(data, ref targetSpecificData.Footswitch.EventData3);
                    data = PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(data, ref targetSpecificData.Footswitch.Reserved);
                    break;
                case PS3TMAPI.TargetSpecificEventType.InstallPackageProgress:
                    targetSpecificData.InstallPackageProgress = new PS3TMAPI.InstallPackageProgress();
                    data = PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(data, ref targetSpecificData.InstallPackageProgress.Percent);
                    break;
                case PS3TMAPI.TargetSpecificEventType.InstallPackagePath:
                    targetSpecificData.InstallPackagePath = new PS3TMAPI.InstallPackagePath();
                    targetSpecificData.InstallPackagePath.Path = PS3TMAPI.Utf8ToString(data, 1024U);
                    break;
                case PS3TMAPI.TargetSpecificEventType.PRXLoad:
                    targetSpecificData.PRXLoad = new PS3TMAPI.NotifyPRXLoadData();
                    data = PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(data, ref targetSpecificData.PRXLoad.PPUThreadID);
                    data = PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(data, ref targetSpecificData.PRXLoad.PRXID);
                    data = PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(data, ref targetSpecificData.PRXLoad.Timestamp);
                    break;
                case PS3TMAPI.TargetSpecificEventType.PRXUnload:
                    targetSpecificData.PRXUnload = new PS3TMAPI.NotifyPRXUnloadData();
                    data = PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(data, ref targetSpecificData.PRXUnload.PPUThreadID);
                    data = PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(data, ref targetSpecificData.PRXUnload.PRXID);
                    data = PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(data, ref targetSpecificData.PRXUnload.Timestamp);
                    break;
                case PS3TMAPI.TargetSpecificEventType.ProcessCreate:
                    targetSpecificData.PPUProcessCreate = new PS3TMAPI.PPUProcessCreateData();
                    data = PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(data, ref targetSpecificData.PPUProcessCreate.ParentProcessID);
                    if ((long)storage - (long)num2 - 4L > 0L)
                    {
                        targetSpecificData.PPUProcessCreate.Filename = PS3TMAPI.Utf8ToString(data, uint.MaxValue);
                        break;
                    }
                    break;
                case PS3TMAPI.TargetSpecificEventType.ProcessExit:
                    targetSpecificData.PPUProcessExit = new PS3TMAPI.PPUProcessExitData();
                    data = PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(data, ref targetSpecificData.PPUProcessExit.ExitCode);
                    break;
                case PS3TMAPI.TargetSpecificEventType.PPUExcTrap:
                case PS3TMAPI.TargetSpecificEventType.PPUExcPrevInt:
                case PS3TMAPI.TargetSpecificEventType.PPUExcIllInst:
                case PS3TMAPI.TargetSpecificEventType.PPUExcTextHtabMiss:
                case PS3TMAPI.TargetSpecificEventType.PPUExcTextSlbMiss:
                case PS3TMAPI.TargetSpecificEventType.PPUExcDataHtabMiss:
                case PS3TMAPI.TargetSpecificEventType.PPUExcFloat:
                case PS3TMAPI.TargetSpecificEventType.PPUExcDataSlbMiss:
                case PS3TMAPI.TargetSpecificEventType.PPUExcDabrMatch:
                case PS3TMAPI.TargetSpecificEventType.PPUExcStop:
                case PS3TMAPI.TargetSpecificEventType.PPUExcStopInit:
                    targetSpecificData.PPUException = new PS3TMAPI.PPUExceptionData();
                    data = PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(data, ref targetSpecificData.PPUException.ThreadID);
                    data = PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(data, ref targetSpecificData.PPUException.HWThreadNumber);
                    data = PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(data, ref targetSpecificData.PPUException.PC);
                    data = PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(data, ref targetSpecificData.PPUException.SP);
                    break;
                case PS3TMAPI.TargetSpecificEventType.PPUExcAlignment:
                    targetSpecificData.PPUAlignmentException = new PS3TMAPI.PPUAlignmentExceptionData();
                    data = PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(data, ref targetSpecificData.PPUAlignmentException.ThreadID);
                    data = PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(data, ref targetSpecificData.PPUAlignmentException.HWThreadNumber);
                    data = PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(data, ref targetSpecificData.PPUAlignmentException.DSISR);
                    data = PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(data, ref targetSpecificData.PPUAlignmentException.DAR);
                    data = PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(data, ref targetSpecificData.PPUAlignmentException.PC);
                    data = PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(data, ref targetSpecificData.PPUAlignmentException.SP);
                    break;
                case PS3TMAPI.TargetSpecificEventType.PPUExcDataMAT:
                    targetSpecificData.PPUDataMatException = new PS3TMAPI.PPUDataMatExceptionData();
                    data = PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(data, ref targetSpecificData.PPUDataMatException.ThreadID);
                    data = PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(data, ref targetSpecificData.PPUDataMatException.HWThreadNumber);
                    data = PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(data, ref targetSpecificData.PPUDataMatException.DSISR);
                    data = PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(data, ref targetSpecificData.PPUDataMatException.DAR);
                    data = PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(data, ref targetSpecificData.PPUDataMatException.PC);
                    data = PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(data, ref targetSpecificData.PPUDataMatException.SP);
                    break;
                case PS3TMAPI.TargetSpecificEventType.PPUThreadCreate:
                    targetSpecificData.PPUThreadCreate = new PS3TMAPI.PPUThreadCreateData();
                    data = PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(data, ref targetSpecificData.PPUThreadCreate.ThreadID);
                    break;
                case PS3TMAPI.TargetSpecificEventType.PPUThreadExit:
                    targetSpecificData.PPUThreadExit = new PS3TMAPI.PPUThreadExitData();
                    data = PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(data, ref targetSpecificData.PPUThreadExit.ThreadID);
                    break;
                case PS3TMAPI.TargetSpecificEventType.SPUThreadStart:
                    targetSpecificData.SPUThreadStart = new PS3TMAPI.SPUThreadStartData();
                    data = PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(data, ref targetSpecificData.SPUThreadStart.ThreadGroupID);
                    data = PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(data, ref targetSpecificData.SPUThreadStart.ThreadID);
                    if ((long)storage - (long)num2 - 8L > 0L)
                    {
                        targetSpecificData.SPUThreadStart.ElfFilename = PS3TMAPI.Utf8ToString(data, uint.MaxValue);
                        break;
                    }
                    break;
                case PS3TMAPI.TargetSpecificEventType.SPUThreadStop:
                case PS3TMAPI.TargetSpecificEventType.SPUThreadStopInit:
                    targetSpecificData.SPUThreadStop = new PS3TMAPI.SPUThreadStopData();
                    data = PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(data, ref targetSpecificData.SPUThreadStop.ThreadGroupID);
                    data = PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(data, ref targetSpecificData.SPUThreadStop.ThreadID);
                    data = PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(data, ref targetSpecificData.SPUThreadStop.PC);
                    int num3 = Marshal.ReadInt32(data, 0);
                    data = new IntPtr(data.ToInt64() + (long)Marshal.SizeOf((object)num3));
                    targetSpecificData.SPUThreadStop.Reason = (PS3TMAPI.SPUThreadStopReason)num3;
                    data = PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(data, ref targetSpecificData.SPUThreadStop.SP);
                    break;
                case PS3TMAPI.TargetSpecificEventType.SPUThreadGroupDestroy:
                    targetSpecificData.SPUThreadGroupDestroyData = new PS3TMAPI.SPUThreadGroupDestroyData();
                    data = PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(data, ref targetSpecificData.SPUThreadGroupDestroyData.ThreadGroupID);
                    break;
                case PS3TMAPI.TargetSpecificEventType.SPUThreadStopEx:
                    targetSpecificData.SPUThreadStopEx = new PS3TMAPI.SPUThreadStopExData();
                    data = PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(data, ref targetSpecificData.SPUThreadStopEx.ThreadGroupID);
                    data = PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(data, ref targetSpecificData.SPUThreadStopEx.ThreadID);
                    data = PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(data, ref targetSpecificData.SPUThreadStopEx.PC);
                    int num4 = Marshal.ReadInt32(data, 0);
                    data = new IntPtr(data.ToInt64() + (long)Marshal.SizeOf((object)num4));
                    targetSpecificData.SPUThreadStopEx.Reason = (PS3TMAPI.SPUThreadStopReason)num4;
                    data = PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(data, ref targetSpecificData.SPUThreadStopEx.SP);
                    data = PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(data, ref targetSpecificData.SPUThreadStopEx.MFCDSISR);
                    data = PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(data, ref targetSpecificData.SPUThreadStopEx.MFCDSIPR);
                    data = PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(data, ref targetSpecificData.SPUThreadStopEx.MFCDAR);
                    break;
            }
            targetSpecificEvent.Data = targetSpecificData;
            return targetSpecificEvent;
        }

        private static void EventHandlerWrapper(int target, PS3TMAPI.EventType type, uint param, PS3TMAPI.SNRESULT result, uint length, IntPtr data, IntPtr userData)
        {
            switch (type)
            {
                case PS3TMAPI.EventType.TTY:
                    PS3TMAPI.MarshalTTYEvent(target, param, result, length, data);
                    break;
                case PS3TMAPI.EventType.Target:
                    PS3TMAPI.MarshalTargetEvent(target, param, result, length, data);
                    break;
                case PS3TMAPI.EventType.FTP:
                    PS3TMAPI.MarshalFTPEvent(target, param, result, length, data);
                    break;
                case PS3TMAPI.EventType.PadCapture:
                    PS3TMAPI.MarshalPadCaptureEvent(target, param, result, length, data);
                    break;
                case PS3TMAPI.EventType.FileTrace:
                    PS3TMAPI.MarshalFileTraceEvent(target, param, result, length, data);
                    break;
                case PS3TMAPI.EventType.PadPlayback:
                    PS3TMAPI.MarshalPadPlaybackEvent(target, param, result, length, data);
                    break;
                case PS3TMAPI.EventType.Server:
                    PS3TMAPI.MarshalServerEvent(target, param, result, length, data);
                    break;
            }
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SearchForTargets", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT SearchForTargetsX86(string ipAddressFrom, string ipAddressTo, PS3TMAPI.SearchTargetsCallbackPriv callback, IntPtr userData, int port);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SearchForTargets", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT SearchForTargetsX64(string ipAddressFrom, string ipAddressTo, PS3TMAPI.SearchTargetsCallbackPriv callback, IntPtr userData, int port);

        public static PS3TMAPI.SNRESULT SearchForTargets(string ipAddressFrom, string ipAddressTo, PS3TMAPI.SearchTargetsCallback callback, object userData, int port)
        {
            PS3TMAPI.SearchForTargetsCallbackHandler targetsCallbackHandler = new PS3TMAPI.SearchForTargetsCallbackHandler(callback, userData);
            PS3TMAPI.SearchTargetsCallbackPriv callback1 = new PS3TMAPI.SearchTargetsCallbackPriv(PS3TMAPI.SearchForTargetsCallbackHandler.SearchForTargetsCallback);
            IntPtr intPtr = GCHandle.ToIntPtr(GCHandle.Alloc((object)targetsCallbackHandler));
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.SearchForTargetsX64(ipAddressFrom, ipAddressTo, callback1, intPtr, port);
            return PS3TMAPI.SearchForTargetsX86(ipAddressFrom, ipAddressTo, callback1, intPtr, port);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3StopSearchForTargets", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT StopSearchForTargetsX86();

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3StopSearchForTargets", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT StopSearchForTargetsX64();

        public static PS3TMAPI.SNRESULT StopSearchForTargets()
        {
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.StopSearchForTargetsX64();
            return PS3TMAPI.StopSearchForTargetsX86();
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3IsScanning", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT IsScanningX86();

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3IsScanning", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT IsScanningX64();

        public static PS3TMAPI.SNRESULT IsScanning()
        {
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.IsScanningX64();
            return PS3TMAPI.IsScanningX86();
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3IsValidResolution", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT IsValidResolutionX86(uint monitorType, uint startupResolution);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3IsValidResolution", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT IsValidResolutionX64(uint monitorType, uint startupResolution);

        public static PS3TMAPI.SNRESULT IsValidResolution(uint monitorType, uint startupResolution)
        {
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.IsValidResolutionX64(monitorType, startupResolution);
            return PS3TMAPI.IsValidResolutionX86(monitorType, startupResolution);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SetDisplaySettings", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT SetDisplaySettingsX86(int target, IntPtr executable, uint monitorType, uint connectorType, uint startupResolution, bool HDCP, bool resetAfter);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SetDisplaySettings", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT SetDisplaySettingsX64(int target, IntPtr executable, uint monitorType, uint connectorType, uint startupResolution, bool HDCP, bool resetAfter);

        public static PS3TMAPI.SNRESULT SetDisplaySettings(int target, string executable, uint monitorType, uint connectorType, uint startupResolution, bool HDCP, bool resetAfter)
        {
            PS3TMAPI.ScopedGlobalHeapPtr scopedGlobalHeapPtr = new PS3TMAPI.ScopedGlobalHeapPtr(PS3TMAPI.AllocUtf8FromString(executable));
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.SetDisplaySettingsX64(target, scopedGlobalHeapPtr.Get(), monitorType, connectorType, startupResolution, HDCP, resetAfter);
            return PS3TMAPI.SetDisplaySettingsX86(target, scopedGlobalHeapPtr.Get(), monitorType, connectorType, startupResolution, HDCP, resetAfter);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3MapFileSystem", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT MapFileSystemX86(char driveLetter);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3MapFileSystem", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT MapFileSystemX64(char driveLetter);

        public static PS3TMAPI.SNRESULT MapFileSystem(char driveLetter)
        {
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.MapFileSystemX64(driveLetter);
            return PS3TMAPI.MapFileSystemX86(driveLetter);
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3UnmapFileSystem", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT UnmapFileSystemX86();

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3UnmapFileSystem", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT UnmapFileSystemX64();

        public static PS3TMAPI.SNRESULT UnmapFileSystem()
        {
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.UnmapFileSystemX64();
            return PS3TMAPI.UnmapFileSystemX86();
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ImportTargetSettings", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT ImportTargetSettingsX86(int target, IntPtr szFileName);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ImportTargetSettings", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT ImportTargetSettingsX64(int target, IntPtr szFileName);

        public static PS3TMAPI.SNRESULT ImportTargetSettings(int target, string fileName)
        {
            PS3TMAPI.ScopedGlobalHeapPtr scopedGlobalHeapPtr = new PS3TMAPI.ScopedGlobalHeapPtr(PS3TMAPI.AllocUtf8FromString(fileName));
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.ImportTargetSettingsX64(target, scopedGlobalHeapPtr.Get());
            return PS3TMAPI.ImportTargetSettingsX86(target, scopedGlobalHeapPtr.Get());
        }

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ExportTargetSettings", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT ExportTargetSettingsX86(int target, IntPtr szFileName);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ExportTargetSettings", CallingConvention = CallingConvention.Cdecl)]
        private static extern PS3TMAPI.SNRESULT ExportTargetSettingsX64(int target, IntPtr szFileName);

        public static PS3TMAPI.SNRESULT ExportTargetSettings(int target, string fileName)
        {
            PS3TMAPI.ScopedGlobalHeapPtr scopedGlobalHeapPtr = new PS3TMAPI.ScopedGlobalHeapPtr(PS3TMAPI.AllocUtf8FromString(fileName));
            if (!PS3TMAPI.Is32Bit())
                return PS3TMAPI.ExportTargetSettingsX64(target, scopedGlobalHeapPtr.Get());
            return PS3TMAPI.ExportTargetSettingsX86(target, scopedGlobalHeapPtr.Get());
        }

        private static IntPtr WriteDataToUnmanagedIncPtr<T>(T storage, IntPtr unmanagedBuf)
        {
            bool fDeleteOld = false;
            Marshal.StructureToPtr((object)storage, unmanagedBuf, fDeleteOld);
            return new IntPtr(unmanagedBuf.ToInt64() + (long)Marshal.SizeOf((object)storage));
        }

        private static IntPtr ReadDataFromUnmanagedIncPtr<T>(IntPtr unmanagedBuf, ref T storage)
        {
            storage = (T)Marshal.PtrToStructure(unmanagedBuf, typeof(T));
            return new IntPtr(unmanagedBuf.ToInt64() + (long)Marshal.SizeOf((object)storage));
        }

        private static IntPtr ReadAnsiStringFromUnmanagedIncPtr(IntPtr unmanagedBuf, ref string inputString)
        {
            inputString = Marshal.PtrToStringAnsi(unmanagedBuf);
            return new IntPtr(unmanagedBuf.ToInt64() + (long)inputString.Length + 1L);
        }

        private static IntPtr AllocUtf8FromString(string wcharString)
        {
            if (wcharString == null)
                return IntPtr.Zero;
            byte[] bytes = Encoding.UTF8.GetBytes(wcharString);
            IntPtr destination = Marshal.AllocHGlobal(bytes.Length + 1);
            Marshal.Copy(bytes, 0, destination, bytes.Length);
            Marshal.WriteByte((IntPtr)(destination.ToInt64() + (long)bytes.Length), (byte)0);
            return destination;
        }

        private static string Utf8ToString(IntPtr utf8Ptr, uint maxLength)
        {
            if (utf8Ptr == IntPtr.Zero)
                return "";
            byte numPtr = (byte)utf8Ptr;
            int length;
            for (length = 0; (int)numPtr != 0 && (long)length < (long)maxLength; ++length)
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
            for (int index = 0; index < numArray.Length && (int)numArray[index] != 0; ++index)
                ++count;
            byte[] bytes = new byte[count];
            Buffer.BlockCopy((Array)byteArray, 0, (Array)bytes, 0, count);
            return Encoding.UTF8.GetString(bytes);
        }

        public enum SNRESULT
        {
            SN_E_ERROR = -2147483648,
            SN_E_COMMS_EVENT_MISMATCHED_ERR = -39,
            SN_E_CONNECTED = -38,
            SN_E_PROTOCOL_ALREADY_REGISTERED = -37,
            SN_E_COMMAND_CANCELLED = -36,
            SN_E_CONNECT_TO_GAMEPORT_FAILED = -35,
            SN_E_MODULE_NOT_FOUND = -34,
            SN_E_CHECK_TARGET_CONFIGURATION = -33,
            SN_E_LICENSE_ERROR = -32,
            SN_E_LOAD_MODULE_FAILED = -31,
            SN_E_NOT_SUPPORTED_IN_SDK_VERSION = -30,
            SN_E_FILE_ERROR = -29,
            SN_E_BAD_ALIGN = -28,
            SN_E_DEPRECATED = -27,
            SN_E_DATA_TOO_LONG = -26,
            SN_E_INSUFFICIENT_DATA = -25,
            SN_E_EXISTING_CALLBACK = -24,
            SN_E_DECI_ERROR = -23,
            SN_E_BUSY = -22,
            SN_E_BAD_PARAM = -21,
            SN_E_NO_SEL = -20,
            SN_E_NO_TARGETS = -19,
            SN_E_BAD_MEMSPACE = -18,
            SN_E_TARGET_RUNNING = -17,
            SN_E_DLL_NOT_INITIALISED = -15,
            SN_E_TM_VERSION = -14,
            SN_E_NOT_LISTED = -13,
            SN_E_OUT_OF_MEM = -12,
            SN_E_BAD_UNIT = -11,
            SN_E_LOAD_ELF_FAILED = -10,
            SN_E_TARGET_IN_USE = -9,
            SN_E_HOST_NOT_FOUND = -8,
            SN_E_TIMEOUT = -7,
            SN_E_TM_COMMS_ERR = -6,
            SN_E_COMMS_ERR = -5,
            SN_E_NOT_CONNECTED = -4,
            SN_E_BAD_TARGET = -3,
            SN_E_TM_NOT_RUNNING = -2,
            SN_E_NOT_IMPL = -1,
            SN_S_OK = 0,
            SN_S_PENDING = 1,
            SN_S_NO_MSG = 3,
            SN_S_TM_VERSION = 4,
            SN_S_REPLACED = 5,
            SN_S_NO_ACTION = 6,
            SN_S_TARGET_STILL_REGISTERED = 7,
        }

        public enum ConnectStatus
        {
            Connected,
            Connecting,
            NotConnected,
            InUse,
            Unavailable,
        }

        public delegate int EnumerateTargetsCallback(int target);

        public delegate int EnumerateTargetsExCallback(int target, object userData);

        private delegate int EnumerateTargetsExCallbackPriv(int target, IntPtr unused);

        [Flags]
        public enum BootParameter : ulong
        {
            Default = 0,
            SystemMode = 17,
            ReleaseMode = 1,
            DebugMode = 16,
            MemSizeConsole = 2,
            BluRayEmuOff = 4,
            HDDSpeedBluRayEmu = 8,
            BluRayEmuUSB = 32,
            HostFSTarget = 64,
            DualNIC = 128,
        }

        [Flags]
        public enum BootParameterMask : ulong
        {
            BootMode = 17,
            Memsize = 2,
            BlurayEmulation = 4,
            HDDSpeed = 8,
            BlurayEmuSelect = 32,
            HostFS = 64,
            NIC = 128,
            All = NIC | HostFS | BlurayEmuSelect | HDDSpeed | BlurayEmulation | Memsize | BootMode,
        }

        [Flags]
        public enum ResetParameter : ulong
        {
            Soft = 0,
            Hard = 1,
            Quick = 2,
            ResetEx = 9223372036854775808,
        }

        [Flags]
        public enum ResetParameterMask : ulong
        {
            All = 9223372036854775811,
        }

        [Flags]
        public enum SystemParameter : ulong
        {
            TargetModel60GB = 281474976710656,
            TargetModel20GB = 562949953421312,
            ReleaseCheckMode = 140737488355328,
        }

        [Flags]
        public enum SystemParameterMask : ulong
        {
            TargetModel = 71776119061217280,
            ReleaseCheck = 140737488355328,
            All = ReleaseCheck | TargetModel,
        }

        [Flags]
        public enum TargetInfoFlag : uint
        {
            TargetID = 1,
            Name = 2,
            Info = 4,
            HomeDir = 8,
            FileServingDir = 16,
            Boot = 32,
        }

        public struct TargetInfo
        {
            public PS3TMAPI.TargetInfoFlag Flags;
            public int Target;
            public string Name;
            public string Type;
            public string Info;
            public string HomeDir;
            public string FSDir;
            public PS3TMAPI.BootParameter Boot;
        }

        private struct TargetInfoPriv
        {
            public PS3TMAPI.TargetInfoFlag Flags;
            public int Target;
            public IntPtr Name;
            public IntPtr Type;
            public IntPtr Info;
            public IntPtr HomeDir;
            public IntPtr FSDir;
            public PS3TMAPI.BootParameter Boot;
        }

        public struct TargetType
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string Type;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string Description;
        }

        [StructLayout(LayoutKind.Sequential)]
        public class TCPIPConnectProperties
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 255)]
            public string IPAddress;
            public uint Port;
        }

        public enum ServerEventType : uint
        {
            TargetAdded,
            TargetDeleted,
            DefaultTargetChanged,
        }

        public delegate void ServerEventCallback(int target, PS3TMAPI.SNRESULT res, PS3TMAPI.ServerEventType eventType, object userData);

        private struct ServerEventHeader
        {
            public uint size;
            public uint targetID;
            public PS3TMAPI.ServerEventType eventType;
        }

        [Flags]
        public enum SystemInfoFlag : uint
        {
            SDKVersion = 1,
            TimebaseFreq = 2,
            RTSDKVersion = 4,
            TotalSystemMem = 8,
            AvailableSysMem = 16,
            DCMBufferSize = 32,
        }

        public struct SystemInfo
        {
            public uint CellSDKVersion;
            public ulong TimebaseFrequency;
            public uint CellRuntimeSDKVersion;
            public uint TotalSystemMemory;
            public uint AvailableSystemMemory;
            public uint DCMBufferSize;
        }

        [Flags]
        public enum ExtraLoadFlag : ulong
        {
            EnableLv2ExceptionHandler = 1,
            EnableRemotePlay = 2,
            EnableGCMDebug = 4,
            LoadLibprofSPRXAutomatically = 8,
            EnableCoreDump = 16,
            EnableAccForRemotePlay = 32,
            EnableHUDRSXTools = 64,
            EnableMAT = 128,
            EnableMiscSettings = 9223372036854775808,
            GameAttributeInviteMessage = 256,
            GameAttributeCustomMessage = 512,
            LoadingPatch = 4096,
        }

        [Flags]
        public enum ExtraLoadFlagMask : ulong
        {
            GameAttributeMessageMask = 3840,
            All = 9223372036854783999,
            OverrideTVGUIMask = 9223372036854775808,
        }

        public struct TTYStream
        {
            public uint Index;
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

        public enum TimeoutType
        {
            Default,
            Reset,
            Connect,
            Load,
            GetStatus,
            Reconnect,
            GamePort,
            GameExit,
        }

        public delegate void TTYCallback(int target, uint streamID, PS3TMAPI.SNRESULT res, string data, object userData);

        public delegate void TTYCallbackRaw(int target, uint streamID, PS3TMAPI.SNRESULT res, byte[] data, object userData);

        private class TTYCallbackAndUserData
        {
            public PS3TMAPI.TTYCallback m_callback;
            public object m_userData;
            public PS3TMAPI.TTYCallbackRaw m_callbackRaw;
            public object m_userDataRaw;
        }

        private struct TTYChannel
        {
            public readonly int Target;
            public readonly uint Channel;

            public TTYChannel(int target, uint channel)
            {
                this.Target = target;
                this.Channel = channel;
            }
        }

        public enum UnitType
        {
            PPU,
            SPU,
            SPURAW,
        }

        public enum UnitStatus : uint
        {
            Unknown,
            Running,
            Stopped,
            Signalled,
            Resetting,
            Missing,
            Reset,
            NotConnected,
            Connected,
            StatusChange,
        }

        [Flags]
        public enum LoadFlag : uint
        {
            EnableDebugging = 1,
            UseELFPriority = 256,
            UseELFStackSize = 512,
            WaitBDMounted = 8192,
            PPUNotDebug = 65536,
            SPUNotDebug = 131072,
            IgnoreDefaults = 2147483648,
            ParamSFOUseELFDir = 1048576,
            ParamSFOUseCustomDir = 2097152,
        }

        public enum ProcessStatus : uint
        {
            Creating = 1,
            Ready = 2,
            Exited = 3,
        }

        public struct ProcessInfoHdr
        {
            public PS3TMAPI.ProcessStatus Status;
            public uint NumPPUThreads;
            public uint NumSPUThreads;
            public uint ParentProcessID;
            public ulong MaxMemorySize;
            public string ELFPath;
        }

        public struct ProcessInfo
        {
            public PS3TMAPI.ProcessInfoHdr Hdr;
            public ulong[] ThreadIDs;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ExtraProcessInfo
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
            public uint[] PPUGUIDs;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ProcessLoadParams
        {
            public ulong Version;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public ulong[] Data;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ProcessLoadInfo
        {
            public uint InfoValid;
            public uint DebugFlags;
            public PS3TMAPI.ProcessLoadParams LoadInfo;
        }

        public struct ModuleInfoHdr
        {
            public string Name;
            public sbyte[] Version;
            public uint Attribute;
            public uint StartEntry;
            public uint StopEntry;
            public string ELFName;
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

        public struct PRXSegment
        {
            public ulong Base;
            public ulong FileSize;
            public ulong MemSize;
            public ulong Index;
            public ulong ELFType;
        }

        public struct ModuleInfo
        {
            public PS3TMAPI.ModuleInfoHdr Hdr;
            public PS3TMAPI.PRXSegment[] Segments;
        }

        public struct PRXSegmentEx
        {
            public ulong Base;
            public ulong FileSize;
            public ulong MemSize;
            public ulong Index;
            public ulong ELFType;
            public ulong Flags;
            public ulong Align;
        }

        public struct ModuleInfoEx
        {
            public PS3TMAPI.ModuleInfoHdr Hdr;
            public PS3TMAPI.PRXSegmentEx[] Segments;
        }

        public struct MSELFInfo
        {
            public ulong MSELFFileOffset;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] Reserved;
        }

        public struct ExtraModuleInfo
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
            public uint[] PPUGUIDs;
        }

        public enum PPUThreadState
        {
            Idle,
            Runnable,
            OnProc,
            Sleep,
            Suspended,
            SleepSuspended,
            Stop,
            Zombie,
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

        public struct PPUThreadInfo
        {
            public ulong ThreadID;
            public uint Priority;
            public PS3TMAPI.PPUThreadState State;
            public ulong StackAddress;
            public ulong StackSize;
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

        public struct PPUThreadInfoEx
        {
            public ulong ThreadID;
            public uint Priority;
            public uint BasePriority;
            public PS3TMAPI.PPUThreadState State;
            public ulong StackAddress;
            public ulong StackSize;
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

        public struct SPUThreadInfo
        {
            public uint ThreadGroupID;
            public uint ThreadID;
            public string Filename;
            public string ThreadName;
        }

        [Flags]
        public enum ELFStackSize : uint
        {
            Stack32k = 32,
            Stack64k = 64,
            Stack96k = Stack64k | Stack32k,
            Stack128k = 128,
            Stack256k = 256,
            Stack512k = 512,
            Stack1024k = 1024,
            StackDefault = Stack64k,
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct DebugThreadControlInfoPriv
        {
            public ulong ControlFlags;
            public uint NumEntries;
        }

        public struct ControlKeywordEntry
        {
            public uint MatchConditionFlags;
            public string Keyword;
        }

        public struct DebugThreadControlInfo
        {
            public ulong ControlFlags;
            public PS3TMAPI.ControlKeywordEntry[] ControlKeywords;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct ProcessTreeBranchPriv
        {
            public uint ProcessId;
            public PS3TMAPI.ProcessStatus ProcessState;
            public uint NumPpuThreads;
            public uint NumSpuThreadGroups;
            public ushort ProcessFlags;
            public ushort RawSPU;
            public IntPtr PpuThreadStatuses;
            public IntPtr SpuThreadGroupStatuses;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct PPUThreadStatus
        {
            public ulong ThreadID;
            public PS3TMAPI.PPUThreadState ThreadState;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct SPUThreadGroupStatus
        {
            public uint ThreadGroupID;
            public PS3TMAPI.SPUThreadGroupState ThreadGroupState;
        }

        public struct ProcessTreeBranch
        {
            public uint ProcessID;
            public PS3TMAPI.ProcessStatus ProcessState;
            public ushort ProcessFlags;
            public ushort RawSPU;
            public PS3TMAPI.PPUThreadStatus[] PPUThreadStatuses;
            public PS3TMAPI.SPUThreadGroupStatus[] SPUThreadGroupStatuses;
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

        public enum SPUThreadGroupState : uint
        {
            NotConfigured,
            Configured,
            Ready,
            Waiting,
            Suspended,
            WaitingSuspended,
            Running,
            Stopped,
        }

        public struct SPUThreadGroupInfo
        {
            public uint ThreadGroupID;
            public PS3TMAPI.SPUThreadGroupState State;
            public uint Priority;
            public string GroupName;
            public uint[] ThreadIDs;
        }

        public enum MemoryCompressionLevel : uint
        {
            None = 0,
            BestSpeed = 1,
            BestCompression = 9,
            Default = 4294967295,
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct VirtualMemoryArea
        {
            public ulong Address;
            public ulong Flags;
            public ulong VSize;
            public ulong Options;
            public ulong PageFaultPPU;
            public ulong PageFaultSPU;
            public ulong PageIn;
            public ulong PageOut;
            public ulong PMemTotal;
            public ulong PMemUsed;
            public ulong Time;
            public ulong[] Pages;
        }

        public struct SyncPrimitiveCounts
        {
            public uint NumMutexes;
            public uint NumConditionVariables;
            public uint NumRWLocks;
            public uint NumLWMutexes;
            public uint NumEventQueues;
            public uint NumSemaphores;
            public uint NumLWConditionVariables;
            public uint NumEventFlag;
        }

        public struct MutexAttr
        {
            public uint Protocol;
            public uint Recursive;
            public uint PShared;
            public uint Adaptive;
            public ulong Key;
            public uint Flags;
            public string Name;
        }

        public struct MutexInfo
        {
            public uint ID;
            public PS3TMAPI.MutexAttr Attribute;
            public ulong OwnerThreadID;
            public uint LockCounter;
            public uint ConditionRefCounter;
            public uint ConditionVarID;
            public uint NumWaitAllThreads;
            public ulong[] WaitingThreads;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct LwMutexInfoPriv
        {
            public uint Id;
            public PS3TMAPI.LWMutexAttr Attribute;
            public ulong OwnerThreadId;
            public uint LockCounter;
            public uint NumWaitingThreads;
            public uint NumWaitAllThreads;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct LWMutexAttr
        {
            public uint Protocol;
            public uint Recursive;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
            public string Name;
        }

        public struct LWMutexInfo
        {
            public uint ID;
            public PS3TMAPI.LWMutexAttr Attribute;
            public ulong OwnerThreadID;
            public uint LockCounter;
            public uint NumWaitAllThreads;
            public ulong[] WaitingThreads;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct ConditionVarInfoPriv
        {
            public uint Id;
            public PS3TMAPI.ConditionVarAttr Attribute;
            public uint MutexId;
            public uint NumWaitingThreads;
            public uint NumWaitAllThreads;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ConditionVarAttr
        {
            public uint PShared;
            public ulong Key;
            public uint Flags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
            public string Name;
        }

        public struct ConditionVarInfo
        {
            public uint ID;
            public PS3TMAPI.ConditionVarAttr Attribute;
            public uint MutexID;
            public uint NumWaitAllThreads;
            public ulong[] WaitingThreads;
        }

        private struct LwConditionVarInfoPriv
        {
            public uint Id;
            public PS3TMAPI.LWConditionVarAttr Attribute;
            public uint LwMutexId;
            public uint NumWaitingThreads;
            public uint NumWaitAllThreads;
        }

        public struct LWConditionVarAttr
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
            public string Name;
        }

        public struct LWConditionVarInfo
        {
            public uint ID;
            public PS3TMAPI.LWConditionVarAttr Attribute;
            public uint LWMutexID;
            public uint NumWaitAllThreads;
            public ulong[] WaitingThreads;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct RwLockInfoPriv
        {
            public uint Id;
            public PS3TMAPI.RWLockAttr Attribute;
            public ulong OwnerThreadId;
            public uint NumWaitingReadThreads;
            public uint NumWaitAllReadThreads;
            public uint NumWaitingWriteThreads;
            public uint NumWaitAllWriteThreads;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct RWLockAttr
        {
            public uint Protocol;
            public uint PShared;
            public ulong Key;
            public uint Flags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
            public string Name;
        }

        public struct RWLockInfo
        {
            public uint ID;
            public PS3TMAPI.RWLockAttr Attribute;
            public ulong OwnerThreadID;
            public uint NumWaitingReadThreads;
            public uint NumWaitAllReadThreads;
            public uint NumWaitingWriteThreads;
            public uint NumWaitAllWriteThreads;
            public ulong[] WaitingThreads;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct SemaphoreInfoPriv
        {
            public uint Id;
            public PS3TMAPI.SemaphoreAttr Attribute;
            public uint MaxValue;
            public uint CurrentValue;
            public uint NumWaitingThreads;
            public uint NumWaitAllThreads;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct SemaphoreAttr
        {
            public uint Protocol;
            public uint PShared;
            public ulong Key;
            public uint Flags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
            public string Name;
        }

        public struct SemaphoreInfo
        {
            public uint ID;
            public PS3TMAPI.SemaphoreAttr Attribute;
            public uint MaxValue;
            public uint CurrentValue;
            public uint NumWaitAllThreads;
            public ulong[] WaitingThreads;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct EventQueueInfoPriv
        {
            public uint Id;
            public PS3TMAPI.EventQueueAttr Attribute;
            public ulong Key;
            public uint Size;
            public uint NumWaitingThreads;
            public uint NumWaitAllThreads;
            public uint NumReadableEvQueue;
            public uint NumReadableAllEvQueue;
            public IntPtr WaitingThreadIds;
            public IntPtr QueueEntries;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct EventQueueAttr
        {
            public uint Protocol;
            public uint Type;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
            public string Name;
        }

        public struct SystemEvent
        {
            public ulong Source;
            public ulong Data1;
            public ulong Data2;
            public ulong Data3;
        }

        public struct EventQueueInfo
        {
            public uint ID;
            public PS3TMAPI.EventQueueAttr Attribute;
            public ulong Key;
            public uint Size;
            public uint NumWaitAllThreads;
            public uint NumReadableAllEvQueue;
            public ulong[] WaitingThreadIDs;
            public PS3TMAPI.SystemEvent[] QueueEntries;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct EventFlagWaitThread
        {
            public ulong ID;
            public ulong BitPattern;
            public uint Mode;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct EventFlagAttr
        {
            public uint Protocol;
            public uint PShared;
            public ulong Key;
            public uint Flags;
            public uint Type;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
            public string Name;
        }

        public struct EventFlagInfo
        {
            public uint ID;
            public PS3TMAPI.EventFlagAttr Attribute;
            public ulong BitPattern;
            public uint NumWaitAllThreads;
            public PS3TMAPI.EventFlagWaitThread[] WaitingThreads;
        }

        public enum PowerStatus
        {
            Off,
            On,
            Suspended,
            Unknown,
            SwitchingOn,
        }

        public struct UserMemoryStats
        {
            public uint CreatedSharedMemorySize;
            public uint AttachedSharedMemorySize;
            public uint ProcessLocalMemorySize;
            public uint ProcessLocalTextSize;
            public uint PRXTextSize;
            public uint PRXDataSize;
            public uint MiscMemorySize;
        }

        public struct GamePortIPAddressData
        {
            public uint ReturnValue;
            public uint IPAddress;
            public uint SubnetMask;
            public uint BroadcastAddress;
        }

        [Flags]
        public enum RSXProfilingFlag : ulong
        {
            UseRSXProfilingTools = 1,
            UseFullHUDFeatures = 2,
        }

        [Flags]
        public enum CoreDumpFlag : ulong
        {
            ToDevMS = 1,
            ToAppHome = 2,
            ToDevUSB = 4,
            ToDevHDD0 = 8,
            DisablePPUExceptionDetection = 36028797018963968,
            DisableSPUExceptionDetection = 18014398509481984,
            DisableRSXExceptionDetection = 9007199254740992,
            DisableFootSwitchDetection = 4503599627370496,
            DisableMemoryDump = 3489660928,
            EnableRestartProcess = 32768,
            EnableKeepRunningHandler = 8192,
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ScatteredWrite
        {
            public uint Address;
            public byte[] Data;
        }

        public enum MATCondition : byte
        {
            Transparent,
            Write,
            ReadWrite,
            Error,
        }

        public struct MATRange
        {
            public uint StartAddress;
            public uint Size;
            public PS3TMAPI.MATCondition[] PageConditions;
        }

        public enum PadPlaybackResponse : uint
        {
            Ok = 0,
            InvalidPacket = 2147549186,
            InsufficientMemory = 2147549188,
            Busy = 2147549194,
            NoDev = 2147549229,
        }

        public delegate void PadPlaybackCallback(int target, PS3TMAPI.SNRESULT res, PS3TMAPI.PadPlaybackResponse playbackResult, object userData);

        private class PadPlaybackCallbackAndUserData
        {
            public PS3TMAPI.PadPlaybackCallback m_callback;
            public object m_userData;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct PadData
        {
            public uint TimeHi;
            public uint TimeLo;
            public uint Reserved0;
            public uint Reserved1;
            public byte Port;
            public byte PortStatus;
            public byte Length;
            public byte Reserved2;
            public uint Reserved3;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
            public short[] buttons;
        }

        public delegate void PadCaptureCallback(int target, PS3TMAPI.SNRESULT res, PS3TMAPI.PadData[] padData, object userData);

        private class PadCaptureCallbackAndUserData
        {
            public PS3TMAPI.PadCaptureCallback m_callback;
            public object m_userData;
        }

        [Flags]
        public enum VRAMCaptureFlag : ulong
        {
            Enabled = 1,
            Disabled = 0,
        }

        public class VRAMInfo
        {
            public ulong BPAddress;
            public ulong TopAddressPointer;
            public uint Width;
            public uint Height;
            public uint Pitch;
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

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct PS3Protocol
        {
            public uint Protocol;
            public uint Port;
            public uint LPARDesc;
        }

        private struct PS3ProtocolPriv
        {
            public readonly uint Protocol;
            public readonly uint Port;

            public PS3ProtocolPriv(uint protocol, uint port)
            {
                this.Protocol = port;
                this.Port = protocol;
            }
        }

        private struct CustomProtocolId
        {
            public readonly int Target;
            public readonly PS3TMAPI.PS3ProtocolPriv Protocol;

            public CustomProtocolId(int target, PS3TMAPI.PS3ProtocolPriv protocol)
            {
                this.Target = target;
                this.Protocol = protocol;
            }
        }

        private delegate void CustomProtocolCallbackPriv(int target, PS3TMAPI.PS3Protocol protocol, IntPtr unmanagedBuf, uint length, IntPtr userData);

        public delegate void CustomProtocolCallback(int target, PS3TMAPI.PS3Protocol protocol, byte[] data, object userData);

        private class CusProtoCallbackAndUserData
        {
            public PS3TMAPI.CustomProtocolCallback m_callback;
            public object m_userData;
        }

        [Flags]
        public enum FileServingEventFlag : ulong
        {
            Create = 1,
            Close = 4,
            Read = 8,
            Write = 16,
            Seek = 32,
            Delete = 64,
            Rename = 128,
            SetAttr = 256,
            GetAttr = 512,
            SetTime = 1024,
            MKDir = 2048,
            RMDir = 4096,
            OpenDir = 8192,
            CloseDir = 16384,
            ReadDir = 32768,
            Truncate = 65536,
            FGetAttr64 = 131072,
            GetAttr64 = 262144,
            All = GetAttr64 | FGetAttr64 | Truncate | ReadDir | CloseDir | OpenDir | RMDir | MKDir | SetTime | GetAttr | SetAttr | Rename | Delete | Seek | Write | Read | Close | Create,
        }

        public enum FileTransferNotificationType : uint
        {
            Progress = 0,
            Finish = 1,
            Skipped = 2,
            Cancelled = 3,
            Error = 4,
            Pending = 5,
            Unknown = 6,
            RefreshList = 2147483648,
        }

        public struct FTPNotification
        {
            public PS3TMAPI.FileTransferNotificationType Type;
            public uint TransferID;
            public ulong BytesTransferred;
        }

        public delegate void FTPEventCallback(int target, PS3TMAPI.SNRESULT res, PS3TMAPI.FTPNotification[] ftpNotifications, object userData);

        private class FtpCallbackAndUserData
        {
            public PS3TMAPI.FTPEventCallback m_callback;
            public object m_userData;
        }

        public enum FileTraceType
        {
            GetBlockSize = 1,
            Stat = 2,
            WidgetStat = 3,
            Unlink = 4,
            WidgetUnlink = 5,
            RMDir = 6,
            WidgetRMDir = 7,
            Rename = 14,
            WidgetRename = 15,
            Truncate = 18,
            TruncateNoAlloc = 19,
            Truncate2 = 20,
            Truncate2NoInit = 21,
            OpenDir = 24,
            WidgetOpenDir = 25,
            CHMod = 26,
            MkDir = 27,
            UTime = 29,
            Open = 33,
            WidgetOpen = 34,
            Close = 35,
            CloseDir = 36,
            FSync = 37,
            ReadDir = 38,
            FStat = 39,
            FGetBlockSize = 40,
            Read = 47,
            Write = 48,
            GetDirEntries = 49,
            ReadOffset = 50,
            WriteOffset = 51,
            FTruncate = 52,
            FTruncateNoAlloc = 53,
            LSeek = 56,
            SetIOBuffer = 57,
            OfflineEnd = 9999,
        }

        public enum FileTraceNotificationStatus
        {
            Processed,
            Received,
            Waiting,
            Processing,
            Suspended,
            Finished,
        }

        public struct FileTraceLogData
        {
            public PS3TMAPI.FileTraceLogType1 LogType1;
            public PS3TMAPI.FileTraceLogType2 LogType2;
            public PS3TMAPI.FileTraceLogType3 LogType3;
            public PS3TMAPI.FileTraceLogType4 LogType4;
            public PS3TMAPI.FileTraceLogType6 LogType6;
            public PS3TMAPI.FileTraceLogType8 LogType8;
            public PS3TMAPI.FileTraceLogType9 LogType9;
            public PS3TMAPI.FileTraceLogType10 LogType10;
            public PS3TMAPI.FileTraceLogType11 LogType11;
            public PS3TMAPI.FileTraceLogType12 LogType12;
            public PS3TMAPI.FileTraceLogType13 LogType13;
            public PS3TMAPI.FileTraceLogType14 LogType14;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct FileTraceLogType1
        {
            public string Path;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct FileTraceLogType2
        {
            public string Path1;
            public string Path2;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct FileTraceLogType3
        {
            public ulong Arg;
            public string Path;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct FileTraceLogType4
        {
            public uint Mode;
            public string Path;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct FileTraceLogType6
        {
            public ulong Arg1;
            public ulong Arg2;
            public string Path;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct FileTraceProcessInfo
        {
            public ulong VFSID;
            public ulong FD;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct FileTraceLogType8
        {
            public PS3TMAPI.FileTraceProcessInfo ProcessInfo;
            public uint Arg1;
            public uint Arg2;
            public uint Arg3;
            public uint Arg4;
            public byte[] VArg;
            public string Path;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct FileTraceLogType9
        {
            public PS3TMAPI.FileTraceProcessInfo ProcessInfo;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct FileTraceLogType10
        {
            public PS3TMAPI.FileTraceProcessInfo ProcessInfo;
            public uint Size;
            public ulong Address;
            public uint TxSize;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct FileTraceLogType11
        {
            public PS3TMAPI.FileTraceProcessInfo ProcessInfo;
            public uint Size;
            public ulong Address;
            public ulong Offset;
            public uint TxSize;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct FileTraceLogType12
        {
            public PS3TMAPI.FileTraceProcessInfo ProcessInfo;
            public ulong TargetSize;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct FileTraceLogType13
        {
            public PS3TMAPI.FileTraceProcessInfo ProcessInfo;
            public uint Size;
            public ulong Offset;
            public ulong CurPos;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct FileTraceLogType14
        {
            public PS3TMAPI.FileTraceProcessInfo ProcessInfo;
            public uint MaxSize;
            public uint Page;
            public uint ContainerID;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct FileTraceEvent
        {
            public ulong SerialID;
            public PS3TMAPI.FileTraceType TraceType;
            public PS3TMAPI.FileTraceNotificationStatus Status;
            public uint ProcessID;
            public uint ThreadID;
            public ulong TimeBaseStartOfTrace;
            public ulong TimeBase;
            public byte[] BackTraceData;
            public PS3TMAPI.FileTraceLogData LogData;
        }

        public delegate void FileTraceCallback(int target, PS3TMAPI.SNRESULT res, PS3TMAPI.FileTraceEvent fileTraceEvent, object userData);

        private class FileTraceCallbackAndUserData
        {
            public PS3TMAPI.FileTraceCallback m_callback;
            public object m_userData;
        }

        public enum FileTransferStatus : uint
        {
            Pending = 1,
            Failed = 2,
            Succeeded = 4,
            Skipped = 8,
            InProgress = 16,
            Cancelled = 32,
        }

        public struct FileTransferInfo
        {
            public uint TransferID;
            public PS3TMAPI.FileTransferStatus Status;
            public string SourcePath;
            public string DestinationPath;
            public ulong Size;
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

        public enum DirEntryType : uint
        {
            Unknown,
            Directory,
            Regular,
            Symlink,
        }

        public struct DirEntry
        {
            public PS3TMAPI.DirEntryType Type;
            public uint Mode;
            public PS3TMAPI.Time AccessTime;
            public PS3TMAPI.Time ModifiedTime;
            public PS3TMAPI.Time CreateTime;
            public ulong Size;
            public string Name;
        }

        private struct DirEntryPriv
        {
            public uint Type;
            public uint Mode;
            public PS3TMAPI.Time AccessTime;
            public PS3TMAPI.Time ModifiedTime;
            public PS3TMAPI.Time CreateTime;
            public ulong Size;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public byte[] Name;
        }

        public struct DirEntryEx
        {
            public PS3TMAPI.DirEntryType Type;
            public uint Mode;
            public ulong AccessTimeUTC;
            public ulong ModifiedTimeUTC;
            public ulong CreateTimeUTC;
            public ulong Size;
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

        public struct TargetTimezone
        {
            public int Timezone;
            public int DST;
        }

        public enum ChModFilePermission : uint
        {
            ReadOnly = 256,
            ReadWrite = 384,
        }

        public enum LogCategory : uint
        {
            Off = 0,
            All = 4294967295,
        }

        public struct BDInfo
        {
            public uint bdemu_data_size;
            public byte bdemu_total_entry;
            public byte bdemu_selected_index;
            public byte image_index;
            public byte image_type;
            public string image_file_name;
            public ulong image_file_size;
            public string image_product_code;
            public string image_producer;
            public string image_author;
            public string image_date;
            public uint image_sector_layer0;
            public uint image_sector_layer1;
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

        public enum TargetEventType : uint
        {
            UnitStatusChange = 0,
            ResetStarted = 1,
            ResetEnd = 2,
            Details = 4,
            ModuleLoad = 5,
            ModuleRunning = 6,
            ModuleDoneRemove = 7,
            ModuleDoneResident = 8,
            ModuleStopped = 9,
            ModuleStoppedRemove = 10,
            PowerStatusChange = 11,
            TTYStreamAdded = 12,
            TTYStreamDeleted = 13,
            BDIsotransferStarted = 16,
            BDIsotransferFinished = 17,
            BDFormatStarted = 18,
            BDFormatFinished = 19,
            BDMountStarted = 20,
            BDMountFinished = 21,
            BDUnmountStarted = 22,
            BDUnmountFinished = 23,
            TargetSpecific = 2147483648,
        }

        public struct TGTEventUnitStatusChangeData
        {
            public PS3TMAPI.UnitType Unit;
            public PS3TMAPI.UnitStatus Status;
        }

        private struct TGTEventUnitStatusChangeDataPriv
        {
            public int Unit;
            public uint Status;
        }

        public struct TGTEventDetailsData
        {
            public uint Flags;
        }

        public struct TGTEventModuleEventData
        {
            public uint Unit;
            public uint ModuleID;
        }

        public struct TGTEventBDData
        {
            public uint Result;
            public string Source;
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

        public struct TargetEventData
        {
            public PS3TMAPI.TGTEventUnitStatusChangeData UnitStatusChangeData;
            public PS3TMAPI.TGTEventDetailsData DetailsData;
            public PS3TMAPI.TGTEventModuleEventData ModuleEventData;
            public PS3TMAPI.TGTEventBDData BdData;
        }

        public struct TargetEvent
        {
            public uint TargetID;
            public PS3TMAPI.TargetEventType Type;
            public PS3TMAPI.TargetEventData EventData;
            public PS3TMAPI.TargetSpecificEvent TargetSpecific;
        }

        public delegate void TargetEventCallback(int target, PS3TMAPI.SNRESULT res, PS3TMAPI.TargetEvent[] targetEventList, object userData);

        private class TargetCallbackAndUserData
        {
            public PS3TMAPI.TargetEventCallback m_callback;
            public object m_userData;
        }

        private struct TargetEventHdrPriv
        {
            public uint Size;
            public uint TargetID;
            public uint EventType;
        }

        public enum TargetSpecificEventType : uint
        {
            ProcessCreate = 0,
            ProcessExit = 1,
            ProcessKill = 2,
            ProcessExitSpawn = 3,
            PPUExcTrap = 16,
            PPUExcPrevInt = 17,
            PPUExcAlignment = 18,
            PPUExcIllInst = 19,
            PPUExcTextHtabMiss = 20,
            PPUExcTextSlbMiss = 21,
            PPUExcDataHtabMiss = 22,
            PPUExcFloat = 23,
            PPUExcDataSlbMiss = 24,
            PPUExcDabrMatch = 25,
            PPUExcStop = 26,
            PPUExcStopInit = 27,
            PPUExcDataMAT = 28,
            PPUThreadCreate = 32,
            PPUThreadExit = 33,
            SPUThreadStart = 48,
            SPUThreadStop = 49,
            SPUThreadStopInit = 50,
            SPUThreadGroupDestroy = 51,
            SPUThreadStopEx = 52,
            PRXLoad = 64,
            PRXUnload = 65,
            DAInitialised = 96,
            Footswitch = 112,
            InstallPackageProgress = 128,
            InstallPackagePath = 129,
            CoreDumpComplete = 256,
            CoreDumpStart = 257,
            RawNotify = 4026531855,
        }

        public struct PPUProcessCreateData
        {
            public uint ParentProcessID;
            public string Filename;
        }

        public struct PPUProcessExitData
        {
            public ulong ExitCode;
        }

        public struct PPUExceptionData
        {
            public ulong ThreadID;
            public uint HWThreadNumber;
            public ulong PC;
            public ulong SP;
        }

        public struct PPUAlignmentExceptionData
        {
            public ulong ThreadID;
            public uint HWThreadNumber;
            public ulong DSISR;
            public ulong DAR;
            public ulong PC;
            public ulong SP;
        }

        public struct PPUDataMatExceptionData
        {
            public ulong ThreadID;
            public uint HWThreadNumber;
            public ulong DSISR;
            public ulong DAR;
            public ulong PC;
            public ulong SP;
        }

        public struct PPUThreadCreateData
        {
            public ulong ThreadID;
        }

        public struct PPUThreadExitData
        {
            public ulong ThreadID;
        }

        public struct SPUThreadStartData
        {
            public uint ThreadGroupID;
            public uint ThreadID;
            public string ElfFilename;
        }

        public enum SPUThreadStopReason : uint
        {
            NoException = 0,
            DMAAlignment = 1,
            DMACommand = 2,
            Error = 4,
            MFCFIR = 8,
            MFCSegment = 16,
            MFCStorage = 32,
            NoValue = 64,
            StopCall = 256,
            StopDCall = 512,
            Halt = 1024,
        }

        public struct SPUThreadStopData
        {
            public uint ThreadGroupID;
            public uint ThreadID;
            public uint PC;
            public PS3TMAPI.SPUThreadStopReason Reason;
            public uint SP;
        }

        public struct SPUThreadStopExData
        {
            public uint ThreadGroupID;
            public uint ThreadID;
            public uint PC;
            public PS3TMAPI.SPUThreadStopReason Reason;
            public uint SP;
            public ulong MFCDSISR;
            public ulong MFCDSIPR;
            public ulong MFCDAR;
        }

        public struct SPUThreadGroupDestroyData
        {
            public uint ThreadGroupID;
        }

        public struct NotifyPRXLoadData
        {
            public ulong PPUThreadID;
            public uint PRXID;
            public ulong Timestamp;
        }

        public struct NotifyPRXUnloadData
        {
            public ulong PPUThreadID;
            public uint PRXID;
            public ulong Timestamp;
        }

        public struct FootswitchData
        {
            public ulong EventSource;
            public ulong EventData1;
            public ulong EventData2;
            public ulong EventData3;
            public ulong Reserved;
        }

        public struct InstallPackageProgress
        {
            public uint Percent;
        }

        public struct InstallPackagePath
        {
            public string Path;
        }

        public struct CoreDumpComplete
        {
            public string Filename;
        }

        public struct CoreDumpStart
        {
            public string Filename;
        }

        public struct TargetSpecificData
        {
            public PS3TMAPI.TargetSpecificEventType Type;
            public PS3TMAPI.PPUProcessCreateData PPUProcessCreate;
            public PS3TMAPI.PPUProcessExitData PPUProcessExit;
            public PS3TMAPI.PPUExceptionData PPUException;
            public PS3TMAPI.PPUAlignmentExceptionData PPUAlignmentException;
            public PS3TMAPI.PPUDataMatExceptionData PPUDataMatException;
            public PS3TMAPI.PPUThreadCreateData PPUThreadCreate;
            public PS3TMAPI.PPUThreadExitData PPUThreadExit;
            public PS3TMAPI.SPUThreadStartData SPUThreadStart;
            public PS3TMAPI.SPUThreadStopData SPUThreadStop;
            public PS3TMAPI.SPUThreadStopExData SPUThreadStopEx;
            public PS3TMAPI.SPUThreadGroupDestroyData SPUThreadGroupDestroyData;
            public PS3TMAPI.NotifyPRXLoadData PRXLoad;
            public PS3TMAPI.NotifyPRXUnloadData PRXUnload;
            public PS3TMAPI.FootswitchData Footswitch;
            public PS3TMAPI.InstallPackageProgress InstallPackageProgress;
            public PS3TMAPI.InstallPackagePath InstallPackagePath;
            public PS3TMAPI.CoreDumpStart CoreDumpStart;
            public PS3TMAPI.CoreDumpComplete CoreDumpComplete;
        }

        public struct TargetSpecificEvent
        {
            public uint CommandID;
            public uint RequestID;
            public uint ProcessID;
            public uint Result;
            public PS3TMAPI.TargetSpecificData Data;
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

        private delegate void HandleEventCallbackPriv(int target, PS3TMAPI.EventType type, uint param, PS3TMAPI.SNRESULT result, uint length, IntPtr data, IntPtr userData);

        public delegate void SearchTargetsCallback(string name, string type, PS3TMAPI.TCPIPConnectProperties ConnectInfo, object userData);

        private delegate void SearchTargetsCallbackPriv(IntPtr name, IntPtr type, IntPtr connectInfo, IntPtr userData);

        private class SearchForTargetsCallbackHandler
        {
            private PS3TMAPI.SearchTargetsCallback m_SearchForTargetCallback;
            private object m_UserData;

            public SearchForTargetsCallbackHandler(PS3TMAPI.SearchTargetsCallback callback, object userData)
            {
                this.m_SearchForTargetCallback = callback;
                this.m_UserData = userData;
            }

            public static void SearchForTargetsCallback(IntPtr namePtr, IntPtr typePtr, IntPtr connectInfoPtr, IntPtr userDataPtr)
            {
                PS3TMAPI.SearchForTargetsCallbackHandler target = (PS3TMAPI.SearchForTargetsCallbackHandler)GCHandle.FromIntPtr(userDataPtr).Target;
                PS3TMAPI.TCPIPConnectProperties ConnectInfo = (PS3TMAPI.TCPIPConnectProperties)null;
                if (connectInfoPtr != IntPtr.Zero)
                {
                    ConnectInfo = new PS3TMAPI.TCPIPConnectProperties();
                    Marshal.PtrToStructure(connectInfoPtr, (object)ConnectInfo);
                }
                string name = PS3TMAPI.Utf8ToString(namePtr, uint.MaxValue);
                if (name == "")
                    name = (string)null;
                string type = PS3TMAPI.Utf8ToString(typePtr, uint.MaxValue);
                target.m_SearchForTargetCallback(name, type, ConnectInfo, target.m_UserData);
            }
        }

        private class ScopedGlobalHeapPtr
        {
            private IntPtr m_intPtr = IntPtr.Zero;

            public ScopedGlobalHeapPtr(IntPtr intPtr)
            {
                this.m_intPtr = intPtr;
            }

            ~ScopedGlobalHeapPtr()
            {
                if (!(this.m_intPtr != IntPtr.Zero))
                    return;
                Marshal.FreeHGlobal(this.m_intPtr);
            }

            public IntPtr Get()
            {
                return this.m_intPtr;
            }
        }
    }
}