using Alura.CoisasAFazer.Core.Commands;
using Alura.CoisasAFazer.Core.Models;
using Alura.CoisasAFazer.Infrastructure;
using Alura.CoisasAFazer.Services.Handlers;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Linq;
using Xunit;

namespace Alura.CoisasAFazer.Testes
{
    public class CadastraTarefaHandlerExecute
    {
        [Fact]
        public void DadaTarefaComInfoValidasDeveIncluirNoBD()
        {
            //arrange
            var resultadoEsperado = new CommandResult(true);
            var comando = new CadastraTarefa("Estudar Xunit", new Categoria("Estudo"), new DateTime(2019, 12, 31));    
            
            var mock = new Mock<IRepositorioTarefas>();
            var repo = mock.Object;

            var mockLogger = new Mock<ILogger<CadastraTarefaHandler>>();   

            //Moq substitui este trecho
            //var options = new DbContextOptionsBuilder<DbTarefasContext>()
            //    .UseInMemoryDatabase("DbTarefasContext")
            //    .Options;
            //var contexto = new DbTarefasContext(options);
            //var repo = new RepositorioTarefa(contexto);

            var handler = new CadastraTarefaHandler(repo, mockLogger.Object);

            //act
            var result = handler.Execute(comando); //SUT >> CadastraTarefaHandlerExecute

            //assert
            //var tarefa = repo.ObtemTarefas(t => t.Titulo == "Estudar Xunit").FirstOrDefault();
            Assert.Equal(result.IsSuccess, resultadoEsperado.IsSuccess);          
        }

        //Delegate para ser usado no teste abaixo
        delegate void CapturaMensagemLog(LogLevel level, EventId eventId, object state, Exception exception, Func<object, Exception, string> function);

        [Fact]
        public void DadaTarefaComInforValidasDeveLogar()
        {
            //arrange
            var tituloTarefaEsperado = "Usar Moq para aprofundar conhecimento de API";
            var comando = new CadastraTarefa(tituloTarefaEsperado, new Categoria("Estudo"), new DateTime(2019, 12, 31));

            var mock = new Mock<IRepositorioTarefas>();       
            var mockLogger = new Mock<ILogger<CadastraTarefaHandler>>();

            LogLevel levelCapturado = LogLevel.Error;
            string mensagemCapturada = string.Empty;

            CapturaMensagemLog captura = (level, eventId, state, exception, function) =>
            {
                levelCapturado = level;
                mensagemCapturada = function(state, exception);
            };

            mockLogger.Setup(l => l.Log(It.IsAny<LogLevel>(), It.IsAny<EventId>(), It.IsAny<object>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>())).Callback(captura);

            var handler = new CadastraTarefaHandler(mock.Object, mockLogger.Object);

            //act
            handler.Execute(comando); //SUT >> CadastraTarefaHandlerExecute

            //assert
            Assert.Equal(LogLevel.Debug, levelCapturado);
            Assert.Contains(tituloTarefaEsperado, mensagemCapturada);

        }

        [Fact]
        public void QuandoExceptionForLancadaResultadoIsSuccessDeveSerFalso()
        {
            //arrange
            var comando = new CadastraTarefa("Estudar Xunit", new Categoria("Estudo"), new DateTime(2019, 12, 31));

            var mock = new Mock<IRepositorioTarefas>();
            mock.Setup(r => r.IncluirTarefas(It.IsAny<Tarefa[]>())).Throws(new Exception("Houve um erro na inclusão de tarefas."));
            var repo = mock.Object;

            var mockLogger = new Mock<ILogger<CadastraTarefaHandler>>();

            var handler = new CadastraTarefaHandler(repo, mockLogger.Object);

            //act
            CommandResult resultado = handler.Execute(comando); //SUT >> CadastraTarefaHandlerExecute

            //assert
            Assert.False(resultado.IsSuccess);
        }

        [Fact]
        public void QuandoExceptionForLancadaDeveLogarAMensagemDaExcecao()
        {
            //arrange
            var mensagemDeErroEsperada = "Houve um erro na inclusão de tarefas.";
            var excecaoEsperada = new Exception(mensagemDeErroEsperada);

            var comando = new CadastraTarefa("Estudar Xunit", new Categoria("Estudo"), new DateTime(2019, 12, 31));

            var mockLogger = new Mock<ILogger<CadastraTarefaHandler>>();

            var mock = new Mock<IRepositorioTarefas>();

            mock.Setup(r => r.IncluirTarefas(It.IsAny<Tarefa[]>())).Throws(excecaoEsperada);
            
            var repo = mock.Object;

            var handler = new CadastraTarefaHandler(repo, mockLogger.Object) ;

            //act
            CommandResult resultado = handler.Execute(comando);

            //assert
            mockLogger.Verify(l => l.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<object>(), excecaoEsperada, It.IsAny<Func<object, Exception, string>>()), Times.Once());
        }
    }
}

