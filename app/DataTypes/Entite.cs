namespace ProAdvisor.app {
    public class Entite {
        public string id;

        public Entite(string id) {
            this.id = id;
        }

        public string researchString {
            get {
                if (this is Service) {
                    return id;
                }

                if (this is Entreprise) {
                    if ((this as Entreprise).url == "") {
                        throw new PasDeSiteWebException();
                    }
                    return (this as Entreprise).url;
                }

                return null;
            }
        }
    }
}