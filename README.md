# RemoteControl

A remote control / screen sharing solution.  
(Built with modular components based on C# and .NET)

## Table of Contents

- [About](#about)  
- [Architecture / Components](#architecture--components)  
- [Features](#features)  
- [Setup & Run](#setup--run)  
- [Usage Example](#usage-example)  
- [Contributing](#contributing)  
- [License](#license)

## About

`RemoteControl` is a modular system designed to perform remote desktop control, screen capturing, or similar operations.  
The project includes the following submodules:  
- RemoteControl.CaptureService  
- RemoteControl.IntermediateServer  
- RemoteControl.Shared  
- RemoteControl.WindowsClient  

This architecture separates the client, server, and intermediate layers, allowing them to operate independently.

## Architecture / Components

| Module | Role / Description |
|---|---|
| CaptureService | Handles screen capture, compression, and encoding operations |
| IntermediateServer | Acts as a relay server managing communication between clients and servers |
| Shared | Contains common data models, protocols, and helper classes |
| WindowsClient | Windows client application — includes the user interface and control logic |

The data communication protocol is defined mainly in the Shared project via model and message structures.

## Features

- Screen capture and frame transmission  
- Real-time control commands and interaction  
- Modular, layered architecture  
- Abstracted client ↔ server communication protocol (easily replaceable)  
- Windows client application (GUI)  

## Setup & Run

To run the project locally, follow these steps:

1. Clone this repository:  
   ```bash
   git clone https://github.com/mguler/RemoteControl.git
Open the solution file (RemoteControl.sln) in an IDE (e.g., Visual Studio)

Restore all required NuGet packages

Set startup projects:

IntermediateServer — server role

CaptureService — screen capture service

WindowsClient — client user interface

Run each project (first the server, then the service, and finally the client)

Connect to the server from the client interface and start control

Note: Connection settings (IP, port, etc.) may be defined in configuration files. Update them as needed.

Usage Example
Example usage flow:

Run the server application.

Start the CaptureService and establish a connection to the server.

Open the WindowsClient, enter the server’s IP and port information.

Click the “Connect” button — the client will start receiving the screen feed and sending control commands.

Contributing
Fork the repository

Create a new feature or fix branch (feature/new-feature)

Make your changes and commit them

Open a pull request — remember to include a detailed explanation

Please adhere to coding standards, maintain test coverage, and ensure proper documentation.

License
This project is licensed under the terms described in the LICENSE.txt file.
(See the original license file for details.)
