# Use Session Context for Load Balancing

{NOTE: }

* The `LoadBalanceBehavior` convention determines whether client sessions 
  are allowed to select the topologies that handle their requests.  
  When enabled, a session can select a topology by `tag`.  
  Client requests sent by sessions that use the same tag, are handled by the 
  same topology.  

* Clients and administrators can use tags to load-balance traffic, e.g. by 
  using the topology tagged "users/1-A" for requests sent by this user.  

* In this page:  
   * [LoadBalanceBehavior Usage](../../../client-api/session/configuration/use-session-context-for-load-balancing#loadbalancebehavior-usage)  
   * [LoadBalanceBehavior Options](../../../client-api/session/configuration/use-session-context-for-load-balancing#loadbalancebehavior-options)  
   * [Examples](../../../client-api/session/configuration/use-session-context-for-load-balancing#examples)  

{NOTE/}

{PANEL: LoadBalanceBehavior Usage}

By default (or by explicitly setting the `LoadBalanceBehavior` and 
`ReadBalanceBehavior` conventions to `None`), the cluster designates 
a _primary node_ that all **read** and **write** client requests are 
sent to.  
In case of a failure, the primary node is automatically replaced by another.  

The [ReadBalanceBehavior](../../../client-api/configuration/load-balance-and-failover#conventions-load-balance--failover) 
convention can be used to change this behavior for **read** requests, and spread 
them among all cluster nodes. **write** requests, however, would still always 
be sent to the primary node.  

With the `LoadBalanceBehavior` convention set to `UseSessionContext`, 
the default behavior can change for **write** requests as well: sessions 
can use a `tag` while sending requests, and all the sessions that use 
the same tag will share the same primary node in the database topology.  
To load-balance requests, you can use different tags.  

Consider, for example, a database with three nodes: `A`,`B` and `C`.  
If you are using a per-user context, sessions sending their requests with 
a `users/1-A` tag may be given node **B** as their primary node (for 
both reads and writes), while sessions that send requests with the 
`users/2-A` tag will be given node **C** as primary.  

Besides allowing the session to select the primary node, RavenDB's behavior 
is **unchanged**: replication between database nodes operates normally, and 
write requests sent to one node are replicated to all other nodes.  

Spreading requests between cluster nodes is useful when there is some degree 
of separation in your system, and a specific set of sessions typically handles 
a specific set of documents.  
A common example would be setting the context to the current user or tenant, 
allowing the read and write load to spread across cluster nodes on a fair basis.  

Node failure is handled normally, and even the failure of a primary node to 
handle a particular `tag` will not be visible to your code: failover to another 
database node will happen automatically and transparently.  

{PANEL/}

{PANEL: LoadBalanceBehavior Options}

  * `None`  
    Read requests are handled based on the 
    [ReadBalanceBehavior convention](../../../client-api/configuration/load-balance-and-failover#readbalancebehavior-options).  
    Write requests are handled by the [Preferred Node](../../../client-api/configuration/load-balance-and-failover#preferred-node) 
    calculated by the client.  

  * `UseSessionContext`
     * A client session can select the topology that would handle its requests.  
       The topology is selected by `tag`.  
       Requests that use the same tag, are served by the same topology.  
       The same nodes are used for **read** and **write** requests.  
     * Administrators can disable this feature, overriding a client 
       `LoadBalanceBehavior.UseSessionContext` setting with their own 
       `LoadBalanceBehavior.None` setting.  
     * Administrators can add a hash, to randomize the topology 
       selected for the client when it uses a tag.  
       This option may be used in highly unlikely circumstances like 
       the detection of many or all sessions sending requests to the 
       same primary node, if you want to spread the load differently.  
     * Using this option to choose client request-handling topology 
       influences only the client, causing no change in replication 
       or other server functions.  

  {NOTE: }
  Conflicts may rarely happen, when multiple sessions that use different tags 
  modify a shared document concurrently. This is handled by RavenDB according 
  to your [conflict resolution strategy](../../../server/clustering/replication/replication-conflicts).  
  {NOTE/}

{PANEL/}

{PANEL: Examples}

##Example I
In this example, a client session chooses its topology by tag.  
{CODE LoadBalanceBehavior@ClientApi\Session\Configuration\ChooseTopology.cs /}

##Example II
In this example a client configuration, including the topology 
selection, is placed on the server using 
[PutClientConfigurationOperation](../../../client-api/operations/maintenance/configuration/put-client-configuration).  
{CODE PutClientConfigurationOperation@ClientApi\Session\Configuration\ChooseTopology.cs /}

##Example III
In this example a server-wide client configuration, including topology 
selection, is placed on the server using 
[PutServerWideClientConfigurationOperation](../../../client-api/operations/server-wide/configuration/put-serverwide-client-configuration).  
{CODE PutServerWideClientConfigurationOperation@ClientApi\Session\Configuration\ChooseTopology.cs /}

{PANEL/}

## Related Articles

### Session

- [What is a Session and How Does it Work](../../../client-api/session/what-is-a-session-and-how-does-it-work) 
- [Opening a Session](../../../client-api/session/opening-a-session)
- [Storing Entities](../../../client-api/session/storing-entities)
- [Loading Entities](../../../client-api/session/loading-entities)
- [Saving Changes](../../../client-api/session/saving-changes)
- [Load Balance & Failover](../../../client-api/configuration/load-balance-and-failover)

### Resolving Conflicts

- [Replication Conflicts](../../../server/clustering/replication/replication-conflicts) 
- [Client Conflict Resolution](../../../client-api/cluster/document-conflicts-in-client-side#how-can-the-conflict-can-be-resolved-from-the-client-side)
