# language: pt
@api @consulta @status @contrato
Funcionalidade: Consulta de descarte por ID

  Contexto:
    Dado que a base URL da API é "http://localhost:5038"

  Cenário: Consultar por ID existente retorna 200 e contrato válido
    Dado que exista um descarte criado via POST em "/api/Descartes" com o corpo:
      """
      { "bairro":"Centro", "tipo":1, "quantidadeKg":1.5, "dataHora":"2025-10-10T22:09:47.709Z" }
      """
    Quando eu fizer GET para "/api/Descartes/{idCriado}"
    Então o status da resposta deve ser 200
    E o corpo JSON deve obedecer ao schema "Schemas/descarte.schema.json"
    E o campo "tipo" deve ser 1
    E o campo "quantidadeKg" deve ser 1.5

  Cenário: Consultar por ID inexistente retorna 404
    Quando eu fizer GET para "/api/Descartes/999999"
    Então o status da resposta deve ser 404

