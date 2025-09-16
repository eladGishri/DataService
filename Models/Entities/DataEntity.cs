using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataService.Models.Entities
{
    public class DataEntity
    {
        [Key]
        public string Id { get; set; }
        public string Value { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
