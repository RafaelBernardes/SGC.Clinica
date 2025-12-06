using MediatR;
using SGC.Clinica.Api.Dtos;
using SGC.Clinica.Api.Models;

public record UpdatePatientCommand(int Id, UpdatePatientDto PatientDto) : IRequest<Patient>;