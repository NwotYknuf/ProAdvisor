using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using HtmlAgilityPack;

namespace ProAdvisor.app {
  /*
   * Scrapper pour le site TrustedShop.
   * A besoin de l'adresse du site d'une entreprise pour trouver des avis
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
      int page = 1;
      bool stop = false;
      string trusted_id = await getTsId(research);

      while (!stop) { //On itère sur les pages de commentaire

        string url = "https://www.trustedshops.fr/evaluation/info_" + trusted_id + ".html?shop_id=" + trusted_id + "&page=" + page + "&category=ALL";

        HttpResponseMessage response = await client.GetAsync(url);

        if (response.IsSuccessStatusCode) { //Si la requête échoue ici on a finit le parcours des pages

          HtmlDocument doc = new HtmlDocument();
          doc.LoadHtml(response.Content.ReadAsStringAsync().Result);

          //On récupère uniquement les blocs commentaires
          HtmlNodeCollection comment_nodes = doc.DocumentNode.SelectNodes("//*[@class='col-xs-12 commentblock']");

          foreach (HtmlNode node in comment_nodes) {
            //On récupère les informations qui nous intéressent dans les descendants du bloc actuel
            string date_publi_str = node.SelectSingleNode(".//div[@class='reviewCommentDateOpenedContent']/div[1]/div[2]").InnerText.Trim();
            string date_commande_str = node.SelectSingleNode(".//div[@class='reviewCommentDateOpenedContent']/div[3]/div[2]").InnerText.Trim();
            string commentaire = node.SelectSingleNode(".//div[@class='reviewComment']").InnerText.Trim();
            string note = node.SelectSingleNode(".//meta[@itemprop='ratingValue']").Attributes["content"].Value; //La note se trouve dans un attribut de balise

            //Certains commentaires sont anonymes
            string auteur = "anonnyme";
            HtmlNode node_auteur = node.SelectSingleNode(".//div[@itemprop='author']/a");
            if (node_auteur != null) {
              auteur = node_auteur.InnerText.Trim();
            }

            res.Add(new Review(
              DateTime.ParseExact(date_publi_str, "dd.MM.yyyy", CultureInfo.InvariantCulture),
              commentaire,
              Double.Parse(note),
              new Source("TrustedShops.com", true),
              new Utilisateur(auteur)));

          }
        } else {
          stop = true;
        }
        page++;
      }
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
        throw new Exception($"Aucune entreprise trouvée pour : {research}");
      }

      return tsid_node.InnerText;
    }

  }
}