# Commun Axiom <img src="../../CommunAxiom.png" style="height: 1em" /> - Commons Client <img src="../../ComaxSvc.png" style="height: 1em" /> 

## In this project
This project implements the business logic layer of the Commons Client. This component is referred to as the **Agent** because it runs continuously even when the UI application is shut down. The purpose of the agent is to automatically execute operations based on configurations provided by the user using the [Commons Client](../Client/). Since this application executes most of the client's work, it may be required for more extensive deployments to require more than one node of the agent running in parallel. For a detailed description of functionality, see the documentation website at [https://communaxiom.org/en/commons](https://communaxiom.org/en/commons).


## How to run
There are two ways to run the agent. 

1. In local mode (recommended for testing) <br/>
This mode is intended for development and testing purposes. It assumes that the all components are preconfigured to communicate locally together and bypasses the requirement of the Referee application and specific IP address configurations.
    1. Ensure the project is set to run in local mode. You can do this by ensuring the value of the following property: `"advertisedIp": "127.0.0.1"`. Hardcoding the advertised ip to the loopback address removes the logic around AgentReferee usage.
    


