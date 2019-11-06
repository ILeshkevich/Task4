using System.Collections.Generic;
using System.Threading.Tasks;
using LibGit2Sharp;

namespace GitTool
{
    public interface IGitService
    {
        /// <summary>
        /// Asynchronous cloning git Repository.
        /// </summary>
        /// <param name="url">Url to Git repository. <example>https://github.com/ILeshkevich/Task4.git</example></param>
        /// <returns>True if repository created, false if exception.</returns>
        Task<bool> CloneAsync(string url);

        /// <summary>
        /// Pulling Git repository.
        /// </summary>
        /// <param name="repo">Repository to poll.</param>
        void Pull(Repository repo);

        /// <summary>
        /// Get files files with changes count from local git repository.
        /// </summary>
        /// <param name="repositoryName">Name of local git repository.</param>
        /// <returns>Specific <see cref="Dictionary{String,Int32}"/> if found.</returns>
        Dictionary<string, int> GetFiles(string repositoryName);

        Task<bool> IfExistsAsync(string repoPath);
    }
}