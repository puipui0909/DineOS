using DineOS.Application.Common.Interfaces;
using DineOS.Application.DTOs.Ai;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;

public class AiService : IAiService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly IApplicationDbContext _context;

    public AiService(IApplicationDbContext context, IConfiguration config, HttpClient httpClient)
    {
        _context = context;
        _httpClient = httpClient;
        _apiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY")
          ?? throw new Exception("Missing GEMINI_API_KEY");

        Console.WriteLine($"==== CHECK API KEY: {(!string.IsNullOrEmpty(_apiKey) ? "Đã đọc được" : "TRỐNG RỖNG!")} ====");
        if (!string.IsNullOrEmpty(_apiKey))
        {
            Console.WriteLine($"Key bắt đầu bằng: {_apiKey.Substring(0, 5)}...");
        }
    }

    public async Task<List<AiSuggestionResponse>> GetSuggestion(string userInput)
    {
        try
        {
            // 1. Lấy menu từ Database
            var menu = await _context.MenuItems
                .Where(x => x.IsAvailable)
                .Select(x => new { id = x.Id, name = x.Name, description = x.Description })
                .ToListAsync();

            var menuJson = JsonSerializer.Serialize(menu);

            // 2. Prompt: Ép AI chỉ trả về JSON
            var prompt = $@"
            Bạn là AI tư vấn món ăn cho hệ thống nhà hàng DineOS.
            Menu (JSON):
            {menuJson}
            Yêu cầu của khách:
            ""{userInput}""
            Nhiệm vụ:
            - Phân tích yêu cầu khách (từ khóa, loại món, khẩu vị).
            - Chọn tối đa 3 món phù hợp nhất từ menu.
            Tiêu chí chọn:
            - Ưu tiên tên món chứa từ khóa
            - Ưu tiên mô tả liên quan
            - Nếu không rõ → chọn món phổ biến, dễ ăn
            Quy tắc:
            - CHỈ chọn món có trong menu
            - KHÔNG tự tạo món mới
            - KHÔNG chọn quá 3 món
            Trường hợp KHÔNG tìm được món phù hợp:
            - VẪN phải trả về 1 mảng JSON
            - Nhưng chứa DUY NHẤT 1 phần tử:
              {{
                ""id"": 0,
                ""reason"": ""Không tìm thấy món phù hợp với yêu cầu: {userInput}. Gợi ý: thử món nướng, lẩu hoặc món phổ biến.""
              }}
            Định dạng trả về (BẮT BUỘC):
            - CHỈ trả về JSON
            - KHÔNG thêm text ngoài JSON
            - Format:
            [
              {{ ""id"": 1, ""reason"": ""..."" }}
            ]
            ";

            // 3. URL chuẩn cho năm 2026 (Model 2.5 Flash)
            var url = $"https://generativelanguage.googleapis.com/v1/models/gemini-2.5-flash:generateContent?key={_apiKey}";

            var requestBody = new
            {
                contents = new[]
                {
                new { parts = new[] { new { text = prompt } } }
            }
            };

            // 4. Gửi Request
            HttpResponseMessage httpResponse;

            try
            {
                httpResponse = await _httpClient.PostAsJsonAsync(url, requestBody);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Gemini CALL ERROR: " + ex.Message);
                return new List<AiSuggestionResponse>();
            }

            if (!httpResponse.IsSuccessStatusCode)
            {
                var error = await httpResponse.Content.ReadAsStringAsync();
                Console.WriteLine($"Gemini Error: {error}");
                return new List<AiSuggestionResponse>();
            }

            var json = await httpResponse.Content.ReadAsStringAsync();
            JsonDocument doc;

            try
            {
                doc = JsonDocument.Parse(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine("JSON ROOT PARSE ERROR: " + ex.Message);
                return new List<AiSuggestionResponse>();
            }

            // 5. Trích xuất text từ response của Google
            string rawText = "";

            try
            {
                rawText = doc.RootElement
                    .GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text")
                    .GetString() ?? "";
                Console.WriteLine("==== RAW TEXT ====");
                Console.WriteLine(rawText);
            }
            catch (Exception ex)
            {
                Console.WriteLine("EXTRACT TEXT ERROR: " + ex.Message);
                return new List<AiSuggestionResponse>();
            }

            // 6. Xử lý chuỗi JSON an toàn (Tránh lỗi nếu AI trả về kèm lời dẫn)
            var suggestions = new List<AiSuggestionDto>();
            var startIndex = rawText.IndexOf("[");
            var endIndex = rawText.LastIndexOf("]");

            if (startIndex != -1 && endIndex != -1)
            {
                var cleanedJson = rawText.Substring(startIndex, endIndex - startIndex + 1);
                Console.WriteLine("==== CLEAN JSON ====");
                Console.WriteLine(cleanedJson);
                try
                {
                    suggestions = JsonSerializer.Deserialize<List<AiSuggestionDto>>(cleanedJson, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }) ?? new List<AiSuggestionDto>();
                    Console.WriteLine("==== PARSED SUGGESTIONS ====");
                    Console.WriteLine(JsonSerializer.Serialize(suggestions));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("PARSE JSON ERROR: " + ex.Message);
                    return new List<AiSuggestionResponse>();
                }
            }

            // 7. Query Database để lấy thông tin chi tiết món ăn (tên, mô tả)
            var ids = suggestions.Select(x => x.Id).ToList();
            Console.WriteLine("==== IDS FROM AI ====");
            Console.WriteLine(string.Join(", ", suggestions.Select(x => x.Id)));
            var items = await _context.MenuItems.Where(x => ids.Contains(x.Id)).ToListAsync();

            return items.Select(item =>
            {
                var sugg = suggestions.FirstOrDefault(x => x.Id == item.Id);
                return new AiSuggestionResponse
                {
                    Id = item.Id,
                    Name = item.Name,
                    Description = item.Description,
                    Reason = sugg?.Reason ?? "Món ăn này rất phù hợp với yêu cầu của bạn."
                };
            }).ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine("===== AI ERROR =====");
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);

            // fallback để không trả 500
            var fallback = await _context.MenuItems
                .Where(x => x.IsAvailable)
                .Take(3)
                .ToListAsync();

            return fallback.Select(x => new AiSuggestionResponse
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                Reason = "Hệ thống đang bận, gợi ý món phổ biến"
            }).ToList();
        }
    }
}