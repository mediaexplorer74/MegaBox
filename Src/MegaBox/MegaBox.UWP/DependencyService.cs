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

using CG.Web.MegaApiClient; 
using Megabox.Model; 
using static MegaBox.MainPage;
using Windows.Storage;

//[assembly: Dependency(typeof(MegaBox.UWP.DeviceInfo))]
[assembly: Dependency(typeof(MegaBox.UWP.MegaClient))]
namespace MegaBox.UWP
{

   
    // 3 IMegaClient "interface realization"
    public class MegaClient : IMegaClient
    {
        // our super client (see details of realization in MegaLib project)
        public static MegaApiClient client = new MegaApiClient();  

       
        // Parent Node 
        public static INode parent;
        
        // Nodes, temporary
        public static IEnumerable<INode> nodes;


        // ArNodes / Массив arNodes (емкость -- 10000)
        public static INode[] arNodes = new INode[10000];



        // GetInfo: Say Hello to Service =)
        public string GetInfo()
        {
            // TODO: write some description of service's interface 
            return $"Mega.nz Service";
        }//GetInfo


        // Mega.Nz Login
        public bool MegaLogin()
        {
            client.LoginAnonymous();
            return true;
        }//MegaLogin

        // Mega.Nz Logout
        public bool MegaLogout()
        {
            client.Logout();
            return true;
        }//MegaLogout



        // Get Nodes (files, folders, subfolders). Startpoint: MegaSharedFolderURL.
        public bool MegaGetNodes(string MegaSharedFolderURL)
        {
            client.LoginAnonymous();

            Uri folderLink = new Uri(MegaSharedFolderURL); 
           
            // 1 
            //IEnumerable<INode> nodes (private!)
            nodes = client.GetNodesFromLink(folderLink);

            if (nodes == null) 
                return false;

            // 2
            //INode parent (private!)
            parent = nodes.Single(n => n.Type == NodeType.Root);

            if (parent == null)
                return false;

            // 3
            // Get sub-nodes (folders and files in level 1, 2, etc...)
            ProcessAllSubNodes(nodes, parent);

            client.Logout();

            return true;
        }//MegaGetNodes


        // Process All Sub Nodes (Level 1, 2, etc...)
        private void ProcessAllSubNodes(IEnumerable<INode> nodes, INode parent, int level = 0)
        {
            IEnumerable<INode> children = nodes.Where(x => x.ParentId == parent.Id);

            foreach (INode child in children)
            {
                //string infos = $"- {child.Name} - {child.Size} bytes - {child.CreationDate}";

                string infos = $"{child.Name} [{child.Type}]";



                // Store our contact =)

                if (child.Type.ToString() == "Directory")
                {
                    Contact.MegaFName[Contact.MegaCount] = child.Name.ToUpper();
                }
                else
                {
                    Contact.MegaFName[Contact.MegaCount] = child.Name;
                }
                
                Contact.MegaFKey[Contact.MegaCount] = child.Id;
                Contact.MegaType[Contact.MegaCount] = child.Type.ToString();


                // Store ALL INode object !
                MegaClient.arNodes[Contact.MegaCount] = child;

                // Increase counter 
                Contact.MegaCount++;

                // Recursion =)
                if (child.Type == NodeType.Directory)
                {
                    ProcessAllSubNodes(nodes, child, level + 1);
                }
            }
        }//ProcessAllSubNodes


        // FOR TESTING PURPOSES 
        public string DownloadFileByIndex(int idx)
        {
            string FullPath = "";

            if (arNodes[idx].Type.ToString() != "File")
               return "";
                        
            client.LoginAnonymous();

            FullPath = client.DownloadFile
            (
                arNodes[idx],
                arNodes[idx].Name
            );

            client.Logout();

            return FullPath;
        }//DownloadFileByIndex


        public async void DownloadandRunFileByIndex(int idx)
        {
            // 1. File Downloading 

            string FullLSPath = "";


            // Check "Node" type (Directory or File?) 
            
            if (arNodes[idx].Type.ToString() != "File")
              return;
                        
            client.LoginAnonymous();

            // Download file and get full path in LS ("local storage")
            FullLSPath = client.DownloadFile
            (
                arNodes[idx],
                arNodes[idx].Name
            );

            client.Logout();


            // 2. Storage manipulations                          
            StorageFolder folder = ApplicationData.Current.LocalFolder;//KnownFolders.VideosLibrary;
            IReadOnlyList<StorageFile> files = await folder.GetFilesAsync();

            // Пробегаемся по всем файлам в хранилище... ищем скачанный файлик
            // TODO: че-то придумать с избавлением от цикла!!!
            foreach (StorageFile ffile in files)
            {
                if (ffile.Path == FullLSPath)
                {
                    StorageFolder fLibrary = await KnownFolders.GetFolderForUserAsync(null, KnownFolderId.PicturesLibrary);
                    
                    
                    StorageFile fileCopy = await ffile.CopyAsync(fLibrary, MegaClient.arNodes[idx].Name, NameCollisionOption.ReplaceExisting);
                }
            }

            
            // 3. File Launching
                        
            // Path to the file in the app package to launch
            string appxFile = MegaClient.arNodes[idx].Name;


            // Detect current Local Storage...
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;

            // Construct full path...
            string FullPath = localFolder.Path + "\\" + appxFile;

            var appFile = await localFolder.GetFileAsync(appxFile);

            if (appFile != null)
            {
                // Launch the retrieved file
                var success = await Windows.System.Launcher.LaunchFileAsync(appFile);
            }

        }//DownloadandRunFileByIndex

    }//MegaClient : IMegaClient
   

}
