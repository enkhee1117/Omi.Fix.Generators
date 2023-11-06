namespace Omi.Fix.Txt {
    using System.IO;

    /// <summary>
    ///  Omi Fix Text C# Document
    /// </summary>

    public class Document {

        /// <summary>
        ///  Fix txt documention information
        /// </summary>
        public Information Information = new();

        /// <summary>
        ///  Fix txt fields
        /// </summary>
        public Fields Fields = new();

        /// <summary>
        ///  Fix txt enum values
        /// </summary>
        public Enums Enums = new();

        /// <summary>
        ///  Fix txt messages
        /// </summary>
        public Messages Messages = new();

        /// <summary>
        ///  Fix Txt document from path
        /// </summary>
        public static Document From(string path) {
            var lines = File.ReadLines(path);
            return From(lines);
        }

        /// <summary>
        ///  Fix Txt document from records
        /// </summary>
        public static Document From(IEnumerable<string> lines)
            => new () { // need to parse these in order 
                Information = Information.From(lines),
                Enums = Enums.From(lines),
                Fields = Fields.From(lines),
                Messages = Messages.From(lines)
            };

        /// <summary>
        ///  Convert fix txt to normalized fix specification
        /// </summary>
        public Specification.Document ToSpecification()
            => new () {
                Information = Information.ToSpecification(),
                Header = new Specification.Header(),
                Trailer = new Specification.Trailer(),
                Messages = Messages.ToSpecification(),
                Components = new Specification.Components(),
                Types = Fields.ToSpecification(Enums),
            };

        /// <summary>
        ///  Fix Txt description as string
        /// </summary>
        public override string ToString()
            => $"{Information} fix txt";
    }
}