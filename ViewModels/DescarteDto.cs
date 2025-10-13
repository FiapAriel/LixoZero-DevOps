using System;
using System.ComponentModel.DataAnnotations;
using LixoZero.Models;

namespace LixoZero.ViewModels
{
    public class DescarteDto
    {
        [Required(ErrorMessage = "O campo Bairro é obrigatório.")]
        public string Bairro { get; set; } = "";

        [Required(ErrorMessage = "O campo Tipo é obrigatório.")]
        public TipoMaterial Tipo { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Quantidade deve ser maior que zero.")]
        public double QuantidadeKg { get; set; }

        public DateTime? DataHora { get; set; }
    }
}
