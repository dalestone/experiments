using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace WebApplication1.Controllers
{
    //http://engrmosaic.uncc.edu/mosaic-anywhere/remote-desktop-file-format
    [Route("api/remotedesktop")]
    public class RemoteDesktopController : Controller
    {
        private readonly string rdpFilePath = string.Empty;

        public RemoteDesktopController(IHostingEnvironment env)
        {
            rdpFilePath = System.IO.Path.Combine(env.ContentRootPath, "Templates", "Default.rdp");
        }

        /// <summary>
        /// Get a rdp file
        /// </summary>
        /// <returns></returns>
        // GET: api/remotedesktop
        [HttpGet]
        public FileResult Get()
        {
            var parameters = SetRDPParameters();
            var rdp = CreateRDPFile(parameters);

            HttpContext.Response.ContentType = "application/rdp";
            FileContentResult result = new FileContentResult(rdp.ToArray(), "application/rdp")
            {
                FileDownloadName = "default.rdp"
            };

            return result;
        }

        /// <summary>
        /// Set rdp parameters
        /// </summary>
        /// <returns></returns>
        private List<RemoteDesktopParameter> SetRDPParameters()
        {
            var parameters = CreateRDPParametersList();
            var fullAddress = parameters.Where(p => p.ParameterName == "full address").FirstOrDefault();

            if (fullAddress != null)
            {
                fullAddress.ParameterValue = "8.8.8.8";
            }

            return parameters;
        }

        /// <summary>
        /// Create a dictionary of rdp properties
        /// </summary>
        /// <returns></returns>
        private List<RemoteDesktopParameter> CreateRDPParametersList()
        {
            var lines = System.IO.File.ReadAllLines(rdpFilePath);
            var parameters = new List<RemoteDesktopParameter>();
        
            if (lines.Any())
            {
                for (var i = 0; i < lines.Length; i++)
                {
                    var parameter = lines[i].Split(':');

                    parameters.Add(new RemoteDesktopParameter
                    {
                        //Index = i,
                        ParameterName = parameter[0],
                        ParameterType = parameter[1],
                        ParameterValue = parameter[2]
                    });
                }
            }

            return parameters;
        }        

        /// <summary>
        /// Create a rdp file memory stream
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private MemoryStream CreateRDPFile(List<RemoteDesktopParameter> parameters)
        {
            //Initialize in memory text writer
            MemoryStream ms = new MemoryStream();
            TextWriter tw = new StreamWriter(ms);

            foreach (var parameter in parameters)
            {
                tw.WriteLine($"{parameter.ParameterName}:{parameter.ParameterType}:{parameter.ParameterValue}");
            }

            return ms;
        }
    }

    public class RemoteDesktopParameter
    {
        public string ParameterName { get; set; }
        public string ParameterType { get; set; }
        public string ParameterValue { get; set; }
    }
}
