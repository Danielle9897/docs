﻿using System.Collections.Generic;
using System.Linq;
using Raven.Client.Documents;
using Raven.Client.Documents.Indexes;

namespace Raven.Documentation.Samples.Indexes
{
    public class DynamicFields
    {
        private interface IFoo
        {
            #region syntax
            object CreateField(string name, object value);

            object CreateField(string name, object value, bool stored, bool analyzed);

            object CreateField(string name, object value, CreateFieldOptions options);
            #endregion
        }
        
        #region dynamic_fields_1
        public class Product_1
        {
            public string Id { get; set; }
            
            // The KEYS under the Attributes object will be dynamically indexed 
            // Fields added to this object after index creation time will also get indexed
            public Dictionary<string, object> Attributes { get; set; }
        }
        #endregion
        
        #region dynamic_fields_2
        public class Products_ByAttribute : AbstractIndexCreationTask<Product_1>
        {
            public class IndexEntry
            {
                // The dynamic-index-field declaration
                // Using '_' is just a convention. Any other string can be used instead of '_'
                // The actual field name generated is defined by method CreateField below
                public object _ { get; set; }
            }

            public Products_ByAttribute()
            {
                Map = products => from p in products
                    select new Products_ByAttribute.IndexEntry
                    {
                        // Define the dynamic-index-field
                        // Call 'CreateField' to generate dynamic-index-fields from the Attributes object keys
                        
                        // The field name will be item.Key
                        // The field terms will be derived from item.Value
                        _ = p.Attributes.Select(item => CreateField(item.Key, item.Value))
                    };
            }
        }
        #endregion
        
        #region dynamic_fields_4
        public class Product_2
        {
            public string Id { get; set; }
            
            // The VALUE of ProductType will be dynamically indexed
            public string ProductType { get; set; }
            public int PricePerUnit { get; set; }
        }
        #endregion
        
        #region dynamic_fields_5
        public class Products_ByName : AbstractIndexCreationTask<Product_2>
        {
            public class IndexEntry
            {
                public object _ { get; set; }
            }

            public Products_ByName()
            {
                Map = products => from p in products
                    select new Products_ByName.IndexEntry
                    {
                        // Define the dynamic-index-field
                        // Call 'CreateField' to generate dynamic-index-fields
                        
                        // The field name will be the value of document field 'ProductType'
                        // The field terms will be derived from document field 'PricePerUnit'
                        _ = CreateField(p.ProductType, p.PricePerUnit)
                    };
            }
        }
        #endregion

        #region dynamic_fields_7
        public class Product_3
        {
            public string Id { get; set; }
            
            // For each element in this list, the VALUE of property 'PropName' will be dynamically indexed
            public List<Attribute> Attributes { get; set; }
        }

        public class Attribute
        {
            public string PropName { get; set; }
            public string PropValue { get; set; }
        }
        #endregion
        
        #region dynamic_fields_8
        public class Attributes_ByName : AbstractIndexCreationTask<Product_3>
        {
            public class IndexEntry
            {
                public object _ { get; set; }
            }

            public Attributes_ByName()
            {
                Map = products => from a in products
                    select new Attributes_ByName.IndexEntry
                    {
                        // Define the dynamic-index-field by calling CreateField
                        // A dynamic-index-field will be generated for each item in the Attributes list
                        
                        // For each item, the field name will be the value of field 'PropName'
                        // The field terms will be derived from field 'PropValue'
                        _ = a.Attributes.Select(item => CreateField(item.PropName, item.PropValue)),
                    };
            }
        }
        #endregion

        public void QueryDynamicFields()
        {
            using (var store = new DocumentStore())
            {
                using (var session = store.OpenSession())
                {
                    #region dynamic_fields_3
                    IList<Product_1> matchingDocuments = session
                        .Advanced
                        .DocumentQuery<Product_1, Products_ByAttribute>()
                         // 'Size' is a dynamic-index-field that was indexed from the Attributes object
                        .WhereEquals("Size", 42)
                        .ToList();
                    #endregion
                }
                
                using (var session = store.OpenSession())
                {
                    #region dynamic_fields_6
                    IList<Product_2> matchingDocuments = session
                        .Advanced
                        .DocumentQuery<Product_2, Products_ByName>()
                         // 'Electronics' is the dynamic-index-field that was indexed from document field 'ProductType'
                        .WhereEquals("Electronics", 23)
                        .ToList();
                    #endregion
                }
                
                using (var session = store.OpenSession())
                {
                    #region dynamic_fields_9
                    IList<Product_3> matchingDocuments = session
                        .Advanced
                        .DocumentQuery<Product_3, Attributes_ByName>()
                        // 'Width' is a dynamic-index-field that was indexed from the Attributes list
                        .WhereEquals("Width", 10)
                        .ToList();
                    #endregion
                }
            }
        }
    }
}
