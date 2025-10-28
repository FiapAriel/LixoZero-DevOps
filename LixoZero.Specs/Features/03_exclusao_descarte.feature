# language: pt
@api @delete @status
Funcionalidade: Exclusão de descarte por ID

  Contexto:
    Dado que a base URL da API é "<BASE_URL>"

  Cenário: Deletar um descarte existente retorna 204
    Dado que exista um descarte criado via POST em "/api/Descartes" com o corpo:
      """
      { "bairro":"Bela Vista", "tipo":2, "quantidadeKg":0.7, "dataHora":"2025-10-10T22:09:47.709Z" }
      """
    Quando eu fizer DELETE para "/api/Descartes/{idCriado}"
    Então o status da resposta deve ser 204

  Cenário: Deletar um descarte inexistente retorna 404
    Quando eu fizer DELETE para "/api/Descartes/999999"
    Então o status da resposta deve ser 404

