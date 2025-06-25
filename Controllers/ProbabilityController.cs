using CamelUpProbabilityCalc.Logic;
using CamelUpProbabilityCalc.Models;
using Microsoft.AspNetCore.Mvc;

namespace CamelUpProbabilityCalc.Controllers
{
    // This controller handles API requests for probability calculations
    [ApiController]
    [Route("api/[controller]")]
    public class ProbabilityController : ControllerBase
    {
        // POST endpoint — front end sends current GameState to this endpoint
        [HttpPost]
        public IActionResult Post([FromBody] GameState gameState)
        {
            // Basic check to make sure a game was actually submitted and has all 5 camels
            if (gameState == null || gameState.CamelStates.Count != 5)
            {
                return BadRequest("Invalid game state submitted.");
            }

            // Call the core logic to calculate win chances based on board and dice status
            var probabilities = ProbabilityCalculator.CalculateWinProbabilities(gameState);

            // Return the result as JSON (each camel's probability)
            return Ok(probabilities);
        }
    }
}
