FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR .

# Copy csproj files and restore dependencies
COPY ["Booking.Api/Booking.Api.csproj", "Booking.Api/"]
COPY ["Booking.Application/Booking.Application.csproj", "Booking.Application/"]
COPY ["Booking.Infrastructure/Booking.Infrastructure.csproj", "Booking.Infrastructure/"]
COPY ["Booking.Domain/Booking.Domain.csproj", "Booking.Domain/"]

RUN dotnet restore "Booking.Api/Booking.Api.csproj"

# Copy everything else and build
COPY . .
WORKDIR "Booking.Api"
RUN dotnet build "Booking.Api.csproj" -c Debug -o /app/build

FROM build AS publish
RUN dotnet publish "Booking.Api.csproj" -c Debug -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Booking.Api.dll"]
