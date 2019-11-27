namespace ProAdvisor.app {
    public class Donnee {

        public Donnee(Entreprise entreprise, Review review) {
            this.entreprise = entreprise;
            this.review = review;
        }

        public Entreprise entreprise { get; set; }
        public Review review { get; set; }
    }
}