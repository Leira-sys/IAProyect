using ApíRecognizer;
using ApíRecognizer.Models;
using Azure;
using Azure.AI.FormRecognizer;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using System.Buffers.Text;
using System.IO;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
{
    builder.AllowAnyOrigin()
                 .AllowAnyMethod()
                 .AllowAnyHeader();
}));

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseCors("MyPolicy");
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/getLogsAzure", () =>
{
    using (var context = new AzureRecognizerContext())
    {
        return context.LogAzures.ToList();
    }
});
app.MapGet("/{id}", (int id) =>
{
    using (var context = new AzureRecognizerContext())
    {
        var find = context.LogAzures.Find(id);
        return Results.Ok(find);
    }
});

app.MapPost("/invoice", async (PdfRequest request) =>
{
    try
    {
        byte[] bytes = Convert.FromBase64String(request.Base64);
        using Stream stream = new MemoryStream(bytes);

        string endpoint = "";
        string key = "";
        AzureKeyCredential credential = new AzureKeyCredential(key);
        DocumentAnalysisClient client = new DocumentAnalysisClient(new Uri(endpoint), credential);

        AnalyzeDocumentOperation operation;
        if (request.Type=="FACTURA")
        {
             operation = await client.AnalyzeDocumentAsync(WaitUntil.Completed, "prebuilt-invoice", stream);
        }
        else
        {
             operation = await client.AnalyzeDocumentAsync(WaitUntil.Completed, "prebuilt-receipt", stream);
        }

        AnalyzeResult result = operation.Value;

        RecognizerService pdfRecognizer = new RecognizerService();
        Invoice invoice = null;
        Receipt receipt = null;
        ResponseRecognizer responseRecognizercs = new();
        string json = string.Empty;
        string type = string.Empty;

        if (request.Type == "FACTURA")
        {
            invoice = await pdfRecognizer.GetDataInvoice(result);
            json = JsonSerializer.Serialize(invoice);
            type = request.Type;
            responseRecognizercs.Invoice = invoice;
            responseRecognizercs.Type = type;
        }
        else
        {
            receipt = await pdfRecognizer.GetReceipt(result);
            json = JsonSerializer.Serialize(receipt);
            type = request.Type;
            responseRecognizercs.Receipt = receipt;
            responseRecognizercs.Type = type;
        }
            
        
        //else
        //{
           
        //}
        //var json = "{\"Id\":\"INV-100\",\"CustomerData\":{\"Name\":\"MICROSOFT CORPORATION\",\"Address\":\"123 Other St\"},\"VendorData\":{\"Name\":\"CONTOSO LTD.\",\"Address\":\"123 456th St\"},\"InvoiceNumber\":\"\",\"Products\":[{\"Quantity\":2,\"Price\":30,\"Name\":\"Consulting Services\"},{\"Quantity\":3,\"Price\":10,\"Name\":\"Document Fee\"},{\"Quantity\":10,\"Price\":1,\"Name\":\"Printing Fee\"}],\"Total\":110}";
        //return JsonSerializer.Deserialize<Invoice>(json);
  
       

        LogAzure log = new LogAzure();
        //log.Id = 1;
        log.Tipo =type;
        log.Descripcion = "";
        log.FechaHora = DateTime.Now;
        log.DocumentJson = json;
        log.Estado = true;


        using (var context = new AzureRecognizerContext())
        {
            context.LogAzures.Add(log);
            context.SaveChanges();
            //return Results.Ok(cliente);
        }

        return responseRecognizercs;

    }
    catch (Exception ex)
    {

        throw;
    }

})
.WithName("GetDataInvoice")
.WithOpenApi();

app.Run();


