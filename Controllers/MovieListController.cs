using Microsoft.AspNetCore.Mvc;
using MovieApi.Models;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.AspNetCore.SignalR;

namespace MovieApi.Controllers
{
    [Route("[controller]/[action]")]
    public class MovieListController : Controller
    {
        private readonly MovieContext _context;
        public MovieListController(MovieContext context, IHubContext<Hubs.MovieHub> hubContext)
        {
            _context = context;
        }
        public async Task<IActionResult> List(CancellationToken cancellationToken)
        {
            ViewData["movies"] = await _context.GetAllItems(cancellationToken);
            return View();
        }

        public IActionResult Delete(int id)
        {
            _context.DeleteItem(id);
            return RedirectToAction("List");
        }

        [HttpGet]
        public async Task<IActionResult> CreateAndUpdate(CancellationToken cancellationToken ,int id)
        {
            if(id > 0)
            {
                ViewBag.header = "Edit The Movie";
                MovieItem item = await _context.GetAnItem(cancellationToken,id);
                return View(item);            
            }
            MovieItem nullItem = new MovieItem();
            nullItem.posterSize = 0;
            ViewBag.header = "Create A Movie";
            return View(nullItem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task <IActionResult> CreateAndUpdate(CancellationToken cancellationToken ,MovieItem item)
        {
            
            if(item.posterFile == null)
            {
                item.poster = null;
            }
            else
            {
                var ms = new MemoryStream();
                item.posterFile.CopyTo(ms);
                var fileBytes = ms.ToArray();
                item.poster = fileBytes;
            }

            if(item.id > 0)
            {
               await _context.PutItem(cancellationToken,item);
            }
            else
            {
                _context.PostItem(item); 
            }
            return RedirectToAction("List");
        }
    }
}
