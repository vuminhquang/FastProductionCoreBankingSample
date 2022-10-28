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

# https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-aspnetcore-6.0.10-linux-x64-alpine-binaries
#RUN wget -O aspnetcore.tar.gz https://download.visualstudio.microsoft.com/download/pr/fb38e333-d733-448a-b7e9-7020b720d0e3/9cbdb02e1e42bacc02479312ef8afd09/aspnetcore-runtime-$DOTNET_VERSION-linux-musl-x64.tar.gz \
##    && dotnet_sha512='55cfa0419965dc15127fca3da0f567dd64dc4ca3981f5e3366e01f88c670f91b361548be99c507fcf9fdc51078387a230c3d09d399d7b0345ae4b6e60ae2615f' \
##    && echo "$dotnet_sha512 dotnet.tar.gz" | sha512sum -c - \
#    && mkdir -p /usr/share/dotnet \
#    && tar -zxf dotnet.tar.gz -C /usr/share/dotnet \
#    && rm dotnet.tar.gz \
#    && ln -s /usr/share/dotnet/dotnet /usr/bin/dotnet
    
WORKDIR /app
EXPOSE 5432
########################################################################################################################

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

# CMD chown -R postgres:postgres "$PGDATA" && chmod 777 "$PGDATA"
USER postgres:postgres

#ENTRYPOINT ["/usr/local/bin/docker-entrypoint.sh"]
#CMD ["postgres","&& dotnet", "/app/HyperScalePostgres.dll"]

#CMD ["/bin/sh", "-c", "while true; do echo hello; sleep 10;done"]
ENTRYPOINT ["dotnet", "HyperScalePostgres.dll"]