using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GitApp.Models.Db;
using GitApp.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace GitApp.Repositories
{
    public class DbVcsRepository : IDbVcsRepository
    {
        private readonly ApplicationContext db;

        private const int AllFilesToShow = -1;

        public DbVcsRepository(ApplicationContext context)
        {
            db = context;
        }

        /// <inheritdoc/>
        public IReadOnlyList<VcsRepositoryViewModel> List()
        {
            var result = db.Repositories.Include(r => r.Files).Select(r => new VcsRepositoryViewModel
            {
                Id = r.Id,
                Name = r.Name,
                LastUpdate = r.DateTime,
                Url = r.Url,
                Files = r.Files.OrderByDescending(f => f.ChangesCount)
                    .Select(f => new FileViewModel { Name = f.Name, Count = f.ChangesCount }).ToList(),
            }).ToList();
            db.Dispose();
            return result;
        }

        public VcsRepositoryViewModel FirstOrDefault(string url, int count)
        {
            
            var repo = db.Repositories.Include(r => r.Files).FirstOrDefault(r => r.Url == url);
            if (repo != null)
            {
                return new VcsRepositoryViewModel
                {
                    Id = repo.Id,
                    Name = repo.Name,
                    Url = repo.Url,
                    LastUpdate = repo.DateTime,
                    Files = GetFiles(repo, count),
                };
            }
            else
            {
                return null;
            }
        }

        public VcsRepositoryViewModel FirstOrDefault(string url)
        {
            return FirstOrDefault(url, AllFilesToShow);
        }

        /// <inheritdoc/>
        public VcsRepositoryViewModel FirstOrDefault(int id)
        {
            return FirstOrDefault(id, AllFilesToShow);
        }

        /// <inheritdoc/>
        public Repository GetRepository(int id)
        {
            return db.Repositories.FirstOrDefault(r => r.Id == id);
        }

        /// <inheritdoc/>
        public Repository GetRepository(string repoUrl)
        {
            return db.Repositories.FirstOrDefault(r => r.Url == repoUrl);
        }
        
        /// <inheritdoc/>
        public Task<int> AddAndSaveChangesAsync(Repository repository)
        {
            db.Repositories.AddAsync(repository);
            return db.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public VcsRepositoryViewModel FirstOrDefault(int id, int count)
        {
            var repo = db.Repositories.Include(r => r.Files).FirstOrDefault(r => r.Id == id);
            if (repo != null)
            {
                return new VcsRepositoryViewModel
                {
                    Id = repo.Id,
                    Name = repo.Name,
                    Url = repo.Url,
                    LastUpdate = repo.DateTime,
                    Files = GetFiles(repo, count),
                };
            }
            else
            {
                return null;
            }
        }

        private IReadOnlyList<FileViewModel> GetFiles(Repository repo, int count)
        {
            var files = repo.Files
                .OrderByDescending(f => f.ChangesCount)
                .Select(f => new FileViewModel
                {
                    Name = f.Name,
                    Count = f.ChangesCount,
                });

            if (count != AllFilesToShow)
            {
                files = files.Take(count);
            }

            return files.ToList();
        }

        /// <inheritdoc/>
        public async Task UpdateRepositoryAsync(Repository repository, IEnumerable<File> files)
        {
            repository.DateTime = DateTime.Now;
            repository.Files.Clear();
            //db.RemoveRange(db.Files.Where(f => f.RepositoryId == repository.Id).ToList());
            //db.Files.AddRange(files);
            repository.Files.AddRange(files);
            db.Update(repository);
            await db.SaveChangesAsync();
        }
    }
}