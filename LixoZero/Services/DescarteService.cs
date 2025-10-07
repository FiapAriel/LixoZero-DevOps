using LixoZero.Data;
using LixoZero.Models;
using LixoZero.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace LixoZero.Services
{
    public class DescarteService
    {
        private readonly AppDbContext _context;

        public DescarteService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<DescarteDto>> GetDescartesAsync(int page, int pageSize)
        {
            var totalItems = await _context.Descartes.CountAsync();

            var items = await _context.Descartes
                .OrderByDescending(d => d.DataHora)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(d => new DescarteDto
                {
                    Bairro = d.Bairro,
                    Tipo = d.Tipo,
                    QuantidadeKg = d.QuantidadeKg,
                    DataHora = d.DataHora
                })
                .ToListAsync();

            return new PagedResult<DescarteDto>
            {
                TotalItems = totalItems,
                Items = items
            };
        }

        public async Task<DescarteDto?> GetByIdAsync(int id)
        {
            var d = await _context.Descartes.FindAsync(id);
            if (d == null) return null;

            return new DescarteDto
            {
                Bairro = d.Bairro,
                Tipo = d.Tipo,
                QuantidadeKg = d.QuantidadeKg,
                DataHora = d.DataHora
            };
        }

        public async Task<DescarteDtoComId> CreateAsync(DescarteDto dto)
        {
            if (dto.QuantidadeKg <= 0)
                throw new ArgumentException("A quantidade deve ser maior que zero.");

            var novo = new Descarte
            {
                Bairro = dto.Bairro,
                Tipo = dto.Tipo,
                QuantidadeKg = dto.QuantidadeKg,
                DataHora = dto.DataHora ?? DateTime.UtcNow
            };

            _context.Descartes.Add(novo);
            await _context.SaveChangesAsync();

            return new DescarteDtoComId
            {
                Id = novo.Id,
                Bairro = novo.Bairro,
                Tipo = novo.Tipo,
                QuantidadeKg = novo.QuantidadeKg,
                DataHora = novo.DataHora
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var d = await _context.Descartes.FindAsync(id);
            if (d == null) return false;

            _context.Descartes.Remove(d);
            await _context.SaveChangesAsync();
            return true;
        }
    }

    // Classe auxiliar dentro do mesmo arquivo (evita erro de referência)
    public class PagedResult<T>
    {
        public int TotalItems { get; set; }
        public List<T> Items { get; set; } = new();
    }
}
