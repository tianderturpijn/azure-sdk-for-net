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
    public class SecurityInsightsActions : TestBase
    {
        #region Test setup

        public static object Guid = System.Guid.NewGuid();
        public static string strGuid = Guid.ToString();
        private static readonly string ResourceGroup = "AlertsRG";
        private static readonly string Workspace = "AlertsWS";
        private static readonly string AlertId = "7672aeff-e565-4a00-a858-58486d38ef94";
        private static readonly string AlertId2 = "396013d7-8926-4420-beb6-c8d9dd6dc456";
        private static readonly string ActionId = "553f9438-44f9-43fd-bd4f-70ca14f4c82d";
        private static readonly string LogicAppResourceId = "/subscriptions/44e4eff8-1fcb-4a22-a7d6-992ac7286382/resourceGroups/AlertsRG/providers/Microsoft.Logic/workflows/AlertWSPlaybook";
        private static readonly string triggerUri = "https://prod-18.centralus.logic.azure.com:443/workflows/75f1358b52b249bdb7ba28538582ab25/triggers/When_a_response_to_an_Azure_Sentinel_alert_is_triggered/paths/invoke?api-version=2018-07-01-preview&sp=%2Ftriggers%2FWhen_a_response_to_an_Azure_Sentinel_alert_is_triggered%2Frun&sv=1.0&sig=hkKKGdyJVke1uyOVaYzJysj5aSlNL-SkQ2FMdcDd2uE";

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

        public void SecurityInsightsActions_CreateAction()
        {
            using (var context = MockContext.Start(this.GetType()))
            {
                var securityInsightsClient = GetSecurityInsightsClient(context);
                var ActionRequestProperties = new ActionRequest(LogicAppResourceId, triggerUri: triggerUri);
                var Action = securityInsightsClient.AlertRules.CreateOrUpdateAction(ResourceGroup, Workspace, AlertId, strGuid, ActionRequestProperties);
                ValidateAction(Action);
                //Thread.Sleep(30000);
            }
        }

        [Fact]
        public void SecurityInsightsActions_GetActions()
        {
            using (var context = MockContext.Start(this.GetType()))
            {
                var securityInsightsClient = GetSecurityInsightsClient(context);
                var Actions = securityInsightsClient.Actions.ListByAlertRule(ResourceGroup, Workspace, AlertId2);
                ValidateActions(Actions);
            }
        }

        [Fact]

        public void SecurityInsightsActions_GetAction()
        {
            using (var context = MockContext.Start(this.GetType()))
            {
                var securityInsightsClient = GetSecurityInsightsClient(context);
                var Action = securityInsightsClient.AlertRules.GetAction(ResourceGroup, Workspace, AlertId2, ActionId);
                ValidateAction(Action);
            }
        }

       

        [Fact]
        
        public void SecurityInsightsActions_DeleteAction()
        {
            using (var context = MockContext.Start(this.GetType()))
            {
                var securityInsightsClient = GetSecurityInsightsClient(context);
                securityInsightsClient.AlertRules.DeleteAction(ResourceGroup, Workspace, AlertId, strGuid);
            }
        }
   
        #endregion

        #region Validations

        private void ValidateActions(IPage<ActionResponse> ActionPage)
        {
            Assert.True(ActionPage.IsAny());

            ActionPage.ForEach(ValidateAction);
        }

        private void ValidateAction(ActionResponse Action)
        {
            Assert.NotNull(Action);
        }

        #endregion
    }
}
