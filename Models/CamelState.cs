namespace CamelUpProbability.Models
{
    public class CamelState
    {
        public string Color { get; set; }
        public int Position { get; set; } // Board tile number based on drop zone
        public int StackIndex { get; set; } // 0 is bottom, higher = higher in stack & placement
        public bool HasRolled { get; set; } // Whether this camel's dice has already rolled, if it has, then it's locked in to its current position

        public CamelState(string color, int position, int stackIndex, bool hasRolled)
        {
            Color = color;
            Position = position;
            StackIndex = stackIndex;
            HasRolled = hasRolled;
        }
    }
}
