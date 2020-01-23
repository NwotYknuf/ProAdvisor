using System.Collections.Generic;
using System.Threading.Tasks;

public interface ISourceEntreprise {

    Task<List<Entreprise>> findEntreprise(string research);

}