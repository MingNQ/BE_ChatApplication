# Email Templates - Variable Binding Guide

## Overview
Email templates now support dynamic data binding from `WebSetting` entity, allowing you to customize email content without hardcoding values.

## Available Variables

### Website Information (from WebSetting)
- `{WEBSITE_NAME}` - Website name from `WebSetting.Name`
- `{LOGO_URL}` - Logo URL from `WebSetting.Logo.FullPathUrl`
- `{CONTACT_EMAIL}` - Contact email from `WebSetting.Email`
- `{ADDRESS}` - Address from `WebSetting.Address`
- `{PHONE_NUMBER}` - Phone number from `WebSetting.PhoneNumber`
- `{COPYRIGHT}` - Copyright text from `WebSetting.Copyright`

### Website URLs
- `{WEBSITE_URL}` - Main website URL from `ApplicationInfo.Site`

### Social Media Links (from WebSetting)
- `{FACEBOOK_URL}` - Facebook URL from `WebSetting.Facebook`
- `{INSTAGRAM_URL}` - Instagram URL from `WebSetting.Instagram`
- `{YOUTUBE_URL}` - YouTube URL from `WebSetting.Youtube`
- `{TIKTOK_URL}` - TikTok URL from `WebSetting.Tiktok`
- `{LINKEDIN_URL}` - LinkedIn URL from `WebSetting.Linkedin`
- `{TWITTER_URL}` - Twitter URL from `WebSetting.Twitter`

### User-Specific Variables
- `{USER}` - User name
- `{EMAIL}` - User email address
- `{LINK}` - Verification/action link
- `{CODE}` - Verification code

### System Variables
- `{THIS_YEAR}` - Current year

## Fallback Behavior
If `WebSetting` data is not available, the system will fallback to `ApplicationInfoConfiguration` values:
- `{WEBSITE_NAME}` → `ApplicationInfoConfiguration.Name`
- `{CONTACT_EMAIL}` → `ApplicationInfoConfiguration.Email`
- `{ADDRESS}` → `ApplicationInfoConfiguration.Address`
- `{PHONE_NUMBER}` → `ApplicationInfoConfiguration.Phone`

## Usage Example

### In HTML Template:
```html
<h1>Welcome to {WEBSITE_NAME}</h1>
<img src="{LOGO_URL}" alt="Logo {WEBSITE_NAME}" />
<p>Contact us at: {CONTACT_EMAIL}</p>
<p>Address: {ADDRESS}</p>
<p>Phone: {PHONE_NUMBER}</p>

<div class="social-links">
    <a href="{FACEBOOK_URL}">Facebook</a>
    <a href="{INSTAGRAM_URL}">Instagram</a>
    <a href="{YOUTUBE_URL}">YouTube</a>
</div>

<footer>{COPYRIGHT}</footer>
```

### In Service:
```csharp
// Use async method for better performance with WebSetting data binding
string emailTemplate = await emailTemplateProvider.GetTemplateByNameAsync("currentemail-link", "vi");
```

## File Structure
- `vi/` - Vietnamese templates
- `en/` - English templates

## Template Files
All Vietnamese templates in `vi/` folder have been updated with variable binding support:
- ✅ `currentemail-link.html` - Email verification with link (Updated with WebSetting variables)
- ✅ `currentemail.html` - Email verification with code (Updated with WebSetting variables)
- ✅ `resetpassword.html` - Password reset (Updated with logo variable)
- ✅ `changepwdsuccess.html` - Password change success (Updated with logo and social links)
- ✅ `loginemailcode.html` - Login verification code (Updated with header comments)
- ✅ `verifyemailnewuser.html` - New user verification (Updated with header comments)

English templates in `en/` folder:
- `currentemail-link.html` - Email verification with link
- `currentemail.html` - Email verification with code
- `resetpassword.html` - Password reset
- `changepwdsuccess.html` - Password change success
- `loginemailcode.html` - Login verification code
- `verifyemailnewuser.html` - New user verification

## Notes
- All variables are replaced during template processing
- Missing WebSetting data will use ApplicationInfo fallbacks
- Empty social media URLs will result in empty strings
- Logo URL uses the full path from FileStorage service
