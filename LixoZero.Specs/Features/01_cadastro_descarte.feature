# language: pt
@api @cadastro @status @contrato
Funcionalidade: Cadastro de Descarte

  Contexto:
Dado que a base URL da API é "http://localhost:5038"

  Cenário: Cadastrar descarte válido retorna 201 e corpo no contrato
    Quando eu fizer POST para "/api/Descartes" com o corpo JSON:
      """
      { "bairro":"Centro", "tipo":0, "quantidadeKg":0.25, "dataHora":"2025-10-10T22:09:47.709Z" }
      """
    Então o status da resposta deve ser 201
    E o corpo JSON deve obedecer ao schema "Schemas/descarte.schema.json"
    E o campo "bairro" deve ser "Centro"
    E o campo "tipo" deve ser 0
    E o campo "quantidadeKg" deve ser 0.25

  Cenário: Cadastrar descarte inválido (quantidade <= 0) retorna 400
    Quando eu fizer POST para "/api/Descartes" com o corpo JSON:
      """
      { "bairro":"Centro", "tipo":0, "quantidadeKg":0, "dataHora":"2025-10-10T22:09:47.709Z" }
      """
    Então o status da resposta deve ser 400

