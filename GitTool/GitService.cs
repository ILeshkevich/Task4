using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LibGit2Sharp;
using LibGit2Sharp.Handlers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace GitTool
{
    public class GitService : IGitService
    {
        private const string FolderName = "Repositories";
        private readonly string login;
        private readonly string password;
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly IConfiguration Configuration;

        public GitService(IHostingEnvironment environment, IConfiguration configuration)
        {
            this.login = login;
            this.password = password;
            this.hostingEnvironment = environment;
            login = configuration.("GitHub:Login");
            password = configuration.GetValue<string>("GitHub:Password");
        }

        /// <inheritdoc/>
        public async Task<bool> CloneAsync(string url)
        {
            var path = GetRepositoryPath(url);
            try
            {
                await Task.Run(() => CloneDoWork(url, path));
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public void Pull(Repository repo)
        {
            // Credential information to fetch
            var options = new PullOptions
            {
                FetchOptions = new FetchOptions { CredentialsProvider = new CredentialsHandler(GetCredentials) },
            };

            var signature = new Signature(
                new Identity(login, login), DateTimeOffset.Now);

            // Pull
            Commands.Pull(repo, signature, options);
        }

        // looks good to move it to separate class with a couple of small clear functions
        // add logger

        /// <inheritdoc/>
        public Dictionary<string, int> GetFiles(string path)
        {
            var files = new List<string>();
            try
            {
                using var repo = new Repository(path);

                Pull(repo);

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
            }
            catch
            {
                // ignored
            }

            if (files.Count == 0)
            {
                return null;
            }

            var result = files.GroupBy(f => f)
                .OrderByDescending(f => f.Count())
                .ToDictionary(f => f.Key, f => f.Count());
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

        private void CloneDoWork(string url, string path)
        {
            var co = new CloneOptions
            {
                CredentialsProvider = GetCredentials,
            };
            Repository.Clone(url, path, co);
        }
        
        private string GetRepositoryPath(string fileName)
        {
            var uploads = Path.Combine(hostingEnvironment.WebRootPath, FolderName);
            var repoPath = Path.Combine(uploads, fileName);
            return repoPath.Replace("/", "\\");
        }
    }
}
