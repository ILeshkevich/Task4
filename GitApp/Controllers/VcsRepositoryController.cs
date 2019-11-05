using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GitApp.Hubs;
using GitApp.Models.Db;
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
        private const string FolderName = "Repositories";
        
        private readonly ILogger<VcsRepositoryController> logger;
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly GitService _gitService;
        private readonly IDbVcsRepository repository;
        private readonly IHubContext<UploadStatusHub> hubContext;

        public VcsRepositoryController(ILogger<VcsRepositoryController> logger,
            IHostingEnvironment environment, GitService gitService, IHubContext<UploadStatusHub> hubContext,
            IDbVcsRepository repository)
        {
            this.logger = logger;
            hostingEnvironment = environment;
            this._gitService = gitService;
            this.repository = repository;
            this.hubContext = hubContext;
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

            var fileName = repo.Url.ToGitFileName();
            var repoPath = GetRepositoryPath(fileName);

            var files = _gitService.GetFiles(repoPath).Select(f => new Models.Db.File
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