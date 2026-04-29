using DineOS.Application.DTOs.Ai;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DineOS.Application.Common.Interfaces
{
    public interface IAiService
    {
        Task<List<AiSuggestionResponse>> GetSuggestion(string userInput);

    }
}
