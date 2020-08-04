using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Shwallak.Models
{
    public class OurDB : DbContext
    {
        public DbSet<Article> Articles { get; set; }
        public DbSet<Writer> Writers { get; set; }
        public DbSet<Comment> Commants { get; set; }
        public DbSet<Subscriber> Subscribers { get; set; }
    }
}