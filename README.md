# VISCO OAuth API (C#)

This repository contains:

- `ViscoOAuthApi`: minimal ASP.NET Core Web API (`net8.0`) that uses OAuth-style bearer token validation (JWT bearer auth).
- `ViscoOAuthClient`: sample console app (`net8.0`) that calls the API endpoint with a bearer token.

## API endpoint

- **POST** `/api/xml`
- Requires a valid bearer token.
- Request body:

```json
{
  "key1": "some-string"
}
```

- Response content type: `application/xml`
- Example response:

```xml
<response><key1>some-string</key1><generatedAtUtc>2026-01-01T12:00:00.0000000Z</generatedAtUtc></response>
```

## API OAuth/JWT configuration

Set values in `ViscoOAuthApi/appsettings.json` under `OAuth`:

- `Issuer`
- `Audience`
- `SigningKey`

For production, replace `SigningKey` with a strong secret and load these values from secure configuration (environment variables, secret store, etc).

## Run API locally

```bash
cd ViscoOAuthApi
dotnet restore
dotnet run
```

## Sample client app (calls the API)

The sample client reads values from environment variables:

- `VISCO_API_BASE_URL` (optional, default: `http://localhost:5000/`)
- `VISCO_BEARER_TOKEN` (required)
- `VISCO_KEY1` (optional, default: `sample-key-123`)

Run it:

```bash
cd ViscoOAuthClient
dotnet restore
VISCO_BEARER_TOKEN="<token>" VISCO_KEY1="abc123" dotnet run
```

Expected output (success):

```text
Calling http://localhost:5000/api/xml ...
Success. XML response:
<response><key1>abc123</key1><generatedAtUtc>...</generatedAtUtc></response>
```

## curl example

```bash
curl -X POST http://localhost:5000/api/xml \
  -H "Authorization: Bearer <token>" \
  -H "Content-Type: application/json" \
  -d '{"key1":"abc123"}'
```
