namespace ClassLibrary
{
    public class User(string username)
    {
        public string Username { get; private set; } = username;

        public IUserRole UserType { get; private set; }

        public bool TryChangeUserType(IUserRole newUserType)
        {
            UserType = newUserType;
            return true;
        }
    }
}
