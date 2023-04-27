﻿namespace Omi.Fixml {
    using System.Collections.Generic;

    /// <summary>
    ///  Fixml list of child elements (fields, groups, components)
    /// </summary>

    public class Elements : List<IChild> {

        /// <summary>
        ///  Default constructor
        /// </summary>
        public Elements()
        { }

        /// <summary>
        /// Construct elements from an IEnumerable
        /// </summary>
        public Elements(IEnumerable<IChild> fields) { 
            AddRange(fields);
        }

        /// <summary>
        ///  Gather elements
        /// </summary>
        public static Elements From(object[] items, IParent parent) {
            var elements = new Elements();

            foreach (var item in items ?? Array.Empty<object>()) {
                elements.Add(Child.Field.From(item, parent));
            }

            return elements;
        }

        /// <summary>
        ///  Convert normalized specification fields to fixml elements
        /// </summary>
        public static Elements From(List<Fix.Specification.Field> fields, IParent parent) {
            var elements = new Elements();

            foreach (var element in fields) {
                // Check values?
                elements.Add(Child.Field.From(element, parent));
            }

            return elements;
        }

        /// <summary>
        ///  Write fixml elements
        /// </summary>
        public void Write(StreamWriter stream, int spaces = 0) {
            foreach (var element in this) {
                element.Write(stream, spaces);
            }
        }

        /// <summary>
        ///  Convert fixml elements to normalized fix specification fields 
        /// </summary>
        public List<Fix.Specification.Field> ToSpecification() {
            var enums = new List<Fix.Specification.Field>();

            foreach (var component in this) {
                enums.Add(component.ToSpecification());
            }
            
            return enums;
        }

        /// <summary>
        ///  Verify fixml elements
        /// </summary>
        public void Verify(Fields fields, Components components) {
            foreach (var element in this) {
                element.Verify(fields, components);
            }
        }
    }
}