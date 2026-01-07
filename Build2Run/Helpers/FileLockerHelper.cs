using System;
using System.Diagnostics;
using System.Runtime.InteropServices;


public static class FileLockerHelper
{
    public static Dictionary<int, string> GetLockerProcess(string filePath)
    {
        int result = RmStartSession(out uint handle, 0, Guid.NewGuid().ToString());
        if (result != 0)
            throw new Exception($"RmStartSession failed: {result}");

        try
        {
            result = RmRegisterResources(
                handle,
                1,
                new[] { filePath },
                0,
                null,
                0,
                null);

            if (result != 0)
                throw new Exception($"RmRegisterResources failed: {result}");

            uint needed = 0;
            uint count = 0;
            uint reason = 0;
            
            result = RmGetList(handle, out needed, ref count, null, ref reason);
            if (result != 0 && result != 234)
                throw new Exception($"RmGetList(1) failed: {result}");
            
            var lockProcesses = new Dictionary<int, string>();
            
            if (needed == 0)
                return lockProcesses;

            var processes = new RM_PROCESS_INFO[needed];
            count = needed;
            
            result = RmGetList(handle, out needed, ref count, processes, ref reason);
            if (result != 0)
                throw new Exception($"RmGetList(2) failed: {result}");
            
            foreach (var p in processes)
                lockProcesses.TryAdd(p.Process.dwProcessId, p.strAppName);
            
            return lockProcesses;
        }
        finally
        {
            RmEndSession(handle);
        }
    }

    public static void CloseProcess(int processId)
    {
        try
        {
            var proc = Process.GetProcessById(processId);

            if (proc.MainWindowHandle != IntPtr.Zero)
                proc.CloseMainWindow();
            else
                proc.Kill();
        }
        catch (Exception ex)
        {
        }
    }

    // ===== WinAPI =====

    [DllImport("rstrtmgr.dll", CharSet = CharSet.Unicode)]
    static extern int RmStartSession(out uint handle, int flags, string key);

    [DllImport("rstrtmgr.dll", CharSet = CharSet.Unicode)]
    static extern int RmRegisterResources(
        uint handle,
        uint fileCount,
        string[] files,
        uint processCount,
        RM_UNIQUE_PROCESS[] processes,
        uint serviceCount,
        string[] services);

    [DllImport("rstrtmgr.dll", CharSet = CharSet.Unicode)]
    static extern int RmGetList(
        uint handle,
        out uint needed,
        ref uint count,
        [In, Out] RM_PROCESS_INFO[] processes,
        ref uint reason);

    [DllImport("rstrtmgr.dll")]
    static extern int RmEndSession(uint handle);

    // ===== Structs =====

    [StructLayout(LayoutKind.Sequential)]
    struct FILETIME
    {
        public uint dwLowDateTime;
        public uint dwHighDateTime;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct RM_UNIQUE_PROCESS
    {
        public int dwProcessId;
        public FILETIME ProcessStartTime;
    }

    enum RM_APP_TYPE
    {
        RmUnknownApp = 0,
        RmMainWindow = 1,
        RmOtherWindow = 2,
        RmService = 3,
        RmExplorer = 4,
        RmConsole = 5,
        RmCritical = 1000
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    struct RM_PROCESS_INFO
    {
        public RM_UNIQUE_PROCESS Process;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string strAppName;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string strServiceShortName;

        public RM_APP_TYPE ApplicationType;
        public uint AppStatus;
        public uint TSSessionId;

        [MarshalAs(UnmanagedType.Bool)]
        public bool Restartable;
    }
}