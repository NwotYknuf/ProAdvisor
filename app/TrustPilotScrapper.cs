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
            this.source = "TrustPilot.com";
            client = new HttpClient();
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        }

        public override async Task<List<Review>> getReviews(string research, DateTime limitDate) {

            List<Review> res = new List<Review>();
            bool stop = false;
            int page = 1;
            HtmlDocument doc;
            /*
             * Besoin des www. dans l'url
             * Pas oublier de gérer pour les https://
             */
            /*
            if (!research.StartsWith("www.")) {
                research = "www." + research;
            }
            */
            string url = "https://fr.trustpilot.com/review/" + research + "?page=1";
            HttpResponseMessage reponse = await client.GetAsync(url);
            if (reponse.IsSuccessStatusCode) {
                doc = new HtmlDocument();
                doc.LoadHtml(reponse.Content.ReadAsStringAsync().Result);
                research = doc.DocumentNode.SelectSingleNode(".//span[@class='badge-card__title']").InnerText.Trim();
            }

            while (!stop) { //On itère sur les pages de commentaire

                url = "https://fr.trustpilot.com/review/" + research + "?page=" + page.ToString();
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode) {
                    doc = new HtmlDocument();
                    doc.LoadHtml(response.Content.ReadAsStringAsync().Result);
                    HtmlNodeCollection comment_nodes = doc.DocumentNode.SelectNodes("//div[@class='review-card  ']");

                    if (comment_nodes == null) {
                        throw new PasDeCommentaireException("Pas d'avis trouvé pour : " + research);
                    }

                    foreach (HtmlNode node in comment_nodes) {

                        HtmlNode reported = node.SelectSingleNode(".//div[@class='review-report-banner']");

                        /*
                         * Si le commentaire est signalé on l'ignore
                         */
                        if (reported != null) {
                            continue;
                        }

                        string auteur = node.SelectSingleNode(".//div[@class='consumer-information__name']").InnerText.Trim();
                        HtmlNode rating_node = node.SelectSingleNode(".//div[@class='star-rating star-rating--medium']/img");
                        //format de la note : 1 étoile mauvais , 2 étoiles bas, ...
                        string note_str = rating_node.Attributes["alt"].Value;
                        //Le site utilise un bout de script pour afficher la date au bon format selon le pays. 
                        //On peut récuperer les paramètres du script pour avoir la date
                        string date_str = node.SelectSingleNode(".//div[@class='review-content-header__dates']/script").InnerHtml.Trim();

                        string commentaire = node.SelectSingleNode(".//p[@class='review-content__text']").InnerText.Trim();

                        Regex date_reg = new Regex(@"\d{4}-\d{2}-\d{2}");
                        date_str = date_reg.Match(date_str).Value; //date en yyyy-MM-dd

                        Regex note_reg = new Regex(@"\d");
                        note_str = note_reg.Match(note_str).Value;

                        DateTime date = DateTime.ParseExact(date_str, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                        double note = Double.Parse(note_str);

                        if (date >= limitDate) {
                            Review review = new Review(date, commentaire, note, new Source("Trustpilot.com", true), new Utilisateur(auteur));
                            res.Add(review);
                        } else {
                            stop = true;
                            break;
                        }

                    }

                } else { //Si la requête echoue
                    throw new EntrepriseInconnueException($"Aucune entreprise trouvée pour : {url}");
                }

                HtmlNode bouton_suivant = doc.DocumentNode.SelectSingleNode("//a[@rel='next']");

                if (bouton_suivant == null) { //Si la page ne contient plus de bouton suivant on a fini le parcours
                    stop = true;
                }

                page++;
            }

            return res;

        }

        public async override Task<InfoEntreprise> getEntreprise(string research) {

            string url = "https://fr.trustpilot.com/review/" + research;

            HttpResponseMessage response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode) {

                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(response.Content.ReadAsStringAsync().Result);

                string cat = "";

                HtmlNodeCollection categorie_nodes = doc.DocumentNode.SelectNodes("//a[@class='category category--name']");

                if (categorie_nodes != null) {
                    foreach (HtmlNode node in categorie_nodes) {
                        cat += node.InnerText + ";";
                    }
                }

                string desc = "";

                HtmlNode desc_node = doc.DocumentNode.SelectSingleNode("//div[@class='company-description__text']");
                if (desc_node != null) {
                    desc = desc_node.InnerText;
                }

                string email = "";
                string adrresse = "";
                string telephone = "";

                HtmlNodeCollection contact_points = doc.DocumentNode.SelectNodes("//div[@class='contact-point']");
                if (contact_points != null) {

                    foreach (HtmlNode contact_point in contact_points) {
                        /*
                         * On se sert de l'icone pour deternminer le type de point de contact
                         */

                        HtmlNode icon_node = contact_point.SelectSingleNode("./div[1]/*[name()='svg']/*[name()='use']");
                        HtmlNode contact_info = contact_point.SelectSingleNode("./div[2]");

                        //url
                        if (icon_node.Attributes["xlink:href"].Value == "#icon_at-sign") {
                            email = contact_info.InnerText;
                        }

                        //telephone
                        if (icon_node.Attributes["xlink:href"].Value == "#icon_phone") {
                            telephone = contact_info.InnerText;
                        }

                        //url
                        if (icon_node.Attributes["xlink:href"].Value == "#icon_map-pin") {
                            adrresse = contact_info.InnerText;
                        }

                    }

                }

                string nom = "";

                HtmlNode name_node = doc.DocumentNode.SelectSingleNode("//span[@class='multi-size-header__big']");
                if (name_node != null) {
                    nom = name_node.InnerText;
                }

                return new InfoTrustPilot(research, desc, adrresse, telephone, email, cat, nom);

            }

            throw new EntrepriseInconnueException("Aucune entreprise trouvée pour " + research);
        }

    }
}