using Microsoft.EntityFrameworkCore;
using MiniValidation;
using UpxApi.Data;
using UpxApi.Models;
using NetDevPack.Identity;
using NetDevPack.Identity.Jwt;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using NetDevPack.Identity.Model;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);


#region config

var AllowAll = "_allowAll";

builder.Services.AddDbContext<ContextDb>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentityEntityFrameworkContextConfiguration(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
    b => b.MigrationsAssembly("UpxApi")));

builder.Services.AddIdentityConfiguration();
builder.Services.AddJwtConfiguration(builder.Configuration, "AppSettings");

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ChangeStudentSpot",
        policy => policy.RequireClaim("ChangeStudentSpot"));
});

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: AllowAll, builder =>
    {
        builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "UpxApi",
        Version = "v1",
        Description = "Developed by Matheus Alexander - Student at Facens",
        Contact = new OpenApiContact
        {
            Name = "Matheus Alexander",
            Email = "matheusalexalcantara@gmail.com"
        },
        License = new OpenApiLicense
        {
            Name = "Use under MIT",
            Url = new Uri("https://opensource.org/licenses/MIT")
        }
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Enter the token like this: Bearer {your token}",
        Name = "Authorization",
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey
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
            new string[] { }
        }
    });
});

var app = builder.Build();
app.UseCors(AllowAll);


if (app.Environment.IsDevelopment())
{
    
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthConfiguration();
app.UseHttpsRedirection();


#endregion

RegionMapActions(app);
SpotMapActions(app);
StudentMapActions(app);
UserMapActions(app);

app.Run();

#region regionActions
void RegionMapActions(WebApplication app)
{
    app.MapGet("/region", async
    (ContextDb context) =>
     await context.Regions.ToListAsync())
    .WithName("GetAllRegions")
    .WithTags("Regions");

    app.MapPost("/region", async (
        ContextDb context,
        Region region) =>
    {
        if (!MiniValidator.TryValidate(region, out var errors))
            return Results.ValidationProblem(errors);

        context.Regions.Add(region);

        var result = await context.SaveChangesAsync();

        return result > 0
                ? Results.Created($"/region/{region.RegionId}", region)
                : Results.BadRequest("There was a problem saving the record");
    })
    .ProducesValidationProblem()
    .Produces<Spot>(StatusCodes.Status201Created)
    .Produces(StatusCodes.Status400BadRequest)
    .WithName("PostRegion")
    .WithTags("Regions");

    app.MapPut("/region/{id}", async (
         int id,
         ContextDb context,
         Region region) =>
    {
        if (!MiniValidator.TryValidate(region, out var errors))
            return Results.ValidationProblem(errors);

        var notModifiedRegion = await context.Regions.AsNoTracking<Region>().FirstOrDefaultAsync(f => f.RegionId == id);
        if (notModifiedRegion == null)
            return Results.NotFound();

        context.Regions.Update(region);
        var result = await context.SaveChangesAsync();

        return result > 0
            ? Results.NoContent()
            : Results.BadRequest("There was a problem saving the record");
    })
    .ProducesValidationProblem()
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status400BadRequest)
    .WithName("PutRegion")
    .WithTags("Regions");

    app.MapDelete("/region/{id}", async (
        int id,
        ContextDb context) =>
    {
        var region = await context.Regions.FindAsync(id);
        if (region == null)
            return Results.NotFound();

        context.Regions.Remove(region);
        var result = await context.SaveChangesAsync();

        return result > 0
            ? Results.NoContent()
            : Results.BadRequest("There was a problem saving the record");
    })
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status404NotFound)
    .WithName("DeleteRegion")
    .WithTags("Regions");
}
#endregion

#region spotActions
void SpotMapActions(WebApplication app)
{
    app.MapGet("/spot", async
    (ContextDb context) =>
     await context.Spots.ToListAsync())
    .WithName("GetAllSpots")
    .WithTags("Spots");

    app.MapGet("/spot/{id}", async
        (int id,
         ContextDb context) =>
        await context.Spots.FindAsync(id)
            is Spot spot
                ? Results.Ok(spot)
                : Results.NotFound())
        .Produces<Spot>(StatusCodes.Status200OK)
        .Produces<Spot>(StatusCodes.Status404NotFound)
        .WithName("GetSpotById")
        .WithTags("Spots");

    app.MapPost("/spot", async
        (ContextDb context,
         Spot spot) =>
    {
        if (!MiniValidator.TryValidate(spot, out var errors))
            return Results.ValidationProblem(errors);

        context.Spots.Add(spot);

        var result = await context.SaveChangesAsync();

        return result > 0
                ? Results.Created($"/spot/{spot.SpotId}", spot)
                : Results.BadRequest("There was a problem saving the record");
    })
    .ProducesValidationProblem()
    .Produces<Spot>(StatusCodes.Status201Created)
    .Produces(StatusCodes.Status400BadRequest)
    .WithName("PostSpot")
    .WithTags("Spots");

    app.MapPut("/spot/{id}", async (
         int id,
         ContextDb context,
         Spot spot) =>
    {
        if (!MiniValidator.TryValidate(spot, out var errors))
            return Results.ValidationProblem(errors);

        var notModifiedSpot = await context.Spots.AsNoTracking<Spot>().FirstOrDefaultAsync(f => f.SpotId == id);
        if (notModifiedSpot == null)
            return Results.NotFound();

        context.Spots.Update(spot);
        var result = await context.SaveChangesAsync();

        return result > 0
            ? Results.NoContent()
            : Results.BadRequest("There was a problem saving the record");
    })
    .ProducesValidationProblem()
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status400BadRequest)
    .WithName("PutSpot")
    .WithTags("Spots");

    app.MapDelete("/spot/{id}", async (
        int id,
        ContextDb context) =>
    {
        var spot = await context.Spots.FindAsync(id);
        if (spot == null)
            return Results.NotFound();

        context.Spots.Remove(spot);
        var result = await context.SaveChangesAsync();

        return result > 0
            ? Results.NoContent()
            : Results.BadRequest("There was a problem saving the record");
    })
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status404NotFound)
    .WithName("DeleteSpot")
    .WithTags("Spots");



    app.MapGet("/spotregion/{region}", async (
        string region, ContextDb context) =>
        await context.Spots.Where(p => p.Region == region).ToListAsync()
            is List<Spot> spot
                ? (spot.Count > 0
                    ? Results.Ok(spot)
                    : Results.NotFound())
                : Results.NotFound())
    .Produces<Spot>(StatusCodes.Status200OK)
    .Produces<Spot>(StatusCodes.Status404NotFound)
    .WithName("GetSpotByRegion")
    .WithTags("SpotRegion");
}
#endregion

#region studentActions
void StudentMapActions(WebApplication app)
{
    app.MapGet("/student", [AllowAnonymous] async
      (ContextDb context) =>
       await context.Students.ToListAsync())
      .WithName("GetAllStudents")
      .WithTags("Students");

    app.MapPost("/student", [Authorize] async (ContextDb context,
         Student student) =>
    {
        if (!MiniValidator.TryValidate(student, out var errors))
            return Results.ValidationProblem(errors);

        context.Students.Add(student);

        var result = await context.SaveChangesAsync();

        return result > 0
                ? Results.Created($"/student/{student.Id}", student)
                : Results.BadRequest("There was a problem saving the record");
    })
    .ProducesValidationProblem()
    .Produces<Spot>(StatusCodes.Status201Created)
    .Produces(StatusCodes.Status400BadRequest)
    .WithName("PostStudent")
    .WithTags("Students");

    app.MapGet("/student/{id}", [Authorize] async (
        int id,
        ContextDb context) =>
        await context.Students.FindAsync(id)
            is Student student
                ? Results.Ok(student)
                : Results.NotFound())
    .Produces<Spot>(StatusCodes.Status200OK)
    .Produces<Spot>(StatusCodes.Status404NotFound)
    .WithName("GetStudentById")
    .WithTags("Students");

    app.MapGet("/student/{id}/spot", [Authorize] async (
        int id,
        ContextDb context) =>
        await context.Students.FindAsync(id)
            is Student student
                ? Results.Ok(context.Spots.FindAsync(student.SpotId))
                : Results.NotFound())
    .Produces<Spot>(StatusCodes.Status200OK)
    .Produces<Spot>(StatusCodes.Status404NotFound)
    .WithName("GetStudentSpotById")
    .WithTags("Students");

    app.MapPut("/student/{id}", [Authorize] async (
         UserManager<IdentityUser> userManager,
         int id,
         ContextDb context,
         Student student) =>
    {
        var user = await userManager.FindByIdAsync(id.ToString());
        if (user == null)
            return Results.NotFound();

        if (user.Id != id.ToString())
            return Results.BadRequest("The user id does not match the student id");

        if (!MiniValidator.TryValidate(student, out var errors))
            return Results.ValidationProblem(errors);

        var notModifiedStudent = await context.Students.AsNoTracking<Student>().FirstOrDefaultAsync(f => f.Id == id);
        if (notModifiedStudent == null)
            return Results.NotFound();

        if (notModifiedStudent.SpotId != student.SpotId)
        {
            var oldSpot = await context.Spots.FindAsync(notModifiedStudent.SpotId);
            if (oldSpot != null)
            {
                oldSpot.Occupied = false;
                context.Spots.Update(oldSpot);
            }

            var newSpot = await context.Spots.FindAsync(student.SpotId);
            if (newSpot != null)
            {
                newSpot.Occupied = true;
                context.Spots.Update(newSpot);
            }
        }

        context.Students.Update(student);
        var result = await context.SaveChangesAsync();

        return result > 0
            ? Results.NoContent()
            : Results.BadRequest("There was a problem saving the record");
    })
    .ProducesValidationProblem()
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status400BadRequest)
    .WithName("PutStudent")
    .WithTags("Students");

    app.MapDelete("/student/{id}", [Authorize] async (
        int id,
        ContextDb context) =>
    {
        var student = await context.Students.FindAsync(id);
        if (student == null)
            return Results.NotFound();

        var spot = await context.Spots.FindAsync(student.SpotId);
        if (spot != null)
        {
            spot.Occupied = false;
            context.Spots.Update(spot);
        }

        context.Students.Remove(student);
        var result = await context.SaveChangesAsync();

        return result > 0
            ? Results.NoContent()
            : Results.BadRequest("There was a problem saving the record");
    })
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status404NotFound)
    .WithName("DeleteStudent")
    .WithTags("Students");
}
#endregion

#region UserActions
void UserMapActions(WebApplication app)
{
    app.MapPost("/user/register", [AllowAnonymous] async (
        ContextDb context,
        SignInManager<IdentityUser> signInManager,
        UserManager<IdentityUser> userManager,
        IOptions<AppJwtSettings> appJwtSettings,
        Student student) =>
    {
        if (student == null)
            return Results.BadRequest("Invalid client request");

        if (!MiniValidator.TryValidate(student, out var errors))
            return Results.ValidationProblem(errors);



        context.Students.Add(student);
        var result2 = await context.SaveChangesAsync();
        var user = new IdentityUser
        {
            Id = context.Students.ToList().Last().Id.ToString(),
            UserName = student.Email,
            Email = student.Email,
            EmailConfirmed = true
        };
        var result = await userManager.CreateAsync(user, student.Password);

        if (!result.Succeeded)
            return Results.BadRequest(result.Errors);

        if (result2 <= 0)
            return Results.BadRequest("There was a problem saving the record");


        var jwt = new JwtBuilder()
            .WithUserManager(userManager)
            .WithJwtSettings(appJwtSettings.Value)
            .WithEmail(user.Email)
            .WithJwtClaims()
            .WithUserClaims()
            .WithUserRoles()
            .BuildUserResponse();

        return Results.Ok(jwt);
    })
    .ProducesValidationProblem()
    .Produces(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status400BadRequest)
    .WithName("RegisterUser")
    .WithTags("User");

    app.MapPost("/user/login", [AllowAnonymous] async (
        SignInManager<IdentityUser> signInManager,
        UserManager<IdentityUser> userManager,
        IOptions<AppJwtSettings> appJwtSettings,
        LoginUser login) =>
    {
        if (login == null)
            return Results.BadRequest("Invalid client request");

        if (!MiniValidator.TryValidate(login, out var errors))
            return Results.ValidationProblem(errors);

        var result = await signInManager.PasswordSignInAsync(login.Email, login.Password, false, false);

        if (result.IsLockedOut)
            return Results.BadRequest("User locked out");

        if (!result.Succeeded)
            return Results.BadRequest("User or password invalid");

        var user = await userManager.FindByEmailAsync(login.Email);

        var jwt = new JwtBuilder()
            .WithUserManager(userManager)
            .WithJwtSettings(appJwtSettings.Value)
            .WithEmail(user.Email)
            .WithJwtClaims()
            .WithUserClaims()
            .WithUserRoles()
            .BuildUserResponse();

        return Results.Ok(jwt);
    })
    .ProducesValidationProblem()
    .Produces(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status400BadRequest)
    .WithName("LoginUser")
    .WithTags("User");
    
}
#endregion