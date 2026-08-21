using Microsoft.Win32;
using System;
using System.Reflection;

namespace HonkaiImpactRpc;

internal static class AutoStart
{
    private const string RunKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
    private const string AppRegistryValue = "HonkaiImpact3-DiscordRpc";

    public static void Set()
    {
        try
        {
            using var key = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64)
                .OpenSubKey(RunKeyPath, true);

            if (key == null)
                return;

            key.SetValue(AppRegistryValue, Assembly.GetEntryAssembly().Location);
        }
        catch (Exception e)
        {
            DebugPrint($"Failed to set autostartup: {e.Message}");
        }
    }

    public static void Remove()
    {
        try
        {
            using var key = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64)
                .OpenSubKey(RunKeyPath, true);

            if (key == null)
                return;

            key.DeleteValue(AppRegistryValue, false);
        }
        catch (Exception e)
        {
            DebugPrint($"Failed to remove autostartup: {e.Message}");
        }
    }

    public static bool Check()
    {
        try
        {
            using var key = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64)
                .OpenSubKey(RunKeyPath, false);

            if (key == null)
                return false;

            var value = key.GetValue(AppRegistryValue);
            var exe = Assembly.GetEntryAssembly().Location;
            return exe.Equals(value);
        }
        catch (Exception e)
        {
            DebugPrint($"Failed to check autostartup: {e.Message}");
        }

        return false;
    }

    private static void DebugPrint(string message)
    {
        System.Diagnostics.Debug.Print(message);
    }
}
