using System;
using System.Collections.Generic;

namespace api.Model {
    public class ApiResCommentaire {

        public int Note { get; set; }
        public DateTime Date { get; set; }
        public string Source { get; set; }
        public string Auteur { get; set; }
        public bool RespecteAfnor { get; set; }

        public ApiResCommentaire(int note, DateTime date, string source, string auteur, bool respecteAfnor) {
            Note = note;
            Date = date;
            Source = source;
            Auteur = auteur;
            RespecteAfnor = respecteAfnor;
        }

    }
}