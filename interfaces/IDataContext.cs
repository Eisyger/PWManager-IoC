namespace PWManager.interfaces
{
    public interface IDataContext
    {
        string Name { get; set; }
        string User { get; set; }
        string Password { get; set; }
        string Website { get; set; }
        string Description { get; set; }

        string ToString();
    }
}