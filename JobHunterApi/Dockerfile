# Step 1: Use .NET SDK to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy project file and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy the entire application and build it
COPY . ./
RUN dotnet publish -c Release -o /publish

# Step 2: Use a lightweight .NET runtime for running the app
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /publish .

# Expose the application port
EXPOSE 80

ENV awsdatabase="Server=db-jobhunter-instance.c3wicyiaymao.us-east-1.rds.amazonaws.com;Database=JobHunter_AWS_Database;port=3306;User=hitesh;Password=Hitzeeaws1312!"

# Set the entrypoint
ENTRYPOINT ["dotnet", "JobHunterApi.dll"]
