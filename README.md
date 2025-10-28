# LixoZero — API .NET 8 (SQLite + Docker + CI/CD)

API REST para registro de descartes (ESG), com EF Core + SQLite, Docker Compose, testes (unit/BDD) e pipeline CI/CD no GitHub Actions com deploy em **staging** e **produção**.

## Requisitos
- .NET 8 SDK
- Docker e Docker Compose
- GitHub Actions habilitado no repositório

## Endpoints (principais)
- `POST /api/Descartes`
- `GET  /api/Descartes?pagina=1&tamanho=10`
- `GET  /api/Descartes/{id}`
- `DELETE /api/Descartes/{id}`
- Swagger em: **http://localhost:5038/swagger**

---

## Executar local (sem Docker)
```powershell
dotnet restore
dotnet ef database update
dotnet run
# Swagger: http://localhost:5038/swagger
