using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DynamoDB
{
    public class Program
    {
        private const string accessKey = "";
        private const string secretKey = "";

        public static void Main(string[] args)
        {

            Task t = MainAsync(args);
            t.Wait();

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        public static async Task MainAsync(string[] args)
        {
            string tableName = "ProductList";
            string hashKey = "ProductName";

            Console.WriteLine("Creating credentials and initializing DynamoDB client");
            var credentials = new BasicAWSCredentials(accessKey, secretKey);
            var client = new AmazonDynamoDBClient(credentials, RegionEndpoint.USEast1);

            Console.WriteLine("Verify table => " + tableName);
            var tableResponse = await client.ListTablesAsync();
            if (!tableResponse.TableNames.Contains(tableName))
            {
                Console.WriteLine("Table not found, creating table => " + tableName);
                await client.CreateTableAsync(new CreateTableRequest
                {
                    TableName = tableName,
                    ProvisionedThroughput = new ProvisionedThroughput
                    {
                        ReadCapacityUnits = 3,
                        WriteCapacityUnits = 1
                    },
                    KeySchema = new List<KeySchemaElement>
                    {
                        new KeySchemaElement
                        {
                            AttributeName = hashKey,
                            KeyType = KeyType.HASH
                        }
                    },
                    AttributeDefinitions = new List<AttributeDefinition>
                    {
                        new AttributeDefinition { AttributeName = hashKey, AttributeType=ScalarAttributeType.S }
                    }
                });

                bool isTableAvailable = false;
                while (!isTableAvailable)
                {
                    Console.WriteLine("Waiting for table to be active...");
                    Thread.Sleep(5000);
                    var tableStatus = await client.DescribeTableAsync(tableName);
                    isTableAvailable = tableStatus.Table.TableStatus == "ACTIVE";
                }
            }

            Console.WriteLine("Set a local DB context");
            var context = new DynamoDBContext(client);

            Console.WriteLine("Create an ProductItem object to save");
            ProductList currentItem = new ProductList
            {
                ProductName = "someAwesomeProduct",
                ProductQuantity = 7
            };

            Console.WriteLine("Save an ProductItem object");
            await context.SaveAsync<ProductList>(currentItem);

            
        }
    }
}
