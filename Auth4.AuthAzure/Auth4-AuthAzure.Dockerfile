
FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine AS build
WORKDIR /app
COPY  ./ ./Auth4.AuthAzure/
RUN dotnet restore Auth4.AuthAzure/Auth4.AuthAzure.csproj
WORKDIR /app/Auth4.AuthAzure
RUN dotnet publish -c Release -o out


FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS runtime
WORKDIR /app
COPY --from=build /app/Auth4.AuthAzure/out ./
ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000
ENTRYPOINT ["dotnet", "Auth4.AuthAzure.dll", "--server.urls", "http://localhost:5000", "--environment=Production"]


