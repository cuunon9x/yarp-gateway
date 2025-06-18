# Testing the YARP Gateway with Basket Service

This guide provides step-by-step instructions for testing the YARP API Gateway with the Basket microservice running.

## Prerequisites

1. The Basket service is running at http://localhost:5002
2. The YARP Gateway is running at http://localhost:5000

## Testing Steps

### Option 1: Using PowerShell Script

We've created a PowerShell script that automates testing of the Basket service through YARP Gateway.

1. Open PowerShell and navigate to the YARP Gateway directory:
   ```powershell
   cd D:\Repositories\Net\Yarp-Gateway
   ```

2. Run the script:
   ```powershell
   .\Test-BasketService.ps1
   ```

3. The script will:
   - Check if YARP Gateway is running
   - Check if Basket service is accessible
   - Create a shopping cart
   - Get the cart
   - Update the cart
   - Checkout the cart
   - Verify the cart was deleted after checkout

### Option 2: Manual Testing with cURL

#### Check YARP Gateway Health

```bash
curl -i http://localhost:5000/health
```

#### Check Basket Service Health via YARP

```bash
curl -i http://localhost:5000/health/basket
```

#### Create a Shopping Cart

```bash
curl -i -X POST http://localhost:5000/api/basket \
  -H "Content-Type: application/json" \
  -d '{
    "Username": "test_user",
    "Items": [
      {
        "ProductId": "602d2149e773f2a3990b47f5",
        "ProductName": "iPhone X",
        "Quantity": 1,
        "Price": 950.00
      }
    ]
  }'
```

#### Get the Shopping Cart

```bash
curl -i http://localhost:5000/api/basket/test_user
```

#### Update the Shopping Cart

```bash
curl -i -X POST http://localhost:5000/api/basket \
  -H "Content-Type: application/json" \
  -d '{
    "Username": "test_user",
    "Items": [
      {
        "ProductId": "602d2149e773f2a3990b47f5",
        "ProductName": "iPhone X",
        "Quantity": 1,
        "Price": 950.00
      },
      {
        "ProductId": "602d2149e773f2a3990b47f6",
        "ProductName": "Samsung 10",
        "Quantity": 1,
        "Price": 750.00
      }
    ]
  }'
```

#### Checkout the Shopping Cart

```bash
curl -i -X POST http://localhost:5000/api/basket/checkout \
  -H "Content-Type: application/json" \
  -d '{
    "Username": "test_user",
    "TotalPrice": 1700.00,
    "FirstName": "John",
    "LastName": "Doe",
    "EmailAddress": "john.doe@example.com",
    "AddressLine": "123 Main St",
    "Country": "USA",
    "State": "NY",
    "ZipCode": "10001",
    "CardName": "John Doe",
    "CardNumber": "1234-5678-9012-3456",
    "Expiration": "12/25",
    "CVV": "123",
    "PaymentMethod": 1
  }'
```

#### Verify the Cart was Deleted

```bash
curl -i http://localhost:5000/api/basket/test_user
```

### Option 3: Using Postman

You can also use Postman to test the YARP Gateway with the Basket service.

1. Import the following Postman collection:

```json
{
  "info": {
    "_postman_id": "c8ccf6c6-3e2f-4e1f-9d7f-5a9f8f9b8d1a",
    "name": "YARP Gateway - Basket Service Tests",
    "schema": "https://schema.getpostman.com/json/collection/v2.1.0/collection.json"
  },
  "item": [
    {
      "name": "Check YARP Health",
      "request": {
        "method": "GET",
        "url": {
          "raw": "http://localhost:5000/health",
          "protocol": "http",
          "host": ["localhost"],
          "port": "5000",
          "path": ["health"]
        }
      }
    },
    {
      "name": "Check Basket Service Health",
      "request": {
        "method": "GET",
        "url": {
          "raw": "http://localhost:5000/health/basket",
          "protocol": "http",
          "host": ["localhost"],
          "port": "5000",
          "path": ["health", "basket"]
        }
      }
    },
    {
      "name": "Create Shopping Cart",
      "request": {
        "method": "POST",
        "header": [
          {
            "key": "Content-Type",
            "value": "application/json"
          }
        ],
        "url": {
          "raw": "http://localhost:5000/api/basket",
          "protocol": "http",
          "host": ["localhost"],
          "port": "5000",
          "path": ["api", "basket"]
        },
        "body": {
          "mode": "raw",
          "raw": "{\n  \"Username\": \"test_user\",\n  \"Items\": [\n    {\n      \"ProductId\": \"602d2149e773f2a3990b47f5\",\n      \"ProductName\": \"iPhone X\",\n      \"Quantity\": 1,\n      \"Price\": 950.00\n    }\n  ]\n}"
        }
      }
    },
    {
      "name": "Get Shopping Cart",
      "request": {
        "method": "GET",
        "url": {
          "raw": "http://localhost:5000/api/basket/test_user",
          "protocol": "http",
          "host": ["localhost"],
          "port": "5000",
          "path": ["api", "basket", "test_user"]
        }
      }
    },
    {
      "name": "Update Shopping Cart",
      "request": {
        "method": "POST",
        "header": [
          {
            "key": "Content-Type",
            "value": "application/json"
          }
        ],
        "url": {
          "raw": "http://localhost:5000/api/basket",
          "protocol": "http",
          "host": ["localhost"],
          "port": "5000",
          "path": ["api", "basket"]
        },
        "body": {
          "mode": "raw",
          "raw": "{\n  \"Username\": \"test_user\",\n  \"Items\": [\n    {\n      \"ProductId\": \"602d2149e773f2a3990b47f5\",\n      \"ProductName\": \"iPhone X\",\n      \"Quantity\": 1,\n      \"Price\": 950.00\n    },\n    {\n      \"ProductId\": \"602d2149e773f2a3990b47f6\",\n      \"ProductName\": \"Samsung 10\",\n      \"Quantity\": 1,\n      \"Price\": 750.00\n    }\n  ]\n}"
        }
      }
    },
    {
      "name": "Checkout Shopping Cart",
      "request": {
        "method": "POST",
        "header": [
          {
            "key": "Content-Type",
            "value": "application/json"
          }
        ],
        "url": {
          "raw": "http://localhost:5000/api/basket/checkout",
          "protocol": "http",
          "host": ["localhost"],
          "port": "5000",
          "path": ["api", "basket", "checkout"]
        },
        "body": {
          "mode": "raw",
          "raw": "{\n  \"Username\": \"test_user\",\n  \"TotalPrice\": 1700.00,\n  \"FirstName\": \"John\",\n  \"LastName\": \"Doe\",\n  \"EmailAddress\": \"john.doe@example.com\",\n  \"AddressLine\": \"123 Main St\",\n  \"Country\": \"USA\",\n  \"State\": \"NY\",\n  \"ZipCode\": \"10001\",\n  \"CardName\": \"John Doe\",\n  \"CardNumber\": \"1234-5678-9012-3456\",\n  \"Expiration\": \"12/25\",\n  \"CVV\": \"123\",\n  \"PaymentMethod\": 1\n}"
        }
      }
    },
    {
      "name": "Verify Cart Deleted",
      "request": {
        "method": "GET",
        "url": {
          "raw": "http://localhost:5000/api/basket/test_user",
          "protocol": "http",
          "host": ["localhost"],
          "port": "5000",
          "path": ["api", "basket", "test_user"]
        }
      }
    }
  ]
}
```

2. Run each request in sequence to test the complete flow.

## Troubleshooting

If you encounter any issues during testing:

1. **YARP Gateway Not Responding**
   - Verify the YARP Gateway is running: `dotnet run --project src/YarpGateway/YarpGateway.csproj`
   - Check the logs for any errors: `logs/yarp-gateway-YYYY-MM-DD.log`

2. **Basket Service Not Accessible via YARP**
   - Verify the Basket service is running at http://localhost:5002
   - Check YARP configuration in `appsettings.Development.json`
   - Ensure the route `basket-route` is correctly configured
   - Try accessing the Basket service directly to confirm it's working

3. **Routing Issues**
   - Check YARP Gateway logs for any routing errors
   - Verify that the paths in your requests match the configured routes

4. **Discount Service Integration**
   - If basket checkout uses the Discount service via gRPC, ensure it's running
   - Check logs for any gRPC-related errors

5. **Message Broker Integration**
   - When testing checkout, verify RabbitMQ is running if it's part of the flow
   - Check if messages are being published correctly

## Expected Responses

1. **Create/Update Shopping Cart**
   ```json
   {
     "username": "test_user",
     "items": [
       {
         "productId": "602d2149e773f2a3990b47f5",
         "productName": "iPhone X",
         "quantity": 1,
         "price": 950.00
       }
     ]
   }
   ```

2. **Checkout**
   - A successful checkout typically returns an order ID or confirmation
   - After checkout, the basket should be deleted

3. **Verify Cart Deleted**
   - Should return a 404 Not Found or an empty response
