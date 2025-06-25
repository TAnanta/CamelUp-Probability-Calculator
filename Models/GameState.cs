
using System;
using System.Collections.Generic;
using System.Linq;

namespace CamelUpProbabilityCalc.Models
{
    public class CamelState
    {
        public string DropZone { get; set; } = string.Empty;
        public int StackIndex { get; set; }
    }

    public class DiceRoll
    {
        public string Color { get; set; } = string.Empty;
        public int Value { get; set; }
    }

    public class GameState
    {
        public Dictionary<string, CamelState> CamelStates { get; set; } = new();
        public List<DiceRoll> DiceRolled { get; set; } = new();
        public HashSet<string> CamelsThatMoved { get; set; } = new();

        public GameState DeepClone()
        {
            return new GameState
            {
                CamelStates = this.CamelStates.ToDictionary(
                    kvp => kvp.Key,
                    kvp => new CamelState
                    {
                        DropZone = kvp.Value.DropZone,
                        StackIndex = kvp.Value.StackIndex
                    }),
                DiceRolled = this.DiceRolled
                    .Select(d => new DiceRoll { Color = d.Color, Value = d.Value })
                    .ToList(),
                CamelsThatMoved = new HashSet<string>(this.CamelsThatMoved)
            };
        }

        public void MoveCamelWithStack(string color, int spaces)
        {
            if (!CamelStates.TryGetValue(color, out var camel)) return;

            int currentSpace = ParseDropZoneNumber(camel.DropZone);

            var stackAtSpace = CamelStates
                .Where(c => ParseDropZoneNumber(c.Value.DropZone) == currentSpace)
                .ToList();

            var movingUnit = stackAtSpace
                .Where(c => c.Value.StackIndex >= camel.StackIndex)
                .OrderBy(c => c.Value.StackIndex)
                .Select(c => c.Key)
                .ToList();

            int newSpace = currentSpace + spaces;
            string newDropZone = $"space-{newSpace}";

            Console.WriteLine($"[{color}] moves {spaces} from space-{currentSpace} to space-{newSpace} carrying: {string.Join(", ", movingUnit)}");

            var existing = CamelStates
                .Where(c => ParseDropZoneNumber(c.Value.DropZone) == newSpace && !movingUnit.Contains(c.Key))
                .OrderBy(c => c.Value.StackIndex)
                .Select(c => c.Key)
                .ToList();

            var newStack = existing.Concat(movingUnit).ToList();

            for (int i = 0; i < newStack.Count; i++)
            {
                string camelKey = newStack[i];
                CamelStates[camelKey] = new CamelState
                {
                    DropZone = newDropZone,
                    StackIndex = i
                };
            }

            Console.WriteLine($"→ New stack at {newDropZone}: {string.Join(", ", newStack.Select(c => $"{c}({CamelStates[c].StackIndex})"))}");

            foreach (var movedCamel in movingUnit)
            {
                CamelsThatMoved.Add(movedCamel);
            }
        }

        private int ParseDropZoneNumber(string dropZoneId)
        {
            return dropZoneId.StartsWith("space-") && int.TryParse(dropZoneId[6..], out int num) ? num : 0;
        }

        public IEnumerable<(string Color, int Space, int StackIndex)> Camels =>
            CamelStates.Select(kvp => (
                kvp.Key,
                ParseDropZoneNumber(kvp.Value.DropZone),
                kvp.Value.StackIndex));

        public List<string> GetRemainingCamelColors()
        {
            var allColors = new[] { "Blue", "Green", "Red", "Yellow", "Purple" };
            return allColors
                .Where(color => !DiceRolled.Any(d => d.Color.Equals(color, StringComparison.OrdinalIgnoreCase)))
                .ToList();
        }

        /*===========================DEBUG BOARD STATE PRINTING============================
         * public void PrintBoardState()
        {
            var bySpace = CamelStates
                .GroupBy(kvp => ParseDropZoneNumber(kvp.Value.DropZone))
                .OrderByDescending(g => g.Key);

            Console.WriteLine("---- Final Stack State ----");
            foreach (var group in bySpace)
            {
                var stack = group.OrderBy(c => c.Value.StackIndex)
                                 .Select(c => $"{c.Key}({c.Value.StackIndex})");
                Console.WriteLine($"space-{group.Key}: {string.Join(" → ", stack)}");
            }
            Console.WriteLine();
        }
        =====================================================================================*/
    }
}
