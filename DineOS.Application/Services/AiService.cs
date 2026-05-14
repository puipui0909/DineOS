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

    private List<AiSuggestionResponse> NoResult(string userInput)
    {
        return new List<AiSuggestionResponse>
        {
            new AiSuggestionResponse
            {
                Id = Guid.Empty,
                Name = "Không tìm thấy món phù hợp",
                Description = "",
                Reason = $"Hiện chưa tìm thấy món phù hợp với yêu cầu \"{userInput}\". Hãy thử mô tả khác như món cay nhẹ, món nước hoặc món ngọt."
            }
        };
    }
    private string NormalizeQuery(string input)
    {
        input = input.ToLower();

        if (input.Contains("món nước"))
            input += " bún phở hủ tiếu mì nước soup canh";

        if (input.Contains("ít cay"))
            input += " mild không cay";

        if (input.Contains("ngọt"))
            input += " dessert sweet";

        if (input.Contains("nhẹ bụng"))
            input += " healthy thanh đạm";

        if (input.Contains("thanh mát"))
            input += " giải khát refresh";

        return input;
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
            var normalizedInput = NormalizeQuery(userInput);
            // 2. Prompt: Ép AI chỉ trả về JSON
            var prompt = $@"
            Bạn là AI tư vấn món ăn cho hệ thống nhà hàng DineOS.
            Menu (JSON):
            {menuJson}
            Yêu cầu của khách:
            ""{normalizedInput}""
            Nhiệm vụ:
            - Phân tích yêu cầu khách (từ khóa, loại món, khẩu vị).
            - Chọn tối đa 3 món phù hợp nhất từ menu.
            Tiêu chí chọn:
            - Ưu tiên món có mô tả liên quan đến yêu cầu (ví dụ: giòn, cay, ngọt, mát)
            - Không cần từ khóa phải nằm trong tên món
            - Nếu không chắc, hãy chọn món phổ biến phù hợp nhất
            - KHÔNG được trả về mảng rỗng
            - LUÔN chọn ít nhất 1 món từ menu
            Quy tắc:
            - CHỈ chọn món có trong menu
            - KHÔNG tự tạo món mới
            - KHÔNG chọn quá 3 món
            - id phải CHÍNH XÁC trùng với id trong menu JSON
            - KHÔNG được tự tạo id
            Trường hợp KHÔNG tìm được món phù hợp:
            - VẪN phải trả về 1 mảng JSON
            - Nhưng chứa DUY NHẤT 1 phần tử:
              {{
                ""id"": ""00000000-0000-0000-0000-000000000000"",
                ""reason"": ""Không tìm thấy món phù hợp với yêu cầu: {userInput}. Gợi ý: thử món nướng, lẩu hoặc món phổ biến.""
              }}
            Định dạng trả về (BẮT BUỘC):
            - CHỈ trả về JSON
            - KHÔNG thêm text ngoài JSON
            - Format:
            [
                {{
                ""id"": ""11111111-1111-1111-1111-111111111111"",
                ""reason"": ""...""
                }}
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
                return NoResult(userInput);
            }

            if (!httpResponse.IsSuccessStatusCode)
            {
                var error = await httpResponse.Content.ReadAsStringAsync();
                Console.WriteLine($"Gemini Error: {error}");
                return NoResult(userInput);
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
                return NoResult(userInput);
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
                return NoResult(userInput);
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
                    if (suggestions.Any(x => x.Id == Guid.Empty))
                    {
                        return NoResult(userInput);
                    }
                    Console.WriteLine("==== PARSED SUGGESTIONS ====");
                    Console.WriteLine(JsonSerializer.Serialize(suggestions));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("PARSE JSON ERROR: " + ex.Message);
                    return NoResult(userInput);
                }
            }

            // 7. Query Database để lấy thông tin chi tiết món ăn (tên, mô tả)
            var ids = suggestions.Select(x => x.Id).ToList();
            Console.WriteLine("==== IDS FROM AI ====");
            Console.WriteLine(string.Join(", ", suggestions.Select(x => x.Id)));

            var items = await _context.MenuItems
                .Where(x => ids.Contains(x.Id))
                .ToListAsync();

            // 🔥 FIX: nếu AI trả id sai hoặc rỗng → fallback
            if (!items.Any())
            {
                Console.WriteLine("⚠️ AI trả id không hợp lệ → fallback theo keyword");
                var keywords = normalizedInput
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Where(x => x.Length > 2)
                .ToList();

                items = await _context.MenuItems
                    .Where(x => x.IsAvailable &&
                        keywords.Any(k =>
                            x.Name.ToLower().Contains(k) ||
                            x.Description.ToLower().Contains(k)))
                    .Take(3)
                    .ToListAsync();
            }

            // 🔥 FIX tiếp: vẫn không có → lấy món phổ biến
            if (!items.Any())
            {
                Console.WriteLine("⚠️ Không match keyword → fallback phổ biến");

                items = await _context.MenuItems
                    .Where(x => x.IsAvailable)
                    .Take(3)
                    .ToListAsync();
            }

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