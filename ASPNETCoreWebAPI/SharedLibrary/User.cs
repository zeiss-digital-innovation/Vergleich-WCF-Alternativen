namespace SharedLibrary
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public User() { }
        public User(string name)
        {
            Name = name;
        }
    }
}
