# Builds

```
# Tests only
$ dotnet fake -s run build.fsx --target test
# Deployment
$ dotnet fake -s run build.fsx 
```

# How to run Tests

Example with the `Api` module:

```
$ cd Tests/Api

# Concise test mode
$ dotnet run

# Verbose test mode
$ dotnet run --summary

# Run only integration tests
$ dotnet run --filter 'Api.Integration'

```
