
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /app
COPY  ./ ./Auth4.WebApi/
RUN dotnet restore Auth4.WebApi/Auth4.WebApi.csproj
WORKDIR /app/Auth4.WebApi
RUN dotnet publish -c Release -o out


FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS runtime
WORKDIR /app
COPY --from=build /app/Auth4.WebApi/out ./
ENV ASPNETCORE_URLS=http://+:5001
EXPOSE 5001
ENTRYPOINT ["dotnet", "Auth4.WebApi.dll", "--server.urls", "http://localhost:5001", "--environment=Production"]


