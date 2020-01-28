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

        // GET: api/Entreprise
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Entreprise>>> GetEntreprise() {
            return await _context.Entreprise.ToListAsync();
        }

        // GET: api/Entreprise/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Entreprise>> GetEntreprise(string id) {
            var entreprise = await _context.Entreprise.FindAsync(id);

            if (entreprise == null) {
                return NotFound();
            }

            return entreprise;
        }

    }
}