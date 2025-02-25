using System;
using System.Collections.Generic;
using WixSharp;

namespace Setup;

public class Build
{
    internal class Program
    {
        private const string ProjectName = "Kapibara";
        private const string Version = "1.0";
        private const string NameManufacturer = "FomenkoA";

        static void Main(string[] args)
        {
            var versions = new List<string> { "22", "23" };
            var patterns = new List<string>();

            foreach (var ver in versions)
            {
                var pattern = $@"..\KapibaraV2\bin\Release R{ver}\publish\Revit 20{ver} Release R{ver} addin\*.*";
                patterns.Add(pattern);
            }

            var subDirs = new List<Dir>();
            for (int i = 0; i < versions.Count; i++)
            {
                subDirs.Add(new Dir("20" + versions[i], new Files(patterns[i])));
            }

            var addinsDir = new Dir(@"%AppDataFolder%\Autodesk\Revit\Addins", subDirs.ToArray());

            var project = new Project
            {
                Name = ProjectName,
                UI = WUI.WixUI_ProgressOnly,
                OutDir = "output",
                GUID = new Guid("D56A3F69-DEB4-4332-B726-1DF06709DE7E"),
                MajorUpgrade = MajorUpgrade.Default,
                ControlPanelInfo = { Manufacturer = NameManufacturer },
                Dirs = new[] { addinsDir }
            };

            project.Version = new Version(Version);
            project.BuildMsi();
        }
    }
}