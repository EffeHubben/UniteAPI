using Microsoft.AspNetCore.Mvc;
using WereldbouwerAPI;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace WereldbouwerAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WereldBouwerController : ControllerBase
    {
        private readonly IWereldBouwerRepository _wereldBouwerRepository;
        private readonly IAuthenticationService _authenticationService;
        private readonly ILogger<WereldBouwerController> _logger;

        public WereldBouwerController(IWereldBouwerRepository repository, IAuthenticationService authenticationService, ILogger<WereldBouwerController> logger)
        {
            _wereldBouwerRepository = repository;
            _authenticationService = authenticationService;
            _logger = logger;
        }


        [HttpGet("GetUserId", Name = "GetUserId")]
        [Authorize]
        public ActionResult<string> GetUserId()
        {
            var userId = _authenticationService.GetCurrentAuthenticatedUserId();
            return Ok(userId);
        }


        [HttpGet(Name = "GetWereldBouwer")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<WereldBouwer>>> Get()
        {
            var userId = _authenticationService.GetCurrentAuthenticatedUserId();
            var wereldBouwers = await _wereldBouwerRepository.GetAllAsync();
            return Ok(wereldBouwers);
        }

        [HttpGet("getwereld/{userId}", Name = "GetWereldBouwerByUserId")]
        [Authorize]
        public async Task<ActionResult<WereldBouwer>> GetWereld(string userId)
        {
            //var userId = _authenticationService.GetCurrentAuthenticatedUserId();
            var wereldBouwer = await _wereldBouwerRepository.GetByUserIdAsync(userId);
            if (wereldBouwer == null)
            {
                return NotFound();
            }
            return Ok(wereldBouwer);
        }

        [HttpGet("{wereldBouwerId}", Name = "GetWereldBouwerById")]
        [Authorize]
        public async Task<ActionResult<WereldBouwer>> Get(Guid wereldBouwerId)
        {
            var userId = _authenticationService.GetCurrentAuthenticatedUserId();
            var wereldBouwer = await _wereldBouwerRepository.GetByWereldbouwerIdAsync(wereldBouwerId);
            if (wereldBouwer == null)
            {
                return NotFound();
            }
            return Ok(wereldBouwer);
        }

        [HttpPost(Name = "PostWereldBouwer")]
        [Authorize]
        public async Task<IActionResult> Post(WereldBouwer wereldBouwer)
        {
            var userId = _authenticationService.GetCurrentAuthenticatedUserId();
            wereldBouwer.id = Guid.NewGuid();
            wereldBouwer.ownerUserId = userId;
            await _wereldBouwerRepository.AddAsync(wereldBouwer);
            return CreatedAtRoute("GetWereldBouwer", new { id = wereldBouwer.id }, wereldBouwer);
        }


        [HttpPut("{wereldBouwerId}", Name = "PutWereldBouwer")]
        [Authorize]
        public async Task<ActionResult> Put(Guid wereldBouwerId, WereldBouwer newWereldBouwer)
        {
            var userId = _authenticationService.GetCurrentAuthenticatedUserId();
            var existingWereldBouwer = await _wereldBouwerRepository.GetByWereldbouwerIdAsync(wereldBouwerId);
            if (existingWereldBouwer == null)
            {
                return NotFound();
            }
            newWereldBouwer.id = wereldBouwerId;
            newWereldBouwer.ownerUserId = userId;
            await _wereldBouwerRepository.UpdateAsync(newWereldBouwer);
            return CreatedAtRoute("GetWereldBouwer", new { id = newWereldBouwer.id }, newWereldBouwer);
        }


        [HttpDelete("{wereldBouwerId}", Name = "DeleteWereldBouwer")]
        [Authorize]
        public async Task<IActionResult> Delete(Guid wereldBouwerId)
        {
            var userId = _authenticationService.GetCurrentAuthenticatedUserId();
            var existingWereldBouwer = await _wereldBouwerRepository.GetByWereldbouwerIdAsync(wereldBouwerId);
            if (existingWereldBouwer == null)
            {
                return NotFound();
            }

            await _wereldBouwerRepository.DeleteAsync(wereldBouwerId);
            return Ok(wereldBouwerId);
        }


    }
}
