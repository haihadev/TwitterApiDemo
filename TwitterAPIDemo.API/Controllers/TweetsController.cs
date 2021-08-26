using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwitterAPIDemo.Core.DataAccess;
using TwitterAPIDemo.Core.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TwitterAPIDemo.API.Controllers
{
    [Route("api/Tweets")]
    [ApiController]
    public class TweetsController : ControllerBase
    {
        public TweetsController(IRepository repository, ILogger<TweetsController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        private readonly IRepository _repository;
        private readonly ILogger<TweetsController> _logger;

        // GET: api/Tweets
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tweet>>> Get()
        {
            return Ok(await _repository.Find<Tweet>()
                .Include(t => t.Urls)
                .Include(t => t.Hashtags)
                .ToListAsync());
        }

        // GET api/Tweets/5
        [HttpGet("{id}")]
        public async Task<ActionResult<string>> Get(long id)
        {
            var tweet = await _repository.Get<Tweet>(id);

            if (tweet == null)
            {
                return NoContent();
            }

            return Ok(tweet);
        }

        // POST api/Tweets
        [HttpPost]
        public async Task<ActionResult<Tweet>> Post([FromBody] Tweet tweet)
        {
            await _repository.Add(tweet);
            await _repository.SaveChanges();

            return CreatedAtAction(
                nameof(Get),
                new { id = tweet.Id },
                tweet);
        }

        // PUT api/Tweets/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(long id, [FromBody] Tweet tweet)
        {
            if (id != tweet.Id)
            {
                return BadRequest();
            }

            var tweetToUpdate = await _repository.Get<Tweet>(id);
            if (tweetToUpdate == null)
            {
                return NotFound();
            }

            try
            {
                _repository.Update(tweet);
                await _repository.SaveChanges();
                return Ok();
            }
            catch (DbUpdateConcurrencyException) when (!TweetExists(id))
            {
                return NotFound();
            }
        }

        // DELETE api/Tweets/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var tweet = await _repository.Get<Tweet>(id);
            if (tweet == null)
            {
                return NotFound();
            }

            _repository.Remove(tweet);
            await _repository.SaveChanges();

            return NoContent();
        }

        private bool TweetExists(long id) => _repository.Find<Tweet>().Any(t => t.Id == id);
    }
}
