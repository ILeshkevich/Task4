using System.Threading.Tasks;
using GitApp.Repositories;
using GitApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GitApp.Controllers
{
    public class VcsRepositoryController : Controller
    {
        // ReSharper disable once NotAccessedField.Local
        private readonly ILogger<VcsRepositoryController> logger;
        private readonly IDbVcsRepositoryRepository repositoryRepository;
        private readonly IRepositoryWorker repositoryWorker;

        public VcsRepositoryController(
            ILogger<VcsRepositoryController> logger,
            IDbVcsRepositoryRepository repositoryRepository, 
            IRepositoryWorker repositoryWorker)
        {
            this.logger = logger;
            this.repositoryRepository = repositoryRepository;
            this.repositoryWorker = repositoryWorker;
        }

        public IActionResult Index()
        {
            return View(nameof(Index), repositoryRepository.List());
        }

        public IActionResult Info(int id){
            var repo = repositoryRepository.FirstOrDefault(id);
            if (repo != null)
            {
                return View(nameof(Info), repo);
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int id)
        {
            var repo = repositoryRepository.GetRepository(id);
            if (repo == null)
            {
                return RedirectToAction(nameof(Index));
            }

            await repositoryWorker.UpdateRepositoryAsync(repo);
            
            return View(nameof(Info), repositoryRepository.FirstOrDefault(id));
        }
    }
}