namespace GitTool.Services
{
    public static class StringExtension
    {
        /// <summary>
        /// Convert GitHub url to file name. <example>https://github.com/user/repo.git to user/repo</example>
        /// </summary>
        /// <param name="str">GitHub url. <example>https://github.com/user/repo.git</example></param>
        /// <param name="baseHostUrl">Base url host <example>https://github.com/</example></param>
        /// <returns>String file name.</returns>
        public static string GetGitRepositoryName(this string str, string baseHostUrl = @"https://github.com/")
        {
            // It's better to use string builder for multiple replaces
            return str
                .Replace(baseHostUrl, string.Empty)
                .Replace(@".git", string.Empty);
        }
    }
}
