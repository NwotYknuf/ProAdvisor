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
    class PagesJaunesScrapper : ISourceEntreprise {

        private HttpClient httpClient;

        public PagesJaunesScrapper() {
            httpClient = new HttpClient();
        }

        public async Task<List<Entreprise>> findEntreprise(string research) {

            string url = @"https://www.pagesjaunes.fr/annuaire/chercherlespros?quoiqui=peinture&ou=Autour+de+moi&univers=pagesjaunes&sourceOu=AUTOURDEMOI&accuracy=2230&latitude=49.086464&longitude=6.217728";

            ChromeOptions options = new ChromeOptions();
            //headless pour ne pas ouvrir une fenetre navigateur
            //options.AddArgument("headless");
            //Log level 3 pour ignorer les sorties consoles
            options.AddArgument("log-level=3");
            IWebDriver driver = new ChromeDriver(options);
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));

            driver.Url = url;

            var filter = By.XPath("//a[@class='denomination-links pj-lb pj-link'] | //a[@class='denomination-links pj-link']");
            wait.Until(ExpectedConditions.ElementExists(filter));
            var entrepriseNodes = driver.FindElements(filter);
            int N = entrepriseNodes.Count;

            for (int i = 1; i <= N; i++) {
                driver.Url = url;
                filter = By.XPath("(//a[@class='denomination-links pj-lb pj-link' or @class='denomination-links pj-link'])[" + i.ToString() + "]");
                wait.Until(ExpectedConditions.ElementExists(filter));
                var node = driver.FindElement(filter);
                node.Click();
            }

            driver.Close();

            return null;
        }

        private string jsScriptEvent(string event_type, int n) {
            return "var event = new MouseEvent('" + event_type + "', {" +
                "'view': window," +
                "'bubbles': true," +
                "'cancelable': true" +
                "});" +
                "var myTarget = document.evaluate(\"/html/body/presentation-frame/main/shop-profile/div[2]/div/div/div[1]/div[2]/ratings/div[4]/div/async-list/review[" + n.ToString() + "]/div/div[1]/div[2]/loading-line/div/div/div[2]/span\", document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue;" +
                "var canceled = !myTarget.dispatchEvent(event);";
        }
    }
}