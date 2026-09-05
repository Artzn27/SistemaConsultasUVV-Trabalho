# Sistema de Gestão de Consultas UVV

Trabalho prático da disciplina de Desenvolvimento Web Back-end — aplicação ASP.NET Core MVC com EF Core (Code First), autenticação por cookie e proteção de rotas.

## Integrantes do grupo
Arthur Lima Da Ros


## Tecnologias
- ASP.NET Core MVC (.NET 8)
- Entity Framework Core (Code First + Migrations)
- SQL Server (LocalDB)
- Autenticação via Cookie + `PasswordHasher` (hash seguro de senha)

## Estrutura do projeto
```
SistemaConsultasUVV/
├── Controllers/     # ContaController, ConsultasController, HomeController
├── Models/          # Usuario, Consulta, ViewModels de Conta
├── Data/            # ApplicationDbContext (EF Core)
├── Views/            # Razor Views organizadas por Controller
├── wwwroot/          # CSS estático
├── Program.cs        # DI + pipeline de middlewares
└── appsettings.json   # Connection string
```

## Como configurar e rodar o projeto

### 1. Pré-requisitos
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- SQL Server LocalDB (instalado junto com o Visual Studio) ou uma instância SQL Server acessível
- (Opcional) `dotnet-ef` instalado globalmente:
  ```bash
  dotnet tool install --global dotnet-ef
  ```

### 2. Clonar o repositório
```bash
git clone <link-do-repositorio>
cd SistemaConsultasUVV
```

### 3. Restaurar pacotes
```bash
dotnet restore
```

### 4. Configurar a Connection String
No arquivo `appsettings.json`, ajuste a `DefaultConnection` caso necessário (por padrão já aponta para o LocalDB):
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=SistemaConsultasUVVDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
}
```

### 5. Criar a Migration inicial e o banco de dados (Code First)
```bash
dotnet ef migrations add InicialCreate
dotnet ef database update
```
Isso cria o banco `SistemaConsultasUVVDb` com as tabelas `Usuarios` e `Consultas`.

> Se estiver usando o Visual Studio com o Package Manager Console, os comandos equivalentes são:
> ```powershell
> Add-Migration InicialCreate
> Update-Database
> ```

### 6. Rodar a aplicação
```bash
dotnet run
```
Acesse `https://localhost:<porta>` no navegador (a porta é exibida no terminal).

## Funcionalidades
- **Cadastro de usuário** (`/Conta/Registrar`) — cria conta com nome, e-mail e senha (armazenada como hash).
- **Login** (`/Conta/Login`) — autentica via cookie, com validação de credenciais.
- **Consultas** (`/Consultas`, requer login) — listar, criar, editar e excluir consultas vinculadas ao usuário autenticado.
- Rotas de consulta protegidas com `[Authorize]`; usuário não autenticado é redirecionado ao login.

## Vídeo demonstrativo
🎥 **Link do vídeo:** https://youtu.be/buuOKyjfTV4

## Observações de segurança e arquitetura
- Senhas nunca são armazenadas em texto puro — usa-se `PasswordHasher<Usuario>` (Microsoft.AspNetCore.Identity).
- `UsuarioId` das consultas nunca vem do formulário; é sempre extraído das claims do usuário autenticado, evitando que um usuário manipule dados de outro.
- Pipeline configurado em `Program.cs` com `UseAuthentication()` antes de `UseAuthorization()`.
- `DbContext` registrado via Injeção de Dependência (`AddDbContext`) em `Program.cs`.
