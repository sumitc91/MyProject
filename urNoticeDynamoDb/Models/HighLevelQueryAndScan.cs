﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Runtime;
using urNotice.Common.Infrastructure.Common.Config;

namespace urNoticeDynamoDb.Models
{
    class HighLevelQueryAndScan
    {
        private static string accessKey = AwsConfig._awsAccessKey;
        private static string secretKey = AwsConfig._awsSecretKey;

        private static AmazonDynamoDBClient client = new AmazonDynamoDBClient(accessKey, secretKey, RegionEndpoint.USWest2);

        public void MainMethod()
        {
            try
            {
                DynamoDBContext context = new DynamoDBContext(client);

                CreateBookItem(context)
                ;
                // Get item.
                GetBook(context, 1);

                // Sample forum and thread to test queries.
                string forumName = "Amazon DynamoDB";
                string threadSubject = "DynamoDB Thread 1";
                // Sample queries.
                FindRepliesInLast15Days(context, forumName, threadSubject);
                FindRepliesPostedWithinTimePeriod(context, forumName, threadSubject);

                // Scan table.
                FindProductsPricedLessThanZero(context);
                Console.WriteLine("To continue, press Enter");
                Console.ReadLine();
            }
            catch (AmazonDynamoDBException e) { Console.WriteLine(e.Message); }
            catch (AmazonServiceException e) { Console.WriteLine(e.Message); }
            catch (Exception e) { Console.WriteLine(e.Message); }
        }

        private static void CreateBookItem(DynamoDBContext context)
        {
            Console.WriteLine("\n*** Executing CreateBookItem() ***");
            var book = new Book
            {
                Id=1,
                InPublication=false,
                ISBN = "123",
                PageCount = "10",
                Price=234,
                ProductCategory ="xyz",
                Title="title"

            };

            context.Save(book);
            //productCatalog.PutItem(book);
        }

        private static void GetBook(DynamoDBContext context, int productId)
        {
            Book bookItem = context.Load<Book>(productId);

            Console.WriteLine("\nGetBook: Printing result.....");
            Console.WriteLine("Title: {0} \n No.Of threads:{1} \n No. of messages: {2}",
                                bookItem.Title, bookItem.ISBN, bookItem.PageCount);
        }

        private static void FindRepliesInLast15Days(DynamoDBContext context,
                                                    string forumName,
                                                    string threadSubject)
        {
            string replyId = forumName + "#" + threadSubject;
            DateTime twoWeeksAgoDate = DateTime.UtcNow - TimeSpan.FromDays(15);
            IEnumerable<Reply> latestReplies =
              context.Query<Reply>(replyId, QueryOperator.GreaterThan, twoWeeksAgoDate);
            Console.WriteLine("\nFindRepliesInLast15Days: Printing result.....");
            foreach (Reply r in latestReplies)
                Console.WriteLine("{0}\t{1}\t{2}\t{3}", r.Id, r.PostedBy, r.Message, r.ReplyDateTime);
        }

        private static void FindRepliesPostedWithinTimePeriod(DynamoDBContext context,
                                                              string forumName,
                                                              string threadSubject)
        {
            string forumId = forumName + "#" + threadSubject;
            Console.WriteLine("\nFindRepliesPostedWithinTimePeriod: Printing result.....");

            DateTime startDate = DateTime.UtcNow - TimeSpan.FromDays(30);
            DateTime endDate = DateTime.UtcNow - TimeSpan.FromDays(1);

            IEnumerable<Reply> repliesInAPeriod = context.Query<Reply>(forumId,
                                                                      QueryOperator.Between, startDate, endDate);
            foreach (Reply r in repliesInAPeriod)
                Console.WriteLine("{0}\t{1}\t{2}\t{3}", r.Id, r.PostedBy, r.Message, r.ReplyDateTime);
        }

        private static void FindProductsPricedLessThanZero(DynamoDBContext context)
        {
            int price = 0;
            IEnumerable<Book> itemsWithWrongPrice = context.Scan<Book>(
                  new ScanCondition("Price", ScanOperator.LessThan, price),
                  new ScanCondition("ProductCategory", ScanOperator.Equal, "Book")
                  );
            Console.WriteLine("\nFindProductsPricedLessThanZero: Printing result.....");
            foreach (Book r in itemsWithWrongPrice)
                Console.WriteLine("{0}\t{1}\t{2}\t{3}", r.Id, r.Title, r.Price, r.ISBN);
        }
    }

    [DynamoDBTable("Reply")]
    public class Reply
    {
        [DynamoDBHashKey]    //Partition key
        public string Id { get; set; }

        [DynamoDBRangeKey]  //Sort key
        public DateTime ReplyDateTime { get; set; }

        // Properties included implicitly.
        public string Message { get; set; }
        // Explicit property mapping with object persistence model attributes.
        [DynamoDBProperty("LastPostedBy")]
        public string PostedBy { get; set; }
        // Property to store version number for optimistic locking.
        [DynamoDBVersion]
        public int? Version { get; set; }
    }

    [DynamoDBTable("Thread")]
    public class Thread
    {
        // PK mapping.
        [DynamoDBHashKey]  //Partition key
        public string ForumName { get; set; }
        [DynamoDBRangeKey]  //Sort key
        public DateTime Subject { get; set; }
        // Implicit mapping.
        public string Message { get; set; }
        public string LastPostedBy { get; set; }
        public int Views { get; set; }
        public int Replies { get; set; }
        public bool Answered { get; set; }
        public DateTime LastPostedDateTime { get; set; }
        // Explicit mapping (property and table attribute names are different.
        [DynamoDBProperty("Tags")]
        public List<string> KeywordTags { get; set; }
        // Property to store version number for optimistic locking.
        [DynamoDBVersion]
        public int? Version { get; set; }
    }

    [DynamoDBTable("Forum")]
    public class Forum
    {
        [DynamoDBHashKey]
        public string Name { get; set; }
        // All the following properties are explicitly mapped,
        // only to show how to provide mapping.
        [DynamoDBProperty]
        public int Threads { get; set; }
        [DynamoDBProperty]
        public int Views { get; set; }
        [DynamoDBProperty]
        public string LastPostBy { get; set; }
        [DynamoDBProperty]
        public DateTime LastPostDateTime { get; set; }
        [DynamoDBProperty]
        public int Messages { get; set; }
    }

    [DynamoDBTable("ProductCatalog")]
    public class Book
    {
        [DynamoDBHashKey]    //Partition key
        public int Id { get; set; }
        public string Title { get; set; }
        public string ISBN { get; set; }
        public int Price { get; set; }
        public string PageCount { get; set; }
        public string ProductCategory { get; set; }
        public bool InPublication { get; set; }
    }
}
