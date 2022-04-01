#configuration.actors
dapr run --dapr-http-port 3500 --app-id configuration_actors --app-port 5011 dotnet run

#Test.Actors
dapr run --dapr-http-port 3501 --app-id test_actors2 --app-port 5000 dotnet run

#Test.Api
dapr run --app-id test_api --app-port 7276 --app-ssl dotnet run

#TestWorker
dapr run --app-id testtest dotnet run 0a844738-bec9-47d9-ba6e-6a97535898ca

#ReactClient
dapr run --app-id react_client --app-port 7163 --app-ssl dotnet run

#WorkerManager.Actors
dapr run --dapr-http-port 3501 --app-id worker_manager_actors --app-port 5000 dotnet run
