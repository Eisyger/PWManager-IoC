namespace PWManager.interfaces;

public interface IValidationService
{
    public string ValidationMessage {get;}
    public bool ValidateUserAndPassword(string username, string password);
}