using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobHunterApi.Models
{
    public class CompanyModel
    {
        public int Id { get; set; }
        public string Organization { get; set; }=string.Empty;
        public string Description { get; set; }=string.Empty;
        public string ?Status{get; set;}="Not Applied";
        public string? LastApplied { get; set;}="Yet to Apply";
        public string? Link { get; set;}=string.Empty;

    }
}