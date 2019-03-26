using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrdersAPI.Assistants
{
    public class DoesOrderContainItemsHelper
    {
        // Assist method to determine if order contained items 
        public bool DoesOrderContainItems(string sku, List<string> items)
        {
            // If SKU has 6 items assosiated
            if ((sku == "SKU000001") || (sku == "SKU000002") || (sku == "SKU000004"))
            {
                items.RemoveRange(items.Count - 2, 2);
                if (items.Any(o => o != null))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            // If SKU has 7 items assosiated
            /**
             * .....
             */
            // If SKU has 8 items assosiated
            else
            {
                if (items.Any(x => x != null))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
