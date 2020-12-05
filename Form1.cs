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
            string resumo = string.Empty;
            string imagem = string.Empty;

            var dados = htmlDoc.DocumentNode.Descendants("td").Where(node => node.GetAttributeValue("class", "").Equals("clamp-summary-wrap")).ToList<HtmlNode>();

            foreach (HtmlNode node in dados)
            {
                nome = node.Descendants("a").Where(n => n.GetAttributeValue("class", "").Equals("title")).ToList()[0].ChildNodes[0].InnerText;
                link = node.Descendants("a").ToList()[0].GetAttributeValue("href","");
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
                string linkCompleto;
                linkCompleto = "https://www.metacritic.com" + link;
                int aux = linkCompleto.Length - 15;
                linkCompleto = linkCompleto.Remove(aux, 15);

                string pagina2 = wc.DownloadString(linkCompleto);

                var htmlDoc2 = new HtmlAgilityPack.HtmlDocument();
                htmlDoc2.LoadHtml(pagina2);

                var dados2 = htmlDoc2.DocumentNode.Descendants("span").Where(node2 => node2.GetAttributeValue("class", "").Equals("blurb blurb_expanded")).ToList<HtmlNode>();
                if(dados2.Count>0 && dados2 != null)
                {
                    resumo = dados2[0].InnerText;
                }
                else
                {
                    resumo = "###NÃO ENCONTRADO###";
                }

                dados2 = htmlDoc2.DocumentNode.Descendants("img").Where(node2 => node2.GetAttributeValue("", "").Equals("")).ToList<HtmlNode>();
                if (dados2.Count > 0 && dados2 != null)
                {
                    imagem = dados2[29].OuterHtml;
                    imagem = imagem.Remove(0, 44);
                    int aux2 = 0;
                    for(int i=0; i < imagem.Length; i++)
                    {
                        if(imagem[i]==' ')
                        {
                            break;
                        }
                        aux2++;
                    }
                    int aux3=imagem.Length-aux2;
                    imagem = imagem.Remove(aux2-1, aux3+1);
                }
                else
                {
                    imagem = "###NÃO ENCONTRADO###";
                }

                if (!string.IsNullOrEmpty(nome))
                {
                    Game g = new Game { Nome = nome, Nota = nota ,Resumo = resumo , Imagem = imagem};
                    GameC.Games.Add(g);
                    GameC.SaveChanges();
                    dataGridView1.Rows.Add(nome, nota , linkCompleto,resumo,imagem);
                }

            }
        }
    }
}