using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.IO;

namespace AmazonScraper
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Getting browsers default download location");
            string pathUser = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string pathDownload = Path.Combine(pathUser, "Downloads");
            
            string searchString = "";
            while (string.IsNullOrEmpty(searchString) == true)
            {
                Console.WriteLine("Please enter the amazon search term");
                searchString = Console.ReadLine();
            }
            Console.WriteLine("Initializing chrome webdrier");
            using (IWebDriver driver = new ChromeDriver())
            {
                Console.WriteLine("Navigating to https://www.amazon.com");
                driver.Navigate().GoToUrl("https://www.amazon.com/");

                Console.WriteLine("Finding the search box and entering the search term");
                IWebElement searchBox = driver.FindElement(By.Id("twotabsearchtextbox"));
                searchBox.SendKeys(searchString);

                Console.WriteLine("Finding the search buton");
                IWebElement searchButton = driver.FindElement(By.Id("nav-search-submit-button"));

                Console.WriteLine("Clicking the search button");
                searchButton.Click();


                Console.WriteLine("Wait for the search results to load");
                System.Threading.Thread.Sleep(2000);

                Console.WriteLine("Finding the \"Sort By\" element");
                IWebElement sortDropdown = driver.FindElement(By.Id("a-autoid-0-announce"));

                Console.WriteLine("Clicking the sort by element");
                sortDropdown.Click();

                Console.WriteLine("Finding low to high price button");
                IWebElement lowToHighOption = driver.FindElement(By.XPath("//*[@id='s-result-sort-select']/option[2]"));

                Console.WriteLine("Clicking low to high price");
                lowToHighOption.Click();


                Console.WriteLine("Waiting for search results to sort");
                System.Threading.Thread.Sleep(2000);

                Console.WriteLine("Finding sorted search results");
                IWebElement searchResults = driver.FindElement(By.ClassName("s-result-list"));

                Console.WriteLine("Finding the individual search result elements");
                List<IWebElement> resultElements = searchResults.FindElements(By.CssSelector("div.s-asin div.sg-col-inner div.celwidget div.s-card-container div.a-section div.s-text-center span.rush-component")).ToList();

                Console.WriteLine("Creating streamwriter for output file");
                Console.WriteLine(pathDownload.ToString());

                using (FileStream fs = new FileStream(Path.Combine(pathDownload, "AmazonResults.txt"), FileMode.Create))
                {
                    using (StreamWriter writer = new StreamWriter(fs))
                    {
                        Console.WriteLine("Iterating through the first 10 search result elements");
                        for (int i = 0; i < 10; i++)
                        {
                            Console.WriteLine("Finding the link element for the current search result");
                            IWebElement linkElement = resultElements[i].FindElement(By.TagName("a"));

                            Console.WriteLine("Getting the URL of the link element");
                            string url = linkElement.GetAttribute("href");

                            Console.WriteLine("writing the URL of the link element to the output file");
                            writer.WriteLineAsync(url);
                        }
                    }
                }

            }
            Console.WriteLine("Successfully written to " + pathDownload + " press any key to exit");
            Console.ReadKey();
        }
    }
}