using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;

using Pulumi;
using CloudFormation = Pulumi.Aws.CloudFormation;

class Program
{
    static Task<int> Main()
    {
        return Deployment.RunAsync( () => {

            // Use a copy of the OpenVidu project's CloudFormation template
            var template = System.IO.File.ReadAllText("CF-OpenVidu-latest.yaml");
        
            // Build a CloudFormation Stack using the template and parameters
            var stack = new CloudFormation.Stack("video", new CloudFormation.StackArgs
            {
                TemplateBody = template,
                Parameters = new Dictionary<string, object?>
                {
                    { "WhichCert", "letsencrypt" },
                    { "LetsEncryptEmail", "admin@synthesys.com.au" },
                    { "MyDomainName", "video.synthesys.com.au" },
                    { "PublicElasticIP", "13.55.83.223" },
                    { "OpenViduSecret", "MY_SECRET" },
                    { "InstanceType", "t2.large" },
                    { "KeyName", "ssh1.synthesys.com.au"},
                    // Experimental settings
                    { "FreeHTTPAccesToRecordingVideos", "true" },
                    { "WantToDeployDemos", "true" },
                    // Template settings required not needed
                    { "OwnCertKEY", "not needed" },
                    { "OwnCertCRT", "not needed" },
                    { "OpenViduWebhookHeaders", "not needed" },
                },
            });

            // Export the outputs from the CloudFormation stack
            return new Dictionary<string, object?>
            {
                { "WebsiteURL", stack.Outputs.Apply(sk => sk["WebsiteURL"]) },
                { "WebsiteURLLE", stack.Outputs.Apply(sk => sk["WebsiteURLLE"]) },
                { "DemosURL", stack.Outputs.Apply(sk => sk["DemosURL"]) },
                { "DemosURLLE", stack.Outputs.Apply(sk => sk["DemosURLLE"]) },
            };
    
        });
    }
}
