using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Models
{
    // When user want to add any Item in the shopping cart, we need to keep that data in the database so that if the user will enter in the cart after 5days, he can find that. So we need to keep 3 properties in the database. They are: UserId, MenuitemId, Count of items.

    public class ShoppingCart
    {
        public ShoppingCart()
        {
            Count = 1;
        }
        public int Id { get; set; }
        public string ApplicationUserID { get; set; }

        [NotMapped]
        [ForeignKey("ApplicationUserID")]
        public virtual ApplicationUser ApplicationUser { get; set; }

        public int MenuItemId { get; set; }

        [NotMapped]
        [ForeignKey("MenuItemId")]
        public virtual MenuItem MenuItem { get; set; }

        [Range(1,int.MaxValue,ErrorMessage = "Value Should be 1 or more than 1")]
        public int Count { get; set; }
    }
}
