using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebAPI.Models; 

namespace WebAPI.MyControllers
{
    public class FoodController : ApiController
    { 
        [HttpGet]
        [CrossSite]
        public Food GetFoodByID(string ID)
        {
            Food food = new Food();
            food.Name = ID + "番茄炒蛋";
            food.Introduce = "这是最新菜肴";
            return food;
        
        }

    }
}
