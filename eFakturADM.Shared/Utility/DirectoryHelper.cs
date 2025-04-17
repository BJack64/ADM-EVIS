using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace eFakturADM.Shared.Utility
{
    public static class DirectoryHelper
    {
        public static DirectoryInfo TryGetSolutionDirectoryInfo(string currentPath = null)
        {
            var directory = new DirectoryInfo(
                currentPath ?? Directory.GetCurrentDirectory());
            while (directory != null && !directory.GetFiles("*.sln").Any())
            {
                directory = directory.Parent;
            }
            return directory;
        }
    }
}
