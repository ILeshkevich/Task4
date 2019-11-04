using System.Collections.Generic;
using System.Linq;
using GitApp.Models.Db;
using GitApp.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace GitApp.Repositories
{
    public class DbVcsRepositorySelector : IDbVcsRepositorySelector
    {
        private readonly ApplicationContext db;

        private const int AllFilesToShow = -1;

        public DbVcsRepositorySelector(ApplicationContext context)
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
                Files = r.Files.OrderByDescending(f => f.ChangesCount).Select(f => new FileViewModel { Name = f.Name, Count = f.ChangesCount }).ToList(),
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
    }
}
