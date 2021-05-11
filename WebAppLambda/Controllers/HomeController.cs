using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppLambda.Controllers
{
    [Route("[controller]/{action=Index}")]
    public class HomeController : Controller
    {
        private readonly ISampleTableRepository _sampleTableRepository;

        public HomeController(ISampleTableRepository sampleTableRepository)
        {
            _sampleTableRepository = sampleTableRepository;
        }

        public async Task<IActionResult> Index()
        {
            var all = await _sampleTableRepository.GetAllAsync();
            return View(all);
        }

        public IActionResult Add()
        {
            return View();
        }

        public async Task<IActionResult> Delete(string pk, string sk)
        {
            await _sampleTableRepository.DeleteAsync(pk, sk);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Save([FromForm] SampleTableEntry sampleTableEntry)
        {
            var res = await _sampleTableRepository.SaveAsync(sampleTableEntry);
            return RedirectToAction(nameof(Index));
        }
    }
}
