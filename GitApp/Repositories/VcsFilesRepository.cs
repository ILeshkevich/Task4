using System.Collections.Generic;
using System.Linq;
using GitApp.Models.Db;

namespace GitApp.Repositories
{
    public class VcsFilesRepository : IVcsFilesRepository
    {
        /// <inheritdoc/>
        public IEnumerable<File> ToFileCollection(Dictionary<string, int> files)
        {
            return files.Select(f => new File { Name = f.Key, ChangesCount = f.Value });
        }
    }
}