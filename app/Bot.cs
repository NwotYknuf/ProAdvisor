using System.Collections.Generic;
using System.Threading.Tasks;


namespace ProAdvisor.app {
/*
 * Classe abstraite représentant un bot chargé de récuperer des avis sur un site source ou à partir d'une API
 */
    public abstract class Bot {

        public abstract Task<List<Review>> getReviews(string research);

    }
}