﻿namespace LMS.DTOs
{
    public class ReturnBookDto
    {
        public int reservationNo {  get; set; }
        public string returnby { get; set; }
        public int penalty {  get; set; }
        public string returnDate { get; set; }
        public bool email {  get; set; }
    }
}
