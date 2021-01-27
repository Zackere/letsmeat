cd "$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
cd LetsMeatAPI
dotnet ef database update --connection "$LETSMEATDB_CONNECTION_STRING" || exit 1
dotnet publish LetsMeatAPI.csproj --configuration Release
cd bin/Release/netcoreapp3.1/
7z a letsmeatapi.zip ./publish/*
curl -u \$letsmeatapi:${AZURE_DEPLOYMENT_PASSWORD} --data-binary @letsmeatapi.zip https://letsmeatapi.scm.azurewebsites.net/api/zipdeploy
