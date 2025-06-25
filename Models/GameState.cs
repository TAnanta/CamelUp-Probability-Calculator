using System;
using System.Collections.Generic;
using System.Linq;

namespace CamelUpProbabilityCalc.Models
{
    // Holds info about where a camel is on the board and its position in the stack
    public class CamelState
    {
        public string DropZone { get; set; } = string.Empty; // Which tile the camel is on (like "space-5")
        public int StackIndex { get; set; } // 0 means bottom of the stack, higher numbers mean higher up
    }

    // Represents a single dice roll that has occurred in the current round
    public class DiceRoll
    {
        public string Color { get; set; } = string.Empty; // Which camel’s die this is
        public int Value { get; set; } // The value rolled (1, 2, or 3)
    }

    public class GameState
    {
        // Tracks where all camels are on the board
        public Dictionary<string, CamelState> CamelStates { get; set; } = new();

        // List of dice that have already been rolled this round
        public List<DiceRoll> DiceRolled { get; set; } = new();

        // Creates a deep copy of the game state so the simulation doesn’t mutate the original
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
                    .ToList()
            };
        }

        // Moves a camel (and any camels above it) forward by a certain number of spaces
        public void MoveCamelWithStack(string color, int spaces)
        {
            if (!CamelStates.TryGetValue(color, out var camel)) return;

            int currentSpace = ParseDropZoneNumber(camel.DropZone);

            // Grab the full stack at this space and sort from bottom to top
            var stack = CamelStates
                .Where(c => ParseDropZoneNumber(c.Value.DropZone) == currentSpace)
                .OrderBy(c => c.Value.StackIndex)
                .Select(c => c.Key)
                .ToList();

            // Figure out which part of the stack is moving (this camel + everyone on top of it)
            int startIndex = stack.IndexOf(color);
            var movingUnit = stack.Skip(startIndex).ToList();

            int newSpace = currentSpace + spaces;
            string newDropZone = $"space-{newSpace}";

            // Get camels already at the destination (so we can stack correctly)
            var existing = CamelStates
                .Where(c => ParseDropZoneNumber(c.Value.DropZone) == newSpace && !movingUnit.Contains(c.Key))
                .OrderBy(c => c.Value.StackIndex)
                .Select(c => c.Key)
                .ToList();

            // Stack at the new spot should be existing ones first, then the moving group on top
            var newStack = existing.Concat(movingUnit).ToList();

            // Reassign drop zone and stack position for each camel (bottom camel gets index 0)
            for (int i = 0; i < newStack.Count; i++)
            {
                string camelKey = newStack[i];
                CamelStates[camelKey] = new CamelState
                {
                    DropZone = newDropZone,
                    StackIndex = i
                };
            }
        }

        // Converts a drop zone string like "space-8" into an integer 8
        private int ParseDropZoneNumber(string dropZoneId)
        {
            return dropZoneId.StartsWith("space-") && int.TryParse(dropZoneId[6..], out int num) ? num : 0;
        }

        // Returns all camels with their color, tile number, and stack index
        public IEnumerable<(string Color, int Space, int StackIndex)> Camels =>
            CamelStates.Select(kvp => (
                kvp.Key,
                ParseDropZoneNumber(kvp.Value.DropZone),
                kvp.Value.StackIndex));

        // Filters out which camel dice haven’t been rolled yet
        public List<string> GetRemainingCamelColors()
        {
            var allColors = new[] { "Blue", "Green", "Red", "Yellow", "Purple" };
            return allColors
                .Where(color => !DiceRolled.Any(d => d.Color.Equals(color, StringComparison.OrdinalIgnoreCase)))
                .ToList();
        }
    }
}
