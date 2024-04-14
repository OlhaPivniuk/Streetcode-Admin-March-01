﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Streetcode.BLL.Resources.Errors {
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
    public class ErrorMessages {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ErrorMessages() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Streetcode.BLL.Resources.Errors.ErrorMessages", typeof(ErrorMessages).Assembly);
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
        ///   Looks up a localized string similar to Failed to create a {0}.
        /// </summary>
        public static string CreateFailed {
            get {
                return ResourceManager.GetString("CreateFailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Failed to delete the {0} with id: {1}.
        /// </summary>
        public static string DeleteFailed {
            get {
                return ResourceManager.GetString("DeleteFailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot find any {0}s.
        /// </summary>
        public static string EntitiesNotFound {
            get {
                return ResourceManager.GetString("EntitiesNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot find {0} with categoryId: {1}.
        /// </summary>
        public static string EntityByCategoryIdNotFound {
            get {
                return ResourceManager.GetString("EntityByCategoryIdNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot find {0} with id: {1}.
        /// </summary>
        public static string EntityByIdNotFound {
            get {
                return ResourceManager.GetString("EntityByIdNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot find {0} with the requested PrimaryKey.
        /// </summary>
        public static string EntityByPrimaryKeyNotFound {
            get {
                return ResourceManager.GetString("EntityByPrimaryKeyNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot find {0} with streetcode id: {1}.
        /// </summary>
        public static string EntityByStreetCodeIdNotFound {
            get {
                return ResourceManager.GetString("EntityByStreetCodeIdNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Failed to delete {0}.
        /// </summary>
        public static string FailedToDeleteByPrimaryKey {
            get {
                return ResourceManager.GetString("FailedToDeleteByPrimaryKey", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Potencial Primary Key {0}Id = {1} is not unique.
        /// </summary>
        public static string PotencialPrimaryKeyIsNotUnique {
            get {
                return ResourceManager.GetString("PotencialPrimaryKeyIsNotUnique", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Primary Key of the {0} is not unique.
        /// </summary>
        public static string PrimaryKeyIsNotUnique {
            get {
                return ResourceManager.GetString("PrimaryKeyIsNotUnique", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Value of property {0} = {1} is not unique in {2}.
        /// </summary>
        public static string PropertyMustBeUnique {
            get {
                return ResourceManager.GetString("PropertyMustBeUnique", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Failed to update the {0} with id: {1}.
        /// </summary>
        public static string UpdateFailed {
            get {
                return ResourceManager.GetString("UpdateFailed", resourceCulture);
            }
        }
    }
}
