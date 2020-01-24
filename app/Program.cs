using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace ProAdvisor.app {
    class Program {

        public void trouverEntreprises(List<Bot> bots) {

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

            foreach (ISourceEntreprise scrapper in bots) {

                foreach (string recherche in recherches) {
                    foreach ((double lat, double lon)pos in positions) {
                        try {
                            entreprises.AddRange(scrapper.findEntreprise(recherche, pos.lat, pos.lon).Result);
                        } catch (AggregateException ae) {
                            throw ae.InnerException;
                        }
                    }
                }

            }

            StreamWriter sw = new StreamWriter("entreprises.json");
            sw.Write(JsonConvert.SerializeObject(entreprises));
            sw.Close();
        }

        public static void Main(string[] args) {

            List<Bot> bots = new List<Bot>();

            bots.Add(new PagesJaunesScrapper());
            bots.Add(new TrustPilotScrapper());
            bots.Add(new TrustedShopsScrapper());

            DateTime limitDate = new DateTime(2010, 1, 1);

            StreamReader sr = new StreamReader("entreprises.json");
            List<Entreprise> entreprises = JsonConvert.DeserializeObject<List<Entreprise>>(sr.ReadToEnd());

            List<Entite> entites = new List<Entite>();
            entites.AddRange(entreprises);

            entites.Add(new Service("www.pimkie.fr", "pimkie"));
            entites.Add(new Service("www.moncoffrage.com", "moncoffrage"));

            foreach (Bot bot in bots) {

                if (bot is ISourceInfo) {

                    List<Info> infos = new List<Info>();

                    foreach (Entite entite in entites) {
                        try {
                            infos.Add((bot as ISourceInfo).findInfos(entite).Result);
                        } catch (AggregateException ae) {

                            if (!(ae.InnerException is EntrepriseInconnueException)) {
                                Console.WriteLine(ae.InnerException.Message + ae.InnerException.StackTrace);
                            }
                        }
                    }

                    StreamWriter sw = new StreamWriter("infos" + bot.source + ".json");
                    sw.Write(JsonConvert.SerializeObject(infos));
                    sw.Close();
                }
            }

            foreach (Bot bot in bots) {

                if (bot is ISourceInfo) {

                    List<Review> reviews = new List<Review>();

                    foreach (Entite entite in entites) {
                        try {
                            reviews.AddRange((bot as ISourceReview).findReviews(entite, limitDate).Result);
                        } catch (AggregateException ae) {

                            if (!(ae.InnerException is EntrepriseInconnueException) &&
                                !(ae.InnerException is PasDeCommentaireException)) {
                                Console.WriteLine(ae.InnerException.Message + ae.InnerException.StackTrace);
                            }
                        }
                    }

                    StreamWriter sw = new StreamWriter("reviews" + bot.source + ".json");
                    sw.Write(JsonConvert.SerializeObject(reviews));
                    sw.Close();
                }
            }

            foreach (Bot bot in bots) {
                bot.destroy();
            }

        }
    }
}