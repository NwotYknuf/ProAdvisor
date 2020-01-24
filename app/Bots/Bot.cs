using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProAdvisor.app {
    /*
     * Classe abstraite représentant un bot chargé de récuperer des avis sur un site source ou à partir d'une API
     */
    public abstract class Bot {

        private string _source;

        public string source {
            get { return _source; }
            set { _source = value; }
        }

        public abstract void destroy();
    }
}