using System.Collections.Generic;
using GitApp.Models.ViewModels;

namespace GitApp.Repositories
{
    public interface IDbVcsRepositorySelector
    {
        /// <summary>
        /// Get Version System Control Repository model.
        /// </summary>
        /// <param name="id">Repository Identifier in DB.</param>
        /// <returns>Specific <see cref="VcsRepositoryViewModel"/> if found.</returns>
        VcsRepositoryViewModel FirstOrDefault(int id);

        /// <summary>
        /// Get Version System Control Repository model with with the ability to choose the amount of files.
        /// </summary>
        /// <param name="id">Repository Identifier in DB.</param>
        /// <param name="count">Amount of files to select.</param>
        /// <returns>Specific <see cref="VcsRepositoryViewModel"/> if found.</returns>
        VcsRepositoryViewModel FirstOrDefault(int id, int count);

        /// <summary>
        /// Get list of Version System Control Repository models.
        /// </summary>
        /// <returns>Specific <see cref="List{VcsRepositoryViewModel}"/> if found.</returns>
        IReadOnlyList<VcsRepositoryViewModel> List();

        /// <summary>
        /// Get Version System Control Repository model with with the ability to choose the amount of files.
        /// </summary>
        /// <param name="url">Repository Url in DB.</param>
        /// <param name="count">Amount of files to select.</param>
        /// <returns>Specific <see cref="VcsRepositoryViewModel"/> if found.</returns>
        VcsRepositoryViewModel FirstOrDefault(string url, int count);

        /// <summary>
        /// Get Version System Control Repository model.
        /// </summary>
        /// <param name="url">Repository Url in DB.</param>
        /// <returns>Specific <see cref="VcsRepositoryViewModel"/> if found.</returns>
        VcsRepositoryViewModel FirstOrDefault(string url);
    }
}