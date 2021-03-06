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
using VitoshaBank.Services.CreditPayOffService.Interfaces;
using VitoshaBank.Services.CreditService;
using VitoshaBank.Services.CreditService.Interfaces;
using VitoshaBank.Services.DebitCardService.Interfaces;
using VitoshaBank.Services.IBANGeneratorService.Interfaces;
using VitoshaBank.Services.TransactionService.Interfaces;

namespace VitoshaBank.Controllers
{
    [Route("api/credit")]
    [ApiController]
    public class CreditController : ControllerBase
    {
        private readonly BankSystemContext _context;
        private readonly ILogger<Credits> _logger;
        private readonly ICreditService _creditService;
        private readonly IIBANGeneratorService _IBAN;
        private readonly MessageModel _messageModel;
        private readonly ITransactionService _transactionService;
        private readonly ICreditPayOffService _creditPayOff;
        

        public CreditController(BankSystemContext context, ILogger<Credits> logger, ICreditService creditService, IIBANGeneratorService IBAN, ITransactionService transactionService, ICreditPayOffService creditPayOff)
        {
            _context = context;
            _logger = logger;
            _creditService = creditService;
            _IBAN = IBAN;
            _messageModel = new MessageModel();
            _transactionService = transactionService;
            _creditPayOff = creditPayOff;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<CreditResponseModel>> GetCreditInfo()
        {
            var currentUser = HttpContext.User;
            string username = currentUser.Claims.FirstOrDefault(currentUser => currentUser.Type == "Username").Value;
            return await _creditService.GetCreditInfo(currentUser, username, _creditPayOff, _context,  _messageModel);
        }
        [HttpGet("check")]
        [Authorize]
        public async Task<ActionResult<CreditResponseModel>> GetPayOffInfo()
        {
            var currentUser = HttpContext.User;
            string username = currentUser.Claims.FirstOrDefault(currentUser => currentUser.Type == "Username").Value;
            return await _creditService.GetPayOffInfo(currentUser, username, _creditPayOff, _context, _messageModel);
        }

        [HttpPut("purchase")]
        [Authorize]

        public async Task<ActionResult<MessageModel>> Purchase(CreditRequestModel requestModel)
        {
            var currentUser = HttpContext.User;
            string username = currentUser.Claims.FirstOrDefault(currentUser => currentUser.Type == "Username").Value;
            return await _creditService.SimulatePurchase(requestModel.Credit, requestModel.Product, requestModel.Reciever, currentUser, username, requestModel.Amount, _context,_transactionService, _messageModel);
        }

        [HttpPut("deposit")]
        [Authorize]
        public async Task<ActionResult<MessageModel>> Deposit(CreditRequestModel requestModel)
        {
            //amount = 0.50M;
            var currentUser = HttpContext.User;
            string username = currentUser.Claims.FirstOrDefault(currentUser => currentUser.Type == "Username").Value;
            return await _creditService.AddMoney(requestModel.Credit, currentUser, username, requestModel.Amount, _context, _transactionService, _messageModel);
        }
        [HttpPut("withdraw")]
        [Authorize]
        public async Task<ActionResult<MessageModel>> Withdraw(CreditRequestModel requestModel)
        {
            var currentUser = HttpContext.User;
            string username = currentUser.Claims.FirstOrDefault(currentUser => currentUser.Type == "Username").Value;
            return await _creditService.Withdraw(requestModel.Credit, currentUser, username, requestModel.Amount, "User in ATM", _context, _transactionService, _messageModel);
        }

    }

}
