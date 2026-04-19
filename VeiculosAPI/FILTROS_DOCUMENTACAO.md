# Documentação dos Filtros - Sistema de Locação de Veículos

## Resumo
Este documento descreve os 5 filtros implementados no backend do sistema de locação de veículos, especificando os endpoints, tipos de joins e exemplos de uso.

---

## Filtro 1: Alugueis por Cliente

**Endpoint:** `GET /api/Alugueis/por-cliente/{clienteId}`

**Descrição:** Retorna todos os alugueis associados a um cliente específico.

**Join utilizado:** Cliente → Aluguel (Include)

**Parâmetros:**
- `clienteId` (int): ID do cliente desejado

**Resposta de sucesso (200 OK):**
```json
[
  {
    "id": 1,
    "clienteId": 1,
    "veiculoId": 1,
    "dataInicio": "2026-04-15T10:00:00",
    "dataFim": "2026-04-20T10:00:00",
    "quilometragemInicial": 50000,
    "quilometragemFinal": 50500,
    "valorDiaria": 150.00,
    "valorTotal": 750.00,
    "dataDevolucao": "2026-04-20T14:00:00",
    "cliente": {
      "id": 1,
      "nome": "João Silva",
      "cpf": "123.456.789-00",
      "email": "joao@example.com"
    },
    "veiculo": {
      "id": 1,
      "modelo": "Corolla",
      "anoFabricacao": 2020,
      "quilometragem": 50500,
      "fabricanteId": 1
    }
  }
]
```

**Casos de uso:**
- Consultar histórico de locações de um cliente
- Verificar alugueis em aberto do cliente
- Gerar relatório de locações por cliente

---

## Filtro 2: Alugueis Ativos

**Endpoint:** `GET /api/Alugueis/ativos`

**Descrição:** Retorna apenas os alugueis que ainda não foram devolvidos (DataDevolucao == null).

**Join utilizado:** Aluguel → Cliente + Aluguel → Veiculo (Include x2)

**Parâmetros:** Nenhum

**Resposta de sucesso (200 OK):**
```json
[
  {
    "id": 1,
    "clienteId": 1,
    "veiculoId": 2,
    "dataInicio": "2026-04-18T09:00:00",
    "dataFim": "2026-04-25T09:00:00",
    "quilometragemInicial": 75000,
    "quilometragemFinal": null,
    "valorDiaria": 120.00,
    "valorTotal": 840.00,
    "dataDevolucao": null,
    "cliente": {...},
    "veiculo": {...}
  }
]
```

**Casos de uso:**
- Identificar veículos que ainda estão alugados
- Monitorar alugueis em andamento
- Verificar quais clientes têm alugueis pendentes
- Alertas para datas de devolução próximas

---

## Filtro 3: Veículos por Fabricante

**Endpoint:** `GET /api/Veiculos/por-fabricante/{fabricanteId}`

**Descrição:** Retorna todos os veículos de um fabricante específico.

**Join utilizado:** Fabricante → Veiculo (Include)

**Parâmetros:**
- `fabricanteId` (int): ID do fabricante desejado

**Resposta de sucesso (200 OK):**
```json
[
  {
    "id": 1,
    "modelo": "Corolla",
    "anoFabricacao": 2020,
    "quilometragem": 50500,
    "fabricanteId": 1,
    "fabricante": {
      "id": 1,
      "nome": "Toyota",
      "veiculos": [...]
    }
  },
  {
    "id": 2,
    "modelo": "Camry",
    "anoFabricacao": 2021,
    "quilometragem": 32000,
    "fabricanteId": 1,
    "fabricante": {
      "id": 1,
      "nome": "Toyota",
      "veiculos": [...]
    }
  }
]
```

**Casos de uso:**
- Filtrar veículos por marca
- Consultar frota de um fabricante específico
- Análise de distribuição de marcas na frota
- Manutenção específica por marca

---

## Filtro 4: Pagamentos por Cliente

**Endpoint:** `GET /api/Pagamentos/por-cliente/{clienteId}`

**Descrição:** Retorna todos os pagamentos registrados para um cliente específico.

**Join utilizado:** Aluguel → Cliente + Pagamento → Aluguel (Include + ThenInclude - aninhado)

**Parâmetros:**
- `clienteId` (int): ID do cliente desejado

**Resposta de sucesso (200 OK):**
```json
[
  {
    "id": 1,
    "aluguelId": 1,
    "valor": 750.00,
    "dataPagamento": "2026-04-20T15:00:00",
    "metodoPagamento": "Cartão",
    "aluguel": {
      "id": 1,
      "clienteId": 1,
      "veiculoId": 1,
      "dataInicio": "2026-04-15T10:00:00",
      "dataFim": "2026-04-20T10:00:00",
      "quilometragemInicial": 50000,
      "quilometragemFinal": 50500,
      "valorDiaria": 150.00,
      "valorTotal": 750.00,
      "dataDevolucao": "2026-04-20T14:00:00",
      "cliente": {
        "id": 1,
        "nome": "João Silva",
        "cpf": "123.456.789-00",
        "email": "joao@example.com"
      },
      "veiculo": {...}
    }
  }
]
```

**Casos de uso:**
- Verificar histórico de pagamentos de um cliente
- Confirmar recebimento de pagamentos
- Gerar comprovantes de pagamento
- Relatório financeiro por cliente
- Auditoria de transações

---

## Filtro 5: Total de Pagamentos por Cliente

**Endpoint:** `GET /api/Pagamentos/total-por-cliente/{clienteId}`

**Descrição:** Retorna o valor total pago por um cliente específico (agregação com SUM).

**Join utilizado:** Aluguel → Cliente + Pagamento → Aluguel + Agregação (SUM)

**Parâmetros:**
- `clienteId` (int): ID do cliente desejado

**Resposta de sucesso (200 OK):**
```
2500.00
```

(Retorna um número decimal com o total)

**Casos de uso:**
- Consultar quanto um cliente já pagou no total
- Análise de receita por cliente
- Relatórios financeiros
- Estatísticas de clientes VIP
- Comparação de perfil de pagamento entre clientes

---

## Tipos de Joins Utilizados

### 1. Include (Inner Join implícito)
Usado nos filtros 1, 2 e 3 para incluir dados relacionados:
```csharp
.Include(a => a.Cliente)
.Include(a => a.Veiculo)
```

### 2. ThenInclude (Nested Join/Join aninhado)
Usado no filtro 4 para incluir dados em múltiplos níveis:
```csharp
.Include(p => p.Aluguel)
.ThenInclude(a => a.Cliente)
```

### 3. Agregação (SUM)
Usado no filtro 5 para calcular totais:
```csharp
.SumAsync(p => p.Valor)
```

---

## Testes Recomendados

### Via Swagger UI
1. Acesse: `https://localhost:5065/swagger` (ou a porta configurada)
2. Navegue até o controller desejado
3. Clique em "Try it out"
4. Insira os parâmetros necessários
5. Clique em "Execute"

### Exemplo de Teste Manual

**1. Criar um Fabricante:**
```
POST /api/Fabricantes
Body: {"nome":"Toyota"}
```

**2. Criar um Veículo:**
```
POST /api/Veiculos
Body: {"modelo":"Corolla","anoFabricacao":2020,"quilometragem":50000,"fabricanteId":1}
```

**3. Criar um Cliente:**
```
POST /api/Clientes
Body: {"nome":"João Silva","cpf":"123.456.789-00","email":"joao@example.com"}
```

**4. Criar um Aluguel:**
```
POST /api/Alugueis
Body: {"clienteId":1,"veiculoId":1,"dataInicio":"2026-04-15T10:00:00","dataFim":"2026-04-20T10:00:00","quilometragemInicial":50000,"valorDiaria":150,"valorTotal":750}
```

**5. Criar um Pagamento:**
```
POST /api/Pagamentos
Body: {"aluguelId":1,"valor":750,"dataPagamento":"2026-04-20T15:00:00","metodoPagamento":"Cartão"}
```

**6. Testar Filtro 1:**
```
GET /api/Alugueis/por-cliente/1
```

**7. Testar Filtro 2:**
```
GET /api/Alugueis/ativos
```

**8. Testar Filtro 3:**
```
GET /api/Veiculos/por-fabricante/1
```

**9. Testar Filtro 4:**
```
GET /api/Pagamentos/por-cliente/1
```

**10. Testar Filtro 5:**
```
GET /api/Pagamentos/total-por-cliente/1
```

---

## Códigos de Resposta HTTP

- **200 OK:** Requisição bem-sucedida
- **201 Created:** Recurso criado com sucesso
- **204 No Content:** Operação bem-sucedida sem retorno
- **400 Bad Request:** Dados inválidos ou ausentes
- **404 Not Found:** Recurso não encontrado
- **500 Internal Server Error:** Erro no servidor

---

## Considerações de Performance

- Todos os filtros utilizam `.ToListAsync()` para operações assíncronas
- Os Include statements evitam múltiplas queries ao banco
- O filtro 5 (Total de Pagamentos) utiliza agregação no banco de dados (SUM) para melhor performance
- Considere adicionar paginação para grandes volumes de dados

---

## Versão do Documento
- Data: 19 de abril de 2026
- Status: Pronto para produção
