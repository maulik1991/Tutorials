using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Tutorials.Business.Interface;
using Tutorials.Models;
using Tutorials.Models.ViewModels;
using static Tutorials.Startup;

namespace Tutorials.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILoggerManager _logger;
        //private readonly ILogger _loggerInbuild;
        private readonly IArticleService _articleService;
        private readonly ISectionDetailService _sectionDetailService;
        private readonly TestAbstractClass _testAbstractClass;

        public IOperationTransient TransientOperation { get; }
        public IOperationScoped ScopedOperation { get; }
        public IOperationSingleton SingletonOperation { get; }
        public IOperationTransient TransientOperation2 { get; }
        public IOperationScoped ScopedOperation2 { get; }
        public IOperationSingleton SingletonOperation2 { get; }
        //public IOperationSingletonInstance SingletonInstanceOperation { get; }

        public HomeController(TestAbstractClass testAbstractClass,ILoggerManager logger, IArticleService articleService, ISectionDetailService sectionDetailService
            , IOperationTransient transientOperation,
            IOperationScoped scopedOperation,
            IOperationSingleton singletonOperation, IOperationTransient transientOperation2,
            IOperationScoped scopedOperation2,
            IOperationSingleton singletonOperation2)
        {
            _testAbstractClass = testAbstractClass;
            _logger = logger;
            _articleService = articleService;
            _sectionDetailService = sectionDetailService;
            _testAbstractClass.MethodA();
            _testAbstractClass.MethodB();

            TransientOperation = transientOperation;
            ScopedOperation = scopedOperation;
            SingletonOperation = singletonOperation;
            TransientOperation2 = transientOperation2;
            ScopedOperation2 = scopedOperation2;
            SingletonOperation2 = singletonOperation2;
        }

        public IActionResult Index()
        {
            var a = TransientOperation.GetOperationID();
            var a1 = TransientOperation2.GetOperationID();

            var a2 = ScopedOperation.GetOperationID();
            var a3 = ScopedOperation2.GetOperationID();


            var a4 = SingletonOperation.GetOperationID();
            var a5 = SingletonOperation2.GetOperationID();


            _logger.LogInformation("Test");
            _logger.LogError("Test");
            return View();
        }

        [Route("Article")]
        public async Task<IActionResult> Article(int id, int id2, string desc)
        {
            ArticleViewModel model = new ArticleViewModel();
            var article = await _sectionDetailService.GetSectionById(id2);
            if (article != null)
            {
                model.SectionDetailDto = article;
            }
            var section = await _sectionDetailService.GetAllSectionsForArticle(id);
            if (section != null && section.Count() > 0)
            {
                model.ArticleDetailsDtos = section.ToList();
            }
            return View(model);
        }

        public ViewResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
