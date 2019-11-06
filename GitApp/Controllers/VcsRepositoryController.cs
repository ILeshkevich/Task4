using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GitApp.Hubs;
using GitApp.Repositories;
using GitTool;
using GitTool.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace GitApp.Controllers
{
    public class VcsRepositoryController : Controller
    {
        private const string FolderName = @"wwwroot\Repositories";

        private readonly ILogger<VcsRepositoryController> logger;
        private readonly IWebHostEnvironment hostingEnvironment;
        private readonly IGitService gitService;
        private readonly IDbVcsRepository repository;

        public VcsRepositoryController(
            ILogger<VcsRepositoryController> logger,
            IWebHostEnvironment environment,
            IGitService gitService,
            IHubContext<UploadStatusHub> hubContext,
            IDbVcsRepository repository)
        {
            this.logger = logger;
            hostingEnvironment = environment;
            this.gitService = gitService;
            this.repository = repository;
        }

        public IActionResult Index()
        {
            return View(repository.List());
        }

        public IActionResult Info(int id)
        {
            var repo = repository.FirstOrDefault(id);
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

        public async Task<IActionResult> Update(int id)
        {
            var repo = repository.GetRepository(id);
            if (repo == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var fileName = repo.Url.GetGitRepositoryName();
            var repoPath = GetRepositoryPath(fileName);

            var files = gitService.GetFiles(repoPath).Select(f => new Models.Db.File
            {
                Name = f.Key,
                ChangesCount = f.Value,
                Repository = repo,
            });

            await repository.UpdateRepositoryAsync(repo, files);

            return View(nameof(Info), repository.FirstOrDefault(id));
        }

        private string GetRepositoryPath(string fileName)
        {
            var uploads = Path.Combine(hostingEnvironment.WebRootPath, FolderName);
            var repoPath = Path.Combine(uploads, fileName);
            return repoPath.Replace("/", "\\");
        }
    }
}