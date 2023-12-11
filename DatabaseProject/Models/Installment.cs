namespace DatabaseProject.Models
{
    public class Installment
    {

        public int payment_id { get; set; }
        public string startdate { get; set; }
        public string deadline { get; set; }
        public int amount { get; set; }
        public string status { get; set; }

        public Installment() { }
    }
}