using EmployeeManagment.Security;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagment.Models
{
    public class IpDetectedRepository : IIpDetectedRepository
    {
        private readonly AppDbContext context;
        private readonly ILogger<IpDetectedRepository> logger;

        public IpDetectedRepository(ILogger<IpDetectedRepository> logger, AppDbContext context)
        {
            this.context = context;
            this.logger = logger;
        }
        public IpDetected add(string ip,int idPattern)
        {
            var model = new IpDetected
            {
                Ip = ip,
                requestTime = DateTime.Now,
                Pattern = idPattern
            };
            context.IpDetecteds.Add(model);
            context.SaveChanges();
            return model;
        }
        public List<IpDetected> GetAll()
        {
            return context.IpDetecteds.ToList();
        }
    }
}
