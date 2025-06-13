# RaftLabsTest
RaftLabsTest
│
├── RaftLabsTest.Application       # Service contracts/interfaces
├── RaftLabsTest.Infrastructure    # API client/service implementation
├── RaftLabsTest.Models            # DTOs and internal data models
├── RaftLabsTest.ConsoleApp        # Console app for demonstration
└── RaftLabsTest.Tests             # Unit tests for service logic

**Features**
* Fetch all users (handles pagination)
* Fetch single user by ID
* Async/Await and HttpClientFactory usage
* Centralized API base URL via appsettings.json
* Robust error handling (timeouts, not found, etc.)
* DTOs for structured API response mapping
* Unit testing using xUnit (or your preferred framework)
* Optional (Bonus):
    => In-memory caching using IMemoryCache
    => Retry policy support via Polly (to be optionally discussed/added)


## How To RUN
**Clone the Repository**

git clone https://github.com/SandeshNarvariyaOfficial/RaftLabsTest.git
cd RaftLabsTest

**Build the Solution**
dotnet build

**Run the Console App**
cd RaftLabsTest.ConsoleApp
dotnet run

**Running Tests**
cd RaftLabsTest.Tests
dotnet test



📁 ## Key Components
**Project	Purpose**
RaftLabsTest.Application:	Contains IExternalUserService interface
RaftLabsTest.Infrastructure:	Implements the API logic (ExternalUserService)
RaftLabsTest.Models:	DTOs for user and API responses
RaftLabsTest.ConsoleApp:	Sample CLI client to demonstrate the service
RaftLabsTest.Tests: 	Unit tests validating logic and behavior

![alt text](image.png)
