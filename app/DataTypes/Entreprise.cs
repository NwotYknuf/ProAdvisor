namespace ProAdvisor.app {

    public class Entreprise : Entite {

        public string nom;
        public string url;
        public string adresse;

        public Entreprise(string sIRET, string nom, string url, string adresse) : base(sIRET) {
            this.nom = nom;
            this.url = url;
            this.adresse = adresse;
        }
    }
}