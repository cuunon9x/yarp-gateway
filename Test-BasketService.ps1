# Test-BasketService.ps1
# A PowerShell script to test the Basket API through the YARP Gateway

# Configuration
$yarpGatewayUrl = "http://localhost:5000"  # Default YARP development URL
$username = "test_user"                    # Test username for the shopping cart

# Helper function for HTTP requests with error handling
function Invoke-ApiRequest {
    param (
        [string]$Method,
        [string]$Endpoint,
        [object]$Body = $null
    )

    $url = "$yarpGatewayUrl$Endpoint"
    $headers = @{
        "Content-Type" = "application/json"
        "Accept" = "application/json"
    }

    Write-Host "`n======================================"
    Write-Host "$Method $url" -ForegroundColor Blue
    
    if ($Body) {
        Write-Host "Request Body:" -ForegroundColor Gray
        Write-Host ($Body | ConvertTo-Json -Depth 10) -ForegroundColor Gray
    }
    
    try {
        $params = @{
            Method = $Method
            Uri = $url
            Headers = $headers
            ErrorAction = "Stop"
        }
        
        if ($Body -and $Method -ne "GET") {
            $params.Body = ($Body | ConvertTo-Json -Depth 10)
        }
        
        $response = Invoke-RestMethod @params
        
        Write-Host "Response:" -ForegroundColor Green
        Write-Host ($response | ConvertTo-Json -Depth 10) -ForegroundColor Green
        
        return $response
    } catch {
        Write-Host "ERROR: $_" -ForegroundColor Red
        if ($_.ErrorDetails.Message) {
            Write-Host $_.ErrorDetails.Message -ForegroundColor Red
        }
        return $null
    }
}

# 1. Check if YARP Gateway is running
Write-Host "`n🔍 Checking YARP Gateway health..." -ForegroundColor Yellow
$health = Invoke-ApiRequest -Method "GET" -Endpoint "/health"
if (-not $health) {
    Write-Host "⚠️ YARP Gateway is not responding. Please make sure it's running." -ForegroundColor Red
    exit 1
}

# 2. Check if Basket service is accessible via YARP
Write-Host "`n🔍 Checking Basket service health..." -ForegroundColor Yellow
$basketHealth = Invoke-ApiRequest -Method "GET" -Endpoint "/health/basket"
if (-not $basketHealth) {
    Write-Host "⚠️ Basket service is not responding via YARP. Please make sure it's running." -ForegroundColor Red
    exit 1
}

# 3. Create a new shopping cart
Write-Host "`n🛒 Creating a new shopping cart..." -ForegroundColor Yellow
$newCart = @{
    Username = $username
    Items = @(
        @{
            ProductId = "602d2149e773f2a3990b47f5"
            ProductName = "iPhone X"
            Quantity = 1
            Price = 950.00
        }
    )
}

$cart = Invoke-ApiRequest -Method "POST" -Endpoint "/api/basket" -Body $newCart
if (-not $cart) {
    Write-Host "❌ Failed to create shopping cart." -ForegroundColor Red
    exit 1
}

# 4. Get the shopping cart
Write-Host "`n🔍 Retrieving the shopping cart..." -ForegroundColor Yellow
$retrievedCart = Invoke-ApiRequest -Method "GET" -Endpoint "/api/basket/$username"
if (-not $retrievedCart) {
    Write-Host "❌ Failed to retrieve shopping cart." -ForegroundColor Red
    exit 1
}

# 5. Update the shopping cart (add more items)
Write-Host "`n📝 Updating the shopping cart..." -ForegroundColor Yellow
$updatedCart = @{
    Username = $username
    Items = @(
        @{
            ProductId = "602d2149e773f2a3990b47f5"
            ProductName = "iPhone X"
            Quantity = 1
            Price = 950.00
        },
        @{
            ProductId = "602d2149e773f2a3990b47f6"
            ProductName = "Samsung 10" 
            Quantity = 1
            Price = 750.00
        }
    )
}

$cart = Invoke-ApiRequest -Method "POST" -Endpoint "/api/basket" -Body $updatedCart
if (-not $cart) {
    Write-Host "❌ Failed to update shopping cart." -ForegroundColor Red
    exit 1
}

# 6. Checkout the shopping cart
Write-Host "`n💳 Checking out the shopping cart..." -ForegroundColor Yellow
$checkoutRequest = @{
    Username = $username
    TotalPrice = 1700.00
    FirstName = "John"
    LastName = "Doe"
    EmailAddress = "john.doe@example.com"
    AddressLine = "123 Main St"
    Country = "USA"
    State = "NY"
    ZipCode = "10001"
    CardName = "John Doe"
    CardNumber = "1234-5678-9012-3456"
    Expiration = "12/25"
    CVV = "123"
    PaymentMethod = 1
}

$checkout = Invoke-ApiRequest -Method "POST" -Endpoint "/api/basket/checkout" -Body $checkoutRequest
if (-not $checkout) {
    Write-Host "❌ Failed to checkout the shopping cart." -ForegroundColor Red
} else {
    Write-Host "`n✅ Checkout successful!" -ForegroundColor Green
}

# 7. Verify the cart was deleted after checkout
Write-Host "`n🔍 Verifying cart was deleted after checkout..." -ForegroundColor Yellow
$cartAfterCheckout = Invoke-ApiRequest -Method "GET" -Endpoint "/api/basket/$username"
if ($cartAfterCheckout -and $cartAfterCheckout.Username) {
    Write-Host "⚠️ Warning: Cart still exists after checkout." -ForegroundColor Yellow
} else {
    Write-Host "✅ Cart successfully deleted after checkout." -ForegroundColor Green
}

Write-Host "`n🎉 Test completed successfully!" -ForegroundColor Green
