


## 📦 Technologies Used

| Component              | Tech                          |
|------------------------|-------------------------------|
| Framework              | .NET 8            |
| HTTP Client            | `HttpClient` with `IHttpClientFactory` |
| Caching                | `EF Core` with InMemory provider |
| Testing                | `xUnit`, `Moq`, `EF InMemory` |
| External API           | [https://reqres.in](https://reqres.in) |




## 📌 Features

- ✅ Fetch user by ID
- ✅ Fetch all users across paginated API
- ✅ Caches results in-memory
- ✅ Converts API models to DTOs
- ✅ Handles errors: 404, 500, deserialization issues
- ✅ Unit tests for success & failure flows


- ## 🛠️ Setup Instructions

1. **Clone Repo**
https://github.com/Niyas-Datum/RafLab.git

2. **Install Dependencies**

              dotnet restore
3. **Run Tests**

             dotnet test
4. **Run project**

              dotnet run --project RafLab.api 


- ## 📌 Configuration

            {
                  "ApiSettings": {
                  "BaseUrl": "https://reqres.in/api/",
                  "ApiKey": "reqres-free-v1"
                  }
              }

     **Add Header**

                    httpClient.DefaultRequestHeaders.Add("x-api-key", "reqres-free-v1");






