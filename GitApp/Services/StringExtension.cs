using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GitApp.Services
{
    public static class StringExtension
    {
        public static string GitFileName(this string str)
        {
            return str.Replace(@"https://github.com/", string.Empty).Replace(@".git", string.Empty);
        }
    }
}
