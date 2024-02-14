using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using DinnerPlans.Server.Core.IServices;
using DinnerPlans.Server.Core.Services;
using DinnerPlans.Server.Core;
using DinnerPlans.Server.Persistence.IRepositories;
using DinnerPlans.Server.Persistence.Repositories;
using DinnerPlans.Server.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
builder.Services.AddTransient<IReadRepo, ReadRepo>();
builder.Services.AddTransient<IWriteRepo, WriteRepo>();
builder.Services.AddTransient<IAppService, AppService>();
builder.Services.AddTransient<IKrogerApiService, KrogerApiService>();

builder.Services.AddDbContext<DinnerPlansContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//TODO figure out if I need this?
//builder.Services.AddMvc().AddNewtonsoftJson(options =>
//{
//    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
//});

//from https://auth0.com/blog/aspnet-web-api-authorization/
//and  https://auth0.com/blog/securing-blazor-webassembly-apps/ ?
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(options =>
    {
        options.Authority = $"https://{builder.Configuration["Auth0:Domain"]}/";
        options.TokenValidationParameters =
          new Microsoft.IdentityModel.Tokens.TokenValidationParameters
          {
              ValidAudience = builder.Configuration["Auth0:Audience"],
              ValidIssuer = $"https://{builder.Configuration["Auth0:Domain"]}",
              ValidateLifetime = true,
          };
    });
builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "Glossary", Version = "v1" });
        var securitySchema = new OpenApiSecurityScheme
        {
            Description = "Using the Authorization header with the Bearer scheme.",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            }
        };

        c.AddSecurityDefinition("Bearer", securitySchema);

        c.AddSecurityRequirement(new OpenApiSecurityRequirement
          {
              { securitySchema, new[] { "Bearer" } }
          });
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

//from https://auth0.com/blog/aspnet-web-api-authorization/
app.UseAuthentication();
app.UseAuthorization();


app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
