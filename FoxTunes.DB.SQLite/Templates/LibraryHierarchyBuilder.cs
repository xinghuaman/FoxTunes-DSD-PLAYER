﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 15.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace FoxTunes.Templates
{
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "C:\Source\FoxTunes\FoxTunes.DB.SQLite\Templates\LibraryHierarchyBuilder.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "15.0.0.0")]
    public partial class LibraryHierarchyBuilder : LibraryHierarchyBuilderBase
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public virtual string TransformText()
        {
            this.Write("\r\nCREATE TEMPORARY TABLE IF NOT EXISTS \"LibraryHierarchyLevelLeaf\"\r\n(\r\n\t\"LibraryH" +
                    "ierarchy_Id\" INTEGER NOT NULL, \r\n\t\"LibraryHierarchyLevel_Id\" INTEGER NOT NULL\r\n)" +
                    ";\r\nCREATE UNIQUE INDEX IF NOT EXISTS \"IDX_LibraryHierarchyLevelLeaf\" ON \"Library" +
                    "HierarchyLevelLeaf\"\r\n(\r\n\t\"LibraryHierarchy_Id\"\r\n);\r\n\r\nDELETE FROM \"LibraryHierar" +
                    "chyLevelLeaf\";\r\n\r\nINSERT INTO \"LibraryHierarchyLevelLeaf\"\r\nSELECT LibraryHierarc" +
                    "hy_Id, \"Id\"\r\nFROM \"LibraryHierarchyLevels\"\r\nGROUP BY \"LibraryHierarchy_Id\"\r\nHAVI" +
                    "NG MAX(\"Sequence\")\r\nORDER BY \"Sequence\";\r\n\r\nCREATE TEMPORARY TABLE IF NOT EXISTS" +
                    " \"LibraryHierarchyLevelParent\"\r\n(\r\n\t\"Id\" INTEGER NOT NULL, \r\n\t\"Parent_Id\" INTEGE" +
                    "R NOT NULL\r\n);\r\nCREATE UNIQUE INDEX IF NOT EXISTS \"IDX_LibraryHierarchyLevelPare" +
                    "nt\" ON \"LibraryHierarchyLevelParent\"\r\n(\r\n\t\"Id\"\r\n);\r\n\r\nDELETE FROM \"LibraryHierar" +
                    "chyLevelParent\";\r\n\r\nINSERT INTO \"LibraryHierarchyLevelParent\"\r\nSELECT \"LibraryHi" +
                    "erarchyLevels\".\"Id\" AS \"Id\", \"LibraryHierarchyLevels_Copy\".\"Id\" AS \"Parent_Id\"\r\n" +
                    "FROM \"LibraryHierarchyLevels\"\r\nJOIN \"LibraryHierarchyLevels\" AS \"LibraryHierarch" +
                    "yLevels_Copy\"\r\n\tON \"LibraryHierarchyLevels\".\"LibraryHierarchy_Id\"  = \"LibraryHie" +
                    "rarchyLevels_Copy\".\"LibraryHierarchy_Id\"\r\n\t\tAND \"LibraryHierarchyLevels_Copy\".\"S" +
                    "equence\" < \"LibraryHierarchyLevels\".\"Sequence\"\r\nGROUP BY \"LibraryHierarchyLevels" +
                    "\".\"Id\"\r\nORDER BY \"LibraryHierarchyLevels_Copy\".\"Sequence\";\r\n\r\nCREATE TEMPORARY T" +
                    "ABLE IF NOT EXISTS \"LibraryHierarchy\"\r\n(\r\n\t\"LibraryHierarchy_Id\" INTEGER NOT NUL" +
                    "L, \r\n\t\"LibraryHierarchyLevel_Id\" INTEGER NOT NULL, \r\n\t\"LibraryItem_Id\" INTEGER N" +
                    "OT NULL, \r\n\t\"DisplayValue\" text NOT NULL,\r\n\t\"SortValue\" text NOT NULL,\r\n\t\"IsLeaf" +
                    "\" bit NOT NULL\r\n);\r\nCREATE UNIQUE INDEX IF NOT EXISTS \"IDX_LibraryHierarchy\" ON " +
                    "\"LibraryHierarchy\"\r\n(\r\n\t\"LibraryHierarchy_Id\",\r\n\t\"LibraryHierarchyLevel_Id\",\r\n\t\"" +
                    "LibraryItem_Id\",\r\n\t\"DisplayValue\",\r\n\t\"SortValue\",\r\n\t\"IsLeaf\"\r\n);\r\nCREATE INDEX I" +
                    "F NOT EXISTS \"IDX_LibraryHierarchy_LibraryItem\" ON \"LibraryHierarchy\"\r\n(\r\n\t\"Libr" +
                    "aryItem_Id\"\r\n);\r\n\r\nDELETE FROM \"LibraryHierarchy\";\r\n\r\nWITH \"VerticalMetaData\"\r\nA" +
                    "S\r\n(\r\n\tSELECT \"LibraryItems\".\"Id\", \"LibraryItems\".\"FileName\", \"MetaDataItems\".\"N" +
                    "ame\", \r\n\t\tCASE \r\n\t\t\tWHEN \"MetaDataItems\".\"NumericValue\" IS NOT NULL THEN \'Numeri" +
                    "c\' \r\n\t\t\tWHEN \"MetaDataItems\".\"TextValue\" IS NOT NULL THEN \'Text\' \r\n\t\t\tWHEN \"Meta" +
                    "DataItems\".\"FileValue\" IS NOT NULL THEN \'File\' \r\n\t\tEND AS \"ValueType\",\r\n\t\t\tCASE " +
                    "\r\n\t\t\tWHEN \"MetaDataItems\".\"NumericValue\" IS NOT NULL THEN \"MetaDataItems\".\"Numer" +
                    "icValue\"\r\n\t\t\tWHEN \"MetaDataItems\".\"TextValue\" IS NOT NULL THEN \"MetaDataItems\".\"" +
                    "TextValue\" \r\n\t\t\tWHEN \"MetaDataItems\".\"FileValue\" IS NOT NULL THEN \"MetaDataItems" +
                    "\".\"FileValue\"\r\n\t\tEND AS \"Value\"\r\n\tFROM \"LibraryItems\"\r\n\t\tJOIN \"LibraryItem_MetaD" +
                    "ataItem\" ON \"LibraryItems\".\"Id\" = \"LibraryItem_MetaDataItem\".\"LibraryItem_Id\"\r\n\t" +
                    "\tJOIN \"MetaDataItems\" ON \"MetaDataItems\".\"Id\" = \"LibraryItem_MetaDataItem\".\"Meta" +
                    "DataItem_Id\"\r\n\tORDER BY \"LibraryItems\".\"Id\"\r\n)\r\n,\r\n\"HorizontalMetaData\"\r\nAS\r\n(\r\n" +
                    "");
            
            #line 95 "C:\Source\FoxTunes\FoxTunes.DB.SQLite\Templates\LibraryHierarchyBuilder.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(new PivotViewBuilder(
		this.Database,
		"VerticalMetaData", 
		new[] { "Id", "FileName" }, 
		new[] { "Name" }, 
		new[] { "ValueType", "Value" }, 
		this.MetaDataNames
	).TransformText()));
            
            #line default
            #line hidden
            this.Write(@"
)

SELECT ""LibraryHierarchyLevels"".""LibraryHierarchy_Id"" AS ""LibraryHierarchy_Id"", ""LibraryHierarchyLevels"".""Id"" AS ""LibraryHierarchyLevel_Id"", ""HorizontalMetaData"".""Id"" AS ""LibraryItem_Id"", ""HorizontalMetaData"".""FileName"", ""LibraryHierarchyLevels"".""DisplayScript"", ""LibraryHierarchyLevels"".""SortScript""
");
            
            #line 108 "C:\Source\FoxTunes\FoxTunes.DB.SQLite\Templates\LibraryHierarchyBuilder.tt"

	for(var index = 0; index < this.MetaDataNames.Length; index++)
	{
		
            
            #line default
            #line hidden
            this.Write(",\"Key_");
            
            #line 111 "C:\Source\FoxTunes\FoxTunes.DB.SQLite\Templates\LibraryHierarchyBuilder.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(index));
            
            #line default
            #line hidden
            this.Write("\", \"Value_");
            
            #line 111 "C:\Source\FoxTunes\FoxTunes.DB.SQLite\Templates\LibraryHierarchyBuilder.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(index));
            
            #line default
            #line hidden
            this.Write("_Value\"");
            
            #line 111 "C:\Source\FoxTunes\FoxTunes.DB.SQLite\Templates\LibraryHierarchyBuilder.tt"

	}

            
            #line default
            #line hidden
            this.Write("\t, \"LibraryHierarchyLevels\".\"Id\" = \"LibraryHierarchyLevelLeaf\".\"LibraryHierarchyL" +
                    "evel_Id\" AS \"IsLeaf\"\r\nFROM \"LibraryHierarchyLevels\"\r\n\tJOIN \"LibraryHierarchyLeve" +
                    "lLeaf\" \r\n\t\tON \"LibraryHierarchyLevelLeaf\".\"LibraryHierarchy_Id\" = \"LibraryHierar" +
                    "chyLevels\".\"LibraryHierarchy_Id\" \r\n\tJOIN \"HorizontalMetaData\";\r\n\r\nDELETE FROM \"L" +
                    "ibraryHierarchyItems\";\r\n\r\nINSERT INTO \"LibraryHierarchyItems\" (\"LibraryHierarchy" +
                    "_Id\", \"LibraryHierarchyLevel_Id\", \"DisplayValue\", \"SortValue\", \"IsLeaf\")\r\nSELECT" +
                    " \"LibraryHierarchy_Id\", \"LibraryHierarchyLevel_Id\", \"DisplayValue\", \"SortValue\"," +
                    " \"IsLeaf\"\r\nFROM \"LibraryHierarchy\"\r\nGROUP BY \"LibraryHierarchy_Id\", \"LibraryHier" +
                    "archyLevel_Id\", \"DisplayValue\", \"SortValue\", \"IsLeaf\";\r\n\r\nUPDATE \"LibraryHierarc" +
                    "hyItems\"\r\nSET \"Parent_Id\" = \r\n(\r\n\tSELECT \"LibraryHierarchyItems_Copy\".\"Id\"\r\n\tFRO" +
                    "M \"LibraryHierarchyItems\" AS \"LibraryHierarchyItems_Copy\"\r\n\t\tJOIN \"LibraryHierar" +
                    "chy\" \r\n\t\t\tON \"LibraryHierarchy\".\"LibraryHierarchy_Id\" = \"LibraryHierarchyItems\"." +
                    "\"LibraryHierarchy_Id\"\r\n\t\t\t\tAND \"LibraryHierarchy\".\"LibraryHierarchyLevel_Id\" = \"" +
                    "LibraryHierarchyItems\".\"LibraryHierarchyLevel_Id\"\r\n\t\t\t\tAND \"LibraryHierarchy\".\"D" +
                    "isplayValue\" = \"LibraryHierarchyItems\".\"DisplayValue\"\r\n\t\t\t\tAND \"LibraryHierarchy" +
                    "\".\"SortValue\" = \"LibraryHierarchyItems\".\"SortValue\"\r\n\t\t\t\tAND \"LibraryHierarchy\"." +
                    "\"IsLeaf\" = \"LibraryHierarchyItems\".\"IsLeaf\"\r\n\tJOIN \"LibraryHierarchyLevelParent\"" +
                    " \r\n\t\tON \"LibraryHierarchyLevelParent\".\"Id\" = \"LibraryHierarchyItems\".\"LibraryHie" +
                    "rarchyLevel_Id\"\r\n\tJOIN \"LibraryHierarchy\" AS \"LibraryHierarchy_Copy\" \r\n\t\tON \"Lib" +
                    "raryHierarchy_Copy\".\"LibraryHierarchy_Id\" = \"LibraryHierarchyItems_Copy\".\"Librar" +
                    "yHierarchy_Id\"\r\n\t\t\tAND \"LibraryHierarchy_Copy\".\"LibraryHierarchyLevel_Id\" = \"Lib" +
                    "raryHierarchyLevelParent\".\"Parent_Id\"\r\n\t\t\tAND \"LibraryHierarchy_Copy\".\"LibraryIt" +
                    "em_Id\" = \"LibraryHierarchy\".\"LibraryItem_Id\"\r\n\t\t\tAND \"LibraryHierarchy_Copy\".\"Di" +
                    "splayValue\" = \"LibraryHierarchyItems_Copy\".\"DisplayValue\"\r\n\t\t\tAND \"LibraryHierar" +
                    "chy_Copy\".\"SortValue\" = \"LibraryHierarchyItems_Copy\".\"SortValue\"\r\n\t\t\tAND \"Librar" +
                    "yHierarchy_Copy\".\"IsLeaf\" = \"LibraryHierarchyItems_Copy\".\"IsLeaf\"\r\n\tLIMIT 1\r\n);\r" +
                    "\n\r\nDELETE FROM \"LibraryHierarchyItem_LibraryItem\";\r\n\r\nINSERT INTO \"LibraryHierar" +
                    "chyItem_LibraryItem\" (\"LibraryHierarchyItem_Id\", \"LibraryItem_Id\")\r\nSELECT \"Libr" +
                    "aryHierarchyItems\".\"Id\", \"LibraryHierarchy\".\"LibraryItem_Id\"\r\nFROM \"LibraryHiera" +
                    "rchyItems\"\r\n\tJOIN \"LibraryHierarchy\" ON \"LibraryHierarchy\".\"LibraryHierarchy_Id\"" +
                    " =\"LibraryHierarchyItems\".\"LibraryHierarchy_Id\"\r\n\t\tAND \"LibraryHierarchy\".\"Libra" +
                    "ryHierarchyLevel_Id\" = \"LibraryHierarchyItems\".\"LibraryHierarchyLevel_Id\"\r\n\t\tAND" +
                    " \"LibraryHierarchy\".\"DisplayValue\" = \"LibraryHierarchyItems\".\"DisplayValue\"\r\n\t\tA" +
                    "ND \"LibraryHierarchy\".\"SortValue\" = \"LibraryHierarchyItems\".\"SortValue\"\r\n\t\tAND \"" +
                    "LibraryHierarchy\".\"IsLeaf\" = \"LibraryHierarchyItems\".\"IsLeaf\";");
            return this.GenerationEnvironment.ToString();
        }
    }
    
    #line default
    #line hidden
    #region Base class
    /// <summary>
    /// Base class for this transformation
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "15.0.0.0")]
    public class LibraryHierarchyBuilderBase
    {
        #region Fields
        private global::System.Text.StringBuilder generationEnvironmentField;
        private global::System.CodeDom.Compiler.CompilerErrorCollection errorsField;
        private global::System.Collections.Generic.List<int> indentLengthsField;
        private string currentIndentField = "";
        private bool endsWithNewline;
        private global::System.Collections.Generic.IDictionary<string, object> sessionField;
        #endregion
        #region Properties
        /// <summary>
        /// The string builder that generation-time code is using to assemble generated output
        /// </summary>
        protected System.Text.StringBuilder GenerationEnvironment
        {
            get
            {
                if ((this.generationEnvironmentField == null))
                {
                    this.generationEnvironmentField = new global::System.Text.StringBuilder();
                }
                return this.generationEnvironmentField;
            }
            set
            {
                this.generationEnvironmentField = value;
            }
        }
        /// <summary>
        /// The error collection for the generation process
        /// </summary>
        public System.CodeDom.Compiler.CompilerErrorCollection Errors
        {
            get
            {
                if ((this.errorsField == null))
                {
                    this.errorsField = new global::System.CodeDom.Compiler.CompilerErrorCollection();
                }
                return this.errorsField;
            }
        }
        /// <summary>
        /// A list of the lengths of each indent that was added with PushIndent
        /// </summary>
        private System.Collections.Generic.List<int> indentLengths
        {
            get
            {
                if ((this.indentLengthsField == null))
                {
                    this.indentLengthsField = new global::System.Collections.Generic.List<int>();
                }
                return this.indentLengthsField;
            }
        }
        /// <summary>
        /// Gets the current indent we use when adding lines to the output
        /// </summary>
        public string CurrentIndent
        {
            get
            {
                return this.currentIndentField;
            }
        }
        /// <summary>
        /// Current transformation session
        /// </summary>
        public virtual global::System.Collections.Generic.IDictionary<string, object> Session
        {
            get
            {
                return this.sessionField;
            }
            set
            {
                this.sessionField = value;
            }
        }
        #endregion
        #region Transform-time helpers
        /// <summary>
        /// Write text directly into the generated output
        /// </summary>
        public void Write(string textToAppend)
        {
            if (string.IsNullOrEmpty(textToAppend))
            {
                return;
            }
            // If we're starting off, or if the previous text ended with a newline,
            // we have to append the current indent first.
            if (((this.GenerationEnvironment.Length == 0) 
                        || this.endsWithNewline))
            {
                this.GenerationEnvironment.Append(this.currentIndentField);
                this.endsWithNewline = false;
            }
            // Check if the current text ends with a newline
            if (textToAppend.EndsWith(global::System.Environment.NewLine, global::System.StringComparison.CurrentCulture))
            {
                this.endsWithNewline = true;
            }
            // This is an optimization. If the current indent is "", then we don't have to do any
            // of the more complex stuff further down.
            if ((this.currentIndentField.Length == 0))
            {
                this.GenerationEnvironment.Append(textToAppend);
                return;
            }
            // Everywhere there is a newline in the text, add an indent after it
            textToAppend = textToAppend.Replace(global::System.Environment.NewLine, (global::System.Environment.NewLine + this.currentIndentField));
            // If the text ends with a newline, then we should strip off the indent added at the very end
            // because the appropriate indent will be added when the next time Write() is called
            if (this.endsWithNewline)
            {
                this.GenerationEnvironment.Append(textToAppend, 0, (textToAppend.Length - this.currentIndentField.Length));
            }
            else
            {
                this.GenerationEnvironment.Append(textToAppend);
            }
        }
        /// <summary>
        /// Write text directly into the generated output
        /// </summary>
        public void WriteLine(string textToAppend)
        {
            this.Write(textToAppend);
            this.GenerationEnvironment.AppendLine();
            this.endsWithNewline = true;
        }
        /// <summary>
        /// Write formatted text directly into the generated output
        /// </summary>
        public void Write(string format, params object[] args)
        {
            this.Write(string.Format(global::System.Globalization.CultureInfo.CurrentCulture, format, args));
        }
        /// <summary>
        /// Write formatted text directly into the generated output
        /// </summary>
        public void WriteLine(string format, params object[] args)
        {
            this.WriteLine(string.Format(global::System.Globalization.CultureInfo.CurrentCulture, format, args));
        }
        /// <summary>
        /// Raise an error
        /// </summary>
        public void Error(string message)
        {
            System.CodeDom.Compiler.CompilerError error = new global::System.CodeDom.Compiler.CompilerError();
            error.ErrorText = message;
            this.Errors.Add(error);
        }
        /// <summary>
        /// Raise a warning
        /// </summary>
        public void Warning(string message)
        {
            System.CodeDom.Compiler.CompilerError error = new global::System.CodeDom.Compiler.CompilerError();
            error.ErrorText = message;
            error.IsWarning = true;
            this.Errors.Add(error);
        }
        /// <summary>
        /// Increase the indent
        /// </summary>
        public void PushIndent(string indent)
        {
            if ((indent == null))
            {
                throw new global::System.ArgumentNullException("indent");
            }
            this.currentIndentField = (this.currentIndentField + indent);
            this.indentLengths.Add(indent.Length);
        }
        /// <summary>
        /// Remove the last indent that was added with PushIndent
        /// </summary>
        public string PopIndent()
        {
            string returnValue = "";
            if ((this.indentLengths.Count > 0))
            {
                int indentLength = this.indentLengths[(this.indentLengths.Count - 1)];
                this.indentLengths.RemoveAt((this.indentLengths.Count - 1));
                if ((indentLength > 0))
                {
                    returnValue = this.currentIndentField.Substring((this.currentIndentField.Length - indentLength));
                    this.currentIndentField = this.currentIndentField.Remove((this.currentIndentField.Length - indentLength));
                }
            }
            return returnValue;
        }
        /// <summary>
        /// Remove any indentation
        /// </summary>
        public void ClearIndent()
        {
            this.indentLengths.Clear();
            this.currentIndentField = "";
        }
        #endregion
        #region ToString Helpers
        /// <summary>
        /// Utility class to produce culture-oriented representation of an object as a string.
        /// </summary>
        public class ToStringInstanceHelper
        {
            private System.IFormatProvider formatProviderField  = global::System.Globalization.CultureInfo.InvariantCulture;
            /// <summary>
            /// Gets or sets format provider to be used by ToStringWithCulture method.
            /// </summary>
            public System.IFormatProvider FormatProvider
            {
                get
                {
                    return this.formatProviderField ;
                }
                set
                {
                    if ((value != null))
                    {
                        this.formatProviderField  = value;
                    }
                }
            }
            /// <summary>
            /// This is called from the compile/run appdomain to convert objects within an expression block to a string
            /// </summary>
            public string ToStringWithCulture(object objectToConvert)
            {
                if ((objectToConvert == null))
                {
                    throw new global::System.ArgumentNullException("objectToConvert");
                }
                System.Type t = objectToConvert.GetType();
                System.Reflection.MethodInfo method = t.GetMethod("ToString", new System.Type[] {
                            typeof(System.IFormatProvider)});
                if ((method == null))
                {
                    return objectToConvert.ToString();
                }
                else
                {
                    return ((string)(method.Invoke(objectToConvert, new object[] {
                                this.formatProviderField })));
                }
            }
        }
        private ToStringInstanceHelper toStringHelperField = new ToStringInstanceHelper();
        /// <summary>
        /// Helper to produce culture-oriented representation of an object as a string
        /// </summary>
        public ToStringInstanceHelper ToStringHelper
        {
            get
            {
                return this.toStringHelperField;
            }
        }
        #endregion
    }
    #endregion
}
