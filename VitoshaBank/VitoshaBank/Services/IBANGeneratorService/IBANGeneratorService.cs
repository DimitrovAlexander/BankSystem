﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VitoshaBank.Services.IBANGeneratorService
{
    public static class IBANGeneratorService
    {
        //IBAN=BG 18 VITB 123456789 01 001
        

        public static string GenerateIBANInVitoshaBank(string BankAccountType) 
        {
            Bank
            string countryCode = "BG";
            string uniqueNumber = "18";
            string bankBIC = "VITB";
            string secondUniqueNumber = "123456789";
            string bankAccountTypeCode = "";
            string currentBankAccountNumber = "001";

            if (BankAccountType == "BankAccount")
            {
                bankAccountTypeCode = "01";
            }
            else if (BankAccountType == "Credit")
            {
                bankAccountTypeCode = "02";
            }
            else if (BankAccountType == "Deposit")
            {
                bankAccountTypeCode = "03";
            }
            else
            {
                throw new ArgumentException("Invalid BankAccount type!");
            }
            GetCurrentAvailabeAccountNumber(BankAccountType);
            string IBAN = $"{countryCode}{uniqueNumber}{bankBIC}{secondUniqueNumber}{bankAccountTypeCode}{currentBankAccountNumber}";
            return IBAN;
        }
        private static void GetCurrentAvailabeAccountNumber(string BankAccountType)
        {
            if (BankAccountType == "BankAccount")
            {
                
            }
            else if (BankAccountType == "Credit")
            {
                bankAccountTypeCode = "02";
            }
            else if (BankAccountType == "Deposit")
            {
                bankAccountTypeCode = "02";
            }
            else
            {
                throw new ArgumentException("Invalid BankAccount type!");
            }
        }
    }
}