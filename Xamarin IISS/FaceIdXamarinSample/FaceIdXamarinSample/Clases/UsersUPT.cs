using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;
using System;

namespace FaceIdXamarinSample.Clases
{
    public class UsersUPT
    {
        [JsonProperty(PropertyName = "PID")]
        public string PID { get; set; }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "nombre")]
        public string nombre { get; set; }

        [JsonProperty(PropertyName = "descripcion")]
        public string descripcion { get; set; }

        [JsonProperty(PropertyName = "edad")]
        public int edad { get; set; }
    }
}
