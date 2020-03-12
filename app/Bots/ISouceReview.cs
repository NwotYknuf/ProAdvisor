using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProAdvisor.app {

    /*
     * Interface pour les classes qui sont chargées de trouver des commentaires sur une entité
     */
    public interface ISourceReview {

        Task<List<Review>> findReviews(Entite entite, DateTime date);

    }
}