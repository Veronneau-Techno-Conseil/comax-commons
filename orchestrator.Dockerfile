ARG IMG_NAME=7.0-alpine

# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:${IMG_NAME} AS build-env
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY ./src ./
RUN dotnet restore

# Copy everything else and build
WORKDIR /src/Node
RUN dotnet publish -c Release -o ../out
WORKDIR /src/out

# Build runtime image
FROM vertechcon/comax-runtime:${IMG_NAME}
WORKDIR /app
COPY --from=build-env src/out .

EXPOSE 7718
EXPOSE 30001

RUN chown 1000: ./
RUN chmod -R u+x ./
USER 1000
ENTRYPOINT ["dotnet", "/app/Comax.Commons.Orchestrator.dll"]
