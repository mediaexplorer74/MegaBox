namespace CG.Web.MegaApiClient
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using StorageEverywhere; // !

    public static class PCLHelper
    {

        // GetRootFolder
        public static string GetStorageFolder()
        {
            // get hold of the file system
            IFolder folder = FileSystem.Current.LocalStorage;

            string res = folder.Path;

            return res;
        }

       
    }// PCLHelper class end

}
