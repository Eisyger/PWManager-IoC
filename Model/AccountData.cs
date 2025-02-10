using PWManager.Interfaces;

namespace PWManager.Model
{
    public class AccountData : IDataContext
    {
        public string Name { get; set; } = string.Empty;
        public string User { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Website { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public override string ToString()
        {
            return @$"
    +-----------------------+
    |   ACCOUNT DETAILS     |
    +-----------------------+
    | Name:         {Name}
    | Username:     {User}
    | Password:     {Password}
    | Website:      {Website}
    | Description:  {(string.IsNullOrWhiteSpace(Description) ? " (keine Beschreibung)" : $" {Description}")}
    +-----------------------+";
        }

    }
}
