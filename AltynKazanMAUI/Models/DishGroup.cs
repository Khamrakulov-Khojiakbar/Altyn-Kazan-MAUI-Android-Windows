using System;
using System.Collections.Generic;
using System.Text;

namespace AltynKazanMAUI.Models;

public class DishGroup : List<Dish>
{
    public string CategoryName { get; private set; }

    public DishGroup(string name, List<Dish> dishes) : base(dishes)
    {
        CategoryName = name;
    }
}
