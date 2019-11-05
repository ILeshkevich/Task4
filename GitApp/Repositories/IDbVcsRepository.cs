﻿using System.Collections.Generic;
using System.Threading.Tasks;
using GitApp.Models.Db;
using GitApp.Models.ViewModels;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace GitApp.Repositories
{
    public interface IDbVcsRepository
    {
        /// <summary>
        /// Get Version System Control Repository view model.
        /// </summary>
        /// <param name="id">Repository Identifier in DB.</param>
        /// <returns>Specific <see cref="VcsRepositoryViewModel"/> if found.</returns>
        VcsRepositoryViewModel FirstOrDefault(int id);
        
        /// <summary>
        /// Get Version System Control Repository model.
        /// </summary>
        /// <param name="id">Repository Identifier in DB.</param>
        /// <returns>Specific <see cref="Repository"/> if found.</returns>
        Repository GetRepository(int id);
        
        /// <summary>
        /// Get Version System Control Repository model.
        /// </summary>
        /// <param name="repoUrl">Repository Url in DB.</param>
        /// <returns>Specific <see cref="Repository"/> if found.</returns>
        Repository GetRepository(string repoUrl);

        /// <summary>
        /// Asynchronous add Version Control System Repository model into Database. And SaveChanges.
        /// </summary>
        /// <param name="repository">Version Control System Repository model to insert.</param>
        /// <returns>Doesn't returns anything.</returns>
        Task<int> AddAndSaveChangesAsync(Repository repository);
        
        /// <summary>
        /// Get Version System Control Repository view model with with the ability to choose the amount of files.
        /// </summary>
        /// <param name="id">Repository Identifier in DB.</param>
        /// <param name="count">Amount of files to select.</param>
        /// <returns>Specific <see cref="VcsRepositoryViewModel"/> if found.</returns>
        VcsRepositoryViewModel FirstOrDefault(int id, int count);

        /// <summary>
        /// Get list of Version System Control Repository view models.
        /// </summary>
        /// <returns>Specific <see cref="List{VcsRepositoryViewModel}"/> if found.</returns>
        IReadOnlyList<VcsRepositoryViewModel> List();

        /// <summary>
        /// Get Version System Control Repository view model with with the ability to choose the amount of files.
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
  
        /// <summary>
        /// Update Version System Control Repository and it's files in database.
        /// </summary>
        /// <param name="repository">Repository to update.</param>
        /// <param name="files">New files collection.</param>
        /// <returns>Task.</returns>
        Task UpdateRepositoryAsync(Repository repository, IEnumerable<File> files);
    }
}