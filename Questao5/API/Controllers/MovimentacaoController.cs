using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Questao5.Domain.Entities;

namespace Questao5.API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class MovimentacaoController : ControllerBase
    {
        private readonly SqliteConnection _connection;

        public MovimentacaoController()
        {
            string connectionString = "Data Source=nome-do-banco-de-dados.db";
            _connection = new SqliteConnection(connectionString);
        }

        [HttpPost]
        public IActionResult MovimentarContaCorrente(string idRequisicao, string idContaCorrente, decimal valor, string tipoMovimento)
        {
            var contaCorrente = _connection.QueryFirstOrDefault<ContaCorrente>("SELECT * FROM contacorrente WHERE idcontacorrente = @IdContaCorrente", new { IdContaCorrente = idContaCorrente });
            if (contaCorrente == null)
            {
                return BadRequest("Conta corrente não encontrada.");
            }

            if (!contaCorrente.Ativo)
            {
                return BadRequest("Conta corrente inativa.");
            }

            if (valor <= 0)
            {
                return BadRequest("Valor inválido.");
            }

            if (tipoMovimento != "C" && tipoMovimento != "D")
            {
                return BadRequest("Tipo de movimento inválido.");
            }

            var idempotencia = _connection.QueryFirstOrDefault<Idempotencia>("SELECT * FROM idempotencia WHERE chave_idempotencia = @ChaveIdempotencia", new { ChaveIdempotencia = idRequisicao });
            if (idempotencia != null)
            {
                return Ok(idempotencia.Resultado); // Retorna o resultado armazenado anteriormente
            }

            // Insere o movimento na tabela "movimento"
            var movimento = new Movimento
            {
                IdMovimento = Guid.NewGuid().ToString(),
                IdContaCorrente = idContaCorrente,
                DataMovimento = DateTime.Now.ToString("dd/MM/yyyy"),
                TipoMovimento = tipoMovimento,
                Valor = valor
            };

            _connection.Execute("INSERT INTO movimento (idmovimento, idcontacorrente, datamovimento, tipomovimento, valor) VALUES (@IdMovimento, @IdContaCorrente, @DataMovimento, @TipoMovimento, @Valor)", movimento);

            // Armazena a requisição e o resultado na tabela "idempotencia"
            _connection.Execute("INSERT INTO idempotencia (chave_idempotencia, requisicao, resultado) VALUES (@ChaveIdempotencia, @Requisicao, @Resultado)", new
            {
                ChaveIdempotencia = idRequisicao,
                Requisicao = idRequisicao,
                Resultado = movimento.IdMovimento
            });

            return Ok(movimento.IdMovimento);
        }
    }
}

