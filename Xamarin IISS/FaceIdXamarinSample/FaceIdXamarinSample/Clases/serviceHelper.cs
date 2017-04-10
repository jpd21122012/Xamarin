using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceIdXamarinSample.Clases
{
    class serviceHelper
    {
        MobileServiceClient clienteService = new MobileServiceClient("http://EnigmaMx0.azurewebsites.net");

        public IMobileServiceTable<UsersUPT> _UsersUPT;
        public string cad = "";

        public async Task BuscarRegistros(string nombre)
        {
            _UsersUPT = clienteService.GetTable<UsersUPT>();
            List<UsersUPT> items = await _UsersUPT.Where(
                item => item.PID == nombre).ToListAsync();
        }
    }
}
