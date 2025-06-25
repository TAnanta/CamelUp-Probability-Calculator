using CamelUpProbabilityCalc.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
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
            var remainingDice = gameState.GetRemainingCamelColors();

            // Generate all valid dice roll *orders* (permutations)
            var rollOrders = GetPermutations(remainingDice, remainingDice.Count);

            /*============debug============
            Console.WriteLine("=== Starting Simulation ===");
            gameState.PrintBoardState();
            ===============================*/

            foreach (var order in rollOrders)
            {
                // Generate all value combinations for this roll order


                var valueCombos = GetAllDiceValueCombinations(order.Count()).ToList(); // combo becomes indexable
                var orderList = order.ToList(); // make order indexable

                foreach (var combo in valueCombos)
                {
                    // Always clone from the original board state
                    var cloned = gameState.DeepClone();

                    for (int i = 0; i < orderList.Count; i++)
                    {
                        string color = orderList[i];
                        int value = combo[i];

                        cloned.MoveCamelWithStack(color, value);
                        cloned.DiceRolled.Add(new DiceRoll { Color = color, Value = value });
                    }

                    /* =============debug=============
                    Console.WriteLine("==== Final Board State ====");
                    cloned.PrintBoardState();
                    ===================================*/

            //After all dice have been rolled, determine the current leader
            var leader = cloned.Camels
                        .OrderByDescending(c => c.Space)
                        .ThenByDescending(c => c.StackIndex)
                        .First();

                    /*===================debug==================
                    Console.WriteLine($"Winner: {leader.Color}, Space: {leader.Space}, StackIndex: {leader.StackIndex}");
                    Console.WriteLine("Rolled Dice: " + string.Join(", ", order.Zip(combo, (color, val) => $"{color}:{val}")));
                    Console.WriteLine();
                    //Console.WriteLine("Press any key to continue...");
                    //Console.ReadKey();
                    ================================================*/

                    double branchProb = combo.Select(v => MoveProbabilities[v]).Aggregate(1.0, (a, b) => a * b);
                    probabilities[leader.Color] += branchProb;
                }
            }

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

        // Generate all possible dice value combinations for N dice
        private static List<List<int>> GetAllDiceValueCombinations(int diceCount)
        {
            var results = new List<List<int>>();

            void Backtrack(List<int> current)
            {
                if (current.Count == diceCount)
                {
                    results.Add(new List<int>(current));
                    return;
                }

                foreach (var val in MoveProbabilities.Keys)
                {
                    current.Add(val);
                    Backtrack(current);
                    current.RemoveAt(current.Count - 1);
                }
            }

            Backtrack(new List<int>());
            return results;
        }

        // Helper to generate permutations of a list (used for dice order)
        private static IEnumerable<IEnumerable<T>> GetPermutations<T>(IEnumerable<T> list, int length)
        {
            if (length == 1)
                return list.Select(t => new T[] { t });

            return GetPermutations(list, length - 1)
                .SelectMany(t => list.Where(e => !t.Contains(e)),
                            (t1, t2) => t1.Concat(new T[] { t2 }));
        }
    }
}
