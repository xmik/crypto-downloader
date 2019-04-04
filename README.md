# crypto-downloader

A console application (CLI) to download crypto currency data from Poloniex.

## Usage
You may need to generate the nuget config, run:
```
./tasks generate_nuget_config
```

First, compile the code:
```
./tasks build
```

Then:
```
executable="./src/CryptoDownloaderConsole/bin/Release/netcoreapp2.1/CryptoDownloaderConsole.dll"
# print help
dotnet "${executable}" --help

# list all instruments available on Poloniex
dotnet "${executable}" list
```

## Dependencies
dotnet-sdk-2.1, [here](https://dotnet.microsoft.com/learn/dotnet/hello-world-tutorial/install) are instructions how to install.

## Development
Run unit tests:
```
./tasks utest
```

Run integration tests:
```
./tasks itest
```

Run acceptance tests:
```
./tasks atest
```