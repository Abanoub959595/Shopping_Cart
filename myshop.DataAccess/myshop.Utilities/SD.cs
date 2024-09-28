using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myshop.Utilities
{
    public static class SD
    {
        public static string AdminRole = "Admin";
        public static string EditorRole = "Editor";
        public static string CustomerRole = "Customer";

        public static string Pending { get; set; } = "Pending";
        public static string Approve { get; set; } = "Approved";
        public static string Proccessing { get; set; } = "Proccessing";
        public static string Cancelled { get; set; } = "Cancelled";
        public static string Shipped { get; set; } = "Shipped";
        public static string Refund { get; set; } = "Refund";
        public static string Rejected { get; set; } = "Rejected";
        public static string SessionKey { get; set; } = "ShoppingCartSession";






    }
}
