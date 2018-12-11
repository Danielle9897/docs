# Counters: Overview
---

{NOTE: }

* Counters are numeric data variables that can be added to a document.  
  Use a Counter to count anything that needs counting, like:
   * Sold products  
   * Voting results  
   * Any event related to the document  

* Create and manage Counters using API methods, or through the [Studio](../../../studio/database/documents/document-view/additional-features/counters).  

* In this page:  
  * [Why use Counters?](../../../client-api/session/counters/overview#why-use-counters?)  
  * [Overview](../../../client-api/session/counters/overview#overview)  
  * [Managing Counters](../../../client-api/session/counters/overview#managing-counters)  
      * [Enabling the Counters Feature](../../../client-api/session/counters/overview#enabling-the-counters-feature)  
      * [Counter Methods and the `CountersFor` object](../../../client-api/session/counters/overview#counter-methods-and-the--object)  
      * [Managing Counters using `Operations`](../../../client-api/session/counters/overview#managing-counters-using-)
{NOTE/}

---

{PANEL: Why use Counters?}

* **Convenient counting mechanism**  
Counters are very easy to manage, using simple API methods or through the Studio.  
E.g. Use counters when you want to -  
  - Keep track of the number of times a document has been viewed or rated.  
  - Count how many visitors from certain countries or regions read a document.  
  - Continuously record the number of visitors on an event page.  

* **High performance, Low resources**  
A document includes the Counter's _name_, while the Counter's actual _value_ is kept in a separate location.  
Modifying a Counter's value doesn't require the modification of the document itself.  
This results in a performant and uncostly operation.

* **High-frequency counting**  
Counters are especially useful when a very large number of counting operations is required,  
because of their speed and low resources usage.  
For example:  
  - Use Counters for an online election page, to continuously update a Number-Of-Votes Counter for each candidate.  
  - Continuously update Counters with the number of visitors in different sections of a big online store.  
{PANEL/}

{PANEL: Overview}

* **Design**  
  A document's metadata contains only the ***Counters' names-list*** for that document.  
  The ***Counter Value*** is not kept on the document metadata, but in a separate location.  
  * Therefore, any change such as adding a new counter or deleting a counter will trigger a document change,  
    while the simple modification action of the Counter Value will Not.  

* **Cumulative Counter Actions**  
   - Counter value-modification actions are cumulative, the order in which they are executed doesn't matter.  
     E.g., It doesn't matter if a Counter has been incremented by 2 and then by 7, or by 7 first and then by 2.  
   - When a Counter is deleted, the sequence of Counter actions becomes non-cumulative and may require [special attention](../../../client-api/session/counters/counters-in-a-cluster#concurrent-delete-and-increment).  

* **Counters and conflicts**  
  * Counter actions (for either name or value) do not cause conflicts.  
      - Counter actions can be executed concurrently or in any order, without causing a conflict.  
      - You can successfully modify Counters while their document is being modified by a different client.  
  * Counters are not affected by conflict states of documents they belong to.  
    In such cases, Counter actions can still be performed.  

---

* **Counter Naming Convention**  
    * Valid characters: All visible characters, [including Unicode symbols](../../../studio/database/documents/document-view/additional-features/counters#section)  
    * Length: Up to 512 bytes  
    * Encoding: UTF-8  

* **Counter Value**  
    * Valid range: Signed 64-bit integer (-9223372036854775808 to 9223372036854775807)  

* **Number of Counters per document**  
    * There's no external limitation over the number of Counters you may assign.  

* **`HasCounters` Flag**  
    * When a Counter is added to a document, RavenDB automatically sets a `HasCounters` Flag in the document's metadata.  
    * When all Counters are removed from a document, the server automatically removes this flag.  
{PANEL/}

{PANEL: Managing Counters}

###Enabling the Counters Feature

* Counters management is currently an **experimental feature** of RavenDB, and is disabled by default.  

*  To enable this feature, follow these steps:  
  - Open the RavenDB server folder. E.g. `C:\Users\Dave\Downloads\RavenDB-4.1.1-windows-x64\Server`  
  - Open settings.json for editing.  
  - Enable the Experimental Features -  
    Verify that the json file contains the following line: **"Features.Availability": "Experimental"**  
  - Save settings.json, and restart RavenDB Server.  

---

###Counter Methods and the `CountersFor` object

Managing Counters is performed using the `CountersFor` Session object.  

*  **Counter methods**:  
  - `CountersFor.Increment`: Increment the value of an existing Counter, or create a new Counter if it doesn't exist.  
  - `CountersFor.Delete`: Delete a Counter.  
  - `CountersFor.Get`: Get the current value of a Counter.  
  - `CountersFor.GetAll`: Get _all_ the Counters of a document.  

*  **Usage Flow**:  
  - Open a session.  
  - Create an instance of `CountersFor`.  
      - Either pass the `CountersFor` constructor an explicit document ID, -or-  
      - Pass it an [entity tracked by the session](../../../client-api/session/loading-entities), e.g. a document object returned from [session.query](../../../client-api/session/querying/how-to-query) or from [session.Load](../../../client-api/session/loading-entities#load).  
  - Use Counter methods to manage the document's Counters.  

* **Note: After executing `Increment` or `Delete`, you need to call `session.SaveChanges` for the changes to take effect.**  

*  **Success and Failure**:  
  - As long as the document exists, Counter actions (Increment, Get, Delete etc.) always succeed.
  - When a transaction that includes a Counter modification fails for any reason (e.g. a document concurrency conflict), 
    the Counter modification is reverted.

* **Usage Flow Samples**

  * **Using `CountersFor` by explicitly passing it a document ID (without pre-loading the document):**
{CODE counters_region_CountersFor_without_document_load@ClientApi\Session\Counters\Counters.cs /}

  * **Using `CountersFor` by passing it the document object:**
{CODE counters_region_CountersFor_with_document_load@ClientApi\Session\Counters\Counters.cs /}

---

### Managing Counters using `Operations`

* In addition to working with the high-level Session, you can manage Counters using the low-level [Operations](../../../client-api/operations/what-are-operations).  

* [CounterBatchOperation](../../../client-api/operations/counters/counter-batch) 
can operate on a set of Counters of different documents in a single request.
{PANEL/}

## Related articles
### Studio
- [Studio Counters Management](../../../studio/database/documents/document-view/additional-features/counters#counters)  

###Client-API - Session
- [Create or Modify Counter](../../../client-api/session/counters/create-or-modify)
- [Delete Counter](../../../client-api/session/counters/delete)
- [Retrieve Counter Values](../../../client-api/session/counters/retrieve-counter-values)
- [Counters Interoperability](../../../client-api/session/counters/interoperability)
- [Counters in a Cluster](../../../client-api/session/counters/counters-in-a-cluster)

###Client-API - Operations
- [Counters Operations](../../../client-api/operations/counters/get-counters#operations--counters--how-to-get-counters)
