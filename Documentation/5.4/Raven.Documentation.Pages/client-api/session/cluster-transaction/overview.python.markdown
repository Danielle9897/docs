# Cluster Transaction - Overview

---

{NOTE: }

* A session represents a single [business transaction](https://martinfowler.com/eaaCatalog/unitOfWork.html) 
  (not to be confused with an [ACID transaction](../../../client-api/faq/transaction-support)).  
 
* When opening a session, the session's mode can be set to either:  
    * **SINGLE-NODE** - transaction is executed on a specific node and then replicated
    * **CLUSTER-WIDE** - transaction is registered for execution on all nodes in an atomic fashion

* In this page:  
    * [Open a cluster transaction](../../../client-api/session/cluster-transaction/overview#open-a-cluster-transaction)
    * [Cluster-wide transaction vs. Single-node transaction](../../../client-api/session/cluster-transaction/overview#cluster-wide-transaction-vs.-single-node-transaction)
     
{NOTE/}

---

{PANEL: Open a cluster transaction}

* To work with a cluster transaction open a **cluster-wide session**,  
  by explicitly setting the `transaction_mode` to `TransactionMode.CLUSTER_WIDE`
  {CODE:python open_cluster_session_sync@ClientApi\Session\ClusterTransaction\Overview.py /}

* Similar to the single-node session,  
  any CRUD operations can be made on the cluster-wide session and the session will track them as usual.

{PANEL/}

{PANEL: Cluster-wide transaction vs. Single-node transaction}

{NOTE: }
#### Cluster-Wide
---

* Cluster-wide transactions are **fully ACID** transactions across all the database-group nodes.  
  Implemented by the Raft algorithm, the cluster must first reach a consensus.  
  Once the majority of the nodes have approved the transaction,  
  the transaction is registered for execution in the transaction queue of all nodes in an atomic fashion.  

---

* The transaction will either **succeed on all nodes or be rolled-back**.
    * The transaction is considered successful only when successfully registered on all the database-group nodes.
      Once executed on all nodes, the data is consistent and available on all nodes.  
    * A failure to register the transaction on any node will cause the transaction to roll-back on all nodes and changes will Not be applied.

---

* The only available actions are:
    * **put** or **delete** a document
    * **put** or **delete** a compare-exchange item

---

* To prevent from concurrent documents modifications,  
  the server creates [Atomic-Guards](../../../client-api/session/cluster-transaction/atomic-guards) that will be associated with the documents.  
  An Atomic-Guard will be created when:
    * A new document is created
    * Modifying an existing document that doesn't have yet an Atomic-Guard

---

* Cluster-wide transactions are **conflict-free**.

---

* The cluster-wide transaction is considered **more expensive and less performant**  
  since a cluster consensus is required prior to execution.  

---

* **Prefer a cluster-wide transaction when**:
    * Prioritizing consistency over performance & availability
    * When you would rather fail if a successful operation on all nodes cannot be ensured

{NOTE/}

{NOTE: }
#### Single-Node
---

* A single-node transaction is considered successful once executed successfully on the node the client is communicating with.
  The data is **immediately available** on that node, and it will be **eventually-consistent** across all the other database nodes when the replication process takes place soon after.

---

* **Any action is available** except for a compare-exchange item **put** or **delete**.  
  No Atomic-Guards are created by the server.

---

* **Conflicts** may occur when two concurrent transactions modify the same document on different nodes at the same time.
  They are resolved according to the defined conflict settings, either by using the latest version (default) or by following a conflict resolution script.  
  Revisions are created for the conflicting documents so that any document can be recovered.

---

* The single-node transaction is considered **faster and less expensive**,  
  as no cluster consensus is required for its execution.

---

* **Prefer a single-node transaction when**:  
    * Prioritizing performance & availability over consistency
    * When immediate data persistence is crucial
    * When you must ensure data is written even when other nodes are not reachable at the moment
    * And - when resolving occasional conflicts is acceptable

{NOTE/}

{INFO:Transactions in RavenDB}

For a detailed description of transactions in RavenDB please refer to the [Transaction support in RavenDB](../../../client-api/faq/transaction-support) article.

{INFO/}

{PANEL/}

## Related Articles

### Server Clustering

- [cluster-Wide Transactions](../../../server/clustering/cluster-transactions)


