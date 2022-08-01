# Demo Client For Positions Service

## Pre-requisites
- Active Azure subscription
- Docker Desktop
- Workstation running Docker Desktop should have access to MandaraProducts database

## Steps to configure Positions Service

### Create Azure App Configuration to store configuration for the Positions Service. The configuration needs to have the following keys:
- ConnectionStrings:MandaraEntities. This should be set to the connection string for the MandaraProducts database.
- ServerCertificatePassword. Set this one to "password1234".

Write down the connection string to the App Configuration.

### Login to Azure container registry
```
docker login mandara.azurecr.io
```
Use the provided credentials.

### Run the container
```
docker run -d -p <INCOMING_PORT>:443 -e "ConnectionStrings:AppConfig=<APP-CONFIGURATION-CONNECTION-STRING>" mandara.azurecr.io/positionsservice:1.0.1
```

The steps above should configure and run the Positions Service inside a Docker container. Next, use the sample code from this repository to connect to the service and subscribe to position updates.
