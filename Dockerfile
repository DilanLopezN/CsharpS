# Etapa 1: Build
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /app

# Copiar todos os arquivos
COPY . .

# Restaurar pacotes apenas para o projeto principal
RUN dotnet restore src/Simjob.Framework.Services.Api/Simjob.Framework.Services.Api.csproj

# Publicar o projeto em Release
RUN dotnet publish src/Simjob.Framework.Services.Api/Simjob.Framework.Services.Api.csproj -c Release -o /app/out

# Etapa 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app

# Copiar os binários publicados
COPY --from=build /app/out .

# Copiar o Environment.ini gerado pela pipeline
COPY Environment.ini ./Environment.ini

# Expor a porta 2083
ARG PORT=2083
ENV PORT=${PORT}
EXPOSE ${PORT}

# Definir o entrypoint
ENTRYPOINT ["dotnet", "Simjob.Framework.Services.Api.dll"]