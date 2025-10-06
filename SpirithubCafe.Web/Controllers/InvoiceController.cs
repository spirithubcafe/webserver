using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SpirithubCafe.Web.Data;

namespace SpirithubCafe.Web.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/[controller]")]
public class InvoiceController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<InvoiceController> _logger;

    public InvoiceController(ApplicationDbContext context, ILogger<InvoiceController> logger)
    {
        _context = context;
        _logger = logger;
        QuestPDF.Settings.License = LicenseType.Community;
    }

    [HttpGet("{orderId}")]
    public async Task<IActionResult> GenerateInvoice(int orderId)
    {
        try
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .Include(o => o.Country)
                .Include(o => o.City)
                .Include(o => o.ShippingMethod)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                _logger.LogWarning("Order with ID {OrderId} not found", orderId);
                return NotFound($"Order with ID {orderId} not found");
            }

            _logger.LogInformation("Generating invoice for order {OrderNumber}", order.OrderNumber);

            // Get company info from settings
            var companyName = await GetSettingValue("CompanyName") ?? "SpirithubCafe";
            var companyAddress = await GetSettingValue("CompanyAddress") ?? "";
            var companyPhone = await GetSettingValue("CompanyPhone") ?? "";
            var companyEmail = await GetSettingValue("CompanyEmail") ?? "";

            var pdfBytes = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    page.Header().Element(ComposeHeader);
                    page.Content().Element(ComposeContent);
                    page.Footer().Element(ComposeFooter);
                });

                void ComposeHeader(IContainer container)
                {
                    container.Row(row =>
                    {
                        row.RelativeItem().Column(column =>
                        {
                            column.Item().Text(companyName)
                                .FontSize(20)
                                .Bold()
                                .FontColor(Colors.Brown.Darken2);

                            column.Item().PaddingTop(5).Text(companyAddress)
                                .FontSize(10);

                            column.Item().Text($"Tel: {companyPhone}")
                                .FontSize(10);

                            column.Item().Text($"Email: {companyEmail}")
                                .FontSize(10);
                        });

                        row.RelativeItem().AlignRight().Column(column =>
                        {
                            column.Item().Text("INVOICE")
                                .FontSize(20)
                                .Bold()
                                .FontColor(Colors.Brown.Darken2);

                            column.Item().PaddingTop(5).Text($"Invoice #: {order.OrderNumber}")
                                .FontSize(10);

                            column.Item().Text($"Date: {order.CreatedAt:dd/MM/yyyy}")
                                .FontSize(10);

                            column.Item().Text($"Status: {order.Status}")
                                .FontSize(10)
                                .FontColor(GetStatusColor(order.Status));
                        });
                    });
                }

                void ComposeContent(IContainer container)
                {
                    container.PaddingVertical(20).Column(column =>
                    {
                        // Customer Information
                        column.Item().PaddingBottom(10).Row(row =>
                        {
                            row.RelativeItem().Column(col =>
                            {
                                col.Item().Text("Bill To")
                                    .FontSize(12)
                                    .Bold()
                                    .FontColor(Colors.Brown.Darken1);

                                col.Item().PaddingTop(5).Text($"{order.FirstName} {order.LastName}")
                                    .FontSize(10);

                                col.Item().Text(order.Email)
                                    .FontSize(10);

                                col.Item().Text(order.Phone)
                                    .FontSize(10);

                                col.Item().Text($"{order.AddressLine1}")
                                    .FontSize(10);

                                if (!string.IsNullOrEmpty(order.AddressLine2))
                                {
                                    col.Item().Text(order.AddressLine2)
                                        .FontSize(10);
                                }

                                col.Item().Text($"{order.City?.Name ?? ""}, {order.Country?.Name ?? ""}")
                                    .FontSize(10);

                                col.Item().Text($"Postal Code: {order.PostalCode}")
                                    .FontSize(10);
                            });

                            row.RelativeItem().AlignRight().Column(col =>
                            {
                                col.Item().Text("Shipping Details")
                                    .FontSize(12)
                                    .Bold()
                                    .FontColor(Colors.Brown.Darken1);

                                col.Item().PaddingTop(5).Text($"Method: {order.ShippingMethod?.Name ?? "N/A"}")
                                    .FontSize(10);

                                if (!string.IsNullOrEmpty(order.TrackingNumber))
                                {
                                    col.Item().Text($"Tracking: {order.TrackingNumber}")
                                        .FontSize(10);
                                }

                                col.Item().Text($"Payment: {order.PaymentStatus}")
                                    .FontSize(10)
                                    .FontColor(GetPaymentStatusColor(order.PaymentStatus));
                            });
                        });

                        column.Item().PaddingVertical(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                        // Items Table
                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(40);  // #
                                columns.RelativeColumn(3);    // Product
                                columns.RelativeColumn(2);    // Variant
                                columns.RelativeColumn(1);    // Qty
                                columns.RelativeColumn(1.5f); // Price
                                columns.RelativeColumn(1);    // Tax
                                columns.RelativeColumn(1.5f); // Total
                            });

                            // Header
                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("#");
                                header.Cell().Element(CellStyle).Text("Product");
                                header.Cell().Element(CellStyle).Text("Variant");
                                header.Cell().Element(CellStyle).AlignRight().Text("Qty");
                                header.Cell().Element(CellStyle).AlignRight().Text("Price");
                                header.Cell().Element(CellStyle).AlignRight().Text("Tax");
                                header.Cell().Element(CellStyle).AlignRight().Text("Total");

                                static IContainer CellStyle(IContainer container) => container
                                    .Background(Colors.Brown.Lighten3)
                                    .Padding(5)
                                    .Border(1)
                                    .BorderColor(Colors.Grey.Lighten2);
                            });

                            // Items
                            int itemNumber = 1;
                            foreach (var item in order.OrderItems)
                            {
                                table.Cell().Element(BodyCellStyle).Text(itemNumber.ToString());
                                table.Cell().Element(BodyCellStyle).Text(item.ProductName);
                                table.Cell().Element(BodyCellStyle).Text(item.VariantInfo ?? "-");
                                table.Cell().Element(BodyCellStyle).AlignRight().Text(item.Quantity.ToString());
                                table.Cell().Element(BodyCellStyle).AlignRight().Text($"{item.UnitPrice:N3} OMR");
                                table.Cell().Element(BodyCellStyle).AlignRight().Text($"{item.TaxAmount:N3} OMR");
                                table.Cell().Element(BodyCellStyle).AlignRight().Text($"{item.TotalAmount:N3} OMR");

                                itemNumber++;
                            }

                            static IContainer BodyCellStyle(IContainer container) => container
                                .Padding(5)
                                .Border(1)
                                .BorderColor(Colors.Grey.Lighten2);
                        });

                        column.Item().PaddingTop(15).AlignRight().Column(summaryColumn =>
                        {
                            summaryColumn.Item().Row(row =>
                            {
                                row.ConstantItem(150).Text("Subtotal:")
                                    .FontSize(10);
                                row.ConstantItem(100).AlignRight().Text($"{order.SubTotal:N3} OMR")
                                    .FontSize(10);
                            });

                            summaryColumn.Item().Row(row =>
                            {
                                row.ConstantItem(150).Text("Tax:")
                                    .FontSize(10);
                                row.ConstantItem(100).AlignRight().Text($"{order.TaxAmount:N3} OMR")
                                    .FontSize(10);
                            });

                            summaryColumn.Item().Row(row =>
                            {
                                row.ConstantItem(150).Text("Shipping:")
                                    .FontSize(10);
                                row.ConstantItem(100).AlignRight().Text($"{order.ShippingCost:N3} OMR")
                                    .FontSize(10);
                            });

                            summaryColumn.Item().PaddingTop(5).BorderTop(2).BorderColor(Colors.Brown.Darken2);

                            summaryColumn.Item().PaddingTop(5).Row(row =>
                            {
                                row.ConstantItem(150).Text("TOTAL:")
                                    .FontSize(12)
                                    .Bold()
                                    .FontColor(Colors.Brown.Darken2);
                                row.ConstantItem(100).AlignRight().Text($"{order.TotalAmount:N3} OMR")
                                    .FontSize(12)
                                    .Bold()
                                    .FontColor(Colors.Brown.Darken2);
                            });
                        });

                        // Notes
                        if (!string.IsNullOrEmpty(order.Notes))
                        {
                            column.Item().PaddingTop(20).Column(col =>
                            {
                                col.Item().Text("Notes")
                                    .FontSize(11)
                                    .Bold()
                                    .FontColor(Colors.Brown.Darken1);

                                col.Item().PaddingTop(5)
                                    .Background(Colors.Grey.Lighten4)
                                    .Padding(10)
                                    .Text(order.Notes)
                                    .FontSize(10);
                            });
                        }
                    });
                }

                void ComposeFooter(IContainer container)
                {
                    container.AlignCenter().Text(text =>
                    {
                        text.Span("Thank you for your business!")
                            .FontSize(10)
                            .FontColor(Colors.Grey.Medium);
                    });
                }
            }).GeneratePdf();

            _logger.LogInformation("Invoice generated successfully for order {OrderNumber}", order.OrderNumber);
            return File(pdfBytes, "application/pdf", $"Invoice_{order.OrderNumber}.pdf");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating invoice for order {OrderId}", orderId);
            return StatusCode(500, $"Error generating invoice: {ex.Message}");
        }
    }

    private string GetStatusColor(string status)
    {
        return status.ToLower() switch
        {
            "pending" => Colors.Orange.Medium,
            "processing" => Colors.Blue.Medium,
            "shipped" => Colors.Indigo.Medium,
            "delivered" => Colors.Green.Medium,
            "cancelled" => Colors.Red.Medium,
            _ => Colors.Grey.Medium
        };
    }

    private string GetPaymentStatusColor(string paymentStatus)
    {
        return paymentStatus.ToLower() switch
        {
            "paid" => Colors.Green.Medium,
            "unpaid" => Colors.Orange.Medium,
            "failed" => Colors.Red.Medium,
            "refunded" => Colors.Purple.Medium,
            _ => Colors.Grey.Medium
        };
    }

    private async Task<string?> GetSettingValue(string key)
    {
        var setting = await _context.Settings.FirstOrDefaultAsync(s => s.Key == key);
        return setting?.Value;
    }
}
