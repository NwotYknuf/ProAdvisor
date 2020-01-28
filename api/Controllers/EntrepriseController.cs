using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class EntrepriseController : ControllerBase {
        private readonly ProAdvisorContext _context;

        public EntrepriseController(ProAdvisorContext context) {
            _context = context;
        }

        // GET: api/Entreprise?Ville=Metz&Zone=Borgny&Service=Plomberie
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ApiResEntreprise>>> GetEntreprise(string Ville = null, string Zone = null, string Service = null) {

            var entreprises = await _context.Entreprise.
            Where(x => Ville == null ? true : x.Ville == Ville).
            Where(x => Zone == null ? true : x.ZoneIntervention.Select(x => x.NomVille.ToLower()).Contains(Zone.ToLower())).
            Where(x => Service == null ? true : x.APourServiceEntr.Select(x => x.Nom.ToLower()).Contains(Service.ToLower())).ToListAsync();

            List<ApiResEntreprise> res = new List<ApiResEntreprise>();

            foreach (Entreprise e in entreprises) {
                List<string> zones = new List<string>();
                List<string> services = new List<string>();

                foreach (APourServiceEntr truc in e.APourServiceEntr) {
                    services.Add(truc.NomNavigation.NomService);
                }

                foreach (ZoneIntervention zone in e.ZoneIntervention) {
                    zones.Add(zone.NomVille);
                }

                res.Add(new ApiResEntreprise(e.Siret, e.Siren, e.Nom, e.Representant, e.Description, e.Telephone, e.Email, e.Ville, e.Adresse, e.CodePostal, services, zones));

            }

            return res;
        }

        // GET: api/Entreprise/12345678912345
        [HttpGet("{id}")]
        public async Task<ActionResult<Entreprise>> GetEntreprise(string id) {
            var entreprise = await _context.Entreprise.Where(x => x.Siret == id).FirstOrDefaultAsync();

            if (entreprise == null) {
                return NotFound();
            }

            return entreprise;
        }

        // GET: api/Entreprise/12345678912345/Comments?Source=www.trustpilot.com&AFNOR=true
        [HttpGet("{id}/Comments")]
        public async Task<ActionResult<IEnumerable<ApiResCommentaire>>> GetEntrepriseComments(string id, string Source = null, bool? estAFNOR = null) {
            var entreprise = await _context.Entreprise.Where(x => x.Siret == id).FirstOrDefaultAsync();

            if (entreprise == null) {
                return NotFound();
            }

            List<ApiResCommentaire> res = new List<ApiResCommentaire>();

            var filteredComments = entreprise.Commentaire.
            Where(x => Source == null ? true : x.Source.ToLower() == Source.ToLower()).
            Where(x => estAFNOR == null ? true : x.AuteurNavigation.UrlNavigation.RespecteAfnor == estAFNOR).ToList();

            foreach (Commentaire commentaire in filteredComments) {
                res.Add(new ApiResCommentaire(commentaire.Note, commentaire.Date, commentaire.Source, commentaire.AuteurNavigation.Nom, commentaire.AuteurNavigation.UrlNavigation.RespecteAfnor));
            }

            return res;
        }

    }
}