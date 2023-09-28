# Search Engine: Corax
---

{NOTE: }

* **Corax** is RavenDB's native search engine, introduced in RavenDB 
  version 6.0 as an in-house searching alternative for Lucene.  
  Lucene remains available as well, you can use either search engine 
  as you prefer.  

* The main role of the database's search engine is to **satisfy incoming queries**.  
  In RavenDB, the search engine achieves this by handling each query via an index.  
  If no relevant index exists, the search engine will create one automatically.  
  
    The search engine is the main "moving part" of the indexing mechanism, 
    which processes and indexes documents by index definitions.  

* The search engine supports both [Auto](../../indexes/creating-and-deploying#auto-indexes) 
  and [Static](../../indexes/creating-and-deploying#static-indexes) indexing 
  and can be selected separately for each.  

* The search engine can be selected per server, per database, and per index (for static indexes only).  

* In this page:  
   * [Selecting the Search Engine](../../indexes/search-engine/corax#selecting-the-search-engine)  
      * [Server Wide](../../indexes/search-engine/corax#select-search-engine-server-wide)  
      * [Per Database](../../indexes/search-engine/corax#select-search-engine-per-database)  
      * [Per Index](../../indexes/search-engine/corax#select-search-engine-per-index)  
   * [Unsupported Features](../../indexes/search-engine/corax#unsupported-features)  
      * [Unimplemented Methods](../../indexes/search-engine/corax#unimplemented-methods)  
   * [Handling of Complex JSON Objects](../../indexes/search-engine/corax#handling-of-complex-json-objects)  
   * [Compound Fields](../../indexes/search-engine/corax#compound-fields)  
   * [Limits](../../indexes/search-engine/corax#limits)  
   * [Configuration Options](../../indexes/search-engine/corax#configuration-options)  
   * [Index Training: Compression Dictionaries](../../indexes/search-engine/corax#index-training:-compression-dictionaries)  
   * [Full Text Search for Non-Alphabetical Characters](../../indexes/search-engine/corax#full-text-search-for-non-alphabetical-characters)  

{NOTE/}

---

{PANEL: Selecting the Search Engine}

* You can select your preferred search engine in several scopes:  
   * [Server-wide](../../indexes/search-engine/corax#select-search-engine-server-wide), 
     selecting which search engine will be used by all the databases hosted by this server.  
   * [Per database](../../indexes/search-engine/corax#select-search-engine-per-database), 
     overriding server-wide settings for a specific database.  
   * [Per index](../../indexes/search-engine/corax#select-search-engine-per-index), 
     overriding server-wide and per-database settings.  
     Per-index settings are available only for **static** indexes.  

     {NOTE: }
     Note that the search engine is selected for **new indexes** only.  
     These settings do not apply to existing indexes.  
     {NOTE/}

* These configuration options are available:  
   * [Indexing.Auto.SearchEngineType](../../server/configuration/indexing-configuration#indexing.auto.searchenginetype)  
     Use this option to select the search engine (either `Lucene` or `Corax`) for **auto** indexes.  
     The search engine can be selected **server-wide** or **per database**.  
   * [Indexing.Static.SearchEngineType](../../server/configuration/indexing-configuration#indexing.static.searchenginetype)  
     Use this option to select the search engine (either `Lucene` or `Corax`) for **static** indexes.  
     The search engine can be selected **server-wide**, **per database**, or **per index**.  
   * Read about additional Corax configuration options [here](../../indexes/search-engine/corax#configuration-options).  

---

### Select Search Engine: Server Wide

Select the search engine for all the databases hosted by a server 
by modifying the server's [settings.json](../../server/configuration/configuration-options#settings.json) file.  
E.g. -  
{CODE-BLOCK: csharp}
{
    "Indexing.Auto.SearchEngineType": "Corax"
    "Indexing.Static.SearchEngineType": "Corax"
}
{CODE-BLOCK/}

{NOTE: }
You must restart the server for the new settings to be read and applied.  
{NOTE/}

{NOTE: }
Selecting a new search engine will change the search engine only for indexes created from now on.  

E.g., If my configuration has been `"Indexing.Static.SearchEngineType": "Corax"` 
until now and I now changed it to `"Indexing.Static.SearchEngineType": "Lucene"`, 
static indexes created from now on will use Lucene, but static indexes created 
while Corax was selected will continue using Corax.  

After selecting a new search engine using the above options, change the search 
engine used by an existing index by [resetting](../../client-api/operations/maintenance/indexes/reset-index) 
the index.  
{NOTE/}


---

### Select Search Engine: Per Database

To select the search engine that the database would use, modify the 
relevant Database Record settings. You can easily do this via Studio:  

* Open Studio's [Database Settings](../../studio/database/settings/database-settings) 
  page, and enter `SearchEngine` in the search bar to find the search engine settings.  
  Click `Edit` to modify the default search engine.  

     ![Database Settings](images/corax-04_database-settings_01.png "Database Settings")

* Select your preferred search engine for Auto and Static indexes.  

     ![Corax Database Options](images/corax-05_database-settings_02.png "Corax Database Options")

* To apply the new settings either **disable and re-enable the database** or **restart the server**.  

     ![Default Search Engine](images/corax-06_database-settings_03.png "Default Search Engine")

---

### Select Search Engine: Per index 

You can also select the search engine that would be used by a specific index, 
overriding any per-database and per-server settings.  

#### Select Index Search Engine via Studio:  

* **Indexes-List-View** > **Edit Index Definition**  
  Open Studio's [Index List](../../studio/database/indexes/indexes-list-view) 
  view and select the index whose search engine you want to set.  

    ![Index Definition](images/corax-02_index-definition.png "Index Definition")
    1. Open the index **Configuration** tab.  
    2. Select the search engine you prefer for this index.  
       ![Per-Index Search Engine](images/corax-03_index-definition_searcher-select.png "Per-Index Search Engine")

* The indexes list view will show the changed configuration.  

    ![Search Engine Changed](images/corax-01_search-engine-changed.png "Search Engine Changed")

---

#### Select Index Search Engine using Code

While defining an index using the API, use the `SearchEngineType` 
property to select the search engine that would run the index.  
Available values: `SearchEngineType.Lucene`, `SearchEngineType.Corax`.  

* You can pass the search engine type you prefer:  
  {CODE:csharp index-definition_select-while-creating-index@Indexes/SearchEngines.cs /}  
* And set it in the index definition:  
  {CODE:csharp index-definition_set-search-engine-type@Indexes/SearchEngines.cs /}  

{PANEL/}

{PANEL: Unsupported Features}

Below are the features currently not supported by Corax.  

#### Unsupported during indexing:

* [Boosting](../../indexes/boosting) **document fields** during indexing  
  Note that boosting **documents** IS supported.  
* Indexing [WKT shapes](../../indexes/indexing-spatial-data)  
  Note that indexing **spatial points** IS supported.  
* [Custom analyzers](../../studio/database/settings/custom-analyzers)  
* [Custom Sorters](../../indexes/querying/sorting#creating-a-custom-sorter)  

#### Unsupported while querying:

* [Fuzzy Search](../../client-api/session/querying/text-search/fuzzy-search)  
* [Explanations](../../client-api/session/querying/debugging/include-explanations)  

#### Complex JSON properties:

Complex JSON properties cannot currently be indexed and searched by Corax.  
Read more about this [below](../../indexes/search-engine/corax#handling-of-complex-json-objects).  

#### Unsupported `WHERE` Methods/Terms:  

* [lucene()](../../client-api/session/querying/document-query/how-to-use-lucene)  
* [intersect()](../../indexes/querying/intersection)  

## Unimplemented Methods

Trying to use Corax with an unimplemented method (see 
[Unsupported Features](../../indexes/search-engine/corax#unsupported-features) above) 
will generate a `NotSupportedInCoraxException` exception and end the search.  

{INFO: }
E.g. -  
The following query uses the `intersect` method, which is currently not supported by Corax.  
{CODE-BLOCK: SQL}
from index 'Orders/ByCompany'
where intersect(Count > 10, Total > 3)
{CODE-BLOCK/}

If you set Corax as the search engine for the `Orders/ByCompany` index 
used by the above query, running the query will generate the following 
exception and the search will stop.  
  ![Method Not Implemented Exception](images/corax-07_exception-method-not-implemented.png "Method Not Implemented Exception")
{INFO/}

{PANEL/}

{PANEL: Handling of Complex JSON Objects}

To avoid unnecessary resource usage, the contents of complex JSON properties is not indexed by RavenDB.  
[See below](../../indexes/search-engine/corax#if-corax-encounters-a-complex-property-while-indexing) 
how auto and static indexes handle such fields.  

{NOTE: }
Lucene's approach of indexing complex fields as JSON strings usually makes no 
sense, and is not supported by Corax.  
{NOTE/}

Consider, for example, the following `orders` document:  
{CODE-BLOCK: json}
{
    "Company": "companies/27-A",
    "Employee": "employees/2-A",
    "ShipTo": {
        "City": "Torino",
        "Country": "Italy",
        "Location": {
            "Latitude": 45.0907661,
            "Longitude": 7.687425699999999
        }
    }
}
{CODE-BLOCK/}

As `Location` contains a list of key/value pairs rather than a simple numeric value or a string, 
Corax will not index its contents (see [here](../../indexes/search-engine/corax#if-corax-encounters-a-complex-property-while-indexing) 
what will be indexes).  

There are several ways to handle the indexing of complex JSON objects:  

#### 1. Index a Simple Property Contained in the Complex Field

Index one of the simple key/value properties stored within the nested object.  
In the `Location` field, for example, Location's `Latitude` and `Longitude`.  
can serve us this way:  

{CODE-BLOCK: json}
from order in docs.Orders
select new
{
    Latitude = order.ShipTo.Location.Latitude,
    Longitude = order.ShipTo.Location.Longitude
}
{CODE-BLOCK/}

---

#### 2. Index the Document Using Lucene

As long as Corax doesn't index complex JSON objects, you can always 
select Lucene as your search engine when you need to index nested properties.  

---

#### 3. Disable the Indexing of the Complex Field

You can use Corax as your search engine, but explicitly disable the indexing 
of complex objects.  
When you disable the indexing of a field this way, the field's content 
can still be stored so it may be used in 
[projection queries](../../indexes/querying/projections#projections-and-stored-fields).  

* To disable indexing for a specified field **via Studio**:  
  
     ![Disable indexing of a Nested Field](images/corax-08_disable-indexing-of-nested-field.png "Disable indexing of a Nested Field ")

     1. Open the index definition's **Fields** tab.  
     2. Click **Add Field** to specify what field Corax shouldn't index.  
     3. Enter the name of the field Corax should not index.  
     4. Set to **Yes** when Corax is used since Corax will not index the value.  
     5. Select **No** to disable indexing for the specified field.  

* To disable indexing for a specified field **using Code**:  
  {CODE:csharp index-definition_disable-indexing-for-specified-field@Indexes/SearchEngines.cs /}  

---

#### 4. Turn the complex property into a string

You can handle the complex property as a string.  

{CODE-TABS}
{CODE-TAB-BLOCK:sql:Not_Supported_By_Corax}
from order in docs.Orders
select new
{
    // This will fail for the above document when using Corax
    Location = order.ShipTo.Location
}
{CODE-TAB-BLOCK/}
{CODE-TAB-BLOCK:sql:Use_ToString()}
from order in docs.Orders
select new
{
    // .ToString() will convert the data to a string in JSON format (same as using JsonConvert.Serialize()) 
    Location = order.ShipTo.Location.ToString()
}
{CODE-TAB-BLOCK/}
{CODE-TABS/}

{NOTE: }
Serializing all the properties of a complex property into a single string, 
including names, values, brackets, and so on, produces a string that is 
**not** a good feed for analyzers and is not commonly used for searches.  
It does, however, make sense in some cases to **project** such a string.  
{NOTE/}

---

#### If Corax Encounters a Complex Property While Indexing:  

* An **auto index** will replace a complex field with a `JSON_VALUE` string.  
  This will allow basic queries over the field, like checking if it 
  exists using `Field == null` or `exists(Field)`.  

     Corax will also alert the user as follows:  
     `We have detected a complex field in an auto index. To avoid higher 
     resources usage when processing JSON objects, the values of these fields 
     will be replaced with JSON_VALUE.  
     Please consider querying on individual fields of that object or using 
     a static index.`

* If a **static index** is used and it doesn't explicitly relate 
  to the complex field, Corax will automatically exempt the field 
  from indexing (by defining **Indexing: No** for this field as shown 
  [above](../../indexes/search-engine/corax#disable-the-indexing-of-the-complex-field)).  
  
     If the static index explicitly sets the Indexing flag in 
     any other way but "no", Corax **will** throw an exception:  
     `The value of '{fieldName}' is a complex object. Indexing it 
     as a text isn't supported. You should consider querying on individual 
     fields of that object.`

{PANEL/}

{PANEL: Compound Fields}

{INFO: }
This feature should be applied to very large datasets and specific queries.  
It is meant for **experts only**.  
{INFO/}

A compound field is a Corax index field comprised of 2 simple data elements.  
{NOTE: }
A compound field can currently be composed of exactly **2 elements**.  
{NOTE/}

Expert users can define compound fields to optimize data retrieval: data stored in a compound 
field is sorted as requested by the user, and would later on be retrieved in this order 
with extreme efficiency.  
Compound fields can also be used to unify simple data elements in cohesive units to 
make the index more readable. 

* **Adding a Compound Field**  
  In an index definition, add a compound field using the `CompoundField` method.  
  Pass the method simple data elements in the order by which you want them to be sorted.  
* **Example**  
  An example of an index definition with a compound field can be:  
  {CODE-BLOCK:csharp}
private class Product_Location : AbstractIndexCreationTask<Product>
{
    public Product_Location()
    {
        Map = products => 
            from p in products 
            select new { p.Brand, p.Location };

        // Add a compound field 
        CompoundField(x => x.Brand, x => x.Location);
    }
}
{CODE-BLOCK/}

    The query that uses the indexed data will look no different than if the 
    index included no compound field, but produce the results much faster.  

    {CODE-TABS}
{CODE-TAB-BLOCK:csharp:Query}
using (var s = store.OpenSession())
{
    // Use the internal optimization previously created by the added compound field
    var products = s.Query<Product, Product_Location>()
        .Where(x => x.Brand == "RunningShoes")
        .OrderBy(x => x.Location)
        .ToList();
}
{CODE-TAB-BLOCK/}
{CODE-TAB-BLOCK:sql:RQL}
from Products 
where Brand = "RunningShoes" 
order by Location 
{CODE-TAB-BLOCK/}
{CODE-TABS/}

{PANEL/}

{PANEL: Limits}

* Corax can create and use indexes of more than `int.MaxValue` (2,147,483,647) documents.  
  To match this capacity, queries over Corax indexes can 
  [skip](../../client-api/session/querying/what-is-rql#limit) 
  a number of results that exceeds `int.MaxValue` and 
  [take](../../indexes/querying/paging#example-ii---basic-paging) 
  documents from this location.  

* The maximum number of documents that can be **projected** by a query 
  (using either Corax or Lucene) is `int.MaxValue` (2,147,483,647).  

{PANEL/}

{PANEL: Configuration Options}

Corax configuration options include:  

* [Indexing.Auto.SearchEngineType](../../server/configuration/indexing-configuration#indexing.auto.searchenginetype)  
  [Select](../../indexes/search-engine/corax#selecting-the-search-engine) the search engine for **Auto** indexes.  

* [Indexing.Static.SearchEngineType](../../server/configuration/indexing-configuration#indexing.static.searchenginetype)  
  [Select](../../indexes/search-engine/corax#selecting-the-search-engine) the search engine for **Static** indexes.  

* [Indexing.Corax.IncludeDocumentScore](../../server/configuration/indexing-configuration#indexing.corax.includedocumentscore)  
  Choose whether to include the score value in document metadata when sorting by score.  
  {NOTE: }
  Disabling this option can improve query performance.  
  {NOTE/}

* [Indexing.Corax.IncludeSpatialDistance](../../server/configuration/indexing-configuration#indexing.corax.includespatialdistance)  
  Choose whether to include spatial information in document metadata when sorting by distance.  
  {NOTE: }
  Disabling this option can improve query performance.  
  {NOTE/}

* [Indexing.Corax.MaxMemoizationSizeInMb](../../server/configuration/indexing-configuration#indexing.corax.maxmemoizationsizeinmb)  
  The maximum amount of memory that Corax can use for a memoization clause during query processing.  
  {WARNING: Expert Level Configuration}
  Please configure this option only if you are an expert.
  {WARNING/}

* [Indexing.Corax.DocumentsLimitForCompressionDictionaryCreation](../../server/configuration/indexing-configuration#indexing.corax.documentslimitforcompressiondictionarycreation)  
  Set the maximum number of documents that will be used for the training of a Corax index during dictionary creation.  
  Training will stop when it reaches this limit.  

* [Indexing.Corax.MaxAllocationsAtDictionaryTrainingInMb](../../server/configuration/indexing-configuration#indexing.corax.maxallocationsatdictionarytraininginmb)  
  Set the maximum amount of memory (in MB) that will be allocated for the training of a Corax index during dictionary creation.  
  Training will stop when it reaches this limit.  

{PANEL/}

{PANEL: Index Training: Compression Dictionaries}

When creating Corax indexes, RavenDB analyzes index contents and trains 
[compression dictionaries](https://en.wikibooks.org/wiki/Data_Compression/Dictionary_compression) 
for much higher storage and execution efficiency.  

* The larger the collection, the longer the training process will take.  
  The index, however, will become more efficient in terms of resource usage.  
* The training process can take from a few seconds to up to a minute in multiterabyte collections.  
* The IO speed of the storage system also affects the training time.  

Here are some additional things to keep in mind about Corax indexes compression dictionaries:  

* Compression dictionaries are used to store index terms more efficiently.  
  This can significantly reduce the size of the index, which can improve performance.  
* The training process is **only performed once**, when the index is created.  
* The compression dictionaries are stored with the index and are used for all subsequent 
  operations (indexing and querying).  
* The benefits of compression dictionaries are most pronounced for large collections.  
  {NOTE: }
  Training stops when it reaches either the 
  [number of documents](../../server/configuration/indexing-configuration#indexing.corax.documentslimitforcompressiondictionarycreation)
  threshold (100,000 docs by default) or the 
  [amount of memory](../../server/configuration/indexing-configuration#indexing.corax.maxallocationsatdictionarytraininginmb) 
  threshold (up to 2GB). Both thresholds are configurable.  
  {NOTE/}
* If upon creation there are less than 10,000 documents in the involved collections, 
  it may make sense to manually force an index reset after reaching 
  [100,000](../../server/configuration/indexing-configuration#indexing.corax.documentslimitforcompressiondictionarycreation) 
  documents to force retraining.  
  {NOTE: }
  Indexes are replaced in a [side-by-side](../../studio/database/indexes/indexes-list-view#indexes-list-view---side-by-side-indexing) 
  manner: existing indexes would continue running until the new ones are created, 
  to avoid any interruption to existing queries.  
  {NOTE/}

---

### Corax and the Test Index interface
Corax indexes will **not** train compression dictionaries if they are created in the 
[Test Index](../../studio/database/indexes/create-map-index#test-index) interface, 
because the testing interface is designed for indexing prototyping and the training 
process will add unnecessary overhead.

{PANEL/}

{PANEL: Full Text Search for Non-Alphabetical Characters}

The results returned by Lucene and Corax **may differ** 
when the two engines run the same full text search over 
a text that includes non-alphabetical characters.  

Both search engines remove non-alphabetical characters 
and create search terms for the remaining text.  
Searching for `din%ner`, for example, will remove the `%`, 
leave the two search terms `din` and `ner`, and run a search 
over these two terms.  

Lucene, however, will use its _PharseQuery_ to search for 
phrases combined of the two terms **in the original order 
of the terms**: first `din`, followed by `ner`. Texts 
containing the phrases `ner%din` or `ner din`, for example, 
will **not** be included in the results.  
Corax, on the other hand, will run a sub-query for each 
term as if the search was for `din` OR `ner`. Results for 
both `dinner` and `nerdin` will, therefore, be returned.  

{PANEL/}


## Related Articles

### Indexes

- [Auto Indexes](../../indexes/creating-and-deploying#auto-indexes)  
- [Static Indexes](../../indexes/creating-and-deploying#static-indexes)  
- [Boosting](../../indexes/boosting)
- [Dynamic Fields](../../indexes/using-dynamic-fields)
- [Storing Data in Index](../../indexes/storing-data-in-index)

### Studio
- [Index List View](../../studio/database/indexes/indexes-list-view)  
- [Database Settings](../../studio/database/settings/database-settings)  
- [Custom Analyzers](../../studio/database/settings/custom-analyzers)  

### Configuration
- [Configuration Options](../../server/configuration/configuration-options)
- [Configuration: Indexing](../../server/configuration/indexing-configuration)
