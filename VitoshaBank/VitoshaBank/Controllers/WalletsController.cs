﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitoshaBank.Data.MessageModels;
using VitoshaBank.Data.Models;
using VitoshaBank.Data.RequestModels;
using VitoshaBank.Data.ResponseModels;
using VitoshaBank.Services.IBANGeneratorService.Interfaces;
using VitoshaBank.Services.Interfaces.UserService;
using VitoshaBank.Services.Interfaces.WalletService;
using VitoshaBank.Services.TransactionService.Interfaces;

namespace VitoshaBank.Controllers
{
    [ApiController]
    [Route("api/wallet")]
    public class WalletsController : ControllerBase
    {
        private readonly BankSystemContext _context;
        private readonly ILogger<Wallets> _logger;
        private readonly IWalletsService _walletService;
        private readonly IIBANGeneratorService _IBAN;
        private readonly ITransactionService _transaction;
        private readonly IBCryptPasswordHasherService _BCrypt;
        private readonly MessageModel _messageModel;
        public WalletsController(BankSystemContext context, ILogger<Wallets> logger, IWalletsService walletService, IIBANGeneratorService IBAN, ITransactionService transaction, IBCryptPasswordHasherService BCrypt)
        {
            _context = context;
            _logger = logger;
            _walletService = walletService;
            _IBAN = IBAN;
            _transaction = transaction;
            _messageModel = new MessageModel();
            _BCrypt = BCrypt;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<WalletResponseModel>> GetWalletInfo()
        {
            var currentUser = HttpContext.User;
            string username = currentUser.Claims.FirstOrDefault(currentUser => currentUser.Type == "Username").Value;
            return await _walletService.GetWalletInfo(currentUser, username, _context, _messageModel);
        }

        [HttpPut("deposit")]
        [Authorize]
        public async Task<ActionResult<MessageModel>> DepositInWallet(WalletRequestModel requestModel)
        {
            //Wallet(IBAN), Amount
            var currentUser = HttpContext.User;
            string username = currentUser.Claims.FirstOrDefault(currentUser => currentUser.Type == "Username").Value;
            return await _walletService.AddMoney(requestModel.Wallet, currentUser, username, requestModel.Amount, _context, _transaction, _messageModel);
        }

        [HttpPut("purchase")]
        [Authorize]
        public async Task<ActionResult<MessageModel>> PurchaseWithWallet(WalletRequestModel requestModel)
        {
            //amount = 10000;
            var currentUser = HttpContext.User;
            string username = currentUser.Claims.FirstOrDefault(currentUser => currentUser.Type == "Username").Value;
            return await _walletService.SimulatePurchase(requestModel.Wallet, requestModel.Product, requestModel.Reciever, currentUser, username, requestModel.Amount, _context, _transaction, _BCrypt, _messageModel);
        }


    }
}
