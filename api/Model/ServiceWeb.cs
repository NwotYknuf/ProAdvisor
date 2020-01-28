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

        public virtual ICollection<APourServiceSite> APourServiceSite { get; set; }
        public virtual ICollection<Commentaire> Commentaire { get; set; }
    }
}