﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatDb.Models
{
    public class User
    {
        public virtual List<Message>? MessagesTo { get; set; } = new();
        public virtual List<Message>? MessagesFrom { get; set; } = new();
        public int Id { get; set; }
        public string? FullName { get; set; }
        public string? IpAddress {  get; set; }
        public string? Port { get; set; }
    }
}
