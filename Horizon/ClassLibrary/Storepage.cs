namespace ClassLibrary
{
    public class Storepage(decimal price, DateTime releaseDate) : IDisposable
    {
        public static float Tax = 0.23f;

        private decimal price = price;
        public decimal Price
        {
            get => price;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("Price cannot be lower then 0");
            }
        }
        public decimal ConsumerPrice => Price * (decimal)Tax;

        public DateTime ReleaseDate { get; private set; } = releaseDate;
        public List<string> Screenshots { get; } = new();
        public List<string> supportedLanguages { get; } = new();
        public List<Review> Reviews { get; } = new();

        public float GameScore 
        { 
            get
            {
                if (Reviews.Count == 0)
                    return 0;

                int score = 0;
                Reviews.ForEach(review => score += review.Score);
                return score / Reviews.Count;
            }
        }

        public void Dispose() => Reviews.Clear();
    }
}
