using System.Reflection;
using System.Runtime.InteropServices;
using Android.App;
using devoctomy.DFramework.Logging.Attributes;

[assembly: AssemblyTitle("cachy.Android")]
[assembly: AssemblyDescription("Cross-platform secure credential cache.  Universal Android edition.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("devoctomy")]
[assembly: AssemblyProduct("cachy.Android")]
[assembly: AssemblyCopyright("Copyright © devoctomy 2018-2019")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]
[assembly: AssemblyVersion("1.0.*")]
[assembly: UsesPermission(Android.Manifest.Permission.Internet)]
[assembly: UsesPermission(Android.Manifest.Permission.WriteExternalStorage)]
[assembly: UsesPermission(Android.Manifest.Permission.ReadExternalStorage)]
[assembly: DLoggerAppNameAttribute("cachy")]
