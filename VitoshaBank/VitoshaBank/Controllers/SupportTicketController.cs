﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitoshaBank.Data.MessageModels;
using VitoshaBank.Data.Models;
using VitoshaBank.Data.RequestModels;
using VitoshaBank.Data.ResponseModels;
using VitoshaBank.Services.BankAccountService.Interfaces;
using VitoshaBank.Services.SupportTicketsService.Interfaces;

namespace VitoshaBank.Controllers
{
    [Route("api/support")]
    [ApiController]
    public class SupportTicketController : ControllerBase
    {
        private readonly BankSystemContext _context;
        private readonly ILogger<SupportTickets> _logger;
        private readonly ISupportTicketService _ticketService;
        private readonly MessageModel _messageModel;

        public SupportTicketController(BankSystemContext context, ILogger<SupportTickets> logger, ISupportTicketService ticketService)
        {
            _context = context;
            _logger = logger;
            _ticketService = ticketService;
            _messageModel = new MessageModel();
        }
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<ICollection<SupportTicketResponseModel>>> GetUserTickets()
        {
            var currentUser = HttpContext.User;
            string username = currentUser.Claims.FirstOrDefault(currentUser => currentUser.Type == "Username").Value;
            return await _ticketService.GetUserTicketsInfo(currentUser, username, _context, _messageModel);
        }
        [HttpPost("create")]
        [Authorize]
        public async Task<ActionResult<MessageModel>> CreateTicket(SupportTicketRequestModel requestModel)
        {
            var currentUser = HttpContext.User;
            string username = currentUser.Claims.FirstOrDefault(currentUser => currentUser.Type == "Username").Value;
            return await _ticketService.CreateSupportTicket(currentUser, username,requestModel.Ticket, _context, _messageModel);
        }
    }
}
