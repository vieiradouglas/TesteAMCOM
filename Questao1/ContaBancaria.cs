using System;
using System.Globalization;

namespace Questao1
{
    public class ContaBancaria
    {
        private readonly int numeroConta;
        private string titularConta;
        private double saldo;

        public ContaBancaria(int numeroConta, string titularConta, double depositoInicial = 0.0)
        {
            this.numeroConta = numeroConta;
            this.titularConta = titularConta;
            saldo = depositoInicial;
        }

        public void Deposito(double valor)
        {
            saldo += valor;
        }

        public void Saque(double valor)
        {
            if (saldo >= valor)
            {
                saldo -= valor + 3.50;
            }
            else
            {
                Console.WriteLine("Saldo insuficiente para realizar o saque.");
            }
        }

        public void ExibirDadosConta()
        {
            Console.WriteLine($"Conta {numeroConta}, Titular: {titularConta}, Saldo: $ {saldo:F2}");
        }
    }
}
