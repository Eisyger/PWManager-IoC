namespace PWManager.Entity;

public class SessionEntity
{
    public string EncryptedAccoundService { get; set; } = "";
    public string Salt {get; set;} = "";
    public string Identifier { get; set; } = "";
}