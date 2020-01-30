using System;
using System.Collections.Generic;

namespace api.Model {
    public partial class ServicePropose {
        public ServicePropose() {
            APourServiceEntr = new HashSet<APourServiceEntr>();
            APourServiceSite = new HashSet<APourServiceSite>();
        }

        public string NomService { get; set; }

        public virtual ICollection<APourServiceEntr> APourServiceEntr { get; set; }
        public virtual ICollection<APourServiceSite> APourServiceSite { get; set; }
    }
}