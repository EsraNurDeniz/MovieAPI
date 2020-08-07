using Microsoft.AspNetCore.Http;
using System;
namespace MovieApi.Models{
    public class MovieItem
    {
        public int id { get; set; }
        public string name { get; set; }
        public int year { get; set; }
        public IFormFile posterFile { get; set;}
        public byte[] poster { get; set;}
        public int posterSize {get; set;}

        public string getPoster()
        {
            if(posterSize > 0){
                return Convert.ToBase64String(poster);
            }
            return null;
        }
        public bool isNullImage()
        {
            if(posterSize > 0)
            {
                return false;
            }
            return true;
        }

    }
}