using CamelUpProbabilityCalc.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CamelUpProbabilityCalc.Logic
{
    public static class ProbabilityCalculator
    {
        private static readonly string[] CamelColors = { "Blue", "Green", "Red", "Yellow", "Purple" };

        private static readonly Dictionary<int, double> MoveProbabilities = new()
        {
            { 1, 1.0 / 3 },
            { 2, 1.0 / 3 },
            { 3, 1.0 / 3 }
        };

        public static Dictionary<string, double> CalculateWinProbabilities(GameState gameState)
        {
            var probabilities = CamelColors.ToDictionary(color => color, _ => 0.0);

            var remainingColors = CamelColors
                .Where(color => !gameState.DiceRolled.Any(d => d.Color.Equals(color, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            if (remainingColors.Count == 0)
            {
                var leading = gameState.Camels
                    .OrderByDescending(c => c.Space)
                    .ThenByDescending(c => c.StackIndex)
                    .FirstOrDefault();

                foreach (var color in CamelColors)
                {
                    probabilities[color] = color == leading.Color ? 100.0 : 0.0;
                }

                return probabilities;
            }

            ExploreAllOutcomes(gameState, remainingColors, 1.0, probabilities);

            // Normalize to sum to 100%
            double total = probabilities.Values.Sum();
            if (total > 0)
            {
                foreach (var color in CamelColors.ToList())
                {
                    probabilities[color] = probabilities[color] / total * 100.0;
                }
            }

            return probabilities;
        }

        private static void ExploreAllOutcomes(GameState state, List<string> remainingColors, double currentProb, Dictionary<string, double> winTally)
        {
            if (remainingColors.Count == 0)
            {
                var leader = state.Camels
                    .OrderByDescending(c => c.Space)
                    .ThenByDescending(c => c.StackIndex)
                    .First();

                winTally[leader.Color] += currentProb;
                return;
            }

            foreach (var color in remainingColors)
            {
                foreach (var move in MoveProbabilities)
                {
                    var nextState = state.DeepClone();
                    nextState.MoveCamelWithStack(color, move.Key);
                    nextState.DiceRolled.Add(new DiceRoll { Color = color, Value = move.Key });

                    var nextRemaining = remainingColors.Where(c => c != color).ToList();
                    ExploreAllOutcomes(nextState, nextRemaining, currentProb * move.Value, winTally);
                }
            }
        }
    }
}
