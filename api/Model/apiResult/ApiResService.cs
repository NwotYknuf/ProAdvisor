using System;
using System.Collections.Generic;

namespace api.Model {
    public class ApiResServiceWeb {

        public string UrlService { get; set; }
        public string Nom { get; set; }

        public ApiResServiceWeb(string urlService, string nom) {
            UrlService = urlService;
            Nom = nom;
        }

    }
}