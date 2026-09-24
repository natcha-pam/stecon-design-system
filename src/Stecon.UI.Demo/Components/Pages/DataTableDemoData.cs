using System;
using System.Collections.Generic;
using System.Linq;

namespace Stecon.UI.Demo.Components.Pages;

internal sealed record ApprovalItem(string RequestNo, string Requester, string Project, DateOnly Submitted, string Status);

internal sealed record ResourceItem(
    string JobNo, string EmployeeId, string NameThai, string NameEnglish, string Position,
    string Department, decimal JanHours, decimal FebHours, decimal MarHours, string Remarks);

internal static class DataTableDemoData
{
    public static IReadOnlyList<ApprovalItem> Approvals { get; } = new List<ApprovalItem>
    {
        new("MR-00125", "Somchai Prasert", "Warehouse Expansion", new DateOnly(2026, 9, 1), "Pending"),
        new("MR-00126", "Nattaya Wongsawat", "IT Infrastructure Upgrade", new DateOnly(2026, 9, 3), "Approved"),
        new("MR-00127", "Kittipong Charoen", "Site Safety Equipment", new DateOnly(2026, 9, 5), "Rejected"),
        new("MR-00128", "Areeya Suksawat", "Fleet Maintenance", new DateOnly(2026, 9, 8), "Pending"),
        new("MR-00129", "Thanapon Rattanakul", "Office Renovation", new DateOnly(2026, 9, 10), "Approved"),
    };

    public static IReadOnlyList<ResourceItem> Resources { get; } = new List<ResourceItem>
    {
        new("JOB-2201", "P06343", "สมชาย ประเสริฐกุล", "Somchai Prasertkul", "Site Engineer",
            "Civil Works", 176, 168, 184, "Assigned to Warehouse Expansion project - long remark text to verify safe wrapping at every viewport width."),
        new("JOB-2202", "P06344", "ณัฐยา วงศ์สวัสดิ์", "Nattaya Wongsawat", "Project Manager",
            "Project Controls", 160, 160, 160, "โอนย้ายจากโครงการอื่นเมื่อเดือนที่แล้ว - a long Thai remark to verify wrapping."),
        new("JOB-2203", "P06345", "กิตติพงษ์ เจริญสุข", "Kittipong Charoensuk", "Safety Officer",
            "HSE", 172, 176, 168, "-"),
    };

    public static IReadOnlyList<ApprovalItem> GenerateServerPage(int page, int pageSize, out int totalCount)
    {
        var all = new List<ApprovalItem>();
        for (var i = 1; i <= 47; i++)
        {
            all.Add(new ApprovalItem($"MR-{20000 + i}", $"Requester {i}", $"Project {((i - 1) % 7) + 1}",
                new DateOnly(2026, 1, 1).AddDays(i), i % 3 == 0 ? "Approved" : i % 3 == 1 ? "Pending" : "Rejected"));
        }

        totalCount = all.Count;
        return all.Skip((page - 1) * pageSize).Take(pageSize).ToList();
    }
}
