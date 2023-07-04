
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Questao5.Domain.Entities;

namespace Questao5.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SaldoController : ControllerBase
    {
        private readonly SqliteConnection _connection;

        public SaldoController()
        {
            string connectionString = "Data Source=nome-do-banco-de-dados.db";
            _connection = new SqliteConnection(connectionString);
        }

        [HttpGet("{idContaCorrente}")]
        public IActionResult ConsultarSaldoContaCorrente(string idContaCorrente)
        {
            // Verifica se a conta corrente existe e está ativa
            var contaCorrente = _connection.QueryFirstOrDefault<ContaCorrente>("SELECT * FROM contacorrente WHERE idcontacorrente = @IdContaCorrente", new { IdContaCorrente = idContaCorrente });
            if (contaCorrente == null)
            {
                return BadRequest("Conta corrente não encontrada.");
            }

            if (!contaCorrente.Ativo)
            {
                return BadRequest("Conta corrente inativa.");
            }

            // Calcula o saldo da conta corrente
            var saldo = _connection.QueryFirstOrDefault<decimal>("SELECT SUM(CASE WHEN tipomovimento = 'C' THEN valor ELSE -valor END) FROM movimento WHERE idcontacorrente = @IdContaCorrente", new { IdContaCorrente = idContaCorrente });

            return Ok(new
            {
                NumeroContaCorrente = contaCorrente.Numero,
                NomeTitular = contaCorrente.Nome,
                DataHoraConsulta = DateTime.Now,
                SaldoAtual = saldo
            });
        }
    }
}

