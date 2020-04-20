using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MvcApplication1.Global
{
    public class PageValidation
    {
  
        public int Id;

        
        [Required(ErrorMessage = "Обязательное поле")]
        public string Title;

        
        [MaxLength(160, ErrorMessage="Длина не более 160 символов")]
        public string Description;

        
        [MaxLength(160, ErrorMessage="Длина не более 160 символов")]
        public string Keywords;

        
        public string Content;

        
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