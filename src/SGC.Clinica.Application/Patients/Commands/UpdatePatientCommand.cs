using MediatR;
using SGC.Clinica.Application.Patients.Dtos;
using SGC.Clinica.Domain.Models;

public record UpdatePatientCommand(int Id, UpdatePatientDto PatientDto) : IRequest<Patient>;
