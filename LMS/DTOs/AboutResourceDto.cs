﻿using Microsoft.Identity.Client;

namespace LMS.DTOs
{
    public class AboutResourceDto
    {
       
        public string ISBN { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
        public string Author {  get; set; }
        public int Remain {  get; set; }
        public int borrowed { get; set; }
        public int total {  get; set; }
        public string CupboardId {  get; set; }
        public int ShelfId { get; set; }
        public string Description { get; set; }
        public int pages {  get; set; }
        public double price { get; set; }
        public DateTime addedon {  get; set; }

        public string Imagepath { get; set;}



        


    }
}
