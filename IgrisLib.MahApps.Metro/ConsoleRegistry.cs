using Microsoft.Win32;
using System;

namespace IgrisLib
{
    public class ConsoleRegistry
    {
        public static string registryName => @"FrenchModdingTeam\CCAPI\Consoles";

        public static void Create(string name, string ip)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);
            key.CreateSubKey(registryName);
            key = key.OpenSubKey(registryName, true);
            if (Registry.GetValue($@"HKEY_CURRENT_USER\SOFTWARE\{registryName}", name, null) == null)
            {
                key.CreateSubKey(name);
                key.SetValue(name, ip);
            }
        }

        public static string Read(string name)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);
            key = key.OpenSubKey(registryName, true);
            string cleaning = (key.GetValue(name) as string).Replace("{00000000-0000-0000-0000-000000000000}", "").Replace("%UserProfile%", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
            return cleaning.Contains("|") ? cleaning.Remove(cleaning.IndexOf("|")) : cleaning;
        }

        public static void Write(string name, string ip)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);
            key = key.OpenSubKey(registryName, true);
            key.SetValue(name, ip);
        }

        public static void Delete(string name)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);
            key = key.OpenSubKey(registryName, true);
            key.DeleteValue(name);
        }
    }
}
