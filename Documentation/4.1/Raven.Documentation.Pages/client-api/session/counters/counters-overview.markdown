# Counters: Overview
---

{NOTE: }

* Counters are numeric data variables that can be added to a document.  
  Use a Counter to count anything that needs counting, such as:
   * Sold products  
   * Voting results  
   * Any event related to the document  

* Create and manage Counters using API methods, or through the [Studio](../../../studio/database/documents/document-view/additional-features/counters).  
  * **Counter Value Range** - Long Integer (-2147483648 to 2147483647)  
  * **Number of Counters** -  Unlimited. You can create as many Counters as you like.  
  * **Counter Name** -        You can use whatever characters you choose, [including Unicode symbols](../../../studio/database/documents/document-view/additional-features/counters#section).  

* In this page:  
  * [Why use Counters?](../../../client-api/session/counters/counters-overview#why-use-counters?)  
  * [Managing Counters](../../../client-api/session/counters/counters-overview#managing-counters)  
      * [Enabling the Counters Feature](../../../client-api/session/counters/counters-overview#enabling-the-counters-feature)  
      * [Counter Methods and the `CountersFor` object](../../../client-api/session/counters/counters-overview#counter-methods-and-the--object)  
      * [Managing Counters using 'Operations'](../../../client-api/session/counters/counters-overview#managing-counters-using-operations)
  * [Additional Details](../../../client-api/session/counters/counters-overview#additional-details)  
      * [Success and Failure](../../../client-api/session/counters/counters-overview#success-and-failure)
      * [`HasCounters` Flag](../../../client-api/session/counters/counters-overview#flag)  
      * [Revisions](../../../client-api/session/counters/counters-overview#revisions)

{NOTE/}

---

{PANEL: Why use Counters?}

* **High performance, Low resources**  
Modifying a Counter doesn't require the modification of the whole document.  
  - A Counter is set in the document's metadata. When the server modifies a Counter, 
only the metadata is updated while the document itself is untouched.
This results in a performant and uncostly operation.

* **Concurrent modification in a distributed data network**  
A Counter in a cluster can be modified concurrently by different cluster servers.  
A server does not need to coordinate the modification with other servers, and **a conflict cannot be created**.  
  - To implement this, each server that modifies a Counter stores its local Counter value in the database along with its node tag.  
When a client requests the Counter's value, RavenDB provides it with the accumulated value of the Counter from all nodes.

* **Convenient counting mechanism**  
Counters are very easy to manage, using simple API methods or through the Studio.  
E.g. Use counters when you want to -  
  - Keep track of the number of times a document has been viewed or rated.  
  - Count how many visitors from certain countries or regions read a document.  
  - Continuously record the number of visitors on an event page.  

* **Intensive counting**  
Counters are especially useful when a very large number of counting operations is required,  
because of their speed and low resources usage.  
For example:  
  - Use Counters for an online election page, to continuously update a Number-Of-Votes Counter for each candidate.  
  - Continuously update Counters with the number of visitors in different sections of a big online store.  
{PANEL/}

{PANEL: Managing Counters}
{NOTE: }

###Enabling the Counters Feature

* Counters management is currently an **experimental feature** of RavenDB, and is disabled by default.  

*  To enable this feature, follow these steps:  
  - Open the RavenDB server folder. E.g. `C:\Users\Dave\Downloads\RavenDB-4.1.1-windows-x64\Server`  
  - Open settings.json for editing.  
  - Enable the Experimental Features -  
    Verify that the json file contains the following line: **"Features.Availability": "Experimental"**  
  - Save settings.json, and restart RavenDB Server.  
{NOTE/}

{NOTE: }

###Counter Methods and the `CountersFor` object

Managing Counters is performed using the `CountersFor` Session object.  

*  **`CountersFor` Methods**:  
  - `CountersFor.Increment`: Increment the value of an existing Counter, or create a new Counter if it doesn't exist.  
  - `CountersFor.Delete`: Delete a Counter.  
  - `CountersFor.Get`: Get the current value of a Counter.  
  - `CountersFor.GetAll`: Get _all_ the Counters of a document.  

*  **Usage Flow**:  
  - Open a session.  
  - Create an instance of `CountersFor`:
      - Either pass an explicit document ID to the `CountersFor` constructor, -or-
      - Pass it the document object returned from a [session.Load Document Method](../../../client-api/session/loading-entities#load).  
  - Use `CountersFor` methods to manage the document's Counters.  

* **Note: After executing `Increment` or `Delete`, you need to call `session.SaveChanges` for the changes to take effect.**  

* **Usage Flow Samples**

  * **Using `CountersFor` by explicitly passing it a document ID (without pre-loading the document):**
{CODE counters_region_CountersFor_without_document_load@ClientApi\Session\Counters\Counters.cs /}

  * **Using `CountersFor` by passing it the document object:**
{CODE counters_region_CountersFor_with_document_load@ClientApi\Session\Counters\Counters.cs /}

{NOTE/}

{NOTE: }

### Managing Counters using 'Operations'

* In addition to working with the high-level Session, you can manage Counters using the low-level [Operations](../../../client-api/operations/what-are-operations).  

* [CounterBatchOperation](../../../client-api/operations/counters/counter-batch) 
can operate on a set of Counters of different documents in a single request.
{NOTE/}
{PANEL/}

{PANEL: Additional Details}
{NOTE: }

####Success and Failure:
  * A Counter action (Increment, Get, Delete etc.) always succeeds (as long as the document exists).
  * When a transaction that includes a Counter modification fails for any reason (e.g. a concurrency conflict), the Counter modification is reverted.

####`HasCounters` Flag:
  * When a Counter is added to a document, RavenDB automatically sets the `HasCounters` Flag in the document's metadata.
    When all Counters are removed from a document, the server automatically removes this flag.

####Revisions:
  * When the [Revisions Feature](../../../client-api/session/revisions/what-are-revisions) is activated, 
    a new document revision is created when Creating or Deleting a Counter, because the document's metadata is modified.
    No revision is created when incrementing the Counter's value.  
  * A Revision for a document with Counters, includes an array with all current Counters' names and values.

{NOTE/}
{PANEL/}

## Related articles
### Studio
- [Studio Counters Management](../../../studio/database/documents/document-view/additional-features/counters#counters)  

###Client-API - Session
- [Create or Modify Counter](../../../client-api/session/counters/create-or-modify)
- [Delete Counter](../../../client-api/session/counters/delete)
- [Retrieve Coutner Data](../../../client-api/session/counters/retrieve-counter-values)

###Client-API - Operations
- [Counters Operations](../../../client-api/operations/counters/get-counters#operations--counters--how-to-get-counters)
