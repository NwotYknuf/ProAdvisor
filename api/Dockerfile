FROM mcr.microsoft.com/dotnet/core/sdk:3.1
COPY . /app
WORKDIR /app
EXPOSE 5000
EXPOSE 5001
ENTRYPOINT ["dotnet", "run", "--urls", "http://0.0.0.0:5000"]