FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

COPY TaskManager.Domain/*.csproj ./TaskManager.Domain/
COPY TaskManager.Application/*.csproj ./TaskManager.Application/
COPY TaskManager.Infrastructure/*.csproj ./TaskManager.Infrastructure/
COPY TaskManager.Api/*.csproj ./TaskManager.Api/

RUN dotnet restore TaskManager.Api/TaskManager.Api.csproj

COPY TaskManager.Domain/. ./TaskManager.Domain/
COPY TaskManager.Application/. ./TaskManager.Application/
COPY TaskManager.Infrastructure/. ./TaskManager.Infrastructure/
COPY TaskManager.Api/. ./TaskManager.Api/

RUN dotnet publish TaskManager.Api/TaskManager.Api.csproj -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .

EXPOSE 8080
ENTRYPOINT [ "dotnet", "TaskManager.Api.dll" ]