using Microsoft.Azure.Workflows.UnitTesting.Definitions;
using Microsoft.Azure.Workflows.UnitTesting.ErrorResponses;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net;
using System;

namespace MyLogicApp.Tests.Mocks.sampleWorkflow
{
    /// <summary>
    /// The <see cref="ReadAResourceGroupActionMock"/> class.
    /// </summary>
    public class ReadAResourceGroupActionMock : ActionMock
    {
        /// <summary>
        /// Creates a mocked instance for  <see cref="ReadAResourceGroupActionMock"/> with static outputs.
        /// </summary>
        public ReadAResourceGroupActionMock(TestWorkflowStatus status = TestWorkflowStatus.Succeeded, string name = null, ReadAResourceGroupActionOutput outputs = null)
            : base(status: status, name: name, outputs: outputs ?? new ReadAResourceGroupActionOutput())
        {
        }

        /// <summary>
        /// Creates a mocked instance for  <see cref="ReadAResourceGroupActionMock"/> with static error info.
        /// </summary>
        public ReadAResourceGroupActionMock(TestWorkflowStatus status, string name = null, TestErrorInfo error = null)
            : base(status: status, name: name, error: error)
        {
        }

        /// <summary>
        /// Creates a mocked instance for <see cref="ReadAResourceGroupActionMock"/> with a callback function for dynamic outputs.
        /// </summary>
        public ReadAResourceGroupActionMock(Func<TestExecutionContext, ReadAResourceGroupActionMock> onGetActionMock, string name = null)
            : base(onGetActionMock: onGetActionMock, name: name)
        {
        }
    }


    /// <summary>
    /// Class for ReadAResourceGroupActionOutput representing an object with properties.
    /// </summary>
    public class ReadAResourceGroupActionOutput : MockOutput
    {
        /// <summary>
        /// Resource group information.
        /// </summary>
        public ReadAResourceGroupActionOutputBody Body { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadAResourceGroupActionOutput"/> class.
        /// </summary>
        public ReadAResourceGroupActionOutput()
        {
            this.StatusCode = HttpStatusCode.OK;
            this.Body = new ReadAResourceGroupActionOutputBody();
        }

    }

    /// <summary>
    /// Resource group information.
    /// </summary>
    public class ReadAResourceGroupActionOutputBody
    {
        /// <summary>
        /// The location of the resource group. It cannot be changed after the resource group has been created. Has to be one of the supported Azure Locations, such as West US, East US, West Europe, East Asia, etc.
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// The Name of the resource group.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The resource group properties.
        /// </summary>
        public ReadAResourceGroupActionOutputBodyProperties Properties { get; set; }

        /// <summary>
        /// The ID of the resource group (e.g. /subscriptions/XXX/resourceGroups/YYY).
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Id of the resource that manages this resource group.
        /// </summary>
        public string ManagedBy { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadAResourceGroupActionOutputBody"/> class.
        /// </summary>
        public ReadAResourceGroupActionOutputBody()
        {
            this.Location = string.Empty;
            this.Name = string.Empty;
            this.Properties = new ReadAResourceGroupActionOutputBodyProperties();
            this.Id = string.Empty;
            this.ManagedBy = string.Empty;
        }

    }

    /// <summary>
    /// The resource group properties.
    /// </summary>
    public class ReadAResourceGroupActionOutputBodyProperties
    {
        /// <summary>
        /// The provisioning state.
        /// </summary>
        public string ProvisioningState { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadAResourceGroupActionOutputBodyProperties"/> class.
        /// </summary>
        public ReadAResourceGroupActionOutputBodyProperties()
        {
            this.ProvisioningState = string.Empty;
        }

    }

}