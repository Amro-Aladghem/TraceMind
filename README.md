# TraceMind

TraceMind is an AI-powered observability assistant. It ingests your application's logs, investigates the daily Error and Fatal entries with an AI agent, and saves a structured investigation with a summary, root cause, recommendations, and confidence — so you can understand what went wrong without reading every log line yourself.

<div style={{ textAlign: "center" }}>
<img width="630" height="440" alt="sgFM6wXg8m" src="https://github.com/user-attachments/assets/db3f6d7e-c848-4d5f-a3d2-515ebadc2de4" />
</div>

<img width="1280" height="720" alt="تصميم بدون عنوان (7)" src="https://github.com/user-attachments/assets/11d0dc82-b178-45f1-b546-15d6a3373a0d" />

## 1. What TraceMind Does

- Stores application logs with level, message, exception, request path, method, status code, trace id, IP, domain, and request duration.
- Lets you browse and filter logs by level, time range, endpoint, method, status code, and trace id.
- Runs an AI investigation over today's Error and Fatal logs.
- Saves investigations with a title, summary, root cause, recommendations, confidence, model name, and status.
- Shows previous investigations for the same application, so recurring problems are easy to spot.
- Exposes the agent's progress through notifications: `Queued`, `Running`, `WaitingForUser`, `Completed`, and `Failed`.
- Provides a dark dashboard for investigations, recent logs, notifications, and live agent status.

### From Log to Investigation

1. Your application sends logs to the log endpoint (or they are stored directly in the database).
2. The dashboard automatically sends a `Follow your instructions` message to the chat endpoint. This happens at most once every 10 minutes per browser.
3. The API stores the user message as a chat row, creates a `Queued` notification, queues the request in an in-memory channel, and responds `202 Accepted` immediately.
4. A background worker picks up the request, marks the notification as `Running`, and runs the `LoggingAgent`.
5. The agent calls `GetDailyErrorLogsTool` to retrieve today's aggregated Error and Fatal logs, then analyses the returned records.
6. The agent calls `CreateInvestigationTool` with the generated title, summary, root cause, recommendations, confidence, and model name.
7. The investigation is saved as `Completed`, and the agent finishes with a final notification.
8. The dashboard polls the notification endpoint every 5 seconds and refreshes the investigation and log cards when the agent's state changes.

## 2. The AI Engine (BaseAgent) and LoggingAgent

`BaseAgent` is the shared engine that powers the agent. An agent is a configuration of that engine: an agent type plus its own AI chat implementation and system prompt.

At a high level, the engine loop:

- Loads the chat context for the conversation, filtered to the current agent's own rows.
- Sends the context to the AI model with the agent's system prompt.
- Parses the model's JSON decision: finish, ask the user, or call a tool.
- Executes the requested tool, saves the tool result as a `Tool` chat row, and loops again.
- Finishes with `IsFinished`, `AskUser`, or an error after the configured retry limit.

The AI provider is **Ollama** through `OllamaSharp`. `LoggingAgentChat` uses the model `qwen2.5-coder:7b` and asks it for JSON output constrained by a schema.

Current agent:

- **LoggingAgent** — the investigation agent. It reads today's Error and Fatal logs, groups the recurring problems, and creates an AI investigation. Its system prompt is stored in `Backend/Infrastructure/Resources/LoggingAgentSysPrompt.txt`.

`AgentExecutionService` routes the queued request to the right agent (currently `LoggingAgent`), and `AgentBackgroundWorker` consumes the in-memory channel in the background.

## 3. Tools & Services

Tools are the small operations the AI agent decides to call. Each tool calls a service, and the services do the actual work (database access, aggregation, investigation persistence).

Tool groups:

- **Log tools**
  - `GetLogsTool` — paginated logs with filters.
  - `GetRecentErrorsTool` — newest Error and Fatal logs.
  - `GetLogsByTraceIdTool` — every log that belongs to one HTTP request.
  - `SearchLogsTool` — text search inside messages and exceptions.
  - `GetErrorCountTool` — Error and Fatal counts for a time range.
  - `GetMostFrequentErrorsTool` — errors grouped by exception type.
  - `GetSlowRequestsTool` — requests above a duration threshold.
  - `GetRequestStatisticsTool` — success/failure counts, average/max duration, and error rate.
  - `GetDailyErrorLogsTool` — today's Error and Fatal logs grouped by request path and message.
  - `GetApplicationInfoTool` — application metadata.

- **Investigation tools**
  - `CreateInvestigationTool` — creates a completed investigation with the generated analysis.
  - `GetInvestigationByIdTool` — full details of one investigation.
  - `GetPreviousInvestigationsTool` — paginated investigation history.
  - `UpdateInvestigationTool` — updates the summary, root cause, recommendations, and model name.
  - `UpdateInvestigationStatusTool` — moves an investigation through `Pending`, `Running`, `Completed`, or `Failed`.

- **Application tools**
  - `CreateApplicationTool` — registers a new application.

Tool calls and their results are stored in the chat history as `Tool` rows, so the agent can follow its own reasoning on the next turn.

Backing services:

- `LogService` — log ingestion, filtering, aggregation, daily errors, slow requests, and request statistics.
- `AIInvestigationService` — create, read, update, and paginate investigations.
- `ApplicationService` — application creation.
- `ChatService` — chat history for the agent.
- `NotificationService` — queued/running/completed/failed notifications.
- `AgentChatCoordinator` and `ToolChatCoordinator` — connect the AI responses and tool results to the chat history.

## 4. Configuration

The backend reads its configuration from `Backend/TraceMindApi/appsettings.Development.json` (kept out of version control next to `appsettings.json`).

Create the file yourself and fill in your own values. The important keys are:

- `SQL_CONNECTION_STRING` — PostgreSQL connection string.
- `Ollama_Uri` — Ollama server URL. The backend also falls back to the `OLLAMA_URL` environment variable, then `OllamaUrl`, then `http://localhost:11434`.
- `aiRetryCount` — how many times the AI call is retried before failing.

Never commit real keys, connection strings, or secrets — use placeholders in shared files and keep your local file private.

## 5. Running the Backend

The backend is an ASP.NET Core (.NET 10) solution split into `Domain`, `Application`, `Infrastructure`, `Presentation`, and `TraceMindApi`.

From the `Backend/` folder, apply the EF Core migration:

```bash
dotnet ef database update --project TraceMindApi/TraceMindApi.csproj --startup-project TraceMindApi/TraceMindApi.csproj
```

Run the API:

```bash
dotnet run --project TraceMindApi/TraceMindApi.csproj
```

The API runs on `http://localhost:5182` under the `api/v1/applications` routes.

Main endpoints:

- `POST /api/v1/applications/{applicationId}/chats`
- `POST /api/v1/applications/{applicationId}/chats/{chatSignature}`
- `GET  /api/v1/applications/{applicationId}/chats/{chatSignature}`
- `POST /api/v1/applications/{applicationId}/logs`
- `GET  /api/v1/applications/{applicationId}/logs`
- `GET  /api/v1/applications/{applicationId}/investigations`
- `GET  /api/v1/applications/{applicationId}/investigations/{investigationId}`
- `GET  /api/v1/applications/{applicationId}/notifications`

CORS is enabled for the Vite development origins (`localhost` and `127.0.0.1` on ports `5173` and `5174`).

## 6. Running the Frontend

The frontend is a Vite + React + TypeScript app styled with Tailwind CSS. It uses a black canvas, dark orange accents, and white text.

Create `Frontend/my-app/.env` (there is a `.env.example` to copy) and set:

```env
VITE_API_BASE_URL=http://localhost:5182/api/v1
VITE_APPLICATION_ID=<your-application-guid>
```

Then:

```bash
cd Frontend/my-app
npm install
npm run dev
```

The app includes:

- A **Dashboard** with:
  - AI investigations — click any investigation to open a dialog with the full analysis.
  - Latest logs — the newest entries across the application.
  - Notifications — the latest agent updates, with a full notifications dialog.
  - Agent status — based on the latest notification state, plus totals for notifications, running, and queued requests.
- A **Logs** page with filters, a log table, and pagination.
- There is **no chat interface** in TraceMind. The dashboard triggers the agent automatically; the agent's progress is followed through notifications.
- Automatic agent triggering: the dashboard sends `Follow your instructions` at most once every 10 minutes. The chat signature is created in `sessionStorage` when needed, and the last call time is stored in `localStorage` under `tracemind.lastAgentCallAt.<applicationId>`.
- A notification feed that follows the agent's progress (`Queued`, `Running`, `WaitingForUser`, `Completed`, `Failed`) and polls every 5 seconds.

## 7. Contributing

Contributions are welcome! Feel free to open a pull request: fork the repo, create a feature branch, commit your changes, and describe your PR clearly.

Author: Amro Aladghem
