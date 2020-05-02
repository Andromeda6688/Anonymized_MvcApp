using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcApplication1.Global
{
    public class PageValidation
    {
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }

        
        [Required]
        [StringLength(60)]
        public string Title;


        [StringLength(160)]
        public string Description;


        [StringLength(160)]
        public string Keywords;

        [AllowHtml]
        [UIHint("tinymce_full")]
        public string Content { get; set; }
        
        public int ParentId;
        
        [Required]        
        [RegularExpression("^[A-Za-z0-9]+$",  
            ErrorMessage = "Only latin letters and digits are allowed")]
        public string Address;

        
        public bool IsVisible;

        
        [RegularExpression("^[0-9]+$",
            ErrorMessage = "Only digits are allowed")]
        public System.Nullable<int> DisplayOrder;
    }
}