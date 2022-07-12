
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS debug
ENV ASPNETCORE_URLS=http://+:80  
#ENV ASPNETCORE_URLS=https://+:443  
RUN apt-get update
RUN apt-get install -y unzip
# dls a sh script from ../getvsdbgsh and pipes it to /bin/sh for execution and provides a few arguments, most importantly the /vsdbg which is where the remote debugger will be extracted to
RUN curl -sSL https://aka.ms/getvsdbgsh | /bin/sh /dev/stdin -v latest -l ~/vsdbg


WORKDIR /mvcwork
COPY Web.csproj ./

RUN dotnet restore

COPY . ./
RUN dotnet publish -o out -c Release
# #--no-launch-profile means it doesnt access launchSettings.json
ENTRYPOINT ["dotnet", "run", "--no-launch-profile"] 

# # Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:5.0 as prod

WORKDIR /app/
# #COPY --from=Debug /out/ /app/
COPY --from=debug /mvcwork/out /app/
RUN chmod +x /app/
RUN touch /app/healthfile
CMD dotnet Web.dll
