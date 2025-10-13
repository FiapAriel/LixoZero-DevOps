using System;
using System.ComponentModel.DataAnnotations;

namespace LixoZero.Models
{
    public enum TipoMaterial { Papel, Plastico, Vidro, Metal, Organico }

    public class Descarte
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Bairro { get; set; } = ""; // inicializa p/ evitar null

        [Required]
        public TipoMaterial Tipo { get; set; }

        [Range(0.01, double.MaxValue)]
        public double QuantidadeKg { get; set; }

        public DateTime DataHora { get; set; } = DateTime.UtcNow;
    }
}
