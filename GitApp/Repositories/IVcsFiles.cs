using System.Collections.Generic;
using GitApp.Models.Db;

namespace GitApp.Repositories
{
    public interface IVcsFiles
    {
        /// <summary>
        /// Get Version Control System file models IEnumerable collection from Git repository.
        /// </summary>
        /// <param name="repository">repository.</param>
        /// <returns>Specific <see cref="IEnumerable{File}"/> if found.</returns>
        IEnumerable<File> GetFiles(Repository repository);
    }
}