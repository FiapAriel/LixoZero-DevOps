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

        public async Task<PagedResult<DescarteDtoComId>> GetDescartesAsync(int page, int pageSize)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            var query = _context.Descartes
                .AsNoTracking()
                .OrderByDescending(d => d.DataHora);

            var totalItems = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(d => new DescarteDtoComId
                {
                    Id = d.Id,
                    Bairro = d.Bairro,
                    Tipo = d.Tipo,
                    QuantidadeKg = d.QuantidadeKg,
                    DataHora = d.DataHora
                })
                .ToListAsync();

            return new PagedResult<DescarteDtoComId>
            {
                Items = items,
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize
            };
        }

        // GET por Id 
        public async Task<DescarteDtoComId?> GetByIdAsync(int id)
        {
            return await _context.Descartes
                .AsNoTracking()
                .Where(d => d.Id == id)
                .Select(d => new DescarteDtoComId
                {
                    Id = d.Id,
                    Bairro = d.Bairro,
                    Tipo = d.Tipo,
                    QuantidadeKg = d.QuantidadeKg,
                    DataHora = d.DataHora
                })
                .FirstOrDefaultAsync();
        }

        // CREATE 
        public async Task<DescarteDtoComId> CreateAsync(DescarteDto dto)
        {
            if (dto.QuantidadeKg <= 0)
                throw new ArgumentException("A quantidade deve ser maior que zero.", nameof(dto.QuantidadeKg));

            var entity = new Descarte
            {
                Bairro = dto.Bairro,
                Tipo = dto.Tipo,
                QuantidadeKg = dto.QuantidadeKg,
                DataHora = dto.DataHora ?? DateTime.UtcNow
            };

            _context.Descartes.Add(entity);
            await _context.SaveChangesAsync();

            return new DescarteDtoComId
            {
                Id = entity.Id,
                Bairro = entity.Bairro,
                Tipo = entity.Tipo,
                QuantidadeKg = entity.QuantidadeKg,
                DataHora = entity.DataHora
            };
        }

        // DELETE 
        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.Descartes.FindAsync(id);
            if (entity is null) return false;

            _context.Descartes.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
