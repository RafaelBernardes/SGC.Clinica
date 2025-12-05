using Microsoft.AspNetCore.Mvc;
using MediatR;
using SGC.Clinica.Api.Dtos;
using SGC.Clinica.Api.Application.Patients.Commands;
using SGC.Clinica.Api.Application.Patients.Queries;

namespace SGC.Clinica.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PatientsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> AddPatient([FromBody] AddPatientDto patient)
        {
            var command = new AddPatientCommand(patient);
            var createdPatient = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetPatientById), new { id = createdPatient.Id }, createdPatient);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPatientById(int id)
        {
            var query = new GetPatientByIdQuery(id);
            var patient = await _mediator.Send(query);

            if (patient == null)
            {
                return NotFound();
            }
            
            return Ok(patient);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPatients()
        {
            var query = new GetPatientsListQuery();
            var patients = await _mediator.Send(query);
            
            return Ok(patients);
        }
    }
}