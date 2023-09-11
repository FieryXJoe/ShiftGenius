﻿using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<ShiftGeniusLibDB.Models.TimeOffRequest> TimeOffRequests { get; set; }
    //TODO: public DbSet<ShiftGeniusLibDB.Models.Availibility>
}
