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
                    Fiels = r.Files.Select(f => new FileViewModel { Name = f.Name, Count = f.ChangesCount }).ToList(),
                }).ToList();
        }

        public List<VcsRepositoryViewModel> List(int count)
        {
            return db.Repositories.Include(r => r.Files).Select(r => new VcsRepositoryViewModel
            {
                Id = r.Id,
                Name = r.Name,
                LastUpdate = r.DateTime,
                Url = r.Url,
                Fiels = r.Files.Select(f => new FileViewModel { Name = f.Name, Count = f.ChangesCount }).ToList(),
            }).Take(count).ToList();
        }

        public List<VcsRepositoryViewModel> List(int first, int count)
        {
            return db.Repositories.Include(r => r.Files).Select(r => new VcsRepositoryViewModel
            {
                Id = r.Id,
                Name = r.Name,
                LastUpdate = r.DateTime,
                Url = r.Url,
                Fiels = r.Files.Select(f => new FileViewModel { Name = f.Name, Count = f.ChangesCount }).ToList(),
            }).Skip(first).Take(count).ToList();
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

        public VcsRepositoryViewModel FirstOrDefault(string url)
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
                    Fiels = repo.Files
                    .Where(f => f.RepositoryId == repo.Id)
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
