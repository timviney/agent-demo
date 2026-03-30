using System.ComponentModel;

namespace AgentDemo.Tools;

public class UserDetails
{
    [Description("Get helpful information about the user.")]
    public static async Task<string> GetUserDetails()
    {
        TerminalUi.Current?.LogToolUse(nameof(GetUserDetails));

        // Return hard-coded dummy user data
        var userInfo = new
        {
            Name = "John Smith",
            Email = "john.smith@example.com",
            PhoneNumber = "+44-123123-0123",
            Location = "London, UK",
            PreferredLanguage = "English, GB",
            AccountStatus = "Active",
            MemberSince = "2022-03-15",
            TimeZone = "Europe/London"
        };

        return $"User Information - Name: {userInfo.Name}, Email: {userInfo.Email}, Phone: {userInfo.PhoneNumber}, " +
               $"Location: {userInfo.Location}, Preferred Language: {userInfo.PreferredLanguage}, " +
               $"Account Status: {userInfo.AccountStatus}, Member Since: {userInfo.MemberSince}, " +
               $"Time Zone: {userInfo.TimeZone}";
    }
}