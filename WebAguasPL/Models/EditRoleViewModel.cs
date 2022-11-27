using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebAguasPL.Models
{
    public class EditRoleViewModel
    {
        public string ID { get; set; }

        [Display(Name = "Role")]
        //[Range(1, int.MaxValue, ErrorMessage = "You must select a role.")]
        public string RoleId { get; set; }

        public IEnumerable<SelectListItem> roles { get; set; }
    }
}
