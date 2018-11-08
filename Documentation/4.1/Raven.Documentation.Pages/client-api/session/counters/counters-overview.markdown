# Counters: Overview
---

{NOTE: }

* Counters are numeric data variables that can be added to a document.  
  Use a Counter to count anything that needs counting, such as:
   * Products sales  
   * Voting results  
   * Any event related to the document  

* Create and manage Counters using API methods, or through the [Studio](../../../studio/database/documents/document-view/additional-features/counters/counters).  

* The number of counters that can be added to a document is unlimited.  

* **In this page**:  
  * [Why use Counters?](../../../client-api/session/counters/counters-overview#why-use-counters?)  
  * [Managing Counters](../../../client-api/session/counters/counters-overview#managing-counters)  
      * [Enabling the Counters Feature](../../../client-api/session/counters/counters-overview#enabling-the-counters-feature)  
      * [Counter Methods and the `CountersFor` object](../../../client-api/session/counters/counters-overview#counter-methods-and-the--object)  
      * [Single Counters Management and Batch Counter Operations](../todo)
{NOTE/}

---

{PANEL: Why use Counters?}

* **High performance, Low resources**  
Modifying a Counter doesn't require the modification of the whole document.  
  - A Counter is set in the document's metadata. When the server modifies a Counter, 
only the metadata is updated while the document itself is untouched.
This results in a perfomant and uncostly operation.

* **Concurrent modification in a distributed data network**  
A Counter in a cluster can be modified concurrently by different cluster servers.  
A server does not need to coordinate the modification with other servers, and a conflict cannot be created.  
  - To implement this, each server that modifies a Counter stores its local Counter value in the database along with its node tag.  
When a client requests the Counter's value, RavenDB provides it with the accumulated value of the Counter from all nodes.

* **Convenient counting mechanism**  
Counters are very easy to manage, using simple API methods or through the Studio.  
E.g. Use counters when you want to -  
  - Keep track of the number of times a document has been viewed or rated.  
  - Count how many visitors from certain countries or regions read a document.  
  - Continuously record the number of visitors in an event page.  

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

Managing counters is performed using the `CountersFor` Session object.  

*  **CountersFor methods**:  
  - `CountersFor.Increment`: Increment the value of an existing Counter, or create a new Counter if it doesn't exist.  
  - `CountersFor.Delete`: Delete a Counter.  
  - `CountersFor.Get`: Get the current value of a Counter.  
  - `CountersFor.GetAll`: Get _all_ the Counters of a document.  

*  **Usage flow**:  
  - Open a session.  
  - Use the session to load a document.  
  - Create an instance of `CountersFor`; Pass the document object returned from session.Load as a parameter.  
  - Use `CountersFor` methods to manage the document's Counters.  

* **Note: After executing `Increment` or `Delete`, you need to call `session.SaveChanges` for the changes to take effect.**  

##Usage Flow Sample:
{CODE counters_region_CountersFor@ClientApi\Session\Counters\Counters.cs /}

{NOTE/}

{NOTE: }

###Single Counters Management and Batch Counter Operations  

*  Counters management is divided to **single Counters management** and to **batch Counter operations**.  
  - **[Batch Counter management](../todo)** includes batch operations, like modifying the values of a set of Counters.  
  It is low-level management, that approaches a document and its Counters directly: Opening a session or loading the document is not required.  
  - **Single-Counter management** includes operations like creating a Counter, retrieving its value, modifying and deleting it.  
  It is high-level management, which requires that you open a session and load a document in order to manage its Counters.  
{NOTE/}

{PANEL/}
