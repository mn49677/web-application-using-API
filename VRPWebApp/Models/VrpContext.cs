using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using VRPWebApp.Models;

namespace Models
{
    public class VrpContext: DbContext
    {
        public VrpContext(DbContextOptions options)
            : base(options)
        {
        }
        public DbSet<VrpOrToolsRequestLog> VrpOrToolsRequestLogs { get; set; }
        public DbSet<VrpOrToolsResponseLog> VrpOrToolsResponseLogs { get; set; }

    }
}
