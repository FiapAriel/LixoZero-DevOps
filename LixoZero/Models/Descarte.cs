using System.ComponentModel.DataAnnotations;

namespace LixoZero.Models
{
    public enum TipoMaterial
    {
        Papel,
        Plastico,
        Vidro,
        Metal,
        Organico
    }

    public class Descarte
    {
        [Key]
        public int Id { get; set; }

        public required string Bairro { get; set; }

        public TipoMaterial Tipo { get; set; }

        public double QuantidadeKg { get; set; }

        public DateTime DataHora { get; set; }
    }
}
