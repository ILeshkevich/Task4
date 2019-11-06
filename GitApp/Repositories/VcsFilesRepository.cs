using System.Collections.Generic;
using System.Linq;
using GitApp.Models.Db;
using GitTool;

namespace GitApp.Repositories
{
    public class VcsFilesRepository : IVcsFilesRepository
    {
        private readonly IGitService gitService;

        public VcsFilesRepository(IGitService gitService)
        {
            this.gitService = gitService;
        }

        /// <inheritdoc/>
        public IEnumerable<File> ToFileCollection(Dictionary<string, int> files)
        {
            return files.Select(f => new File { Name = f.Key, ChangesCount = f.Value });
        }
    }
}