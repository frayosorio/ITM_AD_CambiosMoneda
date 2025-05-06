using apiCambiosMoneda.Aplicacion.Servicios;
using apiCambiosMoneda.Core.Interfaces.Repositorios;
using apiCambiosMoneda.Core.Interfaces.Servicios;
using apiCambiosMoneda.Dominio.Entidades;
using Moq;

namespace apiCambiosMoneda.Test
{
    public class MonedaServicioTest
    {
        private readonly IMonedaServicio monedaServicio;
        private readonly Mock<IMonedaRepositorio> monedaRepositorioMock;


        public MonedaServicioTest()
        {
            monedaRepositorioMock = new Mock<IMonedaRepositorio>();
            monedaServicio = new MonedaServicio(monedaRepositorioMock.Object);
        }

        [Fact]
        public async void ObtenerHistorialCambios_DeberiaRetornarListaCambios()
        {
            //arrange
            var idMoneda = 1;
            var desde = new DateTime(2025, 04, 01);
            var hasta = new DateTime(2025, 04, 15);

            var listaCambios = new List<CambioMoneda>
            {
                new CambioMoneda{ IdMoneda=idMoneda, Fecha=new DateTime(2025, 04, 01), Cambio=4100 },
                new CambioMoneda{ IdMoneda=idMoneda, Fecha=new DateTime(2025, 04, 02), Cambio=4105 },
                new CambioMoneda{ IdMoneda=idMoneda, Fecha=new DateTime(2025, 04, 03), Cambio=4099 }
            }.AsEnumerable();

            monedaRepositorioMock.Setup(repositorio => repositorio.ObtenerHistorialCambios(idMoneda, desde, hasta))
                .ReturnsAsync(listaCambios);

            //act
            var resultado = await monedaServicio.ObtenerHistorialCambios(idMoneda, desde, hasta);

            //assert
            Assert.Equal(3, resultado.ToList().Count);
            Assert.Equal(4100, resultado.First().Cambio);
            Assert.Equal(4099, resultado.Last().Cambio);

        }

        [Fact]
        public async void ObtenerPaisesPorMoneda_DeberiaRetornarListaDePaises()
        {
            //arrange
            var idMoneda = 1;

            var listaPaises = new List<Pais> {
                new Pais{ Id=1, Nombre="Colombia", CodigoAlfa2="CO"},
                new Pais{ Id=2, Nombre="Perú", CodigoAlfa2="PE"},
                new Pais{ Id=3, Nombre="Brasil", CodigoAlfa2="BR"},
                new Pais{ Id=4, Nombre="Francia", CodigoAlfa2="FR"}
            };

            monedaRepositorioMock.Setup(repositorio => repositorio.ObtenerPaisesPorMoneda(idMoneda))
                .ReturnsAsync(listaPaises);

            //act
            var resultado = await monedaServicio.ObtenerPaisesPorMoneda(idMoneda);

            //assert
            Assert.Equal(resultado, listaPaises);
            monedaRepositorioMock.Verify(repositorio => repositorio.ObtenerPaisesPorMoneda(idMoneda), Times.Once);
        }


        [Fact]
        public async void ObtenerPaisesPorMoneda_RepositorioLanzaExcepcion_PropagaExcepcion()
        {
            //arrange
            var idMoneda = 3;

            monedaRepositorioMock.Setup(repositorio => repositorio.ObtenerPaisesPorMoneda(idMoneda))
                .ThrowsAsync(new Exception("Error en la base de datos"));

            //act
            var resultado = await Assert.ThrowsAsync<Exception>(() => monedaServicio.ObtenerPaisesPorMoneda(idMoneda));

            //assert
            monedaRepositorioMock.Verify(repositorio => repositorio.ObtenerPaisesPorMoneda(idMoneda), Times.Once);
        }

        [Fact]
        //Prueba 1 Analisis de Inversión: Ante cambios mayores al umbral, aparece la nueva tendencia
        public async void AnalizarInversionDolar_GeneraNuevaTendenciaCuandoUmbralSuperado()
        {
            //arrange
            var moneda = new Moneda
            {
                Id = 1,
                Sigla = "COP",
                Nombre = "Peso Colombiano"
            };
            monedaRepositorioMock.Setup(repositorio => repositorio.Buscar(1, "COP"))
                                .ReturnsAsync(new[]
                                {
                                    moneda
                                });

            monedaRepositorioMock.Setup(repositorio => repositorio.ObtenerHistorialCambios(
                                                    moneda.Id,
                                                    It.IsAny<DateTime>(),
                                                    It.IsAny<DateTime>()))
                                   .ReturnsAsync(new List<CambioMoneda>
                                   {
                                        new CambioMoneda { Fecha=new DateTime(2025, 1, 1), Cambio=1.00 },
                                        new CambioMoneda { Fecha=new DateTime(2025, 1, 2), Cambio=1.05 }, //+ 5%
                                        new CambioMoneda { Fecha=new DateTime(2025, 1, 3), Cambio=1.10 }, //+ 4.7%
                                        new CambioMoneda { Fecha=new DateTime(2025, 1, 4), Cambio=1.05 } //+ 4.7%
                                   });
            //act
            var resultado = await monedaServicio.AnalizarInversionDolar("COP", DateTime.MinValue, DateTime.MaxValue);

            //assert
            Assert.NotEmpty(resultado);
            Assert.All(resultado, r => Assert.Contains(r.recomendacion, new[] { "Vender USD", "Comprar USD" }));

        }


        [Fact]
        //Prueba 2 Analisis de Inversión: Ante cambios menortes al umbral, no aparece nueva tendencia
        public async void AnalizarInversionDolar_NoGeneraNuevaTendenciaCuandoUmbralNoSuperado()
        {
            //arrange
            var moneda = new Moneda
            {
                Id = 1,
                Sigla = "COP",
                Nombre = "Peso Colombiano"
            };
            monedaRepositorioMock.Setup(repositorio => repositorio.Buscar(1, "COP"))
                                .ReturnsAsync(new[]
                                {
                                    moneda
                                });

            monedaRepositorioMock.Setup(repositorio => repositorio.ObtenerHistorialCambios(
                                                    moneda.Id,
                                                    It.IsAny<DateTime>(),
                                                    It.IsAny<DateTime>()))
                                   .ReturnsAsync(new List<CambioMoneda>
                                   {
                                        new CambioMoneda { Fecha=new DateTime(2025, 1, 1), Cambio=1.00 },
                                        new CambioMoneda { Fecha=new DateTime(2025, 1, 2), Cambio=1.003 }, //+ 0.3%
                                        new CambioMoneda { Fecha=new DateTime(2025, 1, 3), Cambio=1.005 } //+ 0.1%
                                   });

            //act
            var resultado = await monedaServicio.AnalizarInversionDolar("COP", DateTime.MinValue, DateTime.MaxValue);

            //assert
            Assert.Single(resultado);
            Assert.Equal("** sin cambio **", resultado.First().recomendacion);

        }

        [Fact]
        //Prueba 3 Analisis de Inversión: Moneda no existe, lanza excepcion
        public async void AnalizarInversionDolar_LanzaExcepcionMonedaNoExiste()
        {
            //arrange
            monedaRepositorioMock.Setup(repositorio => repositorio.Buscar(1, "XXX"))
                                .ReturnsAsync(Array.Empty<Moneda>());

            //act
            await Assert.ThrowsAsync<Exception>(() => monedaServicio.AnalizarInversionDolar("XXX", DateTime.MinValue, DateTime.MaxValue));
        }

        [Fact]
        //Prueba 4 Analisis de Inversión: Tendecias Esperadas
        public async void AnalizarInversionDolar_TendenciasEsperadas()
        {
            //arrange
            var moneda = new Moneda
            {
                Id = 1,
                Sigla = "COP",
                Nombre = "Peso Colombiano"
            };
            monedaRepositorioMock.Setup(repositorio => repositorio.Buscar(1, "COP"))
                                .ReturnsAsync(new[]
                                {
                                    moneda
                                });

            monedaRepositorioMock.Setup(repositorio => repositorio.ObtenerHistorialCambios(
                                                    moneda.Id,
                                                    It.IsAny<DateTime>(),
                                                    It.IsAny<DateTime>()))
                                   .ReturnsAsync(new List<CambioMoneda>
                                   {
                                        new CambioMoneda { Fecha=new DateTime(2025, 1, 1), Cambio=1.00 },
                                        new CambioMoneda { Fecha=new DateTime(2025, 1, 2), Cambio=1.05 }, //+ 5%
                                        new CambioMoneda { Fecha=new DateTime(2025, 1, 3), Cambio=1.10 }, //+ 4.7%
                                        new CambioMoneda { Fecha=new DateTime(2025, 1, 4), Cambio=1.05 },
                                        new CambioMoneda { Fecha=new DateTime(2025, 1, 5), Cambio=1.00 },
                                        new CambioMoneda { Fecha=new DateTime(2025, 1, 6), Cambio=0.95 },
                                        new CambioMoneda { Fecha=new DateTime(2025, 1, 7), Cambio=1.00 },
                                        new CambioMoneda { Fecha=new DateTime(2025, 1, 8), Cambio=1.05 }
                                   });
            //act
            var resultado = await monedaServicio.AnalizarInversionDolar("COP", DateTime.MinValue, DateTime.MaxValue)
                ;

            //assert
            Assert.Equal(resultado.Count(), 3);
            Assert.Equal(resultado.ToList()[1].fechaDesde, new DateTime(2025, 1, 3));
            Assert.Equal(resultado.ToList()[1].fechaHasta, new DateTime(2025, 1, 6));
            Assert.Equal(resultado.ToList()[1].recomendacion, "Comprar USD");

        }

    }

}