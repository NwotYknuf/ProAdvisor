using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers {

    /*
     * Controller qui gère les requêtes sur les services web
     */

    [Route("[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase {
        private readonly ProAdvisorContext _context;

        public ServiceController(ProAdvisorContext context) {
            _context = context;
        }

        private ApiResServiceWeb convert(ServiceWeb s) {

            List<string> services = new List<string>();

            foreach (APourServiceSite truc in _context.APourServiceSite.Where(x => x.Url == s.UrlService)) {
                services.Add(truc.Nom);
            }

            return new ApiResServiceWeb(s.UrlService, s.Nom, s.Description, s.Adresse, s.Telephone, s.Email, s.NumRegistre, s.Representant, services);
        }

        // GET: Service?Service=Plomberie&Gratuit=true&NbCommMin=3
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ApiResServiceWeb>>> GetServices(string Service = null, bool? Gratuit = null, int? NbCommMin = null) {

            var services = await _context.ServiceWeb.
            Where(x => Service == null ? true : x.APourServiceSite.Select(x => x.Nom.ToLower()).Any(x => x.Contains(Service.ToLower()))).
            Where(x => Gratuit == null ? true : x.APourServiceSite.Select(x => x.Nom.ToLower()).Any(x => x.Contains("gratuit"))).
            Where(x => NbCommMin == null ? true : x.Commentaire.Count() >= NbCommMin).ToListAsync();

            List<ApiResServiceWeb> res = new List<ApiResServiceWeb>();

            foreach (ServiceWeb s in services) {
                res.Add(convert(s));
            }

            return res;
        }

        // GET: Service/www.pimkie.fr
        [HttpGet("{url}")]
        public async Task<ActionResult<ApiResServiceWeb>> GetService(string url) {
            var service = await _context.ServiceWeb.Where(x => x.UrlService.ToLower() == url.ToLower()).FirstOrDefaultAsync();

            if (service == null) {
                return NotFound();
            }

            return convert(service);
        }

        // GET: Service/www.pimkie.fr/Comments?Source=www.trustpilot.com&AFNOR=true&Note=3&DateMin=01/01/2018&DateMax=01/01/2019
        [HttpGet("{url}/Comments")]
        public async Task<ActionResult<IEnumerable<ApiResCommentaire>>> GetServiceComments(string url, string Source = null, bool? AFNOR = null, int ? Note = null, string DateMin = null, string DateMax = null) {
            var service = await _context.ServiceWeb.Where(x => x.UrlService == url).FirstOrDefaultAsync();

            if (service == null) {
                return NotFound();
            }

            DateTime? min = null;
            DateTime? max = null;

            if (DateMin != null) {
                min = DateTime.ParseExact(DateMin, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }

            if (DateMax != null) {
                max = DateTime.ParseExact(DateMax, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }

            List<ApiResCommentaire> res = new List<ApiResCommentaire>();

            var filteredComments = _context.Commentaire.Where(x => x.UrlService == service.UrlService).
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