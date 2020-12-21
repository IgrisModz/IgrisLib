using Microsoft.Win32;
using System;

namespace IgrisLib
{
    public class ConsoleRegistry
    {
        public static string RegistryName => @"FrenchModdingTeam\CCAPI\Consoles";

        public static void Create()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);
            string[] registryName = RegistryName.Split('\\');
            foreach (var r in registryName)
            {
                key.CreateSubKey(r);
                key = key.OpenSubKey(r, true);
            }
            key.Close();
        }

        public static void Add(string name, string ip)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);
            key.CreateSubKey(RegistryName);
            key = key.OpenSubKey(RegistryName, true);
            if (Registry.GetValue($@"HKEY_CURRENT_USER\SOFTWARE\{RegistryName}", name, null) == null)
            {
                key.CreateSubKey(name);
                key.SetValue(name, ip);
            }
        }

        public static string Read(string name)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);
            key = key.OpenSubKey(RegistryName, true);
            string cleaning = (key.GetValue(name) as string).Replace("{00000000-0000-0000-0000-000000000000}", "").Replace("%UserProfile%", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
            return cleaning.Contains("|") ? cleaning.Remove(cleaning.IndexOf("|")) : cleaning;
        }

        public static void Write(string name, string ip)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);
            key = key.OpenSubKey(RegistryName, true);
            key.SetValue(name, ip);
        }

        public static void Delete(string name)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);
            key = key.OpenSubKey(RegistryName, true);
            key.DeleteValue(name);
        }
    }
}
