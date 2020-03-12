using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers {

    /*
     * Controller qui gère les requêtes sur les entreprises
     */
    [Route("[controller]")]
    [ApiController]
    public class EntrepriseController : ControllerBase {
        private readonly ProAdvisorContext _context;

        public EntrepriseController(ProAdvisorContext context) {
            _context = context;
        }

        private ApiResEntreprise convert(Entreprise e) {
            List<string> zones = new List<string>();
            List<string> services = new List<string>();

            foreach (APourServiceEntr truc in _context.APourServiceEntr.Where(x => x.Siret == e.Siret)) {
                services.Add(truc.Nom);
            }

            foreach (ZoneIntervention zone in _context.ZoneIntervention.Where(x => x.Siret == e.Siret)) {
                zones.Add(zone.NomVille);
            }

            return new ApiResEntreprise(e.Siret, e.Siren, e.Nom, e.Representant, e.Description, e.Telephone, e.Email, e.Ville, e.Adresse, e.CodePostal, services, zones);

        }

        // GET: Entreprise?Ville=Metz&Zone=Borgny&Service=Plomberie&Gratuit=true&NbCommMin=3
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ApiResEntreprise>>> GetEntreprise(string Ville = null, string Zone = null, string Service = null, bool? Gratuit = null, int ? NbCommMin = null) {

            var entreprises = await _context.Entreprise.
            Where(x => Ville == null ? true : x.Ville.ToLower().Contains(Ville.ToLower())).
            Where(x => Zone == null ? true : x.ZoneIntervention.Select(x => x.NomVille.ToLower()).Any(x => x.Contains(Zone.ToLower()))).
            Where(x => Service == null ? true : x.APourServiceEntr.Select(x => x.Nom.ToLower()).Any(x => x.Contains(Service.ToLower()))).
            Where(x => Gratuit == null ? true : x.APourServiceEntr.Select(x => x.Nom.ToLower()).Any(x => x.Contains("gratuit"))).
            Where(x => NbCommMin == null ? true : x.Commentaire.Count() >= NbCommMin).ToListAsync();

            List<ApiResEntreprise> res = new List<ApiResEntreprise>();

            foreach (Entreprise e in entreprises) {
                res.Add(convert(e));
            }

            return res;
        }

        // GET: Entreprise/12345678912345
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResEntreprise>> GetEntreprise(string id) {
            var entreprise = await _context.Entreprise.Where(x => x.Siret == id).FirstOrDefaultAsync();

            if (entreprise == null) {
                return NotFound();
            }

            return convert(entreprise);
        }

        // GET: Entreprise/12345678912345/Comments?Source=www.trustpilot.com&AFNOR=true&Note=3&DateMin=01/01/2018&DateMax=01/01/2019
        [HttpGet("{id}/Comments")]
        public async Task<ActionResult<IEnumerable<ApiResCommentaire>>> GetEntrepriseComments(string id, string Source = null, bool? AFNOR = null, int ? Note = null, string DateMin = null, string DateMax = null) {
            var entreprise = await _context.Entreprise.Where(x => x.Siret == id).FirstOrDefaultAsync();

            if (entreprise == null) {
                return NotFound();
            }

            List<ApiResCommentaire> res = new List<ApiResCommentaire>();

            DateTime? min = null;
            DateTime? max = null;

            if (DateMin != null) {
                min = DateTime.ParseExact(DateMin, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }

            if (DateMax != null) {
                max = DateTime.ParseExact(DateMax, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }

            var filteredComments = _context.Commentaire.Where(x => x.Siret == entreprise.Siret).
            Where(x => Source == null ? true : x.Source.ToLower() == Source.ToLower()).
            Where(x => Note == null ? true : x.Note == Note).
            Where(x => min == null ? true : x.Date >= min).
            Where(x => max == null ? true : x.Date <= max).ToList();

            foreach (Commentaire commentaire in filteredComments) {
                bool estAFNOR = _context.Source.Where(x => x.Url == commentaire.Source).Select(x => x.RespecteAfnor).FirstOrDefault();
                if (AFNOR != null) {
                    if (AFNOR == estAFNOR) {
                        res.Add(new ApiResCommentaire(commentaire.Note, commentaire.Date, commentaire.Siret, commentaire.Source, commentaire.Auteur, estAFNOR, commentaire.Commentaire1));
                    }
                } else {
                    res.Add(new ApiResCommentaire(commentaire.Note, commentaire.Date, commentaire.Siret, commentaire.Source, commentaire.Auteur, estAFNOR, commentaire.Commentaire1));
                }
            }

            return res;
        }

    }
}