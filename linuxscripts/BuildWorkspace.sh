
echo 'Building projects...'
dotnet build src/BuildingBlocks/Messaging/
#@REM dotnet build src/Services/Discount/Discount.Grpc/
dotnet build src/Services/Catalog/Catalog.Api/
dotnet build src/Services/Basket/Basket.Api/
dotnet build src/Services/Orders/Orders.Api/
dotnet build src/Security/Identity/
dotnet build src/ApiGateways/OcelotApi/
dotnet build src/Frontends/Mvc/Web/
 