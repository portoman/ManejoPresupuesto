using Microsoft.AspNetCore.Mvc.Rendering;

namespace ManejoPresupuesto.Models
{
    public class CuentaCreacionViewModel:Cuenta
    {
        //Modelo que hereda de cuenta incluyendo el Inumerable de tipos de cuentas para desplegar el select
        public IEnumerable<SelectListItem> TiposCuentas { get; set; }
    }
}
