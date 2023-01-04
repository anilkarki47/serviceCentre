using System;
using System.ComponentModel.DataAnnotations;

namespace BikeServiceCentre.Data
{
    public class ItemModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string ItemName { get; set; }
        public string Quantity { get; set; }
        public Guid CreatedBy { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;

    }
}