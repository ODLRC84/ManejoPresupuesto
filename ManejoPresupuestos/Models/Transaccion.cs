using System.ComponentModel.DataAnnotations;

namespace ManejoPresupuestos.Models
{
    public class Transaccion
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        [Display(Name = "Fecha Transaccion")]
        [DataType(DataType.DateTime)]
        public DateTime FechaTransaccion { get; set; } = DateTime.Today;
            //DateTime.Parse(DateTime.Now.ToString("yyy-MM-dd hh:MM tt"));
        public decimal Monto { get; set; }
        [Range(1, maximum: int.MaxValue, ErrorMessage = "Debe Seleccionar una Categoría")]
        [Display(Name = "Categoría")]
        public int CategoriaId { get; set; }
        [StringLength(maximumLength: 100, ErrorMessage = "La nota no debe pasar de {1} caracteres")]
        public string Nota { get; set; }
        [Range(1, maximum: int.MaxValue, ErrorMessage = "Debe Seleccionar una Cuenta")]
        [Display(Name = "Cuentas")]
        public int CuentaId { get; set; }
        [Display(Name = "Tipo Operación")]
        public TipoOperacion TipoOperacionId { get; set; } = TipoOperacion.Ingreso;

        public string Cuenta { get; set; }
        public string Categoria { get; set; }
    }
}
