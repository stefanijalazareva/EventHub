# Email Configuration Guide

## How to Set Up Email Notifications

EventHub now sends confirmation emails when users book tickets. Follow these steps to configure email sending:

### Option 1: Using Gmail (Recommended for Testing)

#### 1. Enable 2-Factor Authentication on Your Gmail Account
- Go to https://myaccount.google.com/security
- Enable 2-Step Verification

#### 2. Generate an App Password
- Go to https://myaccount.google.com/apppasswords
- Select "Mail" and "Windows Computer" (or Other)
- Click "Generate"
- Copy the 16-character password (e.g., `abcd efgh ijkl mnop`)

#### 3. Update Configuration

**In `appsettings.Development.json`:**
```json
{
  "Email": {
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": "587",
    "SenderEmail": "your-email@gmail.com",
    "SenderPassword": "your-16-char-app-password",
    "SenderName": "EventHub"
  }
}
```

**Important:** Replace:
- `your-email@gmail.com` with your actual Gmail address
- `your-16-char-app-password` with the app password from step 2 (remove spaces)

### Option 2: Using Other Email Providers

#### Outlook/Hotmail
```json
{
  "Email": {
    "SmtpHost": "smtp-mail.outlook.com",
    "SmtpPort": "587",
    "SenderEmail": "your-email@outlook.com",
    "SenderPassword": "your-password",
    "SenderName": "EventHub"
  }
}
```

#### Office 365
```json
{
  "Email": {
    "SmtpHost": "smtp.office365.com",
    "SmtpPort": "587",
    "SenderEmail": "your-email@yourdomain.com",
    "SenderPassword": "your-password",
    "SenderName": "EventHub"
  }
}
```

### Testing Email Functionality

1. **Start the application:**
   ```bash
   cd "src/EventHub.Web"
   dotnet run
   ```

2. **Register a new user** using a real email address

3. **Book a ticket** for any event

4. **Check your email** for the confirmation

### Email Template

The confirmation email includes:
- Event name
- Number of tickets
- Total price
- Unique ticket numbers
- Beautiful HTML formatting with your color scheme

### Troubleshooting

**No email received?**
- Check your spam/junk folder
- Verify SMTP credentials are correct
- Make sure you're using an App Password (not your regular Gmail password)
- Check that 2FA is enabled on Gmail

**"Authentication failed" error?**
- You're probably using your regular password instead of an App Password
- Generate a new App Password and try again

**Email not configured?**
- The app will continue to work even if email is not configured
- It will skip email sending silently without breaking the booking process

### Security Note

⚠️ **Never commit email passwords to GitHub!**

The email configuration should be:
- ✅ In `appsettings.Development.json` (local development only)
- ✅ In environment variables (production)
- ❌ Never in `appsettings.json` if you plan to commit it

Consider using User Secrets for local development:
```bash
dotnet user-secrets set "Email:SenderEmail" "your-email@gmail.com"
dotnet user-secrets set "Email:SenderPassword" "your-app-password"
```
