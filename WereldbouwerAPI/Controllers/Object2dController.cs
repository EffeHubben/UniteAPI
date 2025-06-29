using Microsoft.AspNetCore.Mvc;
using WereldbouwerAPI;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;

namespace WereldbouwerAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Object2DController : ControllerBase
    {
        private readonly IObject2DRepository _object2DRepository;
        private readonly IAuthenticationService _authenticationService;
        private readonly ILogger<Object2DController> _logger;

        public Object2DController(IObject2DRepository repository, IAuthenticationService authenticationService, ILogger<Object2DController> logger)
        {
            _object2DRepository = repository;
            _authenticationService = authenticationService;
            _logger = logger;
        }

        [HttpGet("{environmentId}", Name = "GetAllObject2D")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Object2D>>> GetByEnvironmentId(Guid environmentId)
        {
            var objects = await _object2DRepository.GetByEnvironmentIdAsync(environmentId);
            return Ok(objects);
        }

        [HttpPost(Name = "AddObject2D")]
        [Authorize]
        public async Task<ActionResult<Object2D>> AddObject2D(Object2D object2D)
        {
            if (string.IsNullOrEmpty(object2D.prefabId))
            {
                ModelState.AddModelError("prefabId", "The prefabId field is required.");
                return BadRequest(ModelState);
            }

            object2D.id = Guid.NewGuid();
            var result = await _object2DRepository.AddObject2DAsync(object2D);
            return Ok(result);
        }

        [HttpPut("UpdateObject2D", Name = "UpdateObject2D")]
        [Authorize]
        public async Task<ActionResult<Object2D>> UpdateObject2DAsync(Object2D object2D)
        {
            var result = await _object2DRepository.UpdateObject2DAsync(object2D);
            return Ok(result);
        }

        [HttpDelete("{id}", Name = "DeleteObject2D")]
        [Authorize]
        public async Task<IActionResult> DeleteObject2D(Guid id)
        {
            await _object2DRepository.DeleteObjectAsync(id);
            return NoContent();
        }

        [HttpDelete("environment/{environmentId}", Name = "DeleteAllByEnvironmentId")]
        [Authorize]
        public async Task<IActionResult> DeleteAllByEnvironmentId(Guid environmentId)
        {
            await _object2DRepository.DeleteAllByEnvironmentIdAsync(environmentId);
            return NoContent();
        }
    }
}
