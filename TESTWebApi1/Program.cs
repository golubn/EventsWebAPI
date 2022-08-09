using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TESTWebApi1.Context;
using TESTWebApi1.Models;


var builder = WebApplication.CreateBuilder(args);
string connection = "Server=(localdb)\\mssqllocaldb;Database=applicationdb;Trusted_Connection=True;"; 
builder.Services.AddDbContext<EventsContext>(options => options.UseSqlServer(connection));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = AuthOptions.ISSUER,
            ValidateAudience = true,
            ValidAudience = AuthOptions.AUDIENCE,
            ValidateLifetime = true,
            IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
            ValidateIssuerSigningKey = true,
        };
    }); 

var app = builder.Build();
app.UseDefaultFiles();
app.UseStaticFiles();
app.Map("/api/tokens/{login}", async (string login, EventsContext db) =>
{
    RegisterModel? user = await db.RegisterModel.FirstOrDefaultAsync(u => u.Login == login);
    if (user == null) return Results.NotFound(new { message = "Event not found" });
    var claims = new List<Claim> { new Claim(ClaimTypes.Name, user.Login) };
    var jwt = new JwtSecurityToken(
            issuer: AuthOptions.ISSUER,
            audience: AuthOptions.AUDIENCE,
            claims: claims,
            expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(2)),
            signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
    var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
    var response = new
    {
        access_token = encodedJwt
    };

    return Results.Json(response);
});
app.Map("/data", [Authorize] () => new { message = "Hello!" });
app.MapGet("/api/tokens", async (EventsContext db) => await db.RegisterModel.ToListAsync());
app.MapGet("/api/events", async (EventsContext db) => await db.Events.ToListAsync());


app.MapGet("/api/events/{id:int}", async (int id, EventsContext db) =>
{
    Events? event_use = await db.Events.FirstOrDefaultAsync(u => u.Id == id);
    if (event_use == null) return Results.NotFound(new { message = "Event not found" });
    return Results.Json(event_use);
});

app.MapDelete("/api/events/{id:int}", async (int id, EventsContext db) =>
{
    Events? event_use = await db.Events.FirstOrDefaultAsync(u => u.Id == id);
    if (event_use == null) return Results.NotFound(new { message = "Event not found" });
    db.Events.Remove(event_use);
    await db.SaveChangesAsync();
    return Results.Json(event_use);
});

app.MapPost("/api/events", async (Events event_use, EventsContext db) =>
{
    await db.Events.AddAsync(event_use);
    await db.SaveChangesAsync();
    return event_use;
});
app.MapPost("/api/tokens", async (RegisterModel user, EventsContext db) =>
{
    try
    {
        await db.RegisterModel.AddAsync(user);
        await db.SaveChangesAsync();
        return user;
    } catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
        return null;
    }
});
app.MapPut("/api/tokens", async (RegisterModel user, EventsContext db) =>
{
    var event_use = await db.RegisterModel.FirstOrDefaultAsync(u => u.Id == user.Id);
    if (event_use == null) return Results.NotFound(new { message = "Event not found" });
    event_use.Login = user.Login;
    event_use.Password = user.Password;
    event_use.Token = "testfromprogram";
    await db.SaveChangesAsync();
    return Results.Json(event_use);
});

app.MapPut("/api/events", async (Events eventData, EventsContext db) =>
{
    var event_use = await db.Events.FirstOrDefaultAsync(u => u.Id == eventData.Id);
    if (event_use == null) return Results.NotFound(new { message = "Event not found" });
    event_use.Name = eventData.Name;
    event_use.Description = eventData.Description;
 
    event_use.Location = eventData.Location;
    event_use.DateTime = eventData.DateTime;
    event_use.Speaker = eventData.Speaker;
    await db.SaveChangesAsync();
    return Results.Json(event_use);
});

app.Run();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


app.UseEndpoints(endpoints =>
{
    endpoints.MapDefaultControllerRoute();
});
public class AuthOptions
{
    public const string ISSUER = "MyAuthServer";
    public const string AUDIENCE = "MyAuthClient";
    const string KEY = "mysupersecret_secretkey!123";
    public static SymmetricSecurityKey GetSymmetricSecurityKey() =>
        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
}