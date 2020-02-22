// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Net;
using System.Text.RegularExpressions;
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
    public class SecurityInsightsAlertRules : TestBase
    {
        #region Test setup

        public static object Guid = System.Guid.NewGuid();
        public static string strGuid = Guid.ToString();
        private static readonly string ResourceGroup = "AlertsRG";
        private static readonly string Workspace = "AlertsWS";

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

        #region AlertRules
  

        [Fact]
        public void SecurityInsightsAlertRules_List()
        {
            using (var context = MockContext.Start(this.GetType()))
            {
                var securityInsightsClient = GetSecurityInsightsClient(context);
                var alertRules = securityInsightsClient.AlertRules.List(ResourceGroup, Workspace);
                ValidateAlertRules(alertRules);
            }
        }

        [Fact]

        public void SecurityInsightsAlertRules_CreateAlertRule()
        {
            using (var context = MockContext.Start(this.GetType()))
            {
                var securityInsightsClient = GetSecurityInsightsClient(context);
                var Timespan = XmlConvert.ToTimeSpan("PT1H");
                var queryFrequency = XmlConvert.ToTimeSpan("P1D");
                var alertRuleProperties = new ScheduledAlertRule("test Rule", false, Timespan, false, severity:"low", query:"SecurityAlert", queryFrequency:queryFrequency, queryPeriod:queryFrequency, triggerOperator:Microsoft.Azure.Management.SecurityInsights.Models.TriggerOperator.GreaterThan, triggerThreshold:10);
                var alertRule = securityInsightsClient.AlertRules.CreateOrUpdate(ResourceGroup, Workspace, strGuid, alertRuleProperties);
                ValidateAlertRule(alertRule);
            }
        }
        
        [Fact]

        public void SecurityInsightsAlertRules_GetAlertRule()
        {
            using (var context = MockContext.Start(this.GetType()))
            {
                var securityInsightsClient = GetSecurityInsightsClient(context);
                var alertRule = securityInsightsClient.AlertRules.Get(ResourceGroup, Workspace, strGuid);
                ValidateAlertRule(alertRule);
            }
        }

       

        [Fact]
        
        public void SecurityInsightsAlertRules_DeleteAlertRule()
        {
            using (var context = MockContext.Start(this.GetType()))
            {
                var securityInsightsClient = GetSecurityInsightsClient(context);
                securityInsightsClient.AlertRules.Delete(ResourceGroup, Workspace, strGuid);
            }
        }
   
        #endregion

        #region Validations

        private void ValidateAlertRules(IPage<AlertRule> alertRulePage)
        {
            Assert.True(alertRulePage.IsAny());

            alertRulePage.ForEach(ValidateAlertRule);
        }

        private void ValidateAlertRule(AlertRule alertRule)
        {
            Assert.NotNull(alertRule);
        }

        #endregion
    }
}
