FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["FeedbackPlatform/FeedbackPlatform.csproj", "FeedbackPlatform/"]
RUN dotnet restore "FeedbackPlatform/FeedbackPlatform.csproj"
COPY . .
WORKDIR "/src/FeedbackPlatform"
RUN dotnet build "FeedbackPlatform.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FeedbackPlatform.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FeedbackPlatform.dll"]