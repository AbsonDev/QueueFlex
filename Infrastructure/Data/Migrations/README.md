# Migrações do Entity Framework Core

Este diretório contém as migrações do Entity Framework Core para o banco de dados PostgreSQL.

## 📋 Como Criar Migrações

### 1. Usando dotnet CLI

```bash
# Criar uma nova migração
dotnet ef migrations add NomeDaMigracao --project Infrastructure --startup-project Domain

# Exemplo:
dotnet ef migrations add InitialCreate --project Infrastructure --startup-project Domain
dotnet ef migrations add AddUserProfile --project Infrastructure --startup-project Domain
```

### 2. Usando Package Manager Console (Visual Studio)

```powershell
# Navegar para o projeto Infrastructure
cd Infrastructure

# Criar migração
Add-Migration NomeDaMigracao

# Exemplo:
Add-Migration InitialCreate
Add-Migration AddUserProfile
```

## 🚀 Como Aplicar Migrações

### 1. Usando dotnet CLI

```bash
# Aplicar todas as migrações pendentes
dotnet ef database update --project Infrastructure --startup-project Domain

# Aplicar até uma migração específica
dotnet ef database update NomeDaMigracao --project Infrastructure --startup-project Domain

# Exemplo:
dotnet ef database update InitialCreate --project Infrastructure --startup-project Domain
```

### 2. Usando Package Manager Console (Visual Studio)

```powershell
# Navegar para o projeto Infrastructure
cd Infrastructure

# Aplicar todas as migrações
Update-Database

# Aplicar até uma migração específica
Update-Database NomeDaMigracao

# Exemplo:
Update-Database InitialCreate
```

## 📊 Como Remover Migrações

### 1. Remover última migração (não aplicada)

```bash
# Usando dotnet CLI
dotnet ef migrations remove --project Infrastructure --startup-project Domain

# Usando Package Manager Console
Remove-Migration
```

### 2. Remover migração específica

```bash
# Primeiro, reverter o banco para a migração anterior
dotnet ef database update NomeDaMigracaoAnterior --project Infrastructure --startup-project Domain

# Depois remover a migração
dotnet ef migrations remove --project Infrastructure --startup-project Domain
```

## 🔍 Como Verificar Status das Migrações

### 1. Listar migrações

```bash
# Usando dotnet CLI
dotnet ef migrations list --project Infrastructure --startup-project Domain

# Usando Package Manager Console
Get-Migration
```

### 2. Verificar status do banco

```bash
# Verificar se há migrações pendentes
dotnet ef database update --project Infrastructure --startup-project Domain --dry-run
```

## ⚠️ Importante

- **SEMPRE** faça backup do banco antes de aplicar migrações em produção
- **NUNCA** remova migrações que já foram aplicadas em produção
- **TESTE** as migrações em ambiente de desenvolvimento antes de aplicar em produção
- **DOCUMENTE** as mudanças feitas em cada migração

## 🛠️ Estrutura das Migrações

Cada migração contém:

- **Up()**: Método para aplicar as mudanças
- **Down()**: Método para reverter as mudanças
- **Timestamp**: Data/hora da criação da migração
- **Nome**: Nome descritivo da migração

## 📝 Exemplo de Migração

```csharp
public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Código para criar tabelas, índices, etc.
        migrationBuilder.CreateTable(
            name: "tenants",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                // ... outras colunas
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_tenants", x => x.id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // Código para reverter as mudanças
        migrationBuilder.DropTable(name: "tenants");
    }
}
```

## 🔧 Configuração do PostgreSQL

As migrações estão configuradas para PostgreSQL com:

- **UUID** como tipo padrão para IDs
- **snake_case** para nomes de colunas e tabelas
- **Índices** otimizados para performance
- **Constraints** apropriadas para integridade dos dados

## 📚 Recursos Adicionais

- [Documentação oficial do EF Core](https://docs.microsoft.com/en-us/ef/core/)
- [Migrações no EF Core](https://docs.microsoft.com/en-us/ef/core/managing-schemas/migrations/)
- [PostgreSQL com EF Core](https://docs.microsoft.com/en-us/ef/core/providers/npgsql/)