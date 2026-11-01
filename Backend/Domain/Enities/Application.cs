using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Domain.Enities
{
    public class Application
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public DateTime CreationTime { get; set; }
        public bool IsActive { get; set; } = true;
        public string? Domain { get; set; }
        public string? IpAddress { get; set;  }
        public string? Environment { get; set; }

    }
}
