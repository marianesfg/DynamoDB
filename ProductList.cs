using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace DynamoDB
{
    [DynamoDBTable("ProductList")]
 
    public class ProductList
    {
        [DynamoDBHashKey]
        public string ProductName { get; set; }
        public int ProductQuantity { get; set; }
    }
}
