using System.Collections.Generic;

namespace api.Model {

    public class ApiResEntreprise {

        public string Siret { get; set; }
        public string Siren { get; set; }
        public string Nom { get; set; }
        public string Representant { get; set; }
        public string Description { get; set; }
        public string Telephone { get; set; }
        public string Email { get; set; }
        public string Ville { get; set; }
        public string Adresse { get; set; }
        public string CodePostal { get; set; }

        public List<string> serveices { get; set; }
        public List<string> zonesIntervention { get; set; }

        public ApiResEntreprise(string siret, string siren, string nom, string representant, string description, string telephone, string email, string ville, string adresse, string codePostal, List<string> serveices, List<string> zonesIntervention) {
            Siret = siret;
            Siren = siren;
            Nom = nom;
            Representant = representant;
            Description = description;
            Telephone = telephone;
            Email = email;
            Ville = ville;
            Adresse = adresse;
            CodePostal = codePostal;
            this.serveices = serveices;
            this.zonesIntervention = zonesIntervention;
        }

    }

}