﻿namespace Omi.Fixml {
    using System.Linq;

    /// <summary>
    ///  Fixml Message
    /// </summary>

    public class Message : IParent {

        /// <summary>
        ///  Fixml message name
        /// </summary>
        public string Name = string.Empty;

        /// <summary>
        ///  Fixml message type
        /// </summary>
        public string Type = string.Empty;

        /// <summary>
        ///  Fixml message category
        /// </summary>
        public string Category = string.Empty;

        /// <summary>
        ///  Fixml message child elements list (fields, groups, components)
        /// </summary>
        public Elements Elements { get; set; } = new Elements();

        /// <summary>
        ///  Convert xml element to fixml message 
        /// </summary>
        public static Message From(Xml.fixMessage element) {
            var message = new Message() {
                Name = element.name,
                Type = element.msgtype,
                Category = element.msgcat,                     
            };

            message.Elements = Elements.From(element.Items, message); // need ??

            return message;                        
        }

        /// <summary>
        ///  Convert normalized specification messages to fixml messages 
        /// </summary>
        public static Message From(Fix.Specification.Message element) {
            var message = new Message() {
                Name = element.Name,
                Type = element.Type,
                Category = element.Category,
            };

            message.Elements = Elements.From(element.Fields, message);

            return message;
        }

        /// <summary>
        ///  Write fixml message to stream
        /// </summary>
        public void Write(StreamWriter stream, Indent indent) {
            if (HasFields) {
                stream.WriteLine($"{indent.Increment()}<message name=\"{Name}\" msgtype=\"{Type}\" msgcat=\"{Category}\">");

                Elements.Write(stream, indent.Increment().Increment());

                stream.WriteLine( $"{indent.Increment()}</message>");
            }
            else {
                stream.WriteLine($"{indent.Increment()}<message name=\"{Name}\" msgtype=\"{Type}\" msgcat=\"{Category}\"/>");
            }
        }

        /// <summary>
        ///  Does message have fields?
        /// </summary>
        public bool HasFields
            => Elements.Any();

        /// <summary>
        ///  Convert to normalized fix specification message
        /// </summary>
        public Fix.Specification.Message ToSpecification()
            => new () {
                Type = Type, 
                Name = Name, 
                Category = Category,
                Fields = Elements.ToSpecification()
            };

        /// <summary>
        ///  Verify fixml message properties
        /// </summary>
        public void Verify(Fields fields, Components components) {
            if (string.IsNullOrWhiteSpace(Name)) {
                throw new Exception("Message name is missing");
            }

            if (string.IsNullOrWhiteSpace(Type)) {
                throw new Exception("Message type is missing");
            }

            Elements.Verify(fields, components);
        }

        /// <summary>
        ///  Report erroneous fixml message properties
        /// </summary>
        public void Error(Fields fields, Components components, List<string> Errors)
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                Errors.Add("Message name is missing");
            }

            if (string.IsNullOrWhiteSpace(Type))
            {
                Errors.Add("Message type is missing");
            }

            var repeats = Elements.GroupBy(x => x.Name)
              .Where(g => g.Count() > 1)
              .Select(y => y.Key)
              .ToList();
            if ( repeats.Any() )
            {
                foreach ( var repeat in repeats )
                {
                    Errors.Add( $"{repeat} : Tag occurs more than once in message");
                }
            }
            

            Elements.Error(fields, components, Errors);
        }

        /// <summary>
        ///  Display fixml message as string
        /// </summary>
        public override string ToString() // need to update this for missing stuff
            => $"{Name}, {Type}, Fields: {Elements.Count}]";
    }
}