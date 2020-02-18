using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace ProAdvisor.app {

    public class AvisVerifiesScrapper : Bot, ISourceReview {

        private HttpClient client;

        public AvisVerifiesScrapper() {

            this.source = "AvisVerifies.com";
            client = new HttpClient();
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        }

        public async Task<List<Review>> findReviews(Entite entite, DateTime limitDate) {

            List<Review> reviews = new List<Review>();

            string researchString;

            try {
                researchString = ManipUrl.retireWWW(entite.getResearchString());
            } catch (PasDeSiteWebException) {
                throw new EntrepriseInconnueException();
            }

            int page = 1;
            bool done = false;

            while (!done) {

                string url = "https://www.avis-verifies.com/avis-clients/" + researchString + "?p=" + page.ToString();

                HttpResponseMessage response = await client.GetAsync(url);

                if (response.StatusCode == HttpStatusCode.OK) {

                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(await response.Content.ReadAsStringAsync());

                    HtmlNode commentZone = doc.DocumentNode.SelectSingleNode("//div[@class='panel panel-default section reviews']");

                    if (commentZone == null) { //On a pas trouvé le site
                        Console.WriteLine("pas de site");
                        throw new EntrepriseInconnueException();
                    }

                    HtmlNodeCollection commentNodes = commentZone.SelectNodes(".//div[@class='comment']");

                    foreach (HtmlNode commentNode in commentNodes) {

                        HtmlNode noteNode = commentNode.SelectSingleNode(".//span[@itemprop='ratingValue']");
                        double note = Double.Parse(noteNode.InnerText);

                        HtmlNode auteurNode = commentNode.SelectSingleNode(".//span[@itemprop='name']");
                        string auteur = auteurNode.InnerText;

                        HtmlNode dateNode = commentNode.SelectSingleNode(".//meta[@itemprop='datePublished']");
                        DateTime date = DateTime.ParseExact(dateNode.Attributes["content"].Value, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

                        HtmlNode commentaireNode = commentNode.SelectSingleNode(".//div[@itemprop='reviewBody']");
                        string commentaire = commentaireNode.InnerText;

                        reviews.Add(new ReviewBasic(entite.id, this.source, auteur, date, note, commentaire));
                    }

                    HtmlNode btnSuivant = doc.DocumentNode.SelectSingleNode("//ul[@class='pager']/li[4]");

                    if (btnSuivant.Attributes["class"] != null) {
                        done = true;
                    } else {
                        page++;
                    }

                } else {
                    throw new Exception("Erreur " + (int)response.StatusCode + " sur l'url : " + url);
                }
            }

            return reviews;
        }

        public override void destroy() {

        }
    }

}