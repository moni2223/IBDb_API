using IBDb.Models;
using IBDb.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Collections.Generic;

namespace IBDb
{
    public class Seed // here we fill the db with some information
    {
        private readonly DataContext _dataContext;

        public Seed(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public void SeedDataContext()
        {
            if (!_dataContext.Roles.Any())
            {
                var roles = new[]
                {
                    new Role { RoleName = "admin" },
                    new Role { RoleName = "publisher" },
                    new Role { RoleName = "basic" }
                };
                _dataContext.Roles.AddRange(roles);
                _dataContext.SaveChanges();
            }

            if (!_dataContext.Users.Any())
            {
                var adminRole = _dataContext.Roles.Single(r => r.RoleName == "admin");
                var publisherRole = _dataContext.Roles.Single(r => r.RoleName == "publisher");
                var basicRole = _dataContext.Roles.Single(r => r.RoleName == "basic");

                var users = new[]
                {
                    new User { UserName = "admin_user", PasswordHash = "hashed_password", FullName = "Admin User", Email = "admin@example.com", Role = adminRole },
                    new User { UserName = "publisher_user", PasswordHash = "hashed_password", FullName = "Publisher User", Email = "publisher@example.com", Role = publisherRole },
                    new User { UserName = "basic_user", PasswordHash = "hashed_password", FullName = "Basic User", Email = "basic@example.com", Role = basicRole }
                };
                _dataContext.Users.AddRange(users);
                _dataContext.SaveChanges();
            }

            if (!_dataContext.Books.Any())
            {
                var publisherUser = _dataContext.Users.Single(u => u.UserName == "publisher_user");
                var adminUser = _dataContext.Users.Single(u => u.UserName == "admin_user");

                var books = new[]
                {
                    new Book { Title = "Book 1", Price = 19.99m, Description = "Short description of Book 1", CreatedAt = DateTime.UtcNow, Publisher = publisherUser },
                    new Book { Title = "Book 2", Price = 9.99m, Description = "Short description of Book 2.Short description of Book 2.Short description of Book 2.", CreatedAt = DateTime.UtcNow, Publisher = adminUser },
                    new Book { Title = "Book 3", Price = 29.99m, Description = "Short description of Book 3", CreatedAt = DateTime.UtcNow, Publisher = publisherUser }
                };
                _dataContext.Books.AddRange(books);
                _dataContext.SaveChanges();
            }

            if (!_dataContext.Genres.Any())
            {
                var genres = new[]
                {
                    new Genre { Name = "Fiction" },
                    new Genre { Name = "Drama" },
                    new Genre { Name = "Science" }
                };
                _dataContext.Genres.AddRange(genres);
                _dataContext.SaveChanges();
            }

            if (!_dataContext.Reviews.Any())
            {
                var basicUser = _dataContext.Users.Single(u => u.UserName == "basic_user");
                var reviews = new[]
                {
                    new Review { BookID = 1, UserID = basicUser.Id, Rating = 5, Comment = "Great book!", CreatedAt = DateTime.UtcNow },
                    new Review { BookID = 2, UserID = basicUser.Id, Rating = 4, Comment = "Good read.", CreatedAt = DateTime.UtcNow },
                    new Review { BookID = 3, UserID = basicUser.Id, Rating = 3, Comment = "It was okay.", CreatedAt = DateTime.UtcNow }
                };
                _dataContext.Reviews.AddRange(reviews);
                _dataContext.SaveChanges();
            }

            if (!_dataContext.BookGenres.Any())
            {
                var bookGenres = new List<BookGenre>
                {
                    new BookGenre { BookId = 1, GenreId = 1 },
                    new BookGenre { BookId = 2, GenreId = 2 },
                    new BookGenre { BookId = 3, GenreId = 3 }
                };
                _dataContext.BookGenres.AddRange(bookGenres);
                _dataContext.SaveChanges();
            }
        }
    }
}