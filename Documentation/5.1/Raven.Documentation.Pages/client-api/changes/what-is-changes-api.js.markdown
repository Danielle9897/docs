# What Is the Changes API

---

{NOTE: }

* The Changes API is a Push Notifications service, that allows a RavenDB Client to 
  receive messages from a RavenDB Server regarding events that occurred on the server.  
* A client can subscribe to events related to documents, indexes, operations, counters, or time series.  
* The Changes API enables you to notify users of various changes, without requiring 
  any expensive polling.  

* In this page:  
  * [Accessing Changes API](../../client-api/changes/what-is-changes-api#accessing-changes-api)  
  * [Connection interface](../../client-api/changes/what-is-changes-api#connection-interface)  
  * [Subscribing](../../client-api/changes/what-is-changes-api#subscribing)  
  * [Unsubscribing](../../client-api/changes/what-is-changes-api#unsubscribing)  
  * [FAQ](../../client-api/changes/what-is-changes-api#faq)  
     * [Changes API and Database Timeout](../../client-api/changes/what-is-changes-api#changes-api-and-database-timeout)  
     * [Changes API and Method Overloads](../../client-api/changes/what-is-changes-api#changes-api-and-method-overloads)  
  * [Changes API -vs- Data Subscriptions](../../client-api/changes/what-is-changes-api#changes-api--vs--data-subscriptions)  
{NOTE/}

---

{PANEL: Accessing Changes API}

The changes subscription is accessible by a document store through its `IDatabaseChanges` interface.

{CODE:nodejs changes_1@client-api\changes\whatIsChangesApi.js /}

| Parameters | | |
| ------------- | ------------- | ----- |
| **database** | string | Name of database to open changes API for. If `null`, the `database` configured in DocumentStore will be used. |

| Return value | |
| ------------- | ----- |
| `IDatabaseChanges` object | Instance implementing IDatabaseChanges interface. |

{PANEL/}

{PANEL: Connection interface}

Changes object interface extends `IConnectableChanges` interface that represents the connection. It exposes the following properties, methods and events.

| Properties and methods | | |
| ------------- | ------------- | ----- |
| **connected** | boolean | Indicates whether it's connected or not |
| **on("connectionStatus")** | method | Adds a listener for 'connectionStatus' event |
| **on("error")** | method | Adds a listener for 'error' event | 
| **ensureConnectedNow()** | method | Returns a `Promise` resolved once connection to the server is established. | 

{PANEL/}

{PANEL: Subscribing}

To receive notifications regarding server-side events, subscribe using one of the following methods.  

- [forAllDocuments()](../../client-api/changes/how-to-subscribe-to-document-changes#foralldocuments)
- [forAllIndexes()](../../client-api/changes/how-to-subscribe-to-index-changes#forallindexes)
- [forAllOperations()](../../client-api/changes/how-to-subscribe-to-operation-changes#foralloperations)
- [forDocument()](../../client-api/changes/how-to-subscribe-to-document-changes#fordocument)
- [forDocumentsInCollection()](../../client-api/changes/how-to-subscribe-to-document-changes#fordocumentsincollection)
- [forDocumentsStartingWith()](../../client-api/changes/how-to-subscribe-to-document-changes#fordocumentsstartingwith)
- [forIndex()](../../client-api/changes/how-to-subscribe-to-index-changes#forindex)
- [forOperationId()](../../client-api/changes/how-to-subscribe-to-operation-changes#foroperation)

{PANEL/}

{PANEL: Unsubscribing}

To end a subscription (stop listening for particular notifications) use `dispose`.  

{CODE:nodejs changes_2@client-api\changes\whatIsChangesApi.js /}

{PANEL/}

{PANEL: FAQ}

#### Changes API and Database Timeout

One or more open Changes API connections will prevent a database from becoming 
idle and unloaded, regardless of [the configuration value for database idle timeout](../../server/configuration/database-configuration#databases.maxidletimeinsec).  

---

#### Changes API and Method Overloads

{WARNING: }
To get more method overloads, especially ones supporting **delegates**, please add the 
[System.Reactive.Core](https://www.nuget.org/packages/System.Reactive.Core/) package to your project.  
{WARNING/}

{PANEL/}

{PANEL: Changes API -vs- Data Subscriptions}

**Changes API** and [Data Subscription](../../client-api/data-subscriptions/what-are-data-subscriptions) 
are services that a RavenDB Server provides subscribing clients.  
Both services respond to events that take place on the server, by sending updates 
to their subscribers.  

* **Changes API is a Push Notifications Service**.  
   * Changes API subscribers receive **notifications** regarding events that 
     took place on the server, without receiving the actual data entities 
     affected by these events.  
     For the modification of a document, for example, the client will receive 
     a [DocumentChange](../../client-api/changes/how-to-subscribe-to-document-changes#documentchange) 
     object with details like the document's ID and collection name.  

   * The server does **not** keep track of sent notifications or 
     checks clients' usage of them. It is a client's responsibility 
     to manage its reactions to such notifications.  

* **Data Subscription is a Data Consumption Service**.  
   * A Data Subscription task keeps track of document modifications in the 
     database and delivers the documents in an orderly fashion when subscribers 
     indicate they are ready to receive them. 
   * The process is fully managed by the server, leaving very little for 
     the subscribers to do besides consuming the delivered documents.  

---

|    | Data Subscriptions | Changes API 
| -- | -- | 
| What can the server Track | [Documents](../../client-api/data-subscriptions/what-are-data-subscriptions#documents-processing) | [Documents](../../client-api/changes/how-to-subscribe-to-document-changes) <br> [Indexes](../../client-api/changes/how-to-subscribe-to-index-changes) <br> [Operations](../../client-api/changes/how-to-subscribe-to-operation-changes) 
| What can the server Deliver | Documents | Notifications 
| Management | Managed by the Server | Managed by the Client 


{PANEL/}

## Related Articles

### Changes API

- [How to Subscribe to Document Changes](../../client-api/changes/what-is-changes-api)
- [How to Subscribe to Index Changes](../../client-api/changes/how-to-subscribe-to-index-changes)
- [How to Subscribe to Operation Changes](../../client-api/changes/how-to-subscribe-to-operation-changes)
