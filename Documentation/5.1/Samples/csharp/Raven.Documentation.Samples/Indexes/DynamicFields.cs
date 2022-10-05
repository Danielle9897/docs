﻿using System.Collections.Generic;
using System.Linq;
using Raven.Client.Documents;
using Raven.Client.Documents.Indexes;

namespace Raven.Documentation.Samples.Indexes
{
    namespace Syntax
    {
        public class CreateFieldSyntax
        {
            private interface ICreateField
            {
                #region syntax
                object CreateField(string name, object value);

                object CreateField(string name, object value, bool stored, bool analyzed);

                object CreateField(string name, object value, CreateFieldOptions options);
                #endregion
            }
        }
    }
    
    namespace Example1
    {
        #region dynamic_fields_1
        public class Product
        {
            public string Id { get; set; }
            
            // The KEYS under the Attributes object will be dynamically indexed 
            // Fields added to this object after index creation time will also get indexed
            public Dictionary<string, object> Attributes { get; set; }
        }
        #endregion

        #region dynamic_fields_2
        public class Products_ByAttributeKey : AbstractIndexCreationTask<Product>
        {
            public Products_ByAttributeKey()
            {
                Map = products => from p in products
                    select new
                    {
                        // Call 'CreateField' to generate dynamic-index-fields from the Attributes object keys
                        // Using '_' is just a convention. Any other string can be used instead of '_'
                        
                        // The actual field name will be item.Key
                        // The actual field terms will be derived from item.Value
                        _ = p.Attributes.Select(item => CreateField(item.Key, item.Value))
                    };
            }
        }
        #endregion
        
        public class DynamicFields
        {
            public void QueryDynamicFields()
            {
                using var store = new DocumentStore();
                using var session = store.OpenSession();

                #region dynamic_fields_3
                IList<Product> matchingDocuments = session
                    .Advanced
                    .DocumentQuery<Product, Products_ByAttributeKey>()
                     // 'Size' is a dynamic-index-field that was indexed from the Attributes object
                    .WhereEquals("Size", 42)
                    .ToList();
                #endregion
            }
        }
    }
    
    namespace Example2
    {
        #region dynamic_fields_4
        public class Product
        {
            public string Id { get; set; }
            
            // The VALUE of ProductType will be dynamically indexed
            public string ProductType { get; set; }
            public int PricePerUnit { get; set; }
        }
        #endregion
        
        #region dynamic_fields_5
        public class Products_ByProductType : AbstractIndexCreationTask<Product>
        {
            public Products_ByProductType()
            {
                Map = products => from p in products
                    select new
                    {
                        // Call 'CreateField' to generate the dynamic-index-fields
                        // The field name will be the value of document field 'ProductType'
                        // The field terms will be derived from document field 'PricePerUnit'
                        _ = CreateField(p.ProductType, p.PricePerUnit)
                    };
            }
        }
        #endregion
        
        public class DynamicFields
        {
            public void QueryDynamicFields()
            {
                using var store = new DocumentStore();
                using var session = store.OpenSession();

                #region dynamic_fields_6
                IList<Product> matchingDocuments = session
                    .Advanced
                    .DocumentQuery<Product, Products_ByProductType>()
                     // 'Electronics' is the dynamic-index-field that was indexed from document field 'ProductType'
                    .WhereEquals("Electronics", 23)
                    .ToList();
                #endregion
            }
        }
    }
    
    namespace Example3
    {
        #region dynamic_fields_7
        public class Product
        {
            public string Id { get; set; }
            public string Name { get; set; }
            
            // For each element in this list, the VALUE of property 'PropName' will be dynamically indexed
            // e.g. Color, Width, Length (in ex. below) will become dynamic-index-fields
            public List<Attribute> Attributes { get; set; }
        }

        public class Attribute
        {
            public string PropName { get; set; }
            public string PropValue { get; set; }
        }
        #endregion
        
        #region dynamic_fields_8
        public class Attributes_ByName : AbstractIndexCreationTask<Product>
        {
            public Attributes_ByName()
            {
                Map = products => from a in products
                    select new
                    {
                        // Define the dynamic-index-fields by calling CreateField
                        // A dynamic-index-field will be generated for each item in the Attributes list
                        
                        // For each item, the field name will be the value of field 'PropName'
                        // The field terms will be derived from field 'PropValue'
                        _ = a.Attributes.Select(item => CreateField(item.PropName, item.PropValue)),
                        
                        // A regular index field can be defined as well:
                        Name = a.Name
                    };
            }
        }
        #endregion
    
        public class DynamicFields
        {
            public void QueryDynamicFields()
            {
                using var store = new DocumentStore();
                using var session = store.OpenSession();

                #region dynamic_fields_9
                IList<Product> matchingDocuments = session
                    .Advanced
                    .DocumentQuery<Product, Attributes_ByName>()
                     // 'Width' is a dynamic-index-field that was indexed from the Attributes list
                    .WhereEquals("Width", 10)
                    .ToList();
                #endregion
            }
        }
    }
}
