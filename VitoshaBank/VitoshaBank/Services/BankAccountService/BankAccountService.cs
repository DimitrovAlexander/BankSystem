﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using VitoshaBank.Data.Models;
using VitoshaBank.Services.BankAccountService.Interfaces;
using VitoshaBank.Services.IBANGeneratorService.Interfaces;
using VitoshaBank.Services.DebitCardService;
using VitoshaBank.Services.DebitCardService.Interfaces;
using VitoshaBank.Data.ResponseModels;
using VitoshaBank.Data.MessageModels;
using System;
using VitoshaBank.Services.TransactionService.Interfaces;

namespace VitoshaBank.Services.BankAccountService
{
    public class BankAccountService : ControllerBase, IBankAccountService
    {
        public async Task<ActionResult<BankAccountResponseModel>> GetBankAccountInfo(ClaimsPrincipal currentUser, string username, BankSystemContext _context, MessageModel messageModel)
        {
            if (currentUser.HasClaim(c => c.Type == "Roles"))
            {
                var userAuthenticate = await _context.Users.FirstOrDefaultAsync(x => x.Username == username);
                BankAccounts bankAccountExists = null;
                BankAccountResponseModel bankAccountResponseModel = new BankAccountResponseModel();

                if (userAuthenticate == null)
                {
                    messageModel.Message = "User not found";
                    return StatusCode(404, messageModel);
                }
                else
                {
                    bankAccountExists = await _context.BankAccounts.FirstOrDefaultAsync(x => x.UserId == userAuthenticate.Id);
                }

                if (bankAccountExists != null)
                {
                    bankAccountResponseModel.IBAN = bankAccountExists.Iban;
                    bankAccountResponseModel.Amount = Math.Round(bankAccountExists.Amount,2);
                    return StatusCode(200, bankAccountResponseModel);
                }
            }
            else
            {
                messageModel.Message = "You are not authorized to do such actions";
                return StatusCode(403, messageModel);
            }

            messageModel.Message = "You don't have a bank account!";
            return StatusCode(400, messageModel);
        }
        public async Task<ActionResult<MessageModel>> CreateBankAccount(ClaimsPrincipal currentUser, string username, BankAccounts bankAccount, IIBANGeneratorService _IBAN, BankSystemContext _context, IDebitCardService _debitCardService, MessageModel messageModel)
        {
            string role = "";

            if (currentUser.HasClaim(c => c.Type == "Roles"))
            {
                string userRole = currentUser.Claims.FirstOrDefault(currentUser => currentUser.Type == "Roles").Value;
                role = userRole;
            }

            if (role == "Admin")
            {
                var userAuthenticate = await _context.Users.FirstOrDefaultAsync(x => x.Username == username);
                BankAccounts bankAccountExists = null;

                if (userAuthenticate != null)
                {
                    bankAccountExists = await _context.BankAccounts.FirstOrDefaultAsync(x => x.UserId == userAuthenticate.Id);
                }


                if (bankAccountExists == null)
                {
                    if (ValidateUser(userAuthenticate) && ValidateBankAccount(bankAccount))
                    {
                        bankAccount.UserId = userAuthenticate.Id;
                        bankAccount.Iban = _IBAN.GenerateIBANInVitoshaBank("BankAccount", _context);
                        _context.Add(bankAccount);
                        await _context.SaveChangesAsync();
                        Cards card = new Cards();
                        await _debitCardService.CreateDebitCard(currentUser, username, bankAccount, _context, card, messageModel);
                        messageModel.Message = "Bank Account created succesfully";
                        return StatusCode(201, messageModel);
                    }
                    else if (ValidateUser(userAuthenticate) == false)
                    {
                        messageModel.Message = "User not found!";
                        return StatusCode(404, messageModel);
                    }
                    else if (ValidateBankAccount(bankAccount) == false)
                    {
                        messageModel.Message = "Invalid parameteres!";
                        return StatusCode(400, messageModel);
                    }
                }
                messageModel.Message = "BankAccount already exists!";
                return StatusCode(400, messageModel);
            }
            else
            {
                messageModel.Message = "You are not authorized to do such actions";
                return StatusCode(403, messageModel);
            }
        }
        public async Task<ActionResult<MessageModel>> DepositMoney(BankAccounts bankAccount, ClaimsPrincipal currentUser, string username, decimal amount, BankSystemContext _context, ITransactionService _transactionService, MessageModel messageModel)
        {
            var userAuthenticate = await _context.Users.FirstOrDefaultAsync(x => x.Username == username);

            BankAccounts bankAccounts = null;

            if (currentUser.HasClaim(c => c.Type == "Roles"))
            {
                if (userAuthenticate != null)
                {
                    bankAccounts = await _context.BankAccounts.FirstOrDefaultAsync(x => x.UserId == userAuthenticate.Id);
                }
                else
                {
                    messageModel.Message = "User not found!";
                    return StatusCode(404, messageModel);
                }

                if (bankAccounts != null)
                {
                    if (ValidateDepositAmountBankAccount(amount))
                    {
                        bankAccounts.Amount = bankAccounts.Amount + amount;
                        await _context.SaveChangesAsync();
                        Transactions transactions = new Transactions();
                        transactions.RecieverAccountInfo = bankAccounts.Iban;
                        transactions.SenderAccountInfo = "User in Bank";
                        await _transactionService.CreateTransaction(currentUser, amount, transactions, "UserinBank", "BankAccount", "Depositing money from bank office", _context, messageModel);
                        messageModel.Message = "Money deposited succesfully!";
                        return StatusCode(200, messageModel);
                    }
                    messageModel.Message = "Invalid deposit amount!";
                    return StatusCode(400, messageModel);
                }
                else
                {
                    messageModel.Message = "BankAccount not found";
                    return StatusCode(404, messageModel);
                }
            }
            messageModel.Message = "You are not autorized to do such actions!";
            return StatusCode(403, messageModel);
        }
        public async Task<ActionResult<MessageModel>> SimulatePurchase(BankAccounts bankAccount, string product, ClaimsPrincipal currentUser, string username, decimal amount, string reciever, BankSystemContext _context, ITransactionService _transaction, MessageModel _messageModel)
        {
            var userAuthenticate = await _context.Users.FirstOrDefaultAsync(x => x.Username == username);

            BankAccounts bankAccounts = null;

            if (currentUser.HasClaim(c => c.Type == "Roles"))
            {
                if (userAuthenticate != null)
                {
                    bankAccounts = await _context.BankAccounts.FirstOrDefaultAsync(x => x.UserId == userAuthenticate.Id);
                }
                else
                {
                    _messageModel.Message = "User not found!";
                    return StatusCode(404, _messageModel);
                }

                if (bankAccounts != null)
                {
                    if (ValidateDepositAmountBankAccount(amount) && ValidateBankAccount(bankAccounts, amount))
                    {
                        bankAccounts.Amount = bankAccounts.Amount - amount;
                        Transactions transactions = new Transactions();
                        transactions.SenderAccountInfo = bankAccount.Iban;
                        transactions.RecieverAccountInfo = reciever;
                        await _transaction.CreateTransaction(currentUser, amount, transactions, "BankAccount", reciever, $"Purchasing {product}", _context, _messageModel);
                        await _context.SaveChangesAsync();
                        _messageModel.Message = $"Succesfully purhcased {product}.";
                        return StatusCode(200, _messageModel);
                    }
                    else if (ValidateDepositAmountBankAccount(amount) == false)
                    {
                        _messageModel.Message = "Invalid payment amount!";
                        return StatusCode(400, _messageModel);
                    }
                    else if (ValidateBankAccount(bankAccounts, amount) == false)
                    {
                        _messageModel.Message = "You don't have enough money in bank account!";
                        return StatusCode(406, _messageModel);
                    }

                }
                else
                {
                    _messageModel.Message = "BankAccount not found";
                    return StatusCode(404, _messageModel);
                }
            }
            _messageModel.Message = "You are not autorized to do such actions!";
            return StatusCode(403, _messageModel);
        }

        public async Task<ActionResult<MessageModel>> Withdraw(BankAccounts bankAccount, ClaimsPrincipal currentUser, string username, decimal amount, string reciever, BankSystemContext _context, ITransactionService _transaction, MessageModel _messageModel)
        {
            var userAuthenticate = await _context.Users.FirstOrDefaultAsync(x => x.Username == username);

            BankAccounts bankAccounts = null;

            if (currentUser.HasClaim(c => c.Type == "Roles"))
            {
                if (userAuthenticate != null)
                {
                    bankAccounts = await _context.BankAccounts.FirstOrDefaultAsync(x => x.UserId == userAuthenticate.Id);
                }
                else
                {
                    _messageModel.Message = "User not found!";
                    return StatusCode(404, _messageModel);
                }

                if (bankAccounts != null)
                {
                    if (ValidateDepositAmountBankAccount(amount) && ValidateBankAccount(bankAccounts, amount) && ValidateMinAmount(bankAccount, amount))
                    {
                        bankAccounts.Amount = bankAccounts.Amount - amount;
                        Transactions transactions = new Transactions();
                        transactions.SenderAccountInfo = bankAccount.Iban;
                        transactions.RecieverAccountInfo = reciever;
                        await _transaction.CreateTransaction(currentUser, amount, transactions, "BankAccount", reciever, $"Withdrawing {amount} lv", _context, _messageModel);
                        await _context.SaveChangesAsync();
                        _messageModel.Message = $"Succesfully withdrawed {amount} lv.";
                        return StatusCode(200, _messageModel);
                    }
                    else if (ValidateDepositAmountBankAccount(amount) == false)
                    {
                        _messageModel.Message = "Invalid payment amount!";
                        return StatusCode(400, _messageModel);
                    }
                    else if (ValidateBankAccount(bankAccounts, amount) == false)
                    {
                        _messageModel.Message = "You don't have enough money in bank account!";
                        return StatusCode(406, _messageModel);
                    }
                    else if (ValidateMinAmount(bankAccounts, amount) == false)
                    {
                        _messageModel.Message = "Min amount is 10 lv!";
                        return StatusCode(406, _messageModel);
                    }

                }
                else
                {
                    _messageModel.Message = "BankAccount not found";
                    return StatusCode(404, _messageModel);
                }
            }
            _messageModel.Message = "You are not autorized to do such actions!";
            return StatusCode(403, _messageModel);
        }
        public async Task<ActionResult<MessageModel>> DeleteBankAccount(ClaimsPrincipal currentUser, string username, BankSystemContext _context, MessageModel messageModel)
        {
            string role = "";

            if (currentUser.HasClaim(c => c.Type == "Roles"))
            {
                string userRole = currentUser.Claims.FirstOrDefault(currentUser => currentUser.Type == "Roles").Value;
                role = userRole;
            }

            if (role == "Admin")
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == username);
                BankAccounts bankAccountExists = null;
                Cards cardExists = null;
                Credits creditExists = null;

                if (user != null)
                {
                    bankAccountExists = await _context.BankAccounts.FirstOrDefaultAsync(x => x.UserId == user.Id);
                    cardExists = await _context.Cards.FirstOrDefaultAsync(x => x.BankAccountId == bankAccountExists.Id);
                    creditExists = await _context.Credits.FirstOrDefaultAsync(x => x.UserId == user.Id);
                }

                if (user == null)
                {
                    messageModel.Message = "User not found!";
                    return StatusCode(404, messageModel);
                }
                else if (bankAccountExists == null)
                {
                    messageModel.Message = "User doesn't have a bank account!";
                    return StatusCode(400, messageModel);
                }
                else if (cardExists == null)
                {
                    messageModel.Message = "No debit card found!";
                    return StatusCode(400, messageModel);
                }
                else if (creditExists != null)
                {
                    messageModel.Message = "You can't delete bank account if you have an existing credit!";
                    return StatusCode(406, messageModel);
                }
                else
                {
                    _context.Cards.Remove(cardExists);
                    _context.BankAccounts.Remove(bankAccountExists);
                    await _context.SaveChangesAsync();

                    messageModel.Message = $"Succsesfully deleted {user.Username} bank account and debit card!";
                    return StatusCode(200, messageModel);
                }
            }
            else
            {
                messageModel.Message = "You are not authorized to do such actions";
                return StatusCode(403, messageModel);
            }
        }
        private bool ValidateBankAccount(BankAccounts bankAccounts)
        {
            if (bankAccounts.Amount < 0)
            {
                return false;
            }
            return true;
        }
        private bool ValidateDepositAmountBankAccount(decimal amount)
        {
            if (amount > 0)
            {
                return true;
            }

            return false;
        }

        private bool ValidateMinAmount(BankAccounts bankAccounts, decimal amount)
        {
            if (amount >= 10 && amount <= bankAccounts.Amount)
            {
                return true;
            }

            return false;
        }
        private bool ValidateBankAccount(BankAccounts bankAccount, decimal amount)
        {
            if (bankAccount != null && bankAccount.Amount > amount)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool ValidateUser(Users user)
        {
            if (user != null)
            {
                return true;
            }
            return false;
        }
    }
}
