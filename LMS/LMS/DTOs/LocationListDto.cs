namespace LMS.DTOs
{
    public class LocationListDto
    {
        public string CupboardId { get; set; }
        public string CupboardName { get; set; }
        public List<string> ShelfNo { get; set;}
        public int count { get; set; }
    }

    public class LocationDto
    {
       public string CupboardName { get; set; }
    }
}
