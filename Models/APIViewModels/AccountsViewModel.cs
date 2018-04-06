using System.Collections.Generic;
using System;
using openbanking.Models;

namespace openbanking.Models.APIViewModels
{
    public class AccountsModel
    {
        public AccountsModel() {}

        public AccountsModel(   string _id, string country, string currency, string ownerName,
                                string product, string accountType, string availableBalance, string bookedBalance, string valueDatedBalance)
        {
            this.Id = _id;
            this.Country = country;
            this.Currency = currency;
            this.OwnerName = ownerName;
            this.Product = product;
            this.AccountType = accountType;
            this.AvailableBalance = availableBalance;
            this.BookedBalance = bookedBalance;
            this.ValueDatedBalance = valueDatedBalance;
        }
        public string Id { get; set; }
        public string Country { get; set; }
        public string Currency { get; set; }
        public string OwnerName { get; set; }
        public string Product { get; set; }
        public string AccountType { get; set; }
        public string AvailableBalance { get; set; }
        public string BookedBalance { get; set; }
        public string ValueDatedBalance { get; set; }
        
    }
}