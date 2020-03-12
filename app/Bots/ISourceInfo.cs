using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProAdvisor.app {

    /*
     * Interface pour les classes qui sont chargées de trouver des informations concernant une entité
     */
    public interface ISourceInfo {

        Task<Info> findInfos(Entite entite);

    }
}