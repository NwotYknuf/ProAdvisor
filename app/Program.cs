using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;
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

            string[] entreprises = new string[] {
                "www.happybulle.com",
                "www.moncoffrage.com",
                "www.pimkie.fr",
                "www.cadeau-maestro.com"
            };

            List<Bot> bots = new List<Bot>() {
                new TrustedShopsScrapper(),
                new TrustPilotScrapper()
            };

            Parallel.ForEach(bots, bot => {

                StreamWriter logFile = new StreamWriter(bot.source + DateTime.Now.Date.ToString("dd-MM-yyyy") + ".log");

                List<Donnee> donnee = new List<Donnee>();

                foreach (string recherche in entreprises) {

                    DateTime limit_date = new DateTime(2000, 01, 01);

                    DateTime lastComment;
                    string log = $"Recherche d'avis pour {recherche}\n";

                    try {
                        lastComment = dateDerniereReview(recherche, bot.source);
                        log += $"Dernier commentaire trouvé pour cette source : {lastComment.ToString("dd-MM-yyyy")}\n";
                        limit_date = lastComment;
                    } catch (PasDeCommentaireException e) {
                        log += "Aucun avis trouvés précedement\n";
                    }

                    try {

                        InfoEntreprise entreprise = bot.getEntreprise(recherche).Result;

                        List<Donnee> newReviews = new List<Donnee>();

                        foreach (Review review in bot.getReviews(recherche, limit_date).Result) {
                            newReviews.Add(new Donnee(entreprise, review));
                        }
                        log += $"{newReviews.Count} nouveaux avis trouvés\n";
                        donnee.AddRange(newReviews);

                    } catch (AggregateException ae) {

                        if (ae.InnerException is EntrepriseInconnueException) {
                            log += $"Pas trouvé d'entreprise pour {recherche}\n";
                        }

                        if (ae.InnerException is PasDeCommentaireException) {
                            log += $"Pas de nouveaux commentaire pour {recherche}\n";
                        }

                        if (!(ae.InnerException is EntrepriseInconnueException) &&
                            !(ae.InnerException is PasDeCommentaireException)) {
                            log += "Exception non atendue : " + ae.InnerException.Message + "\n" + ae.InnerException.StackTrace + "\n";
                        }
                    }

                    logFile.WriteLine(log);

                }

                logFile.Close();

                StreamWriter sw = new StreamWriter(bot.source + ".json");
                sw.Write(JsonConvert.SerializeObject(donnee));
                sw.Close();

            });

        }
    }
}