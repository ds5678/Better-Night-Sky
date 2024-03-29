﻿using MelonLoader;
using System.Reflection;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Better-Night-Sky")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("Better-Night-Sky")]
[assembly: AssemblyCopyright("© 2021 Wulf Marius, ds5678")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("7888042b-8c0d-4540-be29-a5ea736cc620")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("5.0.0")]
[assembly: AssemblyFileVersion("5.0.0")]
[assembly: MelonInfo(typeof(BetterNightSky.Implementation), BuildInfo.ModName, BuildInfo.ModVersion, BuildInfo.ModAuthor)]
[assembly: MelonGame("Hinterland", "TheLongDark")]

internal static class BuildInfo
{
    internal const string ModName = "Better-Night-Sky";
    internal const string ModAuthor = "Wulf Marius, ds5678";
    internal const string ModVersion = "5.0.0";
}
