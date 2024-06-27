using ERP.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ERP.Api;

internal static class ProductApi
{
    public static RouteGroupBuilder MapProductApi(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/erp")
            .WithTags("Product Api");


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

        group.MapGet("/products", async (AppDbContext db) =>
        {
            var products = await db.Products.Include(p => p.Category).ToListAsync();
            return Results.Json(products);
        })
        .WithOpenApi();


        group.MapPost("/products", async (AppDbContext db, Product newProduct) =>
        {
            var category = await db.Categories.FirstOrDefaultAsync(c => c.CategoryId == newProduct.CategoryId);
            if (category == null)
            {
                return Results.BadRequest("Invalid category ID.");
            }

            newProduct.Created = DateTime.Now;

            db.Products.Add(newProduct);
            await db.SaveChangesAsync();
            return Results.Created($"/products/{newProduct.ProductId}", newProduct);
        })
        .WithOpenApi();


        group.MapPut("/products/{id:int}", async (AppDbContext db, int id, Product updatedProduct) =>
        {
            var product = await db.Products.FindAsync(id);
            if (product == null)
            {
                return Results.NotFound();
            }

            var category = await db.Categories.FirstOrDefaultAsync(c => c.CategoryId == updatedProduct.CategoryId);
            if (category == null)
            {
                return Results.BadRequest("Invalid category ID.");
            }

            product.SkuNumber = updatedProduct.SkuNumber;
            product.CategoryId = updatedProduct.CategoryId;
            product.RecommendationId = updatedProduct.RecommendationId;
            product.Title = updatedProduct.Title;
            product.Price = updatedProduct.Price;
            product.SalePrice = updatedProduct.SalePrice;
            product.ProductArtUrl = updatedProduct.ProductArtUrl;
            product.Description = updatedProduct.Description;
            product.ProductDetails = updatedProduct.ProductDetails;
            product.Inventory = updatedProduct.Inventory;
            product.LeadTime = updatedProduct.LeadTime;

            await db.SaveChangesAsync();
            return Results.NoContent();
        })
        .WithOpenApi();


        group.MapDelete("/products/{id:int}", async (AppDbContext db, int id) =>
        {
            var product = await db.Products.FindAsync(id);
            if (product == null)
            {
                return Results.NotFound();
            }

            db.Products.Remove(product);
            await db.SaveChangesAsync();
            return Results.NoContent();
        })
        .WithOpenApi();

        return group;
    }
}
