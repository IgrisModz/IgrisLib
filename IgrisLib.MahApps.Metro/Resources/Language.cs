using System;
using System.Threading;
using System.Windows;

namespace IgrisLib.Resources
{
    internal static class Language
    {
        internal static ResourceDictionary SetLanguageDictionary()
        {
            ResourceDictionary dict = new ResourceDictionary();
            ResourceDictionary dict2 = new ResourceDictionary();
            switch (Thread.CurrentThread.CurrentCulture.ToString())
            {
                case "en-US":
                case "en-GB":
                case "en-AU":
                case "en-CA":
                case "en-ZN":
                case "en-IE":
                case "en-ZA":
                case "en-JM":
                case "en-029":
                case "en-BZ":
                case "en-TT":
                case "en-ZW":
                case "en-PH":
                    dict.Source = new Uri("pack://application:,,,/IgrisLib.MahApps.Metro;Component/Resources/en-US.xaml", UriKind.Absolute);
                    break;
                case "fr-FR":
                case "fr-BE":
                case "fr-CA":
                case "fr-CH":
                case "fr-LU":
                case "fr-MC":
                    dict.Source = new Uri("pack://application:,,,/IgrisLib.MahApps.Metro;Component/Resources/fr-FR.xaml", UriKind.Absolute);
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
                    dict.Source = new Uri("pack://application:,,,/IgrisLib.MahApps.Metro;Component/Resources/es-ES.xaml", UriKind.Absolute);
                    break;
                case "de-DE":
                case "de-CH":
                case "de-HK":
                case "de-LU":
                case "de-LI":
                    dict.Source = new Uri("pack://application:,,,/IgrisLib.MahApps.Metro;Component/Resources/de-DE.xaml", UriKind.Absolute);
                    break;
                default:
                    dict.Source = new Uri("pack://application:,,,/IgrisLib.MahApps.Metro;Component/Resources/en-US.xaml", UriKind.Absolute);
                    break;
            }
            dict2.MergedDictionaries.Add(dict);
            return dict2;
        }
    }
}
