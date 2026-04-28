using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MusicStore.Models
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using var context = new MusicStoreContext(
                serviceProvider.GetRequiredService<DbContextOptions<MusicStoreContext>>());

            if (context.Genres.Any() || context.Artists.Any() || context.Albums.Any())
                return;

            var stockByCode = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
            {
                ["A02"] = 0, ["A07"] = 0, ["A10"] = 0, ["A11"] = 0, ["A14"] = 1, ["A16"] = 2,
                ["A18"] = 0, ["A19"] = 0, ["A20"] = 1, ["A21"] = 0, ["A24"] = 0, ["A25"] = 0,
                ["A26"] = 0, ["A27"] = 1, ["A28"] = 0, ["A29"] = 0, ["A30"] = 0, ["A31"] = 0,
                ["A32"] = 1, ["A33"] = 1, ["A35"] = 0, ["A37"] = 0, ["A38"] = 0, ["A39"] = 1,
                ["A41"] = 1, ["A42"] = 0, ["A43"] = 0, ["A44"] = 0, ["A48"] = 2, ["A49"] = 0,
                ["A53"] = 1, ["A54"] = 0, ["A55"] = 1, ["A56"] = 1, ["A58"] = 0, ["A60"] = 4,
                ["A61"] = 1, ["A62"] = 1, ["A63"] = 1, ["A64"] = 0, ["A65"] = 1, ["A66"] = 1,
                ["A67"] = 0, ["A68"] = 0, ["A69"] = 0, ["A70"] = 0, ["A71"] = 1, ["A72"] = 1,
                ["A73"] = 0, ["A74"] = 0, ["A75"] = 0, ["A76"] = 0, ["A77"] = 1, ["A78"] = 0,
                ["A79"] = 1, ["A80"] = 0, ["A81"] = 1, ["A82"] = 0
            };

            var products = new (string Code, string Name, string Brand, string ProductClass)[]
            {
                ("A00", "Blue Aguila", "Goorin Bros", "Ajustable"),
                ("A01", "Black Gallo", "Goorin Bros", "Ajustable"),
                ("A02", "Black Toro", "Goorin Bros", "Ajustable"),
                ("A03", "Purplish Pantera", "Goorin Bros", "Ajustable"),
                ("A04", "Yellow Orca", "Goorin Bros", "Ajustable"),
                ("A05", "Red Gallo", "Goorin Bros", "Ajustable"),
                ("A06", "Black / Golden Curva", "Nike", "Ajustable"),
                ("A07", "White Curva", "Under Armor", "Ajustable"),
                ("A08", "Black Curva", "Under Armor", "Ajustable"),
                ("A09", "Red Curva", "Under Armor", "Ajustable"),
                ("A10", "Purplish Curva", "Under Armor", "Ajustable"),
                ("A11", "Black / Curva", "NY", "Unitalla"),
                ("A12", "Blue Curva", "Ediko", "Ajustable"),
                ("A13", "Black Curva", "Tommy Hilfiger", "Ajustable"),
                ("A14", "Yellow Curva", "Levi's", "Ajustable"),
                ("A15", "Grey Curva", "Tommy Hilfiger", "Ajustable"),
                ("A16", "White Curva", "Bass Pro Shops", "Ajustable"),
                ("A17", "Grey Curva", "Polo", "Ajustable"),
                ("A18", "Blue Curva", "Polo", "Unitalla"),
                ("A19", "Black Curva", "Polo", "Unitalla"),
                ("A20", "Black Plana", "Jordan", "Ajustable"),
                ("A21", "Blue Plana", "Jordan", "Ajustable"),
                ("A22", "Black / Sky blue LA Curva", "New Era", "Ajustable"),
                ("A23", "Grey / black LA Curva", "New Era", "Ajustable"),
                ("A24", "Grey / black Yankees Curva", "New Era", "Ajustable"),
                ("A25", "Black / Grey Yankees Plana", "New Era", "Ajustable"),
                ("A26", "Black / Yellow Lakers Plana", "New Era", "Ajustable"),
                ("A27", "Black Miami Heats Plana", "New Era", "Ajustable"),
                ("A28", "White / Blue Miami Heats Plana", "New Era", "Ajustable"),
                ("A29", "Grey / Dark Green Celtics Plana", "New Era", "Ajustable"),
                ("A30", "Grey / Green Celtics Plana", "New Era", "Ajustable"),
                ("A31", "Black / Dark Green Celtics Plana", "New Era", "Ajustable"),
                ("A32", "Black / Orange Astros Plana", "New Era", "Ajustable"),
                ("A33", "White / Blue Astros Plana", "New Era", "Ajustable"),
                ("A34", "Black / Grey Boston Plana", "New Era", "Ajustable"),
                ("A35", "Black Red / Boston / Plana", "New Era", "Ajustable"),
                ("A36", "Grey / Blue Atlanta Plana", "New Era", "Ajustable"),
                ("A37", "Black / Grey San Diego Plana", "New Era", "Ajustable"),
                ("A38", "Black / Pirates Plana", "New Era", "Ajustable"),
                ("A39", "White / Black Pirates Plana", "New Era", "Ajustable"),
                ("A40", "Negra Vicera Verde Bulls", "New Era", "Ajustable"),
                ("A41", "Blue / Yellow LA Plana", "New Era", "Ajustable"),
                ("A42", "Black / Cock", "Goorin Bros", "Unitalla"),
                ("A43", "Blue / Cock", "Goorin Bros", "Unitalla"),
                ("A44", "Black / LA Bucket Hat", "LA", "Unitalla"),
                ("A45", "Rojo / Bulls", "New Era", "Ajustable"),
                ("A46", "Black Fusia / A's", "New Era", "Ajustable"),
                ("A47", "Black Pink / LA", "New Era", "Ajustable"),
                ("A48", "Fusia / Nike", "Nike", "Ajustable"),
                ("A49", "Nude / Nike", "Nike", "Ajustable"),
                ("A50", "White Fusia / Nike", "Nike", "Ajustable"),
                ("A51", "White Black / Puma", "Puma", "Ajustable"),
                ("A52", "Azul Vicera Roja Atlanta Braves", "New Era", "Ajustable"),
                ("A53", "Sky Blue Pink / LA / Plana", "New Era", "Ajustable"),
                ("A54", "Azul Letras Amarillo Golden State", "New Era", "Ajustable"),
                ("A55", "Blue Red / Boston / Plana", "New Era", "Ajustable"),
                ("A56", "Celeste", "Adidas", "Ajustable"),
                ("A57", "Negra Letras Rosa", "Adidas", "Ajustable"),
                ("A58", "Blanco Letras Negras", "Adidas", "Ajustable"),
                ("A59", "Rojo Letras Blanco", "Nike", "Ajustable"),
                ("A60", "Bizarrap", "New Era", "Ajustable"),
                ("A61", "Yellow / Yankees / Plana", "New Era", "Ajustable"),
                ("A62", "Red Dark Blue / Boston / Plana", "New Era", "Ajustable"),
                ("A63", "Beige / LA / Plana", "New Era", "Ajustable"),
                ("A64", "White Red / Bulls / Plana", "New Era", "Ajustable"),
                ("A65", "Black Yellow / LA / Plana", "New Era", "Ajustable"),
                ("A66", "Yellow / White Socks / Plana", "New Era", "Ajustable"),
                ("A67", "Sky Blue White / LA / Plana", "New Era", "Ajustable"),
                ("A68", "White Black / LA / Plana", "New Era", "Ajustable"),
                ("A69", "Black White / LA / Plana", "New Era", "Ajustable"),
                ("A70", "Gris Cafe / Yankees / Curva", "New Era", "Ajustable"),
                ("A71", "Rojo / Nike / Curva", "Nike", "Ajustable"),
                ("A72", "Azul / Nike / Curva", "Nike", "Ajustable"),
                ("A73", "Negro / Nike / Curva", "Nike", "Ajustable"),
                ("A74", "Azul Oscuro / Nike / Curva", "Nike", "Ajustable"),
                ("A75", "Verde Oscuro / Polo / Curva", "Polo", "Ajustable"),
                ("A76", "Gris / Polo / Curva", "Polo", "Ajustable"),
                ("A77", "Corinto Cafe / Freightliner / Curva", "Freightliner", "Ajustable"),
                ("A78", "Cafe Negro / LA / Plana", "New Era", "Ajustable"),
                ("A79", "Blanco Negro Rojo / Bulls / Plana", "New Era", "Ajustable"),
                ("A80", "Beige Cafe / Freightliner / Curva", "Freightliner", "Ajustable"),
                ("A81", "Negro / Bulls / Plana", "New Era", "Ajustable"),
                ("A82", "Cian / Polo / Curva", "Polo", "Ajustable")
            };

            var types = products
                .Select(product => DetermineType(product.Name, product.Brand))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(type => type)
                .Select(type => new Genre { Name = type })
                .ToList();

            var brands = products
                .Select(product => product.Brand.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(brand => brand)
                .Select(brand => new Artist { Name = brand })
                .ToList();

            context.Genres.AddRange(types);
            context.Artists.AddRange(brands);
            context.SaveChanges();

            var typeByName = context.Genres.ToDictionary(type => type.Name, StringComparer.OrdinalIgnoreCase);
            var brandByName = context.Artists.ToDictionary(brand => brand.Name, StringComparer.OrdinalIgnoreCase);

            var caps = products.Select(product => new Album
            {
                Code = product.Code,
                Title = product.Name,
                ProductClass = product.ProductClass,
                Stock = stockByCode.TryGetValue(product.Code, out var stock) ? stock : 0,
                Price = DeterminePrice(product.Brand),
                GenreId = typeByName[DetermineType(product.Name, product.Brand)].Id,
                ArtistId = brandByName[product.Brand.Trim()].Id
            });

            context.Albums.AddRange(caps);
            context.SaveChanges();
        }

        private static string DetermineType(string name, string brand)
        {
            var normalized = name.ToLowerInvariant();
            if (normalized.Contains("bucket hat")) return "Bucket Hat";
            if (normalized.Contains("plana")) return "Plana";
            if (normalized.Contains("curva") || normalized.Contains("cruva")) return "Curva";
            if (brand.Equals("Goorin Bros", StringComparison.OrdinalIgnoreCase)) return "Trucker";
            return "Otros";
        }

        private static decimal DeterminePrice(string brand)
        {
            return brand.Trim() switch
            {
                "New Era" => 240m,
                "Goorin Bros" => 260m,
                "Jordan" => 220m,
                "Nike" => 210m,
                "Polo" => 190m,
                "Adidas" => 190m,
                _ => 180m
            };
        }
    }
}
