namespace ClassLibrary
{
    public class Game : IDisposable
    {
        public static Dictionary<int, Game> Games { get; } = new();

        public enum GameState { InCreation, PendingForVerification, Published }

        public int GameId { get; private set; }
        public GameState State { get; set; } = GameState.InCreation;

        private Storepage? storepage;
        public Storepage Storepage 
        { 
            get
            {
                if (storepage == null)
                    throw new ArgumentNullException("Storepage is null");
                
                return storepage;
            }
            set
            {
                if (value == null && storepage != null)
                    storepage.Dispose();

                storepage = value;
            } 
        }

        public Game(int gameId)
        {
            GameId = gameId;
            Games.Add(gameId, this);
        }

        public void Dispose() =>  Storepage = null;
    }
}
