using System.Threading.Tasks;
using GitApp.Repositories;
using GitApp.Services;
using GitTool;
using Microsoft.AspNetCore.SignalR;

namespace GitApp.Hubs
{
    public class UploadStatusHub : Hub
    {
        private readonly IGitService gitService;
        private readonly IDbVcsRepositoryRepository vcsRepositoryRepository;
        private readonly IRepositoryWorker repositoryWorker;

        public UploadStatusHub(
            IGitService gitService,
            IDbVcsRepositoryRepository vcsRepositoryRepository,
            IRepositoryWorker repositoryWorker)
        {
            this.gitService = gitService;
            this.vcsRepositoryRepository = vcsRepositoryRepository;
            this.repositoryWorker = repositoryWorker;
        }

        public async Task Upload(string repoUrl)
        {
            var repo = vcsRepositoryRepository.GetRepository(repoUrl);
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

            await Clients.Caller.SendAsync("ShowUploadInProgress", repoUrl);
            
            var result = await gitService.CloneAsync(repoUrl);
            if (result)
            {
                await repositoryWorker.CreateRepositoryAsync(repoUrl);
                await Clients.Caller.SendAsync("HideUploadInProgress");
            }
            else
            {
                await Clients.Caller.SendAsync("Error", "Unexpected error!");
            }
        }
    }
}
