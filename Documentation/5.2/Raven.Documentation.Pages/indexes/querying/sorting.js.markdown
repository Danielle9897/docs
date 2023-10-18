# Sort Index Query Results

---

{NOTE: }

* This article provides examples of sorting query results when querying a static-index.  

* __Prior to this article__, please refer to [Sort dynamic queries results](../../client-api/session/querying/sort-query-results) for dynamic-queries examples  
  and general knowledge about Sorting.

* All sorting capabilities provided for a dynamic query can also be used when querying a static-index.

* In this page:
    * [Order by index-field value](../../indexes/querying/sorting#order-by-index-field-value)

    * [Order results when index-field is searchable](../../indexes/querying/sorting#order-results-when-index-field-is-searchable)

    * [Additional Sorting Options](../../indexes/querying/sorting#additional-sorting-options)

{NOTE/}

---

{PANEL: Order by index-field value}

* Use `orderBy` or `orderByDescending` to order the results by the specified index-field.

{CODE-TABS}
{CODE-TAB:nodejs:Query sort_1@indexes\querying\sorting.js /}
{CODE-TAB:nodejs:Index index_1@indexes\querying\sorting.js /}
{CODE-TAB-BLOCK:sql:RQL}
from index "Products/ByUnitsInStock"
where unitsInStock > 10
order by unitsInStock as long desc
{CODE-TAB-BLOCK/}
{CODE-TABS/}

{INFO: }

__Ordering Type__:

* If no ordering type is specified in the query then the server will apply the default lexicographical ordering.

* In the above example, the ordering type was set to `Long`.

* Different ordering can be forced.  
  See section [Force ordering type](../../client-api/session/querying/sort-query-results#force-ordering-type) for all available ordering types.  
  The same syntax used with dynamic queries also applies to queries made on indexes.

{INFO/}

{PANEL/}

{PANEL: Order results when index-field is searchable}

* __When configuring an index-field__ for [full-text search](../../indexes/querying/searching), 
  the content of the index-field is broken down into terms at indexing time. 
  The specific tokenization depends on the [analyzer](../../indexes/using-analyzers) used.

* __When querying such index__, if you order by that searchable index-field, 
  results will come back sorted based on the terms, and not based on the original text of the field.
  
* To overcome this, you can define another index-field that is not searchable and sort by it. 

{CODE-TABS}
{CODE-TAB:nodejs:Index index_2@indexes\querying\sorting.js /}
{CODE-TAB:nodejs:Query sort_2@indexes\querying\sorting.js /}
{CODE-TAB-BLOCK:sql:RQL}
from index "Products/BySearchName" 
where search(name, "ch*")
order by nameForSorting
{CODE-TAB-BLOCK/}
{CODE-TABS/}

{PANEL/}

{PANEL: Additional sorting options}

* When querying an index, the following sorting options are the __same__ as when making a dynamic query.

* Refer to the examples in the links below to see how each option is achieved.

  * [Order by score](../../client-api/session/querying/sort-query-results#order-by-score)

  * [Order by random](../../client-api/session/querying/sort-query-results#order-by-random)

  * [Order by spatial](../../client-api/session/querying/sort-query-results#order-by-spatial)

  * [Chain ordering](../../client-api/session/querying/sort-query-results#chain-ordering)

  * [Custom sorters](../../client-api/session/querying/sort-query-results#custom-sorters)

{PANEL/}

## Related Articles

#### Client API

- [Query Overview](../../client-api/session/querying/how-to-query)
- [Sort dynamic queries results](../../client-api/session/querying/sort-query-results)

### Indexes

- [Indexing Basics](../../indexes/indexing-basics)
- [Sorting & Collation](../../indexes/sorting-and-collation)

### Querying

- [Query an Index](../../indexes/querying/query-index)
- [Filtering](../../indexes/querying/filtering)
- [Paging](../../indexes/querying/paging)
- [Spatial](../../indexes/querying/spatial)
