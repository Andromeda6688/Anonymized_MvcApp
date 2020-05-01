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

        
        [Required(ErrorMessage = "Обязательное поле")]
        [StringLength(60, ErrorMessage = "Длина не более 60 символов")]
        public string Title;


        [StringLength(160, ErrorMessage = "Длина не более 160 символов")]
        public string Description;


        [StringLength(160, ErrorMessage = "Длина не более 160 символов")]
        public string Keywords;

        [AllowHtml]
        [UIHint("tinymce_full")]
        public string Content { get; set; }
        
        public int ParentId;
        
        [Required (ErrorMessage="Обязательнео поле")]        
        [RegularExpression("^[A-Za-z0-9]+$",  
            ErrorMessage = "Допускаются только латинские буквы и цифры")]
        public string Address;

        
        public bool IsVisible;

        
        [RegularExpression("^[0-9]+$",
            ErrorMessage = "Допускается только число")]
        public System.Nullable<int> DisplayOrder;
    }
}