﻿# Get Time Series Entries 
---

{NOTE: }

* Use `timeSeriesFor.get` to retrieve a range of entries from a **single** time series.  
  To retrieve a range of entries from **multiple** series, 
  use the [GetMultipleTimeSeriesOperation](../../../../../document-extensions/timeseries/client-api/operations/get#getmultipletimeseriesoperation) operation.

* The retrieved data can be paged to get the time series entries gradually, one custom-size page at a time.

* By default, the session will track the retrieved time series data. 
  See [disable tracking](../../../../../client-api/session/configuration/how-to-disable-tracking) to learn how to disable.

* When getting the time series entries,  
  you can also _include_ the series' **parent document** and/or **documents referred to by the entry tag**.  
  Learn more [below](../../../../../document-extensions/timeseries/client-api/session/get/get-entries#include-parent-and-tagged-documents).

* Calling `timeSeriesFor.get` will result in a trip to the server unless the series' parent document was loaded  
  (or queried for) with the time series included beforehand.  
  Learn more in [Including time series](../../../../../document-extensions/timeseries/client-api/session/include/overview).

* In this page:  
  * [`get` usage](../../../../../document-extensions/timeseries/client-api/session/get/get-entries#get-usage)
  * [Examples](../../../../../document-extensions/timeseries/client-api/session/get/get-entries#examples)
      * [Get all entries](../../../../../document-extensions/timeseries/client-api/session/get/get-entries#get-all-entries)
      * [Get range of entries](../../../../../document-extensions/timeseries/client-api/session/get/get-entries#get-range-of-entries)
      * [Get entries with multiple values](../../../../../document-extensions/timeseries/client-api/session/get/get-entries#get-entries-with-multiple-values)
  * [Include parent and tagged documents](../../../../../document-extensions/timeseries/client-api/session/get/get-entries#include-parent-and-tagged-documents)
  * [Syntax](../../../../../document-extensions/timeseries/client-api/session/get/get-entries#syntax)

{NOTE/}

---

{PANEL: `get` usage }

* Open a session.  
* Create an instance of `timeSeriesFor` and pass it the following:
    * Provide an explicit document ID, -or-  
      pass an [entity tracked by the session](../../../../../client-api/session/what-is-a-session-and-how-does-it-work#unit-of-work-pattern),
      e.g. a document object returned from [session.query](../../../../../client-api/session/querying/how-to-query) or from [session.load](../../../../../client-api/session/loading-entities#load).
    * Specify the time series name.
* Call `timeSeriesFor.get`.

{PANEL/}

{PANEL: Examples}

{NOTE: }

<a id="get-all-entries" /> __Get all entries__:

---

In this example, we retrieve all entries of the "Heartrate" time series.  
The ID of the parent document is explicitly specified.  

{CODE:nodejs get_Entries_1@documentExtensions\timeSeries\client-api\getEntries.js /}

{NOTE/}
{NOTE: }

<a id="get-range-of-entries" /> __Get range of entries__:

---

In this example, we query for a document and get a range of entries from its "Heartrate" time series.

{CODE:nodejs get_Entries_2@documentExtensions\timeSeries\client-api\getEntries.js /}

{NOTE/}
{NOTE: }

<a id="get-entries-with-multiple-values" /> __Get entries with multiple values__:

---

* Here, we check if a stock's closing price is rising consecutively over three days.  
  This example is based on the sample entries that were entered in [this example](../../../../../document-extensions/timeseries/client-api/session/append#append-entries-with-multiple-values).

* Since each time series entry contains multiple StockPrice values,  
  we include a sample that uses [named time series values](../../../../../document-extensions/timeseries/client-api/named-time-series-values)
  to make the code easier to read.

{CODE-TABS}
{CODE-TAB:nodejs:Native get_Entries_3@documentExtensions\timeSeries\client-api\getEntries.js /}
{CODE-TAB:nodejs:Named get_Entries_3_named@documentExtensions\timeSeries\client-api\getEntries.js /}
{CODE-TABS/}

{NOTE/}

{PANEL/}

{PANEL: Include parent and tagged documents}

* When retrieving time series entries using `timeSeriesFor.get`,  
  you can include the series' parent document and/or documents referred to by the entries [tags](../../../../../document-extensions/timeseries/overview#tags).  

* The included documents will be cached in the session, and instantly retrieved from memory if loaded by the user.

* Use the following syntax to include the parent or tagged documents:

{CODE:nodejs get_Entries_4@documentExtensions\timeSeries\client-api\getEntries.js /}

{PANEL/}

{PANEL: Syntax}

{CODE:nodejs syntax_1@documentExtensions\timeSeries\client-api\getEntries.js /}

<br/>

| Parameter        | Type                       | Description                                                                                                                                                   |
|------------------|----------------------------|---------------------------------------------------------------------------------------------------------------------------------------------------------------|
| **from**         | `Date`                     | Get the range of time series entries starting from this timestamp (inclusive).<br/>Pass `null` to use the minimum date value.                                 |
| **to**           | `Date`                     | Get the range of time series entries ending at this timestamp (inclusive).<br/>Pass `null` to use the maximum date value.                                     |
| **start**        | `number`                   | Paging first entry.<br>E.g. 50 means the first page would start at the 50th time series entry. <br> Default: 0, for the first time-series entry.              |
| **pageSize**     | `number`                   | Paging page-size.<br>E.g. set `pageSize` to 10 to retrieve pages of 10 entries.<br>Default: the equivalent of C# `int.MaxValue`, for all time series entries. |
| **includes**     | `(includeBuilder) => void` | Builder function with a fluent API<br>containing the `includeTags` and `includeDocument` methods.                                                             |

| Return value                 |                                              |
|------------------------------|----------------------------------------------|
| `Promise<TimeSeriesEntry[]>` | A `Promise` resolving to the list of entries |

{CODE:nodejs syntax_2@documentExtensions\timeSeries\client-api\getEntries.js /}

{PANEL/}

## Related articles

**Client API**  
[Time Series API Overview](../../../../../document-extensions/timeseries/client-api/overview)  

**Studio Articles**  
[Studio Time Series Management](../../../../../studio/database/document-extensions/time-series)  

**Querying and Indexing**  
[Time Series Querying](../../../../../document-extensions/timeseries/querying/overview-and-syntax)  
[Time Series Indexing](../../../../../document-extensions/timeseries/indexing)  

**Policies**  
[Time Series Rollup and Retention](../../../../../document-extensions/timeseries/rollup-and-retention)  
