# FAQ: Transaction Support in RavenDB

{PANEL:ACID storage}

All storage operations performed in RavenDB are fully ACID (Atomicity, Consistency, Isolation, Durability), this is because internally RavenDB used a custom made storage engine called *Voron*, which guarantees all the properties of the ACID, no matter if those are executed on document, index or cluster storage data.

{PANEL/}

{PANEL:What is and is not a transaction in RavenDB?}

* A transaction represents a set of operations executed against a database as a single, atomic and isolated unit. 

* A transaction in RavenDB (read or write) is limited to a single HTTP request.  

* The terms "ACID transaction" or "transaction" refer to the storage engine transactions. Whenever a database receives an operation or batch of operations in a request, it will wrap it in a storage transaction, execute the operations and commit the transaction.

* RavenDB ensures that for a single request, all the operations in that request are transactional.    
  * If you are doing a read operation, all the data within that request is read using Snapshot Isolation.  
  * If you are performing a write operation, it is using Serializable Isolation.   

* RavenDB doesn't support a transaction over more than a single HTTP request so the interactive transactions are not implemented by RavenDB (see below for the reasoning behind this).  

* The [session](../../client-api/session/what-is-a-session-and-how-does-it-work) is pure Client API object and does not represent a transaction hence it's not meant to provide interactive transaction semantics. There isn't any server-side reference to the session.

{PANEL/}

{PANEL: Working with Transactions in RavenDB}

Transactional behavior with RavenDB is divided into two modes:

* In the first mode a user can perform all requested operations (read or write) in a single request. This can be achieved by [running a script](../../client-api/operations/patching/single-document).
  Although this is not something that most users typically do. It's rather for specific scenarios where you need to read / make decisions / update document (or documents) within the scope of a single transaction.
  If you want to just mutate a document in a transaction, [JSON Patch](../../client-api/operations/patching/json-patch-syntax) allows you to do that.

* Transactional behavior over multiple requests. With RavenDB, you cannot have a single transaction that spans all those operations with multiple requests. 
  Instead, you are expected to utilize [optimistic concurrency](../../client-api/session/configuration/how-to-enable-optimistic-concurrency) to get the same effective behavior. 
  Your changes will get committed only if no one changed the data you are modifying in the meantime. 

#### No support for interactive transactions

RavenDB client uses HTTP to communicate with RavenDB server. It means that RavenDB doesn't allow to open a transaction on the server side, make multiple operations over a network connection and then commit or rolling it back. 
This is model is called interactive transactions. It is incredibly costly model. Both in terms of engine complexity, the impact on the overall performance of the system and the capabilities you are able to offer.
In [one study](http://nms.csail.mit.edu/~stavros/pubs/OLTP_sigmod08.pdf) the cost of managing the transaction state across multiple network operations was measured in over 40% of the total system performance. 
This is because the server needs to maintain locks and state across potentially very large time frames.

Key to that design decision is that we are able to provide the same guarantees about the state of your data without needing to pay the costs of interactive transactions.
It is a model that is distinct from the classical SQL one, of interactive transactions.

#### Batch transaction model

RavenDB uses the batch transaction model, where RavenDB client submits all the operations to be run in a single transaction in one network call. 
This allows the storage engine inside RavenDB to avoid holding locks for an extended period of time and gives plenty of room to optimize the performance.

The reason for this decision is the usual interaction model in which RavenDB is used. It is used as the transactional system of record for business applications, where you'll commonly need to show data to the user,
let them make changes to it and then save it. You have one request that loads the data and show that to the user, some "think time", and then an set of updates comes from the user, which are then persisted to the database.
That model fits the batch transaction model a lot more closely than the interactive one. There is no need to hold a transaction open for the "think time" of the user. 

All the changes that were sent via _SaveChanges_ are persisted in a single unit, and if you care to avoid lost updates, you need to ensure you use [optimistic concurrency](../../client-api/session/configuration/how-to-enable-optimistic-concurrency).

{PANEL/}

{PANEL:ACID for document operations}

In RavenDB all actions performed on documents are fully ACID. An each document operation or a batch of operations applied to a set of documents sent in a single HTTP request will execute in a single transaction. The ACID properties of RavenDB are:

* _Atomicity_  - All operations are atomic. Either they succeed or fail, not midway operation. In particular, operations on multiple documents will all happen atomically, all the way or none at all.

* _Consistency and Isolation / Consistency of Scans_ - In a single transaction, all operations operate under snapshot isolation. Even if you access multiple documents, you'll get all of their state as it was in the beginning of the request.

* _Visibility_ - All transactions are immediately made available on commit. Thus, if a transaction is commit after updating two docs, you'll always see the updates to those two docs at the same time. (That is, you either see the updates to both, or you don't see the update to either one).

* _Durability_ - If an operation has completed successfully, it was fsync'ed to disk. Reads will never return any data that hasn't been flushed to disk.

All of these constraints are ensured when you use [a session](../session/what-is-a-session-and-how-does-it-work) and store documents in RavenDB with the usage of call [`SaveChanges`](../session/saving-changes).

{PANEL/}

{PANEL:BASE for query operations}

The transaction model is different when indexes are involved, because indexes are BASE (Basically Available, Soft state, Eventual consistency), not ACID. Then the following constraints are applied to query operations:

* _Basically Available_ - Index query results will be always available but they might be stale.

* _Soft state_ - The state of the system could change over the time because some amount of time is needed to perform the indexing. This is an incremental operation the less documents remains to index, the more accurate index results we have.

* _Eventual consistency_ - The database will eventually become consistent once it stops receiving new documents and the indexing operation finishes.

{PANEL/}

## Related Articles

### Server

- [Storage Engine](../../server/storage/storage-engine)
