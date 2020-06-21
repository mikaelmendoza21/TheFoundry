# Retrofitter Foundry

### A .NET Console App to Seed/Update your local Db

Pulls data from magicthegathering.io to set up your FoundryDb.

Run on a schedule (or manually) to update your existing FoundryDb with the latest Sets released.

## Running App

On Windows Powershell

```
# Seed Whole Db
 .\RetrofitterFoundry.exe --seed

 # Update only
  .\RetrofitterFoundry.exe
```

On Linux

```
# whole Db Seed
dotnet RetrofitterFoundry.dll --seed

# Update only
dotnet RetrofitterFoundry.dll
```

### Notes

This assumes you already set up your MongoDb database and collections using these defaults:

- ConnString = "mongodb://localhost:27017"

- DbName = "FoundryDb"

- SetsCollection = "Sets"

- MetaCardsCollection = "MetaCards"
    
- CardsCollection = "Cards"