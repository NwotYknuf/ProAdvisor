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
  /*
   * Scrapper pour le site TrustedShop.
   * A besoin de l'adresse du site d'une entreprise pour trouver des avis
   * Cette classe utilise la librairie Selenium. C'est une librairie qui permet
   * de controller un navigateur. On l'utilise pour simuler des évenements javascript.
   */
  public class TrustedShopsScrapper : Bot, ISourceInfo, ISourceReview {

    private HttpClient client;
    private IWebDriver driver;

    public TrustedShopsScrapper() {

      this.source = "TrustedShops.com";
      client = new HttpClient();
      Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

      /*
       * On initialise un navigateur Selenium
       * L'option headless permet de ne pas ouvrir de fenetre
       */
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
      string trusted_id = await getTsId(research);

      WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));

      string url = "https://www.trustedshops.fr/evaluation/info_" + trusted_id + ".html?shop_id=" + trusted_id + "&page=1&category=ALL";

      driver.Url = url;

      IJavaScriptExecutor js = (IJavaScriptExecutor)driver;

      while (!stop) { //On itère sur les pages de commentaire

        /*
         * On attend une seconde que la page charge
         */
        await Task.Delay(1000);

        try {
          wait.Until(ExpectedConditions.ElementExists(By.XPath(" //review")));
        } catch (WebDriverTimeoutException) {
          throw new PasDeCommentaireException("Pas de commentaires pour :" + research);
        }

        var dates = driver.FindElements(By.XPath("//review"));
        int i = 1;

        foreach (IWebElement review in dates) {

          /*
           * La date n'est disponible que si on met la sourie sur un certain composant dans la balise <review>
           * on exécute donc un script qui simule le mouse over puis le mouse out
           */
          string xpath = "/html/body/presentation-frame/main/shop-profile/div[2]/div/div/div[1]/div[2]/ratings/div[3]/div/async-list/review[" + i.ToString() + "]/div/div[1]/div[3]/loading-line/div/div/div[2]/span";
          string jscode_mouse_over = JsEvent.getEvent(xpath, "'mouseenter'");
          string jscode_mouse_out = JsEvent.getEvent(xpath, "'mouseleave'");

          js.ExecuteScript(jscode_mouse_over);
          /*
           * On attend que la balise apparaisse
           */
          IWebElement date_u = wait.Until(ExpectedConditions.ElementExists(By.XPath(" //div[@class='tooltip-content']")));
          string date_str = date_u.Text;

          Regex find_date = new Regex(@"\d{2}\/\d{2}\/\d{4}");
          date_str = find_date.Match(date_str).Value;

          DateTime date = DateTime.ParseExact(date_str, "dd/MM/yyyy", CultureInfo.InvariantCulture);

          //On lance mouse out pour que l'élément disparaisse
          js.ExecuteScript(jscode_mouse_out);

          /*
           * Cet élément contient un style qui définit le nombre d'étoiles colorées en jaune
           * "
          max - width : 100 % " : 5 étoiles
           * "
          max - width : 20 % " : 1 étoile
           * On peut donc en déduire la note (width/20) pour la ramener sur 5
           */
          IWebElement star_rating = review.FindElement(By.XPath("//review[1]//div[@class='stars-active']"));
          string note_str = star_rating.GetAttribute("style");
          //On cherche le nombre dans le style
          Regex findNote = new Regex(@"\d{1,3}");
          note_str = findNote.Match(note_str).Value;
          double note = double.Parse(note_str) / 20.0;

          string auteur_str = "annonyme";
          try {
            IWebElement auteur = review.FindElement(By.XPath(".//div/div[1]/div[1]/loading-line/div/div/div[2]/div/div[2]/a"));
            auteur_str = auteur.Text;
          } catch (NoSuchElementException) {
            //Le commentaire est annonyme, rien à faire ici
          } catch (Exception e) {
            throw e;
          }

          IWebElement comment = review.FindElement(By.XPath(".//div/div[3]/div/loading-line/div/div/div[2]/span"));
          string comment_str = comment.Text;

          if (date >= limitDate) {
            Review rev = new ReviewBasic(entite.id, this.source, auteur_str, date, note, comment_str);
            res.Add(rev);
            i++;
          } else {
            stop = true;
            break;
          }

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

    public async Task<Info> findInfos(Entite entite) {

      string research;

      try {
        research = entite.researchString;
      } catch {
        throw new EntrepriseInconnueException();
      }

      string trusted_id = await getTsId(research);

      string url = "https://www.trustedshops.fr/evaluation/info_" + trusted_id + ".html?shop_id=" + trusted_id + "&page=1&category=ALL";

      HttpResponseMessage response = await client.GetAsync(url);

      if (response.IsSuccessStatusCode) {

        HtmlDocument doc = new HtmlDocument();
        doc.LoadHtml(response.Content.ReadAsStringAsync().Result);
        HtmlNode mainbox = doc.DocumentNode.SelectSingleNode("//div[@class='main-box d-none d-lg-block no-margin detail-box mt-0']");

        string cat = "";

        HtmlNode shop_not_found_nodes = doc.DocumentNode.SelectSingleNode("//shop-not-found");

        if (shop_not_found_nodes != null) { //L'entreprise n'est plus membre trustedshops
          throw new EntrepriseInconnueException();
        }

        HtmlNodeCollection categorie_nodes = mainbox.SelectNodes(".//shop-search-category");

        if (categorie_nodes != null) {
          foreach (HtmlNode node in categorie_nodes) {
            cat += node.InnerText + ";";
          }
        }

        string desc = "";

        HtmlNode desc_node = mainbox.SelectSingleNode(".//shop-details//div[@innertext and @class='col-12 fw-light mt-1']");
        if (desc_node != null) {
          desc = desc_node.InnerText;
        }

        string nom = "";
        HtmlNode name_node = mainbox.SelectSingleNode(".//div[@class='shop-name fw-light mt-1']");
        if (name_node != null) {
          nom = name_node.InnerText;
        }

        string adresse = "";
        HtmlNode addr_node = mainbox.SelectSingleNode(".//div[@class='address row mt-12']");
        if (addr_node != null) {
          adresse = addr_node.InnerText;
        }

        string email = "";
        HtmlNode email_node = mainbox.SelectSingleNode(".//div[@class='email row mt-12 ng-star-inserted']");
        if (email_node != null) {
          email = email_node.InnerText;
        }

        string telephone = "";
        HtmlNode tel_node = mainbox.SelectSingleNode(".//div[@class='phone row mt-12 ng-star-inserted']");
        if (tel_node != null) {
          telephone = tel_node.InnerText;
        }

        return new InfoTrustedShops(id: entite.id, url: research, nom: nom, categories: cat, description: desc, adresse: adresse, email: email, telephone: telephone);

      }

      if (response.StatusCode == HttpStatusCode.Gone) {
        throw new EntrepriseInconnueException();
      }

      throw new Exception("Unsuccessfull request : " + url);

    }

    public override void destroy() {
      driver.Close();
    }
  }

}