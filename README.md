# Azure-ChatFlow-Server

![.NET](https://img.shields.io/badge/.NET-512BD4?style=flat&logo=dotnet&logoColor=white)
![Azure Functions](https://img.shields.io/badge/Azure%20Functions-0062B1?style=flat&logo=microsoft-azure&logoColor=white)
![Azure SignalR](https://img.shields.io/badge/Azure%20SignalR-0078D4?style=flat&logo=microsoft-azure&logoColor=white)
![Azure Table Storage](https://img.shields.io/badge/Azure%20Table%20Storage-0078D4?style=flat&logo=microsoft-azure&logoColor=white).
![GitHub](https://img.shields.io/badge/GitHub-181717?style=flat&logo=github&logoColor=white)

🚧 **UNDER DEVELOPMENT** 🚧  

Backend for a real-time chat application built with Azure Functions and Azure SignalR Service. This project serves as the server-side component of a chat system, handling message broadcasting and client connections in real-time. It integrates with the [Azure-ChatFlow-Client](https://github.com/arasrasouli/azure-chat-flow-client), a Vue.js/Nuxt.js frontend, to provide a seamless chat experience.

## Project Overview
![image](https://github.com/user-attachments/assets/8b7be52e-0d74-4c59-97d5-88b948fc1dea)

`Azure-ChatFlow-Server` is a serverless backend leveraging Azure Functions to process HTTP requests and Azure SignalR Service for real-time WebSocket communication. The primary functions include:

- **Negotiate**: Provides connection information for clients to connect to the SignalR hub.
- **SendMessage**: Accepts chat messages from clients and broadcasts them to all connected users via the `chatHub`.

This project is designed to be scalable, leveraging Azure’s serverless architecture, and serves as the foundation for a modern chat application.

### Related Repository
- **[Azure-ChatFlow-Client](https://github.com/arasrasouli/azure-chat-flow-client)**: The frontend counterpart built with Vue.js and Nuxt.js, connecting to this backend for real-time chat functionality.

## 🛠 Technologies Used

- **.NET**: Core framework for building the Azure Functions app.
- **Azure Functions**: Serverless compute service hosting the API endpoints.
- **Azure SignalR Service**: Managed service for real-time messaging and WebSocket communication.
- **Azure Table Storage**: NoSQL key-value store used to persist chat history efficiently.


## 🌎 Environment Variables

📄 The environment variables are defined in  
`Azure/ChatFlow.Functions/local.settings.example.json`.

Make sure to configure them correctly for your local development and deployment.
