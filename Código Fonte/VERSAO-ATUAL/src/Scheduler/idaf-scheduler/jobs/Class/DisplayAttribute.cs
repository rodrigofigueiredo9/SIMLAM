using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tecnomapas.EtramiteX.Scheduler.jobs.Class
{/*
    // Summary:
    //     Provides a general-purpose attribute that lets you specify localizable strings
    //     for types and members of entity partial classes.
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class DisplayAttribute : Attribute
    {
        // Summary:
        //     Initializes a new instance of the System.ComponentModel.DataAnnotations.DisplayAttribute
        //     class.
        public DisplayAttribute();

        // Summary:
        //     Gets or sets a value that indicates whether UI should be generated automatically
        //     in order to display this field.
        //
        // Returns:
        //     true if UI should be generated automatically to display this field; otherwise,
        //     false.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     An attempt was made to get the property value before it was set.
        public bool AutoGenerateField { get; set; }
        //
        // Summary:
        //     Gets or sets a value that indicates whether filtering UI is automatically
        //     displayed for this field.
        //
        // Returns:
        //     true if UI should be generated automatically to display filtering for this
        //     field; otherwise, false.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     An attempt was made to get the property value before it was set.
        public bool AutoGenerateFilter { get; set; }
        //
        // Summary:
        //     Gets or sets a value that is used to display a description in the UI.
        //
        // Returns:
        //     The value that is used to display a description in the UI.
        public string Description { get; set; }
        //
        // Summary:
        //     Gets or sets a value that is used to group fields in the UI.
        //
        // Returns:
        //     A value that is used to group fields in the UI.
        public string GroupName { get; set; }
        //
        // Summary:
        //     Gets or sets a value that is used for display in the UI.
        //
        // Returns:
        //     A value that is used for display in the UI.
        public string Name { get; set; }
        //
        // Summary:
        //     Gets or sets the order weight of the column.
        //
        // Returns:
        //     The order weight of the column.
        public int Order { get; set; }
        //
        // Summary:
        //     Gets or sets a value that will be used to set the watermark for prompts in
        //     the UI.
        //
        // Returns:
        //     A value that will be used to display a watermark in the UI.
        public string Prompt { get; set; }
        //
        // Summary:
        //     Gets or sets the type that contains the resources for the System.ComponentModel.DataAnnotations.DisplayAttribute.ShortName,
        //     System.ComponentModel.DataAnnotations.DisplayAttribute.Name, System.ComponentModel.DataAnnotations.DisplayAttribute.Prompt,
        //     and System.ComponentModel.DataAnnotations.DisplayAttribute.Description properties.
        //
        // Returns:
        //     The type of the resource that contains the System.ComponentModel.DataAnnotations.DisplayAttribute.ShortName,
        //     System.ComponentModel.DataAnnotations.DisplayAttribute.Name, System.ComponentModel.DataAnnotations.DisplayAttribute.Prompt,
        //     and System.ComponentModel.DataAnnotations.DisplayAttribute.Description properties.
        public Type ResourceType { get; set; }
        //
        // Summary:
        //     Gets or sets a value that is used for the grid column label.
        //
        // Returns:
        //     A value that is for the grid column label.
        public string ShortName { get; set; }

        // Summary:
        //     Returns the value of the System.ComponentModel.DataAnnotations.DisplayAttribute.AutoGenerateField
        //     property.
        //
        // Returns:
        //     The value of System.ComponentModel.DataAnnotations.DisplayAttribute.AutoGenerateField
        //     if the property has been initialized; otherwise, null.
        public bool? GetAutoGenerateField();
        //
        // Summary:
        //     Returns a value that indicates whether UI should be generated automatically
        //     in order to display filtering for this field.
        //
        // Returns:
        //     The value of System.ComponentModel.DataAnnotations.DisplayAttribute.AutoGenerateFilter
        //     if the property has been initialized; otherwise, null.
        public bool? GetAutoGenerateFilter();
        //
        // Summary:
        //     Returns the value of the System.ComponentModel.DataAnnotations.DisplayAttribute.Description
        //     property.
        //
        // Returns:
        //     The localized description, if the System.ComponentModel.DataAnnotations.DisplayAttribute.ResourceType
        //     has been specified and the System.ComponentModel.DataAnnotations.DisplayAttribute.Description
        //     property represents a resource key; otherwise, the non-localized value of
        //     the System.ComponentModel.DataAnnotations.DisplayAttribute.Description property.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The System.ComponentModel.DataAnnotations.DisplayAttribute.ResourceType property
        //     and the System.ComponentModel.DataAnnotations.DisplayAttribute.Description
        //     property are initialized, but a public static property that has a name that
        //     matches the System.ComponentModel.DataAnnotations.DisplayAttribute.Description
        //     value could not be found for the System.ComponentModel.DataAnnotations.DisplayAttribute.ResourceType
        //     property.
        public string GetDescription();
        //
        // Summary:
        //     Returns the value of the System.ComponentModel.DataAnnotations.DisplayAttribute.GroupName
        //     property.
        //
        // Returns:
        //     A value that will be used for grouping fields in the UI, if System.ComponentModel.DataAnnotations.DisplayAttribute.GroupName
        //     has been initialized; otherwise, null. If the System.ComponentModel.DataAnnotations.DisplayAttribute.ResourceType
        //     property has been specified and the System.ComponentModel.DataAnnotations.DisplayAttribute.GroupName
        //     property represents a resource key, a localized string is returned; otherwise,
        //     a non-localized string is returned.
        public string GetGroupName();
        //
        // Summary:
        //     Returns a value that is used for field display in the UI.
        //
        // Returns:
        //     The localized string for the System.ComponentModel.DataAnnotations.DisplayAttribute.Name
        //     property, if the System.ComponentModel.DataAnnotations.DisplayAttribute.ResourceType
        //     property has been specified and the System.ComponentModel.DataAnnotations.DisplayAttribute.Name
        //     property represents a resource key; otherwise, the non-localized value of
        //     the System.ComponentModel.DataAnnotations.DisplayAttribute.Name property.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The System.ComponentModel.DataAnnotations.DisplayAttribute.ResourceType property
        //     and the System.ComponentModel.DataAnnotations.DisplayAttribute.Name property
        //     are initialized, but a public static property that has a name that matches
        //     the System.ComponentModel.DataAnnotations.DisplayAttribute.Name value could
        //     not be found for the System.ComponentModel.DataAnnotations.DisplayAttribute.ResourceType
        //     property.
        public string GetName();
        //
        // Summary:
        //     Returns the value of the System.ComponentModel.DataAnnotations.DisplayAttribute.Order
        //     property.
        //
        // Returns:
        //     The value of the System.ComponentModel.DataAnnotations.DisplayAttribute.Order
        //     property, if it has been set; otherwise, null.
        public int? GetOrder();
        //
        // Summary:
        //     Returns the value of the System.ComponentModel.DataAnnotations.DisplayAttribute.Prompt
        //     property.
        //
        // Returns:
        //     Gets the localized string for the System.ComponentModel.DataAnnotations.DisplayAttribute.Prompt
        //     property if the System.ComponentModel.DataAnnotations.DisplayAttribute.ResourceType
        //     property has been specified and if the System.ComponentModel.DataAnnotations.DisplayAttribute.Prompt
        //     property represents a resource key; otherwise, the non-localized value of
        //     the System.ComponentModel.DataAnnotations.DisplayAttribute.Prompt property.
        public string GetPrompt();
        //
        // Summary:
        //     Returns the value of the System.ComponentModel.DataAnnotations.DisplayAttribute.ShortName
        //     property.
        //
        // Returns:
        //     The localized string for the System.ComponentModel.DataAnnotations.DisplayAttribute.ShortName
        //     property if the System.ComponentModel.DataAnnotations.DisplayAttribute.ResourceType
        //     property has been specified and if the System.ComponentModel.DataAnnotations.DisplayAttribute.ShortName
        //     property represents a resource key; otherwise, the non-localized value of
        //     the System.ComponentModel.DataAnnotations.DisplayAttribute.ShortName value
        //     property.
        public string GetShortName();
    }*/
}
