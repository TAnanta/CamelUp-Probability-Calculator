using CamelUpProbabilityCalc.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CamelUpProbabilityCalc.Logic
{
    public static class ProbabilityCalculator
    {
        private static readonly string[] CamelColors = { "Blue", "Green", "Red", "Yellow", "Purple" };

        public static Dictionary<string, double> CalculateWinProbabilities(GameState gameState)
        {
            var probabilities = new Dictionary<string, double>();

            // STEP 1: Filter out the camels that still have a dice to roll
            var remainingCamels = CamelColors
                .Where(color => !gameState.DiceRolled.Any(d => d.Color.Equals(color, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            // STEP 2: If all dice are rolled, simply find who's in the lead
            if (remainingCamels.Count == 0)
            {
                var leadingCamel = gameState.Camels
                    .OrderByDescending(c => c.Space)
                    .ThenByDescending(c => c.StackIndex)
                    .FirstOrDefault();

                foreach (var camel in CamelColors)
                {
                    probabilities[camel] = camel == leadingCamel.Color ? 100.0 : 0.0;
                }

                return probabilities;
            }

            // STEP 3: Run simulations for remaining camels
            var winCounts = CamelColors.ToDictionary(color => color, _ => 0);
            int simulations = 10000;
            var random = new Random();

            for (int i = 0; i < simulations; i++)
            {
                var simulatedState = gameState.DeepClone();
                SimulateRound(simulatedState, random);

                var winner = simulatedState.Camels
                    .OrderByDescending(c => c.Space)
                    .ThenByDescending(c => c.StackIndex)
                    .First();

                winCounts[winner.Color]++;
            }

            foreach (var camel in CamelColors)
            {
                probabilities[camel] = (double)winCounts[camel] / simulations * 100.0;
            }

            return probabilities;
        }

        private static void SimulateRound(GameState state, Random random)
        {
            var remaining = state.GetRemainingCamelColors();

            // Shuffle remaining dice
            remaining = remaining.OrderBy(_ => random.Next()).ToList();

            foreach (var color in remaining)
            {
                int move = random.Next(1, 4); // Simulate 1, 2, or 3
                state.MoveCamel(color, move);
            }
        }
    }
}
