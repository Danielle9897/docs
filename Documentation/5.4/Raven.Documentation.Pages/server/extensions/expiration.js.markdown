﻿# Document Expiration
---

{NOTE: }

* The Expiration feature deletes expired documents, documents whose time has passed.  
  Documents that are set with a future expiration time will be automatically deleted.  

* The Expiration feature can be turned on and off while the database is already live with data.  

* In this page:  
  * [Expiration feature usages](../../server/extensions/expiration#expiration-feature-usages)  
  * [Configuring the expiration feature](../../server/extensions/expiration#configuring-the-expiration-feature)  
  * [Setting the document expiration time](../../server/extensions/expiration#setting-the-document-expiration-time)  
  * [Eventual consistency considerations](../../server/extensions/expiration#eventual-consistency-considerations)  
  * [More details](../../server/extensions/expiration#more-details)
 {NOTE/}

---

{PANEL: Expiration feature usages}

* Use the Expiration feature when data is needed to be kept for only a temporary time.  

* Examples:
  * Shopping cart data that is kept for only a specific time  
  * Email links that need to be expired after a few hours  
  * Storing a web application login session details  
  * When using RavenDB to hold cache data from a SQL server.  
{PANEL/}

{PANEL: Configuring the expiration feature}

* By default, the expiration feature is turned off.  

* The delete frequency is configurable, the default value is 60 secs.  

* The Expiration feature can be turned on and off using **Studio**, see [Setting Document Expiration in Studio](../../studio/database/settings/document-expiration).  

* The Expiration feature can also be configured using the **Client**:

{CODE:nodejs expiration_1@Server\Expiration\expiration.js /}

{PANEL/}

{PANEL: Setting the document expiration time}

* To set the document expiration time just add the `@expires` property to the document `@metadata` and set it to contain the appropriate expiration time.  

* Once the Expiration feature is enabled, the document will automatically be deleted at the specified time.  

* **Note**: The date must be in **UTC** format, not local time.  

* The document expiration time can be set using the following code from the client:  

{CODE:nodejs expiration_2@Server\Expiration\expiration.js /}

{PANEL/}

{PANEL: Eventual consistency considerations}

* Once documents are expired, it can take up to the delete frequency interval (60 seconds by default) until these expired documents are actually deleted.  

* Expired documents are _not_ filtered on load/query/indexing time, so be aware that an expired document might still be there after it has expired up to the 'delete frequency interval' timeframe.  
{PANEL/}

{PANEL: More details}

* Internally, each document that has the `@expires` property in the metadata is tracked by the RavenDB server  
  even if the expiration feature is turned off.  
This way, once the expiration feature is turned on we can easily delete all the expired documents.  

* **Note**: Metadata properties starting with `@` are internal for RavenDB usage.  
You should _not_ use the `@expires` property in the metadata for any other purpose other than the built-in expiration feature.  
{PANEL/}

## Related Articles

### Studio

- [Setting Document Expiration in Studio](../../studio/database/settings/document-expiration)
