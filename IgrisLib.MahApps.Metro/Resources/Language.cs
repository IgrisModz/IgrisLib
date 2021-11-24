using System;
using System.Threading;
using System.Windows;

namespace IgrisLib.Resources
{
    /// <summary>
    /// 
    /// </summary>
    public class Language
    {
        private static ResourceDictionary get = null;
        private static object _lock = new object();

        /// <summary>
        /// Return the resources from the current language
        /// </summary>
        public static ResourceDictionary Get
        {
            get
            {
                lock (_lock)
                {
                    if (get == null)
                    {
                        get = new ResourceDictionary();
                        ResourceDictionary dict = new ResourceDictionary();
                        switch (Thread.CurrentThread.CurrentCulture.ToString())
                        {
                            case "fr-FR":
                            case "fr-BE":
                            case "fr-CA":
                            case "fr-CH":
                            case "fr-LU":
                            case "fr-MC":
                                get.Source = new Uri("pack://application:,,,/IgrisLib.MahApps.Metro;Component/Resources/fr-FR.xaml", UriKind.Absolute);
                                break;
                            case "es-ES":
                            case "es-MX":
                            case "es-GT":
                            case "es-CR":
                            case "es-PA":
                            case "es-DO":
                            case "es-VE":
                            case "es-CO":
                            case "es-PE":
                            case "es-AR":
                            case "es-EC":
                            case "es-CL":
                            case "es-UY":
                            case "es-PY":
                            case "es-BO":
                            case "es-SV":
                            case "es-HN":
                            case "es-NI":
                            case "es-PR":
                                get.Source = new Uri("pack://application:,,,/IgrisLib.MahApps.Metro;Component/Resources/es-ES.xaml", UriKind.Absolute);
                                break;
                            case "de-DE":
                            case "de-CH":
                            case "de-HK":
                            case "de-LU":
                            case "de-LI":
                                get.Source = new Uri("pack://application:,,,/IgrisLib.MahApps.Metro;Component/Resources/de-DE.xaml", UriKind.Absolute);
                                break;
                            default:
                                get.Source = new Uri("pack://application:,,,/IgrisLib.MahApps.Metro;Component/Resources/en-US.xaml", UriKind.Absolute);
                                break;
                        }
                        get.MergedDictionaries.Add(dict);
                    }
                    return get;
                }
            }
        }

        private Language() { }
    }
}
