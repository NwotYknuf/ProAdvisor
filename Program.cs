using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TrustedShopsCrawler {
    class Program {
        public static async Task Main(string[] args) {

            TrustedShopsScrapper ts = new TrustedShopsScrapper();
            List<Review> reviews = await ts.getReviews("moncoffrage.com");

            double moyenne = 0.0;

            foreach (Review rev in reviews) {
                moyenne += rev.note;
                Console.WriteLine($"date : {rev.date}, note : {rev.note}, commentaire : \n{rev.commentaire}\n");
            }

            Console.WriteLine($"{reviews.Count} reviews trouvées, note moyenne : {moyenne/reviews.Count}");

        }
    }
}