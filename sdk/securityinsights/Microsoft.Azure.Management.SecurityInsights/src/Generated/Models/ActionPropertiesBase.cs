// <auto-generated>
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for
// license information.
//
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Microsoft.Azure.Management.SecurityInsights.Models
{
    using Microsoft.Rest;
    using Newtonsoft.Json;
    using System.Linq;

    /// <summary>
    /// Action property bag base.
    /// </summary>
    public partial class ActionPropertiesBase
    {
        /// <summary>
        /// Initializes a new instance of the ActionPropertiesBase class.
        /// </summary>
        public ActionPropertiesBase()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the ActionPropertiesBase class.
        /// </summary>
        /// <param name="logicAppResourceId">Logic App Resource Id,
        /// providers/Microsoft.Logic/workflows/{WorkflowID}.</param>
        public ActionPropertiesBase(string logicAppResourceId)
        {
            LogicAppResourceId = logicAppResourceId;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets logic App Resource Id,
        /// providers/Microsoft.Logic/workflows/{WorkflowID}.
        /// </summary>
        [JsonProperty(PropertyName = "logicAppResourceId")]
        public string LogicAppResourceId { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (LogicAppResourceId == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "LogicAppResourceId");
            }
        }
    }
}
