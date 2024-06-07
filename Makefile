VER=0.1.0
CONFIG=Debug
clean:
	dotnet.exe clean -c $(CONFIG)
build:
	dotnet.exe build -c $(CONFIG)
deploy:
	dotnet.exe nuget push ./TelemetrySrcGen.Abstractions/bin/$(CONFIG)/TelemetrySrcGen.Abstractions.$(VER).nupkg --api-key ${ACTORSRCGEN_NUGET_APIKEY} --source https://api.nuget.org/v3/index.json
	dotnet.exe nuget push ./TelemetrySrcGen/bin/$(CONFIG)/TelemetrySrcGen.$(VER).nupkg --api-key ${ACTORSRCGEN_NUGET_APIKEY} --source https://api.nuget.org/v3/index.json
	