# Create or Modify a Counter
---

{NOTE: }

###`CountersFor.Increment`

* Use the `Increment` method to **create** a new Counter, or **modify** an existing Counter's value.  

* `Increment` is a member of the [CountersFor Session object](../../../client-api/session/counters/counters-overview).  

*  If the Counter already exists, `Increment` will increase (or decrease) it value.  
   If the Counter doesn't exist, `Increment` will create it and set its initial value.  

* In this page:
  - [Syntax](../../../client-api/session/counters/create-or-modify#syntax)
  - [Usage](../../../client-api/session/counters/create-or-modify#usage)
  - [Code Sample](../../../client-api/session/counters/create-or-modify#code-sample)
{NOTE/}

---

{PANEL: Syntax}

{CODE Increment-definition@ClientApi\Session\Counters\Counters.cs /}

| Parameters | Type | Description |
| ------------- | ------------- | ------------- |
| `counterName` |  string | Counter's name |
|`incrementValue` | long | Increase Counter by this value. Default value is 1. <br> For a new Counter, this will be its initial value. |
{PANEL/}

{PANEL: Usage}

*  **Flow**:  
  - Open a session  
  - Create an instance of `CountersFor`:
      - Either pass an explicit document ID to the CountersFor constructor -or-
      - Pass the document object returned from a [session.Load Document Method](../../../client-api/session/loading-entities#load)  
  - Execute `CountersFor.Increment`
  - Execute `session.SaveChanges` for the changes to take effect  

* **Note**:
    * Modifying a Counter using `Increment` only takes effect when `session.SaveChanges()` is executed.  
    * To decrease a Counter's value, pass the method a negative "incrementValue".  
{PANEL/}

{PANEL: Code Sample}

{CODE counters_region_Increment@ClientApi\Session\Counters\Counters.cs /}
{PANEL/}

## Related articles
### Studio
- [Studio Counters Management](../../../studio/database/documents/document-view/additional-features/counters#counters)  

###Client-API - Session
- [Counters Overview](../../../client-api/session/counters/counters-overview)
- [Delete Counter](../../../client-api/session/counters/delete)
- [Retrieve Counter Data](../../../client-api/session/counters/retrieve-counter-values)

###Client-API - Operations
- [Counters Operations](../../../client-api/operations/counters/get-counters#operations--counters--how-to-get-counters)
