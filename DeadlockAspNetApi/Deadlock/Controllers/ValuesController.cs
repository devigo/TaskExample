using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Deadlock.Controllers
{
    // More detail and based on this article https://blog.stephencleary.com/2012/07/dont-block-on-async-code.html

    public class ValuesController : ApiController
    {
        public string Get()
        {
            return "Hello World!";
        }

        public string Get(int id)
        {
            var googlePageTask = GetJsonAsync(new Uri("https://google.com"));
            return googlePageTask.Result;
        }

        public static async Task<string> GetJsonAsync(Uri uri)
        {
            // (real-world code shouldn't use HttpClient in a using block; this is just example code)
            using (var client = new HttpClient())
            {
                var responseString = await client.GetStringAsync(uri);
                return responseString;
            }
        }

        //1. The top-level method calls GetJsonAsync (within the UI/ASP.NET context).
        //2. GetJsonAsync starts the REST request by calling HttpClient.GetStringAsync (still within the context).
        //3. GetStringAsync returns an uncompleted Task, indicating the REST request is not complete.
        //4. GetJsonAsync awaits the Task returned by GetStringAsync. The context is captured and will be used to continue running the GetJsonAsync method later. GetJsonAsync returns an uncompleted Task, indicating that the GetJsonAsync method is not complete.
        //5. The top-level method synchronously blocks on the Task returned by GetJsonAsync. This blocks the context thread.
        //6. … Eventually, the REST request will complete. This completes the Task that was returned by GetStringAsync.
        //7. The continuation for GetJsonAsync is now ready to run, and it waits for the context to be available so it can execute in the context.
        //8. Deadlock. The top-level method is blocking the context thread, waiting for GetJsonAsync to complete, and GetJsonAsync is waiting for the context to be free so it can complete.
    }
}
