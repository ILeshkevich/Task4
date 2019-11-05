using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GitApp.Models.Db;
using GitApp.Repositories;
using GitApp.Services;
using GitTool;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;

namespace GitApp.Hubs
{
    public class UploadStatusHub : Hub
    {
        private const string FolderName = "Repositories";
        private readonly GitService gitService;
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly IDbVcsRepository repository;
        private readonly IVcsFiles files;
        
        public UploadStatusHub(GitService gitService, IHostingEnvironment environment, IDbVcsRepository repository, IVcsFiles files)
        {
            this.gitService = gitService;
            hostingEnvironment = environment;
            this.repository = repository;
            this.files = files;
        }

        public async Task Upload(string repoUrl)
        {
            var repo = repository.GetRepository(repoUrl);
            if (repo == null)
            {
                var fileName = repoUrl.ToGitFileName();
                var uploads = Path.Combine(hostingEnvironment.WebRootPath, FolderName);
                var repoPath = Path.Combine(uploads, fileName);
                repoPath = repoPath.Replace("/", "\\");

                await Clients.Caller.SendAsync("Upload", repoUrl);
                var result = await gitService.CloneAsync(repoUrl);
                if (result)
                {
                    repo = new Repository { Name = fileName, Url = repoUrl, DateTime = DateTime.Now };
                    repo.Files = files.GetFiles(repo).ToList();
                    await repository.AddAndSaveChangesAsync(repo);
                    await Clients.Caller.SendAsync("Uploaded");
                }
                else
                {
                    await Clients.Caller.SendAsync("Error");
                }
            }
        }
    }
}