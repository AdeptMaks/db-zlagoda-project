namespace Api.Interfaces.Utils;

public interface IPasswordHasher
{
    public string HashPassword(string password);

    public bool Check(string password, string hashedPassword);
}

