using apiCambiosMoneda.Core.Interfaces.Repositorios;
using apiCambiosMoneda.Core.Interfaces.Servicios;
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


    }
}
