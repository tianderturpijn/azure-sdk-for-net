// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Azure.Management.SecurityInsights;
using Microsoft.Azure.Management.SecurityInsights.Models;
using Microsoft.Azure.Test.HttpRecorder;
using Microsoft.Rest.Azure;
using Microsoft.Rest.ClientRuntime.Azure.TestFramework;
using SecurityInsights.Tests.Helpers;
using Xunit;

namespace SecurityInsights.Tests
{
    public class SecurityInsightsDataConnectors : TestBase
    {
        #region Test setup

        public static object Guid = System.Guid.NewGuid();
        public static string strGuid = Guid.ToString();
        private static readonly string ResourceGroup = "AlertsRG";
        private static readonly string Workspace = "AlertsWS";
        private static readonly string DataConnectorId = "b5bfa9e3-7be5-4dc2-be6c-cdbe1194ca1a";
        private static readonly string MCASDataConnectorId = "bbbddcaa-625f-401d-8ed7-aa119b463a2d";
        private static readonly string TenantId = "4b2462a4-bbee-495a-a0e1-f23ae524cc9c";

        public static TestEnvironment TestEnvironment { get; private set; }


        private static SecurityInsightsClient GetSecurityInsightsClient(MockContext context)
        {
            if (TestEnvironment == null && HttpMockServer.Mode == HttpRecorderMode.Record)
            {
                TestEnvironment = TestEnvironmentFactory.GetTestEnvironment();
            }

            var handler = new RecordedDelegatingHandler { StatusCodeToReturn = HttpStatusCode.OK, IsPassThrough = true };

            var securityInsightsClient = HttpMockServer.Mode == HttpRecorderMode.Record
                ? context.GetServiceClient<SecurityInsightsClient>(TestEnvironment, handlers: handler)
                : context.GetServiceClient<SecurityInsightsClient>(handlers: handler);

            return securityInsightsClient;
        }

        #endregion

        #region Actions
  

        [Fact]

        public void SecurityInsightsDataConnectors_CreateDataConnector()
        {
            using (var context = MockContext.Start(this.GetType()))
            {
                var securityInsightsClient = GetSecurityInsightsClient(context);
                var AlertsDataTypeOfDataConnectorAlerts = new AlertsDataTypeOfDataConnectorAlerts("enabled");
                var AlertsDataTypeOfDataConnector = new AlertsDataTypeOfDataConnector(AlertsDataTypeOfDataConnectorAlerts);
                var DataConnectorProperties = new AATPDataConnector(DataConnectorId, tenantId: TenantId, dataTypes: AlertsDataTypeOfDataConnector);
                var DataConnector = securityInsightsClient.DataConnectors.CreateOrUpdate(ResourceGroup, Workspace, DataConnectorId, DataConnectorProperties);
                ValidateDataConnector(DataConnector);
                //Thread.Sleep(30000);
            }
        }

        [Fact]
        public void SecurityInsightsDataConnectors_GetDataConnectors()
        {
            using (var context = MockContext.Start(this.GetType()))
            {
                var securityInsightsClient = GetSecurityInsightsClient(context);
                var DataConnectors = securityInsightsClient.DataConnectors.List(ResourceGroup, Workspace);
                ValidateDataConnectors(DataConnectors);
            }
        }

        [Fact]

        public void SecurityInsightsDataConnectors_GetDataConnector()
        {
            using (var context = MockContext.Start(this.GetType()))
            {
                var securityInsightsClient = GetSecurityInsightsClient(context);
                var DataConnector = securityInsightsClient.DataConnectors.Get(ResourceGroup, Workspace, MCASDataConnectorId);
                ValidateDataConnector(DataConnector);
            }
        }

       

        [Fact]
        
        public void SecurityInsightsDataConnectors_DeleteDataConnector()
        {
            using (var context = MockContext.Start(this.GetType()))
            {
                var securityInsightsClient = GetSecurityInsightsClient(context);
                securityInsightsClient.DataConnectors.Delete(ResourceGroup, Workspace, DataConnectorId);
            }
        }
   
        #endregion

        #region Validations

        private void ValidateDataConnectors(IPage<DataConnector> DataConnectorPage)
        {
            Assert.True(DataConnectorPage.IsAny());

            DataConnectorPage.ForEach(ValidateDataConnector);
        }

        private void ValidateDataConnector(DataConnector DataConnector)
        {
            Assert.NotNull(DataConnector);
        }

        #endregion
    }
}
