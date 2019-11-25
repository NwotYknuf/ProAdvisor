using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace ProAdvisor.app {

    /*
     * Scrapper pour le site Truspilot.
     * A besoin de l'adresse du site d'une entreprise pour trouver des avis
     */

    public class TrustPilotScrapper : Bot {

        private HttpClient client;

        public TrustPilotScrapper() {
            client = new HttpClient();
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        }

        public override async Task<List<Review>> getReviews(string research) {

            List<Review> res = new List<Review>();
            bool stop = false;
            int page = 1;
            HtmlDocument doc;
            while (!stop) { //On itère sur les pages de commentaire

                string url = "https://fr.trustpilot.com/review/" + research + "?page=" + page.ToString();
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode) {
                    doc = new HtmlDocument();
                    doc.LoadHtml(response.Content.ReadAsStringAsync().Result);
                    HtmlNodeCollection comment_nodes = doc.DocumentNode.SelectNodes("//div[@class='review-card  ']");

                    foreach (HtmlNode node in comment_nodes) {
                        string auteur = node.SelectSingleNode(".//div[@class='consumer-information__name']").InnerText.Trim();
                        HtmlNode rating_node = node.SelectSingleNode(".//div[@class='star-rating star-rating--medium']/img");
                        //format de la note : 1 étoile mauvais , 2 étoiles bas, ...
                        string note_str = rating_node.Attributes["alt"].Value;
                        //Le site utilise un bout de script pour afficher la date au bon format celon le pays. 
                        //On peut récuperer les paramètres du script pour avoir la date
                        string date_str = node.SelectSingleNode(".//div[@class='review-content-header__dates']/script").InnerHtml.Trim();
                        string commentaire = node.SelectSingleNode(".//p[@class='review-content__text']").InnerText.Trim();

                        Regex date_reg = new Regex(@"\d{4}-\d{2}-\d{2}");
                        date_str = date_reg.Match(date_str).Value; //date en yyyy-MM-dd

                        Regex note_reg = new Regex(@"\d");
                        note_str = note_reg.Match(note_str).Value;

                        DateTime date = DateTime.ParseExact(date_str, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                        double note = Double.Parse(note_str);

                        Review review = new Review(date, commentaire, note, "Trustpilot.com", auteur);
                        res.Add(review);
                    }

                } else { //Si la requête echoue
                    throw new Exception($"Requête echouée pour l'url : {url}");
                }

                HtmlNode bouton_suivant = doc.DocumentNode.SelectSingleNode("//a[@rel='next']");

                if (bouton_suivant == null) { //Si la page ne contient plus de boutton suivant on a finit le parcours
                    stop = true;
                }

                page++;
            }

            return res;

        }

    }
}