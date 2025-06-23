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
        public Dictionary<string, CamelState> CamelStates { get; set; } = new Dictionary<string, CamelState>();
        public List<DiceRoll> DiceRolled { get; set; } = new List<DiceRoll>();

        public GameState DeepClone()
        {
            return new GameState
            {
                CamelStates = this.CamelStates
                    .Select(kvp => new KeyValuePair<string, CamelState>(kvp.Key, new CamelState
                    {
                        DropZone = kvp.Value.DropZone,
                        StackIndex = kvp.Value.StackIndex
                    }))
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value),

                DiceRolled = new List<DiceRoll>(this.DiceRolled.Select(d => new DiceRoll
                {
                    Color = d.Color,
                    Value = d.Value
                }))
            };
        }

        public void MoveCamel(string color, int spaces)
        {
            if (!CamelStates.TryGetValue(color, out var baseCamel))
                return;

            int currentSpace = ParseDropZoneNumber(baseCamel.DropZone);

            // Get all camels at this space, sorted bottom-to-top
            var stack = CamelStates
                .Where(c => ParseDropZoneNumber(c.Value.DropZone) == currentSpace)
                .OrderBy(c => c.Value.StackIndex)
                .ToList();

            // Find the index of the moving camel
            int baseIndex = stack.FindIndex(c => c.Key.Equals(color, StringComparison.OrdinalIgnoreCase));

            // Only move the camel and any camels on top of it
            var movingStack = stack.Skip(baseIndex).ToList();

            int newSpace = currentSpace + spaces;
            string newDropZone = $"space-{newSpace}";

            // Find top of destination stack
            int destinationTopIndex = CamelStates
                .Where(c => c.Value.DropZone == newDropZone)
                .Select(c => c.Value.StackIndex)
                .DefaultIfEmpty(-1)
                .Max();

            // Reassign the camels to the new drop zone, stacked above existing
            for (int i = 0; i < movingStack.Count; i++)
            {
                var camelColor = movingStack[i].Key;
                CamelStates[camelColor] = new CamelState
                {
                    DropZone = newDropZone,
                    StackIndex = destinationTopIndex + 1 + i
                };
            }
        }

        public void MoveCamelWithStack(string color, int spaces)
        {
            if (!CamelStates.TryGetValue(color, out var camel))
                return;

            int currentSpace = ParseDropZoneNumber(camel.DropZone);

            // Get full stack at current space (bottom to top)
            var stack = CamelStates
                .Where(c => ParseDropZoneNumber(c.Value.DropZone) == currentSpace)
                .OrderBy(c => c.Value.StackIndex)
                .Select(c => c.Key)
                .ToList();

            // Find where in the stack the moving camel is
            int indexInStack = stack.IndexOf(color);

            // All camels above (including self) move together
            var camelsToMove = stack.Skip(indexInStack).ToList();

            int newSpace = currentSpace + spaces;
            string newDropZone = $"space-{newSpace}";

            // Determine the stack index at the new location
            int baseStackIndex = CamelStates
                .Where(c => c.Value.DropZone == newDropZone)
                .Select(c => c.Value.StackIndex)
                .DefaultIfEmpty(-1)
                .Max() + 1;

            // Move each camel in order, preserving relative stack
            for (int i = 0; i < camelsToMove.Count; i++)
            {
                var c = camelsToMove[i];
                CamelStates[c] = new CamelState
                {
                    DropZone = newDropZone,
                    StackIndex = baseStackIndex + i
                };
            }
        }










        private int ParseDropZoneNumber(string dropZoneId)
        {
            if (dropZoneId.StartsWith("space-") && int.TryParse(dropZoneId.Substring(6), out int number))
                return number;

            return 0;
        }

        public List<string> GetRemainingCamelColors()
        {
            var allColors = new[] { "Blue", "Green", "Red", "Yellow", "Purple" };
            return allColors
                .Where(color => !DiceRolled.Any(d => d.Color.Equals(color, StringComparison.OrdinalIgnoreCase)))
                .ToList();
        }

        public List<(string Color, int Space, int StackIndex)> GetCamelSnapshot()
        {
            return CamelStates.Select(kvp => (
                Color: kvp.Key,
                Space: ParseDropZoneNumber(kvp.Value.DropZone),
                StackIndex: kvp.Value.StackIndex
            )).ToList();
        }

        public IEnumerable<(string Color, int Space, int StackIndex)> Camels
        {
            get
            {
                return CamelStates.Select(kvp => (
                    Color: kvp.Key,
                    Space: ParseDropZoneNumber(kvp.Value.DropZone),
                    StackIndex: kvp.Value.StackIndex
                ));
            }
        }


    }
}
