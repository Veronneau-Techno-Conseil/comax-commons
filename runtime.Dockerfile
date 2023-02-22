ARG IMG_NAME=7.0-alpine


# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:${IMG_NAME}

# Install cultures (same approach as Alpine SDK image)
RUN apk add --no-cache icu-libs

# Disable the invariant mode (set in base image)
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false


