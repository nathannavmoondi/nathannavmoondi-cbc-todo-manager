using System;
using Blazor_Template_Models;
using Microsoft.EntityFrameworkCore;

namespace Blazor_Template_API.Contexts.DataAccess
{
    public class TodoDbContext : DbContext
    {
        public TodoDbContext(DbContextOptions<TodoDbContext> options) : base(options) { }
        public TodoDbContext() { }

        public DbSet<TodoDTO> Todos{ get; set; }
    }
}
