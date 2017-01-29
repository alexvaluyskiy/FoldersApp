FROM microsoft/dotnet:1.1.0-sdk-msbuild-rc3
RUN ls
RUN dotnet restore && dotnet build
EXPOSE 5000
ENTRYPOINT ["dotnet", "run"]
