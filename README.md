![.NET Version](https://img.shields.io/badge/.NET-8.0%20-purple?style=for-the-badge&logo=dotnet)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=csharp&logoColor=white)
![Architecture](https://img.shields.io/badge/Architecture-Clean%20%2F%20DDD-blue?style=for-the-badge)
![Database](https://img.shields.io/badge/Database-SQLite-003B57?style=for-the-badge&logo=sqlite&logoColor=white)
![ORM](https://img.shields.io/badge/ORM-EF%20Core%20%26%20Dapper-orange?style=for-the-badge)
![Tests](https://img.shields.io/badge/Tests-xUnit%20%26%20Moq-green?style=for-the-badge)
![Swagger](https://img.shields.io/badge/Swagger-85EA2D?style=for-the-badge&logo=swagger&logoColor=black)
![License](https://img.shields.io/badge/License-MIT-blue?style=for-the-badge)


# Central de Pedidos API

Uma Web API robusta, escalável e de alta performance desenvolvida em **.NET 8** para o gerenciamento do ciclo de vida de pedidos e seus respectivos itens. O projeto foi estruturado seguindo os padrões mais exigentes de mercado, unindo a segurança do **Entity Framework Core** para operações de escrita e a velocidade do **Dapper** para consultas paginadas complexas.

---

## 🏗️ Arquitetura do Sistema

O projeto foi desenhado utilizando os princípios da **Clean Architecture** (Arquitetura Limpa) e **DDD (Domain-Driven Design)**, garantindo o desacoplamento total entre as regras de negócio e os detalhes de infraestrutura (banco de dados, frameworks, etc.).

### Camadas do Projeto:
* **Domain (Domínio):** O coração da aplicação. Contém as entidades (`Pedido`, `ItemPedido`), enums e exceções de negócio. É uma camada pura, encapsulada e totalmente blindada contra interferências externas.
* **Application (Aplicação):** Orquestra os casos de uso do sistema. Contém as interfaces, serviços (`PedidoService`), DTOs de entrada/saída e os mapeamentos do **AutoMapper**.
* **Infrastructure (Infraestrutura):** Lida com o acesso a dados. Contém o `AppDbContext` do EF Core, as configurações do Fluent API, repositórios e os mapeamentos customizados do **Dapper** (`GuidTypeHandler`).
* **API:** A porta de entrada do sistema. Contém as Controllers, configurações de Injeção de Dependência, o **FluentValidation** e o Middleware de tratamento global de erros.

---

## ⚡ Diferenciais Técnicos e Decisões de Engenharia

### 1. Abordagem Híbrida de Banco de Dados (EF Core + Dapper)
* **Escrita Segura (EF Core):** Operações que exigem transação, validação de estado e consistência agregada (como `CriarPedido` e `Cancelar`) utilizam o Entity Framework Core para garantir a integridade do domínio.
* **Leitura de Alta Performance (Dapper):** O endpoint de listagem paginada (`GET /api/pedidos/todos`) faz o uso de consultas SQL puras e otimizadas através do Dapper, reduzindo drasticamente o consumo de memória e o tempo de resposta do servidor.

### 🧩 Nota Importante para o Avaliador (Regra de Negócio do PDF)
> Conforme solicitado nos requisitos de negócio, o sistema **não permite o cancelamento de pedidos com o status `Pago`**. 
> 
> Como o escopo original do projeto previa apenas o status inicial `Novo` e o fluxo isolado de cancelamento, foi adicionado um método público `AlterarStatus` na entidade de domínio de forma consciente. Esta decisão foi tomada unicamente para permitir a transição simulada para o status `Pago`, tornando a regra de bloqueio de cancelamento exigida no PDF **100% testável** tanto via Swagger quanto através dos Testes Unitários.

### 2. Extensão do Dapper (`GuidTypeHandler`)
Como o **SQLite** armazena identificadores únicos (`Guid`) como strings textuais puro (`TEXT`), foi desenvolvida uma extensão customizada no pipeline do Dapper mapeando nativamente essa conversão e blindando os repositórios contra falhas de *InvalidCastException*.

### 🛡️ 3. Middleware Global de Erros e Fail-Fast Validations
A API conta com validações na porta de entrada utilizando o **FluentValidation**. Se um payload for inválido, a requisição é interrompida imediatamente (*Fail-Fast*). Caso ocorra uma violação de regra de negócio (`DomainException`) no coração do sistema, o **Middleware de Exceções Global** intercepta a falha e formata uma resposta HTTP limpa e padronizada.

---

## 🧪 Cobertura de Testes Automatizados

O projeto conta com uma suíte de testes unitários robusta utilizando **xUnit** e **Moq**, cobrindo os seguintes cenários:

* **Domínio (`Domain`):** Testes puros focados no comportamento da entidade `Pedido`, garantindo o recálculo correto do valor total ao adicionar itens e as travas de transição de status.
* **Aplicação (`Service`):** Testes isolados com Mocks que validam o fluxo completo das regras da aplicação e disparo de exceções.
* **API (`Controller`):** Testes focados no comportamento dos endpoints, garantindo os retornos corretos do protocolo REST (`201 Created` com cabeçalho *Location*, `204 NoContent`, `400 BadRequest`, etc.).

---

## 🚀 Como Executar o Projeto (Zero Setup)

A aplicação foi configurada para criar e migrar o banco de dados local SQLite de forma **100% automática** ao iniciar. Não é necessário executar comandos no terminal para começar a testar.

1.  Abra a solução no **Visual Studio** ou **VS Code**.
2.  Defina o projeto **`CentralPedidos.API`** como o projeto de inicialização.
3.  Pressione `F5` ou execute o comando:
    ```bash
    dotnet run --project CentralPedidos.API
    ```
4.  O navegador abrirá automaticamente na página interativa do **Swagger UI**, onde todos os endpoints e payloads documentados estarão disponíveis para testes imediatos.

### Para executar os testes:
No terminal da raiz da solução, execute:
```bash
dotnet test
```
---
