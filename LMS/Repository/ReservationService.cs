﻿using LMS.DTOs;
using LMS.EmailTemplates;
using LMS.Helpers;
using LMS.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.VisualBasic;
using Org.BouncyCastle.Tls;
using System.Linq;
using System.Runtime.InteropServices;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LMS.Repository
{
    public class ReservationService : IReservationService
    {

        private readonly DataContext _Context;
        private readonly INotificationService _notificationService;
        private readonly IEmailService _emailService;
        private readonly JWTService _jwtService;

        //Contructor of the ReservationService
        public ReservationService(DataContext Context, INotificationService notificationService, IEmailService emailService, JWTService jwtService)
        {
            _Context = Context;
            _notificationService = notificationService;
            _emailService = emailService;
            _jwtService = jwtService;
        }


        public async Task<IssueBookFormResponseDto> LoadIssueForm(string isbn)
        {

            var resource = await _Context.Resources.FirstOrDefaultAsync(u => u.ISBN == isbn);

            if (resource == null)  //Check is there any resource
            {
                throw new Exception("ResourceNotFound");
            }
            else //If Resource found pass data to port
            {
                var resourcedto=new IssueBookFormResponseDto
                {
                    ISBN=resource.ISBN,
                    URL = resource.ImageURL,
                };
                return resourcedto;
            }
        }
        public async Task<IssueBookResponseDto> IssueBook(IssueBookRequestDto request,HttpContext httpContext)
        {

            var resource = await _Context.Resources.FirstOrDefaultAsync(u => u.ISBN == request.ISBN);
            var borrower = await _Context.Users.FirstOrDefaultAsync(u => u.UserName == request.BorrowerID);
            
            //Decrease Quantity of resource by 1
            resource.Quantity = resource.Quantity - 1;
            resource.Borrowed=resource.Borrowed + 1;
            await _Context.SaveChangesAsync();


            if (resource.Quantity < 0) //If not enough resources
            {
                resource.Quantity = resource.Quantity + 1;
                resource.Borrowed = resource.Borrowed - 1;
                await _Context.SaveChangesAsync();
                throw new Exception("No of Books not enough");
            }
            else if (borrower.Status == "Loan") //If User in a loan
            {
                resource.Quantity = resource.Quantity + 1;
                resource.Borrowed = resource.Borrowed - 1;
                await _Context.SaveChangesAsync();
                throw new Exception("User in a loan");
            }
            else  //If User Can borrow the book
            {
                var issuer= _jwtService.GetUsername(httpContext);
                var reservation = new Reservation
                {
                    ReservationNo="123456",
                    ResourceId=request.ISBN,
                    BorrowerID = request.BorrowerID,
                    Status = "borrowed",
                    IssuedByID = issuer,
                    IssuedDate = DateOnly.FromDateTime(DateTime.Today),
                    DueDate = DateOnly.Parse(request.dueDate),
                    
                    
                };

                _Context.Reservations.Add(reservation);//Add the Reservation
                borrower.Status = "Loan";
                if (request.requestId != 0)
                {
                    var req=await _Context.Requests.FirstOrDefaultAsync(e => e.Id == request.requestId);
                    _Context.Requests.Remove(req);
                }
                await _Context.SaveChangesAsync();

                var responsefromform = new IssueBookResponseDto //Intialize response dto
                {
                    BorrowerId = reservation.BorrowerID,
                    ISBN = resource.ISBN,
                    ReservationId = reservation.Id,
                };
                await _notificationService.IssueNotification(reservation.Id);
                if(request.Email == true) //If Email is true
                {
                    try
                    {
                        var htmlBody = new EmailTemplate().IssueBookEmail(reservation,borrower.FName+" "+borrower.LName,resource.Title);
                        await _emailService.SendEmail(htmlBody, borrower.Email, "Successfully Issue the Book" + " " + reservation.ResourceId);
                    }
                    catch
                    {
                    }
                    }
                return responsefromform; // return response dto to port
            }

        }
        public async Task<AboutReservationDto> AboutReservation(int resId)
        {

            var reservation = await _Context.Reservations.FirstOrDefaultAsync(u => u.Id == resId);
            

            if (reservation == null)  //Check is there any resource
            {
                throw new Exception("ReservationNotFound");
            }
            else //If Resource found past data to port
            {
                var resource = await _Context.Resources.FirstOrDefaultAsync(u => u.ISBN == reservation.ResourceId);
                var reservationdto = new AboutReservationDto
                {
                    ResId = reservation.Id,
                    ISBN = reservation.ResourceId,
                    BookTitle = resource.Title,
                    UserName = reservation.BorrowerID,
                    DateIssue = reservation.IssuedDate,
                    DueDate = reservation.DueDate,
                    Issuer = reservation.IssuedByID,
                    ReturnDate = reservation.ReturnDate,
                    Status = reservation.Status,
                    ImagePath = resource.ImageURL

    };
                return reservationdto;
            }

        }
        public async Task<bool> ReturnBook(ReturnBookDto request, HttpContext httpContext)
        {
            var reservation= await _Context.Reservations.FirstOrDefaultAsync(u => u.Id == request.reservationNo);

            if (reservation == null)
            {
                throw new Exception("Reservation not found");
            }
            else
            {
                if (reservation.Status == "borrowed"|| reservation.Status == "overdue")
                {
                    var borrower = await  _Context.Users.FirstOrDefaultAsync(u => u.UserName == reservation.BorrowerID);
                    var resource = await  _Context.Resources.FirstOrDefaultAsync(u => u.ISBN == reservation.ResourceId);
                    borrower.Status = "free";
                    resource.Quantity = resource.Quantity + 1;
                    resource.Borrowed = resource.Borrowed - 1;
                    reservation.Status = "reserved";
                    reservation.ReturnDate = DateOnly.Parse(request.returnDate);
                   
                    await _Context.SaveChangesAsync();
                    await  _notificationService.IssueNotification(reservation.Id);

                    if (request.email == true) { 
                        try
                        {
                            
                            var htmlBody = new EmailTemplate().IssueBookEmail(reservation, borrower.FName + " " + borrower.LName, resource.Title);
                            await _emailService.SendEmail(htmlBody,borrower.Email,"Successfully Return the Book"+" "+reservation.ResourceId);
                            
                        }
                        catch
                        {
                            
                        }
                    }

                    return true;
                }
                else
                {
                    throw new Exception("Already Returned");
                }
            }
        }

        public async Task<List<ReservationDto>> SearchReservation(SearchDetails details,HttpContext httpContext)
        {
            var userName = _jwtService.GetUsername(httpContext);
            var userType = _jwtService.GetUserType(httpContext);


            var k = new List<Reservation>();

            if (userType == "admin")
                k = _Context.Reservations.ToList();
            if (userType == "patron")
                k = _Context.Reservations.Where(e => e.BorrowerID == userName).ToList();

            if (details.Keywords == "")
            {

            }
            else if (details.type=="all")
            {

                int number;
                bool success = int.TryParse(details.Keywords, out number);
                if (success)
                {

                    k = k.Where(e =>
                    e.BorrowerID.ToLower().Contains(details.Keywords.ToLower())
                    || e.ResourceId.ToLower().Contains(details.Keywords.ToLower())
                    || e.Id.Equals(number)).ToList();
                }
                else
                {
                    k = k.Where(e =>
                    e.BorrowerID.ToLower().Contains(details.Keywords.ToLower())
                    || e.ResourceId.ToLower().Contains(details.Keywords.ToLower())).ToList();
                }
            }
            else if (details.type=="userId")
            {
                k = k.Where(e => e.BorrowerID.ToLower().Contains(details.Keywords.ToLower())).ToList();
            }
            else if (details.type== "resourceId")
            {
                k = k.Where(e => e.ResourceId.ToLower().Contains(details.Keywords.ToLower())).ToList();
            }
            else if (details.type== "reservationId")
            {
                int number;
                bool success = int.TryParse(details.Keywords, out number);

                if (success)
                {
                    k = _Context.Reservations.Where(e => e.Id.Equals(number)).ToList();
                }
            }



            List<ReservationDto> reservationlist = new List<ReservationDto>();
            foreach(var x in k) {
                var userob = await _Context.Users.FirstOrDefaultAsync(u => u.UserName == x.BorrowerID);
                var res = new ReservationDto
                {
                    reservationNo = x.Id,
                    Resource=x.ResourceId,
                    BorrowerName=x.BorrowerID,
                    UserId=x.BorrowerID,
                    UserName=userob.FName+" "+ userob.LName,
                    DueDate=x.DueDate,
                    Status=x.Status//need to look due or not
                };
                reservationlist.Add(res);
            }
            return reservationlist;


        }

        public async Task<bool> deleteReservation(int id)
        {
            var reservation = await _Context.Reservations.FirstOrDefaultAsync(e => e.Id == id);
            if (reservation == null)
                return false;
            else
            {
                if (reservation.Status != "reserved")
                {
                    var user = await _Context.Users.FirstOrDefaultAsync(e => e.UserName == reservation.BorrowerID);
                    var resource = await _Context.Resources.FirstOrDefaultAsync(u => u.ISBN == reservation.ResourceId);
                    resource.Quantity = resource.Quantity + 1;
                    resource.Borrowed = resource.Borrowed - 1;
                    user.Status = "free";
                    await _Context.SaveChangesAsync();
                }
                _Context.Remove(reservation);
                await _Context.SaveChangesAsync();
                return true;
            }
                
        }

        public async Task<bool> extendDue(int id, string due)
        {
            var reservation = await _Context.Reservations.FirstOrDefaultAsync(e => e.Id == id);
            if(reservation == null)
            {
               return false;
            }
            else
            {
                reservation.DueDate = DateOnly.Parse(due);
                await _Context.SaveChangesAsync();
                return true;
            }

        }

        public async Task setOverdue()
        {
            var currentDate = DateOnly.FromDateTime(DateTime.Today);
            var overdueRecords = _Context.Reservations
               .Where(e => e.Status == "borrowed" && e.DueDate == currentDate)
               .ToList();

            foreach (var record in overdueRecords)
            {
                record.Status = "overdue";
                await _notificationService.SetRemind(record);
            }

            await _Context.SaveChangesAsync();

           
        }


    }
}
