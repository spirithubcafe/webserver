#!/bin/bash

echo "=== SpirithubCofe API Test Script ==="
echo ""

# Base URL
BASE_URL="http://localhost:5212"

echo "1. Testing Register Endpoint:"
echo "POST $BASE_URL/api/auth/register"
echo ""

echo "2. Testing Login Endpoint:"
echo "POST $BASE_URL/api/auth/login"
echo ""

echo "3. Testing Categories Endpoint (requires authentication):"
echo "GET $BASE_URL/api/categories"
echo ""

echo "4. Testing Products Endpoint (requires authentication):"
echo "GET $BASE_URL/api/products"
echo ""

echo "5. Testing Swagger Documentation:"
echo "GET $BASE_URL/swagger"
echo ""

echo "=== API Endpoints Summary ==="
echo ""
echo "Authentication:"
echo "  POST /api/auth/register - Register new user"
echo "  POST /api/auth/login - Login user and get JWT token"
echo ""
echo "Categories:"
echo "  GET /api/categories - List all active categories"
echo "  GET /api/categories/{id} - Get category by ID"
echo "  POST /api/categories - Create new category (Admin only)"
echo "  PUT /api/categories/{id} - Update category (Admin only)"
echo "  DELETE /api/categories/{id} - Delete category (Admin only)"
echo ""
echo "Products:"
echo "  GET /api/products - List products with search and pagination"
echo "  GET /api/products/{id} - Get product by ID"
echo "  POST /api/products - Create new product (Admin only)"
echo "  PUT /api/products/{id} - Update product (Admin only)"
echo "  DELETE /api/products/{id} - Delete product (Admin only)"
echo ""
echo "Usage with curl:"
echo ""
echo "# Register user"
echo 'curl -X POST "$BASE_URL/api/auth/register" \'
echo ' -H "Content-Type: application/json" \'
echo ' -d '\''{"email": "user@example.com", "password": "Password123!", "confirmPassword": "Password123!"}'\'
echo ""
echo "# Login user"
echo 'curl -X POST "$BASE_URL/api/auth/login" \'
echo ' -H "Content-Type: application/json" \'
echo ' -d '\''{"email": "user@example.com", "password": "Password123!"}'\'
echo ""
echo "# Use JWT token for authenticated requests"
echo 'curl -X GET "$BASE_URL/api/categories" \'
echo ' -H "Authorization: Bearer YOUR_JWT_TOKEN_HERE"'
echo ""
echo "=== Documentation ==="
echo "Visit http://localhost:5212/swagger for interactive API documentation"
echo ""