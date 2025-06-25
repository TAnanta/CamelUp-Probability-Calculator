using CamelUpProbabilityCalc.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CamelUpProbabilityCalc.Logic
{
    public static class ProbabilityCalculator
    {
        // These are the five camel colors used throughout the game
        private static readonly string[] CamelColors = { "Blue", "Green", "Red", "Yellow", "Purple" };

        // Each die has a 2/6 chance of rolling a 1, 2, or 3 — standard for Camel Up
        private static readonly Dictionary<int, double> MoveProbabilities = new()
        {
            { 1, 2.0 / 6 },
            { 2, 2.0 / 6 },
            { 3, 2.0 / 6 }
        };

        // Main method that gets called to compute each camel's chance of winning the leg
        public static Dictionary<string, double> CalculateWinProbabilities(GameState gameState)
        {
            // Set up default dictionary, each camel starts at 0% chance
            var probabilities = CamelColors.ToDictionary(color => color, _ => 0.0);

            // Begin exploring all the ways dice can be rolled this round
            // We use a cloned state to avoid messing up the original
            ExploreAllRollOrders(gameState.DeepClone(), new HashSet<string>(), 1.0, probabilities);

            // Once all branches are explored, normalize values to sum up to 100%
            double total = probabilities.Values.Sum();
            if (total > 0)
            {
                foreach (var color in CamelColors)
                {
                    probabilities[color] = probabilities[color] / total * 100.0;
                }
            }

            return probabilities;
        }

        // Recursively simulate every legal combination of dice rolls
        private static void ExploreAllRollOrders(GameState state, HashSet<string> usedDice, double currentProb, Dictionary<string, double> winTally)
        {
            // Base case: all dice have been rolled — pick whoever's winning right now
            if (usedDice.Count == CamelColors.Length)
            {
                var leader = state.Camels
                    .OrderByDescending(c => c.Space)       // First by board space
                    .ThenByDescending(c => c.StackIndex)   // Then by stack position (top wins ties)
                    .First();

                // Add this outcome’s probability to the winner’s tally
                winTally[leader.Color] += currentProb;
                return;
            }

            // Try all unused dice next
            foreach (var color in CamelColors.Where(c => !usedDice.Contains(c)))
            {
                // Try rolling a 1, 2, or 3 (each with 2/6 chance)
                foreach (var move in MoveProbabilities)
                {
                    var nextState = state.DeepClone(); // Avoid side effects

                    // Move that camel (and carry anyone stacked above it)
                    nextState.MoveCamelWithStack(color, move.Key);

                    // Mark this die as rolled
                    var nextUsed = new HashSet<string>(usedDice) { color };
                    nextState.DiceRolled.Add(new DiceRoll { Color = color, Value = move.Key });

                    // Dive deeper into the recursion tree
                    ExploreAllRollOrders(nextState, nextUsed, currentProb * move.Value, winTally);
                }
            }
        }
    }
}
