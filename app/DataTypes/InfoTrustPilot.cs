namespace ProAdvisor.app {
    public class InfoTrustPilot : Info {

        public InfoTrustPilot(string id, string url, string description = "", string adresse = "", string telephone = "", string email = "", string categories = "", string nom = "") : base(id) {
            this.url = url;
            this.description = description;
            this.adresse = adresse;
            this.telephone = telephone;
            this.email = email;
            this.categories = categories;
            this.nom = nom;
        }

        public string url { get; set; }
        public string nom { get; set; }
        public string description { get; set; }
        public string adresse { get; set; }
        public string telephone { get; set; }
        public string email { get; set; }
        public string categories { get; set; }

    }
}