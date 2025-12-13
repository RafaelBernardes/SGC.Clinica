using MediatR;
using SGC.Clinica.Api.Application.Patients.Dtos;
using SGC.Clinica.Api.Domain.Models;

public record UpdatePatientCommand(int Id, UpdatePatientDto PatientDto) : IRequest<Patient>;