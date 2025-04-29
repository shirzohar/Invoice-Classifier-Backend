# שלב 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY . .
RUN dotnet publish -c Release -o out

# שלב 2: Run
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .

# 💡 חשוב! מעתיקים גם את users.db
COPY users.db .

ENTRYPOINT ["dotnet", "BusuMatchProject.dll"]
