using System;
using System.Reflection;
using System.Text.RegularExpressions;

namespace blackhouse.Umbraco.Installer.DocTypes
{
    internal class UdtAssemblyFile
    {
        
        #region Fields

        private readonly static Regex FileVersionRegex = new Regex(@"ver((\.\d+){2,4})(\.[a-zA-Z\-]+)*.udt$", RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Compiled);

        #endregion

        #region Properties

        public Assembly Assembly { get; private set; }

        public string FullFileName { get; private set; }

        public Version FileVersion { get; private set; }

        public string FileMark { get; private set; }

        public bool IsValid { get; private set; }

        #endregion

        #region Constructors

        public UdtAssemblyFile(Assembly assembly, string fullFileName) {
            this.Assembly = assembly;
            this.FullFileName = fullFileName;
            var match = FileVersionRegex.Match(fullFileName);
            if (!match.Success) return;
            this.FileVersion = Version.Parse(match.Groups[1].Value.Trim('.'));
            if (match.Length >= 4 && match.Groups[3].Success)
                this.FileMark = match.Groups[3].Value.Trim('.');
            this.IsValid = true;
        }

        #endregion

    }
}
