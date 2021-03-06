﻿using System;
using System.Collections.Generic;

namespace VitoshaBank.Data.Models
{
    public partial class SupportTickets
    {
        public int Id { get; }
        public int UserId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public DateTime Date { get; set; }
        public bool HasResponce { get; set; }

        public virtual Users User { get; set; }
    }
}
