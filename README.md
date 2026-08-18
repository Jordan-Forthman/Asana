# Asana

A task and project tracker built as a three-tier .NET 8 solution: an ASP.NET
Core Web API over JSON file storage, a shared class library holding the models
and HTTP client, and two front ends that talk to the same API, a console app and
a .NET MAUI cross-platform app.

The point of the layout is that the front ends hold no business logic. They call
service proxies in the library, which call the API over HTTP, which persists
through an enterprise layer to disk. Swapping the storage or adding a third
front end touches one tier.

## Quickstart

Needs the .NET 8 SDK. Nothing else for the API and CLI.

```bash
git clone https://github.com/Jordan-Forthman/Asana.git
cd Asana
dotnet build Asana.Backend.slnf
```

Run the API in one terminal:

```bash
dotnet run --project Asana.API
```

and the console client in another:

```bash
dotnet run --project Asana.CLI
```

```
Choose a menu option:
  ToDos
     1. Create a ToDo
     2. List all ToDos
     ...
  Projects
     6. Create a Project
     7. List all Projects
    12. Exit
1
Name:Ship the repo
Description:Final polish
2
[1] Ship the repo - Final polish
```

`Asana.Backend.slnf` is a solution filter covering the API, library, and CLI.
The full `Asana.sln` also contains the MAUI app, which needs
`dotnet workload install maui` and platform SDKs, so the filter is what builds
on a plain SDK install.

## Configuration

| Setting | Where | Default |
| --- | --- | --- |
| API address the clients call | `ASANA_API_URL` | `http://localhost:5206` |
| Where the API stores JSON | `Storage:Root` in appsettings, or `ASANA_DATA_DIR` | `data/` beside the API project |

Both have working defaults, so no configuration is needed to run locally. The
API's `http` launch profile listens on the address the clients expect.

```bash
ASANA_DATA_DIR=/tmp/asana dotnet run --project Asana.API
ASANA_API_URL=http://localhost:5206 dotnet run --project Asana.CLI
```

If the API is not running, the clients say so and start with an empty list
rather than failing:

```
Could not reach the Asana API at http://localhost:5206
  HttpRequestException: Connection refused (localhost:5206)
  Start it with: dotnet run --project Asana.API
  Or point this client elsewhere with ASANA_API_URL.
```

## API

Swagger UI is served at `/swagger` when running in Development.

| Method | Route | Description |
| --- | --- | --- |
| GET | `/ToDo` | All ToDos |
| GET | `/ToDo/{id}` | One ToDo |
| POST | `/ToDo` | Create or update, keyed on `id` (0 creates) |
| DELETE | `/ToDo/{id}` | Delete, returns the removed ToDo |
| GET | `/Project` | All Projects |
| GET | `/Project/{id}` | One Project |
| POST | `/Project` | Create or update |
| DELETE | `/Project/{id}` | Delete |

```bash
curl -X POST http://localhost:5206/ToDo \
  -H 'Content-Type: application/json' \
  -d '{"name":"Write docs","description":"README pass","isCompleted":false}'

curl http://localhost:5206/ToDo
```

## Layout

```
Asana.API/         ASP.NET Core Web API
  Controllers/     HTTP endpoints for ToDo and Project
  Enterprise/      business layer between controllers and storage
  Database/        JSON file stores, one file per entity, plus FileStorage
Asana.Library/     shared across every front end
  Models/          ToDo and Project
  Services/        singleton proxies the front ends bind to
  Util/            HTTP client and JSON helpers
Asana.CLI/         console front end
Asana.Maui/        cross-platform front end (MVVM, XAML views)
```

**Storage.** Each ToDo and Project is a JSON file named for its id, under
`ToDos/` and `Projects/`. `FileStorage` resolves the root at startup and creates
the folders, so a first run works on a clean machine.

**Service proxies.** `ToDoServiceProxy` and `ProjectServiceProxy` are lazy
singletons guarded by a lock. They cache a local list, push changes to the API,
and reconcile the response back into that list, so both front ends see the same
state without duplicating HTTP code.
