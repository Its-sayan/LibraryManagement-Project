using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;

namespace LibraryManagementSystem.Controllers
{
    public class TransactionsController : Controller
    {
        private readonly LibraryManagementContext _context;
        private const decimal FinePerDay = 1.00m;

        public TransactionsController(LibraryManagementContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Issue(int bookId)
        {
            var memberIdString = HttpContext.Session.GetString("UserSession");
            Console.WriteLine("UserSession: " + memberIdString);
            if (string.IsNullOrEmpty(memberIdString) || !int.TryParse(memberIdString, out int memberId))
            { 
                return RedirectToAction("Login", "Members");
            }
            var transaction = new Transaction
            {
                BookId = bookId,
                MemberId = memberId,
                IssueDate = DateOnly.FromDateTime(DateTime.Now),
                ReturnDate = null,
                Fine = 0 
            };
            _context.Add(transaction); 
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Transactions");
        }


        // GET: Transactions
        public async Task<IActionResult> Index()
        {
            var transactions = await _context.Transactions
                .Include(t => t.Member)
                .Include(t => t.Book)
                .ToListAsync();
            foreach (var transaction in transactions) 
            {
                Console.WriteLine($"Transaction ID: " +
                    $"{transaction.TransactionId}," +
                    $" ReturnDate: " +
                    $"{transaction.ReturnDate}");
            }
            return View(transactions);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Return(int transactionId)
        {
            var transaction = await _context.Transactions.FindAsync(transactionId);
            if (transaction != null && !transaction.ReturnDate.HasValue)
            {
                transaction.ReturnDate = DateOnly.FromDateTime(DateTime.Now);  
                DateTime dueDate = transaction.IssueDate.ToDateTime(TimeOnly.MinValue).AddDays(14);
                DateTime returnDate = transaction.ReturnDate.Value.ToDateTime(TimeOnly.MinValue);
                if (returnDate > dueDate)
                {
                    int daysLate = (returnDate - dueDate).Days;
                    transaction.Fine = daysLate * FinePerDay;
                }
                _context.Update(transaction); 
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
       
        public async Task<IActionResult> Clean()
        {
            var transactions = _context.Transactions.ToList();
            if (transactions.Any())
            {
                _context.Transactions.RemoveRange(transactions);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }


        private bool TransactionExists(int id)
        {
            return _context.Transactions.Any(e => e.TransactionId == id);
        }
    }
}
