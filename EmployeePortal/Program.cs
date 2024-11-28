using EmployeePortal.Data;
using EmployeePortal.Interface;
using EmployeePortal.Models;
using EmployeePortal.Repository;
using EmployeePortal.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.Identity.Web;

var builder = WebApplication.CreateBuilder(args);   

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at h
builder.Services.AddEndpointsApiExplorer();


builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Test01", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."

    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                          new OpenApiSecurityScheme
                          {
                              Reference = new OpenApiReference
                              {
                                  Type = ReferenceType.SecurityScheme,
                                  Id = "Bearer"
                              }
                          },
                         new string[] {}
                    }
                });
});


builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("connectDefault"));
});

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false; // Require at least one digit
    options.Password.RequireLowercase = true; // Require at least one lowercase letter
    options.Password.RequireUppercase = true; // Require at least one uppercase letter
    options.Password.RequireNonAlphanumeric = false; // Require at least one special character
    options.Password.RequiredLength = 6; // Minimum length of 6 characters
    options.Password.RequiredUniqueChars = 1;
})
  .AddEntityFrameworkStores<DataContext>()
    .AddDefaultTokenProviders();

/*builder.Services
    .AddIdentityApiEndpoints<IdentityUser>()
    .AddEntityFrameworkStores<DataContext>()
    .AddDefaultTokenProviders();  //12 require*/





/*builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
  .AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
})

.AddFacebook(options =>
{
    options.AppId = builder.Configuration["Facebook:AppId"]!;
    options.AppSecret = builder.Configuration["Facebook:AppSecret"]!;
    options.Scope.Add("email"); // Optional: request additional permissions
    options.SaveTokens = true;
});*/


// Configure Azure AD Authentication using JWT Bearer tokens
/*builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = $"{builder.Configuration["AzureAd:Instance"]}{builder.Configuration["AzureAd:TenantId"]}";
        options.Audience = builder.Configuration["AzureAd:Audience"];
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["AzureAd:Authority"],
            ValidAudience = builder.Configuration["AzureAd:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });*/


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

// Facebook Authentication
builder.Services.AddAuthentication()
    .AddFacebook(options =>
    {
        options.AppId = builder.Configuration["Facebook:AppId"]!;
        options.AppSecret = builder.Configuration["Facebook:AppSecret"]!;
        options.Scope.Add("email"); // Request additional permissions
        options.SaveTokens = true;
    });

///for microsft entra id



builder.Services.AddScoped<EmployeeRepository>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddHttpClient(); // Register IHttpClientFactory for making HTTP requests
builder.Services.AddCors(c => c.AddPolicy("default", builder => { builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); }));
// Add Facebook configuration settings
//builder.Services.Configure<FacebookSettings>(builder.Configuration.GetSection("Facebook"));


var app = builder.Build();

// Configure the HTTP request pipeline.
/*if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}*/


    app.UseSwagger();
    app.UseSwaggerUI();


/* here for azure*/
/*app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Test01 V1");
    c.RoutePrefix = string.Empty; // Makes Swagger UI accessible at the root URL
});*/

app.UseHttpsRedirection();

/*builder.Services.AddCors(options =>
{
    options.AddPolicy("default", policy =>
    {
        policy.AllowAnyOrigin()   // Allow any origin (you can restrict this to specific domains)
              .AllowAnyMethod()   // Allow any HTTP method (GET, POST, etc.)
              .AllowAnyHeader();  // Allow any headers
    });
});*/

//app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
//app.UseCors(policy =>
//    policy.WithOrigins("https://localhost:4200","http://localhost:4200", "https://employeeportal-c3fxh2ane2anh2gc.southeastasia-01.azurewebsites.net/")
//          .AllowAnyHeader()
//          .AllowAnyMethod()
//          .AllowCredentials());

app.UseCors("default");
app.UseAuthentication();
app.UseAuthorization();



app.MapControllers();
//To see the variuous api in swagger for that corrospondinf 12 require
/*app.MapIdentityApi<IdentityUser>();*/

app.Run();
