using System;

namespace ProAdvisor.app {
    public class ReviewBasic : Review {

        public string auteur;
        public DateTime date;
        public double note;
        public string commentaire;

        public ReviewBasic(string idEntite, string source, string auteur, DateTime date, double note, string commentaire) : base(idEntite, source) {
            this.auteur = auteur;
            this.date = date;
            this.note = note;
            this.commentaire = commentaire;
        }
    }
}