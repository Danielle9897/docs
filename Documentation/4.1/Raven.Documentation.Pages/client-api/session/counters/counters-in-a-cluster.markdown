# Counters In a Cluster
---

{NOTE: }

* In this page:  
  * [Counters in a multi-node Cluster](../../../client-api/session/counters/counters-in-a-cluster#counters-in-a-multi-node-cluster)  
      * [Value Modification](../../../client-api/session/counters/counters-in-a-cluster#value-modification)  
      * [Counters and Replication](../../../client-api/session/counters/counters-in-a-cluster#counters-and-replication)  
      * [Concurrent Modification](../../../client-api/session/counters/counters-in-a-cluster#concurrent-modification)  
  * [Concurrent Delete and Increment](../../../client-api/session/counters/counters-in-a-cluster#concurrent-delete-and-increment)
{NOTE/}

---

{PANEL: Counters in a Multi-Node Cluster}

###Value Modification

* Each node **manages its own portion** of a Counter's total value, independently from other nodes.  

      The total value of "ProductLikes" is 80. Each node independently manages a portion of this total.  

      | Counter Name | Node Tag  | Counter Value |
      |:---:|:---:|:---:|
      | ProductLikes | A | 42 |
      | ProductLikes | B | 28 |
      | ProductLikes | C | 10 |

* When a client uses its node to **modify a Counter's value**, only this node's portion of the value is modified.  

      If a client uses node B to increment ProductLikes by 5, only node B's portion of the value is incremented.  

      | Counter Name | Node Tag  | Counter Value |
      |:---:|:---:|:---:|
      | ProductLikes | A | 42 |
      | ProductLikes | **B** | **33** |
      | ProductLikes | C | 10 |

* When a client **requests for a Counter's value**, the value's portions are collected from all nodes, accumulated, and provided as a single sum.  

      A request for ProductLikes's will yield **85**.  

      | Counter Name | Node Tag  | Counter Value |
      |:---:|:---:|:---:|
      | ProductLikes | A | **42** |
      | ProductLikes | B | **33** |
      | ProductLikes | C | **10** |
      | | | **Total Value: 42+33+10 = 85** |

---

###Counters and Replication

* **Replication due to a Counter _Value_ modification**  
   A Counter value modification is followed by a replication of the new value to the other nodes.  

      E.g.,  
      **1**. A client had used node C to increment ProductLikes by 2.  
      **2**. Node C sent nodes A and B ProductLikes's new node-C value, 12.  
      **3**. All nodes are updated with the same three value:

      | Counter Name | Node Tag  | Counter Value |
      |:---:|:---:|:---:|
      | ProductLikes | A | 42 |
      | ProductLikes | B | 33 |
      | ProductLikes | **C** | **12** |

* **Replication due to a Counter _Name_ modification**  
    - As described in the [Overview](../../../client-api/session/counters/overview#overview) section, creating or deleting a Counter triggers a document change.  
    - As a result, the whole document is replicated to all the nodes in the cluster.  

---

###Concurrent Modification

* **The same Counter can be concurrently modified by multiple clients**  
    As described in the [Counters in a multi-node cluster](../../../client-api/session/counters/counters-in-a-cluster#counters-in-a-multi-node-cluster) section, each node manages its own portion of a Counter's value.  
    As a result, clients can modify the same Counter concurrently.  
      - Nodes do **not** need to coordinate Counter modification with other nodes.  
      - Concurrent value modification cannot cause a conflict.  

{PANEL/}

{PANEL: Concurrent `Delete` and `Increment`}

A sequence of Counter actions is [cumulative](../../../client-api/session/counters/overview#overview), as long as it doesn't [delete](../../../client-api/session/counters/delete) a Counter.  
In a sequence that includes a call to delete the Counter, the order of actions does matter.  

* When [Increment](../../../client-api/session/counters/increment) and [Delete](../../../client-api/session/counters/delete) are called concurrently, their order of execution is unknown and the outcome becomes uncertain.  
  We can see this in two different scenarios:  

  * **In a single-server system**  
    Different ***clients*** may simultaneously request to Delete and Increment the same Counter.  
    The result depends upon the server's order of ***execution***.  
        - If Delete is executed _last_, the Counter will be permanently deleted.  
        - If Delete is executed _before_ Increment, the Counter will be deleted but then re-created with the incrementation as its initial value.  

  * **In a multi-node cluster**  
    Different ***nodes*** may concurrently Delete and Increment the same Counter.  
    The outcome depends upon the order of ***replication***.  
        - If the node that deleted the counter replicates the change _last_, the Counter will be permanently deleted.  
        - If the node that incremented the counter replicates the change _last_, the Counter will be deleted but then re-created 
          with the incrementation as its initial value.  

* In either case, you may find it useful to check a Counter's existence before modifying it.  
  You can do this using [Get](../../../client-api/session/counters/retrieve-counter-values) - a nonexistent Counter will return `NULL`.  
{PANEL/}

## Related articles
### Studio
- [Studio Counters Management](../../../studio/database/documents/document-view/additional-features/counters#counters)  

###Client-API - Session
- [Create or Modify Counter](../../../client-api/session/counters/create-or-modify)
- [Delete Counter](../../../client-api/session/counters/delete)
- [Retrieve Counter Data](../../../client-api/session/counters/retrieve-counter-values)

###Client-API - Operations
- [Counters Operations](../../../client-api/operations/counters/get-counters#operations--counters--how-to-get-counters)
