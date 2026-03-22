using System.ComponentModel;

namespace AgentDemo.Tools;

public class GetContacts
{
    [Description("Get a list of dummy contacts for the user.")]
    public static async Task<string> GetContactsList()
    {
        await Task.CompletedTask;

        var contacts = new[]
        {
            new
            {
                Name = "Alice Johnson",
                Email = "alice.johnson@example.com",
                PhoneNumber = "+44-7700-900101",
                Relationship = "Manager",
                Location = "London, UK"
            },
            new
            {
                Name = "Ben Carter",
                Email = "ben.carter@example.com",
                PhoneNumber = "+44-7700-900102",
                Relationship = "Colleague",
                Location = "Manchester, UK"
            },
            new
            {
                Name = "Susan Smith",
                Email = "susan.smith@example.com",
                PhoneNumber = "+44-7700-564837",
                Relationship = "Sister",
                Location = "London, UK"
            },
            new
            {
                Name = "Priya Shah",
                Email = "priya.shah@example.com",
                PhoneNumber = "+44-7700-900103",
                Relationship = "Friend",
                Location = "Birmingham, UK"
            }
        };

        return "Contacts - " + string.Join(" | ", contacts.Select(contact =>
            $"Name: {contact.Name}, Email: {contact.Email}, Phone: {contact.PhoneNumber}, Relationship: {contact.Relationship}, Location: {contact.Location}"));
    }
}

