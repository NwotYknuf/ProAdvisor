using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OpenQA.Selenium;

namespace ProAdvisor.app {
    class Program {
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

                List<Donnee> donnee = new List<Donnee>();

                foreach (string recherche in entreprises) {

                    try {
                        InfoEntreprise entreprise = bot.getEntreprise(recherche).Result;

                        foreach (Review review in bot.getReviews(recherche, new DateTime(2019, 12, 16)).Result) {
                            donnee.Add(new Donnee(entreprise, review));
                        }
                    } catch (AggregateException ae) {
                        if (!(ae.InnerException is EntrepriseInconnueException) &&
                            !(ae.InnerException is PasDeCommentaireException)) {
                            throw ae.InnerException;
                        }
                    }

                }

                StreamWriter sw = new StreamWriter(bot.source + ".json");
                sw.Write(JsonConvert.SerializeObject(donnee));
                sw.Close();

            });

        }
    }
}