using imaginaryCompany.Data;
using Microsoft.AspNetCore.Components;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace imaginaryCompany.Components
{
    public partial class Search
    {
        private Timer _timer;

        public string SearchTerm { get; set; }
        public iCompanyData TheData = new FakeData();
        public IEnumerable<Software> VersionsResults;
        public string SearchStatus { get; set; } = string.Empty;


        //==================================================================
        [Parameter]
        public EventCallback<string> OnSearchChanged { get; set; }

        private void SearchChanged()
        {
            if (_timer != null)
                _timer.Dispose();

            _timer = new Timer(OnTimerElapsed, null, 500, 0);
        }


        //==================================================================
        private void isInputGood(string SearchTerm,out bool isGood,out bool isSingle)
        {

            try
            {
                //check if its jsut one digit and number 
                Regex justnum = new Regex(@"^[0-9]+$");
                if (justnum.IsMatch(SearchTerm))
                {

                    #if (DEBUG)
                    Console.WriteLine("just num with no major/minor");
                    #endif
                    isSingle = true;
                    isGood = true;
                }
                else
                {
                    //see if not just a number we can parse the version

                            #if (DEBUG)
                            Console.WriteLine("parsing with major/minor");
                            #endif

                    var version = new Version(SearchTerm);
                    isSingle = false;
                    isGood = true;
                }


            }

            catch(Exception ex)
            {
                // maybe do something with errors in future

                    #if (DEBUG)
                    Console.WriteLine(ex.Message);
                    #endif
                isGood = false;
                isSingle = false;
            }
   
     
            
        }

        //==================================================================
        public async Task VersionSearchClick()
        {
            isInputGood(SearchTerm, out bool isGood,out bool isSingle);
            if (isGood == true)
            {


                #if (DEBUG)
                Console.WriteLine("doing search");
                #endif
                SearchStatus = "Doing Search";
                VersionsResults = await FindVersions(SearchTerm,isSingle);
            }
            else if (isGood == false)
            {

                #if (DEBUG)
                Console.WriteLine("input check failed"); ;
                #endif
                SearchStatus = "Please check number entered";
                VersionsResults = null;
            }
            else
            {
                #if (DEBUG)
                Debug.WriteLine("error");
                #endif

                SearchStatus = "Error please check the input";
                VersionsResults = null;
            }

        }

        //==================================================================
        async Task<IEnumerable<Software>> FindVersions(string versioninput,bool single)
        {
            SearchStatus = "Search Done";
            IEnumerable <Software> sData = TheData.GetData();
            IEnumerable<Software> rData = null;
            if (single) 
            {
                //normalize a num to #.# format
                versioninput += ".0";
            }
       
                var startdata = sData.ToList();

                sData.ToList().ForEach(y =>
                {

                    Version xversions = Version.Parse(y.Version);
                    Version searchversion = Version.Parse(versioninput);


                    if (xversions.Major < searchversion.Major || xversions.Major == searchversion.Major && xversions.Minor < searchversion.Minor)
                    {
                      startdata.Remove(y);

                    }

                });

            if (startdata.Count == 0)
            {
                SearchStatus = "No items Found";

            }
                rData = startdata;


            return rData;
        }


        //==================================================================
        private void OnTimerElapsed(object sender)
        {
            OnSearchChanged.InvokeAsync(SearchTerm);
            _timer.Dispose();
        }


    }
}
