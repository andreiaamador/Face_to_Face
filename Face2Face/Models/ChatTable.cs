//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Face2Face.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class ChatTable
    {
        public int EventID { get; set; }
        public int UserID { get; set; }
        public string ChatEntry { get; set; }
    
        public virtual EventTable EventTable { get; set; }
        public virtual UserProfile UserProfile { get; set; }
    }
}
