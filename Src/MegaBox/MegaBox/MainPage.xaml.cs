
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

using System.Diagnostics;

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

using Megabox.Model;

namespace MegaBox
{

    
    public partial class MainPage : ContentPage
    {

        // Этот интерфейс будет определять сигнатуру методов, 
        // реализация которых будет зависеть от конкретной платформы. 
      
        // 1 Interface connector
        IMegaClient megaClient;

        // 2 Interface declaration
        public interface IMegaClient
        {
            string GetInfo();
            bool MegaLogin();
            bool MegaLogout();
            bool MegaGetNodes(string uri);
            string DownloadFileByIndex(int idx);
            void DownloadandRunFileByIndex(int idx);
        }


        public MainPage()
        {
            
            InitializeComponent();

            // A. Interface Init  
            megaClient = DependencyService.Get<IMegaClient>();

            // " W10M Store " , by Win10Mobile Telegram Community =)
            string megaStorage = "https://mega.nz/folder/om4kkKCa#Nv7RCZOhCXlCfzRnKpfCXg"; // 4PDA Astoria repo
                          //"https://mega.nz/folder/SKZxnQAR#EvlQqjMIVQwoxcje9r-jAw"; // W10MGroup repo
                          //"https://mega.nz/#F!SYtigRjB!EhNuflDF9fefSXuolgn0Rw"; // old

            // B. Bind data
            BindingContext = DataFactory.Places;

            // C. Collect and prepare Mega.nz data...
            Preprocess(megaStorage);

            // D. Add new item into "ListView"

            for(int i = 0; i < Contact.MegaCount; i++)
            {
                DataFactory.Places.Add(new GreatPlace
                {
                    Index = i,
                    Title = Contact.MegaFName[i],
                    Handle = Contact.MegaFKey[i],
                    Type = Contact.MegaType[i],
                   
                });
            }
        }//MainPage



        // Collect and prepare Mega.nz data...
        private void Preprocess(string MegaSharedFolderURL)
        {
            Contact.MegaCount = 0; //counter init 

            DataFactory.Places.Clear(); //clear "data list"
         
            megaClient.MegaGetNodes(MegaSharedFolderURL); // fulfill nodes data 

        }//Preprocess


        // Item_Tapped handler
        private async void Item_Tapped(object sender, EventArgs e)
        {
            // Start to analyze our event sender...
            ListView l1 = sender as ListView;
            
            // Get info about selected item (SI)
            var SI = l1.SelectedItem;

            // Get Node's type
            string StRes2 = (SI as MegaBox.GreatPlace).Type;

            // Make simple type control (if not file, then do nothing ) 
            if (StRes2 != "File")
               return;
            
            // File Type
            string StRes1 = (SI as MegaBox.GreatPlace).Title;
            
            // Get the index of selected ListView's item
            int SelectedItemIndex = (SI as MegaBox.GreatPlace).Index;

            // Make Dialog Popup
            bool choosedresult = await DisplayAlert($"{StRes1}", "Do you want to download&launch this file?", "Yes", "No");

            // If user choose "No", do nothing =)
            if (choosedresult == false)
            {
                return;
            }

            // Download and Run file from Mega.Nz 
            megaClient.DownloadandRunFileByIndex(SelectedItemIndex);

        }//Item_Tapped
    }
}
