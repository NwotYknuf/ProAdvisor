using System;
using System.Collections.Generic;

namespace api.Model {
    public partial class APourServiceEntr {
        public string Nom { get; set; }
        public string Siret { get; set; }

        public virtual ServicePropose NomNavigation { get; set; }
        public virtual Entreprise SiretNavigation { get; set; }
    }
}