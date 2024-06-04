using Microsoft.VisualStudio.TestPlatform.TestHost;
using Newtonsoft.Json;
using System.Reflection;
using System.Xml.Linq;

namespace Template.Parser.Cli.UnitTests
{
    [TestClass]
    public class ParserTests
    {
        public string AssemblyPath
        {
            get
            {
                return new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.ToString();
            }
        }

        [TestMethod]
        public void CanParseParameters()
        {
            var parameters = new List<string> { "location=${default_location}", "properties.scope=${current_scope_resource_id}" };

            var parsedParameters = Template.Parser.Cli.Program.BuildParameters(parameters);

            Assert.AreEqual(@"{
  ""parameters"": {
    ""location"": {
      ""value"": ""${default_location}""
    },
    ""properties.scope"": {
      ""value"": ""${current_scope_resource_id}""
    }
  }
}".Replace("\r\n", "\n"), parsedParameters.Replace("\r\n", "\n"));

        }

        [TestMethod]
        public void CanUseDefaultsAndParametersInCLi()
        {
            var tempateFilePath = Path.Combine(AssemblyPath, "exampleTemplates", "exampleTemplate03.json");
            var templateFile = $"-s {tempateFilePath}";
            var parameters = "-p logAnalyticsResourceId=/subscriptions/00000000-0000-0000-0000-000000000000/resourcegroups/${root_scope_id}-mgmt/providers/Microsoft.OperationalInsights/workspaces/${root_scope_id}-la";

            var stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            Template.Parser.Cli.Program.Main(new string[] { templateFile, parameters }).Wait();


            var output = stringWriter.ToString();
            Assert.AreEqual(@"{
  ""type"": ""Microsoft.Authorization/policyAssignments"",
  ""apiVersion"": ""2019-09-01"",
  ""name"": ""Deploy-VMSS-Monitoring"",
  ""location"": ""${default_location}"",
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
        ""value"": ""/subscriptions/00000000-0000-0000-0000-000000000000/resourcegroups/${root_scope_id}-mgmt/providers/Microsoft.OperationalInsights/workspaces/${root_scope_id}-la""
      }
    }
  }
}".Replace("\r\n", "\n"), output.Replace("\r\n", "\n"));
        }

        [TestMethod]
        public void CanUseDefaultsAndNoParametersInCLi()
        {
            var tempateFilePath = Path.Combine(AssemblyPath, "exampleTemplates", "exampleTemplate03.json");
            var templateFile = $"-s {tempateFilePath}";
            
            var stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            Template.Parser.Cli.Program.Main(new string[] { templateFile }).Wait();

            var output = stringWriter.ToString();
            Assert.AreEqual(@"{
  ""type"": ""Microsoft.Authorization/policyAssignments"",
  ""apiVersion"": ""2019-09-01"",
  ""name"": ""Deploy-VMSS-Monitoring"",
  ""location"": ""${default_location}"",
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
}".Replace("\r\n", "\n"), output.Replace("\r\n", "\n"));
        }

        [TestMethod]
        public void CanUseParametersInForNonCompliance()
        {
            var tempateFilePath = Path.Combine(AssemblyPath, "exampleTemplates", "exampleTemplate01.json");
            var templateFile = $"-s {tempateFilePath}";
            var parameters = "-p nonComplianceMessagePlaceholder={donotchange}";

            var stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            Template.Parser.Cli.Program.Main(new string[] { templateFile, parameters }).Wait();


            var output = stringWriter.ToString();
            Assert.AreEqual(@"{
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
        ""message"": ""Web Application Firewall (WAF) {enforcementMode} be enabled for Application Gateway.""
      }
    ],
    ""parameters"": {
      ""effect"": {
        ""value"": ""Audit""
      }
    }
  }
}".Replace("\r\n", "\n"), output.Replace("\r\n", "\n"));
        }


        [TestMethod]
        public void CanUseTypedParameters()
        {
            var tempateFilePath = Path.Combine(AssemblyPath, "exampleTemplates", "exampleTemplate04.json");
            var templateFile = $"-s {tempateFilePath}";
            var parameter1 = "-p stringExample=[[[String]]]ThisIsAString";
            var parameter2 = "-p numberExample=[[[Int64]]]123";

            var stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            Template.Parser.Cli.Program.Main(new string[] { templateFile, parameter1, parameter2 }).Wait();


            var output = stringWriter.ToString();
            Assert.AreEqual(@"{
  ""type"": ""Microsoft.Authorization/policyAssignments"",
  ""apiVersion"": ""2019-09-01"",
  ""name"": ""Audit-AppGW-WAF"",
  ""dependsOn"": [],
  ""properties"": {
    ""description"": ""ThisIsAString"",
    ""displayName"": 123,
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
}".Replace("\r\n", "\n"), output.Replace("\r\n", "\n"));
        }

        [TestMethod]
        public void CanUseParametersFile()
        {
            var tempateFilePath = Path.Combine(AssemblyPath, "exampleTemplates", "exampleTemplate04.json");
            var parametersFilePath = Path.Combine(AssemblyPath, "exampleTemplates", "exampleTemplate04Params.json");
            var templateFile = $"-s {tempateFilePath}";
            var parametersFile = $"-f {parametersFilePath}";


            var stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            Template.Parser.Cli.Program.Main(new string[] { templateFile, parametersFile }).Wait();


            var output = stringWriter.ToString();
            Assert.AreEqual(@"{
  ""type"": ""Microsoft.Authorization/policyAssignments"",
  ""apiVersion"": ""2019-09-01"",
  ""name"": ""Audit-AppGW-WAF"",
  ""dependsOn"": [],
  ""properties"": {
    ""description"": ""ThisIsAStringFromAFile"",
    ""displayName"": 12345,
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
}".Replace("\r\n", "\n"), output.Replace("\r\n", "\n"));
        }

        [TestMethod]
        public void CanUseParseEslzFile()
        {
            var tempateFilePath = Path.Combine(AssemblyPath, "exampleTemplates", "eslzArm.json");
            var parametersFilePath = Path.Combine(AssemblyPath, "exampleTemplates", "eslzArm.test.param.json");
            var templateFile = $"-s {tempateFilePath}";
            var parametersFile = $"-f {parametersFilePath}";


            var stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            Template.Parser.Cli.Program.Main(new string[] { templateFile, parametersFile, "-a" }).Wait();


            var output = stringWriter.ToString();

            var check = JsonConvert.DeserializeObject<List<dynamic>>(output);

            Assert.AreEqual(171, check.Count);
        }
    
        [TestMethod]
        public void CanUseParseMultiAssignmentFile()
        {
            var tempateFilePath = Path.Combine(AssemblyPath, "exampleTemplates", "exampleTemplate05MultipleAssignments.json");
            //var parametersFilePath = Path.Combine(AssemblyPath, "exampleTemplates", "eslzArm.test.param.json");
            var templateFile = $"-s {tempateFilePath}";
            //var parametersFile = $"-f {parametersFilePath}";

            var stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            Template.Parser.Cli.Program.Main(new string[] { templateFile, "-a" }).Wait();


            var output = stringWriter.ToString();

            var check = JsonConvert.DeserializeObject<List<dynamic>>(output);

            Assert.AreEqual(15, check.Count);
        }
        [TestMethod]
        public void CanUseParseAssignmentWithVariableName()
        {
            var tempateFilePath = Path.Combine(AssemblyPath, "exampleTemplates", "exampleTemplate06AssignmentNameVariable.json");
            //var parametersFilePath = Path.Combine(AssemblyPath, "exampleTemplates", "eslzArm.test.param.json");
            var templateFile = $"-s {tempateFilePath}";
            var parameter1 = "-p location=uksouth";
            //var parametersFile = $"-f {parametersFilePath}";

            var stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            Template.Parser.Cli.Program.Main(new string[] { templateFile, parameter1, "-a" }).Wait();


            var output = stringWriter.ToString();

            var check = JsonConvert.DeserializeObject<List<dynamic>>(output);

            Assert.AreEqual("Deploy-Private-DNS-Zones", check[0].name.Value);
        }

        [TestMethod]
        public void CanUseTypedParametersWithEmptyArray()
        {
            var tempateFilePath = Path.Combine(AssemblyPath, "exampleTemplates", "exampleTemplate07.json");
            var templateFile = $"-s {tempateFilePath}";
            var parameter1 = "-p listOfResourceTypesDisallowedForDeletion=[[[Array]]]";

            var stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            Template.Parser.Cli.Program.Main(new string[] { templateFile, parameter1 }).Wait();


            var output = stringWriter.ToString();
            Assert.AreEqual(@"{
  ""type"": ""Microsoft.Authorization/policyAssignments"",
  ""apiVersion"": ""2022-06-01"",
  ""name"": ""DenyAction-Resource-Del"",
  ""dependsOn"": [],
  ""properties"": {
    ""description"": ""This policy enables you to specify the resource types that your organization can protect from accidentals deletion by blocking delete calls using deny action effect."",
    ""displayName"": ""Do not allow deletion of resource types"",
    ""policyDefinitionId"": ""/providers/Microsoft.Authorization/policyDefinitions/78460a36-508a-49a4-b2b2-2f5ec564f4bb"",
    ""enforcementMode"": ""Default"",
    ""parameters"": {
      ""effect"": {
        ""value"": ""DenyAction""
      },
      ""listOfResourceTypesDisallowedForDeletion"": {
        ""value"": []
      }
    }
  }
}".Replace("\r\n", "\n"), output.Replace("\r\n", "\n"));
        }
    
        [TestMethod]
        public void CanUseTypedParametersWithArray()
        {
            var tempateFilePath = Path.Combine(AssemblyPath, "exampleTemplates", "exampleTemplate07.json");
            var templateFile = $"-s {tempateFilePath}";
            var parameter1 = "-p listOfResourceTypesDisallowedForDeletion=[[[Array]]]abc,def,ghi";

            var stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            Template.Parser.Cli.Program.Main(new string[] { templateFile, parameter1 }).Wait();


            var output = stringWriter.ToString();
            Assert.AreEqual(@"{
  ""type"": ""Microsoft.Authorization/policyAssignments"",
  ""apiVersion"": ""2022-06-01"",
  ""name"": ""DenyAction-Resource-Del"",
  ""dependsOn"": [],
  ""properties"": {
    ""description"": ""This policy enables you to specify the resource types that your organization can protect from accidentals deletion by blocking delete calls using deny action effect."",
    ""displayName"": ""Do not allow deletion of resource types"",
    ""policyDefinitionId"": ""/providers/Microsoft.Authorization/policyDefinitions/78460a36-508a-49a4-b2b2-2f5ec564f4bb"",
    ""enforcementMode"": ""Default"",
    ""parameters"": {
      ""effect"": {
        ""value"": ""DenyAction""
      },
      ""listOfResourceTypesDisallowedForDeletion"": {
        ""value"": [
          ""abc"",
          ""def"",
          ""ghi""
        ]
      }
    }
  }
}".Replace("\r\n", "\n"), output.Replace("\r\n", "\n"));
        }

        [TestMethod]
        public void CannotUseEmptyStringAsParameterValue()
        {
            var tempateFilePath = Path.Combine(AssemblyPath, "exampleTemplates", "exampleTemplate07.json");
            var templateFile = $"-s {tempateFilePath}";
            var parameter1 = "-p listOfResourceTypesDisallowedForDeletion=";

            var stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            Assert.ThrowsExceptionAsync<ArgumentNullException>(() => Template.Parser.Cli.Program.Main(new string[] { templateFile, parameter1 }));
            Template.Parser.Cli.Program.Main(new string[] { templateFile, parameter1 }).Wait();
        }
    }
}