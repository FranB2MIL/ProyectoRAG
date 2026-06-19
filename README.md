# ProyectoRAG

A Retrieval-Augmented Generation (RAG) pipeline built with .NET 9 and Clean Architecture. It indexes text into a vector database and answers natural-language questions about that data using semantic search + Claude.

## How it works

1. **Index** — text is sent to Voyage AI to generate an embedding, then stored in PostgreSQL with pgvector.
2. **Retrieve** — a question is embedded the same way, and pgvector returns the most semantically similar chunks using cosine distance.
3. **Generate** — the retrieved chunks are passed to the Anthropic API as context, and Claude generates a natural-language answer grounded in that data.

```
Question → Voyage AI (embedding) → pgvector (similarity search) → Anthropic API (answer)
```

## Tech stack

- **.NET 9** — Clean Architecture (Domain / Application / Infrastructure / WebApi)
- **Voyage AI** — text embeddings (`voyage-3`, 1024 dimensions)
- **PostgreSQL + pgvector** — vector storage and cosine similarity search, running in Docker
- **Anthropic API** — final answer generation (`claude-sonnet-4-6`)
- **Npgsql + Pgvector.NET** — database access from C#

## Architecture

The project follows Clean Architecture. Dependencies only point inward — `Infrastructure` and `WebApi` depend on `Application`, but `Application` and `Domain` know nothing about Voyage AI, pgvector, or Anthropic.

```
ProyectoRAG/
├── ProyectoRAG.Domain/          → core entities, no external dependencies
├── ProyectoRAG.Application/     → interfaces (IEmbeddingService, IDocumentRepository, IChatService)
├── ProyectoRAG.Infrastructure/  → implementations (Voyage AI, pgvector, Anthropic clients)
└── ProyectoRAG.WebApi/          → HTTP endpoints, dependency injection setup
```

This means any provider can be swapped without touching business logic — e.g. replacing Voyage AI with OpenAI embeddings only requires a new `Infrastructure` implementation of `IEmbeddingService`.

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- A [Voyage AI](https://dash.voyageai.com) API key (free tier available)
- An [Anthropic API](https://platform.claude.com) key

## Setup

**1. Clone and restore**

```bash
git clone https://github.com/your-username/ProyectoRAG.git
cd ProyectoRAG
dotnet restore
```

**2. Start PostgreSQL with pgvector**

```bash
docker run -d \
  --name pgvector-dev \
  -e POSTGRES_PASSWORD=postgres \
  -e POSTGRES_DB=proyectorag \
  -p 5433:5432 \
  pgvector/pgvector:pg16
```

**3. Create the schema**

```bash
docker exec -it pgvector-dev psql -U postgres -d proyectorag
```

```sql
CREATE EXTENSION IF NOT EXISTS vector;

CREATE TABLE documents (
    id SERIAL PRIMARY KEY,
    content TEXT NOT NULL,
    embedding vector(1024)
);
```

**4. Configure secrets**

From `ProyectoRAG.WebApi`:

```bash
dotnet user-secrets set "VoyageAI:ApiKey" "your-voyage-api-key"
dotnet user-secrets set "Anthropic:ApiKey" "your-anthropic-api-key"
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5433;Database=proyectorag;Username=postgres;Password=postgres"
```

**5. Run**

```bash
cd ProyectoRAG.WebApi
dotnet run
```

The API will be available at `http://localhost:5218`.

## API reference

### Index a document

```http
POST /api/documents/index
Content-Type: application/json

{ "content": "The premium plan costs $50 per month." }
```

Generates an embedding for the text and stores it in pgvector.

### Search for similar content

```http
POST /api/documents/search
Content-Type: application/json

{ "query": "How much does the premium plan cost?", "topK": 3 }
```

Returns the `topK` most semantically similar stored documents, ranked by relevance. No LLM call involved — pure vector search.

### Ask a question (full RAG pipeline)

```http
POST /api/documents/ask
Content-Type: application/json

{ "query": "How much does the premium plan cost?" }
```

Embeds the question, retrieves relevant context from pgvector, and asks Claude to answer based on that context:

```json
{ "answer": "Based on the provided context, the premium plan costs $50 per month." }
```

## Project status

This is a learning / portfolio project demonstrating an end-to-end RAG pipeline with proper architectural separation between providers and business logic. Possible next steps:
- Importing tabular data (CSV/Excel) as a bulk source for indexing
- A React frontend for interacting with the API visually
- Containerizing the WebApi itself (not just the database)

## License

MIT