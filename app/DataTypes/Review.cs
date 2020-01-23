using System;
namespace ProAdvisor.app {
    public class Review {

        public string commentaire { get; set; }
        public Utilisateur auteur { get; set; }
        public Source source { get; set; }
        public double note { get; set; }
        public DateTime date { get; set; }

        public Review(DateTime date, string commentaire, double note, Source source, Utilisateur auteur) {
            this.date = date;
            this.commentaire = commentaire;
            this.note = note;
            this.auteur = auteur;
            this.source = source;
        }

    }
}