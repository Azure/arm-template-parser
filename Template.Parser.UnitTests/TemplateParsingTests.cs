using Newtonsoft.Json;
using System.Diagnostics;
using System.Reflection;
using Template.Parser.Core;

namespace Template.Parser.UnitTests
{
    [TestClass]
    public class TemplateParsingTests
    {
        public string AssemblyPath
        {
            get
            {
                return new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.ToString();
            }
        }

        [TestMethod]
        public void CanParseExampleTemplate()
        {
            var templateJson = File.ReadAllText($"{AssemblyPath}/exampleTemplates/exampleTemplate01.json");
            var templateParser = new ArmTemplateProcessor(templateJson);
            var result = templateParser.ProcessTemplate();
            Assert.IsNotNull(result);
            var resource = result.ToObject<dynamic>().resources[0];
            Assert.AreEqual("Audit-AppGW-WAF", resource.name.Value);
            Assert.AreEqual("Assign the WAF should be enabled for Application Gateway audit policy.", resource.properties.description.Value);
            Assert.AreEqual("Web Application Firewall (WAF) should be enabled for Application Gateway", resource.properties.displayName.Value);
        }

        [TestMethod]
        public void CanParseAndConvertToJsonStringWithNonComplianceMessage()
        {
            var templateJson = File.ReadAllText($"{AssemblyPath}/exampleTemplates/exampleTemplate01.json");
            var templateParser = new ArmTemplateProcessor(templateJson);
            var result = templateParser.ProcessTemplate();

            var resultJson = JsonConvert.SerializeObject(result.SelectToken("resources")[0], Formatting.Indented);

            var expectedResult = @"{
  ""type"": ""Microsoft.Authorization/policyAssignments"",
  ""apiVersion"": ""2019-09-01"",
  ""name"": ""Audit-AppGW-WAF"",
  ""dependsOn"": [],
  ""properties"": {
    ""description"": ""Assign the WAF should be enabled for Application Gateway audit policy."",
    ""displayName"": ""Web Application Firewall (WAF) should be enabled for Application Gateway"",
    ""policyDefinitionId"": ""/providers/Microsoft.Authorization/policyDefinitions/564feb30-bf6a-4854-b4bb-0d2d2d1e6c66"",
    ""enforcementMode"": ""Default"",
    ""nonComplianceMessages"": [
      {
        ""message"": ""Web Application Firewall (WAF) must be enabled for Application Gateway.""
      }
    ],
    ""parameters"": {
      ""effect"": {
        ""value"": ""Audit""
      }
    }
  }
}";
            Debug.Write(resultJson);
            Assert.AreEqual(expectedResult.Replace("\r\n", "\n"), resultJson.Replace("\r\n", "\n"));

        }


        [TestMethod]
        public void CanParseAndConvertToJsonStringWithScope()
        {
            var templateJson = File.ReadAllText($"{AssemblyPath}/exampleTemplates/exampleTemplate02.json");
            var templateParser = new ArmTemplateProcessor(templateJson);
            var result = templateParser.ProcessTemplate();

            var resultJson = JsonConvert.SerializeObject(result.SelectToken("resources")[0], Formatting.Indented);

            var expectedResult = @"{
  ""type"": ""Microsoft.Authorization/policyAssignments"",
  ""apiVersion"": ""2019-09-01"",
  ""name"": ""Deny-Priv-Esc-AKS"",
  ""dependsOn"": [],
  ""properties"": {
    ""description"": ""Do not allow containers to run with privilege escalation to root in a Kubernetes cluster. This recommendation is part of CIS 5.2.5 which is intended to improve the security of your Kubernetes environments. This policy is generally available for Kubernetes Service (AKS), and preview for AKS Engine and Azure Arc enabled Kubernetes. For more information, see https://aka.ms/kubepolicydoc."",
    ""displayName"": ""Kubernetes clusters should not allow container privilege escalation"",
    ""policyDefinitionId"": ""/providers/Microsoft.Authorization/policyDefinitions/1c6e92c9-99f0-4e55-9cf2-0c234dc48f99"",
    ""enforcementMode"": ""Default"",
    ""parameters"": {
      ""effect"": {
        ""value"": ""deny""
      }
    }
  }
}";
            Debug.Write(resultJson);
            Assert.AreEqual(expectedResult.Replace("\r\n", "\n"), resultJson.Replace("\r\n", "\n"));

        }


        [TestMethod]
        public void CanParseAndConvertToJsonStringWithIdentity()
        {
            var templateJson = File.ReadAllText($"{AssemblyPath}/exampleTemplates/exampleTemplate03.json");
            var templateParser = new ArmTemplateProcessor(templateJson);
            var result = templateParser.ProcessTemplate();

            var resultJson = JsonConvert.SerializeObject(result.SelectToken("resources")[0], Formatting.Indented);

            var expectedResult = @"{
  ""type"": ""Microsoft.Authorization/policyAssignments"",
  ""apiVersion"": ""2019-09-01"",
  ""name"": ""Deploy-VMSS-Monitoring"",
  ""location"": ""westus2"",
  ""dependsOn"": [],
  ""identity"": {
    ""type"": ""SystemAssigned""
  },
  ""properties"": {
    ""description"": ""Enable Azure Monitor for the Virtual Machine Scale Sets in the specified scope (Management group, Subscription or resource group). Takes Log Analytics workspace as parameter. Note: if your scale set upgradePolicy is set to Manual, you need to apply the extension to the all VMs in the set by calling upgrade on them. In CLI this would be az vmss update-instances."",
    ""displayName"": ""Enable Azure Monitor for Virtual Machine Scale Sets"",
    ""policyDefinitionId"": ""/providers/Microsoft.Authorization/policySetDefinitions/75714362-cae7-409e-9b99-a8e5075b7fad"",
    ""enforcementMode"": ""Default"",
    ""parameters"": {
      ""logAnalytics_1"": {
        ""value"": ""defaultString1""
      }
    }
  }
}";
            Debug.Write(resultJson);
            Assert.AreEqual(expectedResult.Replace("\r\n", "\n"), resultJson.Replace("\r\n", "\n"));
        }

        [TestMethod]
        public void CanGenerateMetaData()
        {
            var metaData = PlaceholderInputGenerator.GeneratePlaceholderDeploymentMetadata("${default_location}");

        }
    }
}