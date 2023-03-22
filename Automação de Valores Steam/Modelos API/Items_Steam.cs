using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Automação_de_Valores_Steam.Modelos_API
{
    internal class Items_Steam
    {
        [JsonProperty("lowest_price")]
        public string Menor_Preco_Atual { get; set; }
    }
}
