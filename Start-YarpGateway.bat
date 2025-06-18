@echo off
echo Starting YARP API Gateway for testing...
echo.
echo This will start the YARP Gateway on http://localhost:5000
echo.
echo Make sure your Basket service is running on http://localhost:5002
echo.

cd src\YarpGateway
dotnet run --launch-profile YarpGateway
