# SpirithubCofe API Documentation

## Overview

This is a complete REST API for the SpirithubCofe coffee shop application built with ASP.NET Core 9.0, featuring JWT authentication, full CRUD operations for products and categories, and comprehensive Swagger documentation.

## Features

- ✅ **JWT Authentication** - Token-based authentication for secure API access
- ✅ **User Registration & Login** - Complete user management system
- ✅ **Categories Management** - Full CRUD operations for product categories
- ✅ **Products Management** - Complete product management with search and pagination
- ✅ **Role-based Authorization** - Admin, Staff, and Customer roles
- ✅ **Swagger Documentation** - Interactive API documentation
- ✅ **Clean Architecture** - Properly structured with Application/Domain/Infrastructure layers
- ✅ **Entity Framework Core** - SQLite database with proper relationships
- ✅ **Data Validation** - Comprehensive input validation with proper error handling

## Architecture

```
SpirithubCofe/
├── SpirithubCofe.Domain/          # Domain entities and business logic
├── SpirithubCofe.Application/     # Application services and DTOs
│   ├── DTOs/API/                  # API Data Transfer Objects
│   ├── Services/API/              # API business logic services
│   └── Interfaces/                # Service interfaces
├── SpirithubCofe.Infrastructure/  # Data access and external services
├── SpirithubCofe.Web/            # Web application and API controllers
│   └── Controllers/API/           # REST API controllers
└── SpirithubCofe.Langs/          # Localization resources
```

## Quick Start

### 1. Run the Application

```bash
cd SpirithubCofe.Web
dotnet run
```

The API will be available at: **http://localhost:5212**

### 2. Access Swagger Documentation

Visit: **http://localhost:5212/swagger**

### 3. Test API Endpoints

Use the provided test script:

```bash
./api-test.sh
```

## API Endpoints

### Authentication

#### Register User
```http
POST /api/auth/register
Content-Type: application/json

{
    "email": "user@example.com",
    "password": "Password123!",
    "confirmPassword": "Password123!"
}
```

**Response:**
```json
{
    "success": true,
    "message": "User registered successfully",
    "data": {
        "userId": "user-guid-here",
        "email": "user@example.com"
    }
}
```

#### Login User
```http
POST /api/auth/login
Content-Type: application/json

{
    "email": "user@example.com",
    "password": "Password123!"
}
```

**Response:**
```json
{
    "success": true,
    "message": "Login successful",
    "data": {
        "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
        "expiry": "2024-01-01T12:00:00Z",
        "userId": "user-guid-here",
        "email": "user@example.com",
        "roles": ["Customer"]
    }
}
```

### Categories

All category endpoints require authentication. Admin role required for CUD operations.

#### Get All Categories
```http
GET /api/categories
Authorization: Bearer {your-jwt-token}
```

#### Get Category by ID
```http
GET /api/categories/{id}
Authorization: Bearer {your-jwt-token}
```

#### Create Category (Admin only)
```http
POST /api/categories
Authorization: Bearer {your-jwt-token}
Content-Type: application/json

{
    "name": "Coffee Beans",
    "nameAr": "حبوب القهوة",
    "description": "Premium coffee beans",
    "descriptionAr": "حبوب قهوة فاخرة",
    "slug": "coffee-beans",
    "displayOrder": 1,
    "isActive": true,
    "isDisplayedOnHomepage": true
}
```

#### Update Category (Admin only)
```http
PUT /api/categories/{id}
Authorization: Bearer {your-jwt-token}
Content-Type: application/json

{
    "id": 1,
    "name": "Updated Coffee Beans",
    "nameAr": "حبوب القهوة المحدثة",
    // ... other fields
}
```

#### Delete Category (Admin only)
```http
DELETE /api/categories/{id}
Authorization: Bearer {your-jwt-token}
```

### Products

All product endpoints require authentication. Admin role required for CUD operations.

#### Get All Products (with search and pagination)
```http
GET /api/products?query=coffee&featuredOnly=false&page=1&pageSize=10
Authorization: Bearer {your-jwt-token}
```

**Query Parameters:**
- `query` (optional): Search term for product name
- `featuredOnly` (optional): Filter featured products only
- `page` (optional): Page number (default: 1)
- `pageSize` (optional): Items per page (default: 10)

#### Get Product by ID
```http
GET /api/products/{id}
Authorization: Bearer {your-jwt-token}
```

#### Create Product (Admin only)
```http
POST /api/products
Authorization: Bearer {your-jwt-token}
Content-Type: application/json

{
    "name": "Ethiopian Coffee",
    "nameAr": "قهوة إثيوبية",
    "description": "Premium Ethiopian coffee beans",
    "descriptionAr": "حبوب قهوة إثيوبية فاخرة",
    "sku": "ETH-001",
    "categoryId": 1,
    "minPrice": 15.99,
    "maxPrice": 25.99,
    "imagePath": "/images/ethiopian-coffee.jpg",
    "isActive": true,
    "isFeatured": false
}
```

#### Update Product (Admin only)
```http
PUT /api/products/{id}
Authorization: Bearer {your-jwt-token}
Content-Type: application/json

{
    "id": 1,
    "name": "Updated Ethiopian Coffee",
    "nameAr": "قهوة إثيوبية محدثة",
    // ... other fields
}
```

#### Delete Product (Admin only)
```http
DELETE /api/products/{id}
Authorization: Bearer {your-jwt-token}
```

## Authentication & Authorization

### JWT Token Usage

After successful login, include the JWT token in the Authorization header:

```http
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### User Roles

- **Customer**: Can view categories and products
- **Staff**: Can view and manage basic data
- **Admin**: Full access to all endpoints

### Token Configuration

JWT tokens are configured in `appsettings.json`:

```json
{
    "JwtSettings": {
        "SecretKey": "YourVeryLongSecretKeyThatIsAtLeast32CharactersLongForSpirithubCofeAPI!",
        "Issuer": "SpirithubCofe",
        "Audience": "SpirithubCofe",
        "ExpiryInHours": 1
    }
}
```

## Response Format

All API responses follow a consistent format:

### Success Response
```json
{
    "success": true,
    "message": "Operation completed successfully",
    "data": {
        // Response data here
    }
}
```

### Error Response
```json
{
    "success": false,
    "message": "Error description",
    "errors": [
        "Validation error 1",
        "Validation error 2"
    ]
}
```

### Paginated Response
```json
{
    "success": true,
    "data": [...],
    "totalItems": 50,
    "page": 1,
    "pageSize": 10,
    "totalPages": 5
}
```

## Data Models

### Category DTO
```csharp
public class CategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? NameAr { get; set; }
    public string? Description { get; set; }
    public string? DescriptionAr { get; set; }
    public string Slug { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    public bool IsDisplayedOnHomepage { get; set; }
    public string? ImagePath { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

### Product DTO
```csharp
public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? NameAr { get; set; }
    public string? Description { get; set; }
    public string? DescriptionAr { get; set; }
    public string Sku { get; set; }
    public int CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public decimal MinPrice { get; set; }
    public decimal MaxPrice { get; set; }
    public string? ImagePath { get; set; }
    public bool IsActive { get; set; }
    public bool IsFeatured { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

## Error Handling

The API provides comprehensive error handling:

- **400 Bad Request**: Invalid input data
- **401 Unauthorized**: Missing or invalid JWT token
- **403 Forbidden**: Insufficient permissions
- **404 Not Found**: Resource not found
- **409 Conflict**: Data conflicts (e.g., duplicate email)
- **500 Internal Server Error**: Server-side errors

## Testing

### Using Swagger UI

1. Start the application: `dotnet run`
2. Open **http://localhost:5212/swagger**
3. Click "Authorize" and enter your JWT token
4. Test endpoints interactively

### Using curl

```bash
# Register a new user
curl -X POST "http://localhost:5212/api/auth/register" \
  -H "Content-Type: application/json" \
  -d '{"email": "test@example.com", "password": "Test123!", "confirmPassword": "Test123!"}'

# Login to get JWT token
curl -X POST "http://localhost:5212/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{"email": "test@example.com", "password": "Test123!"}'

# Use the token to access protected endpoints
curl -X GET "http://localhost:5212/api/categories" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN_HERE"
```

### Using Postman

1. Import the API endpoints from Swagger JSON: `/swagger/v1/swagger.json`
2. Set up environment variables for base URL and JWT token
3. Test all endpoints with proper authentication

## Development

### Adding New Endpoints

1. Create DTO classes in `SpirithubCofe.Application/DTOs/API/`
2. Implement service logic in `SpirithubCofe.Application/Services/API/`
3. Create controller in `SpirithubCofe.Web/Controllers/API/`
4. Register services in `Program.cs`

### Database Migrations

```bash
dotnet ef migrations add YourMigrationName -p SpirithubCofe.Infrastructure -s SpirithubCofe.Web
dotnet ef database update -p SpirithubCofe.Infrastructure -s SpirithubCofe.Web
```

## Security Considerations

- JWT tokens expire after 1 hour (configurable)
- Passwords are hashed using ASP.NET Core Identity
- Role-based authorization protects admin endpoints
- HTTPS should be enabled in production
- API rate limiting should be implemented for production use

## Deployment

### Development
```bash
dotnet run --project SpirithubCofe.Web
```

### Production
```bash
dotnet publish -c Release -o ./publish
# Configure production settings in appsettings.Production.json
# Deploy to your hosting platform
```

## Support

For issues and questions:
- Check the Swagger documentation at `/swagger`
- Review the API test script: `./api-test.sh`
- Examine the source code in the Controllers and Services folders

---

**API Status: ✅ Fully Functional**

The API is complete with JWT authentication, full CRUD operations, proper error handling, and comprehensive Swagger documentation.