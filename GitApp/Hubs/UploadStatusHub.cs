using System;
using System.Linq;
using System.Threading.Tasks;
using GitApp.Models.Db;
using GitApp.Repositories;

using GitTool;
using GitTool.Services;
using Microsoft.AspNetCore.SignalR;

namespace GitApp.Hubs
{
    public class UploadStatusHub : Hub
    {
        private readonly IGitService gitService;
        private readonly IDbVcsRepository vcsRepository;
        private readonly IVcsFilesRepository filesRepository;

        public UploadStatusHub(
            IGitService gitService,
            IDbVcsRepository vcsRepository,
            IVcsFilesRepository filesRepository)
        {
            this.gitService = gitService;
            this.vcsRepository = vcsRepository;
            this.filesRepository = filesRepository;
        }

        public async Task Upload(string repoUrl)
        {
            var repo = vcsRepository.GetRepository(repoUrl);
            if (repo != null)
            {
                await Clients.Caller.SendAsync("Error", "Current repository already exist.");
                
                return;
            }

            if (!await gitService.IfExistsAsync(repoUrl.GetGitRepositoryName()))
            {
                await Clients.Caller.SendAsync("Error", "Not found Github repository with this url.");
                
                return;
            }

            var vcsRepositoryName = repoUrl.GetGitRepositoryName();

            // Rename Upload to ShowUploadInProgress
            await Clients.Caller.SendAsync("ShowUploadInProgress", repoUrl);

            var result = await gitService.CloneAsync(repoUrl);
            if (result)
            {
                repo = new Repository { Name = vcsRepositoryName, Url = repoUrl, DateTime = DateTime.Now };

                // Refactor -> split file-system and db-related logic into separate classes
                repo.Files = filesRepository.GetFiles(repo).ToList();

                await vcsRepository.AddAndSaveChangesAsync(repo);

                // Rename Upload to HideUploadInProgress
                await Clients.Caller.SendAsync("HideUploadInProgress");
            }
            else
            {
                await Clients.Caller.SendAsync("Error", "Unexpected error!");
            }
        }
    }
}
