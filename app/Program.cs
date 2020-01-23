using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OpenQA.Selenium;

namespace ProAdvisor.app {
    class Program {

        public static Dictionary<string, string> tableNames = new Dictionary<string, string>() { { "TrustedShops.com", "TrustedShop_commentaires" }, { "TrustPilot.com", "TrustPilot_commentaires" } };

        public static DateTime dateDerniereReview(string url_entreprise, string source) {

            string connectionString = "Server=r-pro-advisor.gq; Port=33069; Database=ods_projet; UID=root; password=HdntL3T8Wnsuasp6";

            var connection = new MySqlConnection(connectionString);
            connection.Open();

            string sql = @"
            SELECT MAX(date)
            FROM " + tableNames[source] + @"
            WHERE url_entreprise LIKE '" + url_entreprise + @"';";

            MySqlCommand cmd = new MySqlCommand(sql, connection);
            MySqlDataReader rdr = cmd.ExecuteReader();

            try {
                rdr.Read();
                DateTime res = rdr.GetDateTime(0);
                connection.Close();
                return res;
            } catch {
                connection.Close();
                throw new PasDeCommentaireException();
            }

        }

        public static void Main(string[] args) {

            PagesJaunesScrapper scrapper = new PagesJaunesScrapper();

            try {
                List<Entreprise> entreprises = scrapper.findEntreprise("").Result;
            } catch (AggregateException ae) {
                throw ae.InnerException;
            }
        }
    }
}