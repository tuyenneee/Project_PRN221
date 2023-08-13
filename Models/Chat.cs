using System;
using System.Collections.Generic;

namespace WebRazor.Models
{
    public partial class Chat
    {
        public int Id { get; set; }
        public int? FromUser { get; set; }
        public int? ToUser { get; set; }
        public string? Message { get; set; }
        public DateTime? CreateDate { get; set; }
        public int? GroupId { get; set; }
        public int? AccountId { get; set; }

        public virtual Account? Account { get; set; }
    }
}
