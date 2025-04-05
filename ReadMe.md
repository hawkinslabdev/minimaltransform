# 🌐 Minimal Transform

Effortlessly convert between data formats with a developer-friendly web app and API.

![Screenshot of Swagger UI](https://raw.githubusercontent.com/hawkinslabdev/minimaltransform/refs/heads/main/Source/image.png);


## 💡 Features
- Instant format conversion between JSON, XML, YAML, CSV
- Secure, restricted API endpoints
- Auto-format detection
- Indentation control
- Web interface with copy/download functionality

## ✨ Supported Formats
- JSON
- XML
- YAML
- CSV

## 📦 Requirements
- .NET 8.0 SDK
- Web server or local development environment

## 🛠️ Setup

### Environment Configuration
Create a `.env` file in the project root to configure security and access:

```bash
# Allowed Origins (comma-separated)
ALLOWED_ORIGINS=https://example.com,http://localhost:3000

# API Keys (comma-separated)
API_KEYS=your-secret-key-1,your-secret-key-2
```

#### Configuration Options
- `ALLOWED_ORIGINS`: Domains allowed to access the API
- `API_KEYS`: Secret keys for API access
- Leave empty for local development (not recommended for production)

## 🚀 Running the Application

### Web Interface
Navigate to `http://localhost:5111/convert` in your browser.

### API Conversion Endpoints
Convert formats using simple POST requests:

#### JSON to XML
```bash
curl -X POST http://localhost:5111/api/convert/json-to-xml \
     -H "Content-Type: application/json" \
     -d '{"name": "John Doe"}'
```

#### Supported Conversions
- `/api/convert/json-to-xml`
- `/api/convert/json-to-yaml`
- `/api/convert/json-to-csv`
- `/api/convert/xml-to-json`
- `/api/convert/xml-to-yaml`
- `/api/convert/xml-to-csv`
- (and more!)

## 🔧 Auto-Detection Conversion
Use the auto-convert endpoint to automatically detect input format:

```bash
curl -X POST "http://localhost:5111/api/convert/auto?targetFormat=xml" \
     -H "Content-Type: application/json" \
     -d '{"name": "John Doe"}'
```

## 🛡️ Security Features

### Cross-Origin Resource Sharing (CORS)
- Strict CORS policy prevents unauthorized API usage
- Only whitelisted origins can access the API
- Configurable through `.env` file

### API Key Protection
- Mandatory API key for non-same-origin requests
- Prevent unauthorized access and potential abuse
- Configure API keys in `.env`
