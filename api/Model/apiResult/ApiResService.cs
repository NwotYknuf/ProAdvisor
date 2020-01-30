using System;
using System.Collections.Generic;

namespace api.Model {
    public class ApiResServiceWeb {

        public string UrlService { get; set; }
        public string Nom { get; set; }
        public string Description { get; set; }
        public string Adresse { get; set; }
        public string Telephone { get; set; }
        public string Email { get; set; }
        public string NumRegistre { get; set; }
        public string Representant { get; set; }

        public List<string> Services;

        public ApiResServiceWeb(string urlService, string nom, string description, string adresse, string telephone, string email, string numRegistre, string representant, List<string> services) {
            UrlService = urlService;
            Nom = nom;
            Description = description;
            Adresse = adresse;
            Telephone = telephone;
            Email = email;
            NumRegistre = numRegistre;
            Representant = representant;
            Services = services;
        }

    }
}