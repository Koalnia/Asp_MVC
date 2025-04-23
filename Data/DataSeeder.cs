using Asp_MVC.Models;
using Microsoft.AspNetCore.Identity;
using System.Data;

namespace Asp_MVC.Data
{
    public class DataSeeder : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public DataSeeder(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<long>>>();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            if (!userManager.Users.Any())
            {

                if (!await roleManager.RoleExistsAsync("User"))
                {
                    await roleManager.CreateAsync(new IdentityRole<long>("User"));
                }
                if (!await roleManager.RoleExistsAsync("Admin"))
                {
                    await roleManager.CreateAsync(new IdentityRole<long>("Admin"));
                }

                var passwordHasher = new PasswordHasher<User>();
                var users = new List<User>
                {
            new User { UserName = "kasia.w@gmail.com", Name = "Katarzyna Wiśniewska", PhoneNumber = "607456789", Email = "kasia.w@gmail.com", PasswordHash = passwordHasher.HashPassword(null, "SecurePass789!") },
            new User { UserName = "t.zielinski@gmail.com", Name = "Tomasz Zieliński", PhoneNumber = "+48123456789", Email = "t.zielinski@gmail.com", PasswordHash = passwordHasher.HashPassword(null, "Pass1234!")},
            new User { UserName = "joanna.lew@gmail.com", Name = "Joanna Lewandowska", PhoneNumber = "444555789", Email = "joanna.lew@gmail.com", PasswordHash = passwordHasher.HashPassword(null, "Joanna2023!")},
            new User { UserName = "michal.nowicki@gmail.com", Name = "Michał Nowicki", PhoneNumber = "888999000", Email = "michal.nowicki@gmail.com", PasswordHash = passwordHasher.HashPassword(null, "Micha12345!")},
            new User { UserName = "agnieszka.k@gmail.com", Name = "Agnieszka Kaczmarek", PhoneNumber = "999000111", Email = "agnieszka.k@gmail.com", PasswordHash = passwordHasher.HashPassword(null, "Agnieszka2024!")},
            new User { UserName = "piotr.w@gmail.com", Name = "Piotr Wiśniewski", PhoneNumber = "666777888", Email = "piotr.w@gmail.com", PasswordHash = passwordHasher.HashPassword(null, "PiotrPass!")},
            new User { UserName = "ewa.majewska@gmail.com", Name = "Ewa Majewska", PhoneNumber = "+48123123123", Email = "ewa.majewska@gmail.com", PasswordHash = passwordHasher.HashPassword(null, "EwaMaj12!")}
        };
                var admins = new List<User>
                {
            new User {  UserName = "admin1@pl",Name = "Mateusz Rykała", PhoneNumber = "123456789", Email = "admin1@pl", PasswordHash = passwordHasher.HashPassword(null, "Hasloha11!")},
            new User {  UserName = "anna.kowalska@gmail.com",Name = "Anna Kowalska", PhoneNumber = "111222333", Email = "anna.kowalska@gmail.com", PasswordHash = passwordHasher.HashPassword(null, "Password123!")},
            new User {  UserName = "jan.nowak@gmail.com",Name = "Jan Nowak", PhoneNumber = "444555666", Email = "jan.nowak@gmail.com", PasswordHash = passwordHasher.HashPassword(null, "Qwerty456!")},
        };


                var advertisements = new List<Advertisement>
        {
            new Advertisement {  UserId = 1, Title = "Sprzedam rower", Description = "Mało używany rower w dobrym stanie marki Mentis", Price = "300 zł", Duration = "30 dni", CreatedAt = "27.01.2025 21:11" },
            new Advertisement {  UserId = 1, Title = "Kupię garaż w Rzeszowie", Description = "Kupię garaż lub miejsce parkingowe w okolicach ul. Cichej", Price = "20-35 tys. zł", Duration = "Do odwołania", CreatedAt = "25.12.2024 14:21" },
            new Advertisement {  UserId = 2, Title = "Oddam starą sofę", Description = "Oddam za darmo starą sofę w kolorze bordowym, odbiór osobisty", Price = "Za darmo", Duration = "Brak", CreatedAt = "03.08.2024 07:21" },
            new Advertisement {  UserId = 2, Title = "Sprzedam Hondę Jazz", Description = "Sprzedam bezwypadkową Hondę Jazz, o przebiegu 30 tys. km", Price = "10 tys. zł, cena do negocjacji", Duration = "2 miesiące", CreatedAt = "06.01.2025 14:14" },
            new Advertisement {  UserId = 2, Title = "Oddam małego kota pod opiekę", Description = "Oddam małego, białego, 1-rocznego kota do opieki.", Price = "Brak", Duration = "Do znalezienia opiekuna", CreatedAt = "13.07.2024 12:39" },
            new Advertisement {  UserId = 3, Title = "Sprzedam używany laptop", Description = "Laptop Dell Inspiron 15, stan bardzo dobry, 8GB RAM, 256GB SSD", Price = "1 200 zł", Duration = "14 dni", CreatedAt = "01.02.2025 10:00" },
            new Advertisement {  UserId = 3, Title = "Kupię używane książki", Description = "Szukam książek science fiction w dobrym stanie", Price = "Do 200 zł", Duration = "1 miesiąc", CreatedAt = "28.01.2025 18:30" },
            new Advertisement {  UserId = 3, Title = "Sprzedam biurko", Description = "Biurko sosnowe, wymiar 120x60 cm, idealne do pracy zdalnej", Price = "300 zł", Duration = "7 dni", CreatedAt = "26.01.2025 15:45" },
            new Advertisement {  UserId = 4, Title = "Sprzedam kurtkę zimową", Description = "Kurtka zimowa damska, rozmiar M, kolor czarny, bardzo ciepła", Price = "150 zł", Duration = "Brak", CreatedAt = "20.01.2025 12:20" },
            new Advertisement {  UserId = 4, Title = "Oddam akwarium", Description = "Akwarium 50L, bez rybek, w komplecie z filtrem i lampą", Price = "Za darmo", Duration = "10 dni", CreatedAt = "19.01.2025 08:30" },
            new Advertisement {  UserId = 4, Title = "Kupię używany aparat fotograficzny", Description = "Poszukuję aparatu marki Canon lub Nikon w dobrym stanie", Price = "Do 2 000 zł", Duration = "Do odwołania", CreatedAt = "17.01.2025 21:11" },
            new Advertisement {  UserId = 5, Title = "Sprzedam pralkę", Description = "Pralka automatyczna Bosch, model z 2020 roku, w pełni sprawna", Price = "800 zł", Duration = "14 dni", CreatedAt = "15.01.2025 10:00" },
            new Advertisement {  UserId = 5, Title = "Oddam regał na książki", Description = "Regał z IKEA, 5 półek, wymiary 200x80x30 cm", Price = "Za darmo", Duration = "Brak", CreatedAt = "13.01.2025 09:45" },
            new Advertisement {  UserId = 5, Title = "Sprzedam fotel biurowy", Description = "Fotel obrotowy, ergonomiczny, w kolorze czarnym, mało używany", Price = "400 zł", Duration = "30 dni", CreatedAt = "11.01.2025 16:30" },
            new Advertisement {  UserId = 5, Title = "Kupię działkę rekreacyjną", Description = "Interesuje mnie działka w okolicach Rzeszowa, powierzchnia 5-10 arów", Price = "Do 50 tys. zł", Duration = "Do odwołania", CreatedAt = "10.01.2025 11:15" },
            new Advertisement {  UserId = 6, Title = "Sprzedam telewizor", Description = "Telewizor LED Samsung, 40 cali, Full HD, model z 2021 roku", Price = "1 500 zł", Duration = "21 dni", CreatedAt = "08.01.2025 18:00" },
            new Advertisement {  UserId = 6, Title = "Oddam książki do nauki języka angielskiego", Description = "Zestaw książek poziom podstawowy i średniozaawansowany", Price = "Za darmo", Duration = "Brak", CreatedAt = "06.01.2025 08:20" },
            new Advertisement {  UserId = 7, Title = "Sprzedam rower miejski", Description = "Rower miejski marki Giant, w kolorze niebieskim, 3-biegowy", Price = "700 zł", Duration = "14 dni", CreatedAt = "04.01.2025 14:50" },
            new Advertisement {  UserId = 7, Title = "Kupię telefon używany", Description = "Szukam telefonu Samsung Galaxy S21 w dobrym stanie", Price = "Do 2 500 zł", Duration = "Do odwołania", CreatedAt = "02.01.2025 11:30" },
            new Advertisement {  UserId = 7, Title = "Sprzedam namiot turystyczny", Description = "Namiot 2-osobowy, wodoodporny, idealny na wędrówki górskie", Price = "250 zł", Duration = "7 dni", CreatedAt = "31.12.2024 13:00" },
            new Advertisement {  UserId = 8, Title = "Oddam stare płyty CD", Description = "Zestaw płyt muzycznych z lat 80. i 90., odbiór osobisty", Price = "Za darmo", Duration = "Brak", CreatedAt = "29.12.2024 15:40" },
            new Advertisement {  UserId = 8, Title = "Sprzedam konsolę do gier", Description = "Konsola PlayStation 4, 500GB, z jednym padem i grą gratis", Price = "900 zł", Duration = "14 dni", CreatedAt = "27.12.2024 19:20" },
            new Advertisement {  UserId = 8, Title = "Kupię stolik kawowy", Description = "Szukam małego stolika kawowego w stylu skandynawskim", Price = "Do 300 zł", Duration = "1 miesiąc", CreatedAt = "25.12.2024 14:00" },
            new Advertisement {  UserId = 9, Title = "Sprzedam zestaw garnków", Description = "Garnki ze stali nierdzewnej, 5 sztuk, stan idealny", Price = "500 zł", Duration = "10 dni", CreatedAt = "23.12.2024 09:30" },
            new Advertisement {  UserId = 9, Title = "Oddam ubrania dla niemowlaka", Description = "Ubranka w rozmiarze 62-68, zarówno dla chłopca, jak i dziewczynki", Price = "Za darmo", Duration = "Brak", CreatedAt = "21.12.2024 08:15" },
            new Advertisement {  UserId = 9, Title = "Sprzedam łóżko piętrowe", Description = "Łóżko piętrowe dla dzieci, z materacami, w kolorze białym", Price = "1 000 zł", Duration = "30 dni", CreatedAt = "19.12.2024 17:45" },
            new Advertisement {  UserId = 10, Title = "Kupię gry planszowe", Description = "Interesują mnie używane gry planszowe w dobrym stanie", Price = "Do 300 zł", Duration = "Do odwołania", CreatedAt = "17.12.2024 14:50" },
            new Advertisement {  UserId = 10, Title = "Oddam stare monitory", Description = "Dwa monitory LCD 19 cali, sprawne, odbiór osobisty", Price = "Za darmo", Duration = "Brak", CreatedAt = "15.12.2024 12:30" },
            new Advertisement {  UserId = 10, Title = "Kupię gitarę w dobrym stanie", Description = "W ramach hobby kupię gitarę", Price = "Do 2000 zł", Duration = "Do odwołania", CreatedAt = "11.09.2024 13:59" },
            new Advertisement {  UserId = 10, Title = "Sprzedam zadbaną gitarę", Description = "Ledwo używana gitara z wzmacniaczem", Price = "1500 zł", Duration = "3 tygodnie", CreatedAt = "20.01.2025 09:30" }
        };
                foreach (var admin in admins)
                {
                    await userManager.CreateAsync(admin);
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
                foreach (var user in users)
                {
                    await userManager.CreateAsync(user);
                    await userManager.AddToRoleAsync(user, "User");
                }
                context.Advertisements.AddRange(advertisements);
                await context.SaveChangesAsync();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}


