using ApíRecognizer.Models;
using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using System.Reflection.Metadata;

namespace ApíRecognizer
{
    public class RecognizerService
    {

        public async Task<Invoice> GetDataInvoice(AnalyzeResult result)
        {
            var invoiceData = new Invoice();
            for (int i = 0; i < result.Documents.Count; i++)
            {
                Console.WriteLine($"Document {i}:");

                AnalyzedDocument document = result.Documents[i];

                if (document.Fields.TryGetValue("VendorName", out DocumentField? vendorNameField))
                {
                    if (vendorNameField.FieldType == DocumentFieldType.String)
                    {
                        string vendorName = vendorNameField.Value.AsString();
                        invoiceData.VendorData.Name = vendorName;
                        Console.WriteLine($"Vendor Name: '{vendorName}', with confidence {vendorNameField.Confidence}");
                    }
                }

                if (document.Fields.TryGetValue("CustomerName", out DocumentField? customerNameField))
                {
                    if (customerNameField.FieldType == DocumentFieldType.String)
                    {
                        string customerName = customerNameField.Value.AsString();
                        invoiceData.CustomerData.Name = customerName;
                        Console.WriteLine($"Customer Name: '{customerName}', with confidence {customerNameField.Confidence}");
                    }
                }

                //para las direcciones

                if (document.Fields.TryGetValue("VendorAddress", out DocumentField? vendorAddressField))
                {
                    if (vendorAddressField.FieldType == DocumentFieldType.Address)
                    {
                        AddressValue vendorAddress = vendorAddressField.Value.AsAddress();
                        invoiceData.VendorData.Address = vendorAddress.StreetAddress;
                        Console.WriteLine($"Customer Name: '{vendorAddress}', with confidence {vendorAddressField.Confidence}");
                    }
                }

                if (document.Fields.TryGetValue("CustomerAddress", out DocumentField? customerAddressField))
                {
                    if (customerAddressField.FieldType == DocumentFieldType.Address)
                    {
                        AddressValue customerAddress = customerAddressField.Value.AsAddress();
                        invoiceData.CustomerData.Address = customerAddress.StreetAddress;
                        Console.WriteLine($"Customer Name: '{customerAddress}', with confidence {customerAddressField.Confidence}");
                    }
                }
                //

                //para id de la factura
                if (document.Fields.TryGetValue("InvoiceId", out DocumentField? invoiceIdField))
                {
                    if (invoiceIdField.FieldType == DocumentFieldType.String)
                    {
                        string invoiceId = invoiceIdField.Value.AsString();
                        invoiceData.Id = invoiceId;
                        Console.WriteLine($"Customer Name: '{invoiceId}', with confidence {invoiceIdField.Confidence}");
                    }
                }
                //

                if (document.Fields.TryGetValue("Items", out DocumentField? itemsField))
                {
                    if (itemsField.FieldType == DocumentFieldType.List)
                    {
                        foreach (DocumentField itemField in itemsField.Value.AsList())
                        {
                            Product product = new Product();
                            Console.WriteLine("Item:");

                            if (itemField.FieldType == DocumentFieldType.Dictionary)
                            {
                                IReadOnlyDictionary<string, DocumentField> itemFields = itemField.Value.AsDictionary();

                                if (itemFields.TryGetValue("Description", out DocumentField? itemDescriptionField))
                                {
                                    if (itemDescriptionField.FieldType == DocumentFieldType.String)
                                    {
                                        string itemDescription = itemDescriptionField.Value.AsString();
                                        product.Name = itemDescription;

                                        Console.WriteLine($"  Description: '{itemDescription}', with confidence {itemDescriptionField.Confidence}");
                                    }
                                }
                                //para la cantidad
                                if (itemFields.TryGetValue("Quantity", out DocumentField? itemQuantityField))
                                {
                                    if (itemQuantityField.FieldType == DocumentFieldType.Double)
                                    {
                                        double itemQuantity = itemQuantityField.Value.AsDouble();
                                        product.Quantity = itemQuantity;
                                        //Console.WriteLine($"  Amount: '{itemAmount.Symbol}{itemAmount.Amount}', with confidence {itemAmountField.Confidence}");
                                    }
                                }

                                //para el precio unitario
                                if (itemFields.TryGetValue("UnitPrice", out DocumentField? itemUnitPriceField))
                                {
                                    if (itemUnitPriceField.FieldType == DocumentFieldType.Currency)
                                    {
                                        CurrencyValue itemUnitPrice = itemUnitPriceField.Value.AsCurrency();
                                        product.Price = itemUnitPrice.Amount;

                                        Console.WriteLine($"  Amount: '{itemUnitPrice.Symbol}{itemUnitPrice.Amount}', with confidence {itemUnitPriceField.Confidence}");
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
                            invoiceData.Products.Add(product);
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
                        invoiceData.Total = invoiceTotal.Amount;
                        Console.WriteLine($"Invoice Total: '{invoiceTotal.Symbol}{invoiceTotal.Amount}', with confidence {invoiceTotalField.Confidence}");
                    }
                }
            }

            return invoiceData;
        }
        public async Task<Receipt> GetReceipt(AnalyzeResult receipts)
        {

            Receipt receiptResult = new();

            foreach (AnalyzedDocument receipt in receipts.Documents)
            {
                if (receipt.Fields.TryGetValue("MerchantName", out DocumentField merchantNameField))
                {
                    if (merchantNameField.FieldType == DocumentFieldType.String)
                    {
                        string merchantName = merchantNameField.Value.AsString();
                        receiptResult.MerchantName = merchantName;
                        Console.WriteLine($"Merchant Name: '{merchantName}', with confidence {merchantNameField.Confidence}");
                    }
                }

                if (receipt.Fields.TryGetValue("TransactionDate", out DocumentField transactionDateField))
                {
                    if (transactionDateField.FieldType == DocumentFieldType.Date)
                    {
                        DateTimeOffset transactionDate = transactionDateField.Value.AsDate();
                        receiptResult.TransactionDate=transactionDate.UtcDateTime;
                        Console.WriteLine($"Transaction Date: '{transactionDate}', with confidence {transactionDateField.Confidence}");
                    }
                }
                if (receipt.Fields.TryGetValue("ReceiptNumber", out DocumentField receiptNumberField))
                {
                    if (receiptNumberField.FieldType == DocumentFieldType.String)
                    {
                        string receiptNumber = receiptNumberField.Value.AsString();
                        receiptResult.ReceiptNumber = receiptNumber;
           
                    }
                }
                if (receipt.Fields.TryGetValue("MerchantAddress", out DocumentField? merchantAddressField))
                {
                    if (merchantAddressField.FieldType == DocumentFieldType.Address)
                    {
                        AddressValue merchantAddress = merchantAddressField.Value.AsAddress();
                        receiptResult.MerchantAddress = merchantAddress.StreetAddress;
                        Console.WriteLine($"Customer Name: '{merchantAddress}', with confidence {merchantAddressField.Confidence}");
                    }
                }

                if (receipt.Fields.TryGetValue("Items", out DocumentField itemsField))
                {
                    if (itemsField.FieldType == DocumentFieldType.List)
                    {
                        foreach (DocumentField itemField in itemsField.Value.AsList())
                        {
                            Console.WriteLine("Item:");

                            if (itemField.FieldType == DocumentFieldType.Dictionary)
                            {
                                IReadOnlyDictionary<string, DocumentField> itemFields = itemField.Value.AsDictionary();

                                if (itemFields.TryGetValue("Description", out DocumentField itemDescriptionField))
                                {
                                    if (itemDescriptionField.FieldType == DocumentFieldType.String)
                                    {
                                        string itemDescription = itemDescriptionField.Value.AsString();

                                        Console.WriteLine($"  Description: '{itemDescription}', with confidence {itemDescriptionField.Confidence}");
                                    }
                                }

                                if (itemFields.TryGetValue("TotalPrice", out DocumentField itemTotalPriceField))
                                {
                                    if (itemTotalPriceField.FieldType == DocumentFieldType.Double)
                                    {
                                        double itemTotalPrice = itemTotalPriceField.Value.AsDouble();

                                        Console.WriteLine($"  Total Price: '{itemTotalPrice}', with confidence {itemTotalPriceField.Confidence}");
                                    }
                                }
                            }
                        }
                    }
                }

                if (receipt.Fields.TryGetValue("Total", out DocumentField totalField))
                {
                    if (totalField.FieldType == DocumentFieldType.Double)
                    {
                        double total = totalField.Value.AsDouble();

                        Console.WriteLine($"Total: '{total}', with confidence '{totalField.Confidence}'");
                    }
                }
            }
            return receiptResult;
        }
    }
}
