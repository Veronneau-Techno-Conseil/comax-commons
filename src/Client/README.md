# Commun Axiom <img src="../../CommunAxiom.png" style="height: 1em" /> - Commons Client <img src="../../Commons.png" style="height: 1em" /> 

## In this project
This project implements the presentation layer for the Commons Client. It cannot run by itself as it needs to authenticate against [accounts.communaxiom.org](accounts.communaxiom.org) and connects with [Commons Agent &nbsp;&nbsp;<img src="../../ComaxSvc.png" style="height: 1em" />](../Silo). Client app essentially holds the server portion of the application logic in the form of api controllers. It serves a [Blazer](https://dotnet.microsoft.com/en-us/apps/aspnet/web-apps/blazor) frontend through webassembly using [ClientUI.Components](../ClientUI.Components/) and [ClientUI.Shared](../ClientUI.Shared/). For a detailed description of functionality, see the documentation website at [https://communaxiom.org/en/commons](https://communaxiom.org/en/commons).

## How to run
There are two ways to run the client. 

1. In local mode (recommended for testing) <br/>
This mode is intended for development and testing purposes. It assumes that the [Commons Agent](../Silo) also runs in local mode on the same machine so that all requests are directed at localhost.
    1. Make sure that the appsettings are set to `"client_mode": "local"`
    2. Ensure that the `OIDC.AUTHORITY` points to a valid online server
    3. Launch the [Commons Agent](../Silo) first, see [README.md](../Silo/README.md) for details
    4. Launch the client using either Visual Studio debug of dotnet run in the current folder
    5. Open the page under the specified port. You should get redirected towards the login page and then the authentication portal. 

2. In cluster mode <br/>
This mode is used when the application is deployed. [Commons Agent](../Silo) is built to support horizontal scalability through load distribution and resilience to node failure. In that aspect, an aditionnal application is required to keep track of the agents running in parallel.
    1. Make sure that the appsettings are set to `"client_mode": "remote"`
    2. Ensure that the `OIDC.AUTHORITY` points to a valid online server
    3. Launch the [Commons Agent](../Silo) first, see [README.md](../Silo/README.md) for details, keep in mind that you should follow instructions to run in remote mode and not in local
    4. Ensure the client `CommonsMembershipt.Host` points to the right address and port
    5. Launch the client using either Visual Studio debug of dotnet run in the current folder
    6. Open the page under the specified port. You should get redirected towards the login page and then the authentication portal.