using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using OneTrack.Models;
using X.PagedList;
using X.PagedList.Extensions;


namespace OneTrack.Controllers
{
    public class LogsController : Controller
    {
        private readonly string _connectionString;

        public LogsController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("LoggerConnection");
        }


        /// <summary>
        ///  Displays the list of application logs.
        /// </summary>
        /// <returns>A view displaying the logs.</returns>
        public IActionResult Index(int? page, DateTime? startDate, DateTime? endDate, string level = null, string search = null)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = "SELECT * FROM Logs WHERE 1=1";

                if (startDate.HasValue)
                {
                    query += " AND TimeStamp >= @StartDate";
                }

                if (endDate.HasValue)
                {
                    query += " AND TimeStamp <= @EndDate";
                }

                if (!string.IsNullOrEmpty(level))
                {
                    query += " AND Level = @Level";
                }

                if (!string.IsNullOrEmpty(search))
                {
                    query += " AND (Message LIKE @Search OR UserName LIKE @Search OR RequestPath LIKE @Search)";
                    search = $"%{search}";
                }

                var logs = connection.Query<OneTrack.Models.LogEntry>(query, new { StartDate = startDate, EndDate = endDate, Level = level, Search = search }).ToList();

                int pageSize = 10;
                int pageNumber = page ?? 1;

                var pagedLogs = logs.ToPagedList(pageNumber, pageSize);

                return View(pagedLogs);
            }
        }

        [HttpPost]
        public IActionResult ClearLogs()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                // Clear the logs from the database
                var query = "DELETE FROM Logs";
                connection.Execute(query);
            }

            // Redirect back to the Index action after clearing logs
            return RedirectToAction("Index");
        }
    }
}
