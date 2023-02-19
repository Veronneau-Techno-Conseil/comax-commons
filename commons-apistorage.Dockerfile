ARG IMG_NAME=7.0-alpine

# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:${IMG_NAME} AS build-env
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY ./src ./
RUN dotnet restore

# Copy everything else and build
WORKDIR /src/GrainStorageService
RUN dotnet publish -c Release -o ../out
WORKDIR /src/out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:${IMG_NAME}
WORKDIR /app
COPY --from=build-env src/out .

# Install cultures (same approach as Alpine SDK image)
RUN apk add --no-cache icu-libs

# Disable the invariant mode (set in base image)
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

EXPOSE 80

RUN chown 1000: ./
RUN chmod -R u+x /app/GrainStorageService.dll
USER 1000
ENTRYPOINT ["dotnet", "/app/GrainStorageService.dll"]
