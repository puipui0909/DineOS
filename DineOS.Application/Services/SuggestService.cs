using DineOS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

public class SuggestService
{
    // ================= MAIN =================
    public List<MenuItem> Suggest(string message, List<MenuItem> menu)
    {
        var msg = message.ToLower();

        var matchedTags = ExtractTagsFromMessage(msg);

        var result = menu
            .Where(m =>
            {
                var tags = GetTagsFromDescription(m.Description);
                return tags.Any(t => matchedTags.Contains(t));
            })
            .ToList();

        // 🔥 fallback (QUAN TRỌNG)
        if (!result.Any())
        {
            result = menu.Take(5).ToList();
        }

        return result.Take(5).ToList();
    }

    // ================= EXTRACT USER INTENT =================
    private List<string> ExtractTagsFromMessage(string msg)
    {
        var tags = new List<string>();

        if (msg.Contains("nước") || msg.Contains("bun") || msg.Contains("phở") || msg.Contains("hu tieu"))
            tags.Add("nuoc");

        if (msg.Contains("cay"))
            tags.Add("cay");

        if (msg.Contains("ngọt"))
            tags.Add("ngot");

        if (msg.Contains("chua"))
            tags.Add("chua");

        if (msg.Contains("mát") || msg.Contains("lạnh"))
            tags.Add("mat");

        if (msg.Contains("nóng"))
            tags.Add("nong");

        if (msg.Contains("nhẹ"))
            tags.Add("nhe");

        if (msg.Contains("no"))
            tags.Add("no");

        if (msg.Contains("chiên") || msg.Contains("rán"))
            tags.Add("chien");

        if (msg.Contains("nướng"))
            tags.Add("nuong");

        if (msg.Contains("hải sản"))
            tags.Add("hai_san");

        if (msg.Contains("bò"))
            tags.Add("bo");

        if (msg.Contains("gà"))
            tags.Add("ga");

        if (msg.Contains("heo") || msg.Contains("sườn"))
            tags.Add("heo");

        if (msg.Contains("uống"))
            tags.Add("do_uong");

        if (msg.Contains("tráng miệng"))
            tags.Add("trang_mieng");

        return tags;
    }

    // ================= PARSE DESCRIPTION =================
    private List<string> GetTagsFromDescription(string description)
    {
        if (string.IsNullOrEmpty(description))
            return new List<string>();

        var raw = description.ToLower().Split(',');

        var tags = new List<string>();

        foreach (var t in raw)
        {
            var tag = t.Trim();

            if (tag.Contains("nước")) tags.Add("nuoc");
            if (tag.Contains("cay")) tags.Add("cay");
            if (tag.Contains("ngọt")) tags.Add("ngot");
            if (tag.Contains("chua")) tags.Add("chua");
            if (tag.Contains("mát") || tag.Contains("lạnh")) tags.Add("mat");
            if (tag.Contains("nóng")) tags.Add("nong");
            if (tag.Contains("nhẹ")) tags.Add("nhe");
            if (tag.Contains("no")) tags.Add("no");
            if (tag.Contains("chiên")) tags.Add("chien");
            if (tag.Contains("nướng")) tags.Add("nuong");
            if (tag.Contains("hải sản")) tags.Add("hai_san");
            if (tag.Contains("bò")) tags.Add("bo");
            if (tag.Contains("gà")) tags.Add("ga");
            if (tag.Contains("heo") || tag.Contains("sườn")) tags.Add("heo");
            if (tag.Contains("đồ uống")) tags.Add("do_uong");
            if (tag.Contains("tráng miệng")) tags.Add("trang_mieng");
        }

        return tags;
    }
}