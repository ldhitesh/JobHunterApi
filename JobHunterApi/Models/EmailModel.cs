using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobHunterApi.Models
{
   
public class EmailModel
{
    public string To { get; set; }=string.Empty;
    public string Subject { get; set; }=string.Empty;
    public string Body { get; set; }=string.Empty;
}
}