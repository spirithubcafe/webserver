using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpirithubCafe.Web.Data;

namespace SpirithubCafe.Web.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/[controller]")]
public class ReportsExportController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ReportsExportController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("excel")]
    public async Task<IActionResult> ExportToExcel(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        // Set default date range if not provided
        var start = startDate ?? DateTime.Now.AddDays(-30);
        var end = endDate ?? DateTime.Now;

        // Fetch all data
        var orders = await _context.Orders
            .Include(o => o.OrderItems)
            .Where(o => o.CreatedAt >= start && o.CreatedAt <= end)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

        var products = await _context.Products
            .Include(p => p.Variants)
            .ToListAsync();

        var users = await _context.Users.ToListAsync();

        // Create Excel workbook
        using var workbook = new XLWorkbook();

        // Add Summary Sheet
        var summarySheet = workbook.Worksheets.Add("Summary");
        AddSummarySheet(summarySheet, orders, products);

        // Add Orders Sheet
        var ordersSheet = workbook.Worksheets.Add("Orders");
        AddOrdersSheet(ordersSheet, orders);

        // Add Order Items Sheet
        var itemsSheet = workbook.Worksheets.Add("Order Items");
        AddOrderItemsSheet(itemsSheet, orders);

        // Add Products Sheet
        var productsSheet = workbook.Worksheets.Add("Products");
        AddProductsSheet(productsSheet, products);

        // Add Customers Sheet
        var customersSheet = workbook.Worksheets.Add("Customers");
        AddCustomersSheet(customersSheet, users);

        // Add Daily Sales Sheet
        var dailySalesSheet = workbook.Worksheets.Add("Daily Sales");
        AddDailySalesSheet(dailySalesSheet, orders);

        // Save to memory stream
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        stream.Position = 0;

        var fileName = $"SpirithubCafe_Report_{start:yyyy-MM-dd}_to_{end:yyyy-MM-dd}.xlsx";
        return File(stream.ToArray(), 
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
            fileName);
    }

    private void AddSummarySheet(IXLWorksheet sheet, List<Domain.Entities.Order> orders, List<Domain.Entities.Product> products)
    {
        // Header
        sheet.Cell(1, 1).Value = "SpirithubCafe - Sales Report Summary";
        sheet.Cell(1, 1).Style.Font.Bold = true;
        sheet.Cell(1, 1).Style.Font.FontSize = 16;
        sheet.Range(1, 1, 1, 2).Merge();

        sheet.Cell(3, 1).Value = "Report Date:";
        sheet.Cell(3, 2).Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm");

        // Key Metrics
        var totalRevenue = orders.Sum(o => o.TotalAmount);
        var totalOrders = orders.Count;
        var averageOrderValue = totalOrders > 0 ? totalRevenue / totalOrders : 0;
        var totalCustomers = orders.Select(o => o.UserId).Distinct().Count();

        sheet.Cell(5, 1).Value = "Total Revenue (OMR)";
        sheet.Cell(5, 2).Value = totalRevenue;
        sheet.Cell(5, 2).Style.NumberFormat.Format = "#,##0.000";

        sheet.Cell(6, 1).Value = "Total Orders";
        sheet.Cell(6, 2).Value = totalOrders;

        sheet.Cell(7, 1).Value = "Average Order Value (OMR)";
        sheet.Cell(7, 2).Value = averageOrderValue;
        sheet.Cell(7, 2).Style.NumberFormat.Format = "#,##0.000";

        sheet.Cell(8, 1).Value = "Total Customers";
        sheet.Cell(8, 2).Value = totalCustomers;

        sheet.Cell(9, 1).Value = "Total Products";
        sheet.Cell(9, 2).Value = products.Count;

        // Order Status Breakdown
        sheet.Cell(11, 1).Value = "Orders by Status";
        sheet.Cell(11, 1).Style.Font.Bold = true;
        
        var statusGroups = orders.GroupBy(o => o.Status).ToList();
        int row = 12;
        foreach (var group in statusGroups)
        {
            sheet.Cell(row, 1).Value = group.Key;
            sheet.Cell(row, 2).Value = group.Count();
            sheet.Cell(row, 3).Value = group.Sum(o => o.TotalAmount);
            sheet.Cell(row, 3).Style.NumberFormat.Format = "#,##0.000";
            row++;
        }

        // Auto-fit columns
        sheet.Columns().AdjustToContents();
    }

    private void AddOrdersSheet(IXLWorksheet sheet, List<Domain.Entities.Order> orders)
    {
        // Headers
        sheet.Cell(1, 1).Value = "Order Number";
        sheet.Cell(1, 2).Value = "Order Date";
        sheet.Cell(1, 3).Value = "Customer Name";
        sheet.Cell(1, 4).Value = "Email";
        sheet.Cell(1, 5).Value = "Phone";
        sheet.Cell(1, 6).Value = "Status";
        sheet.Cell(1, 7).Value = "Payment Status";
        sheet.Cell(1, 8).Value = "Subtotal (OMR)";
        sheet.Cell(1, 9).Value = "Tax (OMR)";
        sheet.Cell(1, 10).Value = "Shipping (OMR)";
        sheet.Cell(1, 11).Value = "Total (OMR)";
        sheet.Cell(1, 12).Value = "Address";
        sheet.Cell(1, 13).Value = "Notes";

        // Style headers
        var headerRange = sheet.Range(1, 1, 1, 13);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

        // Data
        int row = 2;
        foreach (var order in orders)
        {
            sheet.Cell(row, 1).Value = order.OrderNumber;
            sheet.Cell(row, 2).Value = order.CreatedAt;
            sheet.Cell(row, 3).Value = $"{order.FirstName} {order.LastName}";
            sheet.Cell(row, 4).Value = order.Email;
            sheet.Cell(row, 5).Value = order.Phone;
            sheet.Cell(row, 6).Value = order.Status;
            sheet.Cell(row, 7).Value = order.PaymentStatus;
            sheet.Cell(row, 8).Value = order.SubTotal;
            sheet.Cell(row, 9).Value = order.TaxAmount;
            sheet.Cell(row, 10).Value = order.ShippingCost;
            sheet.Cell(row, 11).Value = order.TotalAmount;
            sheet.Cell(row, 12).Value = $"{order.AddressLine1}, {order.AddressLine2}";
            sheet.Cell(row, 13).Value = order.Notes;

            // Format numbers
            sheet.Cell(row, 8).Style.NumberFormat.Format = "#,##0.000";
            sheet.Cell(row, 9).Style.NumberFormat.Format = "#,##0.000";
            sheet.Cell(row, 10).Style.NumberFormat.Format = "#,##0.000";
            sheet.Cell(row, 11).Style.NumberFormat.Format = "#,##0.000";

            row++;
        }

        sheet.Columns().AdjustToContents();
    }

    private void AddOrderItemsSheet(IXLWorksheet sheet, List<Domain.Entities.Order> orders)
    {
        // Headers
        sheet.Cell(1, 1).Value = "Order Number";
        sheet.Cell(1, 2).Value = "Order Date";
        sheet.Cell(1, 3).Value = "Product Name";
        sheet.Cell(1, 4).Value = "Variant Info";
        sheet.Cell(1, 5).Value = "Quantity";
        sheet.Cell(1, 6).Value = "Unit Price (OMR)";
        sheet.Cell(1, 7).Value = "Tax (OMR)";
        sheet.Cell(1, 8).Value = "Total (OMR)";

        // Style headers
        var headerRange = sheet.Range(1, 1, 1, 8);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

        // Data
        int row = 2;
        foreach (var order in orders)
        {
            foreach (var item in order.OrderItems)
            {
                sheet.Cell(row, 1).Value = order.OrderNumber;
                sheet.Cell(row, 2).Value = order.CreatedAt;
                sheet.Cell(row, 3).Value = item.ProductName;
                sheet.Cell(row, 4).Value = item.VariantInfo;
                sheet.Cell(row, 5).Value = item.Quantity;
                sheet.Cell(row, 6).Value = item.UnitPrice;
                sheet.Cell(row, 7).Value = item.TaxAmount;
                sheet.Cell(row, 8).Value = item.TotalAmount;

                // Format numbers
                sheet.Cell(row, 6).Style.NumberFormat.Format = "#,##0.000";
                sheet.Cell(row, 7).Style.NumberFormat.Format = "#,##0.000";
                sheet.Cell(row, 8).Style.NumberFormat.Format = "#,##0.000";

                row++;
            }
        }

        sheet.Columns().AdjustToContents();
    }

    private void AddProductsSheet(IXLWorksheet sheet, List<Domain.Entities.Product> products)
    {
        // Headers
        sheet.Cell(1, 1).Value = "SKU";
        sheet.Cell(1, 2).Value = "Product Name";
        sheet.Cell(1, 3).Value = "Product Name (AR)";
        sheet.Cell(1, 4).Value = "Description";
        sheet.Cell(1, 5).Value = "Origin";
        sheet.Cell(1, 6).Value = "Roast Level";
        sheet.Cell(1, 7).Value = "Process";
        sheet.Cell(1, 8).Value = "Is Active";
        sheet.Cell(1, 9).Value = "Is Featured";
        sheet.Cell(1, 10).Value = "Variants Count";
        sheet.Cell(1, 11).Value = "Total Stock";

        // Style headers
        var headerRange = sheet.Range(1, 1, 1, 11);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

        // Data
        int row = 2;
        foreach (var product in products)
        {
            sheet.Cell(row, 1).Value = product.Sku;
            sheet.Cell(row, 2).Value = product.Name;
            sheet.Cell(row, 3).Value = product.NameAr;
            sheet.Cell(row, 4).Value = product.Description;
            sheet.Cell(row, 5).Value = product.Origin;
            sheet.Cell(row, 6).Value = product.RoastLevel;
            sheet.Cell(row, 7).Value = product.Process;
            sheet.Cell(row, 8).Value = product.IsActive ? "Yes" : "No";
            sheet.Cell(row, 9).Value = product.IsFeatured ? "Yes" : "No";
            sheet.Cell(row, 10).Value = product.Variants?.Count ?? 0;
            sheet.Cell(row, 11).Value = product.Variants?.Sum(v => v.StockQuantity) ?? 0;

            row++;
        }

        sheet.Columns().AdjustToContents();
    }

    private void AddCustomersSheet(IXLWorksheet sheet, List<Domain.Entities.ApplicationUser> users)
    {
        // Headers
        sheet.Cell(1, 1).Value = "User Name";
        sheet.Cell(1, 2).Value = "Email";
        sheet.Cell(1, 3).Value = "Email Confirmed";
        sheet.Cell(1, 4).Value = "Phone Number";
        sheet.Cell(1, 5).Value = "Last Login Date";

        // Style headers
        var headerRange = sheet.Range(1, 1, 1, 5);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

        // Data
        int row = 2;
        foreach (var user in users)
        {
            sheet.Cell(row, 1).Value = user.UserName;
            sheet.Cell(row, 2).Value = user.Email;
            sheet.Cell(row, 3).Value = user.EmailConfirmed ? "Yes" : "No";
            sheet.Cell(row, 4).Value = user.PhoneNumber;
            sheet.Cell(row, 5).Value = user.LastLoginDate?.ToString("yyyy-MM-dd HH:mm") ?? "Never";

            row++;
        }

        sheet.Columns().AdjustToContents();
    }

    private void AddDailySalesSheet(IXLWorksheet sheet, List<Domain.Entities.Order> orders)
    {
        // Headers
        sheet.Cell(1, 1).Value = "Date";
        sheet.Cell(1, 2).Value = "Orders Count";
        sheet.Cell(1, 3).Value = "Revenue (OMR)";

        // Style headers
        var headerRange = sheet.Range(1, 1, 1, 3);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

        // Group by date
        var dailySales = orders
            .GroupBy(o => o.CreatedAt.Date)
            .Select(g => new
            {
                Date = g.Key,
                OrderCount = g.Count(),
                Revenue = g.Sum(o => o.TotalAmount)
            })
            .OrderBy(d => d.Date)
            .ToList();

        // Data
        int row = 2;
        foreach (var day in dailySales)
        {
            sheet.Cell(row, 1).Value = day.Date;
            sheet.Cell(row, 2).Value = day.OrderCount;
            sheet.Cell(row, 3).Value = day.Revenue;
            sheet.Cell(row, 3).Style.NumberFormat.Format = "#,##0.000";

            row++;
        }

        sheet.Columns().AdjustToContents();
    }
}
