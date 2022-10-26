namespace CoreBanking.Application.Core.Models;

public record CustomerArchiveItem
{
    public CustomerArchiveItem(Guid id, string firstname, string lastname)
    {
        Id = id;
        Firstname = firstname;
        Lastname = lastname;
    }

    public Guid Id { get; }
    public string Firstname { get; }
    public string Lastname { get; }
}