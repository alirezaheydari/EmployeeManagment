using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagment.Models
{
    public class SQLCustomIpPattern : ICustomIpPatternRepository
    {
        private readonly AppDbContext context;
        private readonly ILogger<SQLCustomIpPattern> logger;

        public SQLCustomIpPattern(AppDbContext context, ILogger<SQLCustomIpPattern> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        public CustomIpPattern Add(CustomIpPattern model)
        {
            context.CustomIpPatterns.Add(model);
            context.SaveChanges();
            return model;
        }

        public CustomIpPattern GetEnabledPattern()
        {
            var result = context.CustomIpPatterns.FirstOrDefault(x => x.Enabled);
            return result;
        }
    }
}
