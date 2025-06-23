namespace CamelUpProbability.Models
{
    public class DiceRoll
    {
        public string Color { get; set; } //This info will let us know if a dice has been rolled. If these values are not null, then the dice has been rolled.
        public int Value { get; set; }

        public DiceRoll(string color, int value)
        {
            Color = color;
            Value = value;
        }
    }
}
