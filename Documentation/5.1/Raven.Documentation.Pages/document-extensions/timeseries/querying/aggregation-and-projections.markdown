﻿# Querying: Aggregating and Projecting Time Series

---

{NOTE: }

* **Aggregation**  
  Queries can easily create powerful statistics by aggregating time series entries 
  into groups by chosen time frames like an hour or a week, and retrieving 
  values from each group by criteria like `Min` for the lowest value, 
  `Count` for the number of values in the group, etc. Time series data can also 
  be aggregated using the entry [tags](../../../document-extensions/timeseries/overview#tags).  
  
* **Projection** by criteria  
  Queries can explicitly select the criteria by which values would be retrieved 
  and projected to the client.  
  When a query does **not** select specific criteria, RavenDB will consider it 
  an implicit selection of **all** criteria and project to the client the values 
  from each group, that match each criterion.  

    {INFO: Projecting values from Aggregated and Non-Aggregated result-sets}

    * When values are selected from a time series (or a range of time series 
     entries) that **has** been aggregated, they are selected per-group.  
    * When values are selected from a series or a range that **hasn't** 
      been aggregated, they are selected from the entire result-set.  

    {INFO/}

* In this page:  
  * [Aggregation and Projection](../../../document-extensions/timeseries/querying/aggregation-and-projections#aggregation-and-projection)  
  * [Client Usage Examples](../../../document-extensions/timeseries/querying/aggregation-and-projections#client-usage-examples)  

{NOTE/}

---

{PANEL: Aggregation and Projection}

In an RQL query, use the `group by` expression to aggregate 
time series (or ranges of time series entries) in groups by 
a chosen resolution or by the entry tag. Use the `select` 
keyword to choose and project entries by a chosen criterion.  

{INFO: You can aggregate entries by these time units:}  

* **Seconds**  
* **Minutes**  
* **Hours**  
* **Days**  
* **Months**  
* **Quarters**  
* **Years**  

* Entries can also be aggregated by their **tag**.

{INFO/}

{INFO: You can `select` values for projection by these criteria:}

* **Min()** - the lowest value  
* **Max()** - the highest value  
* **Sum()** - sum of all values  
* **Average()** - average value  
* **First()** - values of the first series entry  
* **Last()** - values of the last series entry  
* **Count()** - overall number of values in series entries  
* **Percentile(<number between 0 and 1.0>)** - the value that divides the other 
values in the series by the given ratio.  
* **Slope()** - the difference in value divided by the difference in time between 
the first and last entries.  
* **StandardDeviation()** - the _standard deviation_ of all the values.  

{INFO/}

* In this example, we group entries of users' HeartRate time series 
  and project the lowest and highest values of each group.  
  Each HeartRate entry holds a single value.
    {CODE-BLOCK: JSON}
from Employees as e
where e.Birthday > '1960-01-01'
select timeseries(
    from HeartRate between 
        '2020-05-17T00:00:00.0000000Z' 
        and '2020-05-23T00:00:00.0000000Z'
            where Tag == 'watches/fitbit'
    group by '1 days'
    select min(), max()
)
    {CODE-BLOCK/}
   * **group by '1 days'**  
     We group each user's HeartRate time series entries in consecutive 1-day groups.  
   * **select min(), max()**  
     We select the lowest and highest values of each group and project them to the client.  

* In this example, we group entries of companies' StockPrice time series 
  in consecutive 7-day groups and project the highest and lowest values 
  of each group.  
  Each StockPrice entry holds five values, the query returns the `Max` 
  and `Min` values of each:  
  Values[0] - **Open** - stock price when the trade opens  
  Values[1] - **Close** - stock price when the trade ends  
  Values[2] - **High** - highest stock price during trade time  
  Values[3] - **Low** - lowest stock price during trade time  
  Values[4] - **Volume** - overall trade volume  
    {CODE-BLOCK: JSON}
declare timeseries SP(c) 
{
    from c.StockPrices 
    where Values[4] > 500000
        group by '7 day'
        select max(), min()
}
from Companies as c
where c.Address.Country = 'USA'
select c.Name, SP(c)
    {CODE-BLOCK/}
   * **where Values[4] > 500000**  
     Query stock price behavior when the trade volume is high.  
   * **group by '7 day'**  
     Group each company's StockPrice entries in consecutive 7-day groups.  
   * **select max(), min()**  
     Select the highest (`Max`) and lowest (`Min`) 
     values of each group and project them to the client.  
     Since each entry holds 5 values, the query will project 
     5 `Max` values for each group (the highest Values[0], highest 
     Values[1], etc.) and 5 `Min` values for each group (the lowest 
     Values[0], lowest Values[1], etc.).  
   * **select c.Name, SP(c)**  
     Project the company's name along with the time series query 
     results to make the results easier to read and understand.  

* This example is similar to the one above it, except that time series 
  entries are **not aggregated**, so the highest and lowest values are 
  collected not from each group but from the entire result-set.  
  {CODE-BLOCK: JSON}
declare timeseries SP(c) 
{
    from c.StockPrices 
    where Values[4] > 500000
        select max(), min()
}
from Companies as c
where c.Address.Country = 'USA'
select c.Name, SP(c)
  {CODE-BLOCK/}
    * **select max(), min()**  
      Since there is no aggregation, the entire result-set is queried 
      and the results include only the all-time highest and lowest Open, 
      Close, High, Low and Volume values.  

* In these two example we group time series data by the entry tags. In 
  the first query we simply use `group by tag` and also filter the results based 
  on the tag value. In the second, we access the tag   using `load` and then filter:  
  {CODE-BLOCK: sql}
from Employees as e
select timeseries(
    from HeartRates
    where Tag == 'watches/fitbit' or 'Heartrate_Monitor'
    group by tag
    select min(), max()
)
  {CODE-BLOCK/}
  {CODE-BLOCK: sql}
from Employees as e
select timeseries(
    from HeartRates
    load Tag as monitor
    where monitor == 'watches/fitbit' or 'Heartrate_Monitor'
    group by tag
    select min(), max()
)
  {CODE-BLOCK/}

* Finally, in this example we group by time series in a LINQ query: 
  {CODE LINQ_GroupBy_Tag@DocumentExtensions\TimeSeries\TimeSeriesTests.cs /}

{PANEL/}

{PANEL: Client Usage Examples}

{INFO: }
You can run queries from your client using raw RQL and LINQ.  

* Learn how to run a LINQ time series query [here](../../../document-extensions/timeseries/client-api/session/query/linq-queries).  
* Learn how to run a raw RQL time series query [here](../../../document-extensions/timeseries/client-api/session/query/rql-queries).  

{INFO/}

To aggregate time series entries, use `GroupBy()` in a LINQ query 
or `group by` in a raw RQL query.  
To select time series values for projection, use `Select()` in a LINQ query 
or `select` in a raw RQL query.  

* Here we express the query we've discussed above using 
  LINQ and both RQL syntaxes.  
    {CODE-TABS}
    {CODE-TAB:csharp:LINQ ts_region_LINQ-aggregation-and-projections-StockPrice@DocumentExtensions\TimeSeries\TimeSeriesTests.cs /}
    {CODE-TAB:csharp:Raw-RQL-Select-Syntax ts_region_Raw-RQL-Select-Syntax-aggregation-and-projections-StockPrice@DocumentExtensions\TimeSeries\TimeSeriesTests.cs /}
    {CODE-TAB:csharp:Raw-RQL-Declare-Syntax ts_region_Raw-RQL-Declare-Syntax-aggregation-and-projections-StockPrice@DocumentExtensions\TimeSeries\TimeSeriesTests.cs /}
    {CODE-TABS/}

{PANEL/}

## Related articles

**Time Series Overview**  
[Time Series Overview](../../../document-extensions/timeseries/overview)  

**Studio Articles**  
[Studio Time Series Management](../../../studio/database/document-extensions/time-series)  

**Time Series Indexing**  
[Time Series Indexing](../../../document-extensions/timeseries/indexing)  

**Time Series Queries**  
[Range Selection](../../../document-extensions/timeseries/querying/choosing-query-range)  
[Filtering](../../../document-extensions/timeseries/querying/filtering)  
[Indexed Time Series Queries](../../../document-extensions/timeseries/querying/indexed-queries)

**Policies**  
[Time Series Rollup and Retention](../../../document-extensions/timeseries/rollup-and-retention)  
