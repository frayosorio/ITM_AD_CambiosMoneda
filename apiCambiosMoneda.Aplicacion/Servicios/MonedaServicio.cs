using apiCambiosMoneda.Core.Interfaces.Repositorios;
using apiCambiosMoneda.Core.Interfaces.Servicios;
using apiCambiosMoneda.Dominio.DTOs;
using apiCambiosMoneda.Dominio.Entidades;

namespace apiCambiosMoneda.Aplicacion.Servicios
{
    public class MonedaServicio : IMonedaServicio
    {
        private readonly IMonedaRepositorio repositorio;

        public MonedaServicio(IMonedaRepositorio repositorio)
        {
            this.repositorio = repositorio;
        }

        public async Task<Moneda> Obtener(int Id)
        {
            return await repositorio.Obtener(Id);
        }

        public async Task<IEnumerable<Moneda>> ObtenerTodos()
        {
            return await repositorio.ObtenerTodos();
        }
        public async Task<IEnumerable<Moneda>> Buscar(int Tipo, string Dato)
        {
            return await repositorio.Buscar(Tipo, Dato);
        }

        public async Task<Moneda> Agregar(Moneda Moneda)
        {
            return await repositorio.Agregar(Moneda);
        }

        public async Task<Moneda> Modificar(Moneda Moneda)
        {
            return await repositorio.Modificar(Moneda);
        }

        public async Task<bool> Eliminar(int Id)
        {
            return await repositorio.Eliminar(Id);
        }

        /********** CAMBIOS **********/
        public async Task<IEnumerable<CambioMoneda>> ObtenerCambios(int IdMoneda)
        {
            return await repositorio.ObtenerCambios(IdMoneda);
        }

        public async Task<CambioMoneda> BuscarCambio(int IdMoneda, DateTime Fecha)
        {
            return await repositorio.BuscarCambio(IdMoneda, Fecha);
        }

        public async Task<CambioMoneda> AgregarCambio(CambioMoneda Cambio)
        {
            return await repositorio.AgregarCambio(Cambio);
        }

        public async Task<CambioMoneda> ModificarCambio(CambioMoneda Cambio)
        {
            return await repositorio.ModificarCambio(Cambio);
        }

        public async Task<bool> EliminarCambio(int IdMoneda, DateTime Fecha)
        {
            return await repositorio.EliminarCambio(IdMoneda, Fecha);
        }

        /********** CONSULTAS **********/

        public async Task<CambioMoneda> ObtenerCambioActual(int IdMoneda)
        {
            return await repositorio.ObtenerCambioActual(IdMoneda);
        }

        public async Task<IEnumerable<CambioMoneda>> ObtenerHistorialCambios(int IdMoneda, DateTime Desde, DateTime Hasta)
        {
            return await repositorio.ObtenerHistorialCambios(IdMoneda, Desde, Hasta);
        }

        public async Task<IEnumerable<Pais>> ObtenerPaisesPorMoneda(int IdMoneda)
        {
            return await repositorio.ObtenerPaisesPorMoneda(IdMoneda);
        }


        /********** ANALISIS **********/

        public async Task<IEnumerable<AnalisisInversionDTO>> AnalizarInversionDolar(string siglaMoneda, DateTime desde, DateTime hasta, double umbralPorcentual = 1.0)
        {
            var resultado = new List<AnalisisInversionDTO>();

            var moneda = (await repositorio.Buscar(1, siglaMoneda)).FirstOrDefault();
            if (moneda == null)
            {
                throw new Exception($"Moneda con sigla '{siglaMoneda}' no encontrada");
            }

            var cambios = (await repositorio.ObtenerHistorialCambios(moneda.Id, desde, hasta))
                            .OrderBy(c => c.Fecha)
                            .ToList();

            String tendenciaActual = null;
            DateTime? fechaInicio = null;

            for (int i = 1; i < cambios.Count; i++)
            {
                var anterior = cambios[i - 1];
                var actual = cambios[i];

                double porcentajaVariacion = Math.Abs(actual.Cambio - anterior.Cambio) / anterior.Cambio * 100;

                string nuevaTendencia = porcentajaVariacion >= umbralPorcentual ?
                    (actual.Cambio > anterior.Cambio ? "Vender USD" : "Comprar USD") : tendenciaActual ?? "** sin cambio **";


                if (nuevaTendencia != tendenciaActual)
                {
                    if (tendenciaActual != null && fechaInicio.HasValue)
                    {
                        resultado.Add(new AnalisisInversionDTO
                        {
                            moneda = $"{moneda.Sigla} - {moneda.Nombre}",
                            fechaDesde = fechaInicio.Value,
                            fechaHasta = anterior.Fecha,
                            recomendacion = tendenciaActual
                        });
                    }
                    fechaInicio = anterior.Fecha;
                    tendenciaActual = nuevaTendencia;
                }
            }
            resultado.Add(new AnalisisInversionDTO
            {
                moneda = $"{moneda.Sigla} - {moneda.Nombre}",
                fechaDesde = fechaInicio.Value,
                fechaHasta = cambios.Last().Fecha,
                recomendacion = tendenciaActual
            });

            return resultado;
        }

    }
}
