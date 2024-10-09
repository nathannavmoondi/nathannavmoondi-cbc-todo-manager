using System;
using System.ComponentModel.DataAnnotations;

namespace Blazor_Template_Models
{
    public class TodoDTO
    {
        public TodoDTO()
        {
           Id = Guid.NewGuid();
           Name = string.Empty;
           Details = string.Empty;        
        }
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Details{ get; set; }        
    }
}