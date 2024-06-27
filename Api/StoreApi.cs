using AutoMapper;
using ERP.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using MinimalAPIERP.Dtos;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ERP.Api;

internal static class StoreApi
{
    public static RouteGroupBuilder MapStoreApi(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/erp")
            .WithTags("Store Api");

        // TODO: Mover a config
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            //PropertyNameCaseInsensitive = false,
            //PropertyNamingPolicy = null,
            WriteIndented = true,
            //IncludeFields = false,
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            //ReferenceHandler = ReferenceHandler.Preserve
        };

        group.MapGet("/user", (ClaimsPrincipal user) =>
        {
            return user.Identity;

        })
        .WithOpenApi();

        group.MapGet("/store/{storeid}", async Task<Results<Ok<StoreDto>, NotFound>> (int storeid, AppDbContext db, IMapper mapper) =>
        {
            return mapper.Map<StoreDto>(await db.Stores.FirstOrDefaultAsync(m => m.StoreId == storeid))
                is StoreDto store
                    ? TypedResults.Ok(store)
                    : TypedResults.NotFound();
        })
        .WithOpenApi();

        group.MapPost("/store", async (AppDbContext db, Store newStore) =>
        {
            db.Stores.Add(newStore);
            await db.SaveChangesAsync();
            return Results.Created($"/store/{newStore.StoreId}", newStore);
        })
        .WithOpenApi();

        group.MapPut("/store/{storeid}", async (int storeid, AppDbContext db, Store updatedStore) =>
        {
            var store = await db.Stores.FindAsync(storeid);
            if (store == null)
            {
                return Results.NotFound();
            }

            store.Name = updatedStore.Name;

            await db.SaveChangesAsync();
            return Results.NoContent();
        })
        .WithOpenApi();

        group.MapDelete("/store/{storeid}", async (int storeid, AppDbContext db) =>
        {
            var store = await db.Stores.FindAsync(storeid);
            if (store == null)
            {
                return Results.NotFound();
            }

            db.Stores.Remove(store);
            await db.SaveChangesAsync();
            return Results.NoContent();
        })
        .WithOpenApi();


        group.MapGet("/storea", async Task<Results<Ok<IList<Store>>, NotFound>> (AppDbContext db) =>
            await db.Stores.ToListAsync()
                is IList<Store> stores
                    ? TypedResults.Ok(stores)
                    : TypedResults.NotFound())
            .WithOpenApi();

        group.MapPost("/storea", async (AppDbContext db, Store newStore) =>
        {
            db.Stores.Add(newStore);
            await db.SaveChangesAsync();
            return Results.Created($"/storea/{newStore.StoreId}", newStore);
        })
        .WithOpenApi();

        group.MapPut("/storea", async (AppDbContext db, Store updatedStore) =>
        {
            var store = await db.Stores.FindAsync(updatedStore.StoreId);
            if (store == null)
            {
                return Results.NotFound();
            }

            store.Name = updatedStore.Name;

            await db.SaveChangesAsync();
            return Results.NoContent();
        })
       .WithOpenApi();

        group.MapDelete("/storea/{storeId:int}", async (AppDbContext db, int storeId) =>
        {
            var store = await db.Stores.FindAsync(storeId);
            if (store == null)
            {
                return Results.NotFound();
            }

            db.Stores.Remove(store);
            await db.SaveChangesAsync();
            return Results.NoContent();
        })
        .WithOpenApi();

        group.MapGet("/storeb", async Task<Results<Ok<IList<Store>>, NotFound>> (AppDbContext db, int pageSize = 10, int page = 0) =>
            await db.Stores.Skip(page * pageSize).Take(pageSize).ToListAsync()
                is IList<Store> stores
                    ? TypedResults.Ok(stores)
                    : TypedResults.NotFound())
            .WithOpenApi();

        group.MapPost("/storeb", async (AppDbContext db, Store newStore) =>
        {
            db.Stores.Add(newStore);
            await db.SaveChangesAsync();
            return Results.Created($"/storeb/{newStore.StoreId}", newStore);
        })
       .WithOpenApi();

        group.MapPut("/storeb", async (AppDbContext db, Store updatedStore) =>
        {
            var store = await db.Stores.FindAsync(updatedStore.StoreId);
            if (store == null)
            {
                return Results.NotFound();
            }

            store.Name = updatedStore.Name;

            await db.SaveChangesAsync();
            return Results.NoContent();
        })
        .WithOpenApi();

        group.MapDelete("/storeb/{storeId:int}", async (AppDbContext db, int storeId) =>
        {
            var store = await db.Stores.FindAsync(storeId);
            if (store == null)
            {
                return Results.NotFound();
            }

            db.Stores.Remove(store);
            await db.SaveChangesAsync();
            return Results.NoContent();
        })
        .WithOpenApi();

        group.MapGet("/storec1", async Task<Results<Ok<IList<Store>>, NotFound>> (AppDbContext db, int pageSize = 10, int page = 0) =>
            await db.Stores
            .Skip(page * pageSize)
            .Take(pageSize)
            .Select(store => new { store.StoreId, store.Name })
            .ToListAsync()
                is IList<Store> stores
                    ? TypedResults.Ok(stores)
                    : TypedResults.NotFound())
            .WithOpenApi();

        group.MapPost("/storec1", async (AppDbContext db, Store newStore) =>
        {
            db.Stores.Add(newStore);
            await db.SaveChangesAsync();
            return Results.Created($"/storec1/{newStore.StoreId}", newStore);
        })
        .WithOpenApi();

        group.MapPut("/storec1", async (AppDbContext db, Store updatedStore) =>
        {
            var store = await db.Stores.FindAsync(updatedStore.StoreId);
            if (store == null)
            {
                return Results.NotFound();
            }

            store.Name = updatedStore.Name;

            await db.SaveChangesAsync();
            return Results.NoContent();
        })
       .WithOpenApi();

        group.MapDelete("/storec1/{storeId:int}", async (AppDbContext db, int storeId) =>
        {
            var store = await db.Stores.FindAsync(storeId);
            if (store == null)
            {
                return Results.NotFound();
            }

            db.Stores.Remove(store);
            await db.SaveChangesAsync();
            return Results.NoContent();
        })
        .WithOpenApi();

        group.MapGet("/storec2", async (AppDbContext db, int pageSize = 10, int page = 0) =>
        {
            var data = await db.Stores
                .Skip(page * pageSize)
                .Take(pageSize)
                .Include(s => s.Rainchecks)
                .Select(store => new { store.StoreId, store.Name })
                .ToListAsync();

            return data.Any()
                ? Results.Ok(data)
                : Results.NotFound();
        })
        .WithOpenApi();

        group.MapPost("/storec2", async (AppDbContext db, Store newStore) =>
        {
            db.Stores.Add(newStore);
            await db.SaveChangesAsync();
            return Results.Created($"/storec2/{newStore.StoreId}", newStore);
        })
        .WithOpenApi();

        group.MapPut("/storec2", async (AppDbContext db, Store updatedStore) =>
        {
            var store = await db.Stores.FindAsync(updatedStore.StoreId);
            if (store == null)
            {
                return Results.NotFound();
            }

            store.Name = updatedStore.Name;

            await db.SaveChangesAsync();
            return Results.NoContent();
        })
        .WithOpenApi();

        group.MapDelete("/storec2/{storeId:int}", async (AppDbContext db, int storeId) =>
        {
            var store = await db.Stores.FindAsync(storeId);
            if (store == null)
            {
                return Results.NotFound();
            }

            db.Stores.Remove(store);
            await db.SaveChangesAsync();
            return Results.NoContent();
        })
        .WithOpenApi();

        group.MapGet("/stored", async Task<Results<Ok<IList<Store>>, NotFound>> (AppDbContext db) =>
            await db.Stores.Include(s => s.Rainchecks).ToListAsync()
                is IList<Store> stores
                    ? TypedResults.Ok(stores)
                    : TypedResults.NotFound())
            .WithOpenApi();

        group.MapPost("/stored", async (AppDbContext db, Store newStore) =>
        {
            db.Stores.Add(newStore);
            await db.SaveChangesAsync();
            return Results.Created($"/stored/{newStore.StoreId}", newStore);
        })
        .WithOpenApi();

        group.MapPut("/stored", async (AppDbContext db, Store updatedStore) =>
        {
            var store = await db.Stores.FindAsync(updatedStore.StoreId);
            if (store == null)
            {
                return Results.NotFound();
            }

            store.Name = updatedStore.Name;

            await db.SaveChangesAsync();
            return Results.NoContent();
        })
       .WithOpenApi();

        group.MapDelete("/stored/{storeId:int}", async (AppDbContext db, int storeId) =>
        {
            var store = await db.Stores.FindAsync(storeId);
            if (store == null)
            {
                return Results.NotFound();
            }

            db.Stores.Remove(store);
            await db.SaveChangesAsync();
            return Results.NoContent();
        })
        .WithOpenApi();

        group.MapGet("/storee", async (AppDbContext db) =>
            await db.Stores.Include(s => s.Rainchecks).ToListAsync()
                is IList<Store> stores
                    ? Results.Json(stores, options)
                    : Results.NotFound())
            .WithOpenApi();

        group.MapPost("/storee", async (AppDbContext db, Store newStore) =>
        {
            db.Stores.Add(newStore);
            await db.SaveChangesAsync();
            return Results.Created($"/storee/{newStore.StoreId}", newStore);
        })
       .WithOpenApi();

        group.MapPut("/storee", async (AppDbContext db, Store updatedStore) =>
        {
            var store = await db.Stores.FindAsync(updatedStore.StoreId);
            if (store == null)
            {
                return Results.NotFound();
            }

            store.Name = updatedStore.Name;

            await db.SaveChangesAsync();
            return Results.NoContent();
        })
       .WithOpenApi();

        group.MapDelete("/storee/{storeId:int}", async (AppDbContext db, int storeId) =>
        {
            var store = await db.Stores.FindAsync(storeId);
            if (store == null)
            {
                return Results.NotFound();
            }

            db.Stores.Remove(store);
            await db.SaveChangesAsync();
            return Results.NoContent();
        })
        .WithOpenApi();

        group.MapGet("/storef", async (AppDbContext db) =>
            await db.Stores.Include(s => s.Rainchecks).ToListAsync()
                is IList<Store> stores
                    ? Results.Json(stores, options)
                    : Results.NotFound())
            .WithOpenApi();

        group.MapPost("/storef", async (AppDbContext db, Store newStore) =>
        {
            db.Stores.Add(newStore);
            await db.SaveChangesAsync();
            return Results.Created($"/storef/{newStore.StoreId}", newStore);
        })
        .WithOpenApi();

        group.MapPut("/storef", async (AppDbContext db, Store updatedStore) =>
        {
            var store = await db.Stores.FindAsync(updatedStore.StoreId);
            if (store == null)
            {
                return Results.NotFound();
            }

            store.Name = updatedStore.Name;

            await db.SaveChangesAsync();
            return Results.NoContent();
        })
       .WithOpenApi();

        group.MapDelete("/storef/{storeId:int}", async (AppDbContext db, int storeId) =>
        {
            var store = await db.Stores.FindAsync(storeId);
            if (store == null)
            {
                return Results.NotFound();
            }

            db.Stores.Remove(store);
            await db.SaveChangesAsync();
            return Results.NoContent();
        })
        .WithOpenApi();

        return group;
    }
}
