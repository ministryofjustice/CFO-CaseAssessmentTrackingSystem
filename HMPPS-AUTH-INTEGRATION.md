# HMPPS Auth Integration Guide

This document explains how to integrate HMPPS Auth (Her Majesty's Prison and Probation Service Authentication) alongside the existing Microsoft Entra ID (Azure AD) authentication.

## Overview

The application now supports three authentication methods:
1. **Local Authentication** - Username/password stored in the application database
2. **Microsoft Entra ID** - Enterprise authentication via Azure AD/Microsoft 365
3. **HMPPS Auth** - Government authentication service using OAuth 2.0 Authorization Code flow

## Configuration

### 1. Update appsettings.json

Add your HMPPS Auth credentials to `appsettings.json`:

```json
"HmppsAuth": {
  "Instance": "https://sign-in-dev.hmpps.service.justice.gov.uk",
  "ClientId": "your-client-id-here",
  "ClientSecret": "your-client-secret-here",
  "CallbackPath": "/signin-hmpps",
  "AuthorizationEndpoint": "https://sign-in-dev.hmpps.service.justice.gov.uk/auth/oauth/authorize",
  "TokenEndpoint": "https://sign-in-dev.hmpps.service.justice.gov.uk/auth/oauth/token",
  "UserInformationEndpoint": "https://sign-in-dev.hmpps.service.justice.gov.uk/auth/api/user/me"
}
```

### 2. Environment-Specific Configuration

For production, update the URLs to use the production HMPPS Auth instance:

```json
"HmppsAuth": {
  "Instance": "https://sign-in.hmpps.service.justice.gov.uk",
  "AuthorizationEndpoint": "https://sign-in.hmpps.service.justice.gov.uk/auth/oauth/authorize",
  "TokenEndpoint": "https://sign-in.hmpps.service.justice.gov.uk/auth/oauth/token",
  "UserInformationEndpoint": "https://sign-in.hmpps.service.justice.gov.uk/auth/api/user/me"
}
```

### 3. Register Redirect URI

Register the following redirect URI with HMPPS Auth:
- Development: `http://localhost:7204/signin-hmpps`
- Production: `https://your-domain.com/signin-hmpps`

## How It Works

### Authorization Code Flow

1. **User clicks "Sign in with HMPPS"** on the login page
2. **Redirect to HMPPS Auth** - User is redirected to HMPPS Auth login page with:
   - `response_type=code`
   - `state` (random string for CSRF protection)
   - `client_id`
   - `redirect_uri`

3. **User authenticates** - User enters their HMPPS credentials

4. **Callback with authorization code** - HMPPS Auth redirects back with:
   - 6-digit authorization code
   - Original state value

5. **Token exchange** - Application exchanges the code for tokens by calling the token endpoint with:
   - Authorization header containing Base64-encoded `clientId:clientSecret`
   - Authorization code
   - Grant type: `authorization_code`

6. **User info retrieval** - Application fetches user details from the user information endpoint

7. **Local account linking** - Application links the HMPPS login to an existing user account by email

8. **Session creation** - User is signed in with a local session cookie

## User Experience

### Login Page

Users will see three sign-in options:
1. Local username/password form
2. "Sign in with Microsoft" button (with Microsoft logo)
3. "Sign in with HMPPS" button

### First-Time HMPPS Users

When a user signs in with HMPPS for the first time:
- The system looks up the user by email address from their HMPPS profile
- If a matching user exists, the HMPPS login is linked to that account
- If no matching user exists, the login fails with "Invalid login attempt"
- Users must have an existing account in the system before using HMPPS Auth

### Returning HMPPS Users

Once linked, users can seamlessly sign in using HMPPS Auth without re-linking.

## Security Considerations

1. **State Parameter** - Protects against CSRF attacks during OAuth flow
2. **Client Secret** - Keep this secure; never commit to source control
3. **HTTPS Required** - Always use HTTPS in production
4. **Token Storage** - Access tokens are saved securely in the authentication cookie
5. **Email Verification** - Users are matched by email address from their HMPPS profile

## Troubleshooting

### Common Issues

**"External login failed" error**
- Verify ClientId and ClientSecret are correct
- Check that the redirect URI is registered with HMPPS Auth
- Ensure the callback path matches configuration

**"Invalid login attempt" error**
- User's email from HMPPS must match an existing user account
- User account must be confirmed and not locked

**Missing email claim**
- HMPPS Auth must return an email in the user info response
- Check the UserInformationEndpoint is configured correctly

## Code Structure

### Key Files

- **`DependencyInjection.cs`** - OAuth handler registration and configuration
- **`HmppsAuthSettings.cs`** - Configuration model for HMPPS Auth settings
- **`ExternalLogin.cs`** - Handles OAuth callback and account linking
- **`Login.razor`** - Login page with HMPPS sign-in button
- **`appsettings.json`** - Configuration values

### OAuth Handler

The HMPPS Auth integration uses ASP.NET Core's built-in OAuth authentication handler configured with:
- Authorization endpoint
- Token endpoint  
- User information endpoint
- Claim mappings (username → NameIdentifier, name, email)
- Bearer token authentication for user info requests

## Testing

### Local Development

1. Obtain test credentials from HMPPS Auth team
2. Update `appsettings.Development.json` with dev credentials
3. Ensure redirect URI includes your local port
4. Test the full flow from login to callback

### Integration Testing

Create test users in HMPPS Auth dev environment that match existing users in your database by email address.

## Support

For HMPPS Auth specific issues, contact the HMPPS Digital team or refer to their documentation at:
- Internal HMPPS Auth documentation
- HMPPS Digital Studio

For application integration issues, refer to the development team.
