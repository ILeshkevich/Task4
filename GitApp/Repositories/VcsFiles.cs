using System.Collections.Generic;
using System.IO;
using System.Linq;
using GitApp.Models.Db;
using GitTool;
using Microsoft.AspNetCore.Hosting;
using File = GitApp.Models.Db.File;

namespace GitApp.Repositories
{
    public class VcsFiles : IVcsFiles
    {
        private const string FolderName = "Repositories";
        private readonly GitService gitService;
        private readonly IHostingEnvironment hostingEnvironment;

        public VcsFiles(GitService gitService, IHostingEnvironment hostingEnvironment)
        {
            this.gitService = gitService;
            this.hostingEnvironment = hostingEnvironment;
        }
        
        /// <inheritdoc/>
        public IEnumerable<File> GetFiles(Repository repository)
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