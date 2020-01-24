using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProAdvisor.app {

    public interface ISourceInfo {

        Task<Info> findInfos(Entite entite);

    }
}