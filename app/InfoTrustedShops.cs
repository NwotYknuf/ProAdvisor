namespace ProAdvisor.app {
    public class InfoTrustedShops : InfoEntreprise {

        public InfoTrustedShops(string url, string description = "", string adresse = "", string telephone = "", string email = "", string categories = "", string nom = "", string num_regisre = "", string representant = "") {
            this.url = url;
            this.description = description;
            this.adresse = adresse;
            this.telephone = telephone;
            this.email = email;
            this.categories = categories;
            this.nom = nom;
            this.representant = representant;
            this.num_regisre = num_regisre;
        }

        public string url { get; set; }
        public string nom { get; set; }
        public string description { get; set; }
        public string adresse { get; set; }
        public string telephone { get; set; }
        public string email { get; set; }
        public string categories { get; set; }
        public string num_regisre { get; set; }
        public string representant { get; set; }
    }
}