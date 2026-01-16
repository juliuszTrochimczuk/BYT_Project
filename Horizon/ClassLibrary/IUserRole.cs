namespace ClassLibrary
{
    public interface IUserRole { }

    public class UnregisteredUser(User owner) : IUserRole
    {
        private User Owner { get; set; } = owner;

        public void CreateAccount(DateTime birthDate) => Owner.TryChangeUserType(new RegisteredUser(birthDate));
    }

    public class RegisteredUser(DateTime birthDate) : IUserRole
    {
        public DateTime BirthDate { get; private set; } = birthDate;
        public Dictionary<int, Game> OwnedGames { get; private set; } = new();

        public void DeleteAccount()
        {
            Type subclass = this.GetType();
            if (subclass == typeof(Creator)) 
            {
                Creator thisAsCreator = this as Creator;
                foreach (KeyValuePair<int, Creator> pair in Creator.Creators)
                {
                    if (pair.Value == thisAsCreator)
                    {
                        Creator.Creators.Remove(pair.Key);
                        break;
                    }
                }
            }

            OwnedGames.Clear();
            Console.WriteLine("Deleted an Account");
        }

        public void CreateReview(int gameId, int score, string? text) => Game.Games[gameId].Storepage.Reviews.Add(new(score, text));

        public void PlayGame(int paymentId)
        {
            if (!OwnedGames.ContainsKey(paymentId))
                throw new ArgumentException("Cannot play game that wasn't bought");
            Console.WriteLine("Playing game");
        }

        public void DownloadGame(int paymentId)
        {
            if (!OwnedGames.ContainsKey(paymentId))
                throw new ArgumentException("Cannot download game that wasn't bought");
            Console.WriteLine("Downloading game");
        }

        public void BuyGame(int gameId)
        {
            Game foundGame = Game.Games[gameId];
            Random random = new();
            OwnedGames.Add(random.Next(), foundGame);
        }
    }

    public class Admin(int adminId, DateTime birthDate) : RegisteredUser(birthDate)
    {
        public int AdminId { get; } = adminId;
        public List<HelpTicket> OpenTickets { get; } = new();

        public void ResolveOpenTicket(HelpTicket ticket)
        {
            if (!OpenTickets.Contains(ticket))
                throw new ArgumentException($"This admin dosen't have such ticket open");
            OpenTickets.Remove(ticket);
            ticket.Sender.OpenTickets.Remove(ticket);
        }

        public void DeleteGame(int gameId)
        {
            foreach (Creator creator in Creator.Creators.Values)
            {
                Game? gameToDelete = creator.CreatedGames.FirstOrDefault(game => game.GameId == gameId);
                if (gameToDelete != null)
                {
                    gameToDelete.Dispose();
                    Game.Games.Remove(gameId);
                    creator.CreatedGames.Remove(gameToDelete);
                }
            }
        }

        public void ApproveGame(int gameId)
        {
            Game foundGame = Game.Games[gameId];
            if (foundGame.State == Game.GameState.PendingForVerification)
                foundGame.State = Game.GameState.Published;
        }
    }

    public class Creator : RegisteredUser
    {
        public static Dictionary<int, Creator> Creators { get; } = new();

        public int CreatorId { get; private set; }
        public List<Game> CreatedGames { get; } = new();
        public List<HelpTicket> OpenTickets { get; } = new();

        public Creator(int creatorId, DateTime birthDate) : base(birthDate)
        {
            CreatorId = creatorId;
            Creators.Add(CreatorId, this);
        }

        public void CreateGame(int gameId)
        {
            Game createdGame = new(gameId);
            CreatedGames.Add(createdGame);
        }

        public void EditGame(int gameId)
        {
            Game? foundGame = CreatedGames.Find(game => game.GameId == gameId);
            if (foundGame == null)
                throw new ArgumentException("This creator, haven't created this game");
            
            if (foundGame.State != Game.GameState.PendingForVerification)
                foundGame.State = Game.GameState.InCreation;
        }

        public void SendGameToApprove(int gameId)
        {
            Game? foundGame = CreatedGames.Find(game => game.GameId == gameId);
            if (foundGame == null)
                throw new ArgumentException("This creator, haven't created this game");

            if (foundGame.State == Game.GameState.InCreation)
                foundGame.State = Game.GameState.PendingForVerification;
        }

        public void ContactAdmin(string helpTicketContent, Admin whoToContact)
        {
            HelpTicket helpTicket = new(helpTicketContent, whoToContact, this);
            whoToContact.OpenTickets.Add(helpTicket);
            OpenTickets.Add(helpTicket);
        }

        public static void MoveGameOwnership(int gameId, int oldCreatorId, int newCreatorId)
        {
            Game gameToMove = Game.Games[gameId];
            Creators[oldCreatorId].CreatedGames.Remove(gameToMove);
            Creators[newCreatorId].CreatedGames.Add(gameToMove);
        }
    }
}
