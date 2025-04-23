# Documentation
Project documentation (work in progress)

# Dependencies
The project uses Python.NET. Download and install this with:\
`dotnet add package Python.Runtime`

# Build & Run
**On Windows:**\
Open a project after starting Visual Studio and locate the `.csproj` file within the project directory.\

**On Linux:**\
After cloning the repo, just edit the `.cs` files with whatever editor, build with `dotnet build` and run with `dotnet run` (y>

**NOTE**:\
The project runs on .NET Core 9, specifically (as of writing this) 9.0.102.\
If you do not have this version of .NET Core then your solution would be to update visual studio.

The dotnet sdk (linux) should be updated if you use arch.

# System Architecture
The current setup uses the following diagram for reference in how the data structure is designed:
[!structure-diagram](https://private-user-images.githubusercontent.com/58586628/431741860-d6c281a6-e7ff-498f-82c5-bd568e156a5d.png?jwt=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJnaXRodWIuY29tIiwiYXVkIjoicmF3LmdpdGh1YnVzZXJjb250ZW50LmNvbSIsImtleSI6ImtleTUiLCJleHAiOjE3NDUzOTMyODksIm5iZiI6MTc0NTM5Mjk4OSwicGF0aCI6Ii81ODU4NjYyOC80MzE3NDE4NjAtZDZjMjgxYTYtZTdmZi00OThmLTgyYzUtYmQ1NjhlMTU2YTVkLnBuZz9YLUFtei1BbGdvcml0aG09QVdTNC1ITUFDLVNIQTI1NiZYLUFtei1DcmVkZW50aWFsPUFLSUFWQ09EWUxTQTUzUFFLNFpBJTJGMjAyNTA0MjMlMkZ1cy1lYXN0LTElMkZzMyUyRmF3czRfcmVxdWVzdCZYLUFtei1EYXRlPTIwMjUwNDIzVDA3MjMwOVomWC1BbXotRXhwaXJlcz0zMDAmWC1BbXotU2lnbmF0dXJlPTBhNmZlNzljNDA5YWQ5OWY1NTdlZTg3YTkwMGQ0NmMwZWNkYTI1M2VkM2QxMDU0ZDRjZjFiNTA1MmE3ZDczNjcmWC1BbXotU2lnbmVkSGVhZGVycz1ob3N0In0.1TPGFu9h-RrtJ-OiwahF_7minrKOjF_PiY646b3kHvU)
