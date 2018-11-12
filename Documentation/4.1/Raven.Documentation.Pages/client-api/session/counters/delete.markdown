# Delete a Counter  
---

{NOTE: }

###`CountersFor.Delete`

* Use the `Delete` method to remove a Counter from a document.  

* `Delete` is a member of the [CountersFor Session object](../../../client-api/session/counters/counters-overview).  

* `Delete` will not generate an error if the Counter doesn't exist.  

* In this page:
    - [Syntax](../../../client-api/session/counters/delete#syntax)
    - [Usage](../../../client-api/session/counters/delete#usage)
    - [Code Sample](../../../client-api/session/counters/delete#code-sample)
{NOTE/}

---

{PANEL: Syntax}

{CODE Delete-definition@ClientApi\Session\Counters\Counters.cs /}

| Parameters | Type | Description |
| ------------- | ------------- | ------------- |
| `counterName` |  string | Counter's name |
{PANEL/}

{PANEL: Usage}

*  **Flow**:  
  - Open a session  
  - Create an instance of `CountersFor`:
      - Either pass an explicit document ID to the CountersFor constructor -or-
      - Pass the document object returned from a [session.Load Document Method](../../../client-api/session/loading-entities#load)  
  - Execute `CountersFor.Delete`
  - Execute `session.SaveChanges` for the changes to take effect  

* **Note**:
    * A Counter you deleted will be removed only after the execution of `SaveChanges()`.  
    * Deleting a document deletes its Counters as well.  
{PANEL/}

{PANEL: Code Sample}

{CODE counters_region_Delete@ClientApi\Session\Counters\Counters.cs /}
{PANEL/}

## Related articles
### Studio
- [Studio Counters Management](../../../studio/database/documents/document-view/additional-features/counters#counters)  

###Client-API - Session
- [Counters Overview](../../../client-api/session/counters/counters-overview)
- [Create or Modify Counter](../../../client-api/session/counters/create-or-modify)
- [Retrieve Counter Data](../../../client-api/session/counters/retrieve-counter-values)

###Client-API - Operations
- [Counters Operations](../../../client-api/operations/counters/get-counters#operations--counters--how-to-get-counters)
