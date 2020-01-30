using System;
using System.Collections.Generic;

namespace api.Model {
    public partial class Entreprise {
        public Entreprise() {
            APourServiceEntr = new HashSet<APourServiceEntr>();
            Commentaire = new HashSet<Commentaire>();
            ZoneIntervention = new HashSet<ZoneIntervention>();
        }

        public string Siret { get; set; }
        public string Siren { get; set; }
        public string Nom { get; set; }
        public string Representant { get; set; }
        public string Description { get; set; }
        public string Telephone { get; set; }
        public string Email { get; set; }
        public string Ville { get; set; }
        public string Adresse { get; set; }
        public string CodePostal { get; set; }
        public string Url { get; set; }

        public virtual ICollection<APourServiceEntr> APourServiceEntr { get; set; }
        public virtual ICollection<Commentaire> Commentaire { get; set; }
        public virtual ICollection<ZoneIntervention> ZoneIntervention { get; set; }
    }
}