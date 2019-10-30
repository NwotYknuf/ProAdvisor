using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TrustedShopsCrawler {
    class Program {
        public static async Task Main(string[] args) {

            string url_entreprise = "moncoffrage.com";
            ClientTrusted client = new ClientTrusted();

            List<Review> reviews = await client.getReviews(url_entreprise);
            double moyenne = 0.0;

            foreach (Review review in reviews) {
                moyenne += review.note;
                Console.WriteLine("Date : {0}, Note {1} Commentaire :\n{2}\n", review.date, review.note, review.commentaire);
            }

            moyenne /= reviews.Count;

            Console.WriteLine("Moyenne pour {0} : {1}", url_entreprise, moyenne);

        }
    }
}