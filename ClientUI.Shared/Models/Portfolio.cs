using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.ClientUI.Shared.Models
{
    public class Portfolio
    {
        public readonly string PROJECT = "Project";
        public readonly string DATABASE = "Database";
        public string? ID { get; set; }
        public string? TheType { get; set; }
        public string? Name { get; set; }
        public string? ParentId { get; set; }
    }
}
