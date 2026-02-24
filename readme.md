# FieldTeamEquipmentInventory

Program build with WPF framework, to manage equipment inventory and status.

Each employer, equipment and transaction should be registered to build a historic.

The program uses facial or fingerprint recognition to register the responsible parties.

Facial recognition is performed by the [OpenCvSharp4](https://www.nuget.org/packages/OpenCvSharp4) package, and fingerprint recognition by [DPUruNet](https://www.nuget.org/packages/DPUruNet/) library (WIP).

## Development

The Dotnet SDK version 9.0.302 is required to debug and build this project.

```sh
# Clone the repository
git clone https://github.com/decimo3/FieldTeamEquipmentInventory.git
# Navigate to the folder
cd FieldTeamEquipmentInventory
# Restore project
dotnet restore
# Build project
dotnet build
```

To use this project, some environment variables should be set. They can be set on `dev.env` file, loaded on development scenario:

```sh
# Set false to use facial recognition or true
# to use fingerprint on authentication (WIP)
USE_FINGER=false
# Set true to use SQLite, useful on development
# or false to use PostgreSQL, to run production.
USE_SQLITE=true
# Set this value with connection string of database.
DNS_STRING=Data Source=database.db
```
