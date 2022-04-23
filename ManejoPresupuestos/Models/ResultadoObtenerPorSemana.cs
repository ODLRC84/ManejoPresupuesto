namespace ManejoPresupuestos.Models
{
    public class ResultadoObtenerPorSemana
    {
        public int Semana { get; set; }
        public int Monto { get; set; }
        public TipoOperacion TipoOperacionesId { get; set; }
        public decimal Ingresos { get; set; }
        public decimal Gastos { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
    }
}
