# Counters Interoperability
---

{NOTE: }

* This section describes the relationships between Counters and other RavenDB features:  
   * How Counters are supported by the different features.  
   * How Counters trigger features' execution.  

* In this page:  
  * [Counters and Indexing](../../../client-api/session/counters/interoperability#counters-and-indexing)
  * [Counters and Revisions](../../../client-api/session/counters/interoperability#counters-and-revisions)
  * [Counters and Subscriptions](../../../client-api/session/counters/interoperability#counters-and-subscriptions)
  * [Counters and Smuggler](../../../client-api/session/counters/interoperability#counters-and-smuggler)
  * [Counters and Ongoing Tasks](../../../client-api/session/counters/interoperability#counters-and-ongoing-tasks)
  * [Features triggering by Counter modification](../../../client-api/session/counters/interoperability#features-triggering-by-counter-modification)
{NOTE/}

---

{PANEL: }

### Counters and Indexing  
 * Counter **names** [can be indexed](../../../indexes/indexing-counters#indexes--indexing-counters).  
   When Counters are indexed, modifying their names (by creating or deleting them) **does** initiate re-indexing.  
 * Counter **values** are **not** indexed.  
   Counter value modifications never initiate re-indexing.  

---

###Counters and Queries  
 * You can [query](../../../client-api/session/querying/how-to-project-query-results) documents by Counter names.  
    This query for instance -  
   `from Products select counter("Likes")`  
   retrieves documents from the "Products" collection.  
   Documents that have a Counter named `Likes`, will return the Counter's value. Others will return `null`.  

 * Querying by Counter values is currently not supported.  

---

###Counters and Revisions  
 * A [document revision](../../../client-api/session/revisions/what-are-revisions) includes all the document's Counter names and values.  
   * When Revisions are enabled, a Counter **name** modification **does** initiate the creation of a new document revision.  
   * Counter **value** modifications **never** initiate the creation of new revisions.  

---

###Counters and Subscriptions  
 * Counter **name** modifications [can](../../../client-api/changes/how-to-subscribe-to-counter-changes) be subscribed to.  
 * Counter **value** modifications **cannot** be subscribed to.  

---

###Counters and Smuggler  
 * [Smuggler](../../../client-api/smuggler/what-is-smuggler) is a DocumentStore property, that you can use to [export](../../../client-api/smuggler/what-is-smuggler#databasesmugglerexportoptions) a chosen data types to an external file, or [import](../../../client-api/smuggler/what-is-smuggler#databasesmugglerimportoptions) a data type from an existing file into the database.  
   * To set Smuggler to import or export Counters, set the `OperateOnTypes` parameter to `DatabaseItemType.Counters`.  

---

###Counters and Ongoing Tasks:
[ongoing tasks](../../../studio/database/tasks/ongoing-tasks/general-info) are tasks that you can define and schedule for routine execution.  

 * **Counters and the External Replication task**  
    The ongoing [external replication](../../../studio/database/tasks/ongoing-tasks/external-replication-task) task replicates all data automatically, including Counters.  

* **Counters and the Backup task**  
    Both [backup](../../../studio/database/tasks/ongoing-tasks/backup-task) scenarios, "backup" and "snapshot", back up (and restore) all data automatically, including Counters.  

* **Counters and the ETL task**  
    [ETL](../../../server/ongoing-tasks/etl/basics) is used in order to export data from a Raven database to an external (either Raven or SQL) database.  
    * [SQL ETL](../../../server/ongoing-tasks/etl/sql) - **Not supported**  
      Counters are currently not exported over SQL ETL to SQL databases.  
    * [RavenDB ETL](../../../server/ongoing-tasks/etl/raven) - **Supported**  
      Counters [are](../../../server/ongoing-tasks/etl/raven#counters) exported over RavenDB ETL.  
      ***Note***: While other tasks respond only to document changes, ETL is triggered by changes in Counter VALUES as well.  

{NOTE: }
###Features triggering by Counter modification
As described in the [Overview](../../../client-api/session/counters/overview#overview) section, changes to a Counter's name (by creating or deleting a Counter) 
trigger a document change, while modifying the Counter's value does not.  

The table below summarizes RavenDB features that react to changes in Counter names and/or values.  
Most features are triggered only by document changes (and not by Counter value modification), to prevent exhaustion of system resources when Counters are used extensively.  

| Feature | Triggered by **Counter Name** change (Del/Update) | Triggered by **Counter Value** modification |
|-------------|:-------------:|:-------------:|
| RavenDB ETL Task | Yes | Yes |
| External Replication task | Yes | No |
| Backup Task | Yes | No |
| Subscription update Task | Yes | No |
| Indexing | Yes | No |
| Revision creation | Yes | No |
{NOTE/}
{PANEL/}

## Related articles
### Studio
- [Studio Counters Management](../../../studio/database/documents/document-view/additional-features/counters#counters)  

###Client-API - Session
- [Counters Overview](../../../client-api/session/counters/overview)
- [Create or Modify Counter](../../../client-api/session/counters/create-or-modify)
- [Delete Counter](../../../client-api/session/counters/delete)
- [Retrieve Counter Values](../../../client-api/session/counters/retrieve-counter-values)
- [Counters in a Cluster](../../../client-api/session/counters/counters-in-a-cluster)

###Client-API - Operations
- [Counters Operations](../../../client-api/operations/counters/get-counters#operations--counters--how-to-get-counters)
