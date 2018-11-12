using System;
using Raven.Client.Documents;
using Raven.Client;
using System.Linq;
using System.IO;
using System.Collections.Generic;
//using Raven.Client.Documents.Operations;
using Raven.Client.Documents.Operations.Counters;

namespace Rvn.Ch02
{
    class Program
    {
        static void Main(string[] args)
        {
            var docStore = new DocumentStore
            {
                Urls = new[] { "http://localhost:8080" },
                Database = "Products"
            };
            docStore.Initialize();

            #region counters_region_CountersFor_with_document_load
            // Use CountersFor by passing it a document object

            // 1. Open a session
            using (var session = docStore.OpenSession())
            {
                // 2. Use the session to load a document.
                var document = session.Load<Product>("Products/1-C");

                // 3. Create an instance of `CountersFor`
                //   Pass the document object returned from session.Load as a param.
                var documentCounters = session.CountersFor(document);

                // 4. Use `CountersFor` methods to manage the product document's Counters
                documentCounters.Delete("ProductLikes"); // Delete the "ProductLikes" Counter
                documentCounters.Increment("ProductModified", 15); // Add 15 to Counter "ProductModified"
                var counter = documentCounters.Get("DaysLeftForSale"); // Get value of "DaysLeftForSale"

                // 5. Save the changes to the session
                session.SaveChanges();
            }
            #endregion

            #region counters_region_CountersFor_without_document_load
            // Use CountersFor without loading a document

            // 1. Open a session
            using (var session = docStore.OpenSession())
            {
                // 2. pass an explicit document ID to the CountersFor constructor 
                var documentCounters = session.CountersFor("Products/1-C");

                // 3. Use `CountersFor` methods to manage the product document's Counters
                documentCounters.Delete("ProductLikes"); // Delete the "ProductLikes" Counter
                documentCounters.Increment("ProductModified", 15); // Add 15 to Counter "ProductModified"
                var counter = documentCounters.Get("DaysLeftForSale"); // Get "DaysLeftForSale"'s value

                // 4. Save changes to the session
                session.SaveChanges();
            }
            #endregion

            // remove a counter from a document
            #region counters_region_Delete
            // 1. Open a session
            using (var session = docStore.OpenSession())
            {
                // 2. pass CountersFor's constructor a document ID  
                var documentCounters = session.CountersFor("Products/1-C");

                // 3. Delete the "ProductLikes" Counter
                documentCounters.Delete("ProductLikes");

                // 4. Save changes to the session
                session.SaveChanges();
            }
            #endregion

            // Increment a counter's value
            #region counters_region_Increment
            // 1. Open a session
            using (var session = docStore.OpenSession())
            {
                // 2. pass CountersFor's constructor a document ID  
                var documentCounters = session.CountersFor("Products/1-C");

                // 3. Use `CountersFor.Increment`
                documentCounters.Increment("ProductLikes"); // Increase "ProductLikes" by 1, or create it with a value of 1
                documentCounters.Increment("ProductDislikes", 1); // Increase "ProductDislikes" by 1, or create it with a value of 1
                documentCounters.Increment("ProductPageViews", 15); // Increase "ProductPageViews" by 15, or create it with a value of 15
                documentCounters.Increment("DaysLeftForSale", -10); // Decrease "DaysLeftForSale" by 10, or create it with a value of -10

                // 4. Save changes to the session
                session.SaveChanges();
            }
            #endregion

            // get a counter's value by the counter's name
            #region counters_region_Get
            // 1. Open a session
            using (var session = docStore.OpenSession())
            {
                // 2. pass CountersFor's constructor a document ID  
                var documentCounters = session.CountersFor("Products/1-C");

                // 3. Use `CountersFor.Get` to retrieve a Counter's value
                var DaysLeft = documentCounters.Get("DaysLeftForSale");
                Console.WriteLine("Days Left For Sale: " + DaysLeft);
            }
            #endregion

            // GetAll
            #region counters_region_GetAll
            // 1. Open a session
            using (var session = docStore.OpenSession())
            {
                // 2. pass CountersFor's constructor a document ID  
                var documentCounters = session.CountersFor("Products/1-C");

                // 3. Use GetAll to retrieve all of the document's Counters' names and values.
                var counters = documentCounters.GetAll();

                // list counters' names and values
                foreach (var counter in counters)
                {
                    Console.WriteLine("counter name: " + counter.Key + ", counter value: " + counter.Value);
                }
            }
            #endregion

            // playing with GettAll
            /*
            using (var session = docStore.OpenSession())
            {

                // load all objects of a specific database
                Console.WriteLine("list all documents in a collection");
                var documentsList = session.Advanced.LoadStartingWith<Product>("Products/");
                foreach (var someRecord in documentsList)
                {
                    var documentID = someRecord.Id;
                    //Console.WriteLine();
                    Console.WriteLine("\ndocument ID: " + documentID);

                    var document = session.Load<Product>(documentID); // load a document
                    var documentCounters = session.CountersFor(document); // get the counters for this document

                    //list all counters currently attached to a certain object
                    Console.WriteLine("all counters for this document: ");
                    var documentCountersArray = documentCounters.GetAll().ToArray(); // gett all counters attached to this document
                    foreach (var counter in documentCountersArray)
                    {
                        Console.WriteLine("counter key: " + counter.Key + ", counter value: " + counter.Value);
                    }
                }

                Console.ReadKey();
            }
            */

            //playing with batch operations
            /*
            using (var session = docStore.OpenSession())
            {
                    //	GetCountersOperation
                    var getthem = new GetCountersOperation("Products/1-C", "c2");
                    var resgettem = docStore.Operations.Send(getthem);
                    Console.WriteLine("tararam2 results " + resgettem?.Counters[0]?.TotalValue);


                    // batch
                    var docsies =
                        new List<DocumentCountersOperation>()
                        {
                                new DocumentCountersOperation
                                {
                                    DocumentId = "Products/1-C",
                                    Operations = new List<CounterOperation>
                                        {
                                            new CounterOperation {CounterName = "c1", Type = CounterOperationType.Delete },
                                            new CounterOperation {CounterName = "c2", Type = CounterOperationType.Increment, Delta = -100 },
                                            new CounterOperation {CounterName = "c3", Type = CounterOperationType.Get },
                                        }
                                },
                                new DocumentCountersOperation
                                {
                                    DocumentId = "Users/3",
                                    Operations = new List<CounterOperation>
                                            {
                                                new CounterOperation {CounterName = "u1", Type = CounterOperationType.Delete },
                                                new CounterOperation {CounterName = "u2", Type = CounterOperationType.Increment, Delta = 0100 },
                                                new CounterOperation {CounterName = "u3", Type = CounterOperationType.Get },
                                            }
                                }
                        };
                  
                    var counterBatchObject = new CounterBatch();
                    counterBatchObject.Documents = docsies;
                    counterBatchObject.ReplyWithAllNodesValues = true;

                    var counterBatchOperationObject = new CounterBatchOperation(counterBatchObject);

                    var res = docStore.Operations.Send(counterBatchOperationObject);
                    foreach (var detail in res.Counters)
                    {
                        Console.WriteLine($"name = " +
                      $"{detail.CounterName}, value = {detail.TotalValue}");

                        Console.WriteLine("values per node: ");
                        foreach (var nodeValue in detail.CounterValues)
                        {
                            Console.WriteLine($"{nodeValue.Key[0]}:{nodeValue.Value}");
                        }
                        

                    }
                    

                    Console.ReadKey();
            }
              */

        }

        public class Product
        {
            public string Id { get; set; }
            public string CustomerId { get; set; }
            public DateTime Started { get; set; }
            public DateTime? Ended { get; set; }
            public string Issue { get; set; }
            public int Votes { get; set; }
        }

        private interface IFoo
        {
            #region Increment-definition
            void Increment(string counterName, long incrementValue = 1);
            #endregion

            #region Delete-definition
            void Delete(string counterName);
            #endregion

            #region Get-definition
            long Get(string counterName);
            #endregion

            #region GetAll-definition
            Dictionary<string, long?> GetAll();
            #endregion
        }
    }
}
