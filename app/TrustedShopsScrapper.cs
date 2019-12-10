using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
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
  /*
   * Scrapper pour le site TrustedShop.
   * A besoin de l'adresse du site d'une entreprise pour trouver des avis
   * Cette classe utilise la librairie Selenium. C'est une librairie qui permet
   * de controller un navigateur. On l'utilise pour simuler des évenements javascript.
   */
  public class TrustedShopsScrapper : Bot {

    private HttpClient client;

    public TrustedShopsScrapper() {

      this.source = "TrustedShops.com";
      client = new HttpClient();
      Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
    }

    public override async Task<List<Review>> getReviews(string research) {

      List<Review> res = new List<Review>();
      bool stop = false;
      string trusted_id = await getTsId(research);

      /*
       * On initialise un navigateur Selenium
       * L'option headless permet de ne pas ouvrir de fenetre
       */
      ChromeOptions options = new ChromeOptions();
      //options.AddArgument("headless");
      IWebDriver driver = new ChromeDriver(options);
      WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));

      string url = "https://www.trustedshops.fr/evaluation/info_" + trusted_id + ".html?shop_id=" + trusted_id + "&page=1&category=ALL";

      driver.Url = url;

      IJavaScriptExecutor js = (IJavaScriptExecutor)driver;

      while (!stop) { //On itère sur les pages de commentaire
        /*
         * On attend une seconde que la page charge
         */

        await Task.Delay(3000);
        wait.Until(ExpectedConditions.ElementExists(By.XPath(" //review")));
        var dates = driver.FindElements(By.XPath("//review"));
        int i = 1;

        foreach (IWebElement review in dates) {

          /*
           * La date n'est disponible que si on met la sourie sur un certain composant dans la balise <review>
           * on exécute donc un script qui simule le mouse over puis le mouse out
           */
          string jscode_mouse_over = jsScriptEvent("mouseover", i);
          string jscode_mouse_out = jsScriptEvent("mouseout", i);

          js.ExecuteScript(jscode_mouse_over);

          /*
           * On attend que la balise apparaisse
           */
          IWebElement date_u = wait.Until(ExpectedConditions.ElementExists(By.XPath("//bs-tooltip-container/div[2]")));
          string date_strr = date_u.Text;

          js.ExecuteScript(jscode_mouse_out);
          i++;

        }

        try {
          IWebElement btn_suivant = driver.FindElement(By.XPath("//div[@class='col-auto pr-0 pl-0 next ng-star-inserted']"));
          Actions action = new Actions(driver).MoveToElement(btn_suivant).Click();
          action.Build().Perform();
        } catch {
          stop = true;
        }
      }
      driver.Close();
      return res;

    }

    /*
     * trouve le trusted shop id d'une entreprise
     */
    private async Task<string> getTsId(string research) {
      /*
       * L'entreprise doit être recherchée via l'adresse de son site
       * ex : moncoffrage.com
       */

      string id_url = "https://api.trustedshops.com/rest/public/v2/shops?url=" + research;
      HttpResponseMessage response = await client.GetAsync(id_url);
      string str = await response.Content.ReadAsStringAsync();

      /*
       * On récupère l'id de l'entreprise pour chercher les commentaires
       */
      XmlDocument doc = new XmlDocument();
      doc.LoadXml(str);
      XmlNode tsid_node = doc.SelectSingleNode("response/data/shops/shop/tsId");

      if (tsid_node == null) { //Si on n'a rien trouvé
        throw new EntrepriseInconnueException($"Aucune entreprise trouvée pour : {research}");
      }

      return tsid_node.InnerText;
    }

    /*
     * Le morceau de code Javascript suivant va simuler l'évenement mouseover ou mouseout sur l'élément concerné dans la baslise n
     * On peut demander à Selenium d'executer ce javascript
     */
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