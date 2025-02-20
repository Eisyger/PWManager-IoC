namespace PWManager.Context;

public class AccountEntity
{
    public string EncryptedAccount { get; set; } = "";
    public string Salt {get; set;} = "";
    public string User { get; set; } = "";
}