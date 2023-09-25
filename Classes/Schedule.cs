﻿namespace Classes
{
    public class Schedule
    {
        public int Id { get; set; }
        public int LocalityId { get; set; }
        public Locality Locality { get; set; }
        public int TaskMonthId { get; set; }
        public TaskMonth TaskMonth { get; set; }
        public DateTime DateApproval { get; set; }
    }
}
