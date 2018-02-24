using CucaAPI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CucaAPI
{
    public class DbInitializer
    {
        public static void Initialize(CucaContext context)
        {
            context.Database.EnsureCreated();

            if (context.Cucas.Any())
            {
                return;
            }

            var cucas = new List<Cuca>
            {
                new Cuca() { Date = DateTime.UtcNow.AddDays(15), Value = 15.10M },
                new Cuca() { Date = DateTime.UtcNow.AddDays(30), Value = 17.50M },
                new Cuca() { Date = DateTime.UtcNow.AddMonths(2), Value = 20.20M },
                new Cuca() { Date = DateTime.UtcNow.AddMonths(3), Value = 30.20M },
                new Cuca() { Date = DateTime.UtcNow.AddMonths(4), Value = 12.12M },
                new Cuca() { Date = DateTime.UtcNow.AddMonths(5), Value = 30.60M },
                new Cuca() { Date = DateTime.UtcNow.AddMonths(6), Value = 18.89M },
                new Cuca() { Date = DateTime.UtcNow.AddMonths(7), Value = 12.33M },
            };

            context.Cucas.AddRange(cucas);

            context.SaveChanges();
        }
    }
}

