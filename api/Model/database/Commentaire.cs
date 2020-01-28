using System;
using System.Collections.Generic;

namespace api.Model
{
    public partial class Commentaire
    {
        public int Id { get; set; }
        public int Note { get; set; }
        public DateTime Date { get; set; }
        public string Siret { get; set; }
        public string Source { get; set; }
        public string Auteur { get; set; }
        public string UrlService { get; set; }

        public virtual Auteur AuteurNavigation { get; set; }
        public virtual Entreprise SiretNavigation { get; set; }
        public virtual ServiceWeb UrlServiceNavigation { get; set; }
    }
}
