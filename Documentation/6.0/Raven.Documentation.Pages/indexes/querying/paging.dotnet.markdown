# Page Query Results
---

{NOTE: }

* __Paging__:  
  Paging is the process of fetching a subset (a page) of results from a dataset, rather than retrieving the entire results at once.
  This method enables processing query results one page at a time.

* __Default page size__:  

    * Querying Lucene indexes:  
      If the client's query definition does Not explicitly specify the page size, the server will default to `int.MaxValue` (2,147,483,647).
      In such case, all results will be returned in a single server call.
  
    * Querying Corax indexes:  
      When using [Corax](../../indexes/search-engine/corax) as the search engine, indexes with more than `int.MaxValue` entries can be created and used.
      To match this capacity, queries over Corax indexes can skip a number of results that exceeds `int.MaxValue` and take documents from that location.

* __Performance__:  
  Using paging is beneficial when handling large result datasets, contributing to improved performance.  
  See [paging and performance](../../indexes/querying/paging#paging-and-performance) here below.

* In this page:

    * [No paging example](../../indexes/querying/paging#no-paging-example)
    * [Paging examples](../../indexes/querying/paging#paging-examples)
    * [Paging and performance](../../indexes/querying/paging#paging-and-performance)
    * [Paging through tampered results](../../indexes/querying/paging#paging-through-tampered-results)

{NOTE/}

---

{PANEL: No paging example}

{CODE-TABS}
{CODE-TAB:csharp:Query paging_0_1@Indexes\Querying\Paging.cs /}
{CODE-TAB:csharp:Query_async paging_0_2@Indexes\Querying\Paging.cs /}
{CODE-TAB:csharp:DocumentQuery paging_0_3@Indexes\Querying\Paging.cs /}
{CODE-TAB:csharp:Index index_0@Indexes\Querying\Paging.cs /}
{CODE-TAB-BLOCK:sql:RQL}
from index "Products/ByUnitsInStock"
where UnitsInStock > 10
{CODE-TAB-BLOCK/}
{CODE-TABS/}

{PANEL/}

{PANEL: Paging examples}

{NOTE: }

__Retrieve a specific page__:

---

{CODE-TABS}
{CODE-TAB:csharp:Query paging_1_1@Indexes\Querying\Paging.cs /}
{CODE-TAB:csharp:Query_async paging_1_2@Indexes\Querying\Paging.cs /}
{CODE-TAB:csharp:DocumentQuery paging_1_3@Indexes\Querying\Paging.cs /}
{CODE-TAB:csharp:Index index_0@Indexes\Querying\Paging.cs /}
{CODE-TAB-BLOCK:sql:RQL}
from index "Products/ByUnitsInStock"
where UnitsInStock > 10
limit 20, 10 // skip 20, take 10
{CODE-TAB-BLOCK/}
{CODE-TABS/}

{NOTE/}

{NOTE: }

__Page through all results__:

---

{CODE-TABS}
{CODE-TAB:csharp:Query paging_2_1@Indexes\Querying\Paging.cs /}
{CODE-TAB:csharp:Query_async paging_2_2@Indexes\Querying\Paging.cs /}
{CODE-TAB:csharp:DocumentQuery paging_2_3@Indexes\Querying\Paging.cs /}
{CODE-TAB:csharp:Index index_0@Indexes\Querying\Paging.cs /}
{CODE-TAB-BLOCK:sql:RQL}
from index "Products/ByUnitsInStock"
where UnitsInStock > 10
limit 0, 10 // First loop will skip 0, take 10

// The next loops in the code will each generate the above RQL with an increased 'skip' value:
// limit 10, 10
// limit 20, 10
// limit 30, 10
// ...
{CODE-TAB-BLOCK/}
{CODE-TABS/}

{NOTE/}
{PANEL/}

{PANEL: Paging and performance}

---

__Better performance__:

It is recommended to explicitly set a page size when making a query that is expected to generate a significant number of results.
This practice has several benefits:

* Optimizes bandwidth usage by reducing data transfer between the server and client.
* Prevents delays in response times caused by sending too much data over the network.
* Avoids high memory consumption when dealing with numerous documents.
* Ensures a more manageable user experience by not overwhelming users with massive datasets at once.

---

__Performance hints__:

* By default, if the number of returned results exceeds __2048__, the server will issue a "Page size too big" notification (visible in the Studio) with information about the query.

* This threshold can be customized by modifying the value of the [PerformanceHints.MaxNumberOfResults](../../server/configuration/performance-hints-configuration#performancehints.maxnumberofresults) configuration key.

* As suggested by the hint, you may consider using [Streaming query results](../../client-api/session/querying/how-to-stream-query-results) instead of paging.

![Figure 1. Performance Hint](images/performance-hint.png "Performance Hint")

{PANEL/}

{PANEL: Paging through tampered results}

* The `QueryStatistics` object contains the `TotalResults` property,  
  which represents the total number of matching documents found in the query results.

* The `QueryStatistics` object also contains the `SkippedResults` property.  
  Whenever this property is greater than __0__, that implies the server has skipped that number of results from the index.

* The server will skip duplicate results internally in the following two scenarios:

    1. When making a [Projection query](../../indexes/querying/projections) with [Distinct](../../indexes/querying/distinct).

    2. When querying a [Fanout index](../../indexes/indexing-nested-data#fanout-index---multiple-index-entries-per-document).

* In those cases:

    * The `SkippedResults` property from the stats object will hold the count of skipped (duplicate) results.

    * The `TotalResults` property will be invalidated -  
      it will Not deduct the number of skipped results from the total number of results.

* In order to do proper paging in those scenarios:  
  include the `SkippedResults` value when specifying the number of documents to skip for each page using:  
  `(currentPage * pageSize) + SkippedResults`.

* See the following examples:

{NOTE: }

__A projection query with Distinct__:

---

{CODE-TABS}
{CODE-TAB:csharp:Query paging_3_1@Indexes\Querying\Paging.cs /}
{CODE-TAB:csharp:Query_async paging_3_2@Indexes\Querying\Paging.cs /}
{CODE-TAB:csharp:DocumentQuery paging_3_3@Indexes\Querying\Paging.cs /}
{CODE-TAB:csharp:Index index_0@Indexes\Querying\Paging.cs /}
{CODE-TAB:csharp:Projected_class projected_class@Indexes\Querying\Paging.cs /}
{CODE-TAB-BLOCK:sql:RQL}
from index "Products/ByUnitsInStock"
where UnitsInStock > 10
select distinct Category, Supplier
limit 0, 10  // First loop will skip 0, take 10, etc.  
{CODE-TAB-BLOCK/}
{CODE-TABS/}

{NOTE/}

{NOTE: }

__Querying a Fanout index__:

---

{CODE-TABS}
{CODE-TAB:csharp:Query paging_4_1@Indexes\Querying\Paging.cs /}
{CODE-TAB:csharp:Query_async paging_4_2@Indexes\Querying\Paging.cs /}
{CODE-TAB:csharp:DocumentQuery paging_4_3@Indexes\Querying\Paging.cs /}
{CODE-TAB:csharp:Index index_1@Indexes\Querying\Paging.cs /}
{CODE-TAB-BLOCK:sql:RQL}
from index "Orders/ByProductName"
limit 0, 50  // First loop will skip 0, take 50, etc.
{CODE-TAB-BLOCK/}
{CODE-TABS/}

{NOTE/}
{PANEL/}

## Related Articles

### Querying

- [Query overview](../../client-api/session/querying/how-to-query)
- [Stream query results](../../client-api/session/querying/how-to-stream-query-results)
- [Get query statistics](../../client-api/session/querying/how-to-get-query-statistics)

### Indexes

- [Indexing basics](../../indexes/indexing-basics)
- [Query an index](../../indexes/querying/query-index)
- [Filtering](../../indexes/querying/filtering)
- [Sorting](../../indexes/querying/sorting)  
