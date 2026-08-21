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
            using var baseKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64)
                .OpenSubKey(RunKeyPath, true);

            if (baseKey == null)
                return;

            baseKey.SetValue(AppRegistryValue, Assembly.GetEntryAssembly().Location);
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
            using var baseKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64)
                .OpenSubKey(RunKeyPath, true);

            if (baseKey == null)
                return;

            baseKey.DeleteValue(AppRegistryValue, false);
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
            using var baseKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64)
                .OpenSubKey(RunKeyPath, true);

            if (baseKey == null)
                return false;

            var value = baseKey.GetValue(AppRegistryValue);
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
