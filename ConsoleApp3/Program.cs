using Azure;
using Azure.AI.FormRecognizer;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using Azure.AI.FormRecognizer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ConsoleApp3
{
   class Program
  {
    static async Task Main(string[] args)
    {
      /*
        Remember to remove the key from your code when you're done, and never post it publicly. For production, use
        secure methods to store and access your credentials. For more information, see 
        https://docs.microsoft.com/en-us/azure/cognitive-services/cognitive-services-security?tabs=command-line%2Ccsharp#environment-variables-and-application-configuration
      */
      string endpoint = "https://demoproyectoia.cognitiveservices.azure.com/";
      string key = "dcd82622b77e4d21aca6b050aec2c5af";
      AzureKeyCredential credential = new AzureKeyCredential(key);
      DocumentAnalysisClient client = new DocumentAnalysisClient(new Uri(endpoint), credential);

      // sample document
      Uri invoiceUri = new Uri("https://documentintelligence.ai.azure.com/documents/samples/prebuilt/invoice-english.pdf");

      AnalyzeDocumentOperation operation = await client.AnalyzeDocumentFromUriAsync(WaitUntil.Completed, "prebuilt-invoice", invoiceUri);

      AnalyzeResult result = operation.Value;

      for (int i = 0; i < result.Documents.Count; i++)
      {
        Console.WriteLine($"Document {i}:");

        AnalyzedDocument document = result.Documents[i];

        if (document.Fields.TryGetValue("VendorName", out DocumentField? vendorNameField))
        {
          if (vendorNameField.FieldType == DocumentFieldType.String)
          {
            string vendorName = vendorNameField.Value.AsString();
            Console.WriteLine($"Vendor Name: '{vendorName}', with confidence {vendorNameField.Confidence}");
          }
        }

        if (document.Fields.TryGetValue("CustomerName", out DocumentField? customerNameField))
        {
          if (customerNameField.FieldType == DocumentFieldType.String)
          {
            string customerName = customerNameField.Value.AsString();
            Console.WriteLine($"Customer Name: '{customerName}', with confidence {customerNameField.Confidence}");
          }
        }

        if (document.Fields.TryGetValue("Items", out DocumentField? itemsField))
        {
          if (itemsField.FieldType == DocumentFieldType.List)
          {
            foreach (DocumentField itemField in itemsField.Value.AsList())
            {
              Console.WriteLine("Item:");

              if (itemField.FieldType == DocumentFieldType.Dictionary)
              {
                IReadOnlyDictionary<string, DocumentField> itemFields = itemField.Value.AsDictionary();

                if (itemFields.TryGetValue("Description", out DocumentField? itemDescriptionField))
                {
                  if (itemDescriptionField.FieldType == DocumentFieldType.String)
                  {
                    string itemDescription = itemDescriptionField.Value.AsString();

                    Console.WriteLine($"  Description: '{itemDescription}', with confidence {itemDescriptionField.Confidence}");
                  }
                }

                if (itemFields.TryGetValue("Amount", out DocumentField? itemAmountField))
                {
                  if (itemAmountField.FieldType == DocumentFieldType.Currency)
                  {
                    CurrencyValue itemAmount = itemAmountField.Value.AsCurrency();

                    Console.WriteLine($"  Amount: '{itemAmount.Symbol}{itemAmount.Amount}', with confidence {itemAmountField.Confidence}");
                  }
                }
              }
            }
          }
        }

        if (document.Fields.TryGetValue("SubTotal", out DocumentField? subTotalField))
        {
          if (subTotalField.FieldType == DocumentFieldType.Currency)
          {
            CurrencyValue subTotal = subTotalField.Value.AsCurrency();
            Console.WriteLine($"Sub Total: '{subTotal.Symbol}{subTotal.Amount}', with confidence {subTotalField.Confidence}");
          }
        }

        if (document.Fields.TryGetValue("TotalTax", out DocumentField? totalTaxField))
        {
          if (totalTaxField.FieldType == DocumentFieldType.Currency)
          {
            CurrencyValue totalTax = totalTaxField.Value.AsCurrency();
            Console.WriteLine($"Total Tax: '{totalTax.Symbol}{totalTax.Amount}', with confidence {totalTaxField.Confidence}");
          }
        }

        if (document.Fields.TryGetValue("InvoiceTotal", out DocumentField? invoiceTotalField))
        {
          if (invoiceTotalField.FieldType == DocumentFieldType.Currency)
          {
            CurrencyValue invoiceTotal = invoiceTotalField.Value.AsCurrency();
            Console.WriteLine($"Invoice Total: '{invoiceTotal.Symbol}{invoiceTotal.Amount}', with confidence {invoiceTotalField.Confidence}");
          }
        }
      }

    }
    //static async Task Main(string[] args)
    //{
    //  string endpoint = "https://demoproyectoia.cognitiveservices.azure.com/";
    //  string apiKey = "dcd82622b77e4d21aca6b050aec2c5af";

    //  FormRecognizerClient client= new FormRecognizerClient(new Uri(endpoint),new Azure.AzureKeyCredential(apiKey));
    //  string filePath = "C:\\Users\\eespinoza\\Documents\\Document1.pdf";

    //  using var stream = new FileStream(filePath, FileMode.Open);

    //  Response<FormPageCollection> response=await client.StartRecognizeContentAsync(stream).WaitForCompletionAsync();
    //  FormPageCollection formPages = response.Value;

    //  foreach (FormPage page in formPages)
    //  {
    //    for (int i = 0; i < page.Lines.Count; i++)
    //    {
    //      FormLine line = page.Lines[i];
    //      Console.WriteLine($"  Line {i} has {line.Words.Count} {(line.Words.Count==1?"word":"words")}, and text: '{line.Text}'.");

    //      if (line.Appearance!=null)
    //      {
    //        if (line.Appearance.StyleName==TextStyleName.Handwriting&&line.Appearance.StyleConfidence>0.8)
    //        {
    //          Console.WriteLine("The text is handwritten");
    //        }
    //      }

    //      Console.WriteLine("  Its bounding bos is:");
    //      Console.WriteLine($"  Upper left => X: {line.BoundingBox[0].X},Y={line.BoundingBox[0].Y}");
    //      Console.WriteLine($"  Upper right => X: {line.BoundingBox[1].X},Y={line.BoundingBox[1].Y}");
    //      Console.WriteLine($"  Upper right => X: {line.BoundingBox[2].X},Y={line.BoundingBox[2].Y}");
    //      Console.WriteLine($"  Upper left => X: {line.BoundingBox[3].X},Y={line.BoundingBox[3].Y}");
    //    }
    //    Console.WriteLine($"Form Page {page.PageNumber} has {page.Lines.Count} lines.");
    //  }

    //}
  }
}
