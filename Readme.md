# Wallet API

API RESTful desenvolvida em C# (.NET 8) para gerenciamento de carteiras digitais e transações financeiras.

## Funcionalidades

- Registro e login com autenticação JWT
- Criação automática de carteira no momento do cadastro
- Consulta de saldo da carteira
- Adição de saldo à carteira
- Transferência entre usuários
- Listagem de transações com filtro por período (data inicial e final)

---

## Tecnologias Utilizadas

- ASP.NET Core 8
- Entity Framework Core
- PostgreSQL
- JWT (Json Web Token)
- Swagger (documentação automática)
- Docker (opcional)
- Validações com DataAnnotations
- Middleware para tratamento global de erros

---

## Executando Localmente

### Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [PostgreSQL](https://www.postgresql.org/download/)
- (Opcional) Docker

