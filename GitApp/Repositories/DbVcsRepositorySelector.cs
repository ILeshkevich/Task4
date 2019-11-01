using GitApp.Models.Db;
using GitApp.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GitApp.Repositories
{
    public class DbVcsRepositorySelector
    {
        private readonly ApplicationContext db;

        public DbVcsRepositorySelector(ApplicationContext context)
        {
            db = context;
        }

        public List<VcsRepositoryViewModel> List()
        {
            return db.Repositories.Include(r => r.Files).Select(r => new VcsRepositoryViewModel
            {
                Id = r.Id,
                Name = r.Name,
                LastUpdate = r.DateTime,
                Url = r.Url,
                Fiels = r.Files.OrderByDescending(f => f.ChangesCount).Select(f => new FileViewModel { Name = f.Name, Count = f.ChangesCount }).ToList(),
            }).ToList();
        }

        public VcsRepositoryViewModel FirstOrDefault(int id)
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
                    Fiels = repo.Files
                    .Where(f => f.RepositoryId == id)
                    .OrderByDescending(f => f.ChangesCount)
                    .Select(f => new FileViewModel
                    {
                        Name = f.Name,
                        Count = f.ChangesCount,
                    }).ToList(),
                };
            }
            else
            {
                return null;
            }
        }

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
                    Fiels = repo.Files
                    .Where(f => f.RepositoryId == id)
                    .OrderByDescending(f => f.ChangesCount)
                    .Take(count)
                    .Select(f => new FileViewModel
                    {
                        Name = f.Name,
                        Count = f.ChangesCount,
                    }).ToList(),
                };
            }
            else
            {
                return null;
            }
        }
    }
}
