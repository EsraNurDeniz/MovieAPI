using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MovieApi.Models;
using System.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Cors;


namespace MovieApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors("CorsPolicy")]

    public class MovieItemsController : Controller 
    {
        private readonly MovieContext _context;
        private readonly ILogger<MovieItemsController> _logger;

        public MovieItemsController(MovieContext context, ILogger<MovieItemsController> logger)
        {
            _context = context;
            this._logger = logger;
        }
        
        [HttpGet]
        public async Task<List<MovieItem>> GetMovieItems(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Hi from logger !");
            List<MovieItem> list = await _context.GetAllItems(cancellationToken);
            return list;
        }  
        
        [HttpGet("{id}")]
        public async Task<ActionResult<MovieItem>> GetAMovieItem(CancellationToken cancellationToken, int id)
        { 
            MovieItem movieItem  = await _context.GetAnItem(cancellationToken,id);
            if (movieItem == null)
            {
                return NotFound();
            }
            return movieItem;
        }

        [HttpPost]
        public MovieItem PostMovieItem(MovieItem item)
        {
            return _context.PostItem(item);
        }
        
        [HttpDelete("{id}")]
        public async Task<ActionResult<MovieItem>> DeleteMovieItem(CancellationToken cancellationToken,int id)
        {
            MovieItem movieItem  = await _context.GetAnItem(cancellationToken, id);
            if( movieItem == null)
            {
                return NotFound();
            }
            else
            {
                _context.DeleteItem(id);
                return movieItem;
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<MovieItem>> PutMovieItem(CancellationToken cancellationToken,int id, MovieItem movieItem)
        {
            if (id != movieItem.id)
            {
                return NotFound();
            }
            MovieItem item  = await _context.GetAnItem(cancellationToken, id);
            if(item == null)
            {
                return NotFound();
            }            
            return await _context.PutItem(cancellationToken,movieItem);
        }      
    }
}

