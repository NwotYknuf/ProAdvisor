using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

public class TrustedShopClient : Bot {

    private HttpClient httpClient;

    /*
     * Utilise l'API de trusted shops
     * la version publique est très limité, on ne peut trouver que les 10 derniers commentaires
     */

    public TrustedShopClient() {
        httpClient = new HttpClient();

        /*
         * Necessaire pour ne pas être embeté quand on va parser des nombres.
         * Par défault la culture est en français et les parseurs attendent un
         * nombre avec une virgule comme séparateur ce qui ne nous arrange pas
         */
        Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
    }

    public override async Task<List<Review>> getReviews(string research) {
        /*
         * L'entreprise doit être recherchée via l'adresse de son site
         * ex : moncoffrage.com
         */

        string id_url = "https://api.trustedshops.com/rest/public/v2/shops?url=" + research;
        HttpResponseMessage response = await httpClient.GetAsync(id_url);
        checkStatus(response);
        string str = await response.Content.ReadAsStringAsync();

        /*
         * On récupère l'id de l'entreprise pour chercher les commentaires
         */
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(str);
        XmlNode tsid_node = doc.SelectSingleNode("response/data/shops/shop/tsId");
        string tsId = tsid_node.InnerText;

        /*
         * On récupère les commentaires
         */

        string comm_url = "https://api.trustedshops.com/rest/public/v2/shops/" + tsId + "/reviews";
        response = await httpClient.GetAsync(comm_url);
        checkStatus(response);
        str = await response.Content.ReadAsStringAsync();
        doc.LoadXml(str);

        XmlNodeList rewiew_nodes = doc.SelectNodes("response/data/shop/reviews/review");

        List<Review> reviews = new List<Review>();

        foreach (XmlNode rewiew_node in rewiew_nodes) {

            DateTime date = DateTime.Parse(rewiew_node.SelectSingleNode("confirmationDate").InnerText);
            string commentaire = rewiew_node.SelectSingleNode("comment").InnerText;
            double note = Double.Parse(rewiew_node.SelectSingleNode("mark").InnerText);

            Review review = new Review(date, commentaire, note);
            reviews.Add(review);
        }

        return reviews;

    }

    public void checkStatus(HttpResponseMessage responseMessage) {
        if (responseMessage.StatusCode.Equals(400)) {
            throw new Exception("Mauvaise requête");
        }

        if (responseMessage.StatusCode.Equals(404)) {
            throw new Exception("Aucun résultat");
        }

        if (responseMessage.StatusCode != System.Net.HttpStatusCode.OK) {
            throw new Exception("???");
        }
    }

}