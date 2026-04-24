using Tesseract;
using DogoFinance.Integration.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;
using System.Net.Http;

namespace DogoFinance.Integration.Services
{
    public class DocumentProcessingService : IDocumentProcessingService
    {
        private readonly ILogger<DocumentProcessingService> _logger;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public DocumentProcessingService(ILogger<DocumentProcessingService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _httpClient = new HttpClient();
        }

        public async Task<ExtractedAddressData> ExtractAddressAsync(string imageUrl)
        {
            _logger.LogInformation("Processing document for address extraction using Tesseract OCR: {ImageUrl}", imageUrl);

            try
            {
                var imageBytes = await _httpClient.GetByteArrayAsync(imageUrl);
                string tessdataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tessdata");
                
                if (!Directory.Exists(tessdataPath))
                {
                    _logger.LogWarning("Tessdata directory not found at {Path}. Falling back to default data.", tessdataPath);
                    return GetFallbackData();
                }

                using var engine = new TesseractEngine(tessdataPath, "eng", EngineMode.Default);
                using var img = Pix.LoadFromMemory(imageBytes);
                using var page = engine.Process(img);
                
                var fullText = page.GetText();
                var confidence = page.GetMeanConfidence();

                if (string.IsNullOrWhiteSpace(fullText))
                {
                    _logger.LogWarning("Tesseract detected no text. Returning fallback.");
                    return GetFallbackData();
                }

                var extracted = ParseAddressFromText(fullText);
                extracted.FullText = fullText;
                extracted.ConfidenceScore = (decimal)confidence;

                return extracted;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Tesseract OCR failed for {ImageUrl}", imageUrl);
                return GetFallbackData();
            }
        }

        private ExtractedAddressData GetFallbackData()
        {
            return new ExtractedAddressData
            {
                Address = "123 Mock Street, Lagos Island",
                City = "Lagos",
                State = "Lagos State",
                FullText = "SYSTEM FALLBACK: Tesseract processing failed or no text found.",
                ConfidenceScore = 0.1m
            };
        }

        private ExtractedAddressData ParseAddressFromText(string text)
        {
            var data = new ExtractedAddressData();
            var lines = text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Select(l => l.Trim()).ToList();
            
            // 1. Detect State & City Heuristics (Utility Providers)
            if (text.Contains("Abuja Electric", StringComparison.OrdinalIgnoreCase))
            {
                data.State = "Abuja FCT";
                data.City = "Abuja";
            }
            else if (text.Contains("EKEDC", StringComparison.OrdinalIgnoreCase) || text.Contains("Eko Electric", StringComparison.OrdinalIgnoreCase))
            {
                data.State = "Lagos State";
                data.City = "Lagos (Eko)";
            }
            else if (text.Contains("IKEDC", StringComparison.OrdinalIgnoreCase) || text.Contains("Ikeja Electric", StringComparison.OrdinalIgnoreCase))
            {
                data.State = "Lagos State";
                data.City = "Ikeja";
            }

            // 2. Generic State Detection
            string[] states = { "Lagos", "Abuja", "Oyo", "Rivers", "Kano", "Ogun", "Delta", "Edo", "Anambra", "Enugu", "Kwara", "Kaduna" };
            if (string.IsNullOrEmpty(data.State))
            {
                foreach (var state in states)
                {
                    if (text.Contains(state, StringComparison.OrdinalIgnoreCase))
                    {
                        data.State = state + (state.Equals("Abuja", StringComparison.OrdinalIgnoreCase) ? " FCT" : " State");
                        break;
                    }
                }
            }

            // 3. Search for Label-Based Address (Service Address, Home Address, etc.)
            string[] addressLabels = { "Service Address", "Customer Address", "Property Address", "Address" };
            for (int i = 0; i < lines.Count; i++)
            {
                var currentLine = lines[i];
                foreach (var label in addressLabels)
                {
                    if (currentLine.Contains(label, StringComparison.OrdinalIgnoreCase))
                    {
                        // Clean the label from the line
                        var potentialAddress = Regex.Replace(currentLine, label, "", RegexOptions.IgnoreCase).Trim(':').Trim();
                        
                        // If the line is just the label, the address is likely on the next line(s)
                        if (string.IsNullOrWhiteSpace(potentialAddress) && i + 1 < lines.Count)
                        {
                            potentialAddress = lines[i + 1];
                            // Check if it continues to a third line (common in Nigerian utility bills)
                            if (i + 2 < lines.Count && !IsLabelLine(lines[i + 2]))
                            {
                                potentialAddress += ", " + lines[i + 2];
                            }
                        }
                        
                        if (!string.IsNullOrWhiteSpace(potentialAddress))
                        {
                            data.Address = potentialAddress;
                            goto AddressFound;
                        }
                    }
                }
            }

            // 4. Fallback: Regex Search
            var streetRegex = new Regex(@"(No\.?\s*\d+|Block\s*\w+|\d+)\s+([A-Z0-9][a-z0-9]+\s*)+(Street|Str|Road|Rd|Ave|Close|Cl|Way|Crescent|Cres|Estate|Est|Boulevard)", RegexOptions.IgnoreCase);
            var match = streetRegex.Match(text);
            if (match.Success)
            {
                data.Address = match.Value;
            }

        AddressFound:
            // Final Cleanup
            if (!string.IsNullOrEmpty(data.Address))
            {
                data.Address = data.Address.Trim(',', '.', ' ', ':');
            }

            return data;
        }

        private bool IsLabelLine(string line)
        {
            string[] commonLabels = { "Purchase Type", "Units", "Token", "Amount", "Debt", "Hotline", "Transaction", "Meter" };
            return commonLabels.Any(l => line.Contains(l, StringComparison.OrdinalIgnoreCase));
        }
    }
}
