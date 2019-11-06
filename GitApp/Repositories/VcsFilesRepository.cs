using System.Collections.Generic;
using System.IO;
using System.Linq;
using GitApp.Models.Db;
using GitTool;
using Microsoft.AspNetCore.Hosting;


namespace GitApp.Repositories
{
    public class VcsFilesRepository : IVcsFilesRepository
    {
        private const string FolderName = @"wwwroot\Repositories";
        private readonly IGitService gitService;
        private readonly IWebHostEnvironment hostingEnvironment;

        public VcsFilesRepository(IGitService gitService, IWebHostEnvironment hostingEnvironment)
        {
            this.gitService = gitService;
            this.hostingEnvironment = hostingEnvironment;
        }

        /// <inheritdoc/>
        public IEnumerable<Models.Db.File> GetFiles(Repository repository)
        {
            var repoPath = GetRepositoryPath(repository.Name);
            return gitService.GetFiles(repoPath).Select(f =>
                new Models.Db.File { Name = f.Key, ChangesCount = f.Value, Repository = repository });
        }

        private string GetRepositoryPath(string fileName)
        {
            var uploads = Path.Combine(hostingEnvironment.WebRootPath, FolderName);
            var repoPath = Path.Combine(uploads, fileName);
            return repoPath.Replace("/", "\\");
        }
    }
}