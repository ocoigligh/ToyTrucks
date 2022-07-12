FROM mcr.microsoft.com/dotnet/sdk:5.0 AS debug
ENV ASPNETCORE_URLS=http://+:80

RUN apt-get update
RUN apt-get install -y unzip
RUN apt-get install -y curl
# dls a sh script from ../getvsdbgsh and pipes it to /bin/sh for execution and provides a few arguments, most importantly the /vsdbg which is where the remote debugger will be extracted to
RUN curl -sSL https://aka.ms/getvsdbgsh | /bin/sh /dev/stdin -v latest -l ~/vsdbg

WORKDIR /basketwork
#src\Services\Basket\Basket.Api\Basket.Api.csproj 
#COPY ["Services/Basket/Basket.Api/Basket.Api.csproj", "Services/Basket/Basket.Api/"] 
COPY Basket.Api.csproj ./
#COPY ["Services/Discount/Discount.Grpc/Protos/discount.proto", "Services/Discount/Discount.Grpc/Protos/"]
# # Copy necessary files and restore as distinct layer
# #COPY *.csproj ./
RUN dotnet restore  

# # Copy everything else and build
COPY . ./ 

RUN dotnet publish   -o out -c Release
# #--no-launch-profile means it doesnt access launchSettings.json
ENTRYPOINT ["dotnet", "run",  "--no-launch-profile"]  
# # Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:5.0 as prod

WORKDIR /app/

COPY --from=debug /basketwork/out /app/
RUN chmod +x /app/ 
#will be used for healthcheck
RUN touch /app/healthfile 

CMD dotnet Basket.Api.dll