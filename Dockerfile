# ── Этап 1: сборка ────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем .sln и все .csproj — слои кэшируются, restore быстрый
COPY MetalProcessingSolution.sln ./
COPY MetalProcessingSolution/MetalProcessingSolution.csproj ./MetalProcessingSolution/
COPY Application/Application.csproj                         ./Application/
COPY Domain/Domain.csproj                                   ./Domain/
COPY Infrastructure/Infrastructure.csproj                   ./Infrastructure/

# Восстанавливаем зависимости для всего решения
RUN dotnet restore MetalProcessingSolution.sln

# Копируем весь остальной исходный код
COPY . .

# Публикуем только веб-проект (он подтянет остальные через ProjectReference)
RUN dotnet publish MetalProcessingSolution/MetalProcessingSolution.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# ── Этап 2: образ для запуска ─────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Копируем собранное приложение
COPY --from=build /app/publish .

# Папка для SQLite-файла и загружаемых изображений
# Railway хранит файлы только до следующего деплоя —
# для постоянного хранения подключи Volume в настройках Railway
RUN mkdir -p /app/ExternalUploads

# ASP.NET Core слушает на порту из переменной окружения PORT,
# которую Railway выставляет автоматически
ENV ASPNETCORE_URLS=http://+:${PORT:-8080}

ENTRYPOINT ["dotnet", "MetalProcessingSolution.dll"]
