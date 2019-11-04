using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GitApp.Models.Db;

namespace GitApp.Repositories
{
    public class DbVcsRepositoryWriter
    {
        private readonly ApplicationContext db;

        public DbVcsRepositoryWriter(ApplicationContext context)
        {
            db = context;
        }

        public async Task UpdateRepositoryAsync(IEnumerable<File> files, Repository repository)
        {
            repository.DateTime = DateTime.Now;
            db.Update(repository);
            db.RemoveRange(db.Files.Where(f => f.RepositoryId == repository.Id).ToList());
            db.Files.AddRange(files);
            await db.SaveChangesAsync();
        }
    }
}
