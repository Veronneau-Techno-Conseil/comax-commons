# Commun Axiom <img src="../../CommunAxiom.png" style="height: 1em" /> - Grain Storage Service

## In this project
This project is a web api storage layer for the custom ApiStorageProvider used to provide a simple alternate storage strategy. In it's current state, the application runs alongside a mariadb deployment. In the future, we aim to add a multi-master capability using Raft algorithm provided by dotNext. 

To run the application in dev mode:
- Ensure the code has been built with the DEBUG symbol as this will include automatic database provisioning and launch on docker.
- You must have a docker engine running and accessible from the executing user
- Make sure you have a valid OIDC configuration set and leave DbConfig with default value, the application will use those defaults to provision mariadb automatically on docker. Note that automatic provisioning only work in debug. 

