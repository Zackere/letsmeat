Set-Location $PSScriptRoot

docker stop sql1
docker rm sql1
docker build . --tag sql1
Start-Job -ScriptBlock {
   & docker run --name sql1  -p 1433:1433 sql1
}
sleep 10
Set-Location ../LetsMeatAPI
dotnet ef database update --connection "Server=tcp:localhost,1433;Initial Catalog=letsmeatdb;Persist Security Info=False;User ID=sa;Password=DGSYAgdyq21368#$%^&*dh;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;"
