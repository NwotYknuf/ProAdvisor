using System;
using System.Collections.Generic;

namespace api.Model {
    public partial class ZoneIntervention {
        public string NomVille { get; set; }
        public string Siret { get; set; }

        public virtual Entreprise SiretNavigation { get; set; }
    }
}