using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Automação_de_Valores_Steam.Modelos_API;
using Newtonsoft.Json;
using Npgsql;
using RestSharp;

namespace Automação_de_Valores_Steam
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            this.dataGridView1.Columns["InvestidoEm"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.dataGridView1.Columns["Quantidade"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.dataGridView1.Columns["PrecoPago"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dataGridView1.Columns["PrecoAtual"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dataGridView1.Columns["PercLucro"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dataGridView1.Columns["TotalPago"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dataGridView1.Columns["ValorTotalAtual"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dataGridView1.Columns["LucroBruto"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dataGridView1.Columns["LucroLiquido"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            CarregarDados();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            CarregarDados();
        }

        private List<Items_Investidos> Buscar_Items_Banco()
        {

            List<Items_Investidos> items = new List<Items_Investidos>();

            string connectionString = "Server=localhost;Database=steam;User Id=postgres;Password=123;";

            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (var command = new NpgsqlCommand("" +
                    "SELECT \"Nome\", \"Quantidade\", \"Valor_Pago\", \"Investido_Em\" " +
                    "FROM public.\"Items_Investidos\" " +
                    "ORDER BY \"Investido_Em\" ASC", connection))
                {
                    using (var dr = command.ExecuteReader())
                    {
                        if (dr.HasRows == true)
                        {
                            int colunaItem = dr.GetOrdinal("Nome");
                            int colunaQuantidade = dr.GetOrdinal("Quantidade");
                            int colunaValorPago = dr.GetOrdinal("Valor_Pago");
                            int colunaInvestidoEm = dr.GetOrdinal("Investido_Em");

                            while (dr.Read())
                            {
                                Items_Investidos item = new Items_Investidos();

                                item.Nome = dr.GetString(colunaItem);
                                item.Quantidade = dr.GetInt32(colunaQuantidade);
                                item.Valor_Pago = dr.GetDecimal(colunaValorPago);
                                item.Investido_Em = dr.GetDateTime(colunaInvestidoEm);

                                items.Add(item);
                            }
                        }
                    }
                }
            }

            return items;
        }

        public void CarregarDados()
        {
            dataGridView1.Rows.Clear();

            List<Items_Investidos> items = Buscar_Items_Banco();

            decimal totalInvestido = 0;
            decimal valorVendaTotalAtual = 0;
            decimal lucroBrutoTotal = 0;
            decimal lucroLiquidoTotal = 0;

            for (int i = 0; i < items.Count; i++)
            {
                try
                {
                    Items_Steam itemSteam = new Items_Steam();

                    var Uri = new RestClient("https://steamcommunity.com/");
                    var request = new RestRequest("market/priceoverview/?appid=730&currency=3&currency=7&market_hash_name=" + items[i].Nome, Method.GET);
                    request.AddHeader("x-api-key", "B3D70D284DA36A07062CCD69FB69754E");
                    var resultado = Uri.Execute(request);
                    itemSteam = JsonConvert.DeserializeObject<Items_Steam>(resultado.Content);

                    decimal percLucro = decimal.Parse(itemSteam.Menor_Preco_Atual.Remove(0, 2)) * 100 / items[i].Valor_Pago;

                    decimal totalPago = items[i].Quantidade * items[i].Valor_Pago;
                    totalInvestido += totalPago;

                    decimal valorTotalAtual = items[i].Quantidade * decimal.Parse(itemSteam.Menor_Preco_Atual.Remove(0, 2));
                    valorVendaTotalAtual += valorTotalAtual;

                    decimal lucroBruto = valorTotalAtual - totalPago;
                    lucroBrutoTotal += lucroBruto;

                    decimal lucroLiquido = (valorTotalAtual * (decimal)0.875) - totalPago;
                    lucroLiquidoTotal += lucroLiquido;


                    dataGridView1.Rows.Add(items[i].Nome, items[i].Investido_Em.ToString("dd-MM-yyyy"), items[i].Quantidade, "R$ " + items[i].Valor_Pago.ToString("0.00"), itemSteam.Menor_Preco_Atual, percLucro.ToString("0.00") + "%", "R$ " + totalPago.ToString("0.00"), "R$ " + valorTotalAtual.ToString("0.00"), "R$ " + lucroBruto.ToString("0.00"), "R$ " + lucroLiquido.ToString("0.00"));
                }
                catch
                {
                    MessageBox.Show("Limite de requisições atingida!","Falha");
                    return;
                }
            }

            label5.Text = "R$ " + totalInvestido.ToString("0.00");
            label6.Text = "R$ " + valorVendaTotalAtual.ToString("0.00");
            label7.Text = "R$ " + lucroBrutoTotal.ToString("0.00");
            label8.Text = "R$ " + lucroLiquidoTotal.ToString("0.00");
        }
    }
}
