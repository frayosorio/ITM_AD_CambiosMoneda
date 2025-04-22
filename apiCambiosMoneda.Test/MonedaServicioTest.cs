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


    }
}
