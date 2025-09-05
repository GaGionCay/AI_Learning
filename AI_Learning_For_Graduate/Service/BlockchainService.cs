using Nethereum.Web3;

namespace AI_Learning_For_Graduate.Service
{
    public class BlockchainService
    {
        private readonly Web3 _web3;
        private readonly string _contractAddress = "0x..."; // địa chỉ smart contract
        private readonly string _privateKey = "pave october beach wine wisdom pride pulse dizzy sheriff"; // ví test
        private readonly string _rpcUrl = "https://rpc-mumbai.maticvigil.com"; // Polygon Mumbai

        public BlockchainService()
        {
            _web3 = new Web3(_rpcUrl);
        }

        public async Task<string> GetLatestBlockAsync()
        {
            var block = await _web3.Eth.Blocks.GetBlockNumber.SendRequestAsync();
            return block.Value.ToString();
        }
    }

}
