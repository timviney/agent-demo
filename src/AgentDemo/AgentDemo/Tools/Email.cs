using System.ComponentModel;

namespace AgentDemo.Tools;

public class Email
{
    [Description("Send an email with the given subject and body to the specified address.")]
    public static async Task<string> SendEmail([Description("The email address to send to")] string address, [Description("The email subject.")] string subject, [Description("The email body.")] string body)
    {
        TerminalUi.Current?.LogToolUse(nameof(SendEmail), (nameof(address), address), (nameof(subject), subject), (nameof(body), body));

        // Simulate email sending delay
        await Task.Delay(1000);

        TerminalUi.Current?.ShowEmailSent(address, subject, body);

        // Return a confirmation message
        return $"Email successfully sent to {address} with subject '{subject}'.";
    }
}