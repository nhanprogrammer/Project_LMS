# FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
# WORKDIR /app
# EXPOSE 5050

# FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
# WORKDIR /src
# COPY ["Project_LMS.csproj", "."]
# RUN dotnet restore "Project_LMS.csproj"
# COPY . .
# RUN dotnet build "Project_LMS.csproj" -c Release -o /app/build

# FROM build AS publish
# RUN dotnet publish "Project_LMS.csproj" -c Release -o /app/publish

# FROM base AS final
# WORKDIR /app
# COPY --from=publish /app/publish .
# ENTRYPOINT ["dotnet", "Project_LMS.dll"]

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app

# Copy thư mục publish vào container
COPY ./publish /app

# Mở cổng API
EXPOSE 5050

# Chạy ứng dụng
ENTRYPOINT ["dotnet", "Project_LMS.dll"]