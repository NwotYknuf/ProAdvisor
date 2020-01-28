using System.Collections.Generic;

namespace ProAdvisor.app {

    public class Entreprise : Entite {

        public string nom;
        public string url;
        public string adresse;
        public List<string> prestations;
        public List<string> zonesIntervention;

        public Entreprise(string sIRET, string nom, string url, string adresse, List<string> prestations, List<string> zonesIntervention) : base(sIRET) {
            this.nom = nom;
            this.url = url;
            this.adresse = adresse;
            this.prestations = prestations;
            this.zonesIntervention = zonesIntervention;
        }
    }
}