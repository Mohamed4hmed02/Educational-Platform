namespace Educational_Platform.Application.Abstractions.SecurityInterfaces
{
    public interface IHashingServices
    {
        string Hash(string value);
        bool Verify(string value, string hashedValue);
    }
}
