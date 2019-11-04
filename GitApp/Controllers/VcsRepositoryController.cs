using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GitApp.Hubs;
using GitApp.Models.Db;
using GitApp.Models.ViewModels;
using GitApp.Repositories;
using GitApp.Services;
using GitTool;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


// todo: SignalR support async task tracking...
namespace GitApp.Controllers
{
    public class VcsRepositoryController : Controller
    {
        private readonly ILogger<VcsRepositoryController> logger;
        private readonly ApplicationContext db;
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly GitService _gitService;
        private readonly DbVcsRepositorySelector dbSelector;
        private const string FolderName = "Repositories";
        private readonly DbVcsRepositoryWriter dbWriter;
        private readonly IHubContext<UploadStatusHub> hubContext;

        public VcsRepositoryController(ILogger<VcsRepositoryController> logger, ApplicationContext context, IHostingEnvironment environment, GitService gitService, IHubContext<UploadStatusHub> hubContext)
        {
            this.logger = logger;
            db = context;
            hostingEnvironment = environment;
            this._gitService = gitService;
            this.dbSelector = new DbVcsRepositorySelector(context);
            this.hubContext = hubContext;
        }

        public IActionResult Index()
        {
            return View(dbSelector.List());
        }

        public IActionResult Info(int id)
        {
            var repoService = new DbVcsRepositorySelector(db);
            var repo = repoService.FirstOrDefault(id);
            if (repo != null)
            {
                return View(repo);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(UploadVcsRepositoryViewModel model)
        {
            if (model.RepositoryUrl == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var temp = dbSelector.FirstOrDefault(model.RepositoryUrl);
            if (temp != null)
            {
                return RedirectToAction(nameof(Update), new { id = temp.Id });
            }

            var fileName = model.RepositoryUrl.ToGitFileName();

            var repoPath = GetRepositoryPath(fileName);

            var result = await _gitService.CloneAsync(model.RepositoryUrl, repoPath);

            if (!result)
            {
                return RedirectToAction(nameof(Index));
            }

            var repo = new Repository { Name = fileName, Url = model.RepositoryUrl, DateTime = DateTime.Now };

            repo.Files = _gitService.GetFiles(repoPath).Select(f =>
                new Models.Db.File { Name = f.Key, ChangesCount = f.Value, Repository = repo }).ToList();
            await db.Repositories.AddAsync(repo);
            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int id)
        {
            var repo = await db.Repositories.FirstOrDefaultAsync(r => r.Id == id);
            if (repo == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var fileName = repo.Url.ToGitFileName();
            var repoPath = GetRepositoryPath(fileName);

            var files = _gitService.GetFiles(repoPath).Select(f => new Models.Db.File
            {
                Name = f.Key,
                ChangesCount = f.Value,
                Repository = repo,
            });

            await new DbVcsRepositoryWriter(db).UpdateRepositoryAsync(files, repo);

            return View(nameof(Info), new DbVcsRepositorySelector(db).FirstOrDefault(id));
        }

        private string GetRepositoryPath(string fileName)
        {
            var uploads = Path.Combine(hostingEnvironment.WebRootPath, FolderName);
            var repoPath = Path.Combine(uploads, fileName);
            return repoPath.Replace("/", "\\");
        }
    }
}