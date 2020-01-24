using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProAdvisor.app {

    public interface ISourceEntreprise {

        Task<List<Entreprise>> findEntreprise(string research, double lat, double lon);

    }
}