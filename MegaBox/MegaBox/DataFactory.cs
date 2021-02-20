﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CG.Web.MegaApiClient;
using MegaBox.Model;
using Xamarin.Forms; // !

namespace MegaBox
{
    // DataFactory class
    //public static class DataFactory
    public static class DataFactory
    {
        
        public static IList<DataItem> DataItems { get; private set; }

        public static ObservableCollection<DataItem> mDataItems;

        
        private static DateTime TodayAt(int hour, int minute)
        {
            return new DateTime(DateTime.Now.Year,
                DateTime.Now.Month,
                DateTime.Now.Day,
                hour, minute, 0);
        }


        static string megaStorageURL; // Mega.nz root "directory" URL

        static string CurrectFCategory;     // текущая ф. категория (группа файл. обьектов, каталог)
        static string CurrectFSubcategory;  // текущая ф. подкатегория (подгруппа, подкаталог)

        static int DirectoryCount;          // counter для каталогов
        static int SubdirectoryCount;         // counter для каталогов

        static int EOD;                       // маркер "End of Dir"

        //static bool isBusy;                 // Status of Task 

        //MegaClient MegaClient { get; set; }

        //public Command GetMegaItemsCommand { get; }
        //public Command GetClosestCommand { get; }





        // 0 Constructor
        static DataFactory()
        {
            
            mDataItems = new ObservableCollection<DataItem>();

            //MegaClient = new MegaClient();

            //GetMegaItemsCommand = new Command(async () => await GetMegaItemsAsync());
            //GetClosestCommand = new Command(async () => await GetClosestAsync());

            // Automatic start =)
            GetMegaItemsAsync();

            //mDataItems = DataItems;

            DataItems = mDataItems;            

        }// DataFactory constructor end


        // 1 FilterMegaItemsAsync
        public static async Task FilterMegaItemsAsync(string SearchBoxText)
        {
            
            var FItems = MegaManager.FilterMegaItems(SearchBoxText);

            mDataItems.Clear();

            FItems.ForEach(p => mDataItems.Add(p));


        }//FilterMegaItemsAsync() end 


        // 2 GetMegaItemsAsync
        public static async Task GetMegaItemsAsync()
        {
            

            try
            {
                // phase 1 

                megaStorageURL = "https://mega.nz/#F!SYtigRjB!EhNuflDF9fefSXuolgn0Rw"; // Prod (W10M)

                // phase 2 
                // // Start loading Data from Mega.Nz...
                //await PreprocessAsync(megaStorageURL);
                Preprocess(megaStorageURL);

                // phase 3 
                //MegaManager.GetAllNews(MegaItems);

                // ********************************************************

                mDataItems.Clear();

                for (var i = 0; i < Contact.MegaCount; i++)
                {
                    mDataItems.Add
                    (
                        new DataItem
                        {
                            Id = i, // unique element index !
                            Category = Contact.MegaFCategory[i].Trim(),
                            Headline = Contact.MegaFName[i],
                            Subhead = Contact.MegaFCategory[i].Trim(),
                            DateLine = Contact.MegaFSubcategory[i].Trim(),
                            Image = "Assets/" + MegaManager.GetIconCategoryName(Contact.MegaFCategory[i].Trim()),
                            FileSize= MegaClient.arNodes[i].Size, // todo
                            IsLast = Contact.isEndOfDir[i], // todo

                        }
                    );
                }


            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Error!", ex.Message, "OK");
            }
            finally
            {
                //IsBusy = false;
            }

        }//GetMegaItemsAsync() end 





        // async Task GetClosestAsync
        public static async Task GetClosestAsync()
        {
            //if (IsBusy)// || MegaItems.Count == 0)
            //    return;
            try
            {
                //

                //var first = MegaItems.OrderBy(m => location.CalculateDistance(
                //    new Location(m.Latitude, m.Longitude), DistanceUnits.Miles))
                //    .FirstOrDefault();
                //
                //await Application.Current.MainPage.DisplayAlert("", first.Name + " " +
                //    first.Location, "OK");

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to query location: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Error!", ex.Message, "OK");
            }
        }// GetCLosestAsync end



        // TODO *******************************************

        // Preprocess: Collect and prepare Mega.nz data...
        public static void Preprocess(string MegaSharedFolderURL)
        {
            Contact.MegaCount = 0; // counter init 

            Contact.MegaCount = 0; // counter init 

            CurrectFCategory = "<Root>"; // 'parent' group (file category) init  
            DirectoryCount = 0;

            CurrectFSubcategory = "<Root>"; // 'parent' sub-group (sub-folder) init  
            SubdirectoryCount = 0;

            //ListView01.Items.Clear(); // clear ListView 


            // "no login"
            MegaClient.client.LoginAnonymous(); // "Users only" =)


            // Reconstruct URI
            Uri folderLink = new Uri(MegaSharedFolderURL);

            // Get nodes (folders and files in level 0)
            MegaClient.nodes = MegaClient.client.GetNodesFromLink(folderLink);

            // Get parent node 
            MegaClient.parent = MegaClient.nodes.Single(n => n.Type == NodeType.Root);

            // Get sub-nodes (folders and files in level 1, 2, etc...)
            ProcessAllSubNodes(MegaClient.nodes, MegaClient.parent);

            // Logout
            MegaClient.client.Logout();

        }//Preprocess() end



        // Preprocess: Collect and prepare Mega.nz data...
        private static async Task PreprocessAsync(string MegaSharedFolderURL)
        {
            EOD = 0; 
            Contact.MegaCount = 0; // counter init 

            CurrectFCategory = "<Root>"; // 'parent' group (file category) init  
            DirectoryCount = 0;

            CurrectFSubcategory = "<Root>"; // 'parent' sub-group (sub-folder) init  
            SubdirectoryCount = 0;


            // "no login"
            await MegaClient.client.LoginAnonymousAsync(); // "Users only" =)

            // GetNodes retrieves all files/folders metadata from Mega
            // so this method can be time consuming

            //IEnumerable<INode> nodes = MegaClient.client.GetNodes();


            // Reconstruct URI
            Uri folderLink = new Uri(MegaSharedFolderURL);

            // Get nodes (folders and files in level 0)
            MegaClient.nodes = MegaClient.client.GetNodesFromLink(folderLink);

            // Get parent node 
            MegaClient.parent = MegaClient.nodes.Single(n => n.Type == NodeType.Root);

            // Get sub-nodes (folders and files in level 1, 2, etc...)
            ProcessAllSubNodes(MegaClient.nodes, MegaClient.parent);

            // Logout
            await MegaClient.client.LogoutAsync();

        }//PreprocessAsync() end



        // Process All Sub Nodes (Level 1, 2, etc...)
        static void ProcessAllSubNodes(IEnumerable<INode> nodes, INode parent, int level = 0)
        {

            IEnumerable<INode> children = nodes.Where(x => x.ParentId == parent.Id);

            foreach (INode child in children)
            {

                if (child.Type.ToString() == "Directory")
                {

                    // *** FOLDER object found ***

                    // New Category found? Is Level = 0?? OK!
                    if ((child.Name != CurrectFCategory && child.Size == 0) && (level == 0))
                    {
                        // Change category !
                        CurrectFCategory = $"{child.Name.ToUpper()}";

                        DirectoryCount++; // текущая категория файлов указатель на группу)
                    }

                    // New Category found? Is Level = 0?? OK!
                    if ((child.Name != CurrectFSubcategory && child.Size == 0) && (level <= 1))
                    {
                        // Change category !
                        CurrectFSubcategory = $"{child.Name.ToUpper()}";

                        SubdirectoryCount++; // текущая категория файлов указатель на группу)
                    }

                    //if (Contact.MegaCount - 2 >= 0)
                    //  Contact.isEndOfDir[Contact.MegaCount - 2] = true;
                    Contact.isEndOfDir[EOD] = true;

                }
                else
                {
                    //*** FILE object found ***


                    Contact.MegaFName[Contact.MegaCount]
                    = $"{child.Name}";


                    Contact.MegaFCategory[Contact.MegaCount]
                        = $"{CurrectFCategory}";

                    Contact.MegaFSubcategory[Contact.MegaCount]
                        = $"{CurrectFSubcategory}";


                    Contact.MegaFKey[Contact.MegaCount]
                        = child.Id;

                    Contact.isEndOfDir[Contact.MegaCount] = false;
                    EOD = Contact.MegaCount;

                    // Store ALL INode object !
                    MegaClient.arNodes[Contact.MegaCount] = child;


                    Contact.MegaCount++;

                }


                // Если нашли "потомков" класса "Папка", повторяем маневры... 
                if (child.Type == NodeType.Directory)
                {
                    ProcessAllSubNodes(nodes, child, level + 1);
                }
            }
        }//ProcessAllSubNodes



    }// DataFactory class end

}// namespace MegaBox end

   