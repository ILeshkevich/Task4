using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LibGit2Sharp;

namespace GitTool
{
    // Todo: rename to GitService
    // todo: split Clone usage into two methods: Clone and Pull.
    // Avoid code duplication. move common code into separate methods / classes.
    public class Git
    {
        private readonly string login;
        private readonly string password;

        public Git(string login, string password)
        {
            this.login = login;
            this.password = password;
        }

        public async Task<bool> Clone(string url, string path)
        {
            // try move this into separate thread
            bool result = false;
            new Thread(() =>
            {
                try
                {
                    var co = new CloneOptions
                    {
                        CredentialsProvider = GetCredentials,
                    };
                    Repository.Clone(url, path, co);
                    result = true;
                }
                catch
                {
                    result = false;
                }
            });
            return result;
        }

        // url, usernameFromUrl, types
        private UsernamePasswordCredentials GetCredentials(string url, string usernameFromUrl, SupportedCredentialTypes types)
        {
            return new UsernamePasswordCredentials
            {
                Username = login,
                Password = password,
            };
        }

        // looks good to move it to separate class with a couple of small clear functions
        public Dictionary<string, int> GetFiles(string path)
        {
            var files = new List<string>();
            try
            {
                using var repo = new Repository(path);
                foreach (var commit in repo.Commits)
                {
                    foreach (var parent in commit.Parents)
                    {
                        files.AddRange(repo.Diff.Compare<TreeChanges>(parent.Tree, commit.Tree).Select(change => change.Path));
                    }

                    if (files.Count == 0)
                    {
                        files.AddRange(commit.Tree.Select(file => file.Path));
                    }
                }

                DeleteReadOnlyDirectory(path);
            }
            catch (Exception e)
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine(e.Message);
                Console.ResetColor();
            }

            if (files.Count != 0)
            {
                var result = files.GroupBy(f => f)
                    .OrderByDescending(f => f.Count())
                    .ToDictionary(f => f.Key, f => f.Count());
                return result;
            }

            return null;
        }


        [Obsolete("currently avoid it usage. In fact it doesn't work. And we are going to cache our cloned repositories")]
        private static void DeleteReadOnlyDirectory(string directory)
        {
            foreach (var subdirectory in Directory.EnumerateDirectories(directory))
            {
                DeleteReadOnlyDirectory(subdirectory);
            }

            foreach (var fileName in Directory.EnumerateFiles(directory))
            {
                var fileInfo = new FileInfo(fileName);
                fileInfo.Attributes = FileAttributes.Normal;
                fileInfo.Delete();
            }

            Directory.Delete(directory);
        }
    }
}
