using System;
using System.Collections.Generic;

namespace api.Model {
    public partial class Auteur {
        public Auteur() {
            Commentaire = new HashSet<Commentaire>();
        }

        public string Nom { get; set; }
        public string Url { get; set; }
        public sbyte? IsAnonyme { get; set; }

        public virtual Source UrlNavigation { get; set; }
        public virtual ICollection<Commentaire> Commentaire { get; set; }
    }
}