# README for developers

## Development
Compile all C# projects (this will also install all the dependencies):
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

Run integration tests with [Bats](https://github.com/sstephenson/bats):
```
./tasks itest
```

### Install Bats on Linux
```
git clone --depth 1 https://github.com/sstephenson/bats.git /opt/bats
git clone --depth 1 https://github.com/ztombol/bats-support.git /opt/bats-support
git clone --depth 1 https://github.com/ztombol/bats-assert.git /opt/bats-assert
/opt/bats/install.sh /usr/local
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

## How external dependencies were added
Add external packages references to the C# projects files
```
work$ dotnet add src/CryptoDownloaderConsole/ package --version=0.5.9 DigitalRuby.ExchangeSharp
work$ dotnet add src/CryptoDownloaderConsole/ package --version=2.4.3 CommandLineParser
work$ dotnet add src/CryptoDownloaderConsole/ package --version=2.4.4 NodaTime
work$ dotnet add tests/CryptoDownloaderConsole.Tests/ package --version=2.4.4 NodaTime
work$ dotnet add src/CryptoDownloaderConsole/ package --version=2.0.8 log4net
work$ dotnet add src/CryptoDownloaderConsole/ package --version=4.9.1 Autofac
work$ dotnet add tests/CryptoDownloaderConsole.Tests/ package --version=4.9.1 Autofac
work$ dotnet add tests/CryptoDownloaderConsole.Tests/ package --version=4.10.1 Moq
```
