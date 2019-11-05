namespace GitApp.Services
{
    public static class StringExtension
    {
        /// <summary>
        /// Convert GitHub url to file name. <example>https://github.com/user/repo.git to user/repo</example>
        /// </summary>
        /// <param name="str">GitHub url. <example>https://github.com/user/repo.git</example></param>
        /// <returns>String file name.</returns>
        public static string ToGitFileName(this string str)
        {
            return str.Replace(@"https://github.com/", string.Empty).Replace(@".git", string.Empty);
        }
    }
}
