using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProAdvisor.app {

    /*
     * Interface pour les classes qui sont chargées de trouver des entreprises autour d'une position
     */
    public interface ISourceEntreprise {

        Task<List<Entreprise>> findEntreprise(string research, double lat, double lon);

    }
}