using MediatR;
using SIGMA.API.Reports.DTOs;

namespace SIGMA.API.Reports.Queries.WorkOrders;

// Query que pide las horas de taller (ActualHours) agrupadas por mes para los ultimos N meses, usada por el grafico de "Horas de Taller por Mes" en ReportsPage
public record GetMonthlyWorkshopHoursQuery(int Months = 7) : IRequest<MonthlyWorkshopHoursReportDto>;
