namespace TrilhaNetAzureDesafio
{
    using FluentValidation;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.OpenApi.Models;
    using System;
    using System.IO;
    using System.Text.Json.Serialization;
    using System.Text.Json;
    using TrilhaNetAzureDesafio.Context;
    using TrilhaNetAzureDesafio.EndPoints;
    using TrilhaNetAzureDesafio.Profiles;
    using TrilhaNetAzureDesafio.Repositories;
    using TrilhaNetAzureDesafio.Repositories.Impl;
    using TrilhaNetAzureDesafio.Services;
    using TrilhaNetAzureDesafio.Validation;
    using TrilhaNetAzureDesafio.ViewModels;
    using Microsoft.AspNetCore.Http.Json;
    using TrilhaNetAzureDesafio.Converters;
    using Microsoft.Extensions.Hosting;
    using Microsoft.AspNetCore.Http;
    using Serilog;
    using System.Globalization;
    using Serilog.Formatting.Compact;
    using System.Text;
    using Microsoft.Data.SqlClient;
    using Azure.Data.Tables;

    sealed class Program
    {
        static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(configuration);

            //Entity Framework settings
            var connectionBuilder = new SqlConnectionStringBuilder();
            var dataSource = configuration["DatabaseConnection:DataSource"];
            var isLocalDb = configuration.GetValue<Boolean>("DatabaseConnection:localDb");
            if (isLocalDb) dataSource = @$"(localdb)\{configuration["DatabaseConnection:DataSource"]}";
            var initialCatalog = configuration["DatabaseConnection:InitialCatalog"];
            var dbPath = Path.Combine(Directory.GetCurrentDirectory(), "Databases");
            if (!Directory.Exists(dbPath))
            {
                Directory.CreateDirectory(dbPath);
            }
            var dbFileName = Path.Combine(dbPath, $"{configuration["DatabaseConnection:DatabaseName"]}.mdf");
            connectionBuilder.DataSource = dataSource;
            connectionBuilder.InitialCatalog = initialCatalog;
            connectionBuilder.IntegratedSecurity = true;
            connectionBuilder.AttachDBFilename = dbFileName;
            var connection = connectionBuilder.ConnectionString;

            services.AddDbContext<RHContext>(options =>
                options.UseSqlServer(connection)
            );

            //azure table
            // With account name and key
            var url = configuration.GetValue<String>("AzureTableStorage:Url");
            var account = configuration.GetValue<String>("AzureTableStorage:UserAccount");
            var key = configuration.GetValue<String>("AzureTableStorage:UserKey");

            var client = new TableServiceClient(
                endpoint: new Uri($"{url}/{account}"),
                credential: new TableSharedKeyCredential(account, key)
             );

            services.AddSingleton<TableServiceClient>(client);

            services.AddScoped<IAzureTableContext, AzureTableContext>();

            //AutoMapper
            services.AddAutoMapper(settings =>
            {
                settings.AddProfiles([new FuncionarioProfile()]);
            });

            services.AddSerilog((services, lc) => lc
               .ReadFrom.Services(services)
               .Enrich.FromLogContext()
               .WriteTo.Console(formatProvider: CultureInfo.CurrentCulture)
               .WriteTo.File(formatter: new CompactJsonFormatter(),
                             path: "./logs/log-.txt",
                             rollingInterval: RollingInterval.Day,
                             rollOnFileSizeLimit: true,
                             retainedFileCountLimit: 10,
                             encoding: Encoding.UTF8
               )
           );

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader();
                    });
            });

            services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromHours(1);
            });

            
            //repositories
            services.AddScoped<IFuncionarioRepository, FuncionarioRepository>();
            services.AddScoped<IFuncionarioLogRepository, FuncionarioLogRepository>();

            //services
            services.AddScoped<IFuncionarioService, FuncionarioService>();

            //Validation
            services.AddScoped<IValidator<AddFuncionarioViewModel>, AddFuncionarioValidation>();
            services.AddScoped<IValidator<FuncionarioViewModel>, FuncionarioValidation>();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo()
                {
                    Title = "Api Desafio Azure",
                    Version = "1.0",
                    Description = "Solução do Desafio de projeto referente a um sistema de cadastro de funcionários utilizando minimal apis com Entity Framework e o uso de Azure Tables, para a gravação do log.",
                    //TermsOfService = new Uri("https://example.com/terms"),
                    Contact = new OpenApiContact()
                    {
                        Name = "Leonardo Buta",
                        //Url = new Uri("")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Termos de uso",
                        Url = new Uri("https://app.dio.me/terms/")
                    }
                });
            });

            services.Configure<JsonOptions>(options =>
            {
                options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.SerializerOptions.WriteIndented = true;
                options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                options.SerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString;
                options.SerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
                options.SerializerOptions.Converters.Add(new JsonGuidConverter());
            });
        }

        static void ConfigureApplication(WebApplication app)
        {
            //headers de resposta das solicitações
            var policyCollection = new HeaderPolicyCollection()
                .AddFrameOptionsDeny()
                .AddXssProtectionBlock()
                .AddContentTypeOptionsNoSniff()
                .AddStrictTransportSecurityMaxAgeIncludeSubDomains(maxAgeInSeconds: 60 * 60 * 24 * 365) // maxage = one year in seconds
                .AddReferrerPolicyStrictOriginWhenCrossOrigin()
                .RemoveServerHeader()
                .AddContentSecurityPolicy(builder =>
                {
                    builder.AddObjectSrc().None();
                    builder.AddFormAction().Self();
                    builder.AddFrameAncestors().None();
                })
                .AddCrossOriginOpenerPolicy(builder =>
                {
                    builder.SameOrigin();
                })
                .AddCrossOriginEmbedderPolicy(builder =>
                {
                    builder.RequireCorp();
                })
                .AddCrossOriginResourcePolicy(builder =>
                {
                    builder.SameOrigin();
                }); //.AddCustomHeader("X-My-Test-Header", "Header value")


            app.UseSerilogRequestLogging();

            app.UseSecurityHeaders(policyCollection);
            app.UseCors();
            app.UseHsts();
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Api Desafio Azure V1"));
            }

            // Swagger
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();

            app.MapGet("/", (HttpContext context) =>
            {
                var builder = new UriBuilder
                {
                    Scheme = context.Request.Scheme,
                    Host = context.Request.Host.Host,
                    Port = context.Request.Host.Port ?? 80,
                    Path = context.Request.PathBase
                };
                builder.Path += "swagger";

                return new HomeEndPoint("Bem vindo a API de funcionários - Api Desafio Azure", builder.Uri);
            })
            .WithName("GetIndex")
            .WithTags("Home")
            .ExcludeFromDescription();

            app.MapFuncionarioEndPints();
        }


        static void Main(String[] args)
        {
            // The initial "bootstrap" logger is able to log errors during start-up. It's completely replaced by the
            // logger configured in `AddSerilog()` below, once configuration and dependency-injection have both been
            // set up successfully.
            Log.Logger = new LoggerConfiguration()
                            .Enrich.FromLogContext()
                            .WriteTo.Console(formatProvider: CultureInfo.CurrentCulture)
                            .CreateBootstrapLogger();

            Log.Information("Starting up!");

            try
            {
                var builder = WebApplication.CreateBuilder(args);
                builder.Configuration.SetBasePath(Directory.GetCurrentDirectory());
                builder.Configuration.AddJsonFile("config.json", false, true);

                ConfigureServices(builder.Services, builder.Configuration);

                var app = builder.Build();

                ConfigureApplication(app);

                app.Run();
            }
            catch (ApplicationException ex)
            {
                Log.Fatal(ex, "Application terminated unexpectedly.");
            }
            finally
            {
                Log.Information("Finishing app!");
                Log.CloseAndFlush();
            }
        }
    }
}