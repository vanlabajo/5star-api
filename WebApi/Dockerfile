FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine AS publish
WORKDIR /src
COPY ["Core/Core.csproj", "Core/"]
COPY ["Repository/Repository.csproj", "Repository/"]
COPY ["Services/Services.csproj", "Services/"]
COPY ["WebApi/WebApi.csproj", "WebApi/"]
RUN dotnet restore "Core/Core.csproj" --runtime alpine-x64
RUN dotnet restore "Repository/Repository.csproj" --runtime alpine-x64
RUN dotnet restore "Services/Services.csproj" --runtime alpine-x64
RUN dotnet restore "WebApi/WebApi.csproj" --runtime alpine-x64
COPY . .

WORKDIR "/src/WebApi"
RUN dotnet publish "WebApi.csproj" -c Release -o /app/publish \
  --no-restore \
  --runtime alpine-x64 \
  --self-contained true \
  -p:PublishTrimmed=true \
  -p:PublishSingleFile=true

FROM mcr.microsoft.com/dotnet/runtime-deps:5.0-alpine AS final
WORKDIR /app
EXPOSE 80
EXPOSE 443
COPY --from=publish /app/publish .
ENTRYPOINT ["./WebApi"]