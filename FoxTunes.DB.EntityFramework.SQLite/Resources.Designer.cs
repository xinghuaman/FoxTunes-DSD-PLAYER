﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FoxTunes {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("FoxTunes.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to WITH 
        ///&quot;ImageItems_Lookup&quot; AS
        ///(
        ///	SELECT *
        ///	FROM &quot;ImageItems&quot; 
        ///	WHERE &quot;FileName&quot; = @fileName AND &quot;ImageType&quot; = @imageType 
        ///)
        ///
        ///INSERT INTO &quot;ImageItems&quot; (&quot;FileName&quot;, &quot;ImageType&quot;) 
        ///SELECT @fileName, @imageType
        ///WHERE NOT EXISTS(SELECT * FROM &quot;ImageItems_Lookup&quot;);
        ///
        ///WITH 
        ///&quot;ImageItems_Lookup&quot; AS
        ///(
        ///	SELECT *
        ///	FROM &quot;ImageItems&quot; 
        ///	WHERE &quot;FileName&quot; = @fileName AND &quot;ImageType&quot; = @imageType 
        ///),
        ///
        ///&quot;{0}Item_ImageItem_Lookup&quot; AS 
        ///(
        ///	SELECT &quot;{0}Item_ImageItem&quot;.*
        ///	FROM &quot;{0}Item_ImageItem&quot;
        ///		JOIN &quot;ImageIt [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string AddImageItems {
            get {
                return ResourceManager.GetString("AddImageItems", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to WITH &quot;OrderedLibraryItems&quot;
        ///AS
        ///(
        ///	SELECT  &quot;LibraryItems&quot;.&quot;Id&quot;, &quot;LibraryItems&quot;.&quot;DirectoryName&quot;, &quot;LibraryItems&quot;.&quot;FileName&quot;, &quot;LibraryHierarchyItems&quot;.&quot;LibraryHierarchy_Id&quot;, &quot;LibraryHierarchyItems&quot;.&quot;SortValue&quot;, &quot;LibraryHierarchyItems&quot;.&quot;DisplayValue&quot;
        ///	FROM  &quot;LibraryItems&quot;
        ///		JOIN &quot;LibraryHierarchyItem_LibraryItem&quot; ON &quot;LibraryItems&quot;.&quot;Id&quot; = &quot;LibraryHierarchyItem_LibraryItem&quot;.&quot;LibraryItem_Id&quot;
        ///		JOIN &quot;LibraryHierarchyItems&quot; ON &quot;LibraryHierarchyItem_LibraryItem&quot;.&quot;LibraryHierarchyItem_Id&quot; = &quot;LibraryHierarchyItems&quot;. [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string AddLibraryHierarchyNodeToPlaylist {
            get {
                return ResourceManager.GetString("AddLibraryHierarchyNodeToPlaylist", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to INSERT INTO &quot;LibraryHierarchy&quot; (&quot;LibraryHierarchy_Id&quot;, &quot;LibraryHierarchyLevel_Id&quot;, &quot;LibraryItem_Id&quot;, &quot;DisplayValue&quot;, &quot;SortValue&quot;, &quot;IsLeaf&quot;)
        ///VALUES (@libraryHierarchyId, @libraryHierarchyLevelId, @libraryItemId, @displayValue, @sortValue, @isLeaf).
        /// </summary>
        internal static string AddLibraryHierarchyRecord {
            get {
                return ResourceManager.GetString("AddLibraryHierarchyRecord", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to INSERT INTO &quot;LibraryItems&quot; (&quot;DirectoryName&quot;, &quot;FileName&quot;, &quot;Status&quot;) 
        ///SELECT @directoryName, @fileName, @status
        ///WHERE NOT EXISTS(SELECT * FROM &quot;LibraryItems&quot; WHERE &quot;FileName&quot; = @fileName).
        /// </summary>
        internal static string AddLibraryItem {
            get {
                return ResourceManager.GetString("AddLibraryItem", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to WITH 
        ///&quot;MetaDataItems_Lookup&quot; AS
        ///(
        ///	SELECT *
        ///	FROM &quot;MetaDataItems&quot; 
        ///	WHERE &quot;Name&quot; = @name AND &quot;Type&quot; = @type
        ///		AND ((&quot;NumericValue&quot; IS NULL AND @numericValue IS NULL) OR &quot;NumericValue&quot; = @numericValue)
        ///		AND ((&quot;TextValue&quot; IS NULL AND @textValue IS NULL) OR &quot;TextValue&quot; = @textValue) 
        ///		AND ((&quot;FileValue&quot; IS NULL AND @fileValue IS NULL) OR &quot;FileValue&quot; = @fileValue)
        ///)
        ///
        ///INSERT INTO &quot;MetaDataItems&quot; (&quot;Name&quot;, &quot;Type&quot;, &quot;NumericValue&quot;, &quot;TextValue&quot;, &quot;FileValue&quot;) 
        ///SELECT @name, @type, @numericValue, @textValu [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string AddMetaDataItems {
            get {
                return ResourceManager.GetString("AddMetaDataItems", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to INSERT INTO &quot;PlaylistItems&quot; (&quot;Sequence&quot;, &quot;DirectoryName&quot;, &quot;FileName&quot;, &quot;Status&quot;) 
        ///SELECT @sequence, @directoryName, @fileName, @status.
        /// </summary>
        internal static string AddPlaylistItem {
            get {
                return ResourceManager.GetString("AddPlaylistItem", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to WITH 
        ///&quot;PropertyItems_Lookup&quot; AS
        ///(
        ///	SELECT *
        ///	FROM &quot;PropertyItems&quot; 
        ///	WHERE &quot;Name&quot; = @name 
        ///		AND ((&quot;NumericValue&quot; IS NULL AND @numericValue IS NULL) OR &quot;NumericValue&quot; = @numericValue)
        ///		AND ((&quot;TextValue&quot; IS NULL AND @textValue IS NULL) OR &quot;TextValue&quot; = @textValue) 
        ///)
        ///
        ///INSERT INTO &quot;PropertyItems&quot; (&quot;Name&quot;, &quot;NumericValue&quot;, &quot;TextValue&quot;) 
        ///SELECT @name, @numericValue, @textValue
        ///WHERE NOT EXISTS(SELECT * FROM &quot;PropertyItems_Lookup&quot;);
        ///
        ///WITH 
        ///&quot;PropertyItems_Lookup&quot; AS
        ///(
        ///	SELECT *
        ///	FROM &quot;PropertyIte [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string AddPropertyItems {
            get {
                return ResourceManager.GetString("AddPropertyItems", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to DELETE FROM &quot;PlaylistItems&quot;;
        ///DELETE FROM &quot;PlaylistItem_MetaDataItem&quot;;.
        /// </summary>
        internal static string ClearPlaylist {
            get {
                return ResourceManager.GetString("ClearPlaylist", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to INSERT INTO &quot;PlaylistItem_MetaDataItem&quot; (&quot;PlaylistItem_Id&quot;, &quot;MetaDataItem_Id&quot;)
        ///SELECT &quot;PlaylistItems&quot;.&quot;Id&quot;, &quot;LibraryItem_MetaDataItem&quot;.&quot;MetaDataItem_Id&quot;
        ///FROM &quot;PlaylistItems&quot;
        ///	JOIN &quot;LibraryItems&quot; 
        ///		ON &quot;PlaylistItems&quot;.&quot;FileName&quot; = &quot;LibraryItems&quot;.&quot;FileName&quot;
        ///	JOIN &quot;LibraryItem_MetaDataItem&quot; 
        ///		ON &quot;LibraryItems&quot;.&quot;Id&quot; = &quot;LibraryItem_MetaDataItem&quot;.&quot;LibraryItem_Id&quot;
        ///WHERE &quot;PlaylistItems&quot;.&quot;Status&quot; = @status;.
        /// </summary>
        internal static string CopyMetaDataItems {
            get {
                return ResourceManager.GetString("CopyMetaDataItems", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to BEGIN TRANSACTION;
        ///CREATE TABLE [PlaylistItems](
        ///    [Id] INTEGER CONSTRAINT [sqlite_master_PK_PlaylistItems] PRIMARY KEY NOT NULL, 
        ///    [Sequence] bigint NOT NULL, 
        ///    [DirectoryName] text NOT NULL, 
        ///    [FileName] text NOT NULL, 
        ///    [Status] bigint NOT NULL);
        ///CREATE TABLE [PlaylistItem_MetaDataItem](
        ///    [Id] INTEGER PRIMARY KEY NOT NULL, 
        ///    [PlaylistItem_Id] INTEGER NOT NULL REFERENCES PlaylistItems([Id]) ON DELETE CASCADE, 
        ///    [MetaDataItem_Id] INTEGER NOT NULL REFERENCES MetaDataItems([Id]) ON DEL [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Database {
            get {
                return ResourceManager.GetString("Database", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT &quot;MetaDataItems&quot;.&quot;Name&quot;, &quot;MetaDataItems&quot;.&quot;Type&quot;, &quot;MetaDataItems&quot;.&quot;NumericValue&quot;,  &quot;MetaDataItems&quot;.&quot;TextValue&quot;, &quot;MetaDataItems&quot;.&quot;FileValue&quot;
        ///FROM &quot;LibraryHierarchyItems&quot;
        ///	JOIN &quot;LibraryHierarchyItem_LibraryItem&quot; ON &quot;LibraryHierarchyItems&quot;.&quot;Id&quot; = &quot;LibraryHierarchyItem_LibraryItem&quot;.&quot;LibraryHierarchyItem_Id&quot;
        ///	JOIN &quot;LibraryItem_MetaDataItem&quot; ON &quot;LibraryHierarchyItem_LibraryItem&quot;.&quot;LibraryItem_Id&quot; = &quot;LibraryItem_MetaDataItem&quot;.&quot;LibraryItem_Id&quot;
        ///	JOIN &quot;MetaDataItems&quot; ON &quot;MetaDataItems&quot;.&quot;Id&quot; = &quot;LibraryItem_Met [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string GetLibraryHierarchyMetaDataItems {
            get {
                return ResourceManager.GetString("GetLibraryHierarchyMetaDataItems", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT &quot;Id&quot;, &quot;LibraryHierarchy_Id&quot;, &quot;DisplayValue&quot;, &quot;IsLeaf&quot;
        ///FROM &quot;LibraryHierarchyItems&quot;
        ///WHERE &quot;LibraryHierarchy_Id&quot; = @libraryHierarchyId
        ///	AND ((@libraryHierarchyItemId IS NULL AND &quot;LibraryHierarchyItems&quot;.&quot;Parent_Id&quot; IS NULL) OR &quot;LibraryHierarchyItems&quot;.&quot;Parent_Id&quot; = @libraryHierarchyItemId)
        ///ORDER BY &quot;SortValue&quot;, &quot;DisplayValue&quot;.
        /// </summary>
        internal static string GetLibraryHierarchyNodes {
            get {
                return ResourceManager.GetString("GetLibraryHierarchyNodes", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to WITH RECURSIVE 
        ///
        ///LibraryHierarchyParents(&quot;Root&quot;, &quot;Id&quot;, &quot;Parent_Id&quot;, &quot;DisplayValue&quot;)
        ///AS
        ///(
        ///	SELECT &quot;Id&quot;, &quot;Id&quot;, &quot;Parent_Id&quot;, &quot;DisplayValue&quot;
        ///	FROM &quot;LibraryHierarchyItems&quot;
        ///	WHERE &quot;LibraryHierarchy_Id&quot; = @libraryHierarchyId
        ///		AND ((@libraryHierarchyItemId IS NULL AND &quot;LibraryHierarchyItems&quot;.&quot;Parent_Id&quot; IS NULL) OR &quot;LibraryHierarchyItems&quot;.&quot;Parent_Id&quot; = @libraryHierarchyItemId)
        ///	UNION ALL 
        ///	SELECT &quot;Root&quot;, &quot;LibraryHierarchyItems&quot;.&quot;Id&quot;, &quot;LibraryHierarchyItems&quot;.&quot;Parent_Id&quot;, &quot;LibraryHierarchyItems&quot;.&quot;DisplayVa [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string GetLibraryHierarchyNodesWithFilter {
            get {
                return ResourceManager.GetString("GetLibraryHierarchyNodesWithFilter", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to UPDATE &quot;LibraryItems&quot;
        ///SET &quot;Status&quot; = @status.
        /// </summary>
        internal static string SetLibraryItemStatus {
            get {
                return ResourceManager.GetString("SetLibraryItemStatus", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to UPDATE &quot;PlaylistItems&quot;
        ///SET &quot;Status&quot; = @status.
        /// </summary>
        internal static string SetPlaylistItemStatus {
            get {
                return ResourceManager.GetString("SetPlaylistItemStatus", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to UPDATE &quot;PlaylistItems&quot;
        ///SET &quot;Sequence&quot; = &quot;Sequence&quot; + @offset
        ///WHERE &quot;Status&quot; = @status AND &quot;Sequence&quot; &gt;= @sequence.
        /// </summary>
        internal static string ShiftPlaylistItems {
            get {
                return ResourceManager.GetString("ShiftPlaylistItems", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Byte[].
        /// </summary>
        internal static byte[] SQLite_Interop_x64 {
            get {
                object obj = ResourceManager.GetObject("SQLite_Interop_x64", resourceCulture);
                return ((byte[])(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Byte[].
        /// </summary>
        internal static byte[] SQLite_Interop_x86 {
            get {
                object obj = ResourceManager.GetObject("SQLite_Interop_x86", resourceCulture);
                return ((byte[])(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to INSERT INTO &quot;MetaDataItems&quot; (&quot;Name&quot;, &quot;Type&quot;, &quot;NumericValue&quot;)
        ///SELECT @name, @type, @numericValue
        ///WHERE NOT EXISTS(SELECT * FROM &quot;MetaDataItems&quot; WHERE &quot;Name&quot; = @name AND &quot;Type&quot; = @type AND &quot;NumericValue&quot; = @numericValue);
        ///
        ///WITH &quot;MetaData&quot;
        ///AS
        ///(
        ///	SELECT 
        ///		&quot;LibraryItem_MetaDataItem&quot;.&quot;LibraryItem_Id&quot; AS &quot;Id&quot;,
        ///		&quot;MetaDataItems&quot;.&quot;Name&quot;,
        ///		&quot;MetaDataItems&quot;.&quot;NumericValue&quot;,
        ///		&quot;MetaDataItems&quot;.&quot;TextValue&quot;
        ///	FROM &quot;LibraryItem_MetaDataItem&quot; 
        ///		JOIN &quot;MetaDataItems&quot; 
        ///			ON &quot;MetaDataItems&quot;.&quot;Id&quot; = &quot;LibraryItem_Me [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string VariousArtists {
            get {
                return ResourceManager.GetString("VariousArtists", resourceCulture);
            }
        }
    }
}
