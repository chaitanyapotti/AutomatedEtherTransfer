using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CsvHelper;
using CsvHelper.Configuration;
using Nethereum.HdWallet;
using Nethereum.Util;
using Nethereum.Web3.Accounts.Managed;

namespace AutomatedEtherTransfer_Windows
{
    class Program
    {

        static void Main(string[] args)
        {
            Task.Run(async () => await TestSend()).GetAwaiter().GetResult();

        }

        private static async Task TestSend()
        {
            var account = SetupTestAccount();
            var recipients = ReadCsv();

            //var recipients = new List<Records>() { new Records() { Value = 10000000000000000, Address = "0x5CC494843e3f4AC175A5e730c300b011FAbF2cEa" } };
            for (var index = 350; index < recipients.Count; index++)
            {
                var recipient = recipients[index];
                try
                {
                    var web3 = GetTestConnection(account);
                    var transactionHash = await SendTestEther(account, recipient, web3);
                    Debug.Print(transactionHash);
                }
                catch (System.Exception)
                {
                    MessageBox.Show("Failed");
                }
            }
        }

        private static Task<string> SendTestEther(Account account, Records recipient, Web3 web3)
        {
            var transactionInput = new TransactionInput()
            {
                From = account.Address,
                To = recipient.Address,
                GasPrice = new HexBigInteger(Web3.Convert.ToWei(4, UnitConversion.EthUnit.Gwei)),
                Value = new HexBigInteger(Web3.Convert.ToWei(recipient.Value, UnitConversion.EthUnit.Ether))
            };

            return web3.Eth.TransactionManager.SendTransactionAsync(transactionInput);

        }

        private static Web3 GetTestConnection(Account account)
        {
            return new Web3(account, "https://mainnet.infura.io/v3/dc22c9c6245742069d5fe663bfa8a698");
        }

        private static Account SetupTestAccount() => Account.LoadFromKeyStoreFile(
            @"D:\OneDrive - iitb.ac.in\Electus\temp account\UTC--2018-08-30T12-36-39.662Z--44dc6004343063a0227d801d1d30440028cc6f8d",
            "@password");

        private static List<Records> ReadCsv()
        {
            string filePath = @"C:\Users\Naina\source\repos\BcfTransfer.csv";
            if (File.Exists(filePath))
            {
                using (StreamReader stream = new StreamReader(filePath))
                {
                    CsvReader reader = new CsvReader(stream, new Configuration
                    {
                        TrimOptions = TrimOptions.Trim,
                        HasHeaderRecord = true,
                        HeaderValidated = null
                    });
                    reader.Configuration.RegisterClassMap<RecordMapper>();

                    return reader.GetRecords<Records>().OrderByDescending(x => x.Value).ToList();
                }
            }
            else
            {
                return null;
            }
        }
    }

    class Records
    {
        public string Address { get; set; }

        public decimal Value { get; set; }
    }

    sealed class RecordMapper : ClassMap<Records>
    {
        public RecordMapper()
        {
            Map(x => x.Address).Index(0);
            Map(x => x.Value).Index(1);
        }
    }
}
