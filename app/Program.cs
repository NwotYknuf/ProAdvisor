using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ProAdvisor.app {
    class Program {
        public static void Main(string[] args) {

            Console.WriteLine("Entrez l'url que vous souhaitez rechercher :");
            string recherche = Console.ReadLine();

            List<Bot> bots = new List<Bot>();

            bots.Add(new TrustPilotScrapper());
            bots.Add(new TrustedShopsScrapper());

            ConcurrentDictionary<string, List<Review>> reviewsPerSource = new ConcurrentDictionary<string, List<Review>>();

            Console.WriteLine($"Recherche d'avis pour : {recherche}");

            /*
             * Boucle en parallèle pour chaque bot
             */
            Parallel.ForEach(
                bots, new ParallelOptions { MaxDegreeOfParallelism = 4 },
                (bot) => {
                    try {
                        List<Review> reviews = bot.getReviews(recherche).Result;
                        reviewsPerSource.TryAdd(bot.source, reviews);
                        Console.WriteLine($"{reviews.Count} review(s) trouvée(s) pour la source {bot.source}");
                    } catch (Exception e) {
                        Console.WriteLine($"Erreur pour la source {bot.source} :\n{e.Message}");
                    }
                }
            );

            List<Donnee> donnes = new List<Donnee>();

            foreach (var keyValuePair in reviewsPerSource) {

                foreach (Review review in keyValuePair.Value) {
                    Donnee donne = new Donnee(new Entreprise(recherche), review);
                    donnes.Add(donne);
                }

            }

            if (donnes.Count > 0) {
                double moyenne = 0.0;
                foreach (Donnee donnee in donnes) {
                    moyenne += donnee.review.note;
                }
                moyenne /= donnes.Count;

                string output = JsonConvert.SerializeObject(donnes);
                StreamWriter sw = new StreamWriter("res.json");
                sw.WriteLine(output);
                sw.Close();

                Console.WriteLine($"{donnes.Count} reviews trouvées au total, note moyenne : {moyenne}");
            } else {
                Console.WriteLine($"Aucun avis trouvé pour {recherche}");
            }

        }
    }
}