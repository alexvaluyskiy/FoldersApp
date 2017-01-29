FROM microsoft/dotnet:1.1.0-sdk-msbuild-rc3
RUN git clone https://github.com/alexvaluyskiy/FoldersApp.git
WORKDIR FoldersApp/src/FoldersApp
RUN dotnet restore && dotnet build
EXPOSE 5000
ENTRYPOINT ["dotnet", "run"]
