using System;
using System.Linq;
using System.Threading.Tasks;
using GitApp.Models.Db;
using GitApp.Repositories;
using GitTool;

namespace GitApp.Services
{
    public class RepositoryWorker : IRepositoryWorker
    {
        private readonly IGitService gitService;
        private readonly IVcsFilesRepository filesRepository;
        private readonly IDbVcsRepositoryRepository vcsRepositoryRepository;
        
        public RepositoryWorker(
            IGitService gitService, 
            IVcsFilesRepository filesRepository,
            IDbVcsRepositoryRepository vcsRepositoryRepository)
        {
            this.gitService = gitService;
            this.filesRepository = filesRepository;
            this.vcsRepositoryRepository = vcsRepositoryRepository;
        }
        
        /// <inheritdoc/>
        public async Task CreateRepositoryAsync(string repoUrl)
        {
            var vcsRepositoryName = repoUrl.GetGitRepositoryName();
            var repo = new Repository { Name = vcsRepositoryName, Url = repoUrl, DateTime = DateTime.Now };
            var filesDictionary = gitService.GetFiles(repo.Name);
            repo.Files = filesRepository.ToFileCollection(filesDictionary).ToList();
            await vcsRepositoryRepository.AddAndSaveChangesAsync(repo);
        }
        
        /// <inheritdoc/>
        public async Task<Repository> UpdateRepositoryAsync(Repository repository)
        {
            var repositoryName = repository.Url.GetGitRepositoryName();
            await vcsRepositoryRepository.DeleteFilesAsync(repository);
            repository.Files = filesRepository.ToFileCollection(gitService.GetFiles(repositoryName)).ToList();
            await vcsRepositoryRepository.UpdateRepositoryAsync(repository);
            
            return repository;
        }
    }
}