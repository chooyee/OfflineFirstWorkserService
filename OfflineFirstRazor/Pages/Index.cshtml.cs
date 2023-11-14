using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OfflineFirstRazor.Pages
{
    public class IndexModel : PageModel
    {
        public string Message { get; set; }
        public string GotoPage { get; set; }
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            if (!HttpContext.Session.Keys.Contains("UserName"))
            {
                Response.Redirect("/?err=No session found");
            }
            GotoPage = Request.Query["gotoPage"];


        }
    }
}