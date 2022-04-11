FROM public.ecr.aws/lambda/dotnet:6 AS base

FROM mcr.microsoft.com/dotnet/sdk:6.0-bullseye-slim as build
WORKDIR /src
COPY ["ConfigTest.csproj", "ConfigTest/"]
RUN dotnet restore "ConfigTest/ConfigTest.csproj"

WORKDIR "/src/ConfigTest"
COPY . .
RUN dotnet build "ConfigTest.csproj" --configuration Release --output /app/build

FROM build AS publish
RUN dotnet publish "ConfigTest.csproj" \
            --configuration Release \ 
            --runtime linux-x64 \
            --self-contained true \ 
            --output /app/publish \
            -p:PublishReadyToRun=true  

FROM base AS final
WORKDIR /var/task
COPY --from=publish /app/publish .
CMD ["ConfigTest::ConfigTest.LambdaEntryPoint::FunctionHandlerAsync"]a