# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj and sln files and restore dependencies
COPY *.sln .
COPY src/Core.Domain/*.csproj ./src/Core.Domain/
COPY src/Core.Application/*.csproj ./src/Core.Application/
COPY src/Infrastructure/*.csproj ./src/Infrastructure/
COPY src/Presentation/API/*.csproj ./src/Presentation/API/
COPY tests/Application.UnitTests/*.csproj ./tests/Application.UnitTests/
COPY tests/API.IntegrationTests/*.csproj ./tests/API.IntegrationTests/

# Restore dependencies for all projects
RUN dotnet restore

# Copy the rest of the source code
COPY . .
WORKDIR "/src/src/Presentation/API"
RUN dotnet build "API.csproj" -c Release -o /app/build

# Stage 2: Test the application
FROM build AS test
WORKDIR /src
RUN dotnet test --logger "console;verbosity=detailed"

# Stage 3: Publish the application
FROM build AS publish
WORKDIR "/src/src/Presentation/API"
RUN dotnet publish "API.csproj" -c Release -o /app/publish

# Stage 4: Create the final runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Expose the port the app will run on
EXPOSE 8080
EXPOSE 8081

# Set the entrypoint for the container
ENTRYPOINT ["dotnet", "API.dll"] 