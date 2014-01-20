using System;
using Microsoft.WindowsAzure.Storage.Auth;
using SeekU.Commanding;

namespace SeekU.Azure.Commanding
{
    public class AzureCommandBus : ICommandBus
    {
        //private static StorageCredentials credentials;

        static AzureCommandBus()
        {
            //credentials = new StorageCredentials();
        }

        public void Send<T>(T command) where T : ICommand
        {
            //var client = new Microsoft.WindowsAzure.Storage.Blob.CloudBlobClient(new Uri(""), )
        }
    }
}
