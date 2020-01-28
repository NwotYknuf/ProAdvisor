using System;
using System.Collections.Generic;

namespace api.Model {
    public partial class APourServiceSite {
        public string Nom { get; set; }
        public string Url { get; set; }

        public virtual ServicePropose NomNavigation { get; set; }
        public virtual ServiceWeb UrlNavigation { get; set; }
    }
}