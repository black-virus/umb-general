using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using blackhouse.Installer;
using Umbraco.Core.Services;

namespace blackhouse.Umbraco.Installer.DocTypes
{
    internal class AssemblyDocTypeInstall : InstallBase
    {


        #region Fields

        private readonly Assembly assembly;
        private IEnumerable<UdtAssemblyFile> files;
        private readonly ServiceContext services;

        #endregion

        #region Properties

        private IEnumerable<UdtAssemblyFile> AssemblyFiles {
            get {
                if (this.files != null) return this.files;
                this.files = this.GetUdtFiles();
                return this.files;
            }
        }

        public bool HaveSqlFiles {
            get { return this.AssemblyFiles.Any(); }
        }

        #endregion

        #region Constructors

        public AssemblyDocTypeInstall(Assembly assembly, ServiceContext services)
            : base("Umbraco DocTypes - " + assembly.GetName().Name) {
            this.assembly = assembly;
            this.services = services;
        }

        #endregion

        #region Methods
    
        protected override Version GetLatestVersion() {
            return this.AssemblyFiles
                .OrderByDescending(file => file.FileVersion)
                .First()
                .FileVersion;
        }

        protected override bool UpToVersion(Version version) {
            var fileToRun = this.AssemblyFiles.FirstOrDefault(file => file.FileVersion == version);
            if (fileToRun == null) return false;
            try {
                this.RunPackingInstaller(fileToRun.FullFileName);
            } catch (Exception exc) {
                Trace.Write(exc);
                return false;
            }
            return true;
        }

        private void RunPackingInstaller(string fullFileName) {
            XElement root;
            using (var st = this.assembly.GetManifestResourceStream(fullFileName)) {
                root = XElement.Load(st);
            }
            this.services.PackagingService.ImportContentTypes(root);
        }

        private IEnumerable<UdtAssemblyFile> GetUdtFiles() {
            return this.assembly.GetManifestResourceNames()
                .Where(s => s.EndsWith(".udt"))
                .Select(s => new UdtAssemblyFile(assembly, s))
                .Where(af => af.IsValid);
        }

        #endregion

    }
}
