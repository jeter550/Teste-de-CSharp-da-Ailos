using Questao5.Domain.Enumerators;

namespace Questao5.Domain.Entities
{
    public class Movimento
    {
        public string Id { get; set; }
        public string ContaCorrenteId { get; set; }
        public DateTime DataMovimento { get; set; }
        public TipoMovimento TipoMovimento { get; set; }
        public decimal Valor { get; set; }
    }

}