using System.Threading.Tasks;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using openbanking.Data;
using openbanking.Models;
using System;
using System.Linq;
using System.Collections.Generic;

namespace openbanking.ViewComponents
{
    // Controller for the view component that shows a list of recent articles
    public class AccountList : ViewComponent
    {
        private ApplicationDbContext _db;

        public AccountList(ApplicationDbContext db)
        {
            _db = db;
        
        }

        // This function fetches data for the view component.
        // It is an async function so it can run in parallell with other async functions.
        public async Task<IViewComponentResult> InvokeAsync()
        {

            var list = await _db.AccountList.ToListAsync();
        
            //list.Sort((x, y) => DateTime.Compare(x.PostedTime, y.PostedTime));
            list.Reverse();
            return View(list);
        }
    }
}