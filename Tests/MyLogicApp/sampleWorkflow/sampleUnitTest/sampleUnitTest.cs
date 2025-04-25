using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Workflows.UnitTesting.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyLogicApp.Tests.Mocks.sampleWorkflow;

namespace MyLogicApp.Tests
{
    /// <summary>
    /// The unit test class.
    /// </summary>
    [TestClass]
    public class sampleUnitTest
    {
        /// <summary>
        /// The unit test executor.
        /// </summary>
        public TestExecutor TestExecutor;

        [TestInitialize]
        public void Setup()
        {
            this.TestExecutor = new TestExecutor("sampleWorkflow/testSettings.config");
        }

        /// <summary>
        /// A sample unit test for executing the workflow named sampleWorkflow with static mocked data.
        /// This method shows how to set up mock data, execute the workflow, and assert the outcome.
        /// </summary>
        [TestMethod]
        public async Task sampleWorkflow_sampleUnitTest_ExecuteWorkflow_SUCCESS_Sample1()
        {
            // PREPARE Mock
            // Generate mock trigger data.
            var triggerMockOutput = new WhenAHTTPRequestIsReceivedTriggerOutput();
            // Sample of how to set the properties of the triggerMockOutput
            // triggerMockOutput.Body.Id = "SampleId";
            var triggerMock = new WhenAHTTPRequestIsReceivedTriggerMock(outputs: triggerMockOutput);

            // Generate mock action data.
            var actionMockOutput = new ReadAResourceGroupActionOutput();
            // Sample of how to set the properties of the actionMockOutput
            // actionMockOutput.Body.Name = "SampleResource";
            // actionMockOutput.Body.Id = "SampleId";
            var actionMock = new ReadAResourceGroupActionMock(name: "Read_a_resource_group", outputs: actionMockOutput);

            // ACT
            // Create an instance of UnitTestExecutor, and run the workflow with the mock data.
           var testMock = new TestMockDefinition(
                triggerMock: triggerMock,
                actionMocks: new Dictionary<string, ActionMock>()
                {
                    {actionMock.Name, actionMock}
                });
            var testRun = await this.TestExecutor
                .Create()
                .RunWorkflowAsync(testMock: testMock).ConfigureAwait(continueOnCapturedContext: false);

            // ASSERT
            // Verify that the workflow executed successfully, and the status is 'Succeeded'.
            Assert.IsNotNull(value: testRun);
            Assert.AreEqual(expected: TestWorkflowStatus.Succeeded, actual: testRun.Status);
        }

        /// <summary>
        /// A sample unit test for executing the workflow named sampleWorkflow with dynamic mocked data.
        /// This method shows how to set up mock data, execute the workflow, and assert the outcome.
        /// </summary>
        [TestMethod]
        public async Task sampleWorkflow_sampleUnitTest_ExecuteWorkflow_SUCCESS_Sample2()
        {
            // PREPARE
            // Generate mock trigger data.
            var triggerMockOutput = new WhenAHTTPRequestIsReceivedTriggerOutput();
            // Sample of how to set the properties of the triggerMockOutput
            // triggerMockOutput.Body.Flag = true;
            var triggerMock = new WhenAHTTPRequestIsReceivedTriggerMock(outputs: triggerMockOutput);

            // Generate mock action data.
            // OPTION 1 : defining a callback class
            var actionMock = new ReadAResourceGroupActionMock(name: "Read_a_resource_group", onGetActionMock: ReadAResourceGroupActionMockOutputCallback);
            // OPTION 2: defining inline using a lambda
            /*var actionMock = new ReadAResourceGroupActionMock(name: "Read_a_resource_group", onGetActionMock: (testExecutionContext) =>
            {
                return new ReadAResourceGroupActionMock(
                    status: TestWorkflowStatus.Succeeded,
                    outputs: new ReadAResourceGroupActionOutput {
                        // set the desired properties here
                        // if this acount contains a JObject Body
                        // Body = "something".ToJObject()
                    }
                );
            });*/

            // ACT
            // Create an instance of UnitTestExecutor, and run the workflow with the mock data.
            var testMock = new TestMockDefinition(
                triggerMock: triggerMock,
                actionMocks: new Dictionary<string, ActionMock>()
                {
                    {actionMock.Name, actionMock}
                });
            var testRun = await this.TestExecutor
                .Create()
                .RunWorkflowAsync(testMock: testMock).ConfigureAwait(continueOnCapturedContext: false);

            // ASSERT
            // Verify that the workflow executed successfully, and the status is 'Succeeded'.
            Assert.IsNotNull(value: testRun);
            Assert.AreEqual(expected: TestWorkflowStatus.Succeeded, actual: testRun.Status);
        }

        #region Mock generator helpers

        /// <summary>
        /// The callback method to dynamically generate mocked data for the action named 'actionName'.
        /// You can modify this method to return different mock status, outputs, and error based on the test scenario.
        /// </summary>
        /// <param name="context">The test execution context that contains information about the current test run.</param>
        public ReadAResourceGroupActionMock ReadAResourceGroupActionMockOutputCallback(TestExecutionContext context)
        {
            // Sample mock data : Modify the existing mocked data dynamically for "actionName".
            return new ReadAResourceGroupActionMock(
                status: TestWorkflowStatus.Succeeded,
                outputs: new ReadAResourceGroupActionOutput {
                    // set the desired properties here
                    // if this acount contains a JObject Body
                    // Body = "something".ToJObject()
                }
            );
        }

        #endregion
    }
}