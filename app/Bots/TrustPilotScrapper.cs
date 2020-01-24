using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace ProAdvisor.app {

    /*
     * Scrapper pour le site Truspilot.
     * A besoin de l'adresse du site d'une entreprise pour trouver des avis
     */

    public class TrustPilotScrapper : Bot, ISourceInfo, ISourceReview {

        private HttpClient client;
        private IWebDriver driver;

        public TrustPilotScrapper() {
            this.source = "TrustPilot.com";
            client = new HttpClient();
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            ChromeOptions options = new ChromeOptions();
            //headless pour ne pas ouvrir une fenetre navigateur
            options.AddArgument("headless");
            //Log level 3 pour ignorer les sorties consoles
            options.AddArgument("log-level=3");
            driver = new ChromeDriver(options);

        }

        public async Task<List<Review>> findReviews(Entite entite, DateTime limitDate) {

            string research;

            try {
                research = entite.researchString;
            } catch {
                throw new EntrepriseInconnueException();
            }
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
                            Review review = new ReviewBasic(entite.id, this.source, auteur, date, note, commentaire);
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

        public async Task<Info> findInfos(Entite entite) {

            string research;

            try {
                research = entite.researchString;
            } catch {
                throw new EntrepriseInconnueException();
            }

            string url = "https://fr.trustpilot.com/review/" + research;

            driver.Url = url;
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;

            for (int i = 0; i < 5; i++) {
                js.ExecuteScript("window.scrollBy(0,250)");
                await Task.Delay(100);
            }

            try {
                driver.FindElement(By.XPath("//div[@class='container error-page']"));
                throw new EntrepriseInconnueException();
            } catch { }

            string cat = "";

            try {
                var categorie_nodes = driver.FindElements(By.XPath("//a[@class='category category--name']"));

                if (categorie_nodes != null) {
                    foreach (IWebElement node in categorie_nodes) {
                        cat += node.Text + ";";
                    }
                }
            } catch { }

            string desc = "";

            try {
                var desc_node = driver.FindElement(By.XPath("//div[@class='company-description__text']"));

                if (desc_node != null) {
                    desc = desc_node.Text;
                }
            } catch { }

            string email = "";
            string adrresse = "";
            string telephone = "";

            try {
                var contact_points = driver.FindElements(By.XPath("//div[@class='contact-point']"));
                if (contact_points != null) {

                    foreach (IWebElement contact_point in contact_points) {
                        /*
                         * On se sert de l'icone pour deternminer le type de point de contact
                         */

                        IWebElement icon_node = contact_point.FindElement(By.XPath("./div[1]/*[name()='svg']/*[name()='use']"));
                        IWebElement contact_info = contact_point.FindElement(By.XPath("./div[2]"));

                        //url
                        if (icon_node.GetAttribute("xlink:href") == "#icon_at-sign") {
                            email = contact_info.Text;
                        }

                        //telephone
                        if (icon_node.GetAttribute("xlink:href") == "#icon_phone") {
                            telephone = contact_info.Text;
                        }

                        //url
                        if (icon_node.GetAttribute("xlink:href") == "#icon_map-pin") {
                            adrresse = contact_info.Text;
                        }

                    }

                }
            } catch { }

            string nom = "";

            try {
                IWebElement name_node = driver.FindElement(By.XPath("//span[@class='multi-size-header__big']"));
                if (name_node != null) {
                    nom = name_node.Text;
                }
            } catch { }

            return new InfoTrustPilot(research, desc, adrresse, telephone, email, cat, nom);

        }

        public override void destroy() {
            driver.Close();
        }
    }
}