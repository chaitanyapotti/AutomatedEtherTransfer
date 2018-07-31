using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Nethereum.HdWallet;
using Nethereum.Util;
using Nethereum.Web3.Accounts.Managed;

namespace AutomatedEtherTransfer_Windows
{
    class Program
    {

        static void Main(string[] args)
        {
            //MainSend();
            Task.Run(async () => await TestSend()).GetAwaiter().GetResult();

        }

        private static async Task TestSend()
        {
            var account = SetupTestAccount();
            var recipients = new Wallet("blue inherit drum enroll amused please camp false estate flash sell right", null).GetAddresses();
            var transanctionHashes = new List<string>();

            //var recipients = new List<Records>() { new Records() { Value = 10000000000000000, Address = "0x5CC494843e3f4AC175A5e730c300b011FAbF2cEa" } };
            foreach (var recipient in recipients)
            {
                try
                {
                    var web3 = GetTestConnection(account);
                    var balance = await web3.Eth.GetBalance.SendRequestAsync(recipients[0]);
                    var transactionHash = await SendTestEther(account, recipient, web3);
                }
                catch (System.Exception)
                {
                    MessageBox.Show("Failed");
                }
            }
        }

        private static Task<string> SendTestEther(Account account, string recipient, Web3 web3)
        {
            var transactionInput = new TransactionInput()
            {
                From = account.Address,
                GasPrice = new HexBigInteger(Web3.Convert.ToWei(1.5, UnitConversion.EthUnit.Gwei)),
                To = recipient,
                Value = new HexBigInteger(new BigInteger(0.1))
            };

            return web3.Eth.TransactionManager.SendTransactionAsync(transactionInput);

        }

        private static Web3 GetTestConnection(Account account)
        {
            return new Web3(account, "https://rinkeby.infura.io/v3/dc22c9c6245742069d5fe663bfa8a698");
        }

        private static Account SetupTestAccount() => new Wallet("blue inherit drum enroll amused please camp false estate flash sell right", null).GetAccount(0);
    }
}
