# Commun Axiom <img src="../../CommunAxiom.png" style="height: 1em" /> - Commons Client <img src="../../ComaxSvc.png" style="height: 1em" /> 

## In this project
This project implements the business logic layer of the Commons Client. This component is referred to as the **Agent** because it runs continuously even when the UI application is shut down. The purpose of the agent is to automatically execute operations based on configurations provided by the user using the [Commons Client](../Client/). Since this application executes most of the client's work, it may be required for more extensive deployments to require more than one node of the agent running in parallel. For a detailed description of functionality, see the documentation website at [https://communaxiom.org/en/commons](https://communaxiom.org/en/commons).


## How to run
There are two ways to run the agent. 

1. In local mode (recommended for testing) <br/>
This mode is intended for development and testing purposes. It assumes that the all components are preconfigured to communicate locally together and bypasses the requirement of the Referee application and specific IP address configurations.
    1. Ensure the project is set to run in local mode. You can do this by ensuring the value of the following property: `"advertisedIp": "127.0.0.1"`. Hardcoding the advertised ip to the loopback address removes the logic around AgentReferee usage.
    2. You can either make sure your you have `"storage": "litedb"` or have it at "api" and make sure your this bloc is set: 
        ``` json
        "ApiStorage": {
            "SenderName": "CommonsAgent",
            "ApiStorageUri": "https://localhost:7066",
            "SerializationProvider": "standard",
            "SerializationConfig": ""
        }
        ```
    3. If your storage mode is api, make sure that you follow proper instructions to run [Grain storage service](../GrainStorageService/).
    4. Run the application through `dotnet run` or using your preferred dotnet debugger.
2. In cluster mode <br/>
This mode is designed to simulate a production environment when running the orchestrator. In this mode, the system assumes that there might be more than one isntance of the application running in parallel and requires a third party to supervise the cluster. This third party is called the Referee.
    1. Follow instructions at [Referee](../Referee/) to launch the application and make it visible to the orchestrator. 
    2. Ensure the project is set to run in cluster mode. You can do this by changing the value of the following property to your local ip address: `"advertisedIp": "127.0.0.1"`. 
    3. Ensure that the membership configuration section points to the Referee instance lanched at 1.
    4. You can either make sure your you have `"storage": "litedb"` or have it at "api" and make sure your this bloc is set: 
        ``` json
        "ApiStorage": {
            "SenderName": "CommonsAgent",
            "ApiStorageUri": "https://localhost:7066",
            "SerializationProvider": "standard",
            "SerializationConfig": ""
        }
        ```
    5. If your storage mode is api, make sure that you follow proper instructions to run [Grain storage service](../GrainStorageService/).
    6. Run the application through `dotnet run` or using your preferred dotnet debugger.


