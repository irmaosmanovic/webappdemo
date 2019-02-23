using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebApplicationDemo.Models;

namespace WebApplicationDemo.Controllers
{
    [Authorize]//make allways authorize request
    public class FacebookController : Controller
    {
        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        // GET: Facebook
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Get last 6 posts from facebook
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> PostsView()
        {
            var currentClaims = await UserManager.GetClaimsAsync(HttpContext.User.Identity.GetUserId());

            var accessToken = currentClaims.FirstOrDefault(x => x.Type == "urn:tokens:facebook");

            if(accessToken == null)
            {
                return (new HttpStatusCodeResult(HttpStatusCode.NotFound, "FB token not exists!"));
            }

            string url = String.Format("https://graph.facebook.com/me/feed?fields=created_time,attachments&limit=6&access_token={0}", accessToken.Value);
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "GET";
            using (HttpWebResponse response = await request.GetResponseAsync() as HttpWebResponse)
            {
                StreamReader reader  = new StreamReader(response.GetResponseStream());

                string result = await reader.ReadToEndAsync();

                dynamic jsonObj = System.Web.Helpers.Json.Decode(result);
             
                ViewBag.JSON = result;

                //FacebookPostModels fbPosts = new JavaScriptSerializer().Deserialize<FacebookPostModels>(result);
                FacebookPostModels fbPosts = JsonConvert.DeserializeObject<FacebookPostModels>(result.ToString());

                return View(fbPosts);

            }

                

        }
        /// <summary>
        /// Get profile data from facebook API
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> ProfileView()
        {
            var currentClaims = await UserManager.GetClaimsAsync(HttpContext.User.Identity.GetUserId());

            var accessToken = currentClaims.FirstOrDefault(x => x.Type == "urn:tokens:facebook");

            if (accessToken == null)
            {
                return (new HttpStatusCodeResult(HttpStatusCode.NotFound, "FB token not exists!"));
            }

            string url = String.Format("https://graph.facebook.com/me?fields=email,first_name,last_name&access_token={0}", accessToken.Value);
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "GET";
            using (HttpWebResponse response = await request.GetResponseAsync() as HttpWebResponse)
            {
                StreamReader reader = new StreamReader(response.GetResponseStream());

                string result = await reader.ReadToEndAsync();

                dynamic jsonObj = System.Web.Helpers.Json.Decode(result);            
                ViewBag.JSON = result;

                //FacebookPostModels fbPosts = new JavaScriptSerializer().Deserialize<FacebookPostModels>(result);
                FacebookProfileModels fbProfile = JsonConvert.DeserializeObject<FacebookProfileModels>(result.ToString());

                return View(fbProfile);
            }          


        }

        /// <summary>
        /// Save json to file on disk with specified path
        /// </summary>
        /// <param name="profileData"></param>
        /// <returns></returns>
        public ActionResult SaveProfile(FacebookProfileModels profileData)
        {
          
            string path = @"C:\Users\Public\profile.txt";
            var json = JsonConvert.SerializeObject(profileData);

            //Create file if not exist and overwrite if does
            if (!System.IO.File.Exists(path))
            {
                System.IO.File.Create(path);
                TextWriter tw = new StreamWriter(path);
                tw.WriteLine(json);
                tw.Close();
            }
            else
            {
                TextWriter tw = new StreamWriter(path);
                tw.WriteLine(json);
                tw.Close();
            }

            return View();

        }
    }
}