using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProAdvisor.app {

    public interface ISourceReview {

        Task<List<Review>> findReviews(Entite entite, DateTime date);

    }
}