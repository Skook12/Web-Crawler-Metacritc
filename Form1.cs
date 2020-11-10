using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace wcr
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {


        }

        private void button1_Click(object sender, EventArgs e)
        {

            var wc = new WebClient();
            string pagina = wc.DownloadString("https://www.metacritic.com/browse/games/release-date/new-releases/all/date");

            var htmlDoc = new HtmlAgilityPack.HtmlDocument();
            htmlDoc.LoadHtml(pagina);

            dataGridView1.Rows.Clear();
            GameContext GameC = new GameContext();
            GameC.Iniciar();

            string id = string.Empty;
            string nome = string.Empty;
            string nota = string.Empty;
            string link = string.Empty;


            var dados = htmlDoc.DocumentNode.Descendants("td").Where(node => node.GetAttributeValue("class", "").Equals("clamp-summary-wrap")).ToList<HtmlNode>();

            foreach (HtmlNode node in dados)
            {
                nome = node.Descendants("a").Where(n => n.GetAttributeValue("class", "").Equals("title")).ToList()[0].ChildNodes[0].InnerText;
                try
                {
                    nota = node.Descendants("div").Where(n => n.GetAttributeValue("class", "").Equals("metascore_w large game positive")).ToList()[0].ChildNodes[0].InnerText;
                }
                catch (System.ArgumentOutOfRangeException)
                {
                    try
                    {
                        nota = node.Descendants("div").Where(n => n.GetAttributeValue("class", "").Equals("metascore_w large game mixed")).ToList()[0].ChildNodes[0].InnerText;
                    }
                    catch (System.ArgumentOutOfRangeException)
                    {
                        try
                        {
                            nota = node.Descendants("div").Where(n => n.GetAttributeValue("class", "").Equals("metascore_w large game negative")).ToList()[0].ChildNodes[0].InnerText;
                        }
                        catch (System.ArgumentOutOfRangeException)
                        {

                        }
                    }
                }

                
                if (!string.IsNullOrEmpty(nome))
                {
                    Game g = new Game { Nome = nome, Nota = nota };
                    GameC.Games.Add(g);
                    GameC.SaveChanges();
                    dataGridView1.Rows.Add(nome, nota);
                }

            }
        }
    }
}