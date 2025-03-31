using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using MimeKit;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MailKit;

namespace JobHunterApi
{
    public class GoogleOAuthHelper
    {
        // Scopes for Gmail API
        private static string[] Scopes = { GmailService.Scope.GmailSend };
        private static string ApplicationName = "Job Hunter";

        // Path to the credentials file (should be the same in both cases)
        private static string CredentialPath = "gmail_api_credentials.json";

        public static async Task<GmailService> AuthenticateAsync()
        {
            try
            {
                UserCredential credential;

                // Check if the credential file exists and use it
                if (File.Exists(CredentialPath))
                {
                    using (var stream = new FileStream(CredentialPath, FileMode.Open, FileAccess.Read))
                    {
                        credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                            GoogleClientSecrets.Load(stream).Secrets,
                            Scopes,
                            "user", // Unique identifier for the user
                            CancellationToken.None,
                            new FileDataStore("GmailApiTokens/client_secret.json", true),
                            new LocalServerCodeReceiver()  );
                    }
                }
                else
                {
                    // If the file doesn't exist, we create the OAuth flow
                    using (var stream = new FileStream("GmailApiTokens/client_secret.json", FileMode.Open, FileAccess.Read))
                    {
                        credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                            GoogleClientSecrets.Load(stream).Secrets,
                            Scopes,
                            "user", // Unique identifier for the user
                            CancellationToken.None,
                            new FileDataStore(CredentialPath, true)
                            );
                    }
                }

                // Create and return the Gmail service
                var service = new GmailService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = ApplicationName,
                });

                return service;
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error during authentication: {ex.Message}");
                throw;
            }
        }

        public static async Task SendEmailAsync(GmailService service, string fromEmail, string toEmail, string subject, string body,string senderName)
        {
            try
            {
                // Create the email message
                var message = new Message
                {
                    Raw = Base64UrlEncode(CreateEmail(fromEmail, toEmail, subject, body,senderName))
                };

                // Send the email using the Gmail API
                await service.Users.Messages.Send(message, fromEmail).ExecuteAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
                throw;
            }
        }

        // Create MimeMessage using MimeKit
        private static MimeMessage CreateEmail(string from, string to, string subject, string body,string senderName)
        {
            var emailMessage = new MimeMessage();
            // Use the display name and email address for both From and To
            emailMessage.From.Add(new MailboxAddress(senderName, from));
            emailMessage.To.Add(new MailboxAddress(to.ToString(), to));
            emailMessage.Subject = subject;

            emailMessage.Body = new TextPart("plain")
            {
                Text = body
            };


            using (var memoryStream = new MemoryStream())
            {
                emailMessage.WriteTo(memoryStream);
                return emailMessage;
            }
        }

        // Base64Url encoding for the raw email message
        private static string Base64UrlEncode(MimeMessage email)
        {
            var rawMessage = email.ToString();
            var byteArray = System.Text.Encoding.UTF8.GetBytes(rawMessage);
            return Convert.ToBase64String(byteArray)
                .Replace('+', '-')
                .Replace('/', '_')
                .Replace("=", "");
        }
    }
}
