namespace ClassLibrary
{
    public class Review(int score, string? text)
    {
        private int score = score;
        public int Score 
        { 
            get => score; 
            private set
            {
                if (value > 0 && value < 11)
                    score = value;
                else
                    throw new ArgumentOutOfRangeException("Score is not between 1 and 10");
            } 
        }

        public string? Text { get; private set; } = text;
    }
}
