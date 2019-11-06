using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using GitTool.Services;
using LibGit2Sharp;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace GitTool
{
    public class GitService : IGitService
    {
        private const string FolderName = @"wwwroot\Repositories";
        private readonly string login;
        private readonly string password;
        private readonly IWebHostEnvironment hostingEnvironment;

        public GitService(IWebHostEnvironment hostingEnvironment, IConfiguration cfg)
        {
            this.login = cfg["GitHub:Login"];
            this.password = cfg["GitHub:Password"];
            this.hostingEnvironment = hostingEnvironment;
        }

        public async Task<bool> IfExistsAsync(string repoPath)
        {
            var client = new HttpClient();
            var responseString = await client.GetStringAsync("https://github.com/" + repoPath);
            return responseString.Contains($"<title>GitHub - {repoPath}:");
        }

        /// <inheritdoc/>
        public async Task<bool> CloneAsync(string url)
        {
            var path = GetRepositoryPath(url.GetGitRepositoryName());
            try
            {
                await Task.Run(() => CloneDoWork(url, path));
                return true;
            }
            catch (Exception e)
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
                FetchOptions = new FetchOptions { CredentialsProvider = GetCredentials },
            };

            var signature = new Signature(
                new Identity(login, login), DateTimeOffset.Now);

            // Pull
            Commands.Pull(repo, signature, options);
        }

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
                        files.AddRange(repo.Diff.Compare<TreeChanges>(parent.Tree, commit.Tree)
                            .Select(change => change.Path));
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
        private UsernamePasswordCredentials GetCredentials(
            string url,
            string usernameFromUrl,
            SupportedCredentialTypes types)
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
            try
            {
                Repository.Clone(url, path, co);
            }
            catch (Exception e)
            {
                
            }
        }

        private string GetRepositoryPath(string fileName)
        {
            var uploads = Path.Combine(hostingEnvironment.ContentRootPath, FolderName);
            var repoPath = Path.Combine(uploads, fileName);
            return repoPath.Replace("/", "\\");
        }
    }
}