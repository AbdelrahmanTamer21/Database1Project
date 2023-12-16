using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseProject.Models
{
    public class Advisors_Graduation_Plan
    {

        public GraduationPlan graduationPlan { get; set; }
        public Advisor advisor { get; set; }


        public Advisors_Graduation_Plan() { }

        public Advisors_Graduation_Plan(GraduationPlan graduationPlan, Advisor advisor)
        {
            this.graduationPlan = graduationPlan;
            this.advisor = advisor;
        }
    }
}