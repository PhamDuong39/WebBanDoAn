﻿namespace WebBanDoAn.ViewModels.OrderModels
{
    public class PaymentInformationModel
    {
        public string OrderType { get; set; }
        public double Amount { get; set; }
        public string OrderDescription { get; set; }
        public string Name { get; set; }


        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public int PaymentId { get; set; }
    }
}
