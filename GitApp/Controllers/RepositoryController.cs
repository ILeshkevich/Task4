using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GitApp.Models.Db;
using GitApp.Models.ViewModels;
using GitTool;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GitApp.Controllers
{
    // Try to avoid fat controllers
    // Move db-related logic into repositories
    // Move business-logic into services
    // Todo: Always remove unused code.
    // Todo: sync code style, investigate why .editorconfig doesn't work
    // Todo: get rid of logger or add usage.
    public class RepositoryController : Controller
    {
        private readonly ILogger<RepositoryController> logger;
        private readonly ApplicationContext db;
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly Git git;

        public RepositoryController(ILogger<RepositoryController> logger, ApplicationContext context, IHostingEnvironment environment, Git git)
        {
            this.logger = logger;
            db = context;
            hostingEnvironment = environment;
            this.git = git;
        }

        public IActionResult Index()
        {
            // SOLID
            // Todo: rename Repository... into VcsRepository
            // todo: try to use Automapper library.
            // I suggest to move db.files.select... into repository and intoduce new variable
            // todo: investigate if Include is required here or not.
            List<RepositoryViewModel> model =
                db.Repositories.Include(r => r.Files).Select(r => new RepositoryViewModel
                {
                    Id = r.Id,
                    Name = r.Name,
                    LastUpdate = r.DateTime,
                    Url = r.Url,
                    Fiels = r.Files.Select(f => new FileViewModel { Name = f.Name, Count = f.ChangesCount }).ToList(),
                }).ToList();

            return View(model);
        }

        public IActionResult Info(int id)
        {
            // Code duplication - move into separate method.
            var repo = db.Repositories.FirstOrDefault(r => r.Id == id);
            if (repo != null)
            {
                return View(new RepositoryViewModel
                { Id = repo.Id, Name = repo.Name, Url = repo.Url, LastUpdate = repo.DateTime, Fiels =
                db.Files.Select(f => new FileViewModel { Name = f.Name, Count = f.ChangesCount }).ToList() });
            }

            return RedirectToAction("Index");

            // C# Extension classes/methods
            var url = Url.Action(nameof(Index), nameof(RepositoryController).Replace("Controller", string.Empty));
        }

        [HttpPost]
        // Todo: update all hardcoded action names with nameof()
        public async Task<IActionResult> Upload(string repositoryUrl)
        {
            if (repositoryUrl != null)
            {
                var temp = db.Repositories.FirstOrDefault(r => r.Url == repositoryUrl);
                if (temp != null)
                {
                    return RedirectToAction(nameof(Update), new { id = temp.Id });
                }

                // get rid of filename. It is ok to make FileName -> full repository url
                // Enhancement: remove base host name, remove git extension. So file name will be like /Folder/Folder2/File.cs
                var fileName = repositoryUrl.Remove(0, repositoryUrl.LastIndexOf(@"/") + 1).Replace(".git", string.Empty);
                // Todo: make hardcoded "Repositories" to be a constant
                var uploads = Path.Combine(hostingEnvironment.WebRootPath, "Repositories");
                var repoPath = Path.Combine(uploads, fileName);

                DirectoryInfo info = new DirectoryInfo(uploads);
                if (info.GetDirectories().Any(d => d.Name == fileName))
                {
                    Directory.Delete(repoPath, true);
                }

                var result = await git.Clone(repositoryUrl, repoPath);
                if (result)
                {
                    // todo: try to initialize Files in {}
                    Repository repo = new Repository { Name = fileName, Url = repositoryUrl, DateTime = DateTime.Now };
                    // Check if `, Repository = repo` is required here.
                    repo.Files = git.GetFiles(repoPath).Select(f => new Models.Db.File { Name = f.Key, ChangesCount = f.Value, Repository = repo }).ToList();
                    await db.Repositories.AddAsync(repo);
                   // await db.Files.AddRangeAsync(files);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Info", fileName);
                }
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Update(int id)
        {
            var repo = db.Repositories.FirstOrDefault(r => r.Id == id);

            if (repo != null)
            {
                var fileName = repo.Url.Remove(0, repo.Url.LastIndexOf(@"/") + 1).Replace(".git", string.Empty);
                var uploads = Path.Combine(hostingEnvironment.WebRootPath, "Repositories");
                var repoPath = Path.Combine(uploads, fileName);
                var result = await git.Clone(repo.Url, repoPath);
                if (result)
                {
                    var files = git.GetFiles(repoPath).Select(f => new Models.Db.File { Name = f.Key, ChangesCount = f.Value, Repository = repo });

                    repo.DateTime = DateTime.Now;
                    db.Update(repo);
                    db.RemoveRange(db.Files.Where(f => f.RepositoryId == id).ToList());
                    db.Files.AddRange(files);
                    await db.SaveChangesAsync();
                    RepositoryViewModel model = new RepositoryViewModel { Id = repo.Id, Name = repo.Name, Url = repo.Url, LastUpdate = repo.DateTime, Fiels = files.Select(f => new FileViewModel { Name = f.Name, Count = f.ChangesCount }).ToList() };
                    return View("Info", model);
                }
            }

            return RedirectToAction("Index");
        }
    }
}