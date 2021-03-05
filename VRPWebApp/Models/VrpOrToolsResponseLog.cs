using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VRPWebApp.Models
{
    public class VrpOrToolsResponseLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public DateTime DateTimeOfResponse {get; set;}
        public string Response { get; set; } // response JSON

        public virtual VrpOrToolsRequestLog requestLog { get; set; }


    }
}
