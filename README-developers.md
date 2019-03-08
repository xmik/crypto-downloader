# README for developers

## Development
Compile all C# projects:
```
./tasks build
# OR:
dotnet build
```

Run C# tests:
```
./tasks test
# OR:
dotnet test
```

Run the Console project:
```
./tasks run
# OR:
dotnet run --project=./src/CryptoDownloaderConsole/
```

## How the C# solution was set up
```
# Create a C# .sln file (solution)
work$ dotnet new sln --name CryptoDownloader

# Create a C# project of type Console (CLI application)
work$ mkdir -p src/CryptoDownloaderConsole
work$ cd src/CryptoDownloaderConsole
work/src/CryptoDownloaderConsole$ dotnet new console
work/src/CryptoDownloaderConsole$ cd ../..

# Reference the console C# project from the .sln file
work$ dotnet sln ./CryptoDownloader.sln add ./src/CryptoDownloaderConsole/CryptoDownloaderConsole.csproj

# Create a C# project with tests
work$ mkdir -p tests/CryptoDownloaderConsole.Tests
work$ cd tests/CryptoDownloaderConsole.Tests
work/tests/CryptoDownloaderConsole.Tests$ dotnet new xunit
work/tests/CryptoDownloaderConsole.Tests$ cd ../..

# Reference the tests C# project from the .sln file
work$ dotnet sln ./CryptoDownloader.sln add tests/CryptoDownloaderConsole.Tests/CryptoDownloaderConsole.Tests.csproj

# Reference the console C# project from the tests C# project
work$ dotnet add tests/CryptoDownloaderConsole.Tests/CryptoDownloaderConsole.Tests.csproj reference src/CryptoDownloaderConsole/CryptoDownloaderConsole.csproj
```
