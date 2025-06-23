using CamelUpProbabilityCalc.Logic;
using CamelUpProbabilityCalc.Models;
using Microsoft.AspNetCore.Mvc;

namespace CamelUpProbabilityCalc.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProbabilityController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post([FromBody] GameState gameState)
        {
            if (gameState == null || gameState.CamelStates.Count != 5)
            {
                return BadRequest("Invalid game state submitted.");
            }

            var probabilities = ProbabilityCalculator.CalculateWinProbabilities(gameState);
            return Ok(probabilities);
        }
    }
}
