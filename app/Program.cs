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
                "www.moncoffrage.com",
                "www.pimkie.fr",
                "www.alzkeaze.caz",
                "www.habitatpresto.com"
            };

            List<Bot> bots = new List<Bot>();

            bots.Add(new TrustPilotScrapper());
            bots.Add(new TrustedShopsScrapper());

            List<Donnee> donees = new List<Donnee>();

            foreach (string entreprise in entreprises) {
                ConcurrentDictionary<string, List<Review>> reviewsPerSource = new ConcurrentDictionary<string, List<Review>>();
                List<Donnee> donnes_entreprise = new List<Donnee>();

                Console.WriteLine($"Recherche d'avis pour : {entreprise}");

                /*
                 * Boucle en parallèle pour chaque bot
                 */
                Parallel.ForEach(
                    bots, new ParallelOptions { MaxDegreeOfParallelism = 4 },
                    (bot) => {
                        try {
                            List<Review> reviews = bot.getReviews(entreprise).Result;
                            reviewsPerSource.TryAdd(bot.source, reviews);
                            Console.WriteLine($"{reviews.Count} review(s) certifiées AFNOR trouvée(s) pour la source {bot.source}");
                        } catch (AggregateException ae) {

                            foreach (Exception e in ae.InnerExceptions) {
                                if (e is PasDeCommentaireException) {
                                    Console.WriteLine($"Aucun commentaires trouvés pour {entreprise} sur la source {bot.source}");
                                } else {
                                    if (e is EntrepriseInconnueException) {
                                        Console.WriteLine($"Aucune entreprise trouvée pour {entreprise} sur la source {bot.source}");
                                    } else {
                                        if (e is DriverServiceNotFoundException) {
                                            Console.WriteLine($"Driver Selenium non installé, impossible de trouver des commentaires pour {bot.source}");
                                        } else {
                                            throw e;
                                        }
                                    }
                                }
                            }
                        }
                    }
                );

                foreach (var keyValuePair in reviewsPerSource) {

                    foreach (Review review in keyValuePair.Value) {
                        Donnee donne = new Donnee(new Entreprise(entreprise), review);
                        donnes_entreprise.Add(donne);
                    }

                }

                if (donnes_entreprise.Count > 0) {
                    double moyenne = 0.0;
                    foreach (Donnee donnee in donnes_entreprise) {
                        moyenne += donnee.review.note;
                    }
                    moyenne /= donnes_entreprise.Count;
                    donees.AddRange(donnes_entreprise);
                    Console.WriteLine($"{donnes_entreprise.Count} reviews trouvées au total, note moyenne : {moyenne}");
                } else {
                    Console.WriteLine($"Aucun avis trouvé pour {entreprise}");
                }

                Console.WriteLine();

            }

            string output = JsonConvert.SerializeObject(donees);
            StreamWriter sw = new StreamWriter("res.json");
            sw.WriteLine(output);
            sw.Close();

        }
    }
}