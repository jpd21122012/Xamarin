using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Sync;

namespace FaceIdXamarinSample.Clases
{
    class TodoItemManager
    {

        private TodoItemManager()
        {
            client = new MobileServiceClient("https://EnigmaMx0.azurewebsites.net");
            userTableObj = client.GetTable<UsersUPT>();
        }

        private static TodoItemManager defaultInstance = null;
    private MobileServiceClient client;

    IMobileServiceTable<UsersUPT> userTableObj;
    public static TodoItemManager DefaultManager
    {
        get
        {
            if (defaultInstance == null)
            {
                defaultInstance = new TodoItemManager();
            }
            return defaultInstance;
        }
    }

    public MobileServiceClient CurrentClient
    {
        get { return client; }
    }

    public bool IsOfflineEnabled
    {
        get { return userTableObj is IMobileServiceSyncTable<UsersUPT>; }
    }

    public async Task<ObservableCollection<UsersUPT>> GetTodoItemsAsync(string idBuscar)
        {
            UsersUPT u = new UsersUPT();
            try
            {
                IEnumerable<UsersUPT> items =  await userTableObj.Where(userTableObj => userTableObj.PID == idBuscar).ToEnumerableAsync();
                return new ObservableCollection<UsersUPT>(items);
            }
            catch (MobileServiceInvalidOperationException msioe)
            {
                Debug.WriteLine($"Invalid sync operation: {msioe.Message}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Sync Error: {ex.Message}");
            }
            return null;
        }

    public async Task SaveTaskAsync(UsersUPT item)
    {
        if (item.Id == null)
        {
            await userTableObj.InsertAsync(item);
        }
        else
        {
            await userTableObj.UpdateAsync(item);
        }
    }

}
}
