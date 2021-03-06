﻿using System;
using System.Collections.Generic;

namespace VitoshaBank.Data.Models
{
    public partial class Users
    {
        public Users()
        {
            SupportTickets = new HashSet<SupportTickets>();
        }

        public int Id { get; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime RegisterDate { get; set; }
        public int? LastTransactionId { get; set; }
        public DateTime BirthDate { get; set; }
        public bool IsAdmin { get; set; }
        public string Email { get; set; }
        public bool IsConfirmed { get; set; }
        public string ActivationCode { get; set; }

        public virtual Transactions LastTransaction { get; set; }
        public virtual Cards Cards { get; set; }
        public virtual ChargeAccounts ChargeAccounts { get; set; }
        public virtual Credits Credits { get; set; }
        public virtual Deposits Deposits { get; set; }
        public virtual Wallets Wallets { get; set; }
        public virtual ICollection<SupportTickets> SupportTickets { get; set; }
    }
}
