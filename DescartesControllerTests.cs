using LixoZero.Controllers;
using LixoZero.Services;
using LixoZero.Models;
using LixoZero.ViewModels;
using LixoZero.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace LixoZero.Tests
{
    public class DescartesControllerTests
    {
        private DbContextOptions<AppDbContext> GetInMemoryDbOptions()
        {
            return new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
        }

        private DescartesController GetController(DbContextOptions<AppDbContext> options)
        {
            var context = new AppDbContext(options);
            var service = new DescarteService(context);
            return new DescartesController(service);
        }

        [Fact]
public async Task GetDescartes_DeveRetornar200()
{
    // Arrange
    var options = GetInMemoryDbOptions();
    using (var context = new AppDbContext(options))
    {
        context.Descartes.Add(new Descarte
        {
            Bairro = "Teste",
            Tipo = TipoMaterial.Metal,
            QuantidadeKg = 3
        });
        context.SaveChanges();
    }

    var controller = GetController(options);

    // Act
    var result = await controller.GetDescartes();

    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result.Result);
    var pagedResult = Assert.IsType<PagedResult<DescarteDto>>(okResult.Value);

    Assert.NotNull(pagedResult.Items);
    Assert.Single(pagedResult.Items);
}


        [Fact]
        public async Task CreateDescarte_DeveRetornar201()
        {
            // Arrange
            var options = GetInMemoryDbOptions();
            var controller = GetController(options);

            var dto = new DescarteDto
            {
                Bairro = "Centro",
                Tipo = TipoMaterial.Papel,
                QuantidadeKg = 5.0
            };

            // Act
            var result = await controller.CreateDescarte(dto);

            // Assert
            var created = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(201, created.StatusCode);
        }

        [Fact]
        public async Task CreateDescarte_DeveRetornar400_SeInvalido()
        {
            // Arrange
            var options = GetInMemoryDbOptions();
            var controller = GetController(options);
            controller.ModelState.AddModelError("Bairro", "Obrigatório");

            var dto = new DescarteDto
            {
                Bairro = "",
                Tipo = TipoMaterial.Papel,
                QuantidadeKg = 0
            };

            // Act
            var result = await controller.CreateDescarte(dto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GetDescarteById_DeveRetornar404_SeNaoEncontrado()
        {
            var options = GetInMemoryDbOptions();
            var controller = GetController(options);

            var result = await controller.GetDescarteById(999);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task DeleteDescarte_DeveRetornar204()
        {
            // Arrange
            var options = GetInMemoryDbOptions();
            int id;
            using (var context = new AppDbContext(options))
            {
                var descarte = new Descarte
                {
                    Bairro = "Teste",
                    Tipo = TipoMaterial.Plastico,
                    QuantidadeKg = 1
                };
                context.Descartes.Add(descarte);
                context.SaveChanges();
                id = descarte.Id;
            }

            var controller = GetController(options);

            // Act
            var result = await controller.DeleteDescarte(id);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteDescarte_DeveRetornar404_SeNaoExistir()
        {
            var options = GetInMemoryDbOptions();
            var controller = GetController(options);

            var result = await controller.DeleteDescarte(999);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
