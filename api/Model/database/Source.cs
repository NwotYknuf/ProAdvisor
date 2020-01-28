using System;
using System.Collections.Generic;

namespace api.Model
{
    public partial class Source
    {
        public Source()
        {
            Auteur = new HashSet<Auteur>();
        }

        public string Url { get; set; }
        public string Nom { get; set; }
        public bool RespecteAfnor { get; set; }

        public virtual ICollection<Auteur> Auteur { get; set; }
    }
}
