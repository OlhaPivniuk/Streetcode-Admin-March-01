﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Streetcode.BLL.Resources.Errors.ValidationErrors.Fact {
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
    public class ReorderFactErrors {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ReorderFactErrors() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Streetcode.BLL.Resources.Errors.ValidationErrors.Fact.ReorderFactErrors", typeof(ReorderFactErrors).Assembly);
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
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot update property Number in fact.
        /// </summary>
        public static string CannotUpdateNumberInFact {
            get {
                return ResourceManager.GetString("CannotUpdateNumberInFact", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The incoming array of Ids is null or has no elements..
        /// </summary>
        public static string IncomingFactIdArrIsNullOrEmpty {
            get {
                return ResourceManager.GetString("IncomingFactIdArrIsNullOrEmpty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Fact with Id={0} does not exist or has StreetcodeId different from {1}.
        /// </summary>
        public static string IncorrectFactIdInArray {
            get {
                return ResourceManager.GetString("IncorrectFactIdInArray", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The number of IDs transferred in the array ({0}) does not correspond to the number of facts ({1}) for the StreetcodeId = {2}.
        /// </summary>
        public static string IncorrectIdsNumberInArray {
            get {
                return ResourceManager.GetString("IncorrectIdsNumberInArray", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to There are no facts with corresponding StreetcodeId = {0}.
        /// </summary>
        public static string ThereAreNoFactsWithCorrespondingStreetcodeId {
            get {
                return ResourceManager.GetString("ThereAreNoFactsWithCorrespondingStreetcodeId", resourceCulture);
            }
        }
    }
}
