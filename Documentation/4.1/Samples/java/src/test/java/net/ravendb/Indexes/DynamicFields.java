package net.ravendb.Indexes;

import net.ravendb.client.documents.DocumentStore;
import net.ravendb.client.documents.IDocumentStore;
import net.ravendb.client.documents.indexes.AbstractIndexCreationTask;
import net.ravendb.client.documents.session.IDocumentSession;

import java.util.List;

public class DynamicFields {

    //region dynamic_fields_1

    public static class Product {
        private String id;
        private List<Attribute> attributes;

        public String getId() {
            return id;
        }

        public void setId(String id) {
            this.id = id;
        }

        public List<Attribute> getAttributes() {
            return attributes;
        }

        public void setAttributes(List<Attribute> attributes) {
            this.attributes = attributes;
        }
    }

    public static class Attribute {
        private String name;
        private String value;

        public String getName() {
            return name;
        }

        public void setName(String name) {
            this.name = name;
        }

        public String getValue() {
            return value;
        }

        public void setValue(String value) {
            this.value = value;
        }
    }
    //endregion

    //region dynamic_fields_2
    public static class Products_ByAttribute extends AbstractIndexCreationTask {
        public static class Result {
            private String color;
            private String size;

            public String getColor() {
                return color;
            }

            public void setColor(String color) {
                this.color = color;
            }

            public String getSize() {
                return size;
            }

            public void setSize(String size) {
                this.size = size;
            }
        }

        public Products_ByAttribute() {
            map = "docs.Products.Select(p => new { " +
                "    _ = p.attributes.Select(attribute => this.CreateField(attribute.name, attribute.value, false, true)) " +
                "})";
        }
    }
    //endregion

    public DynamicFields() {
        try (IDocumentStore store = new DocumentStore()) {
            try (IDocumentSession session = store.openSession()) {
                //region dynamic_fields_4
                List<Product> results = session
                    .query(Products_ByAttribute.Result.class, Products_ByAttribute.class)
                    .whereEquals("color", "red")
                    .ofType(Product.class)
                    .toList();
                //endregion
            }
        }
    }
}
