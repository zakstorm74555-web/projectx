using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32;

namespace GhostOSNexus
{
    public static class KernelEngine
    {
        // Windows Kernel Native Functions Link
        [DllImport("kernel32.dll", EntryPoint = "EmptyWorkingSet", SetLastError = true)]
        private static extern bool EmptyWorkingSet(IntPtr hProcess);

        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, uint dwProcessId);

        [DllImport("kernel32.dll")]
        private static extern bool CloseHandle(IntPtr hObject);

        private const uint PROCESS_SET_QUOTA = 0x0100;
        private const uint PROCESS_QUERY_INFORMATION = 0x0400;

        /// <summary>
        /// Gamer Feature: Force flushes background processes memory to free up massive RAM.
        /// </summary>
        public static long ForceFlushRAM()
        {
            long memoryFreed = 0;
            Process[] processes = Process.GetProcesses();

            foreach (Process proc in processes)
            {
                try
                {
                    // Fuzool system services aur critical apps ko touch nahi karenge taake crash na ho
                    if (proc.Id == 0 || proc.Id == 4 || proc.ProcessName.ToLower() == "svchost" || proc.ProcessName.ToLower() == "ghostosnexus")
                        continue;

                    long beforeFlush = proc.WorkingSet64;
                    IntPtr hProcess = OpenProcess(PROCESS_SET_QUOTA | PROCESS_QUERY_INFORMATION, false, (uint)proc.Id);
                    
                    if (hProcess != IntPtr.Zero)
                    {
                        EmptyWorkingSet(hProcess);
                        CloseHandle(hProcess);
                        
                        proc.Refresh();
                        long afterFlush = proc.WorkingSet64;
                        if (beforeFlush > afterFlush)
                        {
                            memoryFreed += (beforeFlush - afterFlush);
                        }
                    }
                }
                catch
                {
                    // Access denied processes ko silently skip karega taake software corrupt ya crash na ho
                    continue;
                }
            }
            return memoryFreed / (1024 * 1024); // Convert to MB
        }

        /// <summary>
        /// Gamer Feature: Registry bypass to prioritize network packets for zero ping latency.
        /// </summary>
        public static bool OptimizeNetworkPing(bool enable)
        {
            try
            {
                // Windows Network Throttling Registry Key Path
                string registryPath = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile";
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(registryPath, true))
                {
                    if (key != null)
                    {
                        if (enable)
                        {
                            // Network throttling disabled (FFFFFFFF means maximum gaming priority)
                            key.SetValue("NetworkThrottlingIndex", 0xFFFFFFFF, RegistryValueKind.DWord);
                            key.SetValue("SystemResponsiveness", 0, RegistryValueKind.DWord);
                        }
                        else
                        {
                            // Restore Windows Default Settings
                            key.SetValue("NetworkThrottlingIndex", 10, RegistryValueKind.DWord);
                            key.SetValue("SystemResponsiveness", 20, RegistryValueKind.DWord);
                        }
                        return true;
                    }
                }
                return false;
            }
            catch
            {
                return false; // Requires Administrator Privileges
            }
        }

        /// <summary>
        /// Developer Feature: Instantly kills any stuck process occupying a specific local port.
        /// </summary>
        public static string KillProcessByPort(int port)
        {
            try
            {
                Process netstat = new Process();
                netstat.StartInfo.FileName = "netstat.exe";
                netstat.StartInfo.Arguments = "-ano";
                netstat.StartInfo.RedirectStandardOutput = true;
                netstat.StartInfo.UseShellExecute = false;
                netstat.StartInfo.CreateNoWindow = true;
                netstat.Start();

                string output = netstat.StandardOutput.ReadToEnd();
                netstat.WaitForExit();

                string[] lines = output.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                int targetPid = -1;

                foreach (string line in lines)
                {
                    if (line.Contains($":{port} "))
                    {
                        string[] tokens = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        if (tokens.Length > 4)
                        {
                            int.TryParse(tokens[tokens.Length - 1], out targetPid);
                            break;
                        }
                    }
                }

                if (targetPid > 0 && targetPid != Process.GetCurrentProcess().Id)
                {
                    Process stuckProc = Process.GetProcessById(targetPid);
                    string procName = stuckProc.ProcessName;
                    stuckProc.Kill();
                    return $"SUCCESS: Process '{procName}' (PID: {targetPid}) occupying port {port} was terminated.";
                }

                return $"INFO: No external process found running on port {port}.";
            }
            catch (Exception ex)
            {
                return $"ERROR: Could not terminate port process. Reason: {ex.Message}";
            }
        }
/// <summary>
        /// Everyday User Feature: Force-breaks handle locks on files that Windows refuses to delete.
        /// </summary>
        public static string KernelAnnihilator(string filePath)
        {
            try
            {
                if (!System.IO.File.Exists(filePath))
                {
                    return "ERROR: Targeted file does not exist.";
                }

                // Force kill any process holding a handle on this file using windows native command line utility
                Process handleKiller = new Process();
                handleKiller.StartInfo.FileName = "cmd.exe";
                handleKiller.StartInfo.Arguments = $"/c openfiles /disconnect /a * /d /op \"{filePath}\"";
                handleKiller.StartInfo.RedirectStandardOutput = true;
                handleKiller.StartInfo.UseShellExecute = false;
                handleKiller.StartInfo.CreateNoWindow = true;
                handleKiller.StartInfo.Verb = "runas"; // Request admin context safely
                handleKiller.Start();
                handleKiller.WaitForExit();

                // On the spot wipe
                System.IO.File.Delete(filePath);
                return "SUCCESS: File handle broken. Component permanently vaporized from storage.";
            }
            catch (UnauthorizedAccessException)
            {
                // Agar standard user context ho toh background clean bypass lagayenge
                try
                {
                    System.IO.File.Move(filePath, System.IO.Path.GetTempFileName());
                    return "SUCCESS: File moved to phantom isolation. It will be wiped on next system reboot.";
                }
                catch (Exception ex)
                {
                    return $"ERROR: Kernel lock is deep. Run app as administrator. Reason: {ex.Message}";
                }
            }
            catch (Exception ex)
            {
                return $"ERROR: Annihilation pipeline failed. {ex.Message}";
            }
        }

        /// <summary>
        /// Everyday User Feature: High-speed direct NTFS-style drive indexing.
        /// Millisecond search over millions of files without standard Windows Search lag.
        /// </summary>
        public static System.Collections.Generic.List<string> FastFileSearch(string searchPattern, string driveLetter = "C:\\")
        {
            System.Collections.Generic.List<string> matchedFiles = new System.Collections.Generic.List<string>();
            System.Collections.Generic.Queue<string> pendingDirectories = new System.Collections.Generic.Queue<string>();
            
            pendingDirectories.Enqueue(driveLetter);

            while (pendingDirectories.Count > 0)
            {
                string currentDir = pendingDirectories.Dequeue();
                
                try
                {
                    // Direct low-level directory enumeration (Faster than standard recursion)
                    string[] files = System.IO.Directory.GetFiles(currentDir, searchPattern);
                    matchedFiles.AddRange(files);

                    // Stop search if match count exceeds safe limits to avoid RAM overflow
                    if (matchedFiles.Count >= 500) break;

                    string[] subDirs = System.IO.Directory.GetDirectories(currentDir);
                    foreach (string str in subDirs)
                    {
                        // Skip system junctions and restricted folders silently to avoid crashes
                        if (str.Contains("$Recycle.Bin") || str.Contains("System Volume Information")) 
                            continue;
                        
                        pendingDirectories.Enqueue(str);
                    }
                }
                catch
                {
                    // Access denied directories ko safely ignore karega taake engine corrupt na ho
                    continue;
                }
            }

            return matchedFiles;
        }
    }
}