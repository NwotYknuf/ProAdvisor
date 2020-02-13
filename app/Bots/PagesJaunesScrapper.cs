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
    class PagesJaunesScrapper : Bot, ISourceEntreprise {

        private HttpClient httpClient;
        private IWebDriver driver;

        public PagesJaunesScrapper() {
            httpClient = new HttpClient();
            this.source = "pagesjaunes.fr";

            ChromeOptions options = new ChromeOptions();
            //headless pour ne pas ouvrir une fenetre navigateur
            options.AddArgument("headless ");
            options.AddArgument("--no-sandbox");
            options.AddArgument("log-level=3");
            driver = new ChromeDriver(options);

        }

        public override void destroy() {
            driver.Close();
        }

        public async Task<List<Entreprise>> findEntreprise(string research, double lat, double lon) {

            System.Globalization.CultureInfo.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            List<Entreprise> res = new List<Entreprise>();

            string url = @"https://www.pagesjaunes.fr/annuaire/chercherlespros" +
                "?quoiqui=" + research +
                "&ou=Autour%20de%20moi" +
                "&longitude=" + lon.ToString() +
                "&latitude=" + lat.ToString();

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));

            driver.Url = url;

            bool done = false;
            bool cookies = false;
            List<string> hrefs = new List<string>();

            while (!done) {
                var filter = By.XPath(" //a[@class='denomination-links pj-lb pj-link'] | //a[@class='denomination-links pj-link']");
                wait.Until(ExpectedConditions.ElementExists(filter));
                var entrepriseNodes = driver.FindElements(filter);
                await Task.Delay(1000);

                foreach (IWebElement node in entrepriseNodes) {
                    hrefs.Add(node.GetAttribute("href"));
                }

                try {
                    if (!cookies) {
                        filter = By.XPath("//button[@id='toutAccepter']");
                        wait.Until(ExpectedConditions.ElementExists(filter));
                        var cookiesBtn = driver.FindElement(filter);
                        cookiesBtn.Click();
                        cookies = true;
                    }

                    filter = By.XPath("//a[@class='link_pagination next pj-lb pj-link']");
                    wait.Until(ExpectedConditions.ElementExists(filter));
                    var button = driver.FindElement(filter);
                    button.Click();

                } catch {
                    done = true;
                }

            }

            foreach (string href in hrefs) {

                HttpClient client = new HttpClient();

                HttpResponseMessage response = await client.GetAsync(href);

                if (response.IsSuccessStatusCode) {

                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(await response.Content.ReadAsStringAsync());

                    HtmlNode siretNode = doc.DocumentNode.SelectSingleNode("//div[@class='row siret']/span");

                    if (siretNode != null) {
                        string siret = siretNode.InnerText;

                        HtmlNode nomNode = doc.DocumentNode.SelectSingleNode("//h1[@class='noTrad no-margin']");
                        string nom = nomNode.InnerText;

                        HtmlNode adresseNode = doc.DocumentNode.SelectSingleNode("//a[@class='teaser-item black-icon address streetAddress clearfix map-click-zone pj-lb pj-link']");
                        string adresse = adresseNode.InnerText;

                        string siteWeb = "";
                        var linkNodes = doc.DocumentNode.SelectNodes("//a[@class='teaser-item black-icon pj-lb pj-link']");
                        if (linkNodes != null) {
                            foreach (var node in linkNodes)
                                if (node.SelectSingleNode("./span[@class='icon icon-lien']") != null) {
                                    siteWeb = node.InnerText;
                                }
                        }

                        HtmlNodeCollection prestationNodes = doc.DocumentNode.SelectNodes("//div[@class='ligne prestations marg-btm-m generique']//li");
                        List<string> prestations = new List<string>();

                        if (prestationNodes != null) {
                            foreach (HtmlNode prestationNode in prestationNodes) {
                                prestations.Add(prestationNode.InnerText);
                            }
                        }

                        HtmlNodeCollection zoneNodes = doc.DocumentNode.SelectNodes("//div[@class='zone-chalandise']/div//li");
                        List<string> zones = new List<string>();

                        if (zoneNodes != null) {
                            foreach (HtmlNode zoneNode in zoneNodes) {
                                zones.Add(zoneNode.InnerText);
                            }
                        }

                        res.Add(new Entreprise(siret, nom, siteWeb, adresse, prestations, zones));

                        //On attend un peu pour eviter de se faire flag comme bot
                        Random rnd = new Random();
                        await Task.Delay(rnd.Next(1000, 2000));
                    }

                }
            }

            return res;
        }

    }
}