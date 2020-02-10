using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace ProAdvisor.app {

    public class SocAvisGarentisScrapper : Bot, ISourceReview {

        private HttpClient client;

        public SocAvisGarentisScrapper() {

            this.source = "SocieteDesAvisGarentis.fr";
            client = new HttpClient();
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        }

        public async Task<List<Review>> findReviews(Entite entite, DateTime limitDate) {

            int page = 1;
            string research;

            try {
                research = entite.getResearchString();
            } catch (PasDeSiteWebException e) {
                throw new EntrepriseInconnueException();
            } catch (Exception e) {
                throw e;
            }

            string url = "https://www.societe-des-avis-garantis.fr/" + research;
            HttpResponseMessage reponse = await client.GetAsync(url);
            HtmlDocument doc = new HtmlDocument();;

            if (reponse.IsSuccessStatusCode) {
                doc.LoadHtml(reponse.Content.ReadAsStringAsync().Result);
            } else {

                //Match uniquement le nom du site pour se débarasser des www et .xyz
                Regex extract = new Regex(@"(?:([a-z0-9](?:[a-z0-9-]{0,61}[a-z0-9])?)\.)+[a-z0-9][a-z0-9-]{0,61}[a-z0-9]");
                Match match = extract.Match(research);

                if (match.Groups.Count > 1) {
                    research = match.Groups[1].Value;
                    url = "https://www.societe-des-avis-garantis.fr/" + research;
                    reponse = await client.GetAsync(url);
                    if (reponse.IsSuccessStatusCode) {
                        doc = new HtmlDocument();
                        doc.LoadHtml(reponse.Content.ReadAsStringAsync().Result);
                    } else {
                        throw new EntrepriseInconnueException();
                    }
                }
            }

            HtmlNode erreur404 = doc.DocumentNode.SelectSingleNode("//div[@class='thememount-big-icon']");
            HtmlNode suspended = doc.DocumentNode.SelectSingleNode("//div[@id='suspended']");

            if (erreur404 != null || suspended != null) {
                //L'entreprise n'est pas répertoriée ou suspendue
                throw new EntrepriseInconnueException();
            }

            string[] options = new string[] {
                "avis=positif",
                "avis=neutre",
                "avis=negatif"
            };

            List<Review> reviews = new List<Review>();

            foreach (string option in options) {

                bool stop = false;

                while (!stop) {
                    url = "https://www.societe-des-avis-garantis.fr/" + research + "/?agp=" + page.ToString() + "&" + option;

                    reponse = await client.GetAsync(url);
                    doc = new HtmlDocument();
                    doc.LoadHtml(await reponse.Content.ReadAsStringAsync());

                    HtmlNodeCollection review_nodes = doc.DocumentNode.SelectNodes("//div[@class='full-testimonial']");

                    if (review_nodes != null) {
                        foreach (HtmlNode review_node in review_nodes) {

                            HtmlNode note_node = review_node.SelectSingleNode(".//span[@itemprop='ratingValue']");
                            double note = Double.Parse(note_node.InnerText);

                            HtmlNode comment_node = review_node.SelectSingleNode(".//span[@class='reviewOnlyContent']");
                            string comment = comment_node.InnerText;

                            Regex date_reg = new Regex(@"publié le (\d\d\/\d\d\/\d\d) à");
                            string fullText = Regex.Replace(review_node.InnerText, @"\t|\n|\r", "");
                            Match match = date_reg.Match(fullText);
                            string date_str = match.Groups[1].Value;
                            DateTime date = DateTime.ParseExact(date_str, "dd/MM/yy", CultureInfo.InvariantCulture);

                            HtmlNode auteur_node = review_node.SelectSingleNode(".//span[@style='font-weight:bold;']");
                            string auteur = auteur_node.InnerText.Trim();

                            if (date < limitDate) {
                                stop = true;
                                break;
                            } else {
                                reviews.Add(new ReviewBasic(entite.id, this.source, auteur, date, note, comment));
                            }
                        }
                        page++;
                    } else {
                        stop = true;
                    }
                }
            }

            if (reviews.Count == 0) {
                throw new PasDeCommentaireException();
            }

            return reviews;

        }

        public override void destroy() {

        }
    }

}