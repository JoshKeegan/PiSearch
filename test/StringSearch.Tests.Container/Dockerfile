FROM mcr.microsoft.com/dotnet/sdk:7.0.100
WORKDIR /app

# TODO: Would prefer the build wasn't done here so that it can reference projects outside of this directory
#	later if required, but seems to be some sort off issue when building on windows then copying into the container
#	so this will work for now, but should come back to this.
COPY *.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet build -c Release

ENTRYPOINT dotnet test --no-build -c Release --logger "trx;logfilename=/artefacts/testResults/ContainerTests.trx"