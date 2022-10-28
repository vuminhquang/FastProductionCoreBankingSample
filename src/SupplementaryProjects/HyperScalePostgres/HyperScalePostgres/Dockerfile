FROM mcr.microsoft.com/dotnet/aspnet:6.0.10-alpine3.16-amd64 AS dotnet-build

#FROM postgres:15.0-alpine AS base
FROM citusdata/citus:alpine AS base
# .NET globalization APIs will use invariant mode by default because DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=true is set
# by the base runtime-deps image. See https://aka.ms/dotnet-globalization-alpine-containers for more information.

# ASP.NET Core version
ENV DOTNET_VERSION=6.0.10
ENV ASPNET_VERSION=6.0.10
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=true
ENV DOTNET_RUNNING_IN_CONTAINER=true

# Install ASP.NET Core
RUN apk add --no-cache bash icu-libs krb5-libs libgcc libintl libssl1.1 libstdc++ zlib
COPY --from=dotnet-build /usr/share/dotnet /usr/share/dotnet
RUN ln -s /usr/share/dotnet/dotnet /usr/bin/dotnet
    
WORKDIR /app
EXPOSE 5432

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["SupplementaryProjects/HyperScalePostgres/HyperScalePostgres/HyperScalePostgres.csproj", "SupplementaryProjects/HyperScalePostgres/HyperScalePostgres/"]
COPY ["SupplementaryProjects/HyperScalePostgres/HyperScalePostgres.Infrastructure/HyperScalePostgres.Infrastructure.csproj", "SupplementaryProjects/HyperScalePostgres/HyperScalePostgres.Infrastructure/"]
RUN dotnet restore "SupplementaryProjects/HyperScalePostgres/HyperScalePostgres/HyperScalePostgres.csproj"
COPY . .
WORKDIR "/src/SupplementaryProjects/HyperScalePostgres/HyperScalePostgres"
RUN dotnet build "HyperScalePostgres.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HyperScalePostgres.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

USER postgres:postgres
ENTRYPOINT ["dotnet", "HyperScalePostgres.dll"]