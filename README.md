# IntelligenceQueryEngine

This is a REST API that transforms demographic profile data into a fully queryable intelligence system. Supports advanced filtering, sorting, pagination, and natural language search (e.g., "young males from nigeria"). Built with ASP.NET Core 9.0 and SQLite. Enables marketing teams and analysts to extract insights without writing complex database queries.

# Intelligence Query Engine

A REST API for demographic intelligence with advanced filtering, sorting, pagination, and natural language search.

## Live URL

`https://your-app-url.onrender.com` (Replace with your actual deployed URL)

## Clone & Run

```bash
git clone https://github.com/Precious-Adeoye/IntelligenceQueryEngine.git
cd intelligence-query-engine
dotnet restore
dotnet build
dotnet run
````
## API Endpoints
Method	Endpoint	Description
GET	/api/profiles	Filter, sort, paginate profiles
GET	/api/profiles/search?q=	Natural language query

## Natural Language Parsing
The parser uses regex pattern matching (no AI). Keywords map to filters:

young → ages 16-24
male/female → gender filter

adult/teenager/child/senior → age_group filter

from [country] → country filter

above/below [age] → min_age/max_age filter

## Limitations
No compound ranges (e.g., "between 20 and 30")

No negations (e.g., "not from nigeria")

No multiple countries

Misspelled words return error

## Technologies
ASP.NET Core 9.0

SQLite

Entity Framework Core

