FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
EXPOSE 8080
COPY ["../..src/PerfectBreakfast.API/PerfectBreakfast.API.csproj", "src/PerfectBreakfast.API/"]
COPY ["../..src/PerfectBreakfast.Application/PerfectBreakfast.Application.csproj", "src/PerfectBreakfast.Application/"]
COPY ["../..src/PerfectBreakfast.Domain/PerfectBreakfast.Domain.csproj", "src/PerfectBreakfast.Domain/"]
COPY ["../..src/PerfectBreakfast.Infrastructure/PerfectBreakfast.Infrastructure.csproj", "src/PerfectBreakfast.Infrastructure/"]
RUN dotnet restore "src/PerfectBreakfast.API/PerfectBreakfast.API.csproj"
COPY . .
WORKDIR "/src/src/PerfectBreakfast.API"
RUN dotnet build "PerfectBreakfast.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PerfectBreakfast.API.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PerfectBreakfast.API.dll"]


# docker build -t yourusername/yourappname:tagversion .
# docker build -t viethungdev23/backend-dotnet8 -f .\src\PerfectBreakfast.API\Dockerfile .