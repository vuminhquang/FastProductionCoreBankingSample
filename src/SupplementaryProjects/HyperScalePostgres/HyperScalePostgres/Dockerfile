#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0.10-alpine3.16-amd64 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

########################################################################################################################
# Build any external system and include into final, just copy from Dockerfile to Dockerfile
# Build postgres
FROM  postgres:15.0-alpine as postgres-build
RUN runDeps="$( \
    		scanelf --needed --nobanner --format '%n#p' --recursive /usr/local \
    			| tr ',' '\n' \
    			| sort -u \
    			| awk 'system("[ -e /usr/local/lib/" $1 " ]") == 0 { next } { print "so:" $1 }' \
    # Remove plperl, plpython and pltcl dependencies by default to save image size
    # To use the pl extensions, those have to be installed in a derived image
    			| grep -v -e perl -e python -e tcl \
    	)"; \
    echo $runDeps > /var/lib/runDeps.txt
# Build postgres done
# Copy postgres to final images
# https://github.com/docker-library/postgres/blob/master/Dockerfile-alpine.template
FROM base AS final
RUN set -eux; \
	addgroup -g 70 -S postgres; \
	adduser -u 70 -S -D -G postgres -H -h /var/lib/postgresql -s /bin/sh postgres; \
	mkdir -p /var/lib/postgresql; \
	chown -R postgres:postgres /var/lib/postgresql

ENV LANG en_US.utf8
RUN mkdir /docker-entrypoint-initdb.d

COPY --chown=postgres:postgres --from=postgres-build /var/lib/runDeps.txt /var/lib/runDeps.txt
RUN runDeps=$(cat /var/lib/runDeps.txt) \
    apk add --no-cache --virtual .postgresql-rundeps \
		$runDeps \
		bash \
		su-exec \
		tzdata \
		zstd \
        icu-data-full;
    
RUN mkdir -p /var/run/postgresql && chown -R postgres:postgres /var/run/postgresql && chmod 2777 /var/run/postgresql    
ENV PGDATA /var/lib/postgresql/data
# this 777 will be replaced by 700 at runtime (allows semi-arbitrary "--user" values)
RUN mkdir -p "$PGDATA" && chown -R postgres:postgres "$PGDATA" && chmod 700 "$PGDATA"
VOLUME /var/lib/postgresql/data

COPY --chown=postgres:postgres --from=postgres-build /var/lib/postgresql /var/lib/postgresql
COPY --chown=postgres:postgres --from=postgres-build /var/run/postgresql /var/run/postgresql
COPY --chown=postgres:postgres --from=postgres-build /usr/local/bin/postgres /usr/local/bin/postgres
#COPY --chown=postgres:postgres --from=postgres-build /usr/local/share/postgresql/postgresql.conf.sample /usr/local/share/postgresql/postgresql.conf.sample
# Important: copy the needed libraries
COPY --chown=postgres:postgres --from=postgres-build /usr/lib/ /usr/lib/
COPY --chown=postgres:postgres --from=postgres-build /usr/local/lib/postgresql/ /usr/local/lib/postgresql/
COPY --chown=postgres:postgres --from=postgres-build /usr/local/share/postgresql/timezonesets /usr/local/share/postgresql/timezonesets

STOPSIGNAL SIGINT
COPY --from=postgres-build /usr/local/bin/docker-entrypoint.sh /usr/local/bin/docker-entrypoint.sh
#RUN chmod -R 755 /usr/local/bin/docker-entrypoint.sh
RUN chown postgres:postgres /usr/local/bin/docker-entrypoint.sh

ENTRYPOINT ["/usr/local/bin/docker-entrypoint.sh"]
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

FROM final
WORKDIR /app
COPY --from=publish /app/publish .
USER postgres:postgres
ENTRYPOINT ["dotnet", "HyperScalePostgres.dll"]