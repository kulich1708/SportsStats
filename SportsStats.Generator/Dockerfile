# syntax=docker/dockerfile:1

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build

COPY . /source

WORKDIR /source/SportsStats.Generator

ARG TARGETARCH

# Публикуем генератор
RUN dotnet publish -a ${TARGETARCH/amd64/x64} --use-current-runtime --self-contained false -o /app

# Финальный образ
FROM mcr.microsoft.com/dotnet/runtime:10.0-alpine AS final

WORKDIR /app

# Копируем результат сборки
COPY --from=build /app .

ENTRYPOINT ["dotnet", "SportsStats.Generator.dll"]