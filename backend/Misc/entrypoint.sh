( sleep 30s && /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P $SA_PASSWORD -d master -Q "CREATE DATABASE letsmeatdb" ) & /opt/mssql/bin/sqlservr