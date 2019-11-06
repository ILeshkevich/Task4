using System.Collections.Generic;
using GitApp.Models.Db;

namespace GitApp.Repositories
{
    public interface IVcsFilesRepository
    {
        /// <summary>
        /// Convert dictionary (key - file name, value - changes count)
        /// Version Control System file models IEnumerable collection.
        /// </summary>
        /// <param name="files">Files dictionary.</param>
        /// <returns>Specific <see cref="IEnumerable{File}"/> if found.</returns>
        IEnumerable<File> ToFileCollection(Dictionary<string, int> files);
    }
}