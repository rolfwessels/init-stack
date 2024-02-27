FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

##
# Build build project
##
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# find src* | grep '.csproj' | grep -v obj | sed "s|src/||" | sed "s|\(.*\)/\([^/]*\)\.csproj|COPY [\"\1/\2.csproj\", \"\1/\"]|"
COPY ["TemplateDotnetCoreConsoleApp.Cmd/TemplateDotnetCoreConsoleApp.Cmd.csproj", "TemplateDotnetCoreConsoleApp.Cmd/"]
RUN dotnet restore "TemplateDotnetCoreConsoleApp.Cmd/TemplateDotnetCoreConsoleApp.Cmd.csproj"

COPY . .
WORKDIR "/src/TemplateDotnetCoreConsoleApp.Cmd"
RUN dotnet build "TemplateDotnetCoreConsoleApp.Cmd.csproj" -c Release -o /app/build

##
# Publish
##
FROM build AS publish
ARG  VERSION=1.0.1
RUN dotnet publish "TemplateDotnetCoreConsoleApp.Cmd.csproj" -p:FileVersion=${VERSION} -p:VersionPrefix=${VERSION} -c Release -o /app/publish /p:UseAppHost=false

##
# Make the release
##
FROM base AS final
WORKDIR /app
ARG  VERSION=1.0.1
LABEL version="${VERSION}"
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet","TemplateDotnetCoreConsoleApp.dll","default","service"]
