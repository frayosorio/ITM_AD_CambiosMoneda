using apiCambiosMoneda.Dominio.Entidades;
using apiCambiosMoneda.Core.Interfaces.Servicios;
using Microsoft.AspNetCore.Mvc;
using apiCambiosMoneda.Dominio.DTOs;

namespace apiCambiosMoneda.Presentacion.Controllers
{

    [ApiController]
    [Route("api/monedas")]
    public class MonedaControlador : ControllerBase
    {
        private readonly IMonedaServicio servicio;

        public MonedaControlador(IMonedaServicio servicio)
        {
            this.servicio = servicio;
        }

        [HttpGet("listar")]
        public async Task<ActionResult<IEnumerable<Moneda>>> ObtenerTodos()
        {
            return Ok(await servicio.ObtenerTodos());
        }

        [HttpGet("obtener/{Id}")]
        public async Task<ActionResult<Moneda>> Obtener(int Id)
        {
            return Ok(await servicio.Obtener(Id));
        }

        [HttpGet("buscar/{Tipo}/{Dato}")]
        public async Task<ActionResult<Moneda>> Buscar(int Tipo, string Dato)
        {
            return Ok(await servicio.Buscar(Tipo, Dato));
        }

        [HttpPost("agregar")]
        public async Task<ActionResult<Moneda>> Agregar([FromBody] Moneda Moneda)
        {
            return Ok(await servicio.Agregar(Moneda));
        }

        [HttpPut("modificar")]
        public async Task<ActionResult<Moneda>> Modificar([FromBody] Moneda Moneda)
        {
            return Ok(await servicio.Modificar(Moneda));
        }

        [HttpDelete("eliminar/{Id}")]
        public async Task<ActionResult<bool>> Eliminar(int Id)
        {
            return Ok(await servicio.Eliminar(Id));
        }

        /********** CAMBIOS **********/

        [HttpGet("cambios/{Id}")]
        public async Task<ActionResult<IEnumerable<CambioMoneda>>> ObtenerCambios(int Id)
        {
            return Ok(await servicio.ObtenerCambios(Id));
        }

        [HttpGet("cambios/buscar/{Id}/{Fecha}")]
        public async Task<ActionResult<CambioMoneda>> BuscarCambios(int IdMoneda, DateTime Fecha)
        {
            return Ok(await servicio.BuscarCambio(IdMoneda, Fecha));
        }

        [HttpPost("cambios")]
        public async Task<ActionResult<CambioMoneda>> AgregarCambio([FromBody] CambioMoneda Cambio)
        {
            return Ok(await servicio.AgregarCambio(Cambio));
        }

        [HttpPut("cambios")]
        public async Task<ActionResult<CambioMoneda>> ModificarCambio([FromBody] CambioMoneda Cambio)
        {
            return Ok(await servicio.ModificarCambio(Cambio));
        }

        [HttpDelete("cambios/{IdMoneda}/{Fecha}")]
        public async Task<ActionResult<bool>> EliminarCambio(int IdMoneda, DateTime Fecha)
        {
            return Ok(await servicio.EliminarCambio(IdMoneda, Fecha));
        }

        /********** CONSULTAS **********/

        [HttpGet("cambios/actual/{IdMoneda}")]
        public async Task<ActionResult<CambioMoneda>> ObtenerCambioActual(int IdMoneda)
        {
            return Ok(await servicio.ObtenerCambioActual(IdMoneda));
        }

        [HttpGet("cambios/historial/{IdMoneda}/{Desde}/{Hasta}")]
        public async Task<ActionResult<IEnumerable<CambioMoneda>>> ObtenerHistorialCambios(int IdMoneda, DateTime Desde, DateTime Hasta)
        {
            return Ok(await servicio.ObtenerHistorialCambios(IdMoneda, Desde, Hasta));
        }

        [HttpGet("paises/{IdMoneda}")]
        public async Task<ActionResult<IEnumerable<Pais>>> ObtenerPaisesPorMoneda(int IdMoneda)
        {
            return Ok(await servicio.ObtenerPaisesPorMoneda(IdMoneda));
        }

        /********** ANALISIS **********/
        [HttpGet("analisis/{siglaMoneda}/{desde}/{hasta}")]
        public async Task<IEnumerable<AnalisisInversionDTO>> AnalizarInversionDolar(string siglaMoneda, DateTime desde, DateTime hasta)
        {
            return await servicio.AnalizarInversionDolar(siglaMoneda, desde, hasta);
        }

    }
}
