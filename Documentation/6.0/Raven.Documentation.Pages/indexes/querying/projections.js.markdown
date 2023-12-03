# Project Index Query Results
---

{NOTE: }

* This article provides examples of projecting query results when querying a __static-index__.

* __Prior to this article__, please refer to [Project query results - Overview](../../client-api/session/querying/how-to-project-query-results)  
  for general knowledge about Projections and dynamic-queries examples.

* In this page:

    * [SelectFields](../../indexes/querying/projections#selectfields)

    * [Projection behavior with a static-index](../../indexes/querying/projections#projection-behavior-with-a-static-index)

{NOTE/}

---

{PANEL: SelectFields}

{NOTE: }

__Example I - Projecting individual fields of the document__:

{CODE-TABS}
{CODE-TAB:nodejs:Query projections_1@indexes\querying\projections.js /}
{CODE-TAB:nodejs:Index index_1@indexes\querying\projections.js /}
{CODE-TAB-BLOCK:sql:RQL}
from index "Employees/ByNameAndTitle"
where Title == "sales representative"
select FirstName as EmployeeFirstName, LastName as EmployeeLastName
{CODE-TAB-BLOCK/}
{CODE-TABS/}

* Since the index-fields in this example are not [Stored in the index](../../indexes/storing-data-in-index), and no projection behavior was defined,  
  resulting values for `FirstName` & `LastName` will be retrieved from the matching Employee document in the storage.

* This behavior can be modified by setting the [projection behavior](../../indexes/querying/projections#projection-behavior-with-a-static-index) used when querying a static-index.

{NOTE/}

{NOTE: }

__Example II - Projecting stored fields__:

{CODE-TABS}
{CODE-TAB:nodejs:Query projections_1_stored@indexes\querying\projections.js /}
{CODE-TAB:nodejs:Index index_1_stored@indexes\querying\projections.js /}
{CODE-TAB-BLOCK:sql:RQL}
from index "Employees/ByNameAndTitleWithStoredFields"
select FirstName, LastName
{CODE-TAB-BLOCK/}
{CODE-TABS/}

* In this example, the projected fields (`FirstName` and `LastName`) are stored in the index,  
  so by default, the resulting values will come directly from the index and not from the Employee document in the storage.

* This behavior can be modified by setting the [projection behavior](../../indexes/querying/projections#projection-behavior-with-a-static-index) used when querying a static-index.
  {NOTE/}

{NOTE: }

__Example III - Projecting arrays and objects__:

{CODE-TABS}
{CODE-TAB:nodejs:Query projections_2@indexes\querying\projections.js /}
{CODE-TAB:nodejs:Index index_2@indexes\querying\projections.js /}
{CODE-TAB-BLOCK:sql:RQL}
// Using simple expression:
from index "Orders/ByCompanyAndShipToAndLines"
where Company == "companies/65-A"
select ShipTo.City as ShipToCity, Lines[].ProductName as Products

// Using JavaScript object literal syntax:
from index "Orders/ByCompanyAndShipToAndLines" as x
where Company == "companies/65-A"
select {
    ShipToCity: x.ShipTo.City,
    Products: x.Lines.map(y => y.ProductName)
}
{CODE-TAB-BLOCK/}
{CODE-TABS/}

{NOTE/}

{NOTE: }

__Example IV - Projection with expression__:

{CODE-TABS}
{CODE-TAB:nodejs:Query projections_3@indexes\querying\projections.js /}
{CODE-TAB:nodejs:Index index_1@indexes\querying\projections.js /}
{CODE-TAB-BLOCK:sql:RQL}
from index "Employees/ByNameAndTitle" as e
select {
    FullName : e.FirstName + " " + e.LastName
}
{CODE-TAB-BLOCK/}
{CODE-TABS/}

{NOTE/}

{NOTE: }

__Example V - Projection with calculations__:

{CODE-TABS}
{CODE-TAB:nodejs:Query projections_4@indexes\querying\projections.js /}
{CODE-TAB:nodejs:Index index_2@indexes\querying\projections.js /}
{CODE-TAB-BLOCK:sql:RQL}
from index "Orders/ByCompanyAndShipToAndLines" as x
select {
    TotalProducts: x.Lines.length,
    TotalDiscountedProducts: x.Lines.filter(x => x.Discount > 0).length,
    TotalPrice: x.Lines
                 .map(l => l.PricePerUnit * l.Quantity)
                 .reduce((a, b) => a + b, 0)
}
{CODE-TAB-BLOCK/}
{CODE-TABS/}

{NOTE/}

{NOTE: }

__Example VI - Projecting using functions__:

{CODE-TABS}
{CODE-TAB:nodejs:Query projections_5@indexes\querying\projections.js /}
{CODE-TAB:nodejs:Index index_1@indexes\querying\projections.js /}
{CODE-TAB-BLOCK:sql:RQL}
declare function output(x) {
    var format = p => p.FirstName + " " + p.LastName;
    return { FullName: format(x) };
}

from index "Employees/ByNameAndTitle" as e
select output(e)
{CODE-TAB-BLOCK/}
{CODE-TABS/}

{NOTE/}

{NOTE: }

__Example VII - Projecting using a loaded document__:

{CODE-TABS}
{CODE-TAB:nodejs:Query projections_6@indexes\querying\projections.js /}
{CODE-TAB:nodejs:Index index_3@indexes\querying\projections.js /}
{CODE-TAB-BLOCK:sql:RQL}
from index "Orders/ByCompanyAndShippedAt" as o
load o.Company as c
select {
    CompanyName: c.Name,
    ShippedAt: o.ShippedAt
}
{CODE-TAB-BLOCK/}
{CODE-TABS/}

{NOTE/}

{NOTE: }

__Example VIII - Projection with dates__:

{CODE-TABS}
{CODE-TAB:nodejs:Query projections_7@indexes\querying\projections.js /}
{CODE-TAB:nodejs:Index index_4@indexes\querying\projections.js /}
{CODE-TAB-BLOCK:sql:RQL}
from index "Employees/ByFirstNameAndBirthday" as x
select {
    DayOfBirth: new Date(Date.parse(x.Birthday)).getDate(),
    MonthOfBirth: new Date(Date.parse(x.Birthday)).getMonth() + 1,
    Age: new Date().getFullYear() - new Date(Date.parse(x.Birthday)).getFullYear()
}
{CODE-TAB-BLOCK/}
{CODE-TABS/}

{NOTE/}

{NOTE: }

__Example IX - Projection with metadata__:

{CODE-TABS}
{CODE-TAB:nodejs:Query projections_8@indexes\querying\projections.js /}
{CODE-TAB:nodejs:Index index_4@indexes\querying\projections.js /}
{CODE-TAB-BLOCK:sql:RQL}
from index "Employees/ByFirstNameAndBirthday" as x
select {
    Name : x.FirstName,
    Metadata : getMetadata(x)
}
{CODE-TAB-BLOCK/}
{CODE-TABS/}

{NOTE/}
{PANEL/}

{PANEL: Projection behavior with a static-index}

* __By default__, when querying a static-index and projecting query results,  
  the server will try to retrieve the fields' values from the fields [stored in the index](../../indexes/storing-data-in-index).  
  If the index does Not store those fields then the fields' values will be retrieved from the documents.

* This behavior can be modified by setting the __projection behavior__.

* Note: Storing fields in the index can increase query performance when projecting,  
  but this comes at the expense of the disk space used by the index.

{NOTE: }

__Example__:

{CODE-TABS}
{CODE-TAB:nodejs:Query projections_9@indexes\querying\projections.js /}
{CODE-TAB:nodejs:Index index_1_stored@indexes\querying\projections.js /}
{CODE-TAB:nodejs:Projection_class projection_class@indexes\querying\projections.js /}
{CODE-TAB-BLOCK:sql:RQL}
from index "Employees/ByNameAndTitleWithStoredFields"
select FirstName, Title
{CODE-TAB-BLOCK/}
{CODE-TABS/}

The projection behavior in the above example is set to `FromIndexOrThrow` and so the following applies:

* Field `FirstName` is stored in the index so the server will fetch its values from the index.

* However, field `Title` is Not stored in the index so an exception will be thrown when the query is executed.

{NOTE/}

{NOTE: }

__Projection behavior options__:

* `"Default"`  
  Retrieve values from the stored index fields when available.  
  If fields are not stored then get values from the document.

* `"FromIndex"`  
  Retrieve values from the stored index fields when available.  
  A field that is not stored in the index is skipped.

* `"FromIndexOrThrow"`  
  Retrieve values from the stored index fields when available.  
  An exception is thrown if the index does not store the requested field.

* `"FromDocument"`  
  Retrieve values directly from the documents store.  
  A field that is not found in the document is skipped.

* `"FromDocumentOrThrow"`  
  Retrieve values directly from the documents store.  
  An exception is thrown if the document does not contain the requested field.

{NOTE/}

{PANEL/}

## Related Articles

### Querying

- [Query overview](../../client-api/session/querying/how-to-query)
- [Project dynamic query results](../../client-api/session/querying/how-to-project-query-results)

### Indexes

- [Querying an index](../../indexes/querying/query-index)

### Knowledge Base

- [JavaScript engine](../../server/kb/javascript-engine)
