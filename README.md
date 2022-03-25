dapr run --dapr-http-port 3500 --app-id configuration_actors --app-port 5011 dotnet run
dapr run --dapr-http-port 3500 --app-id test_actors --app-port 5010 dotnet run

dapr run --app-id test_api --app-port 7276 --app-ssl dotnet run