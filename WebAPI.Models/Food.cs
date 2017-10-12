using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public class Food
    {
        /// <summary>
        /// ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 菜名
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Introduce   { get; set; }

        /// <summary>
        /// 评级
        /// </summary>
        public string Grade { get; set; }

        /// <summary>
        /// 图片路径
        /// </summary>
        public List<string> ImgFiles { get; set; }
        
        
    }
}
