using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseProject.Models
{
    public class Payment
    {

        public int payment_id { get; set; }
        public int amount { get; set; }
        public DateTime startdate { get; set; }
        public DateTime deadline { get; set; }
        public int n_installments { get; set; }
        public decimal fund_percentage { get; set; }
        public string status { get; set; }
        public Student student { get; set; }
        public Semester semester { get; set; }


        public Payment() { }

        public Payment(int payment_id, int amount, DateTime startdate, DateTime deadline, int n_installments, decimal fund_percentage, string status, Student student, Semester semester)
        {
            this.payment_id = payment_id;
            this.amount = amount;
            this.startdate = startdate;
            this.deadline = deadline;
            this.n_installments = n_installments;
            this.fund_percentage = fund_percentage;
            this.status = status;
            this.student = student;
            this.semester = semester;
        }
    }
}