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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutomatedEtherTransfer
{
    class Program
    {

        static void Main(string[] args)
        {
            var account = SetupAccount();
            var recipients = ReadCsv();
            //var recipients = new List<Records>() { new Records() { Value = 10000000000000000, Address = "0x5CC494843e3f4AC175A5e730c300b011FAbF2cEa" } };
            foreach (var recipient in recipients)
            {
                try
                {
                    var web3 = GetConnection();
                    var receipt = SendEther(account, recipient, web3).Result;
                }
                catch (System.Exception)
                {
                    MessageBox.Show("Failed");                                        
                }
                Thread.Sleep(30000);
            }
        }

        private static async Task<TransactionReceipt> SendEther(Account account, Records recipient, Web3 web3)
        {
            //web3.Eth.
            //Nethereum.

            var transactionPolling = web3.TransactionManager.TransactionReceiptService;

            //var currentBalance = await web3.Eth.GetBalance.SendRequestAsync(account.Address);

            //assumed client is mining already
            //when sending a transaction using an Account, a raw transaction is signed and send using the private key
            return await transactionPolling.SendRequestAndWaitForReceiptAsync(() =>
            {
                var transactionInput = new TransactionInput
                {
                    From = account.Address,
                    //Gas = new HexBigInteger(25000),
                    GasPrice = new HexBigInteger(10 ^ 10),
                    To = recipient.Address,
                    Value = new HexBigInteger(new BigInteger(recipient.Value)),
                    Nonce = web3.Eth.Transactions.GetTransactionCount.SendRequestAsync(account.Address).Result
                };
                var txSigned = new Nethereum.Signer.TransactionSigner();
                var signedTx = txSigned.SignTransaction(account.PrivateKey, transactionInput.To, transactionInput.Value, transactionInput.Nonce);
                var transaction = new Nethereum.RPC.Eth.Transactions.EthSendRawTransaction(web3.Client);
                return transaction.SendRequestAsync(signedTx);
            });
        }

        private static Web3 GetConnection()
        {
            return new Web3("https://mainnet.infura.io");
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
