using System;
using System.Collections.Generic;

namespace api.Model {
    public partial class ServicePropose {
        public string NomService { get; set; }

        public virtual APourServiceEntr APourServiceEntr { get; set; }
        public virtual APourServiceSite APourServiceSite { get; set; }
    }
}