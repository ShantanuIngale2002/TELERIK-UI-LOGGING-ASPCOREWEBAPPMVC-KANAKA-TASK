using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;
using PracticeForTelerickUI01.Models;
using PracticeForTelerickUI01.Repository;
using PracticeForTelerickUI01.Repository.Interface;
using System.Collections;
using System.Diagnostics;
using System.Text;
using Telerik.SvgIcons;
using Microsoft.Extensions.Logging;
using Azure.Core;

namespace PracticeForTelerickUI01.Controllers
{
    public class HomeController : Controller
    {

        private readonly IBookRepository bookRepository; // to access repository
        private readonly ILogger<HomeController> logger; // to access logging functionalities
        public HomeController(IBookRepository bookRepository, ILogger<HomeController> logger)
        {
            this.bookRepository = bookRepository;
            this.logger = logger;
        }




        public IActionResult Index()
        {
            logger.LogInformation("Completed Page Get-Request : <AccessTo> <Home/Index>");
            /*logger.LogInformation(
             "Completed Request {RequestMethod} {RequestPath} at {DateTimeUtc}",
                HttpContext.Request.Method,
                HttpContext.Request.Path,
                DateTime.UtcNow);*/
            return View();
        }
 


        public IActionResult SearchBook()
        {
            logger.LogInformation("Completed Page Get-Request : <AccessTo> <Home/SearchBook>");

            return View();
        }

        [HttpPost]
        public IActionResult SearchBook(string bookCode)
        {
            logger.LogInformation("Completed Page Post-Request : <AccessTo> <Home/SearchBook>");

            // Perform search using the book service
            var foundBook = bookRepository.SearchBookByCode(bookCode);

            if(foundBook == null)
            {
                logger.LogError("Anonymous Direct-Access Page Post-Request : <AccessTo> <Home/SearchBook>");

                ViewBag.SearchResult = "No Book Found";
            }

            return View(foundBook);
        }




        public List<string> GetAllGenres()
        {
            logger.LogInformation("Completed Data Fetch-Request : <DataFetchTo> <Home/GetAllGenres>");

            List<string> Data = bookRepository.GetAllGenres();
            return Data;
        }




        public IActionResult AddBook()
        {
            logger.LogInformation("Completed Page Get-Request : <AccessTo> <Home/AddBook>");

            return View();
        }

        [HttpPost]
        public IActionResult AddBook(BookCompleteDataModel model)
        {
            logger.LogInformation("Completed Page Post-Request : <AccessTo> <Home/AddBook>");

            if (!ModelState.IsValid)
            {
                logger.LogError("Failed to Page Post-Request due to Invalid Model : <ModelAccessTo> <Home/AddBook>");

                ViewBag.IfAdded = "Invalid Data is Provided.";
                return View(model);
            }

            int result = bookRepository.AddBook(model);

            if (result == -1)
            {
                logger.LogError("Failed to Add-Data Page Post-Request due to Already Existing Model.BookCode : <ModelAccessTo> <Home/AddBook>");

                ViewBag.IfAdded = "Book Code Already Exist.";
                model.BookCode = "";
                return View(model);
            }
            else if (result == 1 && HttpContext.Session.GetString("JsonBookCode") == null)
            {
                logger.LogInformation("Completed Data Add-Request on Page Post-Request : <AccessTo> <Home/AddBook>");

                ViewBag.IfAdded = "Success";
                ModelState.Clear();
                return View();
            }
            else if (result == 1 && HttpContext.Session.GetString("JsonBookCode") != null)
            {
                logger.LogInformation("Completed Data Add-Request on Page Post-Request : <AccessTo> <Home/AddBook>");

                TempData["GridProcessResult"] = "GridAddSuccess";
                return RedirectToAction("GetAllBooks"); // if update is performed from grid then redirect to grid
            }
            else
            {
                logger.LogError("Anonymous Failure for Data Add-Request on Page Post-Request : <AccessTo> <Home/AddBook>");

                ViewBag.IfAdded = "Procedure is not executed, Please do agian";
                return View(model);
            }
        }




        
        public IActionResult SearchToUpdateBook()
        {
            logger.LogInformation("Completed Page Get-Request : <AccessTo> <Home/SearchToUpdateBook>");

            var message = TempData["SearchResult"] as string;
            if(message != null)
            {
                ViewBag.SearchResult = message;
            }
            return View();
        }

        [HttpPost]
        public IActionResult SearchToUpdateBook(string bookCode)
        {
            logger.LogInformation("Completed Page Post-Request : <AccessTo> <Home/SearchToUpdateBook>");

            var foundBook = bookRepository.SearchBookByCode(bookCode);

            if (foundBook == null)
            {
                logger.LogError("Failed to Search-Data Page Post-Request due to No-Data-Found : <ModelAccessTo> <Home/SearchToUpdateBook>");

                ViewBag.SearchResult = "No Book Found";
            }
            else
            {
                logger.LogInformation("Completed Search-Data-Request on Page Post-Request : <AccessTo> <Home/SearchToUpdateBook>");
            }

            return View(foundBook);
        }

        public IActionResult UpdateBook(string bookCode)
        {
            logger.LogInformation("Completed Page Get-Request : <AccessTo> <Home/UpdateBook>");

            // handling to get view to update book from grid
            string tempBookCode = HttpContext.Session.GetString("JsonBookCode"); // Retrieve bookCode from TempData pass from jsonupdatebook ajax action
            if(tempBookCode != null)
            {
                bookCode = tempBookCode;
            }

            if (bookCode == null && tempBookCode == null)
            {
                logger.LogError("Anonymous Direct-Access Page Get-Request : <AccessTo> <Home/UpdateBook>");

                TempData["SearchResult"] = "UpdateRedirect";
                return RedirectToAction("SearchToUpdateBook");
            }

            var foundBook = bookRepository.SearchBookByCode(bookCode);

            logger.LogInformation("Completed Search-Data-Request on Page Get-Request : <AccessTo> <Home/UpdateBook>");

            //ViewData["BookCode"] = bookCode;
            return View(foundBook);
        }

        [HttpPost]
        public IActionResult UpdateBook(BookCompleteDataModel model)
        {
            logger.LogInformation("Completed Page Post-Request : <AccessTo> <Home/UpdateBook>");

            int updated = bookRepository.UpdateBook(model);

            logger.LogInformation("Completed Data Update-Request on Page Post-Request : <AccessTo> <Home/UpdateBook>");

            string tempBookCode = HttpContext.Session.GetString("JsonBookCode"); // Retrieve bookCode from TempData pass from jsonupdatebook ajax action
            if(tempBookCode != null)
            {
                TempData["GridProcessResult"] = "GridUpdateSuccess";
                return RedirectToAction("GetAllBooks"); // if update is performed from grid then redirect to grid
            }

            if (updated == 1)
            {
                TempData["SearchResult"] = "UpdateSuccess";
                return RedirectToAction("SearchToUpdateBook");
            }
            return View();
        }





        public IActionResult SearchToDeleteBook()
        {
            logger.LogInformation("Completed Page Get-Request : <AccessTo> <Home/SearchToDeleteBook>");

            var message = TempData["SearchResult"] as string;
            if (message != null)
            {
                ViewBag.SearchResult = message;
            }
            return View();
        }

        [HttpPost]
        public IActionResult SearchToDeleteBook(string bookCode)
        {
            logger.LogInformation("Completed Page Post-Request : <AccessTo> <Home/SearchToUpdateBook>");

            var foundBook = bookRepository.SearchBookByCode(bookCode);

            if (foundBook == null)
            {
                logger.LogError("Failed to Search-Data Page Post-Request due to No-Data-Found : <ModelAccessTo> <Home/SearchToDeleteBook>");

                ViewBag.SearchResult = "No Book Found";
            }
            else
            {
                logger.LogInformation("Completed Search-Data-Request on Page Post-Request : <AccessTo> <Home/SearchToDeleteBook>");
            }

            return View(foundBook);
        }

        public IActionResult DeleteBook(string bookCode)
        {
            logger.LogInformation("Completed Page Get-Request : <AccessTo> <Home/DeleteBook>");

            // handling to get view to update book from grid
            string tempBookCode = HttpContext.Session.GetString("JsonBookCode"); // Retrieve bookCode from TempData pass from jsonupdatebook ajax action
            if (tempBookCode != null)
            {
                bookCode = tempBookCode;
            }

            if (bookCode == null && tempBookCode == null)
            {
                logger.LogError("Anonymous Direct-Access Page Get-Request : <AccessTo> <Home/DeleteBook>");

                TempData["SearchResult"] = "DeleteRedirect";
                return RedirectToAction("SearchToDeleteBook");
            }
            
            var foundBook = bookRepository.SearchBookByCode(bookCode);

            logger.LogInformation("Completed Search-Data-Request on Page Get-Request : <AccessTo> <Home/DeleteBook>");

            return View(foundBook);
        }

        [HttpPost]
        public IActionResult DeleteBook(BookCompleteDataModel model)
        {
            logger.LogInformation("Completed Page Post-Request : <AccessTo> <Home/DeleteBook>");

            int updated = bookRepository.DeleteBook(model.BookCode);

            logger.LogInformation("Completed Data Delete-Request on Page Post-Request : <AccessTo> <Home/DeleteBook>");

            string tempBookCode = HttpContext.Session.GetString("JsonBookCode"); // Retrieve bookCode from TempData pass from jsonupdatebook ajax action
            if (tempBookCode != null)
            {
                TempData["GridProcessResult"] = "GridDeleteSuccess";
                return RedirectToAction("GetAllBooks"); // if deleted from grid then redirect to grid.
            }

            if (updated == 1)
            {
                TempData["SearchResult"] = "DeleteSuccess";
                return RedirectToAction("SearchToDeleteBook");
            }
            return View();
        }






        

        public IActionResult GetAllBooks()
        {
            logger.LogInformation("Completed Page Get-Request : <AccessTo> <Home/GetAllBooks>");

            if (HttpContext.Session.GetString("JsonBookCode") != null)
            {
                HttpContext.Session.Remove("JsonBookCode"); // .Clear() clears all session this can be used to remove specific session.
                //HttpContext.Session.SetString("JsonBookCode", null); // .Clear() clears all the sessions defined in appication hence practice to use this set null when one session is used in specific functionality.
            }

            // Just using viewbag in order to maintain notifications on view
            var message = TempData["GridProcessResult"] as string;

            if(message == "GridDeleteSuccess")
            {
                ViewBag.GridProcessResult = "DeleteSuccess";
            }
            else if(message == "GridUpdateSuccess")
            {
                ViewBag.GridProcessResult = "UpdateSuccess";
            }
            else if(message == "GridAddSuccess")
            {
                ViewBag.GridProcessResult = "AddSuccess";
            }

            if(message != null)
            {
                logger.LogInformation("Completed Grid "+ message +"-Process-Definition Page Get-Request : <AccessTo> <Home/GetAllBooks>");
            }



            return View();
        }


        public IActionResult GetAllBooksForGrid([DataSourceRequest] DataSourceRequest request)
        {
            var data = bookRepository.GetAllBooks();
            DataSourceResult result = data.ToDataSourceResult(request);
            
            if (result != null)
            {
                logger.LogInformation("Completed Json-Data Fetch-Request : <DataFetchTo> <Home/GetAllBooksForGrid>");
            }
            else
            {
                logger.LogError("Failed Json-Data Fetch-Request : <DataFetchTo> <Home/GetAllBooksForGrid>");
            }

            return Json(result);
        }



        [HttpGet]
        public IActionResult Get_Books_Genres()
        {
            var data = bookRepository.GetAllGenres();

            if(data != null)
            {
                logger.LogInformation("Completed Json-Data Fetch-Request : <DataFetchTo> <Home/Get_Books_Genres>");
            }
            else
            {
                logger.LogError("Failed Json-Data Fetch-Request : <DataFetchTo> <Home/Get_Books_Genres>");
            }

            return Json(data);
        }

        [HttpPost]
        public IActionResult ExportTo_ExcelData(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            logger.LogInformation("Completed Excel-Data Export-Request : <ExcelDataExportTo> <Home/ExportTo_ExcelData>");

            return File(fileContents, contentType, fileName);
        }


        
        public IActionResult JSONAddBook()
        {
            logger.LogInformation("Completed Page Get-Request : <AccessTo> <Home/JSONAddBook>");

            HttpContext.Session.SetString("JsonBookCode", "bookCode");
            return RedirectToAction("AddBook", "Home");
        }

        [HttpPost]
        public IActionResult JSONUpdateBook([FromBody] string bookCode)
        {
            logger.LogInformation("Completed Page Post-Request : <AccessTo> <Home/JSONUpdateBook>");

            HttpContext.Session.SetString("JsonBookCode", bookCode);
            return Json(new { redirectToUrl = Url.Action("UpdateBook", "Home") });
        }

        [HttpPost]
        public IActionResult JSONDeleteBook([FromBody] string bookCode)
        {
            logger.LogInformation("Completed Page Post-Request : <AccessTo> <Home/JSONDeleteBook>");

            HttpContext.Session.SetString("JsonBookCode", bookCode);
            return Json(new { redirectToUrl = Url.Action("DeleteBook", "Home") });
        }



        public IActionResult GetVisualizations()
        {
            logger.LogInformation("Completed Page Get-Request : <AccessTo> <Home/GetVisualizations>");

            return View();
        }
        
        public IActionResult GetPerYearBookDistributionData([DataSourceRequest] DataSourceRequest request)
        {
            logger.LogInformation("Completed Page Get-Request : <AccessTo> <Home/GetPerYearBookDistributionData>");

            var yearCount = bookRepository.GetPerYearBookDistribution();
            DataSourceResult result = yearCount.ToDataSourceResult(request);

            if(result!=null)
            {
                logger.LogInformation("Completed Json-Data Fetch-Request : <DataFetchTo> <Home/GetPerYearBookDistributionData>");
            }
            else
            {
                logger.LogError("Failed Json-Data Fetch-Request : <DataFetchTo> <Home/GetPerYearBookDistributionData>");
            }

            return Json(result);
        }

        public IActionResult GetPerGenreBookDistributionData([DataSourceRequest] DataSourceRequest request)
        {
            logger.LogInformation("Completed Page Get-Request : <AccessTo> <Home/GetPerGenreBookDistributionData>");

            var genreCount = bookRepository.GetPerGenreBookDistribution();
            DataSourceResult result = genreCount.ToDataSourceResult(request);

            if (result != null)
            {
                logger.LogInformation("Completed Json-Data Fetch-Request : <DataFetchTo> <Home/GetPerGenreBookDistributionData>");
            }
            else
            {
                logger.LogError("Failed Json-Data Fetch-Request : <DataFetchTo> <Home/GetPerGenreBookDistributionData>");
            }

            return Json(result);
        }






    }
}
