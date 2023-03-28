# Commun Axiom <img src="../../CommunAxiom.png" style="height: 1em" /> - Referee

## In this project
This project is a an api service providing information regarding orleans cluster memberships. Its purpose is to provide cluster clients and nodes with membership information to allow chatter protocol and cluster connectivity. 


## How to run
We are planning on setting up automated provisioning on this application for the mongodb storage but this isn't done yet. To run:
1. Provide a mongodb instance first and enter the credentials in the configuration under member_mongo. 
2. Set up OIDC configurations so that authentication can go through.
3. Launch the application using `dotnet run`
