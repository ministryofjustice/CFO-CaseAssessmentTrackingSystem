# HMPPS Auth Integration - Quick Setup Guide

## Summary of Changes

HMPPS Auth has been integrated as an additional authentication provider alongside Microsoft Entra ID. Users can now sign in using:
1. Local username/password
2. Microsoft Entra ID
3. HMPPS Auth (OAuth 2.0)

## Files Modified

1. **`src/Infrastructure/DependencyInjection.cs`**
   - Added OAuth authentication handler for HMPPS Auth
   - Configured Authorization Code flow with token exchange
   - Added claim mapping for username, name, and email

2. **`src/Server.UI/appsettings.json`**
   - Added HmppsAuth configuration section
   - Updated Content-Security-Policy to allow HMPPS Auth domain

3. **`src/Server.UI/Pages/Identity/Authentication/Login.razor`**
   - Added "Sign in with HMPPS" button
   - Added styling for HMPPS button

## Files Created

1. **`src/Infrastructure/Configurations/HmppsAuthSettings.cs`**
   - Configuration model for HMPPS Auth settings

2. **`HMPPS-AUTH-INTEGRATION.md`**
   - Comprehensive integration documentation

## Quick Start

### 1. Configure HMPPS Auth Credentials

Update your `appsettings.json` or environment-specific config:

```json
"HmppsAuth": {
  "ClientId": "your-client-id",
  "ClientSecret": "your-client-secret"
}
```

### 2. Register Redirect URI

Register these redirect URIs with HMPPS Auth:
- **Development:** `http://localhost:7204/signin-hmpps`
- **Production:** `https://your-domain/signin-hmpps`

### 3. Environment-Specific URLs

**Development (already configured):**
```json
"Instance": "https://sign-in-dev.hmpps.service.justice.gov.uk"
```

**Production:**
```json
"Instance": "https://sign-in.hmpps.service.justice.gov.uk",
"AuthorizationEndpoint": "https://sign-in.hmpps.service.justice.gov.uk/auth/oauth/authorize",
"TokenEndpoint": "https://sign-in.hmpps.service.justice.gov.uk/auth/oauth/token",
"UserInformationEndpoint": "https://sign-in.hmpps.service.justice.gov.uk/auth/api/user/me"
```

## How Authentication Works

### Flow Diagram

```
User → Login Page → Clicks "Sign in with HMPPS"
  ↓
HMPPS Auth (User authenticates)
  ↓
Callback with authorization code
  ↓
Exchange code for access token
  ↓
Retrieve user info (username, name, email)
  ↓
Match user by email to existing account
  ↓
Link external login to local account
  ↓
Sign in complete
```

### Important Notes

- **Email matching is required:** HMPPS Auth users must have an email that matches an existing user in your database
- **No auto-registration:** Users must be pre-created in the system before they can sign in with HMPPS Auth
- **First-time linking:** On first HMPPS Auth sign-in, the external login is automatically linked to the user's account
- **Subsequent logins:** After linking, users can seamlessly sign in using HMPPS Auth

## Testing Locally

1. **Start the application:**
   ```bash
   dotnet run --project src/Server.UI
   ```

2. **Navigate to login page:**
   ```
   https://localhost:7204/pages/authentication/login
   ```

3. **Click "Sign in with HMPPS"**

4. **You'll be redirected to HMPPS Auth dev environment**

5. **After authentication, you'll return to your application**

## Security Configuration

The following security measures are in place:

- **State parameter:** CSRF protection during OAuth flow
- **HTTPS enforcement:** Required in production
- **Secure cookies:** Authentication cookies are secure in production
- **External scheme:** External logins use a separate cookie scheme
- **Token validation:** Access tokens are validated by HMPPS Auth

## Troubleshooting

### "HmppsAuth:ClientId is required" error
- Ensure `ClientId` is set in appsettings.json

### "External login failed"
- Check that redirect URI is registered with HMPPS Auth
- Verify ClientSecret is correct
- Check HMPPS Auth service status

### "Invalid login attempt" after HMPPS authentication
- Ensure user exists in database with matching email
- Verify email is returned from HMPPS Auth user info endpoint
- Check that user account is confirmed and not locked

## Next Steps

1. Obtain production credentials from HMPPS Auth team
2. Update production configuration
3. Test with real HMPPS Auth accounts
4. Train users on multiple authentication options
5. Monitor authentication logs for issues

## Support Contacts

- **HMPPS Auth Service:** Contact HMPPS Digital team
- **Application Issues:** Contact development team

---

For detailed technical information, see `HMPPS-AUTH-INTEGRATION.md`
