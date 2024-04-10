using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.System.Profile;
using Xamarin.Forms;

using System.ServiceModel;

using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

using static MegaBox.MainPage;
using Windows.Storage;

//[assembly: Dependency(typeof(MegaBox.UWP.DeviceInfo))]
[assembly: Dependency(typeof(MegaBox.UWP.MegaClient))]
namespace MegaBox.UWP
{

   
    // 3 IMegaClient "interface realization"
    public class MegaClient : IMegaClient
    {

        // CopyToImageFolderAndRun
        public async void CopyToImageFolderAndRun(string FullLSPath, string ShortFName)
        {
            
            if (FullLSPath == null) 
                return;

            // 1 Copy to Image folder                       
            StorageFolder folder = ApplicationData.Current.LocalFolder;//KnownFolders.VideosLibrary;
            IReadOnlyList<StorageFile> files = await folder.GetFilesAsync();

            // Пробегаемся по всем файлам в хранилище... ищем скачанный файлик
            // TODO: че-то придумать с избавлением от цикла!!!
            foreach (StorageFile ffile in files)
            {
                if (ffile.Path == FullLSPath)
                {
                    StorageFolder fLibrary = await KnownFolders.GetFolderForUserAsync(null, KnownFolderId.PicturesLibrary);
                    
                    
                    StorageFile fileCopy = await ffile.CopyAsync(fLibrary, ShortFName, NameCollisionOption.ReplaceExisting);

                    //подчищаем мусор =)
                    ffile.DeleteAsync();
                }
            }

            
            // 2  File Launching
                        
            // Path to the file in the app package to launch
            string appxFile = ShortFName;


            // Detect current Local Storage...
            //StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            StorageFolder imageFolder = 
                await KnownFolders.GetFolderForUserAsync(null, KnownFolderId.PicturesLibrary); 

            // Construct full path...
            //string FullPath = localFolder.Path + "\\" + appxFile;

            var appFile = await imageFolder.GetFileAsync(appxFile);

            if (appFile != null)
            {
                // Launch the retrieved file
                var success = await Windows.System.Launcher.LaunchFileAsync(appFile);
            }

        }//CopyToImageFolderAndRun end

    }//MegaClient end
   

}// namespace end
