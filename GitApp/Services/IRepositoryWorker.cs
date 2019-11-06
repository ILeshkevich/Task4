using System.Threading.Tasks;
using GitApp.Models.Db;

namespace GitApp.Services
{
    public interface IRepositoryWorker
    {
        /// <summary>
        /// Asynchronous insert Version Control System Repository with it's files into Db.
        /// (Repository should be cloned)
        /// </summary>
        /// <param name="repoUrl">Repository url.</param>
        /// <returns><see cref="Task"/></returns>
        Task CreateRepositoryAsync(string repoUrl);

        /// <summary>
        /// Asynchronous update Version Control System Repository and it's files in Db.
        /// </summary>
        /// <param name="repository">Repository to update.</param>
        /// <returns><see cref="Task{Repository}"/></returns>
        Task<Repository> UpdateRepositoryAsync(Repository repository);
    }
}