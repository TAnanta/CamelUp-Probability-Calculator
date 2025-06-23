using CamelUpProbabilityCalc.Models;
using CamelUpProbabilityCalc.Logic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace CamelUpProbabilityCalc.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }

        [IgnoreAntiforgeryToken]
        public IActionResult OnPostCalculateProbabilities([FromBody] GameState state)
        {
            if (state == null)
            {
                return BadRequest("Invalid game state");
            }

            var result = ProbabilityCalculator.CalculateWinProbabilities(state);
            return new JsonResult(result);
        }
    }
}
