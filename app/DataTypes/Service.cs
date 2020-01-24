namespace ProAdvisor.app {
    public class Service : Entite {

        public string nom;

        public Service(string url, string nom) : base(url) {
            this.nom = nom;
        }
    }
}