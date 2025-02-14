using System;
using System.Globalization;
using System.Reflection.Metadata.Ecma335;

namespace Questao1
{
    public class ContaBancaria
    {
        private const double _CONST_TAXA_SAQUE = 3.5;

        public ContaBancaria(int numero, string titular)
        {
            Numero = numero;
            Titular = titular;
        }

        public ContaBancaria(int numero, string titular, double depositoInicial)
        {
            Numero = numero;
            Titular = titular;
            Saldo = depositoInicial;
        }

        public int Numero { get; }
        public string Titular { get;private set; }
        public double Saldo { get; private set; }

        internal void AlterarNomeTilular(string nome)
        {
            Titular = nome;
        }
        internal void Deposito(double quantia)
        {
            Saldo += quantia;
        }
        internal void Saque(double quantia)
        {
            CobrarTaxa();
            Saldo -= quantia;
        }
        private void CobrarTaxa()
        {
            Saldo -= _CONST_TAXA_SAQUE;
        }
        public override string ToString()
        {
            return $"Conta { Numero }, Titular: { Titular }, Saldo: { Saldo.ToString("C") }";
        }
    }
}
