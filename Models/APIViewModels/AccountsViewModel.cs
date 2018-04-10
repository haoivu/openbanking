using System.Collections.Generic;
using System;
using openbanking.Models;

namespace openbanking.Models.APIViewModels
{
    public class AccountsModel
    {
        public AccountsModel() {}

        public AccountsModel(   int _id, string country, string currency, string ownerName,
                                string product, string accountType, double availableBalance, double bookedBalance, double valueDatedBalance)
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
        public int Id { get; set; }
        public string Country { get; set; }
        public string Currency { get; set; }
        public string OwnerName { get; set; }
        public string Product { get; set; }
        public string AccountType { get; set; }
        public double AvailableBalance { get; set; }
        public double BookedBalance { get; set; }
        public double ValueDatedBalance { get; set; }
        
    }

    // public class AccountsModel
    // {
    //     public AccountsModel() {}

    //     public int _id;
    //     public string country;
    //     public AccountNumber accountNumber;
    //     public string currency;
    //     public string ownerName;
    //     public string product;
    //     public string accountType; // Kan være greit med enum her og
    //     public double availableBalance;
    //     public double bookedBalance;
    //     public double valueDatedBalance;
    //     public AccountLink[] accountLink;

    //     public class AccountNumber {
    //         public string value;
    //         public string _type; //Alternativt enums
    //     }

    //     public class AccountLink {
    //         public string rel;
    //         public string href;
    //     }
    // }
}