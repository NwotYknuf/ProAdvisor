using System;
namespace ProAdvisor.app {
    public class Review {

        //TODO ajouter auteur et source du commentaire

        public string commentaire { get; set; }
        public Utilisateur auteur { get; set; }
        public double note { get; set; }
        public string source { get; set; }
        public DateTime date { get; set; }

        public Review(DateTime date, string commentaire, double note, string source, Utilisateur auteur) {
            this.date = date;
            this.commentaire = commentaire;
            this.note = note;
            this.auteur = auteur;
            this.source = source;
        }

    }
}