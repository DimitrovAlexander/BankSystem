﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitoshaBank.Data.Models;

namespace VitoshaBank.Data.RequestModels
{
    public class WalletRequestModel
    {
        public Wallets Wallet { get; set; }
        public string Product { get; set; }
        public decimal Amount { get; set; }
        public string Username { get; set; }
        public string Reciever { get; set; }
    }
}
