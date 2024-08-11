//using API.
using API.MailSettingsService;
using API.Settings;
using Core.Entities.IdentitiyEntities;
using Core.Interfaces.Service;
using ecommerce_backend.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Repository.Identity;
using Service.AuthServices;
using System.Text;
var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers(options=>options.Filters.Add<LogActivityFilter>());//adding this filter to all the actions 
//it is a global action filter that will be called after each request 

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<IdentityContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("identityContext")));
///s*******************
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    //configurations of password
  /*  options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;
    options.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultProvider;*/

}).AddEntityFrameworkStores<IdentityContext>().AddDefaultTokenProviders();

builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));//read the mail settings 
builder.Services.AddScoped<IEmailSettings,EmailSettings>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; // We use it for to be don't have to let every end point what is the shema because it will make every end point work on bearer schema
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:ValidAudience"],
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:SecretKey"])),
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromMinutes(double.Parse(builder.Configuration["JWT:DurationInMinutes"])),
    };
});

builder.Services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
    //Client.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
    app.UseDeveloperExceptionPage();//make an exception page friendly
}
app.UseStatusCodePagesWithReExecute("/errors/{0}"); //middleware to redirect to the error page with the status code if not found an api to redirect to it

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
