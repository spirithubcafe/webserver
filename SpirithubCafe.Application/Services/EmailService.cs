using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace SpirithubCafe.Application.Services;

/// <summary>
/// Email service interface for sending various types of emails
/// </summary>
public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string htmlBody, string? plainTextBody = null);
    Task SendConfirmationEmailAsync(string to, string confirmationLink);
    Task SendPasswordResetEmailAsync(string to, string resetLink);
    Task SendPasswordResetCodeEmailAsync(string to, string resetCode);
    Task SendWelcomeEmailAsync(string to, string userName);
    Task SendOrderConfirmationEmailAsync(string to, string orderNumber, decimal totalAmount);
    Task SendOrderShippedEmailAsync(string to, string orderNumber, string trackingNumber);
    Task SendNewsletterEmailAsync(string to, string content);
    Task SendContactFormEmailAsync(string senderEmail, string senderName, string subject, string message);
}

/// <summary>
/// SMTP email service implementation
/// </summary>
public class SmtpEmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<SmtpEmailService> _logger;
    private readonly string _smtpServer;
    private readonly int _smtpPort;
    private readonly bool _enableSsl;
    private readonly string _username;
    private readonly string _password;
    private readonly string _fromEmail;
    private readonly string _fromName;

    public SmtpEmailService(IConfiguration configuration, ILogger<SmtpEmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
        
        var emailSettings = configuration.GetSection("EmailSettings");
        _smtpServer = emailSettings["SmtpServer"] ?? throw new InvalidOperationException("SMTP server not configured");
        _smtpPort = int.Parse(emailSettings["SmtpPort"] ?? "465");
        _enableSsl = bool.Parse(emailSettings["EnableSsl"] ?? "true");
        _username = emailSettings["Username"] ?? throw new InvalidOperationException("SMTP username not configured");
        _password = emailSettings["Password"] ?? throw new InvalidOperationException("SMTP password not configured");
        _fromEmail = emailSettings["FromEmail"] ?? _username;
        _fromName = emailSettings["FromName"] ?? "SpirithubCafe";
    }

    public async Task SendEmailAsync(string to, string subject, string htmlBody, string? plainTextBody = null)
    {
        try
        {
            using var client = new SmtpClient(_smtpServer, _smtpPort);
            client.EnableSsl = _enableSsl;
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential(_username, _password);

            using var message = new MailMessage();
            message.From = new MailAddress(_fromEmail, _fromName);
            message.To.Add(to);
            message.Subject = subject;
            message.IsBodyHtml = true;
            message.Body = htmlBody;
            message.BodyEncoding = Encoding.UTF8;
            message.SubjectEncoding = Encoding.UTF8;

            // Add plain text alternative if provided
            if (!string.IsNullOrEmpty(plainTextBody))
            {
                var plainView = AlternateView.CreateAlternateViewFromString(plainTextBody, Encoding.UTF8, "text/plain");
                message.AlternateViews.Add(plainView);
            }

            await client.SendMailAsync(message);
            _logger.LogInformation("Email sent successfully to {To} with subject: {Subject}", to, subject);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {To} with subject: {Subject}", to, subject);
            throw;
        }
    }

    public async Task SendConfirmationEmailAsync(string to, string confirmationLink)
    {
        var subject = "تأكيد البريد الإلكتروني - Confirm Your Email | SpirithubCafe";
        var htmlBody = $@"
<!DOCTYPE html>
<html dir='auto'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Email Confirmation</title>
</head>
<body style='font-family: Arial, sans-serif; margin: 0; padding: 20px; background-color: #f5f5f5;'>
    <div style='max-width: 600px; margin: 0 auto; background-color: white; padding: 30px; border-radius: 10px; box-shadow: 0 2px 10px rgba(0,0,0,0.1);'>
        <div style='text-align: center; margin-bottom: 30px;'>
            <h1 style='color: #8B4513; margin-bottom: 10px;'>SpirithubCafe</h1>
            <div style='height: 3px; background: linear-gradient(90deg, #8B4513, #D2691E); margin: 0 auto; width: 100px;'></div>
        </div>
        
        <div style='text-align: center; margin-bottom: 30px;'>
            <h2 style='color: #333; margin-bottom: 20px;'>Email Confirmation Required</h2>
            <h2 style='color: #333; margin-bottom: 20px; direction: rtl;'>مطلوب تأكيد البريد الإلكتروني</h2>
        </div>
        
        <div style='margin-bottom: 30px; line-height: 1.6;'>
            <p>Please confirm your email address by clicking the button below:</p>
            <p style='direction: rtl;'>يرجى تأكيد عنوان بريدك الإلكتروني بالنقر على الزر أدناه:</p>
        </div>
        
        <div style='text-align: center; margin: 30px 0;'>
            <a href='{confirmationLink}' style='display: inline-block; background-color: #8B4513; color: white; padding: 12px 30px; text-decoration: none; border-radius: 5px; font-weight: bold;'>
                Confirm Email / تأكيد البريد الإلكتروني
            </a>
        </div>
        
        <div style='margin-top: 30px; font-size: 12px; color: #666; text-align: center;'>
            <p>If you didn't request this, please ignore this email.</p>
            <p style='direction: rtl;'>إذا لم تطلب هذا، يرجى تجاهل هذا البريد الإلكتروني.</p>
            <p>© 2024 SpirithubCafe. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";

        var plainTextBody = $@"
SpirithubCafe - Email Confirmation

Please confirm your email address by visiting: {confirmationLink}

If you didn't request this, please ignore this email.

© 2024 SpirithubCafe. All rights reserved.
";

        await SendEmailAsync(to, subject, htmlBody, plainTextBody);
    }

    public async Task SendPasswordResetEmailAsync(string to, string resetLink)
    {
        var subject = "إعادة تعيين كلمة المرور - Password Reset | SpirithubCafe";
        var htmlBody = $@"
<!DOCTYPE html>
<html dir='auto'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Password Reset</title>
</head>
<body style='font-family: Arial, sans-serif; margin: 0; padding: 20px; background-color: #f5f5f5;'>
    <div style='max-width: 600px; margin: 0 auto; background-color: white; padding: 30px; border-radius: 10px; box-shadow: 0 2px 10px rgba(0,0,0,0.1);'>
        <div style='text-align: center; margin-bottom: 30px;'>
            <h1 style='color: #8B4513; margin-bottom: 10px;'>SpirithubCafe</h1>
            <div style='height: 3px; background: linear-gradient(90deg, #8B4513, #D2691E); margin: 0 auto; width: 100px;'></div>
        </div>
        
        <div style='text-align: center; margin-bottom: 30px;'>
            <h2 style='color: #333; margin-bottom: 20px;'>Password Reset Request</h2>
            <h2 style='color: #333; margin-bottom: 20px; direction: rtl;'>طلب إعادة تعيين كلمة المرور</h2>
        </div>
        
        <div style='margin-bottom: 30px; line-height: 1.6;'>
            <p>You have requested to reset your password. Click the button below to reset it:</p>
            <p style='direction: rtl;'>لقد طلبت إعادة تعيين كلمة المرور الخاصة بك. انقر على الزر أدناه لإعادة تعيينها:</p>
        </div>
        
        <div style='text-align: center; margin: 30px 0;'>
            <a href='{resetLink}' style='display: inline-block; background-color: #8B4513; color: white; padding: 12px 30px; text-decoration: none; border-radius: 5px; font-weight: bold;'>
                Reset Password / إعادة تعيين كلمة المرور
            </a>
        </div>
        
        <div style='margin-top: 30px; font-size: 12px; color: #666; text-align: center;'>
            <p>If you didn't request this, please ignore this email or contact support.</p>
            <p style='direction: rtl;'>إذا لم تطلب هذا، يرجى تجاهل هذا البريد الإلكتروني أو الاتصال بالدعم.</p>
            <p>This link will expire in 24 hours for security reasons.</p>
            <p style='direction: rtl;'>ستنتهي صلاحية هذا الرابط خلال 24 ساعة لأسباب أمنية.</p>
            <p>© 2024 SpirithubCafe. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";

        var plainTextBody = $@"
SpirithubCafe - Password Reset

You have requested to reset your password. Visit this link: {resetLink}

If you didn't request this, please ignore this email or contact support.
This link will expire in 24 hours for security reasons.

© 2024 SpirithubCafe. All rights reserved.
";

        await SendEmailAsync(to, subject, htmlBody, plainTextBody);
    }

    public async Task SendPasswordResetCodeEmailAsync(string to, string resetCode)
    {
        var subject = "رمز إعادة تعيين كلمة المرور - Password Reset Code | SpirithubCafe";
        var htmlBody = $@"
<!DOCTYPE html>
<html dir='auto'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Password Reset Code</title>
</head>
<body style='font-family: Arial, sans-serif; margin: 0; padding: 20px; background-color: #f5f5f5;'>
    <div style='max-width: 600px; margin: 0 auto; background-color: white; padding: 30px; border-radius: 10px; box-shadow: 0 2px 10px rgba(0,0,0,0.1);'>
        <div style='text-align: center; margin-bottom: 30px;'>
            <h1 style='color: #8B4513; margin-bottom: 10px;'>SpirithubCafe</h1>
            <div style='height: 3px; background: linear-gradient(90deg, #8B4513, #D2691E); margin: 0 auto; width: 100px;'></div>
        </div>
        
        <div style='text-align: center; margin-bottom: 30px;'>
            <h2 style='color: #333; margin-bottom: 20px;'>Password Reset Code</h2>
            <h2 style='color: #333; margin-bottom: 20px; direction: rtl;'>رمز إعادة تعيين كلمة المرور</h2>
        </div>
        
        <div style='margin-bottom: 30px; line-height: 1.6;'>
            <p>Your password reset code is:</p>
            <p style='direction: rtl;'>رمز إعادة تعيين كلمة المرور الخاص بك هو:</p>
        </div>
        
        <div style='text-align: center; margin: 30px 0;'>
            <div style='display: inline-block; background-color: #f8f9fa; border: 2px dashed #8B4513; padding: 20px 40px; border-radius: 10px; font-size: 24px; font-weight: bold; color: #8B4513; letter-spacing: 3px;'>
                {resetCode}
            </div>
        </div>
        
        <div style='margin-top: 30px; font-size: 12px; color: #666; text-align: center;'>
            <p>Enter this code in the password reset form to continue.</p>
            <p style='direction: rtl;'>أدخل هذا الرمز في نموذج إعادة تعيين كلمة المرور للمتابعة.</p>
            <p>If you didn't request this, please ignore this email or contact support.</p>
            <p style='direction: rtl;'>إذا لم تطلب هذا، يرجى تجاهل هذا البريد الإلكتروني أو الاتصال بالدعم.</p>
            <p>This code will expire in 15 minutes for security reasons.</p>
            <p style='direction: rtl;'>ستنتهي صلاحية هذا الرمز خلال 15 دقيقة لأسباب أمنية.</p>
            <p>© 2024 SpirithubCafe. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";

        var plainTextBody = $@"
SpirithubCafe - Password Reset Code

Your password reset code is: {resetCode}

Enter this code in the password reset form to continue.
If you didn't request this, please ignore this email or contact support.
This code will expire in 15 minutes for security reasons.

© 2024 SpirithubCafe. All rights reserved.
";

        await SendEmailAsync(to, subject, htmlBody, plainTextBody);
    }

    public async Task SendWelcomeEmailAsync(string to, string userName)
    {
        var subject = "أهلاً وسهلاً - Welcome to SpirithubCafe!";
        var htmlBody = $@"
<!DOCTYPE html>
<html dir='auto'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Welcome to SpirithubCafe</title>
</head>
<body style='font-family: Arial, sans-serif; margin: 0; padding: 20px; background-color: #f5f5f5;'>
    <div style='max-width: 600px; margin: 0 auto; background-color: white; padding: 30px; border-radius: 10px; box-shadow: 0 2px 10px rgba(0,0,0,0.1);'>
        <div style='text-align: center; margin-bottom: 30px;'>
            <h1 style='color: #8B4513; margin-bottom: 10px;'>SpirithubCafe</h1>
            <div style='height: 3px; background: linear-gradient(90deg, #8B4513, #D2691E); margin: 0 auto; width: 100px;'></div>
        </div>
        
        <div style='text-align: center; margin-bottom: 30px;'>
            <h2 style='color: #333; margin-bottom: 20px;'>Welcome to Our Coffee Community!</h2>
            <h2 style='color: #333; margin-bottom: 20px; direction: rtl;'>أهلاً وسهلاً بك في مجتمع القهوة لدينا!</h2>
        </div>
        
        <div style='margin-bottom: 30px; line-height: 1.6;'>
            <p>Dear {userName},</p>
            <p>Thank you for joining SpirithubCafe! We're excited to have you as part of our coffee-loving community.</p>
            <p style='direction: rtl;'>عزيزي {userName}،</p>
            <p style='direction: rtl;'>شكراً لك للانضمام إلى SpirithubCafe! نحن متحمسون لوجودك كجزء من مجتمع محبي القهوة لدينا.</p>
        </div>
        
        <div style='background-color: #f8f9fa; padding: 20px; border-radius: 8px; margin: 20px 0;'>
            <h3 style='color: #8B4513; margin-top: 0;'>What's Next? / ما التالي؟</h3>
            <ul style='margin: 0; padding-left: 20px;'>
                <li>Explore our premium coffee collection</li>
                <li>Join our newsletter for exclusive offers</li>
                <li>Follow us on social media</li>
            </ul>
            <ul style='margin: 10px 0 0 0; padding-right: 20px; direction: rtl;'>
                <li>استكشف مجموعة القهوة المميزة لدينا</li>
                <li>اشترك في النشرة الإخبارية للحصول على عروض حصرية</li>
                <li>تابعنا على وسائل التواصل الاجتماعي</li>
            </ul>
        </div>
        
        <div style='text-align: center; margin: 30px 0;'>
            <a href='https://spirithubcafe.com' style='display: inline-block; background-color: #8B4513; color: white; padding: 12px 30px; text-decoration: none; border-radius: 5px; font-weight: bold;'>
                Start Shopping / ابدأ التسوق
            </a>
        </div>
        
        <div style='margin-top: 30px; font-size: 12px; color: #666; text-align: center;'>
            <p>If you have any questions, feel free to contact our support team.</p>
            <p style='direction: rtl;'>إذا كان لديك أي أسئلة، لا تتردد في الاتصال بفريق الدعم لدينا.</p>
            <p>© 2024 SpirithubCafe. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";

        var plainTextBody = $@"
SpirithubCafe - Welcome!

Dear {userName},

Thank you for joining SpirithubCafe! We're excited to have you as part of our coffee-loving community.

What's Next?
- Explore our premium coffee collection
- Join our newsletter for exclusive offers
- Follow us on social media

Visit us at: https://spirithubcafe.com

If you have any questions, feel free to contact our support team.

© 2024 SpirithubCafe. All rights reserved.
";

        await SendEmailAsync(to, subject, htmlBody, plainTextBody);
    }

    public async Task SendOrderConfirmationEmailAsync(string to, string orderNumber, decimal totalAmount)
    {
        var subject = $"تأكيد الطلب - Order Confirmation #{orderNumber} | SpirithubCafe";
        var htmlBody = $@"
<!DOCTYPE html>
<html dir='auto'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Order Confirmation</title>
</head>
<body style='font-family: Arial, sans-serif; margin: 0; padding: 20px; background-color: #f5f5f5;'>
    <div style='max-width: 600px; margin: 0 auto; background-color: white; padding: 30px; border-radius: 10px; box-shadow: 0 2px 10px rgba(0,0,0,0.1);'>
        <div style='text-align: center; margin-bottom: 30px;'>
            <h1 style='color: #8B4513; margin-bottom: 10px;'>SpirithubCafe</h1>
            <div style='height: 3px; background: linear-gradient(90deg, #8B4513, #D2691E); margin: 0 auto; width: 100px;'></div>
        </div>
        
        <div style='text-align: center; margin-bottom: 30px;'>
            <h2 style='color: #333; margin-bottom: 20px;'>Order Confirmation</h2>
            <h2 style='color: #333; margin-bottom: 20px; direction: rtl;'>تأكيد الطلب</h2>
        </div>
        
        <div style='margin-bottom: 30px; line-height: 1.6;'>
            <p>Thank you for your order! Your order has been confirmed and is being processed.</p>
            <p style='direction: rtl;'>شكراً لك على طلبك! تم تأكيد طلبك وهو قيد المعالجة.</p>
        </div>
        
        <div style='background-color: #f8f9fa; padding: 20px; border-radius: 8px; margin: 20px 0;'>
            <h3 style='color: #8B4513; margin-top: 0;'>Order Details / تفاصيل الطلب</h3>
            <p><strong>Order Number / رقم الطلب:</strong> #{orderNumber}</p>
            <p><strong>Total Amount / المبلغ الإجمالي:</strong> {totalAmount:F3} OMR</p>
            <p><strong>Order Date / تاريخ الطلب:</strong> {DateTime.Now:dd/MM/yyyy}</p>
        </div>
        
        <div style='text-align: center; margin: 30px 0;'>
            <a href='https://spirithubcafe.com/orders/{orderNumber}' style='display: inline-block; background-color: #8B4513; color: white; padding: 12px 30px; text-decoration: none; border-radius: 5px; font-weight: bold;'>
                Track Order / تتبع الطلب
            </a>
        </div>
        
        <div style='margin-top: 30px; font-size: 12px; color: #666; text-align: center;'>
            <p>We'll send you another email once your order ships.</p>
            <p style='direction: rtl;'>سنرسل لك بريداً إلكترونياً آخر عندما يتم شحن طلبك.</p>
            <p>© 2024 SpirithubCafe. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";

        var plainTextBody = $@"
SpirithubCafe - Order Confirmation

Thank you for your order! Your order has been confirmed and is being processed.

Order Details:
Order Number: #{orderNumber}
Total Amount: {totalAmount:F3} OMR
Order Date: {DateTime.Now:dd/MM/yyyy}

Track your order at: https://spirithubcafe.com/orders/{orderNumber}

We'll send you another email once your order ships.

© 2024 SpirithubCafe. All rights reserved.
";

        await SendEmailAsync(to, subject, htmlBody, plainTextBody);
    }

    public async Task SendOrderShippedEmailAsync(string to, string orderNumber, string trackingNumber)
    {
        var subject = $"تم شحن الطلب - Order Shipped #{orderNumber} | SpirithubCafe";
        var htmlBody = $@"
<!DOCTYPE html>
<html dir='auto'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Order Shipped</title>
</head>
<body style='font-family: Arial, sans-serif; margin: 0; padding: 20px; background-color: #f5f5f5;'>
    <div style='max-width: 600px; margin: 0 auto; background-color: white; padding: 30px; border-radius: 10px; box-shadow: 0 2px 10px rgba(0,0,0,0.1);'>
        <div style='text-align: center; margin-bottom: 30px;'>
            <h1 style='color: #8B4513; margin-bottom: 10px;'>SpirithubCafe</h1>
            <div style='height: 3px; background: linear-gradient(90deg, #8B4513, #D2691E); margin: 0 auto; width: 100px;'></div>
        </div>
        
        <div style='text-align: center; margin-bottom: 30px;'>
            <h2 style='color: #333; margin-bottom: 20px;'>Your Order Has Shipped!</h2>
            <h2 style='color: #333; margin-bottom: 20px; direction: rtl;'>تم شحن طلبك!</h2>
        </div>
        
        <div style='margin-bottom: 30px; line-height: 1.6;'>
            <p>Great news! Your order is on its way to you.</p>
            <p style='direction: rtl;'>أخبار رائعة! طلبك في طريقه إليك.</p>
        </div>
        
        <div style='background-color: #f8f9fa; padding: 20px; border-radius: 8px; margin: 20px 0;'>
            <h3 style='color: #8B4513; margin-top: 0;'>Shipping Details / تفاصيل الشحن</h3>
            <p><strong>Order Number / رقم الطلب:</strong> #{orderNumber}</p>
            <p><strong>Tracking Number / رقم التتبع:</strong> {trackingNumber}</p>
            <p><strong>Shipped Date / تاريخ الشحن:</strong> {DateTime.Now:dd/MM/yyyy}</p>
        </div>
        
        <div style='text-align: center; margin: 30px 0;'>
            <a href='https://spirithubcafe.com/track/{trackingNumber}' style='display: inline-block; background-color: #8B4513; color: white; padding: 12px 30px; text-decoration: none; border-radius: 5px; font-weight: bold;'>
                Track Package / تتبع الطرد
            </a>
        </div>
        
        <div style='margin-top: 30px; font-size: 12px; color: #666; text-align: center;'>
            <p>Your order should arrive within 2-5 business days.</p>
            <p style='direction: rtl;'>من المفترض أن يصل طلبك خلال 2-5 أيام عمل.</p>
            <p>© 2024 SpirithubCafe. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";

        var plainTextBody = $@"
SpirithubCafe - Order Shipped

Great news! Your order is on its way to you.

Shipping Details:
Order Number: #{orderNumber}
Tracking Number: {trackingNumber}
Shipped Date: {DateTime.Now:dd/MM/yyyy}

Track your package at: https://spirithubcafe.com/track/{trackingNumber}

Your order should arrive within 2-5 business days.

© 2024 SpirithubCafe. All rights reserved.
";

        await SendEmailAsync(to, subject, htmlBody, plainTextBody);
    }

    public async Task SendNewsletterEmailAsync(string to, string content)
    {
        var subject = "SpirithubCafe Newsletter | النشرة الإخبارية";
        var htmlBody = $@"
<!DOCTYPE html>
<html dir='auto'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>SpirithubCafe Newsletter</title>
</head>
<body style='font-family: Arial, sans-serif; margin: 0; padding: 20px; background-color: #f5f5f5;'>
    <div style='max-width: 600px; margin: 0 auto; background-color: white; padding: 30px; border-radius: 10px; box-shadow: 0 2px 10px rgba(0,0,0,0.1);'>
        <div style='text-align: center; margin-bottom: 30px;'>
            <h1 style='color: #8B4513; margin-bottom: 10px;'>SpirithubCafe</h1>
            <div style='height: 3px; background: linear-gradient(90deg, #8B4513, #D2691E); margin: 0 auto; width: 100px;'></div>
        </div>
        
        <div style='margin-bottom: 30px;'>
            {content}
        </div>
        
        <div style='text-align: center; margin: 30px 0;'>
            <a href='https://spirithubcafe.com' style='display: inline-block; background-color: #8B4513; color: white; padding: 12px 30px; text-decoration: none; border-radius: 5px; font-weight: bold;'>
                Visit Our Store / زيارة متجرنا
            </a>
        </div>
        
        <div style='margin-top: 30px; font-size: 12px; color: #666; text-align: center;'>
            <p>You're receiving this email because you subscribed to our newsletter.</p>
            <p style='direction: rtl;'>تتلقى هذا البريد الإلكتروني لأنك اشتركت في نشرتنا الإخبارية.</p>
            <p><a href='#' style='color: #8B4513;'>Unsubscribe / إلغاء الاشتراك</a></p>
            <p>© 2024 SpirithubCafe. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";

        await SendEmailAsync(to, subject, htmlBody);
    }

    public async Task SendContactFormEmailAsync(string senderEmail, string senderName, string subject, string message)
    {
        var adminEmail = _configuration.GetSection("EmailSettings")["ReplyToEmail"] ?? "support@spirithubcafe.com";
        var emailSubject = $"Contact Form Submission - {subject}";
        
        var htmlBody = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <title>Contact Form Submission</title>
</head>
<body style='font-family: Arial, sans-serif; margin: 0; padding: 20px; background-color: #f5f5f5;'>
    <div style='max-width: 600px; margin: 0 auto; background-color: white; padding: 30px; border-radius: 10px; box-shadow: 0 2px 10px rgba(0,0,0,0.1);'>
        <div style='text-align: center; margin-bottom: 30px;'>
            <h1 style='color: #8B4513; margin-bottom: 10px;'>SpirithubCafe</h1>
            <div style='height: 3px; background: linear-gradient(90deg, #8B4513, #D2691E); margin: 0 auto; width: 100px;'></div>
        </div>
        
        <h2 style='color: #333;'>New Contact Form Submission</h2>
        
        <div style='background-color: #f8f9fa; padding: 20px; border-radius: 8px; margin: 20px 0;'>
            <p><strong>From:</strong> {senderName}</p>
            <p><strong>Email:</strong> {senderEmail}</p>
            <p><strong>Subject:</strong> {subject}</p>
        </div>
        
        <div style='margin: 20px 0;'>
            <h3 style='color: #8B4513;'>Message:</h3>
            <div style='background-color: white; border: 1px solid #ddd; padding: 20px; border-radius: 5px;'>
                {message.Replace("\n", "<br>")}
            </div>
        </div>
        
        <div style='margin-top: 30px; font-size: 12px; color: #666; text-align: center;'>
            <p>© 2024 SpirithubCafe. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";

        await SendEmailAsync(adminEmail, emailSubject, htmlBody);
    }
}