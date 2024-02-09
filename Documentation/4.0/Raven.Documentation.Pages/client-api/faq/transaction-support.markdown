# FAQ: Transaction Support in RavenDB

---

{NOTE: }  

* In this page:
  * [ACID storage](../../client-api/faq/transaction-support#acid-storage)  
  * [What is and is not a transaction in RavenDB?](../../client-api/faq/transaction-support#what-is-and-is-not-a-transaction-in-ravendb?)  
  * [Working with Transactions in RavenDB ](../../client-api/faq/transaction-support#working-with-transactions-in-ravendb)
     * [Single node model](../../client-api/faq/transaction-support#single-node-model)
     * [Multi-master model](../../client-api/faq/transaction-support#multi-master-model)
     * [Cluster-wide transactions](../../client-api/faq/transaction-support#cluster-wide-transactions)
  * [ACID for document operations](../../client-api/faq/transaction-support#acid-for-document-operations)
  * [BASE for query operations](../../client-api/faq/transaction-support#base-for-query-operations)
  * [Consistency and isolation with the usage of the session](../../client-api/faq/transaction-support#consistency-and-isolation-with-the-usage-of-the-session)

{NOTE/}  

---

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

### Single node model

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

{INFO: }

In [one study](http://nms.csail.mit.edu/~stavros/pubs/OLTP_sigmod08.pdf) the cost of managing the transaction state across multiple network operations was measured in over 40% of the total system performance. 
This is because the server needs to maintain locks and state across potentially very large time frames.

{INFO/}

Key to that design decision is that we are able to provide the same guarantees about the state of your data without needing to pay the costs of interactive transactions.
It is a model that is distinct from the classical SQL one, of interactive transactions.

#### Batch transaction model

RavenDB uses the batch transaction model, where RavenDB client submits all the operations to be run in a single transaction in one network call. 
This allows the storage engine inside RavenDB to avoid holding locks for an extended period of time and gives plenty of room to optimize the performance.

The reason for this decision is the usual interaction model in which RavenDB is used. It is used as the transactional system of record for business applications, where you'll commonly need to show data to the user,
let them make changes to it and then save it. You have one request that loads the data and show that to the user, some "think time", and then an set of updates comes from the user, which are then persisted to the database.
That model fits the batch transaction model a lot more closely than the interactive one. There is no need to hold a transaction open for the "think time" of the user. 

All the changes that were sent via _SaveChanges_ are persisted in a single unit, and if you care to avoid lost updates, you need to ensure you use [optimistic concurrency](../../client-api/session/configuration/how-to-enable-optimistic-concurrency).

<hr/>

### Multi-master model

RavenDB uses the multi-master model. You can make writes to any node in the cluster, and they'll propagate to the other nodes in an asynchronous manner via the [replication](../../server/clustering/replication/replication). 

The interaction of transactions and distributed work is anything but trivial. Let's start from the obvious problem:

* RavenDB allows you to perform write operations on multiple nodes simultaneously.
* RavenDB explicitly allows you to write to a node that was partitioned from the rest of the network.

Taken together, this violates the [CAP theorem](https://en.wikipedia.org/wiki/CAP_theorem). Because you cannot be both consistent and tolerant to partitions at the same time. Sadly, we weren't able to come with a workaround to CAP.
RavenDB's answer to distributed transactional work is nuanced and was designed to give you as the user the choice so you can utilize RavenDB for each of your scenarios.

When running in a multi-node setup, RavenDB still uses transactions. However, they are single-node transactions. That means that the set of changes that you write in a transaction is 
committed to the node you are writing to only. It will then asynchronously replicate to the other nodes. 

#### Replication conflicts

This is an important observation, because you can get into situations where two clients wrote (even with [optimistic concurrency](../../client-api/session/configuration/how-to-enable-optimistic-concurrency) turned on) to 
the same document and both of them committed successfully (each one to a separate node). RavenDB attempts to minimize this situation by designating a [preferred node](../../client-api/configuration/load-balance/overview#the-preferred-node) for writes for each database, 
but it doesn't alleviate this issue. This is a consideration you have to take into account when using single node transactions in RavenDB (see below for running a [cluster-wide transaction](../../client-api/faq/transaction-support#cluster-wide-transactions)).

In such a case, the data will replicate across the cluster, and RavenDB will detect that there were [conflicting](../../server/clustering/replication/replication-conflicts) modifications to the document. 
It will then apply the [conflict resolution](../../studio/database/settings/conflict-resolution) strategy that you choose. That can include selecting manual resolution, 
running a [merge script](../../server/clustering/replication/replication-conflicts#conflict-resolution-script) to reconcile the conflicting versions or simply selecting the latest version.
You are in control of this behavior. 

This behavior was designed under the assumption that if you are writing data to the database, you want to have it persisted. RavenDB will do its utmost to provide that to you, 
allowing you to write to the database even in the case of partitions or partial failure states. 

{WARNING: Lost updates}

If no conflict resolution script is defined for a collection, then by default RavenDB resolves a conflict using the latest version based on the `@last-modified` property of conflicted versions of a document.
That might result in the lost update anomaly.

If you care about avoiding lost updates, you need to ensure you have the conflict resolution script defined accordingly or use [cluster-wide transaction](../../client-api/faq/transaction-support#cluster-wide-transactions).

{WARNING/}

#### Replication & transaction boundary

An important aspect to RavenDB's transactional behavior with regards to asynchronous replication. When replicating modifications to another server, 
RavenDB will ensure that the [transaction boundaries](../../server/clustering/replication/replication#replication-&-transaction-boundary) are kept even when replicating to another server. 
In other words, if you wrote two documents to one node, you are guaranteed that upon seeing the changes to one of those documents in another node, you'll also read the changes on the other one (or a later version). 

<hr/>

### Cluster-wide transactions

RavenDB also supports [cluster-wide transactions](../../client-api/session/cluster-transaction/overview). This feature modifies the way RavenDB commits a transaction, and it is meant to address scenarios 
where you prefer to get failure if the transaction cannot be persisted to a majority of the nodes in the cluster. In other words, this feature is for the scenarios where you want to favor consistency over availability.

For cluster-wide transactions, RavenDB uses the [Raft](../../server/clustering/rachis/what-is-rachis#what-is-raft-?) protocol. It ensures that the transaction is acknowledged by a majority of the nodes in
the cluster and upon commit, will be visible on any node that you'll use henceforth. Like single-node transactions, RavenDB requires that you submit the cluster-wide transaction as a single request to a database 
of all the changes you want to commit. 

Cluster-wide transactions has the notion of [atomic guards](../../session/cluster-transaction/atomic-guards) to prevent an overwrite of a document modified in a cluster transaction by changed made in another cluster transaction.

{INFO: }

The usage of atomic guards makes that cluster-wide transactions are conflict free. There is no way to make a conflict between two versions of the same document. If a document got updated meanwhile by someone else then
`ConcurrencyException` will be thrown.

{INFO/}

{PANEL/}

{PANEL:ACID for document operations}

In RavenDB all actions performed on documents are fully ACID. An each document operation or a batch of operations applied to a set of documents sent in a single HTTP request will execute in a single transaction. The ACID properties of RavenDB are:

* _Atomicity_  - All operations are atomic. Either they succeed or fail, not midway operation. In particular, operations on multiple documents will all happen atomically, all the way or none at all.

* _Consistency and Isolation / Consistency of Scans_ - In a single transaction, all operations operate under snapshot isolation. Even if you access multiple documents, you'll get all of their state as it was in the beginning of the request.

* _Visibility_ - All transactions are immediately made available on commit. Thus, if a transaction is commit after updating two docs, you'll always see the updates to those two docs at the same time. (That is, you either see the updates to both, or you don't see the update to either one).

* _Durability_ - If an operation has completed successfully, it was fsync'ed to disk. Reads will never return any data that hasn't been flushed to disk.

All of these constraints are ensured for a single request to a database when you use [a session](../../session/what-is-a-session-and-how-does-it-work). In particular, it means that each `Load` call is a separate transaction and the
[`SaveChanges`](../session/saving-changes) call will store all documents modified within a session in a single transaction.


{PANEL/}

{PANEL:BASE for query operations}

The transaction model is different when indexes are involved, because indexes are BASE (Basically Available, Soft state, Eventual consistency), not ACID. Then the following constraints are applied to query operations:

* _Basically Available_ - Index query results will be always available but they might be stale.

* _Soft state_ - The state of the system could change over the time because some amount of time is needed to perform the indexing. This is an incremental operation the less documents remains to index, the more accurate index results we have.

* _Eventual consistency_ - The database will eventually become consistent once it stops receiving new documents and the indexing operation finishes.

{PANEL/}

{PANEL: Consistency and isolation with the usage of the session}

#### Single request

The operations executed on documents are executed in ACID transactions. This applies to actions executed in a single HTTP request. As described above there are options to send a [script](../../client-api/operations/patching/single-document) that reads and modifies data in single unit
and it is executed in a single server side transaction hece it provides _Serializable_ isolation in a single-node model and _Cursor Stability_ in a multi-master model (it requires a conflict resolver to be defined, otherwise if offers _Read Committed_).

#### Multiple requests

Although the typicall interaction with a database with the usage of the session involves multiple requests (loading the data and saving it after altering). What are the guarantees when you use the session and 
execute multiple calls to the database? It depends on the configuration of the session.

* The interaction with a database involving multiple requests, with the usage of the session in its default configuration, provides _Read Committed_ for both models - single node and multi-master.

* By enabling the [optimistic concurrency](../../client-api/session/configuration/how-to-enable-optimistic-concurrency) in the session you prevent lost update so it provides _Cursor Stability_ 
(in multi-master a conflict resolver needs to be defined, otherwise _Read Committed_ is guaranteed).

* If the session is configured to use `ClusterWide` transaction mode then the optimisic concurrency and the script patching isn't available. But then you have _Cursor Stability_ even for interactions spanning 
multiple requests to a database. Write operations executed in single `SaveChanges()` guarantee _Serializable_ isolation same as in the `SingleNode` transaction mode.   

{PANEL/}

## Related Articles

### Server

- [Storage Engine](../../server/storage/storage-engine)
- [What is a Session and How Does it Work](../../session/what-is-a-session-and-how-does-it-work)
- [optimistic concurrency](../../client-api/session/configuration/how-to-enable-optimistic-concurrency)
