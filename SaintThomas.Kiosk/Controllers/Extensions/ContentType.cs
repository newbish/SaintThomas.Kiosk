using System;
using System.IO;
using System.Linq;
using Microsoft.Win32;

namespace SaintThomas.Kiosk.Controllers.Extensions
{
    public static class ContentType
    {
        /// <summary>
        /// Gets the MIME type corresponding to the extension of the specified file name.
        /// </summary>
        /// <param name="fileName">The file name to determine the MIME type for.</param>
        /// <returns>The MIME type corresponding to the extension of the specified file name, if found; otherwise, null.</returns>
        public static string GetContentType(this string fileName)
        {
            var extension = Path.GetExtension(fileName);
 
            if (String.IsNullOrWhiteSpace(extension))
            {
                return null;
            }
 
            var registryKey = Registry.ClassesRoot.OpenSubKey(extension);
 
            if (registryKey == null)
            {
                return null;
            }
 
            var value = registryKey.GetValue("Content Type") as string;
             
            return String.IsNullOrWhiteSpace(value) ? null : value;
        }
    }
}