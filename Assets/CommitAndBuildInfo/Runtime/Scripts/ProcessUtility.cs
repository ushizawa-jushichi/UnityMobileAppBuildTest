//#define VERBOSE
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Debug = UnityEngine.Debug;

public static class ProcessUtility
{
    public static string GetStandardOutputFromProcess(string command, string args, int timeoutMilliseconds = -1)
    {
        ConfigureCommandAndArgs(ref command, ref args);
        ProcessStartInfo startInfo = CreateStartupInfo(command, args);

#if VERBOSE
        Debug.Log($"Command:{command} Args:{args}");
#endif
        using (Process process = Process.Start(startInfo))
        {
            if (timeoutMilliseconds >= 0)
            {
                process.WaitForExit(timeoutMilliseconds);
            }
            else
            {
                process.WaitForExit();
            }

            var stderrString = process.StandardError.ReadToEnd();
            if (!string.IsNullOrEmpty(stderrString))
            {
                Debug.LogError(stderrString);
            }
            var stdoutString = process.StandardOutput.ReadToEnd();
#if VERBOSE
            if (!string.IsNullOrEmpty(stdoutString))
            {
                Debug.Log(stdoutString);
            }
            Debug.Log($"ExitCode: {process.ExitCode}");
#endif
            process.Close();
            return stdoutString;
        }
    }

    public async static Task<string> GetStandardOutputFromProcessAsync(string command, string args, int timeoutMilliseconds = int.MaxValue)
    {
        ConfigureCommandAndArgs(ref command, ref args);
        ProcessStartInfo startInfo = CreateStartupInfo(command, args);

#if VERBOSE
        Debug.Log($"Command:{command} Args:{args}");
#endif
        using (Process process = Process.Start(startInfo))
        {
            await Task.Run(() =>
            {
                if (timeoutMilliseconds >= 0)
                {
                    process.WaitForExit(timeoutMilliseconds);
                }
                else
                {
                    process.WaitForExit();
                }
            });

            var stderrString = (await process.StandardError.ReadToEndAsync());
            if (!string.IsNullOrEmpty(stderrString))
            {
                Debug.LogError(stderrString);
            }
            var stdoutString = (await process.StandardOutput.ReadToEndAsync());
#if VERBOSE
            if (!string.IsNullOrEmpty(stdoutString))
            {
                Debug.Log(stdoutString);
            }
            Debug.Log($"ExitCode: {process.ExitCode}");
#endif
            process.Close();
            return stdoutString;
        }
    }

    static void ConfigureCommandAndArgs(ref string command, ref string args)
    {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || UNITY_ANDROID
        command = command.Replace("/", @"\\");
#else
        args = $"-c '{command} {args}'";
        command = "/bin/bash";
#endif
    }

    static ProcessStartInfo CreateStartupInfo(string command, string args)
    {
        return new ProcessStartInfo()
        {
            FileName = command,
            Arguments = args,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            StandardOutputEncoding = Encoding.UTF8,
            StandardErrorEncoding = Encoding.UTF8,
            CreateNoWindow = true,
            WindowStyle = ProcessWindowStyle.Hidden,
#if !(UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || UNITY_ANDROID)
            WorkingDirectory = Application.dataPath + "..",
#endif
        };
    }
}
