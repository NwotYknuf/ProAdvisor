namespace ProAdvisor.app {
    public class Donnee {

        public Donnee(InfoEntreprise entreprise, Review review) {
            this.entreprise = entreprise;
            this.review = review;
        }

        public InfoEntreprise entreprise { get; set; }
        public Review review { get; set; }
    }
}