# ===== BUILD STAGE =====
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy file csproj trước để cache restore
COPY DineOS.Api/*.csproj ./DineOS.Api/
RUN dotnet restore DineOS.Api/DineOS.Api.csproj

# Copy toàn bộ source
COPY . .

# Chuyển vào thư mục API để build
WORKDIR /src/DineOS.Api
RUN dotnet publish -c Release -o /app

# ===== RUNTIME STAGE =====
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app .

# Bắt buộc cho Render
ENV ASPNETCORE_URLS=http://0.0.0.0:10000
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 10000

ENTRYPOINT ["dotnet", "DineOS.Api.dll"]