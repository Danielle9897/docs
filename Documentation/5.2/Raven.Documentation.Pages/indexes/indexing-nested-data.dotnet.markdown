[//]: # (# Fanout Indexes)

# Indexing Nested data

---

{NOTE: }

* JSON documents can have nested structures, where one document contains other objects or arrays of objects.

* Use a static-index to facilitate querying for documents based on the nested data.

* In this page:
 
  * [Sample data](../indexes/indexing-nested-data#sample-data)

  * [Simple index - SINGLE index-entry per document](../indexes/indexing-nested-data#simple-index---single-index-entry-per-document)    
      * [The index](../indexes/indexing-nested-data#theIndex)
      * [The index-entries](../indexes/indexing-nested-data#theIndexEntries)
      * [Querying the index](../indexes/indexing-nested-data#queryingTheIndex)
      * [When to use](../indexes/indexing-nested-data#whenToUse)

  * [Fanout index - MULTIPLE index-entries per document](../indexes/indexing-nested-data#fanout-index---multiple-index-entries-per-document)   
      * [What is a fanout index](../indexes/indexing-nested-data#whatIsFanoutIndex)
      * [Fanout index - Map index example](../indexes/indexing-nested-data#fanoutMapIndex)
      * [Fanout index - Map-Reduce index example](../indexes/indexing-nested-data#fanoutMapReduceIndex)
      * [Performance hints](../indexes/indexing-nested-data#performanceHints)
      * [Paging](../indexes/indexing-nested-data#paging)

{NOTE/}

---

{PANEL: Sample data}

* The examples in this article are based on the following __Class__ and __Sample Data__:

{CODE-TABS}
{CODE-TAB:csharp:Class online_shop_class@Indexes\IndexingNestedData.cs /}
{CODE-TAB:csharp:Sample_data sample_data@Indexes\IndexingNestedData.cs /}
{CODE-TABS/}

{PANEL/}

{PANEL: Simple index - Single index-entry per document}


<a id="theIndex" /> __The index__:

{CODE simple_index@Indexes\IndexingNestedData.cs /}

---

<a id="theIndexEntries" /> __The index-entries__:

![Simple - index-entries](images/indexing-nested-data-1.png "A single index-entry per document")

1. The index-entries content is visible from the Studio [Query view](../studio/database/queries/query-view).

2. Check option: _Show raw index-entries instead of Matching documents_.

3. Each row represents an __index-entry__.  
   The index has a single index-entry per document (3 entries in this example).  

4. The index-field contains a collection of ALL nested values from the document.  
   e.g. The third __index-entry__ has the following values in the _Colors_ __index-field__:  
   `{"black", "blue", "red"}`

---

<a id="queryingTheIndex" /> __Querying the index__:

{CODE-TABS}
{CODE-TAB:csharp:Query simple_index_query_1@Indexes\IndexingNestedData.cs /}
{CODE-TAB:csharp:Query_async simple_index_query_2@Indexes\IndexingNestedData.cs /}
{CODE-TAB:csharp:DocumentQuery simple_index_query_3@Indexes\IndexingNestedData.cs /}
{CODE-TAB-BLOCK:sql:RQL}
from index "Shops/ByTShirt/Simple"
where Colors == "red"
{CODE-TAB-BLOCK/}
{CODE-TABS/}

{CODE results_1@Indexes\IndexingNestedData.cs /}

---

<a id="whenToUse" /> __When to use__:

* This type of index structure is effective for retrieving documents when filtering the query by any of the inner nested values that were indexed.

* However, due to the way the index-entries are generated, this index __cannot__ provide results for a query searching for documents that contain 
  specific sub-objects which satisfy some AND condition. For example:   

    {CODE results_2@Indexes\IndexingNestedData.cs /}

* To address this, you must use a __Fanout index__ - as described below.

{PANEL/}

{PANEL: Fanout index - Multiple index-entries per document}

{NOTE: }

<a id="whatIsFanoutIndex" /> __What is a Fanout index__:

* A fanout index is an index that outputs multiple index-entries per document.  
  A separate index-entry is created for each nested sub-object from the document.
 
* The fanout index is useful when you need to retrieve documents matching query criteria  
  that search for specific sub-objects that comply with some logical conditions.

{NOTE/}

{NOTE: }

<a id="fanoutMapIndex" /> __Fanout index - Map index example__:

{CODE-TABS}
{CODE-TAB:csharp:LINQ_index fanout_index_1@Indexes\IndexingNestedData.cs /}
{CODE-TAB:csharp:JavaScript_index fanout_index_js@Indexes\IndexingNestedData.cs /}
{CODE-TABS/}

{CODE-TABS}
{CODE-TAB:csharp:Query fanout_index_query_1@Indexes\IndexingNestedData.cs /}
{CODE-TAB:csharp:Query_async fanout_index_query_2@Indexes\IndexingNestedData.cs /}
{CODE-TAB:csharp:DocumentQuery fanout_index_query_3@Indexes\IndexingNestedData.cs /}
{CODE-TAB-BLOCK:sql:RQL}
from index "Shops/ByTShirt/Fanout" 
where Color == "red" and Size == "M"
{CODE-TAB-BLOCK/}
{CODE-TABS/}

{CODE results_3@Indexes\IndexingNestedData.cs /}

---

<a id="fanoutMapIndexIndexEntries" /> __The index-entries__:

![Fanout - index-entries](images/indexing-nested-data-2.png "Multiple index-entries per document")

1. The index-entries content is visible from the Studio [Query view](../studio/database/queries/query-view).

2. Check option: _Show raw index-entries instead of Matching documents_.

3. Each row represents an __index-entry__.  
   Each index-entry corresponds to an inner item in the TShirt list.

4. In this example, the total number of index-entries is __12__,  
   which is the total number of inner items in the TShirt list in all __3__ documents in the collection.

{NOTE/}

{NOTE: }

<a id="fanoutMapReduceIndex" /> __Fanout index - Map-Reduce index example__:

* The fanout index concept applies to map-reduce indexes as well:

{CODE-TABS}
{CODE-TAB:csharp:LINQ_index fanout_index_2@Indexes\IndexingNestedData.cs /}
{CODE-TAB:csharp:JavaScript_index map_reduce_2_0@Indexes\JavaScript.cs /}}
{CODE-TABS/}

{CODE-TABS}
{CODE-TAB:csharp:Query fanout_index_query_4@Indexes\IndexingNestedData.cs /}
{CODE-TAB:csharp:Query_async fanout_index_query_5@Indexes\IndexingNestedData.cs /}
{CODE-TAB:csharp:DocumentQuery fanout_index_query_6@Indexes\IndexingNestedData.cs /}
{CODE-TAB-BLOCK:sql:RQL}
from index "Sales/ByTShirtColor/Fanout"
where Color == "black"
{CODE-TAB-BLOCK/}
{CODE-TABS/}

{CODE results_4@Indexes\IndexingNestedData.cs /}

{NOTE/}

{NOTE: }

<a id="performanceHints" /> __Fanout index - Performance hints__:

* Fanout indexes are typically more resource-intensive than other indexes as RavenDB has to index a large number of index-entries. 
  This increased workload can lead to higher CPU and memory utilization, potentially causing a decline in the overall performance of the index.

* When the number of index-entries generated from a single document exceeds a configurable limit,  
  RavenDB will issue a "__High indexing fanout ratio__" alert through the Studio notification center.

* You can control when this performance hint is created by setting the 
  [PerformanceHints.Indexing.MaxIndexOutputsPerDocument](../server/configuration/performance-hints-configuration#performancehints.indexing.maxindexoutputsperdocument) configuration key (default: 1024).

* The following notification will show in the notification area:  

    ![Figure 1. High indexing fanout ratio notification](images/fanout-index-performance-hint-1.png "High indexing fanout ratio notification")

* Clicking the 'Details' button will show the following info:

    ![Figure 2. Fanout index, performance hint details](images/fanout-index-performance-hint-2.png "Fanout index, performance hint details")

{NOTE/}

{NOTE: }

<a id="paging" /> __Fanout index - Paging__:

* A fanout index has more index-entries than the number of documents in the collection indexed.  
  Multiple index-entries "point" to the same document from which they originated,  
  as can be seen in the above [index-entries](../indexes/indexing-nested-data#fanoutMapIndexIndexEntries) example.

* When making a fanout index query that should return full documents (without projecting results),  
  then in this case, the `totalResults` variable (available via the `QueryStatistics` object) will contain  
  the total number of index-entries and Not the total number of resulting documents.

* __To overcome this when paging results__, you must take into account the number of "duplicate"  
  index-entries that are skipped internally by the server when serving the resulting documents.  

* Please refer to [paging through tampered results](../indexes/querying/paging#paging-through-tampered-results) for further explanations and examples. 

{NOTE/}

{PANEL/}

## Related articles

### Indexes

- [What are Indexes](../indexes/what-are-indexes)

### Querying

- [Intersect queries](../indexes/querying/intersection)
