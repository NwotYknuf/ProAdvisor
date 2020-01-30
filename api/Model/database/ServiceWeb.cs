using System;
using System.Collections.Generic;

namespace api.Model {
    public partial class ServiceWeb {
        public ServiceWeb() {
            APourServiceSite = new HashSet<APourServiceSite>();
            Commentaire = new HashSet<Commentaire>();
        }

        public string UrlService { get; set; }
        public string Nom { get; set; }
        public string Description { get; set; }
        public string Adresse { get; set; }
        public string Telephone { get; set; }
        public string Email { get; set; }
        public string NumRegistre { get; set; }
        public string Representant { get; set; }

        public virtual ICollection<APourServiceSite> APourServiceSite { get; set; }
        public virtual ICollection<Commentaire> Commentaire { get; set; }
    }
}