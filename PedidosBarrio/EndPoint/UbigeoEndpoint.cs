using MediatR;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Application.Queries.GetUbigeo;

namespace PedidosBarrio.Api.EndPoint
{
    public static class UbigeoEndpoint
    {
        public static void MapUbigeoEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/Ubigeo")
                           .WithTags("Ubigeo");

            group.MapGet("/departments", async (IMediator mediator) =>
            {
                var result = await mediator.Send(new GetUbigeoQuery());
                return Results.Ok(result);
            })
            .WithName("GetDepartments")
            .Produces<IEnumerable<UbigeoDto>>(StatusCodes.Status200OK);

            group.MapGet("/provinces/{departmentId}", async (string departmentId, IMediator mediator) =>
            {
                var result = await mediator.Send(new GetUbigeoQuery { DepartmentId = departmentId });
                return Results.Ok(result);
            })
            .WithName("GetProvinces")
            .Produces<IEnumerable<UbigeoDto>>(StatusCodes.Status200OK);

            group.MapGet("/districts/{provinceId}", async (string provinceId, IMediator mediator) =>
            {
                var result = await mediator.Send(new GetUbigeoQuery { ProvinceId = provinceId });
                return Results.Ok(result);
            })
            .WithName("GetDistricts")
            .Produces<IEnumerable<UbigeoDto>>(StatusCodes.Status200OK);
        }
    }
}
