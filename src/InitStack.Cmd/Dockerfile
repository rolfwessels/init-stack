FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

##
# Build build project
##
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# find src* | grep '.csproj' | grep -v obj | sed "s|src/||" | sed "s|\(.*\)/\([^/]*\)\.csproj|COPY [\"\1/\2.csproj\", \"\1/\"]|"
COPY ["InitStack.Cmd/InitStack.Cmd.csproj", "InitStack.Cmd/"]
RUN dotnet restore "InitStack.Cmd/InitStack.Cmd.csproj"

COPY . .
WORKDIR "/src/InitStack.Cmd"
RUN dotnet build "InitStack.Cmd.csproj" -c Release -o /app/build

##
# Publish
##
FROM build AS publish
ARG  VERSION=1.0.1
RUN dotnet publish "InitStack.Cmd.csproj" -p:FileVersion=${VERSION} -p:VersionPrefix=${VERSION} -c Release -o /app/publish /p:UseAppHost=false

##
# Make the release
##
FROM base AS final
WORKDIR /app
ARG  VERSION=1.0.1
LABEL version="${VERSION}"
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet","InitStack.dll","default","service"]
