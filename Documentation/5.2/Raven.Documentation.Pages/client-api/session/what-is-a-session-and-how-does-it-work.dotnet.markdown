# What is a Session and How Does it Work  

---

{NOTE: }  

* The **Session**, which is obtained from the [Document Store](../../client-api/what-is-a-document-store),  
  is a [Unit of Work](https://en.wikipedia.org/wiki/Unit_of_work) that represents a single [_business transaction_](https://martinfowler.com/eaaCatalog/unitOfWork.html) on a particular database (not to be confused with a [transaction](../../client-api/faq/transaction-support) in terms of ACID).  

* Basic **document CRUD** actions and **document Queries** are available through the `Session`.  
  More advanced options are available using `Advanced` Session operations.  

* The Session **tracks all changes** made to all entities that it has either loaded, stored, or queried for,  
  and persists to the server only what is needed when `SaveChanges()` is called.  

* The number of server requests allowed per session is configurable (default is 30).  
  See: [Change maximum number of requests per session](../../client-api/session/configuration/how-to-change-maximum-number-of-requests-per-session).  

* In this page:
  * [Concept of the session](../../client-api/session/what-is-a-session-and-how-does-it-work#concept-of-the-session)  
  * [Unit of work pattern](../../client-api/session/what-is-a-session-and-how-does-it-work#unit-of-work-pattern)  
      * [Tracking changes](../../client-api/session/what-is-a-session-and-how-does-it-work#tracking-changes)
      * [Create document example](../../client-api/session/what-is-a-session-and-how-does-it-work#create-document-example)
      * [Modify document example](../../client-api/session/what-is-a-session-and-how-does-it-work#modify-document-example)
  * [Identity map pattern](../../client-api/session/what-is-a-session-and-how-does-it-work#identity-map-pattern)
  * [Batching & Transactions](../../client-api/session/what-is-a-session-and-how-does-it-work#batching-&-transactions)
  * [Concurrency control](../../client-api/session/what-is-a-session-and-how-does-it-work#concurrency-control)<br><br>
  * [Reducing server calls (best practices) for:](../../client-api/session/what-is-a-session-and-how-does-it-work#reducing-server-calls-(best-practices)-for:)  
      * [The N+1 problem](../../client-api/session/what-is-a-session-and-how-does-it-work#the-select-n1-problem)    
      * [Large query results](../../client-api/session/what-is-a-session-and-how-does-it-work#large-query-results)    
      * [Retrieving results on demand (Lazy)](../../client-api/session/what-is-a-session-and-how-does-it-work#retrieving-results-on-demand-lazy)

{NOTE/}  

---

{PANEL: Concept of the session}

The session (`ISession` / `IAsyncDocumentSession`) is based on the [Unit of Work](https://martinfowler.com/eaaCatalog/unitOfWork.html) and [Identity Map](https://martinfowler.com/eaaCatalog/identityMap.html) patterns.
It's a primary interface that your application will interact with.

The session is container that allows you to load, create or update entities and it keeps track of changes. It means that upon a completion of a [business transaction](https://martinfowler.com/eaaCatalog/unitOfWork.html) 
it aggregates the modifications and sends them back to a database.

Note that a business transaction typically spans multiple requests such as loading of documents or execution of queries but the modifications made within the session will be batched and sent together in a single (HTTP) request to a database.

The session is a pure client side object. Openning of the session does not create any connection to a database. Session state isn't reflected on the database side during its duration. 
The changes are sent to a database only on an explicit user's request (see more about [saving changes](../../client-api/session/saving-changes)).

{INFO: }

RavenDB Client API is a native way to interact with a RavenDB database. It _is not_ an Object–relational mapping (ORM) tool. Although if you're familiar with NHibernate of Entity Framework ORMs you'll recognize that
the session is equivalent of NHibernate's session and Entity Framework's DataContext which implement UoW pattern as well.

{INFO/}

{PANEL/}  

{PANEL: Unit of work pattern}  

#### Tracking changes

* Using the Session, perform needed operations on your documents.  
  e.g. create a new document, modify an existing document, query for documents, etc.  
* Any such operation '*loads*' the document as an entity to the Session,  
  and the entity is added to the __Session's entities map__.  
* The Session **tracks all changes** made to all entities stored in its internal map.  
  You don't need to manually track the changes and decide what needs to be saved and what doesn't, the Session will do it for you.  
  Prior to saving, you can review the changes made if necessary. See: [Check for session changes](../../client-api/session/how-to/check-if-there-are-any-changes-on-a-session).
* All the tracked changes are combined & persisted in the database only when calling `SaveChanges()`.    
* Entity tracking can be disabled if needed. See:
    * [Disable entity tracking](../../client-api/session/configuration/how-to-disable-tracking)
    * [Clear session](../../client-api/session/how-to/clear-a-session)

---

#### Create document example  
* The Client API, and the Session in particular, is designed to be as straightforward as possible.  
  Open the session, do some operations, and apply the changes to the RavenDB server.  
* The following example shows how to create a new document in the database using the Session.  

{CODE-TABS}
{CODE-TAB:csharp:Sync session_usage_1@ClientApi\session\WhatIsSession.cs /}
{CODE-TAB:csharp:Async session_usage_1_async@ClientApi\session\WhatIsSession.cs /}
{CODE-TABS/}

#### Modify document example  
* The following example modifies the content of an existing document.  

{CODE-TABS}
{CODE-TAB:csharp:Sync session_usage_2@ClientApi\session\WhatIsSession.cs /}
{CODE-TAB:csharp:Async session_usage_2_async@ClientApi\session\WhatIsSession.cs /}
{CODE-TABS/}

{PANEL/}  

{PANEL:Identity map pattern}  

* The session implements the [Identity Map Pattern](https://martinfowler.com/eaaCatalog/identityMap.html).
* The first `Load()` call goes to the server and fetches the document from the database.  
  The document is then stored as an entity in the Session's entities map.  
* All subsequent `Load()` calls to the same document will simply retrieve the entity from the Session -  
  no additional calls to the server are made.  

{CODE-TABS}
{CODE-TAB:csharp:Sync session_usage_3@ClientApi\session\WhatIsSession.cs /}
{CODE-TAB:csharp:Async session_usage_3_async@ClientApi\session\WhatIsSession.cs /}
{CODE-TABS/}

* Note:  
  To override this behavior and force `Load()` to fetch the latest changes from the server see: 
  [Refresh an entity](../../client-api/session/how-to/refresh-entity).  

{PANEL/}

{PANEL: Batching & Transactions}

#### Batching

* Remote calls to a server over the network are among the most expensive operations an application makes.  
  The session optimizes this by batching all __write operations__ it has tracked into the `SaveChanges()` call.  
* When calling SaveChanges, the session checks its state for all changes made that need to be saved in the database,  
  and combines them into a single batch that is sent to the server as a __single remote call__.

#### Transactions

* The client API will not attempt to provide transactional semantics over the entire session. The session **does not** represent a transaction (nor a transaction scope) in terms of ACID transactions. 
  RavenDB provides transactions over individual request so each call made with the usage of the session will be processed in separate transaction on a database side. It applies to both reads and writes. 

##### Read transactions

* Each call retrieving data from a database will generate a separate request - multiple requests means separate transactions.
* You can read _multiple_ documents in a single request by using overloads of `Load()` method allowing to specify collection of IDs or a prefix of ID. Also a query which can return multiple documents is executed in a single request,
  hence it's processed in a single read transaction. Also [includes](../../client-api/session/loading-entities#load-with-includes) allow to retrieve additional documents in a single request.

##### Write transactions

* The batched operations that are sent in the `SaveChanges()` will complete transactionally since this call generates a single request to a database. 
  In other words, either all changes are saved as a **Single Atomic Transaction** or none of them are.  
  So once SaveChanges returns successfully, it is guaranteed that all changes are persisted to the database.  
* The SaveChanges is the only time when a RavenDB Client API sends updates to the server from the Session, so you will experience a reduced number of network calls.
* In order to execute an operation which loads and updates a document in the same write transaction then the patching feature is the way to go. 
  Either with the usage of [JavaScript patch](../../client-api/operations/patching/single-document) syntax or [JSON Patch](../../client-api/operations/patching/json-patch-syntax) syntax.

#### Transaction mode

* The session's transaction mode can be set to either:
  * __Single-Node__ - transaction is executed on a specific node and then replicated
  * __Cluster-Wide__ - transaction is registered for execution on all nodes in an atomic fashion
* Learn more about these modes in [Cluster-wide vs. Single-node](../../client-api/session/cluster-transaction/overview#cluster-wide-transaction-vs.-single-node-transaction) transactions. 

{WARNING: }

By saying "session's transaction mode" we're reffering to the transaction that will be executed on the server side upon sending the request to a database. As mentioned earlier, the session itself does not represent an ACID transaction.

{WARNING/}

{INFO:Transactions in RavenDB}

For the detailed description of transactions in RavenDB please navigate to a [dedicated aricle](../../client-api/faq/transaction-support).

{INFO/}

{PANEL/}

{PANEL: Concurrency control}

The typical model of the usage of the session is:

  * load documents
  * modify the documents
  * save changes

A sample real case scenario would be:

  * loading an entity from a database,
  * showing Edit form on the screen,
  * and updating it after user completes editing.

When using the session the above interaction with a database will be splitted into two parts - the load part and save changes part. Both of them will be executed separately, via their own HTTP requests.
It means that the data that was loaded and edited could be changed by another user meanwhile. The session API provides concurrency control features to deal with that.

#### Default strategy on single node

* The concurrency checks are turned off by default. It means that with the default configuration of the session, concurrent changes to the same document will use the Last Write Wins strategy.   
* Second write of an updated document will override previos version, causing data loss. This behavior is a consideration you have to take into account when using the session with single node transaction mode.

#### Optimistic concurrency on single node

* The modify / edit stage can take a while, happen offline etc. In order to prevent the overwites it's possible to configure the session to use [optimistic concurrency](../../client-api/session/configuration/how-to-enable-optimistic-concurrency).  
* Once it's enabled then the session will do a version tracking, so it will check that any modified document was changed since the time it was loaded into the session. The version it keeps track of is a [change vector](../../server/clustering/replication/change-vector).  
* When `SaveChanges()` is called, the session also sends the version of the documents that were modified, so a database can check if they has been changed meanwhile. 
If they did change, it will abort the transaction with a `ConcurrencyException` and the caller can retry or handle the error accordingly.

#### Concurrency control in cluster wide transactions

* In the case of a cluster wide transaction, RavenDB server maintains a cluster-wide version for each modified document and updates that through Raft protocol. It means that when using a session with the cluster-wide transaction mode
then you'll get a `ConcurrencyException` upon `SaveChanges()` call if someone else has modified and saved the same data in another cluster-wide transaction meanwhile.  
* More info about cluster transactions can be foud [here](../../client-api/session/cluster-transaction/overview).

{PANEL/}

{PANEL: Reducing server calls (best practices) for:}

#### The select N+1 problem
* The Select N+1 problem is common 
  with all ORMs and ORM-like APIs.  
  It results in an excessive number of remote calls to the server, which makes a query very expensive.  
* Make use of RavenDB's `include()` method to include related documents and avoid this issue.  
  See: [Document relationships](../../client-api/how-to/handle-document-relationships)  
<br>
#### Large query results
* When query results are large and you don't want the overhead of keeping all results in memory,  
  then you can [Stream query results](../../client-api/session/querying/how-to-stream-query-results).  
  A single server call is executed and the client can handle the results one by one.  
* [Paging](../../indexes/querying/paging) also avoids getting all query results at one time,  
  however, multiple server calls are generated - one per page retrieved.  
<br>
#### Retrieving results on demand (Lazy)
* Query calls to the server can be delayed and executed on-demand as needed using `Lazily()`
* See [Perform queries lazily](../../client-api/session/querying/how-to-perform-queries-lazily)

{PANEL/}

## Related Articles  

### Client API  

- [Open a Session](../../client-api/session/opening-a-session)
- [Storing Entities](../../client-api/session/storing-entities)
- [Loading Entities](../../client-api/session/loading-entities)
- [Saving Changes](../../client-api/session/saving-changes)
- [Query Overview](../../client-api/session/querying/how-to-query)
- [Querying an Index](../../indexes/querying/query-index)
- [What is a Document Store](../../client-api/what-is-a-document-store)
- [Optimistic Concurrency](../../client-api/session/configuration/how-to-enable-optimistic-concurrency)
- [Transaction Support](../../client-api/faq/transaction-support)
