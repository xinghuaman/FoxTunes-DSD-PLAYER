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
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("FoxTunes.UI.Windows.Properties.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to (function () {
        ///    var parts = [];
        ///    if (tag.iscompilation || tag.__ft_variousartists) {
        ///        parts.push(strings.general_variousartists);
        ///    }
        ///    else if (tag.artist) {
        ///        parts.push(tag.artist);
        ///    }
        ///    if (tag.year) {
        ///        parts.push(tag.year);
        ///    }
        ///    if (tag.album) {
        ///        parts.push(tag.album);
        ///    }
        ///    else {
        ///        parts.push(directoryname(file));
        ///    }
        ///    return parts.join(&quot; - &quot;);
        ///})().
        /// </summary>
        internal static string Grouping {
            get {
                return ResourceManager.GetString("Grouping", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to (function () {
        ///    if (!file) {
        ///        return version();
        ///    }
        ///    var parts = [];
        ///    if (tag.disccount != 1 &amp;&amp; tag.disc) {
        ///        parts.push(tag.disc);
        ///    }
        ///    if (tag.track) {
        ///        parts.push(zeropad2(tag.track, tag.trackcount, 2));
        ///    }
        ///    if (tag.artist) {
        ///        parts.push(tag.artist);
        ///    }
        ///    if (tag.album) {
        ///        parts.push(tag.album);
        ///    }
        ///    if (tag.title) {
        ///        parts.push(tag.title);
        ///    }
        ///    else {
        ///        parts.push(filename(file));
        ///    }
        ///    if (tag [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string NowPlaying {
            get {
                return ResourceManager.GetString("NowPlaying", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;DataTemplate 
        ///    xmlns=&quot;http://schemas.microsoft.com/winfx/2006/xaml/presentation&quot;
        ///    xmlns:x=&quot;http://schemas.microsoft.com/winfx/2006/xaml&quot;
        ///    xmlns:Core=&quot;clr-namespace:FoxTunes;assembly=FoxTunes.Core&quot; 
        ///    xmlns:Windows=&quot;clr-namespace:FoxTunes;assembly=FoxTunes.UI.Windows&quot;
        ///    xmlns:ViewModel=&quot;clr-namespace:FoxTunes.ViewModel;assembly=FoxTunes.UI.Windows&quot;
        ///    DataType=&quot;{x:Type Core:PlaylistItem}&quot;&gt;
        ///    &lt;Windows:PlaybackState&gt;&lt;/Windows:PlaybackState&gt;
        ///&lt;/DataTemplate&gt;.
        /// </summary>
        internal static string PlaybackState {
            get {
                return ResourceManager.GetString("PlaybackState", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to (function () {
        ///    var parts = [];
        ///    if (tag.disccount != 1 &amp;&amp; tag.disc) {
        ///        parts.push(tag.disc);
        ///    }
        ///    if (tag.track) {
        ///        parts.push(zeropad2(tag.track, tag.trackcount, 2));
        ///    }
        ///    if (tag.title) {
        ///        parts.push(tag.title);
        ///    }
        ///    else {
        ///        parts.push(filename(file));
        ///    }
        ///    return parts.join(&quot; - &quot;);
        ///})().
        /// </summary>
        internal static string Playlist {
            get {
                return ResourceManager.GetString("Playlist", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;ControlTemplate
        ///    xmlns=&quot;http://schemas.microsoft.com/winfx/2006/xaml/presentation&quot;
        ///    xmlns:x=&quot;http://schemas.microsoft.com/winfx/2006/xaml&quot;
        ///    xmlns:Core=&quot;clr-namespace:FoxTunes;assembly=FoxTunes.Core&quot; 
        ///    xmlns:Windows=&quot;clr-namespace:FoxTunes;assembly=FoxTunes.UI.Windows&quot;
        ///    xmlns:ViewModel=&quot;clr-namespace:FoxTunes.ViewModel;assembly=FoxTunes.UI.Windows&quot;
        ///    TargetType=&quot;{x:Type Window}&quot;&gt;
        ///    &lt;ControlTemplate.Resources&gt;
        ///        &lt;ViewModel:WindowBase x:Key=&quot;ViewModel&quot;&gt;&lt;/ViewModel:WindowBase&gt;        /// [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string WindowBase {
            get {
                return ResourceManager.GetString("WindowBase", resourceCulture);
            }
        }
    }
}
