namespace TrilhaNetAzureDesafio.EndPoints
{
    using FluentValidation;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.HttpResults;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Collections.Generic;
    using TrilhaNetAzureDesafio.Filters;
    using TrilhaNetAzureDesafio.Services;
    using TrilhaNetAzureDesafio.ViewModels;
    using Microsoft.AspNetCore.Routing;

    public static class FuncionarioEndPoints
    {
        public static void MapFuncionarioEndPints(this WebApplication app)
        {
            ArgumentNullException.ThrowIfNull(app, nameof(app));
            var resourceName = "Funcionarios";

            var mapping = app.MapGroup("/funcionario/v1");

            mapping.MapGet("/", ListAction)
                .WithName($"{resourceName} - Listagem")
                .WithSummary("Listagem de funcionários")
                .WithDescription("Efetua a listagem de funcionários, de acordo com os critérios informados.")
                .WithTags([resourceName])
                .Produces(statusCode: StatusCodes.Status200OK, contentType: "application/json", responseType: typeof(PagedViewModel<FuncionarioViewModel>))
                .Produces(statusCode: StatusCodes.Status500InternalServerError, contentType: "application/json", responseType: typeof(ProblemHttpResult))
                .WithOpenApi();

            mapping.MapGet("/{id}", FindAction)
                .WithName($"{resourceName} - Localizar")
                .WithSummary("Localizar funcionário")
                .WithDescription("Retorna um funcionário cadastrado no sistema, de acordo com os critérios informados.")
                .WithTags([resourceName])
                .Produces(statusCode: StatusCodes.Status200OK, contentType: "application/json", responseType: typeof(FuncionarioViewModel))
                .Produces(statusCode: StatusCodes.Status400BadRequest, contentType: "application/json", responseType: typeof(BadRequestResult))
                .Produces(statusCode: StatusCodes.Status404NotFound, contentType: "application/json", responseType: typeof(NotFoundResult))
                .Produces(statusCode: StatusCodes.Status500InternalServerError, contentType: "application/json", responseType: typeof(ProblemHttpResult))
                .WithOpenApi();

            mapping.MapPost("/", InsertAction)
                .WithName($"{resourceName} - Incluir")
                .WithSummary("Incluir funcionário")
                .WithDescription("Cadastra um novo funcionário no sistema")
                .WithTags([resourceName])
                .Produces(statusCode: StatusCodes.Status201Created, contentType: "application/json", responseType: typeof(Created<AddFuncionarioViewModel>))
                .Produces(statusCode: StatusCodes.Status400BadRequest, contentType: "application/json", responseType: typeof(BadRequestResult))
                .Produces(statusCode: StatusCodes.Status422UnprocessableEntity, contentType: "application/json", responseType: typeof(UnprocessableEntity<IDictionary<String, String[]>>))
                .Produces(statusCode: StatusCodes.Status500InternalServerError, contentType: "application/json", responseType: typeof(ProblemHttpResult))
                .WithOpenApi();

            mapping.MapPut("/", UpdateAction)
                .WithName($"{resourceName} - Alterar")
                .WithSummary("Alterar funcionário")
                .WithDescription("Altera os dados cadastrais do funcionário")
                .WithTags([resourceName])
                .Produces(statusCode: StatusCodes.Status204NoContent, contentType: "application/json", responseType: typeof(NoContentResult))
                .Produces(statusCode: StatusCodes.Status400BadRequest, contentType: "application/json", responseType: typeof(BadRequestResult))
                .Produces(statusCode: StatusCodes.Status404NotFound, contentType: "application/json", responseType: typeof(NotFoundResult))
                .Produces(statusCode: StatusCodes.Status422UnprocessableEntity, contentType: "application/json", responseType: typeof(UnprocessableEntity<IDictionary<String, String[]>>))
                .Produces(statusCode: StatusCodes.Status500InternalServerError, contentType: "application/json", responseType: typeof(ProblemHttpResult))
                .WithOpenApi(); 

            mapping.MapDelete("/{id}", DeleteAction)
                .WithName($"{resourceName} - excluir")
                .WithSummary("Excluir funcionário")
                .WithDescription("Exclui um funcionário cadastrado no sistema, de acordo com os critérios informados.")
                .WithTags([resourceName])
                .Produces(statusCode: StatusCodes.Status204NoContent, contentType: "application/json", responseType: typeof(NoContentResult))
                .Produces(statusCode: StatusCodes.Status400BadRequest, contentType: "application/json", responseType: typeof(BadRequestResult))
                .Produces(statusCode: StatusCodes.Status404NotFound, contentType: "application/json", responseType: typeof(NotFoundResult))
                .Produces(statusCode: StatusCodes.Status500InternalServerError, contentType: "application/json", responseType: typeof(ProblemHttpResult))
                .WithOpenApi();

            //mapping.MapGet("/log", ([FromServices] IFuncionarioService service) => 
            //{
            //    return service.Log();
            //});

        }

        static Results<Ok<PagedViewModel<FuncionarioViewModel>>, ProblemHttpResult> ListAction(
            [FromServices] IFuncionarioService service,
            [AsParameters] CriteriaFilter filter
        )
        {
            try
            {
                var page = 1;
                var nome = String.Empty;
                if (filter != null)
                {
                    if (filter.Page.HasValue)
                    {
                        page = filter.Page.Value;
                        if (page < 1)
                        {
                            page = 1;
                        }
                    }

                    if (!String.IsNullOrEmpty(filter.Nome) && !String.IsNullOrWhiteSpace(filter.Nome))
                    {
                        nome = filter.Nome;
                    }
                }
                var result = service.List(nome, page);
                return TypedResults.Ok(result);
            }
            catch (ApplicationException ex)
            {
                return TypedResults.Problem(
                    statusCode: StatusCodes.Status500InternalServerError,
                    detail: ex.Message,
                    title: "Houve um erro ao tentar listar os funcionários."
                );
            }
        }

        static Results<Ok<FuncionarioViewModel>, NotFound, BadRequest, ProblemHttpResult> FindAction(
            [FromServices] IFuncionarioService service,
            [FromQuery] String id
        )
        {
            try
            {
                if (!String.IsNullOrEmpty(id) && !String.IsNullOrWhiteSpace(id))
                {
                    if (Guid.TryParse(id, out var nrid))
                    {
                        var result = service.Find(nrid);
                        if (result != null)
                        {
                            return TypedResults.Ok(result);
                        }
                        return TypedResults.NotFound();
                    }
                }
                return TypedResults.BadRequest();
            }
            catch (ApplicationException ex)
            {
                return TypedResults.Problem(
                    statusCode: StatusCodes.Status500InternalServerError,
                    detail: ex.Message,
                    title: "Houve um erro ao tentar localizar o funcionário."
                );
            }
        }

        static Results<Created<AddFuncionarioViewModel>, BadRequest, UnprocessableEntity<IDictionary<String, String[]>>, ProblemHttpResult> InsertAction(
            [FromServices] IFuncionarioService service,
            [FromServices] IValidator<AddFuncionarioViewModel> validator,
            [FromBody] AddFuncionarioViewModel entity
        )
        {
            try
            {
                if (entity != null)
                {
                    var validation = validator.Validate(entity);
                    if (validation.IsValid)
                    {
                        var createdId = service.Insert(entity);
                        if (createdId != Guid.Empty)
                        {
                            return TypedResults.Created($"/funcionario/v1/{createdId}", entity);
                        }
                    }
                    else
                    {
                        return TypedResults.UnprocessableEntity(validation.ToDictionary());
                    }
                }
                return TypedResults.BadRequest();
            }
            catch (ApplicationException ex)
            {
                return TypedResults.Problem(
                    statusCode: StatusCodes.Status500InternalServerError,
                    detail: ex.Message,
                    title: "Houve um erro ao tentar incluir o funcionário."
                );
            }
        }

        static Results<NoContent, NotFound, BadRequest, UnprocessableEntity<IDictionary<String, String[]>>, ProblemHttpResult> UpdateAction(
            [FromServices] IFuncionarioService service,
            [FromServices] IValidator<FuncionarioViewModel> validator,
            [FromBody] FuncionarioViewModel entity
        )
        {
            try
            {
                if (entity != null)
                {
                    var validation = validator.Validate(entity);
                    if (validation.IsValid)
                    {
                        var exists = service.Exists(entity.Id.Value);
                        if (exists)
                        {
                            var result = service.Update(entity);
                            if (result)
                            {
                                return TypedResults.NoContent();
                            }
                        }
                        else
                        {
                            return TypedResults.NotFound();
                        }
                    }
                    else
                    {
                        return TypedResults.UnprocessableEntity(validation.ToDictionary());
                    }
                }
                return TypedResults.BadRequest();
            }
            catch (ApplicationException ex)
            {
                return TypedResults.Problem(
                    statusCode: StatusCodes.Status500InternalServerError,
                    detail: ex.Message,
                    title: "Houve um erro ao tentar alterar o funcionário."
                );
            }
        }

        static Results<NoContent, NotFound, BadRequest, ProblemHttpResult> DeleteAction(
            [FromServices] IFuncionarioService service,
            [FromQuery] String id
        )
        {
            try
            {
                if (!String.IsNullOrEmpty(id) && !String.IsNullOrWhiteSpace(id))
                {
                    if (Guid.TryParse(id, out var nrid))
                    {
                        var exists = service.Exists(nrid);
                        if (exists)
                        {
                            var result = service.Delete(nrid);
                            if (result)
                            {
                                return TypedResults.NoContent();
                            }
                        }
                        else
                        {
                            return TypedResults.NotFound();
                        }
                    }
                }
                return TypedResults.BadRequest();
            }
            catch (ApplicationException ex)
            {
                return TypedResults.Problem(
                    statusCode: StatusCodes.Status500InternalServerError,
                    detail: ex.Message,
                    title: "Houve um erro ao tentar excluir o funcionário."
                );
            }
        }

    }
}
