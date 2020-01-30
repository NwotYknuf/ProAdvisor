using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers {
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

        // GET: api/Service?Ville=Metz&Zone=Borgny&Service=Plomberie
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ApiResServiceWeb>>> GetServices(string Service = null) {

            var services = await _context.ServiceWeb.
            Where(x => Service == null ? true : x.APourServiceSite.Select(x => x.Nom.ToLower()).Contains(Service.ToLower())).ToListAsync();

            List<ApiResServiceWeb> res = new List<ApiResServiceWeb>();

            foreach (ServiceWeb s in services) {
                res.Add(convert(s));
            }

            return res;
        }

        // GET: api/Service/www.pimkie.fr
        [HttpGet("{url}")]
        public async Task<ActionResult<ApiResServiceWeb>> GetService(string url) {
            var service = await _context.ServiceWeb.Where(x => x.UrlService.ToLower() == url.ToLower()).FirstOrDefaultAsync();

            if (service == null) {
                return NotFound();
            }

            return convert(service);
        }

        // GET: api/Service/www.pimkie.fr/Comments?Source=www.trustpilot.com&AFNOR=true
        [HttpGet("{url}/Comments")]
        public async Task<ActionResult<IEnumerable<ApiResCommentaire>>> GetServiceComments(string url, string Source = null, bool? estAFNOR = null) {
            var service = await _context.ServiceWeb.Where(x => x.UrlService == url).FirstOrDefaultAsync();

            if (service == null) {
                return NotFound();
            }

            List<ApiResCommentaire> res = new List<ApiResCommentaire>();

            var filteredComments = _context.Commentaire.Where(x => x.UrlService == service.UrlService).
            Where(x => Source == null ? true : x.Source.ToLower() == Source.ToLower()).ToList();

            foreach (Commentaire commentaire in filteredComments) {
                bool AFNOR = _context.Source.Where(x => x.Url == commentaire.Source).Select(x => x.RespecteAfnor).FirstOrDefault();
                if (estAFNOR != null) {
                    if (estAFNOR == AFNOR) {
                        res.Add(new ApiResCommentaire(commentaire.Note, commentaire.Date, commentaire.Siret, commentaire.Source, commentaire.Auteur, AFNOR, commentaire.Commentaire1));
                    }
                } else {
                    res.Add(new ApiResCommentaire(commentaire.Note, commentaire.Date, commentaire.Siret, commentaire.Source, commentaire.Auteur, AFNOR, commentaire.Commentaire1));
                }
            }

            return res;

        }
    }
}