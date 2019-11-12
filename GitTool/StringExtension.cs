using System.Text;

namespace GitTool.Services
{
    public static class StringExtension
    {
        /// <summary>
        /// Convert GitHub url to file name. <example>https://github.com/user/repo.git to user/repo</example>
        /// </summary>
        /// <param name="str">GitHub url. <example>https://github.com/user/repo.git</example></param>
        /// <param name="baseHostUrl">Base url host. <example>https://github.com/</example></param>
        /// <returns>String file name.</returns>
        public static string GetGitRepositoryName(this string str, string baseHostUrl = @"https://github.com/")
        {
            var result = new StringBuilder(str, 50);
            result = result
                .Replace(baseHostUrl, string.Empty)
                .Replace(@".git", string.Empty);
         
            return result.ToString();
        }
    }
}
