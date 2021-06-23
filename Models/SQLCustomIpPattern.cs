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

        public IEnumerable<CustomIpPattern> GetAllPattern()
        {
            var result = context.CustomIpPatterns.ToList();
            return result;
        }
        public CustomIpPattern Get(int id)
        {
            var result = context.CustomIpPatterns.Find(id);
            if(result == null)
            {
                logger.LogError("this is empty");
            }
            return result;
        }

        public CustomIpPattern GetEnabledPattern()
        {
            var result = context.CustomIpPatterns.FirstOrDefault(x => x.Enabled);
            return result;
        }

        public IEnumerable<CustomIpPattern> GetEnabledPatterns()
        {
            var result = context.CustomIpPatterns.Where(x => x.Enabled).ToList();
            return result;
        }

        public CustomIpPattern Update(CustomIpPattern model)
        {
            model.Enabled = !model.Enabled;
            var patternChanged = context.CustomIpPatterns.Attach(model);
            patternChanged.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            context.SaveChanges();
            return model;
        }

        public CustomIpPattern Delete(int id)
        {

            var pattern = context.CustomIpPatterns.Find(id);
            if (pattern != null)
            {
                context.CustomIpPatterns.Remove(pattern);
                context.SaveChanges();
            }
            return pattern;

        }
    }
}
