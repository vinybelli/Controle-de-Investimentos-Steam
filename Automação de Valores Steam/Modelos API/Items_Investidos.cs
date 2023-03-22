using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automação_de_Valores_Steam.Modelos_API
{
    internal class Items_Investidos
    {
        public string Nome { get; set; }
        public int Quantidade { get; set; }
        public decimal Valor_Pago { get; set; }
        public DateTime Investido_Em { get; set; }
    }
}
