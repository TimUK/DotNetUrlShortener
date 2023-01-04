using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using URLShortener.Data;
using URLShortener.DataModels;
using URLShortener.Interfaces;

namespace URLShortener.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class LinksController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly LinkAPIContext _linkAPIContext;
        private readonly IUrlNameGeneratorService _nameGenerator;

        public LinksController(UserManager<IdentityUser> userManager, LinkAPIContext linkAPIContext, IUrlNameGeneratorService nameGenerator)
        {
            _userManager = userManager;
            _linkAPIContext = linkAPIContext;
            _nameGenerator = nameGenerator;
        }

        /// <summary>
        /// Gets all links owned by this user
        /// </summary>
        [HttpGet]
        public ActionResult<List<Link>> Index()
        {
            if (ModelState.IsValid)
            {
                List<Claim> identifierClaims = User.FindAll(ClaimTypes.NameIdentifier).ToList();
                var userId = identifierClaims.Last().Value;
                List<Link> links = _linkAPIContext.Links.Where(x => x.Owner ==userId).ToList();
                return Ok(links);
            }
            else
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Gets object info for specific link
        /// </summary>
        [HttpGet("{LinkId}")]
        public async Task<ActionResult<Link>> GetLink(Guid LinkId)
        {
            List<Claim> identifierClaims = User.FindAll(ClaimTypes.NameIdentifier).ToList();
            var userId = identifierClaims.Last().Value;
            var link = await _linkAPIContext.Links.FirstOrDefaultAsync(x => x.Id == LinkId);

            if (link == null) return BadRequest();

            return Ok(link);
        }

        /// <summary>
        /// Creates a new link
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<List<Link>>> PostLink(Link newLink)
        {
            if(ModelState.IsValid)
            {
                List<Claim> identifierClaims = User.FindAll(ClaimTypes.NameIdentifier).ToList();
                var userId = identifierClaims.Last().Value;
                newLink.Owner = userId;
                newLink.ShortName = _nameGenerator.Generate();
                _linkAPIContext.Links.Add(newLink);
                await _linkAPIContext.SaveChangesAsync();
                return CreatedAtAction("GetLink", new { LinkId = newLink.Id }, newLink);
            }
            else
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Deletes a link by Id
        /// </summary>
        [HttpDelete("{LinkId}")]
        public async Task<ActionResult> DeleteLink(Guid LinkId)
        {
            var link = await _linkAPIContext.Links.FirstOrDefaultAsync(x => x.Id == LinkId);
            
            if(link == null) return BadRequest();

            _linkAPIContext.Remove(link);
            await _linkAPIContext.SaveChangesAsync();
            return NoContent();
        }
    }
}
