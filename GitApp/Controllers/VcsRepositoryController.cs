using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GitApp.Models.Db;
using GitApp.Models.ViewModels;
using GitApp.Repositories;
using GitApp.Services;
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
    public class VcsRepositoryController : Controller
    {
        private readonly ILogger<VcsRepositoryController> logger;
        private readonly ApplicationContext db;
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly Git git;
        private static string folderName = "Repositories";

        public VcsRepositoryController(ILogger<VcsRepositoryController> logger, ApplicationContext context, IHostingEnvironment environment, Git git)
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
            return View(new DbVcsRepositorySelector(db).List());
        }

        public IActionResult Info(int id)
        {
            var repo = new DbVcsRepositorySelector(db).FirstOrDefault(id);
            if (repo != null)
            {
                return View(repo);
            }

            return RedirectToAction(nameof(Index));
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
                var fileName = repositoryUrl.GitFileName();

                // Todo: make hardcoded "Repositories" to be a constant
                var uploads = Path.Combine(hostingEnvironment.WebRootPath, folderName);
                var repoPath = Path.Combine(uploads, fileName);

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

                    return RedirectToAction(nameof(Info), fileName);
                }
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int id)
        {
            var repo = await db.Repositories.FirstOrDefaultAsync(r => r.Id == id);
            if (repo != null)
            {
                var fileName = repo.Url.GitFileName();
                var uploads = Path.Combine(hostingEnvironment.WebRootPath, folderName);
                var repoPath = Path.Combine(uploads, fileName);

                var files = git.GetFiles(repoPath).Select(f => new Models.Db.File
                {
                    Name = f.Key,
                    ChangesCount = f.Value,
                    Repository = repo,
                });

                await new DbVcsRepositoryWriter(db).UpdateRepositoryAsync(files, repo);
                return View(nameof(Info), new DbVcsRepositorySelector(db).FirstOrDefault(id));
            }

            return RedirectToAction(nameof(Index));
        }
    }
}