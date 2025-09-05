using AI_Learning_For_Graduate.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AI_Learning_For_Graduate.Pages.Blockchain
{
    public class IndexModel : PageModel
    {
        private readonly BlockchainService _blockchainService;

        public string? BlockNumber { get; set; }

        public IndexModel()
        {
            _blockchainService = new BlockchainService();
        }

        public async Task OnGetAsync()
        {
            BlockNumber = await _blockchainService.GetLatestBlockAsync();
        }
    }
}
