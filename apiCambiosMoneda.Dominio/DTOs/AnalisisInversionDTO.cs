namespace apiCambiosMoneda.Dominio.DTOs
{
    public class AnalisisInversionDTO
    {
        public string moneda {  get; set; }
        public DateTime fechaDesde { get; set; }
        public DateTime fechaHasta { get; set; }
        public string recomendacion  { get; set; }

    }
}
