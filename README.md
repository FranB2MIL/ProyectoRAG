# Destiny Loremaster RAG

A Retrieval-Augmented Generation (RAG) pipeline built with .NET 9 and Clean Architecture. It indexes the *Destiny* lore timeline from a structured `.docx` document and answers natural-language questions about it using semantic search + Claude — like having a loremaster who actually read every Grimoire card.

This started as a generic RAG learning project (originally tested with a vehicle catalog from Excel) and evolved into something more interesting: narrative, unstructured text is where RAG actually earns its keep, instead of competing with a simple SQL query.

## How it works

1. **Parse** — a `.docx` file is read section by section. Heading styles (`Heading1`, `Heading2`...) define lore eras/sections, and italicized sub-topics within paragraphs (e.g. *"Rhulk and the Ahslid:"*) define finer-grained entries. This keeps each indexed chunk thematically focused instead of mixing multiple topics together.
2. **Index** — each entry is sent to Voyage AI to generate an embedding (batched in a single request), then stored in PostgreSQL with pgvector, tagged with its section/sub-topic as metadata.
3. **Retrieve** — a question is embedded the same way, and pgvector returns the most semantically similar entries using cosine distance.
4. **Generate** — the retrieved entries are passed to the Anthropic API as context, and Claude generates a natural-language answer grounded in that lore.

```
Question → Voyage AI (embedding) → pgvector (similarity search) → Anthropic API (answer)
```

## Why narrative text instead of tabular data

RAG shines when answers require semantic understanding of unstructured text — nuance, relationships between entities, conceptual comparisons. Structured data (rows and columns, like a vehicle catalog) is usually better served by a direct SQL query: exact, fast, and free of the precision loss that comes from compressing rows into chunks of vector-searchable text. A 200+ page lore timeline, full of characters, philosophies, and interconnected events, is a much better fit for what RAG is actually good at.

## Tech stack

- **.NET 9** — Clean Architecture (Domain / Application / Infrastructure / WebApi)
- **Voyage AI** — text embeddings (`voyage-3`, 1024 dimensions)
- **PostgreSQL + pgvector** — vector storage and cosine similarity search
- **Anthropic API** — final answer generation (`claude-sonnet-4-6`)
- **Npgsql + Pgvector.NET** — database access from C#
- **DocumentFormat.OpenXml** — parsing `.docx` structure (headings, italics) for content-aware chunking
- **Docker + Docker Compose** — the entire stack (API + database) runs in containers

## Architecture

The project follows Clean Architecture. Dependencies only point inward — `Infrastructure` and `WebApi` depend on `Application`, but `Application` and `Domain` know nothing about Voyage AI, pgvector, or Anthropic.

```
ProyectoRAG/
├── ProyectoRAG.Domain/          → core entities (LoreEntry), no external dependencies
├── ProyectoRAG.Application/     → interfaces (IEmbeddingService, IDocumentRepository, IChatService, IDocxReader)
├── ProyectoRAG.Infrastructure/  → implementations (Voyage AI, pgvector, Anthropic, OpenXml docx parser)
└── ProyectoRAG.WebApi/          → HTTP endpoints, dependency injection setup
```

This means any provider can be swapped without touching business logic — e.g. replacing Voyage AI with OpenAI embeddings only requires a new `Infrastructure` implementation of `IEmbeddingService`.

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- A [Voyage AI](https://dash.voyageai.com) API key (free tier available)
- An [Anthropic API](https://platform.claude.com) key

## Setup

### Option A — Docker Compose (recommended)

This runs the entire stack — API and database — with a single command. No need to install .NET or PostgreSQL locally.

**1. Clone the repo**

```bash
git clone https://github.com/your-username/ProyectoRAG.git
cd ProyectoRAG
```

**2. Create a `.env` file** in the project root with your API keys:

```env
VOYAGE_API_KEY=your-voyage-api-key
ANTHROPIC_API_KEY=your-anthropic-api-key
```

**3. Start everything**

```bash
docker compose up --build
```

The API will be available at `http://localhost:5218`. The database schema (pgvector extension + `documents` table) needs to be created once — see step 4 below.

**4. Create the schema** (first run only)

```bash
docker exec -it proyectorag-postgres psql -U postgres -d proyectorag
```

```sql
CREATE EXTENSION IF NOT EXISTS vector;

CREATE TABLE documents (
    id SERIAL PRIMARY KEY,
    content TEXT NOT NULL,
    embedding vector(1024)
);
```

### Option B — Manual setup (without Docker for the API)

If you prefer running the WebApi directly with `dotnet run` while still using Docker for the database:

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

### Import lore from a `.docx` file

```http
POST /api/documents/import-docx
Content-Type: multipart/form-data

file: <your-lore-file.docx>
```

Parses the document by heading structure and italicized sub-topics, generates embeddings for all entries in a single batched request, and indexes them.

```json
{ "message": "8 lore entries indexed successfully" }
```

### Search for similar content

```http
POST /api/documents/search
Content-Type: application/json

{ "query": "What is the Witness's philosophy?", "topK": 3 }
```

Returns the `topK` most semantically similar stored entries, ranked by relevance. No LLM call involved — pure vector search.

### Ask a question (full RAG pipeline)

```http
POST /api/documents/ask
Content-Type: application/json

{ "query": "What is the Winnower's argument about morality?" }
```

Embeds the question, retrieves relevant lore context from pgvector, and asks Claude to answer based on that context:

```json
{ "answer": "Based on the provided context, the Winnower's argument about morality is that if moral good is not suffering and moral bad is suffering, then it is a moral imperative that death exists to remove all except those who must live..." }
```

If the question falls outside the indexed lore, Claude says so explicitly instead of guessing — verified by asking about content that was never indexed.

## Project status

This is a learning / portfolio project demonstrating an end-to-end RAG pipeline with proper architectural separation between providers and business logic, currently being rebuilt around narrative content instead of tabular data.

**Completed so far:** embeddings, vector search, LLM-generated answers, structure-aware `.docx` chunking (headings + sub-topics), full containerization with Docker Compose. Validated with a sample of the Destiny lore timeline — confirmed accurate retrieval, multi-chunk synthesis on comparative questions, and honest "I don't have that information" responses when content isn't indexed.

**In progress — turning this into a full Destiny Loremaster app:**
1. **Personality** — give the assistant a loremaster/professor voice instead of a generic Q&A tone
2. **Scale the index** — batch-import the full 200+ page lore timeline (current `.docx` import works, but needs batching for documents this large) and filter out non-content entries (titles, table of contents)
3. **Backend readiness** — configure CORS so a separate frontend can call the API
4. **React frontend** — a chat interface with Destiny-inspired visual identity
5. **Full containerization** — add the frontend to `docker-compose.yml` alongside the API and database

**Other possible directions:**
- A router that decides between semantic search (RAG) and structured queries (Text-to-SQL) depending on the question type
- Hybrid search (semantic + BM25) to improve precision on proper nouns (character names, item names)
- Deploying the containerized stack to a cloud provider (Azure, AWS)

## License

MIT