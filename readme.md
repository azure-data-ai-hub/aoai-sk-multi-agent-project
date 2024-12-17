I will now add this `README.md` file to your repository.Here's a comprehensive `README.md` file for your project:

### README.md

```markdown
# Multi-Agent Project

This project consists of a ReactJS frontend and a C# WebAPI backend. The backend utilizes Microsoft Semantic Kernel and Azure OpenAI services to provide multiple agent interactions.

## Project Structure

- **frontend/**: Contains the ReactJS frontend application.
- **MultiAgentWebAPI/**: Contains the C# WebAPI backend application.

## ReactJS Frontend

This project was bootstrapped with [Create React App](https://github.com/facebook/create-react-app).

### Available Scripts

In the frontend directory, you can run:

- `npm start`: Runs the app in development mode.
- `npm test`: Launches the test runner.
- `npm run build`: Builds the app for production.
- `npm run eject`: Ejects the app, giving full control over the configuration.

## C# WebAPI Backend

The backend is built with ASP.NET Core and integrates with Azure OpenAI and the Microsoft Semantic Kernel.

### Key Features

- **CosmosDB Integration**: Utilizes Azure CosmosDB for data storage.
- **OpenAI Integration**: Integrates with Azure OpenAI for chat and agent functionalities.
- **Swagger**: Provides API documentation and testing through Swagger UI.

### Key Endpoints

- `GET /`: Welcome message for the API.
- `POST /ProjectManagerAgentChat`: Interact with the Project Manager Agent.
- `POST /ScheduleAgentChat`: Interact with the Schedule Agent.
- `POST /FinanaceAgentChat`: Interact with the Finance Agent.
- `POST /MultiAgentChat`: Multi-agent chat interaction.
- `POST /Chat`: General chat endpoint.
- `GET /Vectorize`: Vectorization endpoint.
- `POST /VectorSearch`: Vector search endpoint.
- `POST /MaintenanceCopilotChat`: Chat with the Maintenance Copilot.

### Setting Up Dotnet Secrets
dotnet user-secrets init
dotnet user-secrets set "AzureOpenAI:ApiKey" "<your-api-key>"
dotnet user-secrets set "AzureOpenAI:EndPoint" "<your-endpoint>"
dotnet user-secrets set "AzureOpenAI:DeploymentName" "<your-deployment>"

### Running the Backend

1. **Build the project**:
   ```sh
   dotnet build
2. **Rune the project**:
   ```sh
   dotnet run



