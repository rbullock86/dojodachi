using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Dojodachi.Controllers
{

    public static class SessionExtensions
    {
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }
        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            string value = session.GetString(key);
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
    }

    public class DojodachiController : Controller
    {
        Random Rand = new Random();


        [HttpGet]
        [Route("Dojodachi")]
        public IActionResult Index()
        {
            Console.WriteLine("Hitting Index");
            // winning condition.
            if( HttpContext.Session.GetInt32("Fullness") >= 100 &&
                HttpContext.Session.GetInt32("Happiness") >= 100 &&
                HttpContext.Session.GetInt32("Energy") >= 100 )
            {
                HttpContext.Session.SetInt32("GameState", 0) ;
                HttpContext.Session.SetString("Message", "Congratulations! You win.");
            }

            // losing condition
            else if( HttpContext.Session.GetInt32("Fullness") <= 0 ||
                HttpContext.Session.GetInt32("Happiness") <= 0 ||
                HttpContext.Session.GetInt32("Energy") <= 0 )
            {
                HttpContext.Session.SetInt32("GameState", 0) ;
                HttpContext.Session.SetString("Message", "Congratulations! You suck.");
            }

            // new Session condition
            if(HttpContext.Session.GetInt32("Fullness") == null){
                BuildSession();
            }

            return View("index");
        }
        

        // redirect

        // feed
        [HttpGet("Dojodachi/feed")]
        public IActionResult Feed()
        {
            Console.WriteLine("Feeding Dojodachi");

            // Shouldn't Actually fire since button is disabled
            if(HttpContext.Session.GetInt32("Meals") <= 0)
            {
                HttpContext.Session.SetString("Message", "How did you feed it if there wasn't any food....");
            }

            // Every case there is a meal available
            else
            {
                HttpContext.Session.SetInt32("Meals", HttpContext.Session.GetInt32("Meals").GetValueOrDefault() - 1);

                // Dachimood randomizng plus conditionals
                int dachiMood = Rand.Next(1, 5);
                if(dachiMood != 1)
                {
                    int? nFullness = HttpContext.Session.GetInt32("Fullness");
                    HttpContext.Session.SetInt32("Fullness", nFullness.GetValueOrDefault() + Rand.Next(5, 11));
                    HttpContext.Session.SetString("Message", $"Your dachi gained {HttpContext.Session.GetInt32("Fullness") - nFullness.GetValueOrDefault()} fullness!");
                }
                else
                {
                    HttpContext.Session.SetString("Message", $"Your dachi does not like your food!");
                }
            }
            return RedirectToAction("Index");
        }

        // play
        [HttpGet("Dojodachi/play")]
        public IActionResult Play()
        {
            Console.WriteLine("Playing with Dojodachi");
            int dachiMood = Rand.Next(1, 5);
            int? nHappiness = HttpContext.Session.GetInt32("Happiness");
            HttpContext.Session.SetInt32("Energy", HttpContext.Session.GetInt32("Energy").GetValueOrDefault() - 5);
            // randomize moods and act accordingly
            if (dachiMood == 1)
            {
                HttpContext.Session.SetString("Message", $"You played with dachi but it did not care for it!");
            }
            else
            {
                HttpContext.Session.SetInt32("Happiness", HttpContext.Session.GetInt32("Happiness").GetValueOrDefault() + Rand.Next(5,11));
                HttpContext.Session.SetString("Message", $"You played with dachi gaining it {HttpContext.Session.GetInt32("Happiness").GetValueOrDefault() - nHappiness}.");
            }
            return RedirectToAction("Index");
        }

        // work
        [HttpGet("Dojodachi/work")]
        public IActionResult Work()
        {
            Console.WriteLine("Dojodachi is working");
            
            HttpContext.Session.SetInt32("Energy", HttpContext.Session.GetInt32("Energy").GetValueOrDefault() - 5);
            int? nMeals = HttpContext.Session.GetInt32("Meals");
            HttpContext.Session.SetInt32("Meals", HttpContext.Session.GetInt32("Meals").GetValueOrDefault() + Rand.Next(1,4));
            HttpContext.Session.SetString("Message", $"Dachi worked and gained {HttpContext.Session.GetInt32("Meals").GetValueOrDefault() -nMeals}.");

            return RedirectToAction("Index");
        }

        // sleep
        [HttpGet("Dojodachi/sleep")]
        public IActionResult Sleep()
        {
            Console.WriteLine("Dojodachi is Sleeping");
            
            HttpContext.Session.SetInt32("Energy", HttpContext.Session.GetInt32("Energy").GetValueOrDefault() + 15);
            HttpContext.Session.SetInt32("Fullness", HttpContext.Session.GetInt32("Fullness").GetValueOrDefault() - 5);
            HttpContext.Session.SetInt32("Happiness", HttpContext.Session.GetInt32("Happiness").GetValueOrDefault() - 5);
            HttpContext.Session.SetString("Message", "Dachi had a good night's rest!");
            
            return RedirectToAction("Index");
        }

        // restart
        [HttpGet("Dojodachi/restart")]
        public IActionResult Restart()
        {
            Console.WriteLine("Restarting Dojodachi");
            BuildSession();
            return RedirectToAction("Index");
        }


        public void BuildSession()
        {
            Console.WriteLine("Building New Session.");
            HttpContext.Session.SetInt32("Happiness", 20);
            HttpContext.Session.SetInt32("Fullness", 20);
            HttpContext.Session.SetInt32("Energy", 20);
            HttpContext.Session.SetInt32("Meals", 3);
            HttpContext.Session.SetInt32("GameState", 1);
            HttpContext.Session.SetString("Message", "Welcome to Dojodachi. Good luck.");
        }
    }
}