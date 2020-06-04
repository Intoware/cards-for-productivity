# Cards For Productivity

Cards For Productivity is an open-source application that allows sprint teams to easily select story points for a set of stories.

## .NET Project

The below assumes that you are in the `CardsForProductivity.API` directory.

### Setup

You will need to set the appsettings.json (or appsettings.Development.json) file to suit your environment. Below is a table containing each of the settings you will need to set.

| Setting                   | Description                                                                                                                          | Example                   |
|---------------------------|--------------------------------------------------------------------------------------------------------------------------------------|---------------------------|
| ConnectionStrings.MongoDB | The connection string for MongoDB - used for storing data.                                                                           | mongodb://localhost:27017 |
| Storage.DatabaseName      | The name of the Mongo database.                                                                                                      | CardsForProductivity      |
| Domain                    | The location of the front-end, used for setting the allowed CORS origin. For development, this will likely be http://localhost:4200. | http://localhost:4200     |

### Development server

Run `dotnet run` on your local machine for a dev server.

## Web App

This project was generated with [Angular CLI](https://github.com/angular/angular-cli) version 9.1.7.

The below assumes that you are in the `CardsForProductivity.WebApp` directory.

### Setup

Set the `api` environment variable to the base URL of the API (e.g. http://localhost:5000).

### Development server

Run `ng serve` for a dev server. Navigate to `http://localhost:4200/`. The app will automatically reload if you change any of the source files.

### Build

Run `ng build` to build the project. The build artifacts will be stored in the `dist/` directory. Use the `--prod` flag for a production build.

### Further help

To get more help on the Angular CLI use `ng help` or go check out the [Angular CLI README](https://github.com/angular/angular-cli/blob/master/README.md).
