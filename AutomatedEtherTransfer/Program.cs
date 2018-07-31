using CsvHelper;
using CsvHelper.Configuration;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows.Forms;
using Nethereum.Util;

namespace AutomatedEtherTransfer
{
    class Program
    {

        static void Main(string[] args)
        {
            //MainSend();
            Task.Run(async () => await Send()).GetAwaiter().GetResult();

        }

        private static async Task Send()
        {
            var account = SetupAccount();
            var recipients = ReadCsv();
            //This account is for test purposes only. No real ether in it.
            //var recipients = new Wallet("blue inherit drum enroll amused please camp false estate flash sell right", null).GetAddresses();
            var transanctionHashes = new List<string>();
            
            //var recipients = new List<Records>() { new Records() { Value = 10000000000000000, Address = "0x5CC494843e3f4AC175A5e730c300b011FAbF2cEa" } };
            foreach (var recipient in recipients)
            {
                try
                {
                    var web3 = GetConnection(account);
                    //var balance = await web3.Eth.GetBalance.SendRequestAsync(recipients[0]);
                    var transactionHash = await SendEther(account, recipient, web3);
                }
                catch (System.Exception)
                {
                    MessageBox.Show("Failed");
                }
            }
        }

        private static Task<string> SendEther(Account account, Records recipient, Web3 web3)
        {
            var transactionInput = new TransactionInput()
            {
                From = account.Address,
                GasPrice = new HexBigInteger(Web3.Convert.ToWei(1.5, UnitConversion.EthUnit.Gwei)),
                To = recipient.Address,
                Value = new HexBigInteger(new BigInteger(recipient.Value))
            };

            return web3.Eth.TransactionManager.SendTransactionAsync(transactionInput);
            
        }

        private static Web3 GetConnection(Account account)
        {
            return new Web3(account, "https://mainnet.infura.io");
        }

        private static Account SetupAccount()
        {
            var password = "@Password";
            var accountFilePath = @"filePath";
            return Account.LoadFromKeyStoreFile(accountFilePath, password);
        }

        private static List<Records> ReadCsv()
        {
            string filePath = @"C:\Users\Potti\source\repos\ConversionFiles\XrcfRecipients.csv";
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

                    return reader.GetRecords<Records>().ToList();
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
            Map(x => x.Address).Name("Address");
            Map(x => x.Value).Name("Value");
        }
    }
}
