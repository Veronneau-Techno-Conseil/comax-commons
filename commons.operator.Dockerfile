ARG IMG_NAME=7.0-alpine

# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:${IMG_NAME} AS build-env
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY ./src ./
RUN dotnet restore

WORKDIR /src/CommonsAgentOperator
RUN dotnet publish -c Release -o ../out
WORKDIR /src/out

# Build runtime image
FROM vertechcon/comax-runtime:${IMG_NAME}
WORKDIR /app
COPY --from=build-env src/out .


RUN addgroup -g 1000 -S comax \
 && adduser -u 1000 -H -D -G comax comax \
 && chmod +x /app/CommunAxiomCommonsAgentOperator.dll

USER comax
ENTRYPOINT [ "/app/CommunAxiomCommonsAgentOperator.dll" ]
