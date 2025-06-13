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


