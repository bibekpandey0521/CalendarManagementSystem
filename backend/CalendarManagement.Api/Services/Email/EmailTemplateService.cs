namespace CalendarManagement.Api.Services.Email;

public interface IEmailTemplateService
{
    string GeneratePasswordResetEmail(string recipientEmail, string resetUrl);
}

public sealed class EmailTemplateService : IEmailTemplateService
{
    public string GeneratePasswordResetEmail(string recipientEmail, string resetUrl)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Password Reset Request</title>
    <style>
        body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background-color: #f4f4f4; margin: 0; padding: 0; }}
        .container {{ max-width: 600px; margin: 40px auto; background-color: #ffffff; border-radius: 8px; box-shadow: 0 2px 4px rgba(0,0,0,0.1); overflow: hidden; }}
        .header {{ background-color: #2563eb; color: #ffffff; padding: 30px 40px; text-align: center; }}
        .header h1 {{ margin: 0; font-size: 24px; font-weight: 600; }}
        .content {{ padding: 40px; }}
        .content p {{ color: #333333; line-height: 1.6; margin: 0 0 16px; }}
        .button {{ display: inline-block; background-color: #2563eb; color: #ffffff; padding: 12px 24px; border-radius: 6px; text-decoration: none; font-weight: 600; margin: 20px 0; }}
        .button:hover {{ background-color: #1d4ed8; }}
        .footer {{ background-color: #f9fafb; padding: 20px 40px; text-align: center; font-size: 12px; color: #6b7280; }}
        .warning {{ color: #dc2626; font-size: 12px; margin-top: 20px; }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>Calendar Management System</h1>
        </div>
        <div class=""content"">
            <p>Hello,</p>
            <p>We received a request to reset your password for your Calendar Management System account.</p>
            <p>To reset your password, click the button below:</p>
            <p style=""text-align: center;"">
                <a href=""{resetUrl}"" class=""button"">Reset Password</a>
            </p>
            <p>Or copy and paste this link into your browser:</p>
            <p style=""word-break: break-all; color: #2563eb;"">{resetUrl}</p>
            <p><strong>This link will expire in 30 minutes.</strong></p>
            <p>If you did not request a password reset, please ignore this email. Your password will remain unchanged.</p>
            <p class=""warning"">For security reasons, never share this link with anyone.</p>
        </div>
        <div class=""footer"">
            <p>&copy; {DateTime.UtcNow.Year} Calendar Management System. All rights reserved.</p>
            <p>This is an automated message. Please do not reply to this email.</p>
        </div>
    </div>
</body>
</html>";
    }
}
