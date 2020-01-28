using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace ProAdvisor.app {
    class Program {

        public static List<Entreprise> trouverEntreprises(List<Bot> bots, string quoi, (double lat, double lon)ou) {

            List<Entreprise> entreprises = new List<Entreprise>();

            foreach (Bot bot in bots) {

                if (bot is ISourceEntreprise) {
                    ISourceEntreprise scrapper = bot as ISourceEntreprise;

                    try {
                        entreprises.AddRange(scrapper.findEntreprise(quoi, ou.lat, ou.lon).Result);
                    } catch (AggregateException ae) {
                        throw ae.InnerException;
                    }
                }

            }

            return entreprises;

        }

        public static DateTime findLastComment(Bot bot, Entite entite) {
            return new DateTime(2010, 1, 1); //TODO
        }

        public static List<Review> trouverReviews(List<Bot> bots, Entite entite) {

            List<Review> reviews = new List<Review>();
            DateTime dateLimite;

            foreach (Bot bot in bots) {

                if (bot is ISourceInfo) {

                    dateLimite = findLastComment(bot, entite);

                    try {
                        reviews.AddRange((bot as ISourceReview).findReviews(entite, dateLimite).Result);
                    } catch (AggregateException ae) {

                        if (!(ae.InnerException is EntrepriseInconnueException) &&
                            !(ae.InnerException is PasDeCommentaireException)) {
                            Console.WriteLine(ae.InnerException.Message + ae.InnerException.StackTrace);
                        }
                    }

                }
            }

            return reviews;

        }

        public static List<Info> trouverInfos(List<Bot> bots, Entite entite) {

            List<Info> infos = new List<Info>();

            foreach (Bot bot in bots) {

                if (bot is ISourceInfo) {

                    try {
                        infos.Add((bot as ISourceInfo).findInfos(entite).Result);
                    } catch (AggregateException ae) {

                        if (!(ae.InnerException is EntrepriseInconnueException)) {
                            Console.WriteLine(ae.InnerException.Message + ae.InnerException.StackTrace);
                        }
                    }

                }
            }

            return infos;
        }

        public static List<Entite> getEntites() {
            return new List<Entite>(); //TODO
        }

        public static void Main(string[] args) {

            List<Bot> bots = new List<Bot>();

            bots.Add(new PagesJaunesScrapper());
            bots.Add(new TrustPilotScrapper());
            bots.Add(new TrustedShopsScrapper());

            List<string> recherches = new List<string>() {
                "plombier",
                "maçon",
                "carreleur"
            };

            List < (double lat, double lon) > positions = new List < (double lat, double lon) > () {
                (49.1149772, 6.1824283),
                (44.841225, -0.5800364)
            };

            List<Entreprise> entreprises = new List<Entreprise>();

            foreach (string recherche in recherches) {
                foreach (var pos in positions) {
                    entreprises.AddRange(trouverEntreprises(bots, recherche, pos));
                }
            }

            StreamWriter sw = new StreamWriter("entreprises.json");
            sw.Write(JsonConvert.SerializeObject(entreprises));
            sw.Close();

            List<Entite> entites = getEntites();

            List<Info> infos = new List<Info>();
            List<Review> reviews = new List<Review>();

            foreach (Entite entite in entites) {
                infos.AddRange(trouverInfos(bots, entite));
                reviews.AddRange(trouverReviews(bots, entite));
            }

            foreach (Bot bot in bots) {
                bot.destroy();
            }

        }
    }
}