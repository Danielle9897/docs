# Retrieve Counter Values  
---

{NOTE: }

###`CountersFor.Get` & `CountersFor.GetAll`

* You can retrieve the value of a single Counter (**Get**), or the values of all the Counters of a document (**GetAll**).

* `Get` & `GetAll` are members of the [CountersFor Session object](../../../client-api/session/counters/counters-overview).  

* In this page:  

  * [Get Method - Retrieve a single Counter's value](../../../client-api/session/counters/retrieve-counter-values#get-method---retrieve-a-single-counter)  
      - [Get Syntax](../../../client-api/session/counters/retrieve-counter-values#get-syntax)  
      - [Get Usage](../../../client-api/session/counters/retrieve-counter-values#get-usage-flow)  
      - [Get Code Sample](../../../client-api/session/counters/retrieve-counter-values#get-code-sample)  

  * [GetAll Method - Retrieve all Counters of a document](../../../client-api/session/counters/retrieve-counter-values#getall-method---retrieve-all-counters-of-a-document)  
      - [GetAll Syntax](../../../client-api/session/counters/retrieve-counter-values#getall-syntax)  
      - [GetAll Usage](../../../client-api/session/counters/retrieve-counter-values#getall-usage-flow)  
      - [GetAll Code Sample](../../../client-api/session/counters/retrieve-counter-values#getall-code-sample)
{NOTE/}

---

{PANEL: Get Method - Retrieve a single Counter's value}

{NOTE: }

#### Get: Syntax

* Use `Get` to retrieve the current value of a single Counter.  

{CODE Get-definition@ClientApi\Session\Counters\Counters.cs /}

| Parameters | Type | Description |
| ------------- | ------------- | ------------- |
| `counterName` |  string | Counter's name |

| Return Type | Description |
| ------------- | ------------- |
| `long` | Counter's current value |
{NOTE/}

{NOTE: }

#### Get: Usage Flow

  - Open a session  
  - Create an instance of `CountersFor`:
      - Either pass an explicit document ID to the CountersFor constructor -or-
      - Pass the document object returned from a [session.Load Document Method](../../../client-api/session/loading-entities#load)  
  - Execute `CountersFor.Get`
{NOTE/}

{NOTE: }

#### Get: Code Sample

{CODE counters_region_Get@ClientApi\Session\Counters\Counters.cs /}
{NOTE/}
{PANEL/}


{PANEL: GetAll Method - Retrieve ALL Counters of a document}

{NOTE: }

#### **GetAll**: Syntax
* Use `GetAll` to retrieve all names and values of a document's Counters.  

{CODE GetAll-definition@ClientApi\Session\Counters\Counters.cs /}

| Return Type | Description |
| ------------- | ------------- |
| Dictionary | An array of Counter names and values |
{NOTE/}

{NOTE: }

####**GetAll**: Usage Flow

  - Open a session.    - 
  - Create an instance of `CountersFor`:
      - Either pass an explicit document ID to the CountersFor constructor -or-
      - Pass the document object returned from a [session.Load Document Method](../../../client-api/session/loading-entities#load).  
  - Execute `CountersFor.GetAll`.
{NOTE/}

{NOTE: }

####**GetAll**: Code Sample
{CODE counters_region_GetAll@ClientApi\Session\Counters\Counters.cs /}
{NOTE/}

{PANEL/}

## Related articles
### Studio
- [Studio Counters Management](../../../studio/database/documents/document-view/additional-features/counters#counters)  

###Client-API - Session
- [Counters Overview](../../../client-api/session/counters/counters-overview)
- [Create or Modify Counter](../../../client-api/session/counters/create-or-modify)
- [Delete Counter](../../../client-api/session/counters/delete)

###Client-API - Operations
- [Counters Operations](../../../client-api/operations/counters/get-counters#operations--counters--how-to-get-counters)
