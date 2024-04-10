using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CG.Web.MegaApiClient; // MEGA Api !

namespace MegaBox.Model
{
   
    public class MegaManager
    {
        // Filter by Category
        public static void GetItemsbyCategory(string category, ObservableCollection<DataItem> megaItems)
        {
            category = $"{category}".ToUpper(); 

            var allItems = getMegaItems();

            var filteredMegaItems = allItems.Where(p => p.Category == category).ToList();

            megaItems.Clear();

            filteredMegaItems.ForEach(p => megaItems.Add(p));
        }// GetNewsbyCategory


        // GetAllNews
        public static void GetAllNews(ObservableCollection<DataItem> megaItems)
        {
            var allItems = getMegaItems();

            var MegaItems = allItems.ToList();

            megaItems.Clear();

            MegaItems.ForEach(p => megaItems.Add(p));
        }//GetAllNews end


        // Filter by Name
        public static void GetItemsbyName(string headline, ObservableCollection<DataItem> megaItems)
        {
            List<DataItem> allItems = getMegaItems();

            List<DataItem> filteredMegaItems = allItems.Where(p => p.Headline == headline).ToList();

            megaItems.Clear();

            filteredMegaItems.ForEach(p => megaItems.Add(p));
        }//GetNewsbyName end

       
        //
        public static string GetIconCategoryName(string StartName)
        {
            // Foolish-proof =)
            //StartName = StartName.Trim();
            StartName = $"{StartName.ToUpper()}";
            
            //DEBUG
            //if (StartName == "DEPENDENCIES")
            //         Debug.WriteLine(StartName);
            
            string FinishName;

            if ($"{StartName}" == $"!Guides".ToUpper())
            {
                FinishName = "01guides.png";
            }
            else if ($"{StartName}" == $"Apps".ToUpper())
            {
                FinishName = "02apps.png";
            }
            else if ($"{StartName}" == $"Camera + Photos".ToUpper())
            {
                FinishName = "03camera.png";
            }
            else if ($"{StartName}" == $"Customization".ToUpper())
            {
                FinishName = "04customization.png";
            }
            else if ($"{StartName}" == $"Dependencies".ToUpper())
            {
                FinishName = "05dependencies.png";
            }
            else if ($"{StartName}" == $"Emulators".ToUpper())
            {
                FinishName = "06emulators.png";
            }
            else if ($"{StartName}" == $"Hp exclusive apps".ToUpper())
            {
                FinishName = "07hpexclusive.png";
            }
            else if
                ( $"{StartName}" == $"Microsoft apps (Stock,leaked,retired,Extensions, etc)".ToUpper() ) 
            {
                FinishName = "08microsoft.png";
            }
            else if ($"{StartName}" == $"Multimedia".ToUpper())
            {
                FinishName = "09multimedia.png";
            }
            else if ($"{StartName}" == $"Old memories and resources".ToUpper())
            {
                FinishName = "10oldmemories.png";
            }
            else if ($"{StartName}" == $"Productivity".ToUpper())
            {
                FinishName = "11productivity.png";
            }
            else if ($"{StartName}" == $"Social".ToUpper())
            {
                FinishName = "12social.png";
            }
            else if ($"{StartName}" == $"Tools and Tweaks".ToUpper())
            {
                FinishName = "13tweaks.png";
            }
            else if ($"{StartName}" == $"Travel, Weather, News, Sports, Navigation".ToUpper())
            {
                FinishName = "14travel.png";
            }
            else if ($"{StartName}" == $"W10M PC Tools".ToUpper())
            {
                FinishName = "15mobile.png";
            }
            else if ($"{StartName}" == $"Xbox and Non Xbox live Games".ToUpper())
            {
                FinishName = "16xbox.png";
            }
            else 
            {
                // other (category mismatch))
                FinishName = "17layers.png";
            }

            return FinishName;
        }

        // getMegaItems
        public static List<DataItem> getMegaItems()
        {
          
            List<DataItem> items = new List<DataItem>();

            // определяем число контактов
            int numberOfContacts = Contact.MegaCount;// CountMegaContacts(); 

            for (var i = 0; i < numberOfContacts; i++)
            {
                items.Add
                (
                    new DataItem()
                    {
                        Id = i, // unique element index !
                        Category = Contact.MegaFCategory[i].Trim(),
                        Headline = Contact.MegaFName[i],
                        Subhead = Contact.MegaFCategory[i].Trim(),
                        DateLine = Contact.MegaFSubcategory[i].Trim(),
                        Image = "Assets/" + MegaManager.GetIconCategoryName(Contact.MegaFCategory[i].Trim()),
                        FileSize = MegaClient.arNodes[i].Size, // todo
                        IsLast = Contact.isEndOfDir[i], // todo
                        DateModified = MegaClient.arNodes[i].ModificationDate,//.ToString(),
                    }
                ); 
            }

            // RND: Sorting 
            //items.Sort(delegate (DataItem x, DataItem y)
            //{
            //    if (x.DateModified == null && y.DateModified == null) return 0;
            //    else if (x.DateModified == null) return -1;
            //    else if (y.DateModified == null) return 1;
            //    else return x.DateModified.CompareTo(y.DateModified);
            //});

            return items;
        }


        // filterMegaItems
        public static List<DataItem> FilterMegaItems(string SearchText)
        {

            var items = new List<DataItem>();

            // определяем число контактов
            int numberOfContacts = Contact.MegaCount;// CountMegaContacts(); 

            if (SearchText != "")
            {
                // get FILTERED items

                for (var i = 0; i < numberOfContacts; i++)
                {
                    if (Contact.MegaFName[i].Contains(SearchText) == true)
                    {
                        items.Add
                        (
                            new DataItem()
                            {
                                Id = i, // unique element index !
                                Category = Contact.MegaFCategory[i].Trim(),
                                Headline = Contact.MegaFName[i],
                                Subhead = Contact.MegaFCategory[i].Trim(),
                                DateLine = Contact.MegaFSubcategory[i].Trim(),
                                Image = "Assets/" + MegaManager.GetIconCategoryName(Contact.MegaFCategory[i].Trim()),
                                FileSize = MegaClient.arNodes[i].Size, // todo
                                IsLast = Contact.isEndOfDir[i], // todo
                                DateModified = MegaClient.arNodes[i].ModificationDate,//.ToString(),
                            }
                        );
                    }
                }
            }
            else
            {
                // get ALL Items 

                for (var i = 0; i < numberOfContacts; i++)
                {
                    items.Add
                    (
                        new DataItem()
                        {
                            Id = i, // unique element index !
                            Category = Contact.MegaFCategory[i].Trim(),
                            Headline = Contact.MegaFName[i],
                            Subhead = Contact.MegaFCategory[i].Trim(),
                            DateLine = Contact.MegaFSubcategory[i].Trim(),
                            Image = "Assets/" + MegaManager.GetIconCategoryName(Contact.MegaFCategory[i].Trim()),
                            FileSize = MegaClient.arNodes[i].Size, // todo
                            IsLast = Contact.isEndOfDir[i], // todo
                            DateModified = MegaClient.arNodes[i].ModificationDate,//.ToString(),
                        }
                    );
                    
                }
               
            }

            // RND: Sorting 
            //items.Sort(delegate (DataItem x, DataItem y)
            //{
            //    if (x.DateModified == null && y.DateModified == null) return 0;
            //    else if (x.DateModified == null) return -1;
            //    else if (y.DateModified == null) return 1;
            //    else return x.DateModified.CompareTo(y.DateModified);
            //});

            return items;
        }//filter


    }// MegaManager class end 
        
}
