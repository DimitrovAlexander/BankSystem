﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using VitoshaBank.Data.MessageModels;
using VitoshaBank.Data.Models;
using VitoshaBank.Data.ResponseModels;
using VitoshaBank.Services.DebitCardService.Interfaces;
using VitoshaBank.Services.IBANGeneratorService.Interfaces;

namespace VitoshaBank.Services.BankAccountService.Interfaces
{
    public interface IBankAccountService
    {

        public Task<ActionResult<BankAccountResponseModel>> GetBankAccountInfo(ClaimsPrincipal currentUser, string username, BankSystemContext _context, MessageModel messageModel);
        public Task<ActionResult<Users>> DeleteBankAccount(ClaimsPrincipal currentUser, string username, BankSystemContext _context, MessageModel messageModel);
       
        public Task<ActionResult> CreateBankAccount(ClaimsPrincipal currentUser, string username, BankAccounts bankAccount, IIBANGeneratorService _IBAN, BankSystemContext _context, IDebitCardService _debitCardService, MessageModel messageModel);
        
    }
}
