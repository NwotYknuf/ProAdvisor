using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TrustedShopsCrawler {
    class Program {
        public static void Main(string[] args) {

            string recherche = "www.pimkie.fr";
            Console.WriteLine($"Recherche d'avis pour {recherche}");

            TrustPilotScrapper tps = new TrustPilotScrapper();
            List<Review> tp_reviews = tps.getReviews(recherche).Result;
            Console.WriteLine($"{tp_reviews.Count} reviews trouvées pour TrustPilot");

            TrustedShopsScrapper tss = new TrustedShopsScrapper();
            List<Review> ts_reviews = tss.getReviews(recherche).Result;
            Console.WriteLine($"{ts_reviews.Count} reviews trouvées pour TrustedShops");

            double moyenne = 0.0;

            List<Review> all_reviews = new List<Review>();
            all_reviews.AddRange(tp_reviews);
            all_reviews.AddRange(ts_reviews);

            foreach (Review rev in all_reviews) {
                moyenne += rev.note;
                //Console.WriteLine($"date : {rev.date}, note : {rev.note}, commentaire : \n{rev.commentaire}\n");
            }

            string output = JsonConvert.SerializeObject(all_reviews);
            StreamWriter sw = new StreamWriter("res.json");
            sw.WriteLine(output);
            sw.Close();

            Console.WriteLine($"{all_reviews.Count} reviews trouvées au toal, note moyenne : {moyenne/all_reviews.Count}");

        }
    }
}