using apiCambiosMoneda.Core.Interfaces.Repositorios;
using apiCambiosMoneda.Dominio.Entidades;
using apiCambiosMoneda.Infraestructura.Repositorio.Contextos;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace apiCambiosMoneda.Infraestructura.Repositorio.Repositorios
{
    public class MonedaRepositorio : IMonedaRepositorio
    {

        private CambiosMonedaContext context;

        public MonedaRepositorio(CambiosMonedaContext context)
        {
            this.context = context;
        }

        public async Task<Moneda> Obtener(int Id)
        {
            return (Moneda)(await context.Monedas.FindAsync(Id));
        }
        public async Task<IEnumerable<Moneda>> ObtenerTodos()
        {
            return await context.Monedas.ToArrayAsync();
        }

        public async Task<IEnumerable<Moneda>> Buscar(int Tipo, string Dato)
        {
            return await context.Monedas
                                   .Where(item => (Tipo == 0 && item.Nombre.Contains(Dato))
                                   || (Tipo == 1 && item.Sigla.Contains(Dato))
                                   || (Tipo == 2 && item.Emisor.Contains(Dato)))// Filtrar elementos 
                                   .ToListAsync(); // Convertir a una lista IEnumerable<Monedas>
        }

        public async Task<Moneda> Agregar(Moneda Moneda)
        {
            context.Monedas.Add(Moneda);
            await context.SaveChangesAsync();
            return Moneda;
        }

        public async Task<Moneda> Modificar(Moneda Moneda)
        {
            var monedaExistente = await context.Monedas.FindAsync(Moneda.Id);
            if (monedaExistente == null)
            {
                return null;
            }
            context.Entry(monedaExistente).CurrentValues.SetValues(Moneda);
            await context.SaveChangesAsync();
            return (Moneda)(await context.Monedas.FindAsync(Moneda.Id));
        }

        public async Task<bool> Eliminar(int Id)
        {
            var monedaExistente = await context.Monedas.FindAsync(Id);
            if (monedaExistente == null)
            {
                return false;
            }

            context.Monedas.Remove(monedaExistente);
            await context.SaveChangesAsync();
            return true;
        }

        /********** CAMBIOS **********/

        public async Task<IEnumerable<CambioMoneda>> ObtenerCambios(int IdMoneda)
        {
            return await context.CambiosMoneda
                                    .Where(cm => cm.IdMoneda == IdMoneda)
                                     .Include(cm => cm.Moneda) // Incluye la Moneda relacionado con el Cambio
                                    .ToArrayAsync();
        }

        public async Task<CambioMoneda> BuscarCambio(int IdMoneda, DateTime Fecha)
        {
            return await context.CambiosMoneda.FirstOrDefaultAsync(cm => cm.IdMoneda == IdMoneda && cm.Fecha == Fecha);
        }

        public async Task<CambioMoneda> AgregarCambio(CambioMoneda Cambio)
        {
            context.CambiosMoneda.Add(Cambio);
            await context.SaveChangesAsync();
            return Cambio;
        }

        public async Task<CambioMoneda> ModificarCambio(CambioMoneda Cambio)
        {
            var cambioExistente = await context.CambiosMoneda.FirstOrDefaultAsync(cm => cm.IdMoneda == Cambio.IdMoneda && cm.Fecha == Cambio.Fecha);
            if (cambioExistente == null)
            {
                return null;
            }
            context.Entry(cambioExistente).CurrentValues.SetValues(cambioExistente);
            await context.SaveChangesAsync();
            return (CambioMoneda)(await context.CambiosMoneda.FirstOrDefaultAsync(cm => cm.IdMoneda == Cambio.IdMoneda && cm.Fecha == Cambio.Fecha));
        }

        public async Task<bool> EliminarCambio(int IdMoneda, DateTime Fecha)
        {
            var cambioExistente = await context.CambiosMoneda.FirstOrDefaultAsync(cm => cm.IdMoneda == IdMoneda && cm.Fecha == Fecha);
            if (cambioExistente == null)
            {
                return false;
            }

            context.CambiosMoneda.Remove(cambioExistente);
            await context.SaveChangesAsync();
            return true;
        }

        /********** CONSULTAS **********/

        public async Task<IEnumerable<CambioMoneda>> ObtenerHistorialCambios(int IdMoneda, DateTime Desde, DateTime Hasta)
        {
            return await context.CambiosMoneda
                                   .Where(cm => cm.IdMoneda == IdMoneda && Desde <= cm.Fecha && cm.Fecha <= Hasta)
                                   .Include(cm => cm.Moneda) // Incluye la Moneda relacionado con el Cambio
                                   .ToArrayAsync();
        }

        public async Task<CambioMoneda> ObtenerCambioActual(int IdMoneda)
        {
            return await context.CambiosMoneda
                                .Where(cm => cm.IdMoneda == IdMoneda)
                                .OrderByDescending(cm => cm.Fecha)
                                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Pais>> ObtenerPaisesPorMoneda(int IdMoneda)
        {
            return await context.Paises
                                .Where(item => item.Moneda.Id == IdMoneda) // Filtrar elementos 
                                .ToListAsync(); // Convertir a una lista IEnumerable<Pais>
        }
    }
}
