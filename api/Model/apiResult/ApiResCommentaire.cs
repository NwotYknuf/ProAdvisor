using System;
using System.Collections.Generic;

namespace api.Model {
    public class ApiResCommentaire {

        public float Note { get; set; }
        public DateTime Date { get; set; }
        public string Siret { get; set; }
        public string Source { get; set; }
        public string Auteur { get; set; }
        public bool RespecteAfnor { get; set; }
        public string Commentaire { get; set; }

        public ApiResCommentaire(float note, DateTime date, string siret, string source, string auteur, bool respecteAfnor, string commentaire) {
            Note = note;
            Date = date;
            Siret = siret;
            Source = source;
            Auteur = auteur;
            RespecteAfnor = respecteAfnor;
            Commentaire = commentaire;
        }

    }
}