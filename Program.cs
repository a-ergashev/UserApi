using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace UserApi
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<UserDb>(o => o.UseInMemoryDatabase("Users"));
            var app = builder.Build();

            app.MapPost("/upload", (HttpRequest request, UserDb context) =>
            {
                var files = request.Form.Files;
                if (files.Count == 1 && files[0].FileName.EndsWith(".csv"))
                {
                    using (var stream = File.Create("users.txt"))
                        files[0].CopyTo(stream);

                    using (var reader = new StreamReader("users.txt"))
                    {
                        while (!reader.EndOfStream)
                        {
                            string[] data = reader.ReadLine().Split(',');
                            User user = new()
                            {
                                Id = Guid.Parse(data[0]),
                                Username = data[1],
                                Age = int.Parse(data[2]),
                                City = data[3],
                                Phone = data[4],
                                Email = data[5]
                            };

                            if (context.Users.Find(user.Id) is User found)
                                context.Entry(found).CurrentValues.SetValues(user);
                            else
                                context.Users.Add(user);
                        }
                    }

                    context.SaveChanges();
                    return Results.Ok();
                }

                return TypedResults.BadRequest();
            });

            app.MapGet("/users", (UserDb db,
                [FromQuery] string Order,
                [FromQuery] int Count) =>
            {
                var users = from u in db.Users
                            select u;
                users = Order.ToLower() switch
                {
                    "desc" => users.OrderByDescending(s => s.Username),
                    _ => users.OrderBy(s => s.Username),
                };
                return users.Take(Count).AsNoTracking().ToList();
            });

            app.Run();
        }
    }
}