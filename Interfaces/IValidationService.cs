namespace PWManager.Interfaces;

public interface IValidationService
{
    public (bool Valid, string Message) ValidateUserAndPassword(string username, char[] password);
}