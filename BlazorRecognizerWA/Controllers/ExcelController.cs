using BlazorRecognizerWA.Models;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Net;

namespace BlazorRecognizerWA.Controllers
{
    public class ExcelController : ControllerBase
    {
        [HttpPost]
        public async Task<HttpResponseMessage> ExportarLogsAExcel(List<LogAzure> logs)
        {
            var nombreArchivo = $"LogsAzure.xlsx";
            //var a = GenerarExcel(nombreArchivo, logs);
            
            return await GenerarExcel(nombreArchivo, logs);

        }
        public async Task< HttpResponseMessage> GenerarExcel(string nombreArchivo, List<LogAzure> logs)
        {
            try
            {
                DataTable dataTable = new DataTable("Logs");
                dataTable.Columns.AddRange(new DataColumn[]
                {
                new DataColumn("Id"),
                new DataColumn("Tipo"),
                new DataColumn("Descripcion"),
                new DataColumn("Fecha Hora")
                });

                foreach (var log in logs)
                {
                    dataTable.Rows.Add(log.Id,
                        log.Tipo,
                        log.Descripcion,
                        log.FechaHora
                        );
                }

                using (XLWorkbook wb = new XLWorkbook())
                {
                    wb.Worksheets.Add(dataTable);

                    using (MemoryStream stream = new MemoryStream())
                    {
                        wb.SaveAs(stream);
                        var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        var fileName = $"Prueba_{DateTime.Now:yyyy-MM-dd}.xlsx";

                        HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK)
                        {
                            Content = new ByteArrayContent(stream.ToArray())
                        };
                        response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                        {
                            FileName = fileName
                        };
                        response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);

                        return response;

                        //return File(stream.ToArray(),
                        //    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        //    nombreArchivo);
                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }
           

        }
    }
}
