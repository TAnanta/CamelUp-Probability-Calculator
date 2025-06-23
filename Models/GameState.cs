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
            if (!CamelStates.TryGetValue(color, out var camel))
                return;

            int currentSpace = ParseDropZoneNumber(camel.DropZone);
            int newSpace = currentSpace + spaces;
            string newDropZone = $"space-{newSpace}";

            int maxStackIndex = CamelStates
                .Where(c => c.Value.DropZone == newDropZone)
                .Select(c => c.Value.StackIndex)
                .DefaultIfEmpty(-1)
                .Max();

            CamelStates[color] = new CamelState
            {
                DropZone = newDropZone,
                StackIndex = maxStackIndex + 1
            };
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
