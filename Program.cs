using System.Data;
using System.IO.Compression;
using chairs_dotnet7_api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<DataContext>(opt => opt.UseInMemoryDatabase("chairlist"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var app = builder.Build();

var chairs = app.MapGroup("api/chair");

//TODO: ASIGNACION DE RUTAS A LOS ENDPOINTS
chairs.MapPost("/", AddChair);
chairs.MapGet("/", GetChairs);
chairs.MapGet("/{name}", GetChairByName);
chairs.MapPut("/{id}", UpdateChair);
chairs.MapPut("/{id}/{stock}", IncrementStock);
chairs.MapPost("/purchase", MakePurchase);
chairs.MapDelete("/{id}", DeleteChair);



app.Run();



//TODO: ENDPOINTS SOLICITADOS
static IResult AddChair(DataContext db, [FromBody] Chair chair){
    if(db.Chairs.FirstOrDefault(c => c.Nombre == chair.Nombre) != null){
        return TypedResults.Problem();
    }
    var chairr = new Chair {Nombre = chair.Nombre, Tipo = chair.Tipo, Material = chair.Material, Color = chair.Color, Altura = chair.Altura, Anchura = chair.Anchura, Profundidad = chair.Profundidad, Precio = chair.Precio, Stock = chair.Stock};
    db.Add(chairr);
    db.SaveChanges();
    return TypedResults.Created($"/{chair.Id}", chair);
}
static IResult GetChairs(DataContext db)
{
    return TypedResults.Ok(db.Chairs.ToList());
}

static IResult GetChairByName (DataContext db, string name){

    var chair = db.Chairs.FirstOrDefault(c => c.Nombre == name);
    if(chair != null){
        return TypedResults.Ok(chair);
    }
    return TypedResults.NotFound();
}

static IResult UpdateChair (DataContext db, int id, [FromBody] Chair chairs){

    var chair = db.Chairs.Find(id);
    if(chair != null){
        chair.Nombre = chairs.Nombre  ?? chair.Nombre;
        chair.Material = chairs.Material ?? chair.Material;
        chair.Tipo = chairs.Tipo ?? chair.Tipo;
        chair.Color = chairs.Color ?? chair.Color;
        if(chairs.Altura != null){
            chair.Altura = chairs.Altura;
        } 
        if(chairs.Anchura != null){
            chair.Anchura = chairs.Anchura;
        }
        if(chairs.Profundidad != null){
             chair.Profundidad = chairs.Profundidad;
        }
        if(chairs.Precio != null){
            chair.Precio = chairs.Precio;
        }
        db.SaveChanges();
        return TypedResults.NoContent();
    }
    return TypedResults.NotFound();
}

static IResult IncrementStock(DataContext db, int id, int stock){

    var chair = db.Chairs.Find(id);

    if(chair != null){
        chair.Stock += stock;
        db.SaveChanges();
        return TypedResults.Ok(chair);
    }
    
    return TypedResults.NotFound();
}

static IResult MakePurchase(DataContext db, [FromBody] ChairDTO chairDTO){

    var chair = db.Chairs.Find(chairDTO.Id);

    if(chair != null){
        if(chair.Stock > chairDTO.Cantidad){
            int realPrice = (int)(chairDTO.Cantidad * chair.Precio);
            if(realPrice == chairDTO.Pago){
                chair.Stock -= chairDTO.Cantidad;
                db.SaveChanges();
                return TypedResults.Ok("Venta realizada!");
            }
            return TypedResults.BadRequest("Error en los datos proporcionados");
        }
        return TypedResults.BadRequest("Error en los datos proporcionados");
    }
    return TypedResults.NotFound();
}

static IResult DeleteChair(DataContext db, int id){
    var chair = db.Chairs.Find(id);

    if(chair != null){
        db.Chairs.Remove(chair);
        db.SaveChanges();
        return TypedResults.NoContent();
    }
    return TypedResults.NotFound();
}

