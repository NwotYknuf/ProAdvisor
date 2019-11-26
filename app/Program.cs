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

            Console.WriteLine("Entrez le l'url que vous souhaitez rechercher :");
            string recherche = Console.ReadLine();

            List<Bot> bots = new List<Bot>();

            bots.Add(new TrustPilotScrapper());
            bots.Add(new TrustedShopsScrapper());

            ConcurrentDictionary<string, List<Review>> reviewsPerSource = new ConcurrentDictionary<string, List<Review>>();

            Console.WriteLine($"Recherche d'avis pour : {recherche}");

            /*
             * Boucle en parallel pour chaque bot
             */
            Parallel.ForEach(
                bots, new ParallelOptions { MaxDegreeOfParallelism = 4 },
                (bot) => {
                    try {
                        List<Review> reviews = bot.getReviews(recherche).Result;
                        reviewsPerSource.TryAdd(bot.source, reviews);
                        Console.WriteLine($"{reviews.Count} reveiw(s) trouvée(s) pour la source {bot.source}");
                    } catch (Exception e) {
                        Console.WriteLine($"Erreur pour la source {bot.source} :\n{e.Message}");
                    }
                }
            );

            List<Review> all_reviews = new List<Review>();

            foreach (var keyValuePair in reviewsPerSource) {
                all_reviews.AddRange(keyValuePair.Value);
            }

            if (all_reviews.Count > 0) {
                double moyenne = 0.0;
                foreach (Review rev in all_reviews) {
                    moyenne += rev.note;
                }
                moyenne /= all_reviews.Count;

                string output = JsonConvert.SerializeObject(all_reviews);
                StreamWriter sw = new StreamWriter("res.json");
                sw.WriteLine(output);
                sw.Close();

                Console.WriteLine($"{all_reviews.Count} reviews trouvées au total, note moyenne : {moyenne}");
            } else {
                Console.WriteLine($"Aucuns avis trouvés pour {recherche}");
            }

        }
    }
}