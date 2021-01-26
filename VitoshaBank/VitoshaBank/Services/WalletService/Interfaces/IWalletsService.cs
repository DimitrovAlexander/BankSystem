﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using VitoshaBank.Data.MessageModels;
using VitoshaBank.Data.Models;
using VitoshaBank.Data.ResponseModels;
using VitoshaBank.Services.IBANGeneratorService.Interfaces;

namespace VitoshaBank.Services.Interfaces.WalletService
{
    public interface IWalletsService
    {
        public Task<ActionResult<WalletResponseModel>> GetWalletInfo(ClaimsPrincipal currentUser, string username, BankSystemContext _context, MessageModel _messageModel);
        public Task<ActionResult<MessageModel>> CreateWallet(ClaimsPrincipal currentUser, string username, Wallets wallet, IIBANGeneratorService _IBAN, BankSystemContext _context, MessageModel _messageModel);
        public Task<ActionResult<MessageModel>> DepositMoney(Wallets wallet, ClaimsPrincipal currentUser, string username, decimal amount, BankSystemContext _context, MessageModel _messageModel);
        public Task<ActionResult<MessageModel>> SimulatePurchase(Wallets wallet, string product, ClaimsPrincipal currentUser, string username, decimal amount, BankSystemContext _context, MessageModel _messageModel);
        public Task<ActionResult<MessageModel>> DeleteWallet(ClaimsPrincipal currentUser, string username, BankSystemContext _context, MessageModel _messageModel);
    }
}