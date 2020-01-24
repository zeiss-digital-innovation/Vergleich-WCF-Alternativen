namespace SharedLibrary
{
    public class User
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public User() { }
        public User(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
