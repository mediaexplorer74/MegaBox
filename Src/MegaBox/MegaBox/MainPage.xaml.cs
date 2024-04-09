
using Xamarin.Forms;
using MegaBox.Model; // !

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
             void CopyToImageFolderAndRun(string FullLSPath, string ShortFName);
        }

        
        public MainPage()
        {
            InitializeComponent();

            // Interface Init  
            megaClient = DependencyService.Get<IMegaClient>();

            BindingContext = DataFactory.DataItems;

            // Apply items sorting ////////////////////////
            string SearchBoxText = "";
            DataFactory.FilterMegaItemsAsync(SearchBoxText);
            ////////////////////////////////////////
        }//MainPage


        // loginEntry_TextChanged
        private void loginEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            string SearchBoxText = "";

            if (e.NewTextValue != null)
                SearchBoxText = e.NewTextValue.ToString();

            DataFactory.FilterMegaItemsAsync(SearchBoxText);

        }//loginEntry_TextChanged



        // !!! ItemSelected handling
        private async void timelineList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            // starting...
            int idx = -1;
            string StRes2 = "";

            if (e.SelectedItem == null)
            {
               return;
            }

            var SI = e.SelectedItem;

            idx = (SI as MegaBox.Model.DataItem).Id;

            if ( (idx < 0) || (idx > 10000) )
                return;

            StRes2 = (SI as MegaBox.Model.DataItem).Headline;

           
            // Make Dialog Popup
            bool choosedresult = await DisplayAlert
             (
                MegaClient.arNodes[idx].Name //+ " [" +
                //MegaClient.arNodes[idx].Size.ToString() + " bytes]",
                //MegaClient.arNodes[idx].ModificationDate.ToString() + " -modified " 
                //+ "]"
                ,
                "Do you want to download & launch this file?",
                "Yes", 
                "No"
             );


            // If user choose "No", do nothing =)
            if (choosedresult == false)
            {
                return;
            }

            
            // определяем локальное хранилище
            //IFolder localFolder = FileSystem.Current.LocalStorage;

            //DEBUG
            //Debug.WriteLine(localFolder.Path);


            // Phase 2

            //"MEGA login"
            MegaClient.client.LoginAnonymous();



            // Скачиваем файл и получаем полный путь к нему...
            string FullLSPath = MegaClient.client.DownloadFile
            (
                MegaClient.arNodes[idx],
                MegaClient.arNodes[idx].Name
            );

            //"MEGA logout"
            MegaClient.client.Logout();


            // -----------------
            string ShortFName = MegaClient.arNodes[idx].Name;

            // RnD
            megaClient.CopyToImageFolderAndRun(FullLSPath, ShortFName);

            // finishing...
            ((ListView)sender).SelectedItem = null;

        }// Item_Selected

    }
}
