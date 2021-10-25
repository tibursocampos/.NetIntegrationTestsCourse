using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Alura.CoisasAFazer.WebApp.Controllers;
using Alura.CoisasAFazer.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Alura.CoisasAFazer.Services.Handlers;
using Alura.CoisasAFazer.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Alura.CoisasAFazer.Core.Models;

namespace Alura.CoisasAFazer.Testes
{
    public class TarefasControllerEndpointCadastraTarefa
    {
        [Fact]
        public void DadaTarefaComInformacoesValidasDeveRetornar200()
        {
            //arrange
            var mockLogger = new Mock<ILogger<CadastraTarefaHandler>>();
            var mock = new Mock<IRepositorioTarefas>();
            mock.Setup(r => r.ObtemCategoriaPorId(20)).Returns(new Categoria(20, "Estudo"));     

            var controlador = new TarefasController(mock.Object, mockLogger.Object);
            var model = new CadastraTarefaVM();
            model.IdCategoria = 20;
            model.Titulo = "Estuda Xunit";
            model.Prazo = new DateTime(2019, 12, 31);

            //act
            var retorno = controlador.EndpointCadastraTarefa(model);

            //assert
            Assert.IsType<OkResult>(retorno); //retorna 200
        }

        [Fact]
        public void QuandoExcecaoForLancadaDeveRetornarStatusCode500()
        {
            //arrange
            var mockLogger = new Mock<ILogger<CadastraTarefaHandler>>();
            var mock = new Mock<IRepositorioTarefas>();
            mock.Setup(r => r.ObtemCategoriaPorId(20)).Returns(new Categoria(20, "Estudo"));
            mock.Setup(r => r.IncluirTarefas(It.IsAny<Tarefa[]>())).Throws(new Exception("Houve um erro"));

            var controlador = new TarefasController(mock.Object, mockLogger.Object);
            var model = new CadastraTarefaVM();
            model.IdCategoria = 20;
            model.Titulo = "Estuda Xunit";
            model.Prazo = new DateTime(2019, 12, 31);

            //act
            var retorno = controlador.EndpointCadastraTarefa(model);

            //assert
            Assert.IsType<StatusCodeResult>(retorno);
            var statusCodeRetornado = (retorno as StatusCodeResult).StatusCode;
            Assert.Equal(500, statusCodeRetornado);

        }
    }
}
