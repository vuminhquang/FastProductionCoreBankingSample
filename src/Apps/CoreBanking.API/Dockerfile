FROM mcr.microsoft.com/dotnet/aspnet:6.0.10-alpine3.16-amd64 AS base
WORKDIR /app

########################################################################################################################
# Build any external system and include into final, just copy from Dockerfile to Dockerfile
# To remote building docker: run tmux, podman system service --time=0 tcp:50.0.0.1:2979

# Build nginx # Extract from here: https://zawadidone.nl/using-docker-images-from-scratch/
FROM  nginx:alpine as nginx-build
# Build nginx done

# Build EventStoreDb
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS eventstore-build
ARG RUNTIME=linux-x64
#RUN apt install -y git
RUN mkdir /build
#RUN git clone https://github.com/EventStore/EventStore.git /build
# Instead of git clone, very slow, download using wget
RUN wget --no-check-certificate -O /tmp/7zip.deb http://ftp.us.debian.org/debian/pool/main/p/p7zip/p7zip_16.02+dfsg-8_amd64.deb; \
    dpkg -i /tmp/7zip.deb;
RUN wget --no-check-certificate -O /tmp/7zip-full.deb http://ftp.us.debian.org/debian/pool/main/p/p7zip/p7zip-full_16.02+dfsg-8_amd64.deb; \
    dpkg -i /tmp/7zip-full.deb;
RUN GIT_URL="https://github.com/EventStore/EventStore/archive/refs/heads/master.zip"; \
    wget --no-check-certificate -O /tmp/master.zip $GIT_URL;
RUN 7z x /tmp/master.zip -o/tmp/master; \
    cp -r /tmp/master/EventStore-master/* /build;
WORKDIR /build/src
RUN dotnet restore --runtime=${RUNTIME}
# "publish" image
FROM eventstore-build as eventstore-publish
ARG RUNTIME=alpine-x64
RUN dotnet publish --configuration=Release --runtime=${RUNTIME} --self-contained \
     --framework=net6.0 --output /publish EventStore.ClusterNode
# "runtime" image
FROM mcr.microsoft.com/dotnet/runtime:6.0.10-alpine3.16-amd64 AS eventstore-runtime
ARG RUNTIME=alpine-x64
ARG UID=1000
ARG GID=1000
RUN if [[ "${RUNTIME}" = "alpine-x64" ]];\
    then \
        apk update && \
        apk add --no-cache \
        curl; \
    else \
        apt update && \
        apt install -y \
        curl && \
        rm -rf /var/lib/apt/lists/*; \
    fi
WORKDIR /opt/eventstore
RUN addgroup --gid ${GID} "eventstore" && \
    adduser \
    --disabled-password \
    --gecos "" \
    --ingroup "eventstore" \
    --no-create-home \
    --uid ${UID} \
    "eventstore"
COPY --chown=eventstore:eventstore --from=eventstore-publish /publish ./
RUN mkdir -p /var/lib/eventstore && \
    mkdir -p /var/log/eventstore && \
    mkdir -p /etc/eventstore && \
    chown -R eventstore:eventstore /var/lib/eventstore /var/log/eventstore /etc/eventstore
USER eventstore
RUN printf "ExtIp: 0.0.0.0\n\
IntIp: 0.0.0.0" >> /etc/eventstore/eventstore.conf
# Build EventStoreDb done

# Copy to final docker
FROM base AS final
RUN apk update && apk upgrade
RUN apk add --no-cache tzdata
RUN addgroup -S tor && adduser -S tor -G tor
# copy nginx begin ---------------------------------------------------------------------------
# Customise static content, and configuration
#COPY --from=nginx-build /etc/passwd /etc/passwd
COPY --from=nginx-build [ \
    "/lib/ld-musl-x86_64.so.1", \
    "/lib/libssl.so.1.1", \
    "/lib/libcrypto.so.1.1", \
    "/lib/libz.so.1", \
    "/lib/ld-musl-x86_64.so.1", \
    "/lib/" \
    ]
COPY --from=nginx-build ["/usr/lib/libpcre2-8.so.0", "/usr/lib/"]
COPY --from=nginx-build ["/usr/sbin/nginx", "/usr/sbin/nginx"]
COPY --from=nginx-build ["/var/log/nginx", "/var/log/nginx"]
COPY --from=nginx-build ["/etc/nginx", "/etc/nginx"]
COPY --from=nginx-build ["/usr/share/nginx/html/index.html", "/usr/share/nginx/html/index.html"]
# Run as non-root -> need create folders and permissions
RUN mkdir -p /var/cache/nginx/ && \
    chown tor /var/cache/nginx/
#COPY default.conf /etc/nginx/conf.d/
#COPY nginx.conf /etc/nginx/
COPY Gateways/nginx/nginx.conf /etc/nginx/
#Change default stop signal from SIGTERM to SIGQUIT
#STOPSIGNAL SIGQUIT
## install nginx done, copy files for using https with nginx
COPY Gateways/nginx/cert.crt /etc/ssl/certs/cert.crt
COPY Gateways/nginx/cert.key /etc/ssl/private/cert.key
# copy nginx done ------------------------------------------------------------------------------

# copy eventstoredb begin ----------------------------------------------------------------------
COPY --from=eventstore-runtime /opt/eventstore /opt/eventstore
COPY --from=eventstore-runtime /etc/eventstore /etc/eventstore
RUN mkdir -p /var/lib/eventstore && \
    mkdir -p /var/log/eventstore && \
    mkdir -p /etc/eventstore && \
    chown -R tor /var/lib/eventstore /var/log/eventstore /etc/eventstore
RUN chown -R tor /opt/eventstore
# copy eventstoredb done -----------------------------------------------------------------------

# Install tor
RUN apk add --no-cache tor curl && \
    rm /var/cache/apk/* && \
    cp /etc/tor/torrc.sample /etc/tor/torrc

########################################################################################################################

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
# Copy the main application prj
COPY ["Apps/CoreBanking.API/CoreBanking.API.csproj", "Apps/CoreBanking.API/"]
# https://andrewlock.net/optimising-asp-net-core-apps-in-docker-avoiding-manually-copying-csproj-files-part-2/
# Copy other Application projects files
COPY BuildingBlocks/Application/*/*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p BuildingBlocks/Application/${file%.*}/ && mv $file BuildingBlocks/Application/${file%.*}/; done
# Copy other Application projects files
COPY BuildingBlocks/Domain/*/*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p BuildingBlocks/Domain/${file%.*}/ && mv $file BuildingBlocks/Domain/${file%.*}/; done
# Copy other Application projects files
COPY BuildingBlocks/Infrastructure/*/*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p BuildingBlocks/Infrastructure/${file%.*}/ && mv $file BuildingBlocks/Infrastructure/${file%.*}/; done
# Restore the main application
RUN dotnet restore "Apps/CoreBanking.API/CoreBanking.API.csproj"
COPY . .
WORKDIR "/src/Apps/CoreBanking.API"
RUN dotnet build "CoreBanking.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CoreBanking.API.csproj" -c Release -o /app/publish

FROM final
WORKDIR /app
ENV TZ=Asia/Ho_Chi_Minh
COPY --from=publish /app/publish .

USER tor
ENTRYPOINT ["dotnet", "CoreBanking.API.dll"]
