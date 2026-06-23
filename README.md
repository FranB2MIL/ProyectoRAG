# Grimoires of Sol

A full-stack Destiny lore assistant powered by a RAG (Retrieval-Augmented Generation) pipeline. Ask anything about the Destiny universe and receive answers from a Lightbearer who has spent decades studying the forgotten corners of this system's history.

Built with .NET 9, React, PostgreSQL + pgvector, Voyage AI, and the Anthropic API — fully containerized with Docker Compose.

---

## How it works

1. **Parse** — a `.docx` lore timeline is read section by section. Heading styles define eras and sections; italicized sub-topics within paragraphs define finer-grained entries. Each chunk stays thematically focused.
2. **Index** — entries are sent to Voyage AI in batches to generate embeddings (1024 dimensions), then stored in PostgreSQL with pgvector.
3. **Retrieve** — when a question arrives, it's rewritten into 3 alternative queries by Claude Haiku, then all 4 queries are embedded and searched in parallel. Results are fused using Reciprocal Rank Fusion (RRF) combining both semantic (vector) and lexical (BM25) search.
4. **Generate** — the retrieved entries are passed to Claude Sonnet as context, and the Lightbearer persona formulates a narrative answer grounded in that lore.

```
Question → Query Rewriting (Haiku) → Voyage AI (4x embeddings)
        → pgvector (semantic) + PostgreSQL (BM25) → RRF fusion
        → Anthropic API (Sonnet) → answer
```

---

## Why narrative text instead of tabular data

RAG shines on unstructured narrative content — nuance, relationships between entities, conceptual comparisons. A 200+ page lore timeline full of characters, philosophies, and interconnected events across billions of years is a much better fit than tabular data, which is better served by direct SQL.

---

## Tech stack

**Backend**
- .NET 9 — Clean Architecture (Domain / Application / Infrastructure / WebApi)
- Voyage AI — text embeddings (`voyage-3`, 1024 dimensions)
- PostgreSQL + pgvector — vector storage and cosine similarity search
- Full-text search — PostgreSQL `tsvector` + GIN index for BM25 lexical search
- Anthropic API — query rewriting (`claude-haiku-4-5`) + answer generation (`claude-sonnet-4-6`)
- DocumentFormat.OpenXml — structure-aware `.docx` parsing (headings + italic sub-topics)
- AspNetCoreRateLimit — IP-based rate limiting (5 requests/min, 30/hour)

**Frontend**
- React + Vite — chat interface with landing page and routing
- nginx — serves the frontend and proxies `/api/*` to the backend

**Infrastructure**
- Docker + Docker Compose — entire stack (frontend + API + database) runs in 3 containers
- nginx reverse proxy — single origin for frontend and API, production-ready

---

## Architecture

```
ProyectoRAG/
├── ProyectoRAG.Domain/          → LoreEntry entity
├── ProyectoRAG.Application/     → interfaces (IEmbeddingService, IDocumentRepository,
│                                   IChatService, IDocxReader, IQueryRewritingService)
├── ProyectoRAG.Infrastructure/  → Voyage AI, pgvector, Anthropic, OpenXml parser
├── ProyectoRAG.WebApi/          → HTTP endpoints, DI setup, rate limiting
└── frontend/                    → React app + nginx config + Dockerfile
    ├── src/
    │   ├── pages/               → LandingPage, ChatPage
    │   └── components/          → Header, ChatWindow, Message, ChatInput
    ├── Dockerfile
    └── nginx.conf
```

---

## Prerequisites

- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- A [Voyage AI](https://dash.voyageai.com) API key (free tier available)
- An [Anthropic API](https://platform.claude.com) key

---

## Setup

**1. Clone the repo**

```bash
git clone https://github.com/your-username/ProyectoRAG.git
cd ProyectoRAG
```

**2. Create a `.env` file** in the project root:

```env
VOYAGE_API_KEY=your-voyage-api-key
ANTHROPIC_API_KEY=your-anthropic-api-key
```

**3. Start the stack**

```bash
docker compose up --build
```

This builds and starts 3 containers: PostgreSQL + pgvector, the .NET WebApi, and the React frontend served by nginx.

**4. Create the database schema** (first run only)

```bash
docker exec -it proyectorag-postgres psql -U postgres -d proyectorag
```

```sql
CREATE EXTENSION IF NOT EXISTS vector;

CREATE TABLE documents (
    id SERIAL PRIMARY KEY,
    content TEXT NOT NULL,
    embedding vector(1024),
    content_tsv tsvector GENERATED ALWAYS AS (to_tsvector('english', content)) STORED
);

CREATE INDEX idx_documents_tsv ON documents USING GIN(content_tsv);
```

**5. Index the lore**

Upload your `.docx` lore document:

```bash
curl -X POST http://localhost:5173/api/documents/import-docx \
  -F "file=@your-lore-file.docx"
```

The importer parses the document by heading structure and italic sub-topics, generates embeddings in batches with automatic retry/backoff for rate limits, and stores everything in pgvector.

**6. Open the app**

Navigate to `http://localhost:5173`.

---

## API reference

### Import lore from `.docx`

```http
POST /api/documents/import-docx
Content-Type: multipart/form-data

file: <your-lore-file.docx>
```

### Search (vector + BM25, no LLM)

```http
POST /api/documents/search
Content-Type: application/json

{ "query": "Sword Logic", "topK": 6 }
```

### Ask a question (full RAG pipeline)

```http
POST /api/documents/ask
Content-Type: application/json

{ "query": "What is the Winnower's argument about morality?" }
```

Rate limited: 5 requests per minute, 30 per hour per IP.

---

## Lore persona

The assistant answers as a Lightbearer who has spent decades studying the deep, forgotten corners of this universe's history. It speaks with narrative gravity, cites context naturally, and — critically — will not invent lore that isn't in the indexed documents. If the answer isn't there, it says so in character.

---

## Project status

Functional and deployed locally. The lore database currently indexes the Complete Destiny Timeline (Parts 1 and 2), covering everything from Before Time through the end of Destiny 2.

**Possible next steps:**
- Deploy to a cloud provider (Railway, Render, or Azure) for public access
- Re-index with a content length filter to remove low-quality short chunks
- Add Cohere Rerank as a post-retrieval quality pass
- Expand the lore database with additional sources

---

## License

MIT