using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.Filters;
using WebAPI.Models;

namespace WebAPI.MyControllers
{
    //[RequestAuthorize] //票据验证特征
    public class FoodController : ApiController
    {
        [HttpGet]
        [CrossSite]
        public Food GetFoodByID(string ID)
        {
            Food food = new Food();
            food.ID = ID;
            food.Name = ID + "番茄炒蛋";
            food.Introduce = "这是最新菜肴";
            food.Price = 10;
            food.Grade = "A+";
            return food;
        }

        [HttpGet]
        [CrossSite]
        public HttpResponseMessage GetFood()
        {
            Food food = new Food();
            food.ID = "234";
            food.Name = "番茄炒蛋";
            food.Introduce = "这是最新菜肴";
            food.Price = 10;
            food.Grade = "A+";

           string str =  Jil.JSON.Serialize<Food>(food);
          
            HttpResponseMessage result = new HttpResponseMessage { Content = new StringContent(str, Encoding.GetEncoding("UTF-8"), "application/json") };

            return result;
        }



        [HttpGet]
        [CrossSite]
        public HttpResponseMessage  GetAllFood()
        {
            List<Food> foods = new List<Food>();

            for (int i = 0; i < 9999; i++)
            {
                Food food = new Food();
                food.ID = i.ToString();
                food.Name = i.ToString() + "番茄炒蛋";
                food.Introduce = "这是最新菜肴";
                food.Price = 10;
                food.Grade = "A+";
                foods.Add(food);
            }

            string str = Jil.JSON.Serialize<List<Food>>(foods);
          
            HttpResponseMessage result = new HttpResponseMessage { Content = new StringContent(str, Encoding.GetEncoding("UTF-8"), "application/json") };

            return result;
        }

    }

   
}
